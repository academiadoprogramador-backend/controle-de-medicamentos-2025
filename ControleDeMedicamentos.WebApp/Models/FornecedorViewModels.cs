using ControleDeMedicamentos.Dominio.ModuloFornecedor;
using System.ComponentModel.DataAnnotations;

namespace ControleDeMedicamentos.WebApp.Models;

public class CadastrarFornecedorViewModel
{
    [Required(ErrorMessage = "O campo 'Nome' é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo 'Nome' deve conter entre 2 e 100 caracteres.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O campo 'Telefone' é obrigatório.")]
    [RegularExpression(
        @"^\(?\d{2}\)?\s?(9\d{4}|\d{4})-?\d{4}$",
        ErrorMessage = "O campo 'Telefone' deve seguir o padrão (DDD) 0000-0000 ou (DDD) 00000-0000."
    )]
    public string Telefone { get; set; }

    [Required(ErrorMessage = "O campo 'CNPJ' é obrigatório.")]
    [RegularExpression(
        @"^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$",
        ErrorMessage = "O campo 'CNPJ' deve seguir o formato 00.000.000/0000-00."
    )]
    public string Cnpj { get; set; }

    public CadastrarFornecedorViewModel() { }

    public CadastrarFornecedorViewModel(string nome, string telefone, string cnpj) : this()
    {
        Nome = nome;
        Telefone = telefone;
        Cnpj = cnpj;
    }
}

public class EditarFornecedorViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O campo 'Nome' é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo 'Nome' deve conter entre 2 e 100 caracteres.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O campo 'Telefone' é obrigatório.")]
    [RegularExpression(
        @"^\(?\d{2}\)?\s?(9\d{4}|\d{4})-?\d{4}$",
        ErrorMessage = "O campo 'Telefone' deve seguir o padrão (DDD) 0000-0000 ou (DDD) 00000-0000."
    )]
    public string Telefone { get; set; }

    [Required(ErrorMessage = "O campo 'CNPJ' é obrigatório.")]
    [RegularExpression(
        @"^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$",
        ErrorMessage = "O campo 'CNPJ' deve seguir o formato 00.000.000/0000-00."
    )]
    public string Cnpj { get; set; }

    public EditarFornecedorViewModel() { }

    public EditarFornecedorViewModel(Guid id, string nome, string telefone, string cnpj) : this()
    {
        Id = id;
        Nome = nome;
        Telefone = telefone;
        Cnpj = cnpj;
    }
}

public class ExcluirFornecedorViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; }

    public ExcluirFornecedorViewModel() { }

    public ExcluirFornecedorViewModel(Guid id, string nome) : this()
    {
        Id = id;
        Nome = nome;
    }
}

public class VisualizarFornecedoresViewModel
{
    public List<DetalhesFornecedorViewModel> Registros { get; }

    public VisualizarFornecedoresViewModel(List<Fornecedor> fornecedores)
    {
        Registros = new List<DetalhesFornecedorViewModel>();

        foreach (var f in fornecedores)
        {
            var detalhesVM = new DetalhesFornecedorViewModel(
                f.Id,
                f.Nome,
                f.Telefone,
                f.Cnpj
            );

            Registros.Add(detalhesVM);
        }
    }
}

public class DetalhesFornecedorViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public string Cnpj { get; set; }

    public DetalhesFornecedorViewModel(Guid id, string nome, string telefone, string cnpj)
    {
        Id = id;
        Nome = nome;
        Telefone = telefone;
        Cnpj = cnpj;
    }
}
