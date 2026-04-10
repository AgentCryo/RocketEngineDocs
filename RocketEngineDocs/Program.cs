using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RocketEngineDocs.Components;
using RocketEngineDocs.Layout;

namespace RocketEngineDocs;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        var docPages = typeof(Program).Assembly
            .GetTypes()
            .Where(t =>
                typeof(IDocPage).IsAssignableFrom(t)
                && !t.IsAbstract
                && t.IsClass
                && t.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Length == 0
            );

        foreach (var pageType in docPages)
        {
            var page = (IDocPage)Activator.CreateInstance(pageType)!;
            var docBuilder = page.BuildDoc();
            NavMenu.NavRegistry.RegisterDoc(docBuilder);
        }
        
        var devlogPages = typeof(Program).Assembly
            .GetTypes()
            .Where(t =>
                typeof(DevlogBase).IsAssignableFrom(t)
                && !t.IsAbstract
                && t.IsClass
                && t.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Length == 0
            );

        foreach (var pageType in devlogPages)
        {
            // Create instance so OnInitialized runs
            var instance = (DevlogBase)Activator.CreateInstance(pageType)!;
            DevlogBase.RegisterDevlog(instance.Header);
        }
        
        await builder.Build().RunAsync();
    }
}