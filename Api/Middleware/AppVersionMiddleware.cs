using System.Text.RegularExpressions;

namespace Api.Middleware
{
    public class AppVersionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Version _minVersion = new Version("1.0.0");
        private readonly string[] allowedVersions = new[] { "1.0.0" };

        // Opcional: rutas que no requieren versión
        private readonly List<Regex> _excludedPaths = new()
        {
            new Regex(@"^/chatHub", RegexOptions.IgnoreCase),
            new Regex(@"^/api/Admin", RegexOptions.IgnoreCase),
            new Regex(@"^/swagger", RegexOptions.IgnoreCase)
        };

        public AppVersionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.ToString();

            // Si la ruta está excluida, continuar
            if (_excludedPaths.Any(r => r.IsMatch(path)))
            {
                await _next(context);
                return;
            }
            //bloque de pruevas 
            // fin bloque de pruevas
            

            // Leer versión del header
            if (!context.Request.Headers.TryGetValue("App-Version", out var versionHeader))
            {
                Console.WriteLine(versionHeader);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Falta el header App-Version.");
                return;
            }

            if (!Version.TryParse(versionHeader.First(), out var version))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Formato de versión inválido.");
                return;
            }

            if (!allowedVersions.Contains(version.ToString()))
            {
                context.Response.StatusCode = StatusCodes.Status426UpgradeRequired;
                await context.Response.WriteAsync("Versión no soportada.");
                return;
            }

            if (version < _minVersion)
            {
                context.Response.StatusCode = StatusCodes.Status426UpgradeRequired;
                await context.Response.WriteAsync("Actualizá la app a la versión 1.0.0 o superior.");
                return;
            }

            await _next(context); // Continuar hacia el controlador
        }
    }
}
