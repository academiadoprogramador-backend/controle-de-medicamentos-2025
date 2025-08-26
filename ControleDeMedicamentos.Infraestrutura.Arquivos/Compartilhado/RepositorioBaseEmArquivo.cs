using ControleDeMedicamentos.Dominio.Compartilhado;

namespace ControleDeMedicamentos.Infraestrutura.Arquivos.Compartilhado;

public abstract class RepositorioBaseEmArquivo<Tipo> where Tipo : EntidadeBase<Tipo>
{
    protected ContextoDados contextoDados;
    protected List<Tipo> registros = new List<Tipo>();

    protected RepositorioBaseEmArquivo(ContextoDados contextoDados)
    {
        this.contextoDados = contextoDados;

        registros = ObterRegistros();
    }

    protected abstract List<Tipo> ObterRegistros();

    public void CadastrarRegistro(Tipo novoRegistro)
    {
        registros.Add(novoRegistro);

        contextoDados.Salvar();
    }

    public bool EditarRegistro(Guid idSelecionado, Tipo registroAtualizado)
    {
        Tipo registroSelecionado = SelecionarRegistroPorId(idSelecionado);

        if (registroSelecionado == null)
            return false;

        registroSelecionado.AtualizarRegistro(registroAtualizado);

        contextoDados.Salvar();

        return true;
    }

    public bool ExcluirRegistro(Guid idSelecionado)
    {
        for (int i = 0; i < registros.Count; i++)
        {
            if (registros[i] == null)
                continue;

            else if (registros[i].Id == idSelecionado)
            {
                registros.Remove(registros[i]);

                contextoDados.Salvar();

                return true;
            }
        }

        return false;
    }

    public List<Tipo> SelecionarRegistros()
    {
        return registros;
    }

    public Tipo SelecionarRegistroPorId(Guid idSelecionado)
    {
        var registro = registros.Find(r => r.Id == idSelecionado);

        return registro;
    }
}
