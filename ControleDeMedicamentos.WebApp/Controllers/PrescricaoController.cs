using ControleDeMedicamentos.Dominio.ModuloPrescricao;
using ControleDeMedicamentos.Infraestrutura.Arquivos.Compartilhado;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloMedicamento;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloPaciente;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloPrescricao;
using ControleDeMedicamentos.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControleDeMedicamentos.WebApp.Controllers;

public class PrescricaoController : Controller
{
    private readonly ContextoDados contexto;
    private readonly RepositorioPrescricaoEmArquivo repositorioPrescricao;
    private readonly RepositorioMedicamentoEmArquivo repositorioMedicamento;
    private readonly RepositorioPacienteEmArquivo repositorioPaciente;

    public PrescricaoController(
        ContextoDados contexto,
        RepositorioPrescricaoEmArquivo repositorioPrescricao,
        RepositorioMedicamentoEmArquivo repositorioMedicamento,
        RepositorioPacienteEmArquivo repositorioPaciente
    )
    {
        this.contexto = contexto;
        this.repositorioPrescricao = repositorioPrescricao;
        this.repositorioMedicamento = repositorioMedicamento;
        this.repositorioPaciente = repositorioPaciente;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var prescricoes = repositorioPrescricao.SelecionarRegistros();

        var visualizarVm = new VisualizarPrescricoesViewModel(prescricoes);

        return View(visualizarVm);
    }

    [HttpGet]
    public IActionResult Cadastrar()
    {
        var pacientesDisponiveis = repositorioPaciente.SelecionarRegistros();

        var cadastrarVm = new CadastrarPrescricaoViewModel(pacientesDisponiveis);

        return View(cadastrarVm);
    }

    [HttpPost]
    public IActionResult Cadastrar(CadastrarPrescricaoViewModel cadastrarVm)
    {
        if (!ModelState.IsValid)
        {
            var pacientesDisponiveis = repositorioPaciente.SelecionarRegistros();

            cadastrarVm.PacientesDisponiveis = pacientesDisponiveis
                .Select(p => new SelectListItem(p.Nome, p.Id.ToString()))
                .ToList();

            return View(cadastrarVm);
        }

        var pacienteSelecionado = repositorioPaciente.SelecionarRegistroPorId(cadastrarVm.PacienteId);

        var entidade = new Prescricao(
            cadastrarVm.Descricao,
            cadastrarVm.DataValidade,
            cadastrarVm.CrmMedico,
            pacienteSelecionado
        );

        repositorioPrescricao.CadastrarRegistro(entidade);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Gerenciar(Guid id)
    {
        var medicamentosDisponiveis = repositorioMedicamento.SelecionarRegistros();

        var prescricaoSelecionada = repositorioPrescricao.SelecionarRegistroPorId(id);

        var gerenciarVm = new GerenciarPrescricaoViewModel(
            id,
            prescricaoSelecionada.Descricao,
            prescricaoSelecionada.CrmMedico,
            prescricaoSelecionada.Paciente.Id,
            prescricaoSelecionada.Paciente.Nome,
            prescricaoSelecionada.MedicamentosPrescritos,
            medicamentosDisponiveis
        );

        return View(gerenciarVm);
    }

    [HttpPost]
    public IActionResult AdicionarMedicamentoPrescrito(Guid idPrescricao, AdicionarMedicamentoPrescritoViewModel adicionarMedicamentoVm)
    {
        var prescricaoSelecionada = repositorioPrescricao.SelecionarRegistroPorId(idPrescricao);

        var medicamentoSelecionado = repositorioMedicamento.SelecionarRegistroPorId(adicionarMedicamentoVm.MedicamentoId);

        prescricaoSelecionada.AdicionarMedicamentoPrescrito(
            medicamentoSelecionado,
            adicionarMedicamentoVm.DosagemMedicamento,
            adicionarMedicamentoVm.PeriodoMedicamento,
            adicionarMedicamentoVm.QuantidadeMedicamento
        );

        contexto.Salvar();

        return RedirectToAction(nameof(Gerenciar), new { id = idPrescricao });
    }

    [HttpPost]
    public IActionResult RemoverMedicamentoPrescrito(Guid idPrescricao, Guid idMedicamentoPrescrito)
    {
        var prescricaoSelecionada = repositorioPrescricao.SelecionarRegistroPorId(idPrescricao);

        prescricaoSelecionada.RemoverMedicamentoPrescrito(idMedicamentoPrescrito);

        contexto.Salvar();

        return RedirectToAction(nameof(Gerenciar), new { id = idPrescricao });
    }
}
