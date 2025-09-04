using ControleDeMedicamentos.WebApp.DependencyInjection;

namespace ControleDeMedicamentos.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Injeção de dependências criadas por nós
        builder.Services.AddCamadaInfraestrutura();

        builder.Services.AddSerilogConfig(builder.Logging, builder.Configuration);

        // Injeção de dependências da Microsoft.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Middleware - Funções que executam durante cada requisição e resposta HTTP
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
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
}
