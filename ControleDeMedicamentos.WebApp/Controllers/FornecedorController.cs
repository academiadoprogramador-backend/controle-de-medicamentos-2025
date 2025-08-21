using ControleDeMedicamentos.Dominio.ModuloFornecedor;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloFornecedor;
using ControleDeMedicamentos.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeMedicamentos.WebApp.Controllers;

public class FornecedorController : Controller
{
    private readonly RepositorioFornecedorEmArquivo repositorioFornecedor;

    public FornecedorController(RepositorioFornecedorEmArquivo repositorioFornecedor)
    {
        this.repositorioFornecedor = repositorioFornecedor;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var Fornecedors = repositorioFornecedor.SelecionarRegistros();

        var visualizarVm = new VisualizarFornecedoresViewModel(Fornecedors);

        return View(visualizarVm);
    }

    [HttpGet]
    public IActionResult Cadastrar()
    {
        var cadastrarVm = new CadastrarFornecedorViewModel();

        return View(cadastrarVm);
    }

    [HttpPost]
    public IActionResult Cadastrar(CadastrarFornecedorViewModel cadastrarVm)
    {
        if (!ModelState.IsValid)
            return View(cadastrarVm);

        var entidade = new Fornecedor(
            cadastrarVm.Nome,
            cadastrarVm.Telefone,
            cadastrarVm.Cnpj
        );

        repositorioFornecedor.CadastrarRegistro(entidade);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Editar(Guid id)
    {
        var registro = repositorioFornecedor.SelecionarRegistroPorId(id);

        var editarVm = new EditarFornecedorViewModel(
            registro.Id,
            registro.Nome,
            registro.Telefone,
            registro.Cnpj
        );

        return View(editarVm);
    }

    [HttpPost]
    public IActionResult Editar(EditarFornecedorViewModel editarVm)
    {
        if (!ModelState.IsValid)
            return View(editarVm);

        var fornecedorEditado = new Fornecedor(
            editarVm.Nome,
            editarVm.Telefone,
            editarVm.Cnpj
        );

        repositorioFornecedor.EditarRegistro(editarVm.Id, fornecedorEditado);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Excluir(Guid id)
    {
        var registro = repositorioFornecedor.SelecionarRegistroPorId(id);

        var excluirVm = new ExcluirFornecedorViewModel(
            registro.Id,
            registro.Nome
        );

        return View(excluirVm);
    }

    [HttpPost]
    public IActionResult Excluir(ExcluirFornecedorViewModel excluirVm)
    {
        repositorioFornecedor.ExcluirRegistro(excluirVm.Id);

        return RedirectToAction(nameof(Index));
    }
}
