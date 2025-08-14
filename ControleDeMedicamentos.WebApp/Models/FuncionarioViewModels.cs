using ControleDeMedicamentos.Dominio.ModuloFuncionario;
using System.ComponentModel.DataAnnotations;

namespace ControleDeMedicamentos.WebApp.Models;

public class CadastrarFuncionarioViewModel
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

    [Required(ErrorMessage = "O campo 'CPF' é obrigatório.")]
    [RegularExpression(
        @"^\d{3}\.\d{3}\.\d{3}-\d{2}$",
        ErrorMessage = "O campo 'CPF' deve seguir o formato 000.000.000-00."
    )]
    public string Cpf { get; set; }

    public CadastrarFuncionarioViewModel() { }

    public CadastrarFuncionarioViewModel(string nome, string telefone, string cpf) : this()
    {
        Nome = nome;
        Telefone = telefone;
        Cpf = cpf;
    }
}

public class EditarFuncionarioViewModel
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

    [Required(ErrorMessage = "O campo 'CPF' é obrigatório.")]
    [RegularExpression(
        @"^\d{3}\.\d{3}\.\d{3}-\d{2}$",
        ErrorMessage = "O campo 'CPF' deve seguir o formato 000.000.000-00."
    )]
    public string Cpf { get; set; }

    public EditarFuncionarioViewModel() { }

    public EditarFuncionarioViewModel(Guid id, string nome, string telefone, string cpf) : this()
    {
        Id = id;
        Nome = nome;
        Telefone = telefone;
        Cpf = cpf;
    }
}

public class ExcluirFuncionarioViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; }

    public ExcluirFuncionarioViewModel() { }

    public ExcluirFuncionarioViewModel(Guid id, string nome) : this()
    {
        Id = id;
        Nome = nome;
    }
}

public class VisualizarFuncionariosViewModel
{
    public List<DetalhesFuncionarioViewModel> Registros { get; }

    public VisualizarFuncionariosViewModel(List<Funcionario> funcionarios)
    {
        Registros = [];

        foreach (var f in funcionarios)
        {
            var detalhesVM = new DetalhesFuncionarioViewModel(
                f.Id,
                f.Nome,
                f.Telefone,
                f.Cpf
            );

            Registros.Add(detalhesVM);
        }
    }
}

public class DetalhesFuncionarioViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public string Cpf { get; set; }

    public DetalhesFuncionarioViewModel(Guid id, string nome, string telefone, string cpf)
    {
        Id = id;
        Nome = nome;
        Telefone = telefone;
        Cpf = cpf;
    }
}