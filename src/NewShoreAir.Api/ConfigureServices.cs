using Exceptionless;
using Microsoft.EntityFrameworkCore;
using NewShoreAir.Business.Application;
using NewShoreAir.DataAccess;

namespace NewShoreAir.Api
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddConfigureServices<TContext>(this IServiceCollection services,
            IConfiguration configuration) where TContext : DbContext
        {
            services.ConfigureControllers()
                    .ConfigureDatabase<TContext>(configuration)
                    .AddDataAccessServices(configuration)
                    .AddBusinessServices()
                    .ConfigureCors()
                    .AddSwaggerGen();

            return services;
        }
        public static WebApplication ConfigurePipeline(this WebApplication app, IConfiguration configuration)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionless(configuration);

            app.UseExceptionMiddleware();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("CorsPolicy");

            app.MapControllers();

            return app;
        }

        private static IServiceCollection ConfigureDatabase<TContext>(this IServiceCollection services,
            IConfiguration configuration) where TContext : DbContext
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            int indiceCaracterBuscado = baseDirectory.IndexOf(@"\bin");
            string textoCortado = baseDirectory.Substring(0, indiceCaracterBuscado);

            var defaultConnection = configuration.GetConnectionString("ConnectionString");
            var connectionString = defaultConnection!.Replace("{dbPath}", textoCortado);

            services.AddDbContext<TContext>(options =>
                options.UseSqlite(connectionString,
                b => b.MigrationsAssembly(typeof(TContext).Assembly.FullName)));

            return services;
        }

        private static IServiceCollection ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            return services;
        }
        private static IServiceCollection ConfigureControllers(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            return services;
        }
    }
}