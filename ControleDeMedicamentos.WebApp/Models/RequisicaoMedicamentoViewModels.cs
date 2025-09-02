using ControleDeMedicamentos.Dominio.ModuloFuncionario;
using ControleDeMedicamentos.Dominio.ModuloMedicamento;
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

    public VisualizarRequisicoesMedicamentoViewModel(List<RequisicaoEntrada> requisicoesEntrada)
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