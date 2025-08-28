using ControleDeMedicamentos.Dominio.Compartilhado;
using ControleDeMedicamentos.Dominio.ModuloPaciente;
using System.Text.RegularExpressions;

namespace ControleDeMedicamentos.Dominio.ModuloPrescricao;

public class Prescricao : EntidadeBase<Prescricao>
{
    public string Descricao { get; set; }
    public DateTime DataEmissao { get; set; }
    public DateTime DataValidade { get; set; }
    public string CrmMedico { get; set; }
    public Paciente Paciente { get; set; }

    public Prescricao() { }

    public Prescricao(
        string descricao,
        DateTime dataValidade,
        string crmMedico,
        Paciente paciente
    )
    {
        Id = Guid.NewGuid();
        Descricao = descricao;
        DataEmissao = DateTime.Now;
        DataValidade = dataValidade;
        CrmMedico = crmMedico;
        Paciente = paciente;
    }

    public override void AtualizarRegistro(Prescricao registroAtualizado)
    {
        Descricao = registroAtualizado.Descricao;
        DataValidade = registroAtualizado.DataValidade;
        CrmMedico = registroAtualizado.CrmMedico;
        Paciente = registroAtualizado.Paciente;
    }

    public override string Validar()
    {
        string erros = "";

        if (string.IsNullOrWhiteSpace(Descricao))
            erros += "O campo 'Descrição' é obrigatório.\n";

        if (!Regex.IsMatch(CrmMedico, @"^\d{4,7}-?[A-Z]{2}$"))
            erros += "O campo 'CRM do Médico' deve seguir o padrão 1111000-UF.\n";

        if (DataValidade < DateTime.Now)
            erros += "O campo 'Data de Validade' não pode ser no passado.\n";

        if (Paciente is null)
            erros += "O campo 'Paciente' é obrigatório.\n";

        return erros;
    }
}
