using ControleDeMedicamentos.Dominio.ModuloFornecedor;
using ControleDeMedicamentos.Dominio.ModuloMedicamento;
using Dapper;
using System.Data;

namespace ControleDeMedicamentos.Infraestrutura.BancoDeDados.ModuloMedicamento;

public class RepositorioMedicamentoEmBancoDeDados(IDbConnection connection) : IRepositorioMedicamento
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
            FornecedorId = novoRegistro.Fornecedor?.Id
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

        var linhas = connection.Execute(sql, new
        {
            Id = idSelecionado,
            registroAtualizado.Nome,
            registroAtualizado.Descricao,
            FornecedorId = registroAtualizado.Fornecedor?.Id
        });

        return linhas > 0;
    }

    public bool ExcluirRegistro(Guid idSelecionado)
    {
        const string sql = @"DELETE FROM [TBMedicamento] WHERE [Id] = @Id;";

        var linhas = connection.Execute(sql, new { Id = idSelecionado });

        return linhas > 0;
    }

    public List<Medicamento> SelecionarRegistros()
    {
        // Multi-mapping para popular a propriedade Fornecedor
        const string sql = @"
            SELECT 
                m.[Id], m.[Nome], m.[Descricao],
                f.[Id] AS FornecedorId, f.[Nome], f.[Telefone], f.[Cnpj]
            FROM [TBMedicamento] m
            INNER JOIN [TBFornecedor] f ON f.[Id] = m.[FornecedorId]
            ORDER BY m.[Nome];
        ";

        var lista = connection.Query<Medicamento, Fornecedor, Medicamento>(
            sql,
            map: (med, forn) =>
            {
                med.Fornecedor = forn;
                return med;
            },
            splitOn: "FornecedorId"
        ).ToList();

        return lista;
    }

    public Medicamento? SelecionarRegistroPorId(Guid idSelecionado)
    {
        const string sql = @"
            SELECT 
                m.[Id], m.[Nome], m.[Descricao],
                f.[Id] AS FornecedorId, f.[Nome], f.[Telefone], f.[Cnpj]
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