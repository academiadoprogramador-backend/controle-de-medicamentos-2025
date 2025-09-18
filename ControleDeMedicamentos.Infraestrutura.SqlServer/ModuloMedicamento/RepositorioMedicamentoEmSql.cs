using ControleDeMedicamentos.Dominio.ModuloFornecedor;
using ControleDeMedicamentos.Dominio.ModuloMedicamento;
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

        return connection.Query<Medicamento, Fornecedor, Medicamento>(
            sql,
            map: (med, forn) =>
            {
                med.Fornecedor = forn;
                return med;
            },
            param: new { Id = idSelecionado },
            splitOn: "FornecedorId"
        ).FirstOrDefault();
    }
}