using ControleDeMedicamentos.Dominio.ModuloPaciente;
using Dapper;
using System.Data;

namespace ControleDeMedicamentos.Infraestrutura.SqlServer.ModuloPaciente;

public class RepositorioPacienteEmSql
{
    private readonly IDbConnection connection;

    public RepositorioPacienteEmSql(IDbConnection connection)
    {
        this.connection = connection;
    }

    public void CadastrarRegistro(Paciente novoRegistro)
    {
        const string sql = @"
            INSERT INTO [TBPaciente]
                ([Id], [Nome], [Telefone], [CartaoSus], [Cpf])
            VALUES (@Id, @Nome, @Telefone, @CartaoSus, @Cpf);
        ";

        connection.Execute(sql, new
        {
            novoRegistro.Id,
            novoRegistro.Nome,
            novoRegistro.Telefone,
            novoRegistro.CartaoSus,
            novoRegistro.Cpf
        });
    }

    public bool EditarRegistro(Guid idSelecionado, Paciente registroAtualizado)
    {
        const string sql = @"
            UPDATE [TBPaciente]
               SET [Nome]      = @Nome,
                   [Telefone]  = @Telefone,
                   [CartaoSus] = @CartaoSus,
                   [Cpf]       = @Cpf
             WHERE [Id] = @Id;
        ";

        var linhas = connection.Execute(sql, new
        {
            Id = idSelecionado,
            registroAtualizado.Nome,
            registroAtualizado.Telefone,
            registroAtualizado.CartaoSus,
            registroAtualizado.Cpf
        });

        return linhas > 0;
    }

    public bool ExcluirRegistro(Guid idSelecionado)
    {
        const string sql = @"DELETE FROM [TBPaciente] WHERE [Id] = @Id;";

        var linhas = connection.Execute(sql, new { Id = idSelecionado });

        return linhas > 0;
    }

    public List<Paciente> SelecionarRegistros()
    {
        const string sql = @"
            SELECT [Id], [Nome], [Telefone], [CartaoSus], [Cpf]
              FROM [TBPaciente]
            ORDER BY [Nome];
        ";

        return connection.Query<Paciente>(sql).ToList();
    }

    public Paciente? SelecionarRegistroPorId(Guid idSelecionado)
    {
        const string sql = @"
            SELECT [Id], [Nome], [Telefone], [CartaoSus], [Cpf]
              FROM [TBPaciente]
             WHERE [Id] = @Id;
        ";

        return connection.QueryFirstOrDefault<Paciente>(sql, new { Id = idSelecionado });
    }
}
