using ControleDeMedicamentos.Dominio.ModuloMedicamento;
using System.Diagnostics.CodeAnalysis;

namespace ControleDeMedicamentos.Dominio.ModuloPrescricao;

public class MedicamentoPrescrito
{
    public Guid Id { get; set; }
    public Medicamento Medicamento { get; set; }
    public string Dosagem { get; set; }
    public string Periodo { get; set; }
    public int Quantidade { get; set; }

    public MedicamentoPrescrito() { }

    public MedicamentoPrescrito(Medicamento medicamento, string dosagem, string periodo, int quantidade)
    {
        Id = Guid.NewGuid();
        Medicamento = medicamento;
        Dosagem = dosagem;
        Periodo = periodo;
        Quantidade = quantidade;
    }
}