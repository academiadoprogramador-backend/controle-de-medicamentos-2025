using ControleDeMedicamentos.Dominio.ModuloFornecedor;
using ControleDeMedicamentos.Dominio.ModuloFuncionario;
using ControleDeMedicamentos.Dominio.ModuloMedicamento;
using ControleDeMedicamentos.Dominio.ModuloPaciente;
using ControleDeMedicamentos.Dominio.ModuloPrescricao;
using ControleDeMedicamentos.Dominio.ModuloRequisicaoMedicamento;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ControleDeMedicamentos.Infraestrutura.Arquivos.Compartilhado;

public class ContextoDados
{
    public List<Funcionario> Funcionarios { get; set; } = new List<Funcionario>();
    public List<Fornecedor> Fornecedores { get; set; } = new List<Fornecedor>();
    public List<Medicamento> Medicamentos { get; set; } = new List<Medicamento>();
    public List<Paciente> Pacientes { get; set; } = new List<Paciente>();
    public List<Prescricao> Prescricoes { get; set; } = new List<Prescricao>();
    public List<RequisicaoEntrada> RequisicoesEntrada { get; set; } = new List<RequisicaoEntrada>();

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
        Fornecedores = contextoArmazenado.Fornecedores;
        Medicamentos = contextoArmazenado.Medicamentos;
        Pacientes = contextoArmazenado.Pacientes;
        Prescricoes = contextoArmazenado.Prescricoes;
        RequisicoesEntrada = contextoArmazenado.RequisicoesEntrada;
    }
}
