using ControleDeMedicamentos.Dominio.Compartilhado;
using System.Text.RegularExpressions;

namespace ControleDeMedicamentos.Dominio.ModuloFuncionario;

public class Funcionario : EntidadeBase<Funcionario>
{
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public string Cpf { get; set; }

    public Funcionario() { }

    public Funcionario(string nome, string telefone, string cpf) : this()
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Telefone = telefone;
        Cpf = cpf;
    }

    public override void AtualizarRegistro(Funcionario registroEditado)
    {
        Nome = registroEditado.Nome;
        Telefone = registroEditado.Telefone;
        Cpf = registroEditado.Cpf;
    }

    public override string Validar()
    {
        string erros = "";

        if (string.IsNullOrWhiteSpace(Nome))
            erros += "O campo 'Nome' é obrigatório.\n";

        else if (Nome.Length < 2 || Nome.Length > 100)
            erros += "O campo 'Nome' deve conter entre 2 e 100 caracteres.\n";

        if (string.IsNullOrWhiteSpace(Telefone))
            erros += "O campo 'Telefone' é obrigatório.\n";

        else if (!Regex.IsMatch(Telefone, @"^\(?\d{2}\)?\s?(9\d{4}|\d{4})-?\d{4}$"))
            erros += "O campo 'Telefone' deve seguir o padrão (DDD) 0000-0000 ou (DDD) 00000-0000.\n";

        if (string.IsNullOrWhiteSpace(Cpf))
            erros += "O campo 'CPF' é obrigatório.\n";

        else if (!Regex.IsMatch(Cpf, @"^\d{3}\.\d{3}\.\d{3}-\d{2}$"))
            erros += "O campo 'CPF' deve seguir o formato 000.000.000-00.\n";

        return erros;
    }
}
