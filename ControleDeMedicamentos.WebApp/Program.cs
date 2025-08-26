using ControleDeMedicamentos.Infraestrutura.Arquivos.Compartilhado;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloFornecedor;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloFuncionario;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloMedicamento;
using ControleDeMedicamentos.WebApp.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace ControleDeMedicamentos.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Injeção de dependências criadas por nós
        builder.Services.AddScoped((_) => new ContextoDados(true));
        builder.Services.AddScoped<RepositorioMedicamentoEmArquivo>();
        builder.Services.AddScoped<RepositorioFornecedorEmArquivo>();
        builder.Services.AddScoped<RepositorioFuncionarioEmArquivo>();
        
        SerilogConfig.AddSerilogConfig(builder.Services, builder.Logging, builder.Configuration);

        // Injeção de dependências da Microsoft.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
