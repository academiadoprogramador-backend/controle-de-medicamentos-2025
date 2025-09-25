using ControleDeMedicamentos.Dominio.ModuloFornecedor;
using ControleDeMedicamentos.Dominio.ModuloFuncionario;
using ControleDeMedicamentos.Dominio.ModuloMedicamento;
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

    public void CadastrarRequisicaoSaida(RequisicaoSaida requisicaoSaida)
    {
        throw new NotImplementedException();
    }

    public List<RequisicaoSaida> SelecionarRequisicoesSaida()
    {
        return [];
    }
}
