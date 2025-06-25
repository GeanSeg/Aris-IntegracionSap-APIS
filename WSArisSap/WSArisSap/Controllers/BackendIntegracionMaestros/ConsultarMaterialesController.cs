using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace WSArisSap.Controllers
{
    [ApiController]
    [Route("api/")]
    public class ConsultarMaterialesController : ControllerBase
    {
        public class MaterialResult
        {
            public string ID { get; set; }
            public string Sociedad { get; set; }
            public string Material { get; set; }
            public string DescripcionMaterial { get; set; }
            public string Accion { get; set; }

        }

        public class MaterialMigradoDto
        {

            public string MATNR { get; set; }
            public string MAKTX { get; set; }
            public string WERKS { get; set; }
            public string LGORT { get; set; }
            public string MEINS { get; set; }
            public string GEWEI { get; set; }
            public string MENGE { get; set; }
            public string MATKL { get; set; }
            public string MTART { get; set; }
            public string SPART { get; set; }
            public string XCHPF { get; set; }
            public string XCHPF_C { get; set; }
            public string PRICE_UN { get; set; }
            public string WAERS { get; set; }
        }

        public class RootResponse
        {
            public DataResponse Data { get; set; }
        }

        public class DataResponse
        {
            public List<MaterialMigradoDto> T_DATA_MAT { get; set; }
        }

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public ConsultarMaterialesController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet("ConsultarMaterialesController")]
        public async Task<IActionResult> ImportarDesdeApiSap(
        [FromQuery] string? I_BUKRS = null,
        [FromQuery] string? I_FECCREA_I = null,
        [FromQuery] string? I_FECCREA_F = null,
        [FromQuery] string? I_FECMOD_I = null,
        [FromQuery] string? I_FECMOD_F = null)
        {
            if (string.IsNullOrWhiteSpace(I_BUKRS))
            {
                return BadRequest(new { Error = "Debe enviar el parámetro I_BUKRS (compañía)." });
            }

            string companiaNombre = I_BUKRS switch
            {
                "1600" => "ROCIO S.A.C",
                "1700" => "AVIAGEN S.A.C",
                _ => "Compañía desconocida"
            };

            string tipoConsulta = (I_FECCREA_I != null && I_FECCREA_F != null)
                 ? "Creado"
                 : (I_FECMOD_I != null && I_FECMOD_F != null)
                 ? "Modificado"
                 : "Ingresar una Fecha de Creacion o Modificacion";

            string baseUrl = "https://sap-qas-genesysavi.rocio.com.pe/sap-api/api/ObtenerMaterialController";
            string apiUrl = $"{baseUrl}?I_BUKRS={I_BUKRS}&I_FECCREA_I={I_FECCREA_I}&I_FECCREA_F={I_FECCREA_F}&I_FECMOD_I={I_FECMOD_I}&I_FECMOD_F={I_FECMOD_F}";

            string connectionString = _configuration.GetConnectionString("SQL_GENESYS_QAS");

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, $"Error al consumir API externa: {response.ReasonPhrase}");

                var json = await response.Content.ReadAsStringAsync();
                var materialesWrapper = JsonSerializer.Deserialize<RootResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var materiales = materialesWrapper?.Data?.T_DATA_MAT;

                if (materiales == null || materiales.Count == 0)
                {
                    return Ok(new
                    {
                        Mensaje = "No se encontraron materiales.",
                    });
                }

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                foreach (var item in materiales)
                {
                    await InsertarEnSql(item, connection);
                }

                // Después de insertar, hacer el SELECT
                var materialesExistentes = new List<MaterialResult>();

                string query = @"SELECT ID, MATNR, MAKTX, WERKS, LGORT, MEINS, GEWEI, MENGE, MATKL, MTART, SPART, XCHPF, XCHPF_C, PRICE_UN, WAERS 
                         FROM AMTRO_TA_TEMPORAL_MATERIALES_SAP";
                using var cmd = new SqlCommand(query, connection);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    materialesExistentes.Add(new MaterialResult
                    {
                        ID = reader["ID"].ToString(),
                        Sociedad = companiaNombre,
                        Material = reader["MATNR"].ToString().TrimStart('0'),
                        DescripcionMaterial = reader["MAKTX"].ToString(),
                        Accion = tipoConsulta
                    });
                }

                return Ok(new
                {
                    Mensaje = "Materiales importados e insertados correctamente desde la API externa.",
                    DatosActuales = materialesExistentes
                });
            }
            catch (Exception ex)
            {
                // Captura del error y lectura de la tabla para incluirla en la respuesta
                try
                {
                    var materialesExistentes = new List<MaterialResult>();
                    using var connection = new SqlConnection(connectionString);
                    await connection.OpenAsync();

                    string query = @"SELECT ID, MATNR, MAKTX, WERKS, LGORT, MEINS, GEWEI, MENGE, MATKL, MTART, SPART, XCHPF, XCHPF_C, PRICE_UN, WAERS 
                             FROM AMTRO_TA_TEMPORAL_MATERIALES_SAP";
                    using var cmd = new SqlCommand(query, connection);
                    using var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        materialesExistentes.Add(new MaterialResult
                        {
                            ID = reader["ID"].ToString(),
                            Sociedad = companiaNombre,
                            Material = reader["MATNR"].ToString().TrimStart('0'),
                            DescripcionMaterial = reader["MAKTX"].ToString(),
                            Accion = tipoConsulta
                        });
                    }

                    return Ok(new
                    {
                        Advertencia = ex.Message,
                        DatosActuales = materialesExistentes
                    });
                }
                catch (Exception innerEx)
                {
                    return StatusCode(500, new
                    {
                        Error = ex.Message,
                        ErrorLectura = innerEx.Message,

                    });
                }
            }
        }



        private async Task InsertarEnSql(MaterialMigradoDto item, SqlConnection conn)
        {
            string query = @"
                INSERT INTO AMTRO_TA_TEMPORAL_MATERIALES_SAP 
                (MATNR, MAKTX, WERKS, LGORT, MEINS, GEWEI, MENGE, MATKL, MTART, SPART, XCHPF, XCHPF_C, PRICE_UN, WAERS)
                VALUES 
                (@matnr, @maktx, @werks, @lgort, @meins, @gewei, @menge, @matkl, @mtart, @spart, @xchpf, @xchpf_c, @price_un, @waers)";

            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@matnr", ValidarTexto(item.MATNR));
            cmd.Parameters.AddWithValue("@maktx", ValidarTexto(item.MAKTX));
            cmd.Parameters.AddWithValue("@werks", ValidarTexto(item.WERKS));
            cmd.Parameters.AddWithValue("@lgort", ValidarTexto(item.LGORT));
            cmd.Parameters.AddWithValue("@meins", ValidarTexto(item.MEINS));
            cmd.Parameters.AddWithValue("@gewei", ValidarTexto(item.GEWEI));
            cmd.Parameters.AddWithValue("@menge", ParseDecimal(item.MENGE));
            cmd.Parameters.AddWithValue("@matkl", ValidarTexto(item.MATKL));
            cmd.Parameters.AddWithValue("@mtart", ValidarTexto(item.MTART));
            cmd.Parameters.AddWithValue("@spart", ValidarTexto(item.SPART));
            cmd.Parameters.AddWithValue("@xchpf", ValidarTexto(item.XCHPF));
            cmd.Parameters.AddWithValue("@xchpf_c", ValidarTexto(item.XCHPF_C));
            cmd.Parameters.AddWithValue("@price_un", ParseDecimal(item.PRICE_UN));
            cmd.Parameters.AddWithValue("@waers", ValidarTexto(item.WAERS));

            await cmd.ExecuteNonQueryAsync();
        }

        private object ValidarTexto(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Trim() == "000000000000000000")
                return DBNull.Value;

            return input.Trim();
        }

        private object ParseDecimal(string input)
        {
            if (decimal.TryParse(input, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var result))
                return result;

            return DBNull.Value;
        }
    }
}
