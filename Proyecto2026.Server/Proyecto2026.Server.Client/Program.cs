using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Proyecto2026.Servicios.ServicioHTTP;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IHttpService, HttpService>(); //INYECCION DE DEPENDENCIAS PARA EL SERVICIO HTTP

await builder.Build().RunAsync();