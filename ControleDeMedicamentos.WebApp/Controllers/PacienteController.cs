using ControleDeMedicamentos.Dominio.ModuloPaciente;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloPaciente;
using ControleDeMedicamentos.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeMedicamentos.WebApp.Controllers;

public class PacienteController : Controller
{
    private readonly RepositorioPacienteEmArquivo repositorioPaciente;

    public PacienteController(RepositorioPacienteEmArquivo repositorioPaciente)
    {
        this.repositorioPaciente = repositorioPaciente;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var Pacientes = repositorioPaciente.SelecionarRegistros();

        var visualizarVm = new VisualizarPacientesViewModel(Pacientes);

        return View(visualizarVm);
    }

    [HttpGet]
    public IActionResult Cadastrar()
    {
        var cadastrarVm = new CadastrarPacienteViewModel();

        return View(cadastrarVm);
    }

    [HttpPost]
    public IActionResult Cadastrar(CadastrarPacienteViewModel cadastrarVm)
    {
        if (!ModelState.IsValid)
            return View(cadastrarVm);

        var entidade = new Paciente(
            cadastrarVm.Nome,
            cadastrarVm.Telefone,
            cadastrarVm.CartaoSus,
            cadastrarVm.Cpf
        );

        repositorioPaciente.CadastrarRegistro(entidade);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Editar(Guid id)
    {
        var registro = repositorioPaciente.SelecionarRegistroPorId(id);

        var editarVm = new EditarPacienteViewModel(
            registro.Id,
            registro.Nome,
            registro.Telefone,
            registro.CartaoSus,
            registro.Cpf
        );

        return View(editarVm);
    }

    [HttpPost]
    public IActionResult Editar(EditarPacienteViewModel editarVm)
    {
        if (!ModelState.IsValid)
            return View(editarVm);

        var PacienteEditado = new Paciente(
            editarVm.Nome,
            editarVm.Telefone,
            editarVm.CartaoSus,
            editarVm.Cpf
        );

        repositorioPaciente.EditarRegistro(editarVm.Id, PacienteEditado);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Excluir(Guid id)
    {
        var registro = repositorioPaciente.SelecionarRegistroPorId(id);

        var excluirVm = new ExcluirPacienteViewModel(
            registro.Id,
            registro.Nome
        );

        return View(excluirVm);
    }

    [HttpPost]
    public IActionResult Excluir(ExcluirPacienteViewModel excluirVm)
    {
        repositorioPaciente.ExcluirRegistro(excluirVm.Id);

        return RedirectToAction(nameof(Index));
    }
}
