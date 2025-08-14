using ControleDeMedicamentos.Infraestrutura.Arquivos.Compartilhado;
using ControleDeMedicamentos.Infraestrutura.Arquivos.ModuloFuncionario;

namespace ControleDeMedicamentos.WebApp;

public class Program
{


    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Inje��o de depend�ncia
        builder.Services.AddScoped(ConfigurarContextoDados);
        builder.Services.AddScoped<RepositorioFuncionarioEmArquivo>();  // Injetar um servi�o por requisi��o HTTP
        //builder.Services.AddSingleton();                              // Instanciar uma vez o servi�o e injetar em todas as requisi��es
        //builder.Services.AddTransient();                              // Instanciar o servi�o TODA VEZ que for chamado em uma requisi��o

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

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
