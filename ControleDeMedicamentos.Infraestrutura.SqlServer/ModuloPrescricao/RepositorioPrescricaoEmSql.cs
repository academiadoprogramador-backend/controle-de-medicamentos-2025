using ControleDeMedicamentos.Dominio.ModuloFornecedor;
using ControleDeMedicamentos.Dominio.ModuloMedicamento;
using ControleDeMedicamentos.Dominio.ModuloPaciente;
using ControleDeMedicamentos.Dominio.ModuloPrescricao;
using Dapper;
using System.Data;

namespace ControleDeMedicamentos.Infraestrutura.SqlServer.ModuloPrescricao;

public class RepositorioPrescricaoEmSql(IDbConnection connection)
{
    public void CadastrarRegistro(Prescricao nova)
    {
        const string insertPrescricao = @"
            INSERT INTO [TBPrescricao]
                ([Id], [Descricao], [DataEmissao], [DataValidade], [CrmMedico], [PacienteId])
            VALUES
                (@Id, @Descricao, @DataEmissao, @DataValidade, @CrmMedico, @PacienteId);
        ";

        connection.Open();

        var tx = connection.BeginTransaction();

        connection.Execute(insertPrescricao, new
        {
            nova.Id,
            nova.Descricao,
            nova.DataEmissao,
            nova.DataValidade,
            nova.CrmMedico,
            PacienteId = nova.Paciente.Id
        }, tx);

        const string insertMedicamento = @"
            INSERT INTO [TBMedicamentoPrescrito]
                ([Id], [PrescricaoId], [MedicamentoId], [Dosagem], [Periodo], [Quantidade])
            VALUES
                (@Id, @PrescricaoId, @MedicamentoId, @Dosagem, @Periodo, @Quantidade);
        ";

        foreach (var med in nova.MedicamentosPrescritos)
        {
            connection.Execute(insertMedicamento, new
            {
                med.Id,
                PrescricaoId = nova.Id,
                MedicamentoId = med.Medicamento.Id,
                med.Dosagem,
                med.Periodo,
                med.Quantidade
            }, tx);
        }

        tx.Commit();

        connection.Close();
    }

    public void Editar(Guid idPrescricao, Prescricao prescricaoAtualizada)
    {
        const string updatePrescricao = @"
            UPDATE [TBPrescricao]
               SET [Descricao]    = @Descricao,
                   [DataValidade] = @DataValidade,
                   [CrmMedico]    = @CrmMedico,
                   [PacienteId]   = @PacienteId
             WHERE [Id] = @Id;
        ";

        connection.Open();

        var tx = connection.BeginTransaction();

        connection.Execute(updatePrescricao, new
        {
            Id = idPrescricao,
            prescricaoAtualizada.Descricao,
            prescricaoAtualizada.DataValidade,
            prescricaoAtualizada.CrmMedico,
            PacienteId = prescricaoAtualizada.Paciente.Id
        }, tx);

        const string deleteMedicamentos = @"DELETE FROM [TBMedicamentoPrescrito] WHERE [PrescricaoId] = @PrescricaoId;";

        connection.Execute(deleteMedicamentos, new { PrescricaoId = idPrescricao }, tx);

        const string insertMedicamento = @"
            INSERT INTO [TBMedicamentoPrescrito]
                ([Id], [PrescricaoId], [MedicamentoId], [Dosagem], [Periodo], [Quantidade])
            VALUES
                (@Id, @PrescricaoId, @MedicamentoId, @Dosagem, @Periodo, @Quantidade);
        ";

        foreach (var med in prescricaoAtualizada.MedicamentosPrescritos)
        {
            connection.Execute(insertMedicamento, new
            {
                med.Id,
                PrescricaoId = idPrescricao,
                MedicamentoId = med.Medicamento.Id,
                med.Dosagem,
                med.Periodo,
                med.Quantidade
            }, tx);
        }

        tx.Commit();

        connection.Close();
    }

    public List<Prescricao> SelecionarRegistros()
    {
        const string sqlPrescricoes = @"
            SELECT 
                -- Bloco da Prescricao
                p.[Id],
                p.[Descricao],
                p.[DataEmissao],
                p.[DataValidade],
                p.[CrmMedico],

                -- Marcador de split entre Prescricao -> Paciente
                p.[PacienteId] AS PacienteId,

                -- Bloco do Paciente (note o Id com nome 'Id' para mapear Paciente.Id)
                pa.[Id]        AS [Id],
                pa.[Nome],
                pa.[Telefone],
                pa.[CartaoSus],
                pa.[Cpf]
            FROM [TBPrescricao] p
            INNER JOIN [TBPaciente] pa ON pa.[Id] = p.[PacienteId];
        ";

        var prescricoes = connection.Query<Prescricao, Paciente, Prescricao>(
            sqlPrescricoes,
            (p, pa) => { p.Paciente = pa; return p; },
            splitOn: "PacienteId"
        ).ToList();

        const string sqlMeds = @"
            SELECT
                -- bloco MedicamentoPrescrito
                mp.[Id]                AS [Id],
                mp.[Dosagem]           AS [Dosagem],
                mp.[Periodo]           AS [Periodo],
                mp.[Quantidade]        AS [Quantidade],
                mp.[PrescricaoId],     -- usaremos no split e/ou lookup
                mp.[MedicamentoId],    -- idem

                -- marcador + bloco Medicamento
                m.[Id]                 AS [MedicamentoId],   -- split marker
                m.[Id]                 AS [Id],
                m.[Nome]               AS [Nome],
                m.[Descricao]          AS [Descricao],
                m.[FornecedorId]       AS [FornecedorId],

                -- marcador + bloco Fornecedor
                f.[Id]                 AS [FornecedorId],    -- split marker
                f.[Id]                 AS [Id],
                f.[Nome]               AS [Nome],
                f.[Telefone]           AS [Telefone],
                f.[Cnpj]               AS [Cnpj],

                -- marcador + bloco Prescricao (mínimo necessário)
                p.[Id]                 AS [PrescricaoId],    -- split marker
                p.[Id]                 AS [Id]
            FROM [TBMedicamentoPrescrito] mp
            JOIN [TBMedicamento]  m ON m.[Id]  = mp.[MedicamentoId]
            JOIN [TBFornecedor]   f ON f.[Id]  = m.[FornecedorId]
            JOIN [TBPrescricao]   p ON p.[Id]  = mp.[PrescricaoId];
        ";

        var medicamentosPrescritos = connection.Query<
            MedicamentoPrescrito, // 1º bloco: mp.*
            Medicamento,          // 2º bloco: começa no marcador MedicamentoId
            Fornecedor,           // 3º bloco: começa no marcador FornecedorId
            Prescricao,           // 4º bloco: começa no marcador PrescricaoId
            (Guid PrescricaoId, MedicamentoPrescrito MP)
        >(
            sqlMeds,
            (mp, m, f, p) =>
            {
                m.Fornecedor = f;
                mp.Medicamento = m;
                mp.Prescricao = p; // temos a propriedade agora

                return (p.Id, mp); // devolvo também a chave para o lookup
            },
            splitOn: "MedicamentoId,FornecedorId,PrescricaoId"
        );

        var dicionario = medicamentosPrescritos.ToLookup(x => x.PrescricaoId, x => x.MP);

        foreach (var prescricao in prescricoes)
            prescricao.MedicamentosPrescritos = dicionario[prescricao.Id].ToList();

        return prescricoes;
    }

    public Prescricao? SelecionarRegistroPorId(Guid id)
    {
        return SelecionarRegistros().FirstOrDefault(x => x.Id.Equals(id));
    }
}
