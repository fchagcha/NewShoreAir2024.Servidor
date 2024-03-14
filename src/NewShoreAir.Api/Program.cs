using NewShoreAir.Api;
using NewShoreAir.DataAccess.Persistencia;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddConfigureServices<NewShoreAirDbContext>(builder.Configuration);

builder
    .Build()
    .ConfigurePipeline(builder.Configuration)
    .Run();