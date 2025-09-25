using ControleDeMedicamentos.Dominio.ModuloFornecedor;
using ControleDeMedicamentos.Dominio.ModuloFuncionario;
using ControleDeMedicamentos.Dominio.ModuloMedicamento;
using ControleDeMedicamentos.Dominio.ModuloPaciente;
using ControleDeMedicamentos.Dominio.ModuloPrescricao;
using ControleDeMedicamentos.Dominio.ModuloRequisicaoMedicamento;
using Dapper;
using System.Data;

namespace ControleDeMedicamentos.Infraestrutura.SqlServer.ModuloRequisicaoMedicamento;

public class RepositorioRequisicaoMedicamentoEmSql(IDbConnection connection)
{
    public void CadastrarRequisicaoEntrada(RequisicaoEntrada requisicao)
    {
        const string sql = @"
            INSERT INTO [TBRequisicaoEntrada]
                ([Id], [DataOcorrencia], [FuncionarioId], [MedicamentoId], [QuantidadeRequisitada])
            VALUES (@Id, @DataOcorrencia, @FuncionarioId, @MedicamentoId, @QuantidadeRequisitada);
        ";

        connection.Execute(sql, new
        {
            requisicao.Id,
            requisicao.DataOcorrencia,
            FuncionarioId = requisicao.Funcionario.Id,
            MedicamentoId = requisicao.Medicamento.Id,
            requisicao.QuantidadeRequisitada
        });
    }

    public List<RequisicaoEntrada> SelecionarRequisicoesEntrada()
    {
        const string sql = @"
            SELECT re.[Id], re.[DataOcorrencia], re.[FuncionarioId], re.[MedicamentoId], re.[QuantidadeRequisitada],
                   f.[Id] AS FuncionarioId2, f.[Nome], f.[Telefone], f.[Cpf],
                   m.[Id] AS MedicamentoId2, m.[Nome], m.[Descricao], m.[FornecedorId],
                   fo.[Id] AS FornecedorId, fo.[Nome], fo.[Telefone], fo.[Cnpj]
            FROM [TBRequisicaoEntrada] re
            JOIN [TBFuncionario] f ON f.Id = re.FuncionarioId
            JOIN [TBMedicamento] m ON m.Id = re.MedicamentoId
            JOIN [TBFornecedor] fo ON fo.Id = m.FornecedorId;
        ";

        return connection.Query<RequisicaoEntrada, Funcionario, Medicamento, Fornecedor, RequisicaoEntrada>(
            sql,
            (re, func, med, forn) =>
            {
                med.Fornecedor = forn;
                re.Funcionario = func;
                re.Medicamento = med;

                return re;
            },
            splitOn: "FuncionarioId2,MedicamentoId2,FornecedorId"
        ).ToList();
    }

    public void CadastrarRequisicaoSaida(RequisicaoSaida requisicao)
    {
        const string sql = @"
            INSERT INTO [TBRequisicaoSaida]
                ([Id], [DataOcorrencia], [FuncionarioId], [PrescricaoId])
            VALUES (@Id, @DataOcorrencia, @FuncionarioId, @PrescricaoId);
        ";

        connection.Execute(sql, new
        {
            requisicao.Id,
            requisicao.DataOcorrencia,
            FuncionarioId = requisicao.Funcionario.Id,
            PrescricaoId = requisicao.Prescricao.Id
        });
    }

