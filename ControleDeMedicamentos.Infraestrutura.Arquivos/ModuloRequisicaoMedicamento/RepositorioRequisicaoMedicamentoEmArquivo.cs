using ControleDeMedicamentos.Dominio.ModuloRequisicaoMedicamento;
using ControleDeMedicamentos.Infraestrutura.Arquivos.Compartilhado;

namespace ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloRequisicaoMedicamento;

public class RepositorioRequisicaoMedicamentoEmArquivo
{
    private readonly ContextoDados contexto;
    private readonly List<RequisicaoEntrada> requisicoesEntrada;
    private readonly List<RequisicaoSaida> requisicoesSaida;

    public RepositorioRequisicaoMedicamentoEmArquivo(ContextoDados contexto)
    {
        this.contexto = contexto;

        requisicoesEntrada = contexto.RequisicoesEntrada;
        requisicoesSaida = contexto.RequisicoesSaida;
    }

    public void CadastrarRequisicaoEntrada(RequisicaoEntrada requisicao)
    {
        requisicoesEntrada.Add(requisicao);

        contexto.Salvar();
    }

    public List<RequisicaoEntrada> SelecionarRequisicoesEntrada()
    {
        return requisicoesEntrada;
    }

    public void CadastrarRequisicaoSaida(RequisicaoSaida requisicao)
    {
        requisicoesSaida.Add(requisicao);

        contexto.Salvar();
    }

    public List<RequisicaoSaida> SelecionarRequisicoesSaida()
    {
        return requisicoesSaida;
    }
}