namespace NewShoreAir.DataAccess
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<IProvider, Provider>();
            services.AddScoped<IUoWService, UoWService>();
            services.AddScoped<IUoWCommand, UoWCommand>();


            services.AddExceptionless(configuration);

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services
            .AddOptions<VuelosApiSettings>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("VuelosApiSettings").Bind(settings);
            });

            services.AddHttpClient();
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddSingleton<IApiService, ApiService>();
            services.AddSingleton<IVueloApi, VueloApi>();

            return services;
        }
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<ExceptionMiddleware>();

            return applicationBuilder;
        }
    }
}