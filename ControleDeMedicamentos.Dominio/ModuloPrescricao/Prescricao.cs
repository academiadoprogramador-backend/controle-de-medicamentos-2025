using ControleDeMedicamentos.Dominio.Compartilhado;
using ControleDeMedicamentos.Dominio.ModuloMedicamento;
using ControleDeMedicamentos.Dominio.ModuloPaciente;
namespace ControleDeMedicamentos.Dominio.ModuloPrescricao;

public class Prescricao : EntidadeBase<Prescricao>
{
    public DateTime DataEmissao { get; set; }
    public string CrmMedico { get; set; }
    public Paciente Paciente { get; set; }
    public List<MedicamentoPrescrito> MedicamentosPrescritos { get; set; } = new List<MedicamentoPrescrito>();

    public Prescricao() { }

    public Prescricao(string crmMedico, Paciente paciente)
    {
        Id = Guid.NewGuid();
        DataEmissao = DateTime.Now;
        CrmMedico = crmMedico;
        Paciente = paciente;
    }

    public MedicamentoPrescrito AdicionarMedicamentoPrescrito(Medicamento medicamento, string dosagem, string periodo, int quantidade)
    {
        var medicamentoPrescrito = new MedicamentoPrescrito(medicamento, dosagem, periodo, quantidade);

        MedicamentosPrescritos.Add(medicamentoPrescrito);

        return medicamentoPrescrito;
    }

    public bool RemoverMedicamentoPrescrito(Guid medicamentoPrecritoId)
    {
        var medicamentoPrescrito = MedicamentosPrescritos.Find(m => m.Id == medicamentoPrecritoId);

        if (medicamentoPrescrito is null)
            return false;

        MedicamentosPrescritos.Remove(medicamentoPrescrito);

        return true;
    }

    public override void AtualizarRegistro(Prescricao registroAtualizado)
    {
        CrmMedico = registroAtualizado.CrmMedico;
        Paciente = registroAtualizado.Paciente;
    }

    public override string Validar()
    {
        string erros = "";

        if (string.IsNullOrWhiteSpace(CrmMedico))
            erros += "O campo 'CRM do Médico' é obrigatório.\n";


        if (Paciente is null)
            erros += "O campo 'Paciente' é obrigatório.\n";

        return erros;
    }
}