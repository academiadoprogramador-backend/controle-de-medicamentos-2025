using ControleDeMedicamentos.Dominio.ModuloFornecedor;
using ControleDeMedicamentos.Dominio.ModuloFuncionario;
using ControleDeMedicamentos.Dominio.ModuloMedicamento;
using ControleDeMedicamentos.Dominio.ModuloPrescricao;
using ControleDeMedicamentos.Dominio.ModuloRequisicaoMedicamento;
using Dapper;
using System.Data;

namespace ControleDeMedicamentos.Infraestrutura.SqlServer.ModuloMedicamento;

public class RepositorioMedicamentoEmSql(IDbConnection connection) 
{
    public void CadastrarRegistro(Medicamento novoRegistro)
    {
        const string sql = @"
            INSERT INTO [TBMedicamento]
                ([Id], [Nome], [Descricao], [FornecedorId])
            VALUES (@Id, @Nome, @Descricao, @FornecedorId);
        ";

        connection.Execute(sql, new
        {
            novoRegistro.Id,
            novoRegistro.Nome,
            novoRegistro.Descricao,
            FornecedorId = novoRegistro.Fornecedor.Id
        });
    }

    public bool EditarRegistro(Guid idSelecionado, Medicamento registroAtualizado)
    {
        const string sql = @"
            UPDATE [TBMedicamento]
               SET [Nome]         = @Nome,
                   [Descricao]    = @Descricao,
                   [FornecedorId] = @FornecedorId
             WHERE [Id] = @Id;
        ";

        var linhasAlteradas = connection.Execute(sql, new
        {
            Id = idSelecionado,
            registroAtualizado.Nome,
            registroAtualizado.Descricao,
            FornecedorId = registroAtualizado.Fornecedor?.Id
        });

        return linhasAlteradas == 1;
    }

    public bool ExcluirRegistro(Guid idSelecionado)
    {
        const string sql = @"DELETE FROM [TBMedicamento] WHERE [Id] = @Id;";

        var linhasAlteradas = connection.Execute(sql, new { Id = idSelecionado });

        return linhasAlteradas == 1;
    }

    public List<Medicamento> SelecionarRegistros()
    {
        const string sql = @"
            SELECT 
                m.[Id], m.[Nome], m.[Descricao],
                f.[Id] AS FornecedorId, f.[Id], f.[Nome], f.[Telefone], f.[Cnpj]
            FROM [TBMedicamento] m
            INNER JOIN [TBFornecedor] f ON f.[Id] = m.[FornecedorId];
        ";

        var medicamentos = connection.Query<Medicamento, Fornecedor, Medicamento>(
            sql,
            map: (med, forn) =>
            {
                med.Fornecedor = forn;
                return med;
            },
            splitOn: "FornecedorId"
        ).ToList();

        CarregarRequisicoes(medicamentos);

        return medicamentos;
    }

    public Medicamento? SelecionarRegistroPorId(Guid idSelecionado)
    {
        const string sql = @"
            SELECT 
                m.[Id], m.[Nome], m.[Descricao],
                f.[Id] AS FornecedorId, f.[Id], f.[Nome], f.[Telefone], f.[Cnpj]
            FROM [TBMedicamento] m
            INNER JOIN [TBFornecedor] f ON f.[Id] = m.[FornecedorId]
            WHERE m.[Id] = @Id;
        ";

        var medicamentoSelecionado =  connection.Query<Medicamento, Fornecedor, Medicamento>(
            sql,
            map: (med, forn) =>
            {
                med.Fornecedor = forn;
                return med;
            },
            param: new { Id = idSelecionado },
            splitOn: "FornecedorId"
        ).FirstOrDefault();

        if (medicamentoSelecionado is not null)
            CarregarRequisicoes([medicamentoSelecionado]);

        return medicamentoSelecionado;
    }

