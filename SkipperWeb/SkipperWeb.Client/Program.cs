using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;

namespace SkipperWeb.Client;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        // Set culture globally
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

        await builder.Build().RunAsync();
    }
}