using ControleDeMedicamentos.Infraestrutura.Arquivos.Compartilhado;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloFornecedor;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloFuncionario;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloMedicamento;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloPaciente;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloPrescricao;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloRequisicaoMedicamento;
using ControleDeMedicamentos.Infraestrutura.SqlServer.ModuloPaciente;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ControleDeMedicamentos.WebApp.DependencyInjection;

public static class InfraestruturaConfig
{
    public static void AddCamadaInfraestrutura(this IServiceCollection services, IConfiguration configuracao)
    {
        services.AddScoped<IDbConnection>(_ =>
        {
            var connectionString = configuracao["SQL_CONNECTION_STRING"];

            return new SqlConnection(connectionString);
        });

        services.AddScoped<RepositorioPacienteEmSql>();

        services.AddScoped((_) => new ContextoDados(true));
        services.AddScoped<RepositorioMedicamentoEmArquivo>();
        services.AddScoped<RepositorioFornecedorEmArquivo>();
        services.AddScoped<RepositorioFuncionarioEmArquivo>();
        services.AddScoped<RepositorioPacienteEmArquivo>();
        services.AddScoped<RepositorioPrescricaoEmArquivo>();
        services.AddScoped<RepositorioRequisicaoMedicamentoEmArquivo>();
    }
}
