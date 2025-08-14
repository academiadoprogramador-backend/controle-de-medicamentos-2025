using ControleDeMedicamentos.Dominio.ModuloFuncionario;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ControleDeMedicamentos.Infraestrutura.Arquivos.Compartilhado;

public class ContextoDados
{
    public List<Funcionario> Funcionarios { get; set; } = new List<Funcionario>();

    private string pastaArmazenamento = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ControleDeMedicamentos"
    );
    private string arquivoArmazenamento = "dados.json";

    public ContextoDados() { }

    public ContextoDados(bool carregarDados) : this()
    {
        if (carregarDados)
            Carregar();
    }

    public void Salvar()
    {
        string caminhoCompleto = Path.Combine(pastaArmazenamento, arquivoArmazenamento);

        JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
        jsonOptions.WriteIndented = true;
        jsonOptions.ReferenceHandler = ReferenceHandler.Preserve;

        var jsonString = JsonSerializer.Serialize(this, jsonOptions);

        if (!Directory.Exists(pastaArmazenamento))
            Directory.CreateDirectory(pastaArmazenamento);

        File.WriteAllText(caminhoCompleto, jsonString);
    }

    public void Carregar()
    {
        string caminhoCompleto = Path.Combine(pastaArmazenamento, arquivoArmazenamento);

        if (!File.Exists(caminhoCompleto)) return;

        string jsonString = File.ReadAllText(caminhoCompleto);

        if (string.IsNullOrWhiteSpace(jsonString)) return;

        JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
        jsonOptions.ReferenceHandler = ReferenceHandler.Preserve;

        ContextoDados? contextoArmazenado = JsonSerializer.Deserialize<ContextoDados>(jsonString, jsonOptions);

        if (contextoArmazenado == null) return;

        Funcionarios = contextoArmazenado.Funcionarios;
    }
}
