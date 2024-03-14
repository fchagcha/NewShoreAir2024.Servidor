namespace NewShoreAir.DataAccess.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string requestBody = await ObtenerRequestBodyAsync(context);

            try
            {
                await _next(context);
            }
            catch (SecurityTokenValidationException ex)
            {
                await ProcesaExcepcionAsync(context, ex, ex.Message, StatusCodes.Status401Unauthorized, requestBody);
            }
            catch (CustomValidationException ex)
            {
                var validaciones =
                    ex.Errors
                    .Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}")
                    .ToList();

                string mensajeValidaciones = string.Join(", ", validaciones);

                var mensaje = $"{ex.Message}, {mensajeValidaciones}";

                await ProcesaExcepcionAsync(context, ex, mensaje, 409, requestBody);
            }
            catch (CustomException ex)
            {
                await ProcesaExcepcionAsync(context, ex, ex.Message, 422, requestBody);
            }
            catch (DbException ex)
            {
                await ProcesaExcepcionAsync(context, ex, "Error de base de datos", StatusCodes.Status416RequestedRangeNotSatisfiable, requestBody);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await ProcesaExcepcionAsync(context, ex, "Error de concurrencia, vuelva a procesar la petición", 4002, requestBody);
            }
            catch (Exception ex)
            {
                var statusCode = StatusCodes.Status400BadRequest;
                var mensaje = "Se ha producido un error en el servidor";

                if (ex is DbUpdateConcurrencyException ||
                    ex.InnerException is DbUpdateConcurrencyException ||
                    ex.InnerException?.InnerException is DbUpdateConcurrencyException)
                {
                    statusCode = 4002;
                    mensaje = "Error de concurrencia, vuelva a procesar la petición";
                }

                await ProcesaExcepcionAsync(context, ex, mensaje, statusCode, requestBody);
            }
        }

        private static async Task<string> ObtenerRequestBodyAsync(HttpContext context)
        {
            var requestBody = string.Empty;
            if (context is not null && context.Request.ContentLength > 0)
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            return requestBody;
        }
        private Task ProcesaExcepcionAsync(
            HttpContext context,
            Exception exception,
            string mensaje,
            int statusCode,
            string data)
        {
            RegistraExceptionless(context, exception, data);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(JsonConvert.SerializeObject(mensaje));
        }
        private void RegistraExceptionless(
            HttpContext context,
            Exception exception,
            string data)
        {
            var claimsPrincipal = context?.User;

            var nombreUsuario =
                claimsPrincipal
                ?.Identity
                ?.Name ?? string.Empty;

            exception
                .ToExceptionless()
                .SetReferenceId(Guid.NewGuid().ToString("N"))
                .AddObject(data)
                .SetHttpContext(context)
                .SetUserIdentity(nombreUsuario)
                .Submit();
        }
    }
}