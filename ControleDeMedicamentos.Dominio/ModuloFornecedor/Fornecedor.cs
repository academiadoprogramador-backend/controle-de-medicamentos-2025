using ControleDeMedicamentos.Dominio.Compartilhado;

namespace ControleDeMedicamentos.Dominio.ModuloFornecedor;

public class Fornecedor : EntidadeBase<Fornecedor>
{
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public string Cnpj { get; set; }

    public Fornecedor() { }

    public Fornecedor(string nome, string telefone, string cnpj) : this()
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Telefone = telefone;
        Cnpj = cnpj;
    }

    public override void AtualizarRegistro(Fornecedor registroEditado)
    {
        Nome = registroEditado.Nome;    
        Telefone = registroEditado.Telefone;
        Cnpj = registroEditado.Cnpj;
    }

    public override string Validar()
    {
        string erros = "";

        if (string.IsNullOrEmpty(Nome))
            erros += "O campo Nome é obrigatório.\n";

        if (string.IsNullOrEmpty(Telefone))
            erros += "O campo Telefone é obrigatório.\n";

        if (string.IsNullOrEmpty(Cnpj))
            erros += "O campo CNPJ é obrigatório.\n";

        return erros.Trim();
    }
}
