using ControleDeMedicamentos.Infraestrutura.Arquivos.Compartilhado;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloFuncionario;

namespace ControleDeMedicamentos.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Injeção de dependências criadas por nós
        builder.Services.AddScoped(ConfigurarContextoDados);
        builder.Services.AddScoped<RepositorioFuncionarioEmArquivo>();          // Injeta uma instância do serviço por requisição (ação) HTTP, essa instância acompanha a requisição do cliente
        //builder.Services.AddSingleton<RepositorioFuncionarioEmArquivo>();     // Injeta uma instância única do serviço globalmente
        //builder.Services.AddTransient<RepositorioFuncionarioEmArquivo>();     // Injeta uma instância nova do serviço toda vez que houver uma dependência ao longo de uma requisição

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

    private static ContextoDados ConfigurarContextoDados(IServiceProvider serviceProvider)
    {
        return new ContextoDados(true);
    }
}
