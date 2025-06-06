using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/")]
public class SincronizarMaterialesController : ControllerBase
{
    private readonly ILogger<SincronizarMaterialesController> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public SincronizarMaterialesController(
        ILogger<SincronizarMaterialesController> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
    }

    public class LoteMaterialDto
    {
        public string MATNR { get; set; }
        public string CHARG { get; set; }
        public decimal CLABS { get; set; }
    }

    [HttpGet("SincronizarMaterialesController")]
    public async Task<IActionResult> ImportarMaterialesDesdeSAP()
    {
        string connectionString = _configuration.GetConnectionString("SQL_GENESYS_QAS");

        // Verificar si la cadena de conexión es nula o vacía
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogError("La cadena de conexión no está configurada.");
            return StatusCode(500, "Error interno en el servidor. Cadena de conexión no configurada.");
        }

        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("OBTENER_PARAMETROS_MATERIAL_LOTE", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                string matnr = reader["MATNR"]?.ToString();
                string werks = reader["WERKS"]?.ToString();
                string burks = reader["BURKS"]?.ToString();
                string iAlmacenId = reader["mate_iAlmacenId"]?.ToString();

                if (string.IsNullOrEmpty(matnr) || string.IsNullOrEmpty(werks) || string.IsNullOrEmpty(burks))
                {
                    _logger.LogWarning($"Valores incompletos. MATNR: {matnr}, WERKS: {werks}, BURKS: {burks}");
                    continue;
                }

                _logger.LogInformation($"SQL Data -> MATNR: {matnr}, WERKS: {werks}, BURKS: {burks}, AlmacenId: {iAlmacenId}");

                await ConsultarSAP(matnr, werks, burks, iAlmacenId, connectionString);
            }

            return Ok("Sincronización completada.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al sincronizar materiales");
            return StatusCode(500, "Error interno en el servidor.");
        }
    }

    private async Task ConsultarSAP(string matnr, string werks, string burks, string iAlmacenId, string connectionString)
    {
        try
        {
            string url = $"http://localhost:5104/api/ObtenerLoteMaterialController?I_MARTNR={matnr}&I_BUKRS={burks}&I_WERKS={werks}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error al consultar la API externa: {response.StatusCode}");
                return;
            }

            var responseData = await response.Content.ReadFromJsonAsync<List<LoteMaterialDto>>();
            if (responseData == null || responseData.Count == 0)
            {
                _logger.LogWarning("La API externa no devolvió datos");
                return;
            }

            foreach (var item in responseData)
            {
                if (item.CLABS != 0)
                {
                    _logger.LogInformation($"API Data -> MATNR:{item.MATNR}, CHARG:{item.CHARG}, CLABS:{item.CLABS}, AlmacenId:{iAlmacenId}");
                    await InsertarDataSAPaSQL(item.MATNR, item.CHARG, item.CLABS, iAlmacenId, connectionString);
                    await ProcesarDataHaciaARIS(connectionString);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al consumir la API externa de materiales");
        }
    }

    private async Task InsertarDataSAPaSQL(string matnr, string charg, decimal clabs, string iAlmacenId, string connectionString)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("AMTRO_SP_INSERTAR_LOTE_MATERIAL", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@matnr", matnr);
            command.Parameters.AddWithValue("@charg", charg);
            command.Parameters.AddWithValue("@clabs", clabs);
            command.Parameters.AddWithValue("@mate_iAlmacenId", iAlmacenId);

            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar data en SQL");
        }
    }

    private async Task ProcesarDataHaciaARIS(string connectionString)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("AMTRO_SP_ASYNC_MATERIAL_LOTE", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar datos hacia ARIS");
        }
    }
}
