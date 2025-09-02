using ControleDeMedicamentos.Dominio.ModuloRequisicaoMedicamento;
using ControleDeMedicamentos.Infraestrutura.Arquivos.Compartilhado;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloFuncionario;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloMedicamento;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloRequisicaoMedicamento;
using ControleDeMedicamentos.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControleDeMedicamentos.WebApp.Controllers;

public class RequisicaoMedicamentoController : Controller
{
    private readonly ContextoDados contexto;
    private readonly RepositorioRequisicaoMedicamentoEmArquivo repositorioRequisicaoMedicamento;
    private readonly RepositorioMedicamentoEmArquivo repositorioMedicamento;
    private readonly RepositorioFuncionarioEmArquivo repositorioFuncionario;

    public RequisicaoMedicamentoController(
        ContextoDados contexto,
        RepositorioRequisicaoMedicamentoEmArquivo repositorioRequisicaoMedicamento,
        RepositorioMedicamentoEmArquivo repositorioMedicamento,
        RepositorioFuncionarioEmArquivo repositorioFuncionario
    )
    {
        this.contexto = contexto;
        this.repositorioRequisicaoMedicamento = repositorioRequisicaoMedicamento;
        this.repositorioMedicamento = repositorioMedicamento;
        this.repositorioFuncionario = repositorioFuncionario;
    }

    public IActionResult Index()
    {
        var requisicoesEntrada = repositorioRequisicaoMedicamento.SelecionarRequisicoesEntrada();

        var visualizarVm = new VisualizarEstoqueViewModel(requisicoesEntrada);
        
        return View(visualizarVm);
    }

    [HttpGet]
    public IActionResult CadastrarRequisicaoEntrada()
    {
        var medicamentosDisponiveis = repositorioMedicamento.SelecionarRegistros();
        var funcionariosDisponiveis = repositorioFuncionario.SelecionarRegistros();

        var cadastrarVm = new CadastrarRequisicaoEntradaViewModel(medicamentosDisponiveis, funcionariosDisponiveis);

        return View(cadastrarVm);
    }

    [HttpPost]
    public IActionResult CadastrarRequisicaoEntrada(CadastrarRequisicaoEntradaViewModel cadastrarVm)
    {
        if (!ModelState.IsValid)
        {
            var medicamentosDisponiveis = repositorioMedicamento.SelecionarRegistros();

            cadastrarVm.MedicamentosDisponiveis = medicamentosDisponiveis
                .Select(m => new SelectListItem(m.Nome, m.Id.ToString()))
                .ToList();

            var funcionariosDisponiveis = repositorioFuncionario.SelecionarRegistros();

            cadastrarVm.FuncionariosDisponiveis = funcionariosDisponiveis
                .Select(f => new SelectListItem(f.Nome, f.Id.ToString()))
                .ToList();

            return View(cadastrarVm);
        }

        var medicamentoSelecionado = repositorioMedicamento.SelecionarRegistroPorId(cadastrarVm.MedicamentoId);

        var funcionarioSelecionado = repositorioFuncionario.SelecionarRegistroPorId(cadastrarVm.FuncionarioId);

        var requisicaoEntrada = new RequisicaoEntrada(
            funcionarioSelecionado,
            medicamentoSelecionado,
            cadastrarVm.QuantidadeRequisitada
        );

        medicamentoSelecionado.AdicionarAoEstoque(requisicaoEntrada);

        repositorioRequisicaoMedicamento.CadastrarRequisicaoEntrada(requisicaoEntrada);

        return RedirectToAction(nameof(Index));
    }
}
