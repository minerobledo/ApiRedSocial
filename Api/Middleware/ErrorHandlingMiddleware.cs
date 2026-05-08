namespace Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;

                var response = new
                {
                    message = ex.Message,
                    // stackTrace = ex.StackTrace // solo en desarrollo
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
