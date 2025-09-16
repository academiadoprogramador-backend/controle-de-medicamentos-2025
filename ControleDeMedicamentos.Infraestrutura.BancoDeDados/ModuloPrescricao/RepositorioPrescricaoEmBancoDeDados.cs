using ControleDeMedicamentos.Dominio.ModuloFornecedor;
using ControleDeMedicamentos.Dominio.ModuloMedicamento;
using ControleDeMedicamentos.Dominio.ModuloPaciente;
using ControleDeMedicamentos.Dominio.ModuloPrescricao;
using Dapper;
using Microsoft.Win32;
using System.Data;

namespace ControleDeMedicamentos.Infraestrutura.BancoDeDados.ModuloPrescricao;

public class RepositorioPrescricaoEmBancoDeDados(IDbConnection connection)
{
    public void CadastrarRegistro(Prescricao nova)
    {
        const string insertPrescricao = @"
            INSERT INTO [TBPrescricao]
                ([Id], [Descricao], [DataEmissao], [DataValidade], [CrmMedico], [PacienteId])
            VALUES
                (@Id, @Descricao, @DataEmissao, @DataValidade, @CrmMedico, @PacienteId);
        ";

        connection.Execute(insertPrescricao, new
        {
            nova.Id,
            nova.Descricao,
            nova.DataEmissao,
            nova.DataValidade,
            nova.CrmMedico,
            PacienteId = nova.Paciente.Id
        });

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
            });
        }
    }

    public bool EditarRegistro(Guid idSelecionado, Prescricao atualizada)
    {
        const string updatePrescricao = @"
            UPDATE [TBPrescricao]
               SET [Descricao]    = @Descricao,
                   [DataValidade] = @DataValidade,
                   [CrmMedico]    = @CrmMedico,
                   [PacienteId]   = @PacienteId
             WHERE [Id] = @Id;
        ";

        var linhas = connection.Execute(updatePrescricao, new
        {
            Id = idSelecionado,
            atualizada.Descricao,
            atualizada.DataValidade,
            atualizada.CrmMedico,
            PacienteId = atualizada.Paciente.Id
        });

        // Limpa medicamentos antigos e insere novamente
        const string deleteMedicamentos = @"DELETE FROM [TBMedicamentoPrescrito] WHERE [PrescricaoId] = @PrescricaoId;";

        connection.Execute(deleteMedicamentos, new { PrescricaoId = idSelecionado });

        const string insertMedicamento = @"
            INSERT INTO [TBMedicamentoPrescrito]
                ([Id], [PrescricaoId], [MedicamentoId], [Dosagem], [Periodo], [Quantidade])
            VALUES
                (@Id, @PrescricaoId, @MedicamentoId, @Dosagem, @Periodo, @Quantidade);
        ";

        foreach (var med in atualizada.MedicamentosPrescritos)
        {
            connection.Execute(insertMedicamento, new
            {
                med.Id,
                PrescricaoId = idSelecionado,
                MedicamentoId = med.Medicamento.Id,
                med.Dosagem,
                med.Periodo,
                med.Quantidade
            });
        }

        return linhas > 0;
    }

    public bool ExcluirRegistro(Guid idSelecionado)
    {
        using var tx = connection.BeginTransaction();

        connection.Execute(
            @"DELETE FROM [TBMedicamentoPrescrito] WHERE [PrescricaoId] = @Id;",
            new { Id = idSelecionado }, tx);

        var linhas = connection.Execute(
            @"DELETE FROM [TBPrescricao] WHERE [Id] = @Id;",
            new { Id = idSelecionado }, tx);

        tx.Commit();

        return linhas > 0;
    }

    public List<Prescricao> SelecionarRegistros()
    {
        // 1) Carrega prescrições com paciente
        const string sqlPrescricoes = @"
            SELECT p.[Id], p.[Descricao], p.[DataEmissao], p.[DataValidade], p.[CrmMedico],
                   pa.[Id] AS PacienteId, pa.[Nome], pa.[Telefone], pa.[CartaoSus], pa.[Cpf]
              FROM [TBPrescricao] p
              JOIN [TBPaciente]  pa ON pa.[Id] = p.[PacienteId];
        ";

        var prescricoes = connection.Query<Prescricao, Paciente, Prescricao>(
            sqlPrescricoes,
            (p, pa) => { p.Paciente = pa; return p; },
            splitOn: "PacienteId"
        ).ToList();

        // 2) Carrega medicamentos prescritos com Medicamento + Fornecedor + Prescricao
        //    Use marcadores claros p/ splitOn: MedicamentoId, FornecedorId, PrescricaoId
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

        // 3) Distribui os medicamentos nas respectivas prescrições
        var lookup = medicamentosPrescritos.ToLookup(x => x.PrescricaoId, x => x.MP);

        foreach (var p in prescricoes)
            p.MedicamentosPrescritos = lookup[p.Id].ToList();

        return prescricoes;
    }

    public Prescricao? SelecionarRegistroPorId(Guid idSelecionado)
    {
        return SelecionarRegistros().FirstOrDefault(p => p.Id == idSelecionado);
    }

    public List<Prescricao> SelecionarPrescricoesDoPaciente(Guid idPaciente)
    {
        return SelecionarRegistros().FindAll(p => p.Paciente.Id == idPaciente);
    }
}