    private void CarregarRequisicoes(List<Medicamento> medicamentos)
    {
        if (medicamentos.Count == 0) return;

        var idsMedicamentos = medicamentos.Select(m => m.Id).ToArray();

        var dictMedicamentos = medicamentos.ToDictionary(m => m.Id, m => m);

        // ========== ENTRADAS ==========
        const string sqlEntradas = @"
            SELECT 
                e.[Id], e.[DataOcorrencia], e.[QuantidadeRequisitada],
                e.[MedicamentoId],
                fun.[Id] AS FuncionarioId, fun.[Nome]
            FROM [TBRequisicaoEntrada] e
            LEFT JOIN [TBFuncionario] fun ON fun.[Id] = e.[FuncionarioId]
            WHERE e.[MedicamentoId] IN @Ids;
        ";

        var entradasSelecionadas = connection.Query(sqlEntradas, new { Ids = idsMedicamentos });

        foreach (var entradaSelecionada in entradasSelecionadas)
        {
            var medId = (Guid)entradaSelecionada.MedicamentoId;

            if (!dictMedicamentos.TryGetValue(medId, out var medicamento)) continue;

            var funcionario = new Funcionario
            {
                Id = (Guid)entradaSelecionada.FuncionarioId,
                Nome = (string)entradaSelecionada.Nome
            };

            var requisicaoEntrada = new RequisicaoEntrada
            {
                Id = (Guid)entradaSelecionada.Id,
                DataOcorrencia = (DateTime)entradaSelecionada.DataOcorrencia,
                Funcionario = funcionario,
                Medicamento = medicamento,
                QuantidadeRequisitada = (int)entradaSelecionada.QuantidadeRequisitada
            };

            medicamento.RequisicoesEntrada.Add(requisicaoEntrada);
        }

        // ========== SAÍDAS ==========
        const string sqlSaidas = @"
            SELECT
                rs.[Id]            AS RequisicaoSaidaId,
                rs.[DataOcorrencia],
                rs.[FuncionarioId],
                fun.[Id]           AS FuncionarioId, fun.[Nome] AS FuncionarioNome,

                p.[Id]             AS PrescricaoId,

                mp.[MedicamentoId] AS ItemMedicamentoId,
                mp.[Quantidade]    AS ItemQuantidade
            FROM [TBRequisicaoSaida] rs

            LEFT JOIN [TBPrescricao] p
                ON p.[Id] = rs.[PrescricaoId]

            INNER JOIN [TBMedicamentoPrescrito] mp
                ON mp.[PrescricaoId] = p.[Id]

            LEFT JOIN [TBFuncionario] fun
                ON fun.[Id] = rs.[FuncionarioId]

            WHERE mp.[MedicamentoId] IN @Ids;
        ";

        var saidasSelecionadas = connection.Query(sqlSaidas, new { Ids = idsMedicamentos });

        var saidasPorMed = new Dictionary<(Guid MedicamentoId, Guid RequisicaoSaidaId), RequisicaoSaida>();

        foreach (var saidaSelecionada in saidasSelecionadas)
        {
            var medId = (Guid)saidaSelecionada.ItemMedicamentoId;

            if (!dictMedicamentos.TryGetValue(medId, out var medicamento)) continue;

            var chave = (medId, (Guid)saidaSelecionada.RequisicaoSaidaId);

            if (!saidasPorMed.TryGetValue(chave, out var reqSaida))
            {
                var funcionario = new Funcionario
                {
                    Id = (Guid)saidaSelecionada.FuncionarioId,
                    Nome = (string)saidaSelecionada.FuncionarioNome
                };

                reqSaida = new RequisicaoSaida
                {
                    Id = (Guid)saidaSelecionada.RequisicaoSaidaId,
                    DataOcorrencia = (DateTime)saidaSelecionada.DataOcorrencia,
                    Funcionario = funcionario,
                    Prescricao = new Prescricao
                    {
                        Id = (Guid)saidaSelecionada.PrescricaoId,
                        MedicamentosPrescritos = new List<MedicamentoPrescrito>()
                    }
                };

                medicamento.RequisicoesSaida.Add(reqSaida);
                saidasPorMed[chave] = reqSaida;
            }

            var medicamentoPrescrito = new MedicamentoPrescrito
            {
                Medicamento = medicamento,
                Quantidade = (int)saidaSelecionada.ItemQuantidade
            };

            reqSaida.Prescricao.MedicamentosPrescritos.Add(medicamentoPrescrito);
        }
    }
}