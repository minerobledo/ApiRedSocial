using Infrastructure;
using Infrastructure.Hubs;
using System.Reflection;
using Aplication.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Aplication.Features.Frinship.Command.ResonceFriendshipRequest;
using Microsoft.AspNetCore.HttpOverrides;
using System.Text;
using Api.Middleware;

// --- INICIO DE LA APLICACIÓN ---
Console.WriteLine("--- Aplicación ASP.NET Core Iniciando ---");

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var portEnv = Environment.GetEnvironmentVariable("PORT");
    var port = string.IsNullOrWhiteSpace(portEnv) ? 8080 : int.Parse(portEnv); // fallback para desarrollo local
    serverOptions.ListenAnyIP(port);
    Console.WriteLine($"✅ Kestrel escuchando en puerto {port}");
});

// Configuración adicional
builder.Configuration.AddJsonFile("appsettingssecrets.json", optional: true, reloadOnChange: true);
// REMOVIDO: builder.Configuration.AddJsonFile("firebase-credentials.json", optional: true, reloadOnChange: true);
Console.WriteLine("Archivos de configuración cargados (appsettingssecrets.json).");

// Infraestructura y servicios
builder.Services.AddHttpClient();
Console.WriteLine("HttpClient agregado.");
builder.Services.AddInfrastructure(builder.Configuration);
Console.WriteLine("Servicios de infraestructura agregados.");

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies([
        typeof(Program).Assembly,
        typeof(ResonceFriendshipRequestCommandHandler).Assembly
    ]);
});
Console.WriteLine("MediatR configurado.");

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
Console.WriteLine("AutoMapper configurado.");

builder.Services.AddLogging();
Console.WriteLine("Servicios de logging agregados.");

// Autenticación y autorización
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
Console.WriteLine("Autenticación JWT Bearer agregada.");

builder.Services.AddAuthorization();
Console.WriteLine("Servicios de autorización agregados.");

// Configuración de CORS
Console.WriteLine("Iniciando configuración de ForwardedHeadersOptions para proxies.");
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
       ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear(); // Lo agregás si usás un proxy externo
    options.KnownProxies.Clear();
});
Console.WriteLine("ForwardedHeadersOptions configuradas.");

Console.WriteLine("Iniciando configuración de políticas CORS.");

// --- CAMBIO CLAVE AQUÍ: VOLVER A LEER DESDE LA CONFIGURACIÓN ---
// Comentamos o eliminamos la línea de hardcoding
// var hardcodedAllowedOrigins = new string[] { /* ... */ };
// Y volvemos a la línea original para leer desde builder.Configuration
var allowedOrigins = new[]
{
    "https://purple-arya-anfe-admin.web.app/#/photos",
    "https://purple-arya-anfe-admin.web.app",
    "https://redselecta.com",
    Environment.GetEnvironmentVariable("Cors_AllowedOrigins_")
}
.Where(origin => !string.IsNullOrWhiteSpace(origin)) // Filtrar nulos o vacíos
.ToArray();
// Mantenemos este log para confirmar qué valores se están leyendo.
Console.WriteLine($"Orígenes permitidos leídos de la configuración (Cors:AllowedOrigins): [{string.Join(", ", allowedOrigins)}]");

builder.Services.AddCors(options =>
{
    Console.WriteLine("Agregando política CORS 'DefaultPolicy'.");
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins) // <-- Usamos la variable 'allowedOrigins' que se lee de la configuración
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
        Console.WriteLine($"Política 'DefaultPolicy' configurada con: Orígenes: {string.Join(", ", allowedOrigins)}, AllowAnyHeader, AllowAnyMethod, AllowCredentials.");
    });
});
//builder.Services.AddCors(options =>
//{
//    Console.WriteLine("Agregando política CORS 'DefaultPolicy'.");
//    options.AddPolicy("DefaultPolicy", policy =>
//    {
//        policy.AllowAnyOrigin()// <-- Usamos la variable 'allowedOrigins' que se lee de la configuración
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();
//        Console.WriteLine($"Política 'DefaultPolicy' configurada con: Orígenes: {string.Join(", ", allowedOrigins)}, AllowAnyHeader, AllowAnyMethod, AllowCredentials.");
//    });
//});
Console.WriteLine("Configuración de servicios CORS finalizada.");


// Controladores y Swagger
builder.Services.AddControllers();
Console.WriteLine("Controladores agregados.");

builder.Services.AddEndpointsApiExplorer();
Console.WriteLine("Endpoint API Explorer agregado.");

builder.Services.AddSwaggerGen();
Console.WriteLine("SwaggerGen agregado.");



var app = builder.Build();
Console.WriteLine("Aplicación construida.");

// Middleware pipeline
Console.WriteLine("Configurando el pipeline de middlewares...");

app.UseMiddleware<AppVersionMiddleware>();
Console.WriteLine("Middleware AppVersionMiddleware agregado.");
app.UseMiddleware<ErrorHandlingMiddleware>();



app.UseForwardedHeaders();
Console.WriteLine("Middleware UseForwardedHeaders agregado.");

app.UseWebSockets();

app.Use((context, next) =>
{
    if (context.Request.Headers.TryGetValue("X-Forwarded-Proto", out var proto) && proto == "https")
    {
        context.Request.Scheme = "https";
        context.Request.Host = new HostString(context.Request.Host.Host, 443);
    }
    return next();
});
app.UseRouting();
Console.WriteLine("Middleware UseRouting agregado.");

app.UseCors("DefaultPolicy"); // Necesario para que EnableCors funcione en endpoints específicos
Console.WriteLine("Middleware UseCors con política 'DefaultPolicy' agregado.");

app.UseAuthentication();
Console.WriteLine("Middleware UseAuthentication agregado.");

app.UseAuthorization();
Console.WriteLine("Middleware UseAuthorization agregado.");

if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Entorno de desarrollo detectado. Agregando Swagger UI.");
    app.UseSwagger();
    app.UseSwaggerUI();
    Console.WriteLine("Swagger UI agregado.");
}

// ⚠️ Descomentá si querés redirigir HTTP -> HTTPS (Railway ya lo hace)
//// app.UseHttpsRedirection();
// Console.WriteLine("Middleware UseHttpsRedirection (comentado, manejado por Railway).");

// SignalR
app.MapHub<ChatHub>("/chatHub");
Console.WriteLine("Hub de SignalR mapeado en /chatHub.");

// Reprogramar concursos al iniciar
using (var scope = app.Services.CreateScope())
{
    var bacap = scope.ServiceProvider.GetRequiredService<IBackapService>();
    Console.WriteLine("Iniciando reprogramación de concursos...");
    try
    {
        await bacap.ReprogramarConcursosAsync();
        Console.WriteLine("Reprogramación de concursos completada.");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error durante la reprogramación de concursos: {ex.Message}");
    }
}

// Mapear controladores
app.MapControllers();
Console.WriteLine("Controladores mapeados.");

Console.WriteLine("--- Pipeline de middlewares configurado. La aplicación está lista para recibir solicitudes. ---");

try
{
    app.Run();
    Console.WriteLine("La aplicación ha terminado de ejecutarse.");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"--- ERROR CRÍTICO AL INICIAR LA APLICACIÓN: {ex.Message} ---");
    Console.Error.WriteLine(ex.StackTrace);
}