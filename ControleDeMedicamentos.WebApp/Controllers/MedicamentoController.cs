using ControleDeMedicamentos.Dominio.ModuloMedicamento;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloFornecedor;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloMedicamento;
using ControleDeMedicamentos.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeMedicamentos.WebApp.Controllers;

public class MedicamentoController : Controller
{
    private readonly RepositorioMedicamentoEmArquivo repositorioMedicamento;
    private readonly RepositorioFornecedorEmArquivo repositorioFornecedor;

    public MedicamentoController(
        RepositorioMedicamentoEmArquivo repositorioMedicamento,
        RepositorioFornecedorEmArquivo repositorioFornecedor
    )
    {
        this.repositorioMedicamento = repositorioMedicamento;
        this.repositorioFornecedor = repositorioFornecedor;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var medicamentos = repositorioMedicamento.SelecionarRegistros();

        var visualizarVm = new VisualizarMedicamentosViewModel(medicamentos);

        return View(visualizarVm);
    }

    [HttpGet]
    public IActionResult Cadastrar()
    {
        var fornecedoresDisponiveis = repositorioFornecedor.SelecionarRegistros();

        var cadastrarVm = new CadastrarMedicamentoViewModel(fornecedoresDisponiveis);

        return View(cadastrarVm);
    }

    [HttpPost]
    public IActionResult Cadastrar(CadastrarMedicamentoViewModel cadastrarVm)
    {
        if (!ModelState.IsValid)
            return View(cadastrarVm);

        var fornecedorSelecionado = repositorioFornecedor.SelecionarRegistroPorId(cadastrarVm.FornecedorId);

        var entidade = new Medicamento(
            cadastrarVm.Nome,
            cadastrarVm.Descricao,
            fornecedorSelecionado
        );

        repositorioMedicamento.CadastrarRegistro(entidade);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Editar(Guid id)
    {
        var registro = repositorioMedicamento.SelecionarRegistroPorId(id);

        var fornecedoresDisponiveis = repositorioFornecedor.SelecionarRegistros();

        var editarVm = new EditarMedicamentoViewModel(
            registro.Id,
            registro.Nome,
            registro.Descricao,
            registro.Fornecedor.Id,
            fornecedoresDisponiveis
        );

        return View(editarVm);
    }

    [HttpPost]
    public IActionResult Editar(EditarMedicamentoViewModel editarVm)
    {
        if (!ModelState.IsValid)
            return View(editarVm);

        var fornecedorSelecionado = repositorioFornecedor.SelecionarRegistroPorId(editarVm.FornecedorId);

        var MedicamentoEditado = new Medicamento(
            editarVm.Nome,
            editarVm.Descricao,
            fornecedorSelecionado
        );

        repositorioMedicamento.EditarRegistro(editarVm.Id, MedicamentoEditado);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Excluir(Guid id)
    {
        var registro = repositorioMedicamento.SelecionarRegistroPorId(id);

        var excluirVm = new ExcluirMedicamentoViewModel(
            registro.Id,
            registro.Nome
        );

        return View(excluirVm);
    }

    [HttpPost]
    public IActionResult Excluir(ExcluirMedicamentoViewModel excluirVm)
    {
        repositorioMedicamento.ExcluirRegistro(excluirVm.Id);

        return RedirectToAction(nameof(Index));
    }
}
