using ControleDeMedicamentos.Dominio.ModuloRequisicaoMedicamento;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloFuncionario;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloMedicamento;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloPaciente;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloPrescricao;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloRequisicaoMedicamento;
using ControleDeMedicamentos.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControleDeMedicamentos.WebApp.Controllers;

public class RequisicaoMedicamentoController : Controller
{
    private readonly RepositorioRequisicaoMedicamentoEmArquivo repositorioRequisicaoMedicamento;
    private readonly RepositorioMedicamentoEmArquivo repositorioMedicamento;
    private readonly RepositorioFuncionarioEmArquivo repositorioFuncionario;
    private readonly RepositorioPacienteEmArquivo repositorioPaciente;
    private readonly RepositorioPrescricaoEmArquivo repositorioPrescricao;

    public RequisicaoMedicamentoController(
        RepositorioRequisicaoMedicamentoEmArquivo repositorioRequisicaoMedicamento,
        RepositorioMedicamentoEmArquivo repositorioMedicamento,
        RepositorioFuncionarioEmArquivo repositorioFuncionario,
        RepositorioPacienteEmArquivo repositorioPaciente,
        RepositorioPrescricaoEmArquivo repositorioPrescricao
    )
    {
        this.repositorioRequisicaoMedicamento = repositorioRequisicaoMedicamento;
        this.repositorioMedicamento = repositorioMedicamento;
        this.repositorioFuncionario = repositorioFuncionario;
        this.repositorioPaciente = repositorioPaciente;
        this.repositorioPrescricao = repositorioPrescricao;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var requisicoesEntrada = repositorioRequisicaoMedicamento.SelecionarRequisicoesEntrada();

        var visualizarVm = new VisualizarRequisicoesMedicamentoViewModel(requisicoesEntrada, []);

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

        var funcionarioSelecionado = repositorioFuncionario.SelecionarRegistroPorId(cadastrarVm.FuncionarioId);
        var medicamentoSelecionado = repositorioMedicamento.SelecionarRegistroPorId(cadastrarVm.MedicamentoId);

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
