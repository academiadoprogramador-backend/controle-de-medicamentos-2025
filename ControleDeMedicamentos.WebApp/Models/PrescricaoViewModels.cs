using ControleDeMedicamentos.Dominio.ModuloPaciente;
using ControleDeMedicamentos.Dominio.ModuloPrescricao;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ControleDeMedicamentos.WebApp.Models;

public class CadastrarPrescricaoViewModel
{
    [Required(ErrorMessage = "O campo 'Descrição' é obrigatório.")]
    public string Descricao { get; set; }

    [Required(ErrorMessage = "O campo 'Data de Validade' é obrigatório.")]
    public DateTime DataValidade { get; set; }

    [Required(ErrorMessage = "O campo 'CRM do Médico' é obrigatório.")]
    [RegularExpression(
        @"^\d{4,7}-?[A-Z]{2}$",
        ErrorMessage = "O campo 'CRM do Médico' deve seguir o padrão 1111000-UF."
    )]
    public string CrmMedico { get; set; }

    [Required(ErrorMessage = "O campo 'Paciente' é obrigatório.")]
    public Guid PacienteId { get; set; }
    public List<SelectListItem> PacientesDisponiveis { get; set; } = new List<SelectListItem>();

    public CadastrarPrescricaoViewModel() { }

    public CadastrarPrescricaoViewModel(List<Paciente> pacientes) : this()
    {
        // Projeção de listas
        PacientesDisponiveis = pacientes
            .Select(p => new SelectListItem(p.Nome, p.Id.ToString()))
            .ToList();

    }
}

public class VisualizarPrescricoesViewModel
{
    public List<DetalhesPrescricaoViewModel> Registros { get; }

    public VisualizarPrescricoesViewModel(List<Prescricao> prescricoes)
    {
        Registros = prescricoes
            .Select(p => new DetalhesPrescricaoViewModel(
                p.Id,
                p.Descricao,
                p.CrmMedico,
                p.Paciente.Nome,
                p.DataEmissao,
                p.DataValidade
            ))
            .ToList();
    }
}

public class DetalhesPrescricaoViewModel
{
    public Guid Id { get; set; }
    public string Descricao { get; set; }
    public string CrmMedico { get; set; }
    public string Paciente { get; set; }
    public DateTime DataEmissao { get; set; }
    public DateTime DataValidade { get; set; }

    public DetalhesPrescricaoViewModel(
        Guid id,
        string descricao,
        string crmMedico,
        string paciente,
        DateTime dataEmissao,
        DateTime dataValidade
    )
    {
        Id = id;
        Descricao = descricao;
        CrmMedico = crmMedico;
        Paciente = paciente;
        DataEmissao = dataEmissao;
        DataValidade = dataValidade;
    }
}