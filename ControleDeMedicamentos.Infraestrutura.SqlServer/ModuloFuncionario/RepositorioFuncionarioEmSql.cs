using ControleDeMedicamentos.Dominio.ModuloFuncionario;
using Dapper;
using System.Data;

namespace ControleDeMedicamentos.Infraestrutura.SqlServer.ModuloFuncionario;

public class RepositorioFuncionarioEmSql(IDbConnection connection)
{
    public void CadastrarRegistro(Funcionario novoRegistro)
    {
        const string sql = @"
            INSERT INTO [TBFuncionario]
                ([Id], [Nome], [Telefone], [Cpf])
            VALUES (@Id, @Nome, @Telefone, @Cpf);
        ";

        connection.Execute(sql, new
        {
            novoRegistro.Id,
            novoRegistro.Nome,
            novoRegistro.Telefone,
            novoRegistro.Cpf
        });
    }

    public bool EditarRegistro(Guid idSelecionado, Funcionario registroAtualizado)
    {
        const string sql = @"
            UPDATE [TBFuncionario]
            SET [Nome] = @Nome,
                [Telefone] = @Telefone,
                [Cpf] = @Cpf
             WHERE [Id] = @Id;
        ";

        var linhas = connection.Execute(sql, new
        {
            Id = idSelecionado,
            registroAtualizado.Nome,
            registroAtualizado.Telefone,
            registroAtualizado.Cpf
        });

        return linhas > 0;
    }

    public bool ExcluirRegistro(Guid idSelecionado)
    {
        const string sql = @"DELETE FROM [TBFuncionario] WHERE [Id] = @Id;";

        var linhas = connection.Execute(sql, new { Id = idSelecionado });

        return linhas > 0;
    }

    public List<Funcionario> SelecionarRegistros()
    {
        const string sql = @"
            SELECT [Id], [Nome], [Telefone], [Cpf]
            FROM [TBFuncionario]
            ORDER BY [Nome];
        ";

        return connection.Query<Funcionario>(sql).ToList();
    }

    public Funcionario? SelecionarRegistroPorId(Guid idSelecionado)
    {
        const string sql = @"
            SELECT [Id], [Nome], [Telefone], [Cpf]
            FROM [TBFuncionario]
            WHERE [Id] = @Id;
        ";

        return connection.QueryFirstOrDefault<Funcionario>(sql, new { Id = idSelecionado });
    }
}