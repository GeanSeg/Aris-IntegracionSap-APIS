using Application.Interfaces;
using Application.Services;
using Dominio.Entidad;
using Infrastructure.Configuration;
using Infrastructure.Sap;
using Infrastructure.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SapSettings>(
    builder.Configuration.GetSection("SapSettings"));

Log.Logger = new LoggerConfiguration()
    .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "log.txt"), rollingInterval: RollingInterval.Day)  // Guardar los logs en el archivo log.txt
    .CreateLogger();

//builder.Services.AddScoped<ILogger, _Logger>();
builder.Services.AddSingleton<ILibraryInitializer, LibraryInitialize>();
builder.Services.AddSingleton<IInitializerContextSAP, InitializerContextSAP>();
builder.Services.AddSingleton<ILoggingService, LoggerServicio>();
//builder.Services.AddSingleton<IEntregaService, ReplicaOdooEntregaSap>();
//builder.Services.AddSingleton<EntregaService>();
builder.Services.AddTransient<SapResultadoPedido>();
builder.Services.AddTransient<SapResultadoEntrega>();
builder.Services.AddTransient<SapResultadoFactura>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


