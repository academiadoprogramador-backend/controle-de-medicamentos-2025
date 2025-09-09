using ControleDeMedicamentos.Dominio.ModuloFuncionario;
using ControleDeMedicamentos.Dominio.ModuloMedicamento;
using ControleDeMedicamentos.Dominio.ModuloPrescricao;
using ControleDeMedicamentos.Dominio.ModuloRequisicaoMedicamento;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ControleDeMedicamentos.WebApp.Models;

public class CadastrarRequisicaoEntradaViewModel
{
    [Required(ErrorMessage = "O campo 'Medicamento' é obrigatório.")]
    public Guid MedicamentoId { get; set; }
    public List<SelectListItem>? MedicamentosDisponiveis { get; set; }

    [Required(ErrorMessage = "O campo 'Funcionário' é obrigatório.")]
    public Guid FuncionarioId { get; set; }
    public List<SelectListItem>? FuncionariosDisponiveis { get; set; }

    [Required(ErrorMessage = "O campo 'Quantidade Requisitada' é obrigatório.")]
    [Range(0, int.MaxValue, ErrorMessage = "O campo 'Quantidade Requisitada' deve conter um número positivo válido.")]
    public int QuantidadeRequisitada { get; set; }

    public CadastrarRequisicaoEntradaViewModel() { }

    public CadastrarRequisicaoEntradaViewModel(List<Medicamento> medicamentos, List<Funcionario> funcionarios)
    {
        MedicamentosDisponiveis = medicamentos
            .Select(m => new SelectListItem(m.Nome, m.Id.ToString()))
            .ToList();

        FuncionariosDisponiveis = funcionarios
            .Select(f => new SelectListItem(f.Nome, f.Id.ToString()))
            .ToList();
    }
}

public class VisualizarRequisicoesMedicamentoViewModel
{
    public List<DetalhesRequisicaoEntradaViewModel> RequisicoesEntrada { get; set; }
    public List<DetalhesRequisicaoSaidaViewModel> RequisicoesSaida { get; set; }

    public VisualizarRequisicoesMedicamentoViewModel(List<RequisicaoEntrada> requisicoesEntrada, List<RequisicaoSaida> requisicoesSaida)
    {
        RequisicoesEntrada = requisicoesEntrada
            .Select(r => new DetalhesRequisicaoEntradaViewModel(
                r.Id,
                r.DataOcorrencia,
                r.Funcionario.Nome,
                r.Medicamento.Nome,
                r.QuantidadeRequisitada
            ))
            .ToList();

        RequisicoesSaida = requisicoesSaida
            .Select(r => new DetalhesRequisicaoSaidaViewModel(
                r.Id,
                r.DataOcorrencia,
                r.Funcionario.Nome,
                r.Prescricao.Paciente.Nome,
                r.Prescricao.Descricao,
                r.Prescricao.MedicamentosPrescritos
            ))
            .ToList();
    }
}

public class DetalhesRequisicaoEntradaViewModel
{
    public Guid Id { get; set; }
    public DateTime DataOcorrencia { get; set; }
    public string Funcionario { get; set; }
    public string Medicamento { get; set; }
    public int QuantidadeRequisitada { get; set; }

    public DetalhesRequisicaoEntradaViewModel(
        Guid id,
        DateTime dataOcorrencia,
        string funcionario,
        string medicamento,
        int quantidadeRequisitada
    )
    {
        Id = id;
        DataOcorrencia = dataOcorrencia;
        Funcionario = funcionario;
        Medicamento = medicamento;
        QuantidadeRequisitada = quantidadeRequisitada;
    }
}

public class PrimeiraEtapaCadastrarRequisicaoSaidaViewModel
{
    [Required(ErrorMessage = "O campo 'Funcionário' é obrigatório.")]
    public Guid FuncionarioId { get; set; }
    public List<SelectListItem>? FuncionariosDisponiveis { get; set; }

    [Required(ErrorMessage = "O campo 'CPF do Paciente' é obrigatório.")]
    [RegularExpression(
        @"^\d{3}\.\d{3}\.\d{3}-\d{2}$",
        ErrorMessage = "O campo 'CPF do Paciente' deve seguir o formato 000.000.000-00."
    )]
    public string CpfPaciente { get; set; }

    public PrimeiraEtapaCadastrarRequisicaoSaidaViewModel() { }

    public PrimeiraEtapaCadastrarRequisicaoSaidaViewModel(List<Funcionario> funcionarios)
    {
        FuncionariosDisponiveis = funcionarios
            .Select(f => new SelectListItem(f.Nome, f.Id.ToString()))
            .ToList();
    }
}

public class SegundaEtapaCadastrarRequisicaoSaidaViewModel
{
    public Guid FuncionarioId { get; set; }
    public string Funcionario { get; set; }
    public string Paciente { get; set; }
    public List<DetalhesPrescricaoViewModel> PrescricoesDoPaciente { get; set; } = new List<DetalhesPrescricaoViewModel>();

    public SegundaEtapaCadastrarRequisicaoSaidaViewModel() { }

    public SegundaEtapaCadastrarRequisicaoSaidaViewModel(
        Guid funcionarioId,
        string funcionario,
        string paciente,
        List<Prescricao> prescricoesDoPaciente
    )
    {
        FuncionarioId = funcionarioId;
        Funcionario = funcionario;
        Paciente = paciente;

        PrescricoesDoPaciente = prescricoesDoPaciente
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

public class UltimaEtapaCadastrarRequisicaoSaidaViewModel
{
    public Guid FuncionarioId { get; set; }
    public string Funcionario { get; set; }
    public Guid PrescricaoId { get; set; }
    public string Prescricao { get; set; }
    public string Paciente { get; set; }
    public List<DetalhesMedicamentoPrescritoViewModel> MedicamentosPrescritos { get; set; } = new List<DetalhesMedicamentoPrescritoViewModel>();

    public UltimaEtapaCadastrarRequisicaoSaidaViewModel() { }

    public UltimaEtapaCadastrarRequisicaoSaidaViewModel(
        Guid funcionarioId,
        string funcionario,
        Guid prescricaoId,
        string prescricao,
        string paciente,
        List<MedicamentoPrescrito> medicamentosPrescritos
    )
    {
        FuncionarioId = funcionarioId;
        Funcionario = funcionario;

        PrescricaoId = prescricaoId;
        Prescricao = prescricao;

        Paciente = paciente;

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

public class DetalhesRequisicaoSaidaViewModel
{
    public Guid Id { get; set; }
    public DateTime DataOcorrencia { get; set; }
    public string Funcionario { get; set; }
    public string Paciente { get; set; }
    public string Prescricao { get; set; }
    public List<DetalhesMedicamentoPrescritoViewModel> MedicamentosPrescritos { get; set; } = new List<DetalhesMedicamentoPrescritoViewModel>();

    public DetalhesRequisicaoSaidaViewModel(
        Guid id,
        DateTime dataOcorrencia,
        string funcionario,
        string paciente,
        string prescricao,
        List<MedicamentoPrescrito> medicamentosPrescritos
    )
    {
        Id = id;
        DataOcorrencia = dataOcorrencia;
        Funcionario = funcionario;
        Paciente = paciente;
        Prescricao = prescricao;

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