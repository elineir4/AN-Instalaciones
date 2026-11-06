using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Proyecto2026.BD.Datos;
using Proyecto2026.BD.Datos.Entity;
using Proyecto2026.Repositorio.Repositorios;
using Proyecto2026.Server.Client.Pages;
using Proyecto2026.Server.Components;
using Proyecto2026.Servicios.ServicioHTTP;

var builder = WebApplication.CreateBuilder(args);// Crear un generador de aplicaciones web
                                                 // con los argumentos de línea de comandos


var connectionString = builder.Configuration.GetConnectionString("ConnSqlServer");
// Obtener la cadena de conexión desde el archivo appsettings.json



builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
// Agregar el contexto de la base de datos y configurar la cadena de conexión para SQL Server


builder.Services.AddRazorComponents() //agregar servicios para componentes Razor
    .AddInteractiveServerComponents()//agregar servicios para componentes interactivos del lado del servidor
    .AddInteractiveWebAssemblyComponents();//agregar servicios para componentes interactivos del lado del cliente


#region Servicios
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();// Agregar servicios para controladores y vistas
builder.Services.AddRazorPages();// Agregar servicios para páginas Razor


builder.Services.AddScoped(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
}); // Agregar un servicio HttpClient con la dirección base de la aplicación web
#endregion

#region Inyeccion de dependencias
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<IPagoEmpleadoRepositorio, PagoEmpleadoRepositorio>();
builder.Services.AddScoped<IPagoRepositorio1, PagoRepositorio>(); 
builder.Services.AddScoped<ITrabajoRepositorio, TrabajoRepositorio>();
builder.Services.AddScoped<IReservaRepositorio, ReservaRepositorio>();
builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddScoped<IEmpleadoRepositorio, EmpleadoRepositorio>();

#endregion


var app = builder.Build(); // Crear la aplicación web



if (app.Environment.IsDevelopment())// Si el entorno es de desarrollo, habilitar la depuración de WebAssembly
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else // Si el entorno no es de desarrollo, habilitar el manejo de excepciones y HSTS
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts. //
    app.UseHsts(); // Habilitar HSTS (HTTP Strict Transport Security) para mejorar la seguridad de la aplicación
  
}

#region Middlewares
app.MapControllers();// Mapear los controladores a las rutas correspondientes
#endregion


app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
//todo estos use es para habilitar el uso de archivos estáticos y archivos de Blazor en la aplicación


app.UseRouting();// Habilitar el enrutamiento de la aplicación

app.UseAntiforgery();  // Habilitar la protección contra ataques CSRF (Cross-Site Request Forgery) en los formularios de Blazor

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
//todo esto es el mapeo de los componentes de Razor y Blazor, para que se puedan renderizar en el navegador

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Proyecto2026.Server.Client._Imports).Assembly);
//esto es para mapear los componentes de Razor y Blazor, y habilitar el modo de renderizado
//interactivo del lado del servidor y del lado del cliente
//además de agregar ensamblados adicionales para que se puedan usar en los componentes


app.Run(); 
// Iniciar la aplicación web y escuchar las solicitudes entrantes
