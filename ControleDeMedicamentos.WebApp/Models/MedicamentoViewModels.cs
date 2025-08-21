using ControleDeMedicamentos.Dominio.ModuloFornecedor;
using ControleDeMedicamentos.Dominio.ModuloMedicamento;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ControleDeMedicamentos.WebApp.Models;

public class CadastrarMedicamentoViewModel
{
    [Required(ErrorMessage = "O campo 'Nome' é obrigatório.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "O campo 'Nome' deve conter entre 2 e 50 caracteres.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O campo 'Descrição' é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo 'Descrição' deve conter entre 2 e 100 caracteres.")]
    public string Descricao { get; set; }

    [Required(ErrorMessage = "O campo 'Fornecedor' é obrigatório.")]
    public Guid FornecedorId { get; set; }
    public List<SelectListItem> FornecedoresDisponiveis { get; set; } = new List<SelectListItem>();

    public CadastrarMedicamentoViewModel() { }

    public CadastrarMedicamentoViewModel(List<Fornecedor> fornecedoresDisponiveis)
    {
        foreach (var f in fornecedoresDisponiveis)
        {
            var selecionarVm = new SelectListItem(f.Nome, f.Id.ToString());

            FornecedoresDisponiveis.Add(selecionarVm);
        }
    }
}

public class EditarMedicamentoViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O campo 'Nome' é obrigatório.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "O campo 'Nome' deve conter entre 2 e 50 caracteres.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O campo 'Descrição' é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo 'Descrição' deve conter entre 2 e 100 caracteres.")]
    public string Descricao { get; set; }

    [Required(ErrorMessage = "O campo 'Fornecedor' é obrigatório.")]
    public Guid FornecedorId { get; set; }
    public List<SelectListItem> FornecedoresDisponiveis { get; set; } = new List<SelectListItem>();

    public EditarMedicamentoViewModel() { }

    public EditarMedicamentoViewModel(
        Guid id, 
        string nome,
        string descricao,
        Guid fornecedorId, 
        List<Fornecedor> fornecedoresDisponiveis
    )
    {
        Id = id;
        Nome = nome;
        Descricao = descricao;
        FornecedorId = fornecedorId;

        foreach (var f in fornecedoresDisponiveis)
        {
            var selecionarVm = new SelectListItem(f.Nome, f.Id.ToString());

            FornecedoresDisponiveis.Add(selecionarVm);
        }
    }
}

public class ExcluirMedicamentoViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; }

    public ExcluirMedicamentoViewModel() { }

    public ExcluirMedicamentoViewModel(Guid id, string nome)
    {
        Id = id;
        Nome = nome;
    }
}

public class VisualizarMedicamentosViewModel
{
    public List<DetalhesMedicamentoViewModel> Registros { get; } = new List<DetalhesMedicamentoViewModel>();

    public VisualizarMedicamentosViewModel(List<Medicamento> medicamentos)
    {
        foreach (var m in medicamentos)
        {
            var detalhesVm = new DetalhesMedicamentoViewModel(
                m.Id,
                m.Nome,
                m.Descricao,
                m.Fornecedor.Nome,
                m.QuantidadeEmEstoque,
                m.EmFalta
            );

            Registros.Add(detalhesVm);
        }
    }
}

public class DetalhesMedicamentoViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public string Fornecedor { get; set; }
    public int QuantidadeEmEstoque { get; set; }
    public bool EmFalta { get; set; }

    public DetalhesMedicamentoViewModel(Guid id, string nome, string descricao, string fornecedor, int quantidade, bool emFalta)
    {
        Id = id;
        Nome = nome;
        Descricao = descricao;
        Fornecedor = fornecedor;
        QuantidadeEmEstoque = quantidade;
        EmFalta = emFalta;
    }
}