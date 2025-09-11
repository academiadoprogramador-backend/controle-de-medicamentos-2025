using ControleDeMedicamentos.Dominio.ModuloFornecedor;
using ControleDeMedicamentos.Dominio.ModuloFuncionario;
using ControleDeMedicamentos.Dominio.ModuloPaciente;
using ControleDeMedicamentos.Infraestrutura.Arquivos.Compartilhado;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloMedicamento;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloPrescricao;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloRequisicaoMedicamento;
using ControleDeMedicamentos.Infraestrutura.BancoDeDados.ModuloFornecedor;
using ControleDeMedicamentos.Infraestrutura.BancoDeDados.ModuloFuncionario;
using ControleDeMedicamentos.Infraestrutura.BancoDeDados.ModuloPaciente;
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

        services.AddScoped<IRepositorioFornecedor, RepositorioFornecedorEmBancoDeDados>();
        services.AddScoped<IRepositorioFuncionario, RepositorioFuncionarioEmBancoDeDados>();
        services.AddScoped<IRepositorioPaciente, RepositorioPacienteEmBancoDeDados>();

        services.AddScoped((_) => new ContextoDados(true));
        services.AddScoped<RepositorioMedicamentoEmArquivo>();
        services.AddScoped<RepositorioPrescricaoEmArquivo>();
        services.AddScoped<RepositorioRequisicaoMedicamentoEmArquivo>();
    }
}
