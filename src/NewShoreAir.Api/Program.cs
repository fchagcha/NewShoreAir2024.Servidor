using Exceptionless;
using Microsoft.EntityFrameworkCore;
using NewShoreAir.Business.Application;
using NewShoreAir.DataAccess;
using NewShoreAir.DataAccess.Persistencia;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
int indiceCaracterBuscado = baseDirectory.IndexOf(@"\bin");
string textoCortado = baseDirectory.Substring(0, indiceCaracterBuscado);

var defaultConnection = builder.Configuration.GetConnectionString("ConnectionString");
var connectionString = defaultConnection!.Replace("{dbPath}", textoCortado);

builder.Services.AddDbContext<NewShoreAirDbContext>(options =>
    options.UseSqlite(connectionString,
    b => b.MigrationsAssembly(typeof(NewShoreAirDbContext).Assembly.FullName)));

builder.Services
    .AddDataAccessServices(builder.Configuration)
    .AddBusinessServices();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
                     builder => builder
                                .AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader());
});

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionless(builder.Configuration);

app.UseExceptionMiddleware();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();