    public List<RequisicaoSaida> SelecionarRequisicoesSaida()
    {
        const string sql = @"
            SELECT 
                -- RequisicaoSaida (antes do 1º split)
                rs.[Id], rs.[DataOcorrencia], rs.[FuncionarioId], rs.[PrescricaoId],

                -- SPLIT 1 (Funcionario)
                f.[Id] AS Split_Func,
                -- bloco Funcionario
                f.[Id] AS Id, f.[Nome], f.[Telefone], f.[Cpf],

                -- SPLIT 2 (Prescricao)
                p.[Id] AS Split_Presc,
                -- bloco Prescricao
                p.[Id] AS Id, p.[Descricao], p.[DataEmissao], p.[DataValidade], p.[CrmMedico],

                -- SPLIT 3 (Paciente)
                pa.[Id] AS Split_Pac,
                -- bloco Paciente
                pa.[Id] AS Id, pa.[Nome], pa.[Telefone], pa.[CartaoSus], pa.[Cpf]
            FROM [TBRequisicaoSaida] rs
            JOIN [TBFuncionario] f ON f.Id = rs.FuncionarioId
            JOIN [TBPrescricao] p ON p.Id = rs.PrescricaoId
            JOIN [TBPaciente] pa ON pa.Id = p.PacienteId;
        ";

        var requisicoesSaida = connection.Query<RequisicaoSaida, Funcionario, Prescricao, Paciente, RequisicaoSaida>(
            sql,
            (rs, func, pres, pac) =>
            {
                pres.Paciente = pac;
                rs.Funcionario = func;
                rs.Prescricao = pres;

                return rs;
            },
            splitOn: "Split_Func,Split_Presc,Split_Pac"
        ).ToList();

        const string sqlMeds = @"
            SELECT
                -- ===== bloco MedicamentoPrescrito (1º tipo) =====
                mp.[Id]            AS Id,
                mp.[Dosagem]       AS Dosagem,
                mp.[Periodo]       AS Periodo,
                mp.[Quantidade]    AS Quantidade,
                mp.[PrescricaoId]  AS PrescricaoId,   -- só mantenha se existir essa prop no modelo
                mp.[MedicamentoId] AS MedicamentoId,  -- idem

                -- ===== SPLIT 1 =====
                m.[Id]             AS Split_MedicamentoId,

                -- ===== bloco Medicamento (2º tipo) =====
                m.[Id]             AS Id,
                m.[Nome]           AS Nome,
                m.[Descricao]      AS Descricao,
                m.[FornecedorId]   AS FornecedorId,

                -- ===== SPLIT 2 =====
                f.[Id]             AS Split_FornecedorId,

                -- ===== bloco Fornecedor (3º tipo) =====
                f.[Id]             AS Id,
                f.[Nome]           AS Nome,
                f.[Telefone]       AS Telefone,
                f.[Cnpj]           AS Cnpj,

                -- ===== SPLIT 3 =====
                p.[Id]             AS Split_PrescricaoId,

                -- ===== bloco Prescricao (4º tipo, mínimo) =====
                p.[Id]             AS Id
            FROM [TBMedicamentoPrescrito] mp
            JOIN [TBMedicamento]  m ON m.[Id]  = mp.[MedicamentoId]
            JOIN [TBFornecedor]   f ON f.[Id]  = m.[FornecedorId]
            JOIN [TBPrescricao]   p ON p.[Id]  = mp.[PrescricaoId]
            WHERE mp.[PrescricaoId] = @PrescricaoId;
            ";

        foreach (var requisicaoSaida in requisicoesSaida)
        {
            var medicamentosPrescritos = connection.Query<
                MedicamentoPrescrito,  // 1º tipo
                Medicamento,           // 2º tipo
                Fornecedor,            // 3º tipo
                Prescricao,            // 4º tipo
                MedicamentoPrescrito   // retorno final
            >(
                sqlMeds,
                (mp, m, f, p) =>
                {
                    m.Fornecedor = f;
                    mp.Medicamento = m;
                    mp.Prescricao = p;
                    return mp;
                },
                new { PrescricaoId = requisicaoSaida.Prescricao.Id },
                splitOn: "Split_MedicamentoId,Split_FornecedorId,Split_PrescricaoId"
            );

            requisicaoSaida.Prescricao.MedicamentosPrescritos = medicamentosPrescritos.ToList();
        }

        return requisicoesSaida;
    }
}
