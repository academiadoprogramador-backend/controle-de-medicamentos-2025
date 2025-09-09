using ControleDeMedicamentos.Dominio.ModuloFuncionario;
using ControleDeMedicamentos.Dominio.ModuloPrescricao;

namespace ControleDeMedicamentos.Dominio.ModuloRequisicaoMedicamento;

public class RequisicaoSaida
{
    public Guid Id { get; set; }
    public DateTime DataOcorrencia { get; set; }
    public Funcionario Funcionario { get; set; }
    public Prescricao Prescricao { get; set; }

    public RequisicaoSaida() { }

    public RequisicaoSaida(
        Funcionario funcionario,
        Prescricao prescricao
    )
    {
        Id = Guid.NewGuid();
        DataOcorrencia = DateTime.Now;
        Funcionario = funcionario;
        Prescricao = prescricao;
    }

    public string Validar()
    {
        string erros = string.Empty;

        if (Funcionario == null)
            erros += "O campo \"Paciente\" é obrigatório.";

        if (Prescricao == null)
            erros += "O campo \"Medicamento\" é obrigatório.";

        else if (Prescricao.MedicamentosPrescritos.Count < 1)
            erros += "O campo \"Medicamentos Prescritos da Prescrição\" necessita conter ao menos um medicamento.";

        foreach (var item in Prescricao.MedicamentosPrescritos)
        {
            if (item.Quantidade > item.Medicamento.QuantidadeEmEstoque)
                erros += $"O medicamento \"{item.Medicamento.Nome}\" não está disponível na quantidade requisitada.";
        }

        return erros;
    }
}