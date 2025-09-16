using ControleDeMedicamentos.Dominio.ModuloFornecedor;
using Dapper;
using System.Data;

namespace ControleDeMedicamentos.Infraestrutura.SqlServer.ModuloFornecedor;

public class RepositorioFornecedorEmSql(IDbConnection connection)
{
    public void CadastrarRegistro(Fornecedor novoRegistro)
    {
        const string sql = @"
            INSERT INTO [TBFornecedor]
                ([Id], [Nome], [Telefone], [Cnpj])
            VALUES (@Id, @Nome, @Telefone, @Cnpj);
        ";

        connection.Execute(sql, new
        {
            novoRegistro.Id,
            novoRegistro.Nome,
            novoRegistro.Telefone,
            novoRegistro.Cnpj
        });
    }

    public bool EditarRegistro(Guid idSelecionado, Fornecedor registroAtualizado)
    {
        const string sql = @"
            UPDATE [TBFornecedor]
            SET Nome = @Nome,
                Telefone = @Telefone,
                Cnpj = @Cnpj
            WHERE Id = @Id;
        ";

        var linhas = connection.Execute(sql, new
        {
            Id = idSelecionado,
            registroAtualizado.Nome,
            registroAtualizado.Telefone,
            registroAtualizado.Cnpj
        });

        return linhas > 0;
    }

    public bool ExcluirRegistro(Guid idSelecionado)
    {
        const string sql = @"DELETE FROM [TBFornecedor] WHERE Id = @Id;";

        var linhas = connection.Execute(sql, new { Id = idSelecionado });

        return linhas > 0;
    }

    public List<Fornecedor> SelecionarRegistros()
    {
        const string sql = @"
            SELECT
                [Id], [Nome], [Telefone], [Cnpj]
            FROM [TBFornecedor]
            ORDER BY Nome;
        ";

        return connection.Query<Fornecedor>(sql).ToList();
    }

    public Fornecedor? SelecionarRegistroPorId(Guid idSelecionado)
    {
        const string sql = @"
            SELECT
                [Id], [Nome], [Telefone], [Cnpj]
            FROM [TBFornecedor]
            WHERE Id = @Id;
        ";

        return connection.QueryFirstOrDefault<Fornecedor>(sql, new { Id = idSelecionado });
    }
}