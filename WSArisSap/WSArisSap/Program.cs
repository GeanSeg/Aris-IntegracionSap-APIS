using Application.Interfaces;
using Application.Services;
using Dominio.Entidad;
using Infrastructure.Configuration;
using Infrastructure.Sap;
using Infrastructure.Services;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SapSettings>(
    builder.Configuration.GetSection("SapSettings"));

#region logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        Path.Combine(AppContext.BaseDirectory, "log.txt"),
        rollingInterval: RollingInterval.Day)
    .CreateLogger();
#endregion

builder.Services.AddSingleton<ILibraryInitializer, LibraryInitialize>();
builder.Services.AddSingleton<IInitializerContextSAP, InitializerContextSAP>();
builder.Services.AddSingleton<ILoggingService, LoggerServicio>();

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

#region Builder.Services scope
const string CONFIGCAT_URL =
"https://cdn-global.configcat.com/configuration-files/configcat-sdk-1/ckXeCOmBCEiKRwz7L50yqA/J8fHLeOIFkeE34wQ47f4Cw/config_v6.json";

bool apiActiva = false;

using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
    var client = factory.CreateClient();

    try
    {
        var response = await client.GetAsync(CONFIGCAT_URL);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            apiActiva =
                doc.RootElement
                   .GetProperty("f")
                   .GetProperty("flag_api_activo")
                   .GetProperty("v")
                   .GetProperty("i").GetInt32() == 1;
        }
    }
    catch
    {
        apiActiva = false;
    }
}

if (!apiActiva)
{
    app.MapGet("/{**any}", () => Results.NotFound("API APAGADA POR CONFIGURACIÓN"));
    app.Run();
    return;
}
#endregion

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.MapGet("/flag", () => Results.Ok(42));

app.Run();
