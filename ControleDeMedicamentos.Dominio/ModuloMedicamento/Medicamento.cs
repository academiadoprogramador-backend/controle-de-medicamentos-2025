using ControleDeMedicamentos.Dominio.Compartilhado;
using ControleDeMedicamentos.Dominio.ModuloFornecedor;

namespace ControleDeMedicamentos.Dominio.ModuloMedicamento;

public class Medicamento : EntidadeBase<Medicamento>
{
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public Fornecedor Fornecedor { get; set; }
    public int QuantidadeEmEstoque { get; set; }

    public bool EmFalta
    {
        get { return QuantidadeEmEstoque < 20; }
    }

    public Medicamento() { }

    public Medicamento(
      string nome,
      string descricao,
      Fornecedor fornecedor
  ) : this()
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Descricao = descricao;
        Fornecedor = fornecedor;
    }

    public override void AtualizarRegistro(Medicamento registroAtualizado)
    {
        Nome = registroAtualizado.Nome;
        Descricao = registroAtualizado.Descricao;
        Fornecedor = registroAtualizado.Fornecedor;
    }

    public override string Validar()
    {
        string erros = string.Empty;

        if (string.IsNullOrEmpty(Nome.Trim()))
            erros += "O campo \"Nome\" é obrigatório.";

        if (string.IsNullOrEmpty(Descricao.Trim()))
            erros += "O campo \"Descrição\" é obrigatório.";

        if (Fornecedor == null)
            erros += "O campo \"Fornecedor\" é obrigatório.";

        return erros;
    }
}
