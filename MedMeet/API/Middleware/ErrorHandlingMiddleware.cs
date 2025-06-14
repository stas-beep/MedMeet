namespace API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ErrorHandlingMiddleware> log;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            this.next = next;
            log = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Помилка при обробці запиту");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var response = new
                {
                    error = new
                    {
                        message = "Сталася внутрішня помилка сервера.",
                        detail = ex.Message
                    }
                };

                var json = System.Text.Json.JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
