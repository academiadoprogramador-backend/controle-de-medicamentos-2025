using ControleDeMedicamentos.Infraestrutura.Arquivos.Compartilhado;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloMedicamento;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloPrescricao;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloRequisicaoMedicamento;
using ControleDeMedicamentos.Infraestrutura.BancoDeDados.ModuloFornecedor;
using ControleDeMedicamentos.Infraestrutura.BancoDeDados.ModuloFuncionario;
using ControleDeMedicamentos.Infraestrutura.BancoDeDados.ModuloMedicamento;
using ControleDeMedicamentos.Infraestrutura.BancoDeDados.ModuloPaciente;
using ControleDeMedicamentos.Infraestrutura.BancoDeDados.ModuloPrescricao;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ControleDeMedicamentos.WebApp.DependencyInjection;

public static class InfraestruturaConfig
{
    public static void AddCamadaInfraestrutura(this IServiceCollection services, IConfiguration configuracao)
    {
        services.AddScoped<IDbConnection>(opt =>
        {
            var connectionString = configuracao["SQL_CONNECTION_STRING"];

            return new SqlConnection(connectionString);
        });

        services.AddScoped<RepositorioFornecedorEmBancoDeDados>();
        services.AddScoped<RepositorioFuncionarioEmBancoDeDados>();
        services.AddScoped<RepositorioPacienteEmBancoDeDados>();
        services.AddScoped<RepositorioPrescricaoEmBancoDeDados>();
        services.AddScoped<RepositorioMedicamentoEmBancoDeDados>();

        services.AddScoped((_) => new ContextoDados(true));
        services.AddScoped<RepositorioMedicamentoEmArquivo>();
        services.AddScoped<RepositorioPrescricaoEmArquivo>();
        services.AddScoped<RepositorioRequisicaoMedicamentoEmArquivo>();
    }
}
