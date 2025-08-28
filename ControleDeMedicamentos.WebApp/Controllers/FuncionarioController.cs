using ControleDeMedicamentos.Dominio.ModuloFuncionario;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloFuncionario;
using ControleDeMedicamentos.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeMedicamentos.WebApp.Controllers;

public class FuncionarioController : Controller
{
    private readonly RepositorioFuncionarioEmArquivo repositorioFuncionario;

    public FuncionarioController(RepositorioFuncionarioEmArquivo repositorioFuncionario)
    {
        this.repositorioFuncionario = repositorioFuncionario;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var funcionarios = repositorioFuncionario.SelecionarRegistros();

        var visualizarVm = new VisualizarFuncionariosViewModel(funcionarios);

        return View(visualizarVm);
    }

    [HttpGet]
    public IActionResult Cadastrar()
    {
        var cadastrarVm = new CadastrarFuncionarioViewModel();

        return View(cadastrarVm);
    }

    [HttpPost]
    public IActionResult Cadastrar(CadastrarFuncionarioViewModel cadastrarVm)
    {
        if (!ModelState.IsValid)
            return View(cadastrarVm);

        var entidade = new Funcionario(
            cadastrarVm.Nome,
            cadastrarVm.Telefone,
            cadastrarVm.Cpf
        );

        repositorioFuncionario.CadastrarRegistro(entidade);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Editar(Guid id)
    {
        var registro = repositorioFuncionario.SelecionarRegistroPorId(id);

        var editarVm = new EditarFuncionarioViewModel(
            registro.Id,
            registro.Nome,
            registro.Telefone,
            registro.Cpf
        );

        return View(editarVm);
    }

    [HttpPost]
    public IActionResult Editar(EditarFuncionarioViewModel editarVm)
    {
        if (!ModelState.IsValid)
            return View(editarVm);

        var funcionarioEditado = new Funcionario(
            editarVm.Nome,
            editarVm.Telefone,
            editarVm.Cpf
        );

        repositorioFuncionario.EditarRegistro(editarVm.Id, funcionarioEditado);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Excluir(Guid id)
    {
        var registro = repositorioFuncionario.SelecionarRegistroPorId(id);

        var excluirVm = new ExcluirFuncionarioViewModel(
            registro.Id,
            registro.Nome
        );

        return View(excluirVm);
    }

    [HttpPost]
    public IActionResult Excluir(ExcluirFuncionarioViewModel excluirVm)
    {
        repositorioFuncionario.ExcluirRegistro(excluirVm.Id);

        return RedirectToAction(nameof(Index));
    }
}
