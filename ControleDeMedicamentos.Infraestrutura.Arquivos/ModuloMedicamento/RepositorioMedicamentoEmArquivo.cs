using ControleDeMedicamentos.Dominio.ModuloMedicamento;
using ControleDeMedicamentos.Infraestrutura.Arquivos.Compartilhado;

namespace ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloMedicamento;

public class RepositorioMedicamentoEmArquivo : RepositorioBaseEmArquivo<Medicamento>
{
    public RepositorioMedicamentoEmArquivo(ContextoDados contextoDados) : base(contextoDados) { }

    protected override List<Medicamento> ObterRegistros()
    {
        return contextoDados.Medicamentos;
    }
}
