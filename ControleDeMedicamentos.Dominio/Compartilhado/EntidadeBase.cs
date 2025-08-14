namespace ControleDeMedicamentos.Dominio.Compartilhado;

public abstract class EntidadeBase<Tipo>
{
    public Guid Id { get; set; }

    public abstract void AtualizarRegistro(Tipo registroAtualizado);
    public abstract string Validar();
}
