using ControleDeMedicamentos.Infraestrutura.Arquivos.Compartilhado;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloFuncionario;
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
        builder.Services.AddScoped<RepositorioFuncionarioEmArquivo>();          // Injeta uma instância do serviço por requisição (ação) HTTP, essa instância acompanha a requisição do cliente

        var caminhoAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        var caminhoArquivoLogs = Path.Combine(caminhoAppData, "ControleDeMedicamentos", "erro.log");

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(caminhoArquivoLogs, LogEventLevel.Error)
            .WriteTo.NewRelicLogs(
                endpointUrl: "https://log-api.newrelic.com/log/v1",
                applicationName: "controle-de-medicamentos",
                licenseKey: "06f191c36b160aa357b6185df94d5eb1FFFFNRAL"
            )
            .CreateLogger();

        builder.Logging.ClearProviders();

        builder.Services.AddSerilog();

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
