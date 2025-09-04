using ControleDeMedicamentos.Dominio.ModuloMedicamento;
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
    public List<SelectListItem>? PacientesDisponiveis { get; set; } = new List<SelectListItem>();

    public CadastrarPrescricaoViewModel() { }

    public CadastrarPrescricaoViewModel(List<Paciente> pacientes) : this()
    {
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
                p.DataValidade,
                p.MedicamentosPrescritos
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

    public List<DetalhesMedicamentoPrescritoViewModel> MedicamentosPrescritos { get; set; } = new List<DetalhesMedicamentoPrescritoViewModel>();

    public DetalhesPrescricaoViewModel(
        Guid id,
        string descricao,
        string crmMedico,
        string paciente,
        DateTime dataEmissao,
        DateTime dataValidade,
        List<MedicamentoPrescrito> medicamentosPrescritos
    )
    {
        Id = id;
        Descricao = descricao;
        CrmMedico = crmMedico;
        Paciente = paciente;
        DataEmissao = dataEmissao;
        DataValidade = dataValidade;

        MedicamentosPrescritos = medicamentosPrescritos
          .Select(m => new DetalhesMedicamentoPrescritoViewModel(
              m.Id,
              m.Medicamento.Id,
              m.Medicamento.Nome,
              m.Dosagem,
              m.Periodo,
              m.Quantidade))
          .ToList();
    }
}

public class AdicionarMedicamentoPrescritoViewModel
{
    public Guid MedicamentoId { get; set; }
    public string DosagemMedicamento { get; set; }
    public string PeriodoMedicamento { get; set; }
    public int QuantidadeMedicamento { get; set; }
}

public class DetalhesMedicamentoPrescritoViewModel
{
    public Guid Id { get; set; }
    public Guid MedicamentoId { get; set; }
    public string Medicamento { get; set; }
    public string Dosagem { get; set; }
    public string Periodo { get; set; }
    public int Quantidade { get; set; }

    public DetalhesMedicamentoPrescritoViewModel() { }

    public DetalhesMedicamentoPrescritoViewModel(
        Guid id,
        Guid medicamentoId,
        string nomeMedicamento,
        string dosagem,
        string periodo,
        int quantidade
    ) : this()
    {
        Id = id;
        MedicamentoId = medicamentoId;
        Medicamento = nomeMedicamento;
        Dosagem = dosagem;
        Periodo = periodo;
        Quantidade = quantidade;
    }
}

public class GerenciarPrescricaoViewModel
{
    public Guid Id { get; set; }
    public string Descricao { get; set; }
    public string CrmMedico { get; set; }
    public Guid PacienteId { get; set; }
    public string Paciente { get; set; }

    public List<SelectListItem> MedicamentosDisponiveis { get; set; }
    public List<DetalhesMedicamentoPrescritoViewModel> MedicamentosPrescritos { get; set; } = new List<DetalhesMedicamentoPrescritoViewModel>();

    public GerenciarPrescricaoViewModel() { }

    public GerenciarPrescricaoViewModel(
        Guid id,
        string descricao,
        string crmMedico,
        Guid pacienteId,
        string paciente,
        List<Medicamento> medicamentos
    ) : this()
    {
        Id = id;
        Descricao = descricao;
        CrmMedico = crmMedico;
        PacienteId = pacienteId;
        Paciente = paciente;

        MedicamentosDisponiveis = medicamentos
            .Select(m => new SelectListItem(m.Nome, m.Id.ToString()))
            .ToList();
    }

    public GerenciarPrescricaoViewModel(
        Guid id,
        string descricao,
        string crmMedico,
        Guid pacienteId,
        string paciente,
        List<MedicamentoPrescrito> medicamentosPrescritos,
        List<Medicamento> medicamentos
    ) : this(id, descricao, crmMedico, pacienteId, paciente, medicamentos)
    {
        MedicamentosPrescritos = medicamentosPrescritos
            .Select(m => new DetalhesMedicamentoPrescritoViewModel(
                m.Id,
                m.Medicamento.Id,
                m.Medicamento.Nome,
                m.Dosagem,
                m.Periodo,
                m.Quantidade))
            .ToList();
    }
}