using ControleDeMedicamentos.Dominio.ModuloFuncionario;
using ControleDeMedicamentos.Infraestrutura.Arquivos.Compartilhado;

namespace ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloFuncionario;

public class RepositorioFuncionarioEmArquivo : RepositorioBaseEmArquivo<Funcionario>
{
    public RepositorioFuncionarioEmArquivo(ContextoDados contextoDados) : base(contextoDados) { }

    protected override List<Funcionario> ObterRegistros()
    {
        return contextoDados.Funcionarios;
    }
}
