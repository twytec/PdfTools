using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using PdfTools;
using PdfTools.Data;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMudServices();
builder.Services.AddScoped<MyJsInterop>();
builder.Services.AddScoped<BusyService>();
builder.Services.AddScoped<TranslationService>();
builder.Services.AddScoped<PdfService>();
builder.Services.AddScoped<PdfWorkerClient>();

await builder.Build().RunAsync();
