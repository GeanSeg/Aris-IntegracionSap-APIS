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
    public class ConsultarClientesController : ControllerBase
    {
        public class MaterialResult
        {
            public string ID { get; set; }
            public string OrgVenta { get; set; }
            public string Cliente { get; set; }
            public string CanalDistribucion { get; set; }
            public string Sector { get; set; }
            public string FuncionSocio { get; set; }
            public string InterlocutorComercial { get; set; }


        }

        public class MaterialMigradoDto
        {
            public string KUNNR { get; set; }
            public string VKORG { get; set; }
            public string VTWEG { get; set; }
            public string SPART { get; set; }
            public string PARVW { get; set; }
            public string KTONR { get; set; }
            public string DEFPA { get; set; }
        }

        public class RootResponse
        {
            public DataResponse Data { get; set; }
        }

        public class DataResponse
        {
            public List<MaterialMigradoDto> T_ASIG_CLIENT { get; set; }
        }

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public ConsultarClientesController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet("ConsultarClientesController")]
        public async Task<IActionResult> ImportarDesdeApiSap(
        [FromQuery] string? I_VKORG = null)
        {
            //if (string.IsNullOrWhiteSpace(I_BUKRS))
            //{
            //    return BadRequest(new { Error = "Debe enviar el parámetro I_BUKRS (compañía)." });
            //}

            string companiaNombre = I_VKORG switch
            {
                "1600" => "ROCIO S.A.C",
                "1700" => "AVIAGEN S.A.C",
                _ => "Compañía desconocida"
            };

            //string tipoConsulta = (I_FECCREA_I != null && I_FECCREA_F != null)
            //     ? "Creado"
            //     : (I_FECMOD_I != null && I_FECMOD_F != null)
            //     ? "Modificado"
            //     : "Ingresar una Fecha de Creacion o Modificacion";

            // Nueva URL base de la API
            string baseUrl = "https://sap-qas-genesysavi.rocio.com.pe/sap-api/api/ObtenerAsignacionClientesController";

            // Construcción de la URL con el parámetro I_VKORG
            string apiUrl = $"{baseUrl}?I_VKORG={I_VKORG}";

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

                var materiales = materialesWrapper?.Data?.T_ASIG_CLIENT;

                if (materiales == null || materiales.Count == 0)
                {
                    return Ok(new
                    {
                        Mensaje = "No se encontraron Clientes.",
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
                        OrgVenta = companiaNombre,
                        Cliente = reader["KUNNR"].ToString().TrimStart('0'),
                        CanalDistribucion = reader["VKORG"].ToString(),
                        Sector = "",
                        FuncionSocio = "",
                        InterlocutorComercial = ""
                    });
                }

                return Ok(new
                {
                    Mensaje = "Clientes importados e insertados correctamente desde la API externa.",
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

                    string query = @"SELECT ID, KUNNR, VKORG, VTWEG, SPART, PARVW, KTONR, DEFPA
                             FROM AMTRO_TA_TEMPORAL_CLIENTES_SAP";
                    using var cmd = new SqlCommand(query, connection);
                    using var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        materialesExistentes.Add(new MaterialResult
                        {
                            ID = reader["ID"].ToString(),
                            OrgVenta = companiaNombre,
                            Cliente = reader["KUNNR"].ToString().TrimStart('0'),
                            CanalDistribucion = reader["VKORG"].ToString(),
                            Sector = "",
                            FuncionSocio = "",
                            InterlocutorComercial = ""
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
                INSERT INTO AMTRO_TA_TEMPORAL_CLIENTES_SAP 
                (KUNNR, VKORG, VTWEG, SPART, PARVW, KTONR, DEFPA)
                VALUES 
                (@KUNNR, @VKORG, @VTWEG, @SPART, @PARVW, @KTONR, @DEFPA)";

            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@KUNNR", ValidarTexto(item.KUNNR));
            cmd.Parameters.AddWithValue("@VKORG", ValidarTexto(item.VKORG));
            cmd.Parameters.AddWithValue("@VTWEG", ValidarTexto(item.VTWEG));
            cmd.Parameters.AddWithValue("@SPART", ValidarTexto(item.SPART));
            cmd.Parameters.AddWithValue("@PARVW", ValidarTexto(item.PARVW));
            cmd.Parameters.AddWithValue("@KTONR", ValidarTexto(item.KTONR));
            cmd.Parameters.AddWithValue("@DEFPA", ParseDecimal(item.DEFPA));

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
