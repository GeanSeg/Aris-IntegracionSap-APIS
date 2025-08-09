using Dbosoft.YaNco;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Net.NetworkInformation;
using static System.Runtime.InteropServices.JavaScript.JSType;

[ApiController]
[Route("api/")]
public class CrearEntregaVentaController : ControllerBase
{
    public class NotConsOiItemCrearEntregaVenta
    {
        public string POSNR_DET { get; set; }
        public string KWMENG_DET { get; set; }
        public string VRKME_DET { get; set; }
        public string CHARG_DET { get; set; }


    }

        public class CrearEntregaVentaRequest
    {
        public string DATBI_CAB { get; set; }
        public string VBELN_CAB { get; set; }
        public string BUDAT_CAB { get; set; }
        public List<NotConsOiItemCrearEntregaVenta> Items { get; set; }
    }

    private readonly IConfiguration _configuration;

    public CrearEntregaVentaController(IConfiguration configuration)
    {
        _configuration = configuration;
    }



    private string FormatVbeln(string KnnrInput)
    {
        const int knnrLength = 10;

        // Si el valor es nulo o vacío, devolver cadena vacía
        if (string.IsNullOrWhiteSpace(KnnrInput))
        {
            return "";
        }

        // Limpiar espacios en blanco
        string trimmedKnnr = KnnrInput.Trim();

        // Verificar si el valor comienza con una letra
        if (char.IsLetter(trimmedKnnr[0]))
        {

            return trimmedKnnr;
        }

        // Verificar si el valor es numérico
        if (trimmedKnnr.All(char.IsDigit))
        {
            // Formatear con ceros a la izquierda hasta 18 caracteres
            string formattedKnnr = trimmedKnnr.PadLeft(knnrLength, '0');

            return formattedKnnr;
        }

        // Si no es numérico ni comienza con letra, devolver tal cual con log de advertencia

        return trimmedKnnr;
    }


    [HttpPost("CrearEntregaVentaController")]
    public async Task<IActionResult> CrearEntregaVenta([FromBody] CrearEntregaVentaRequest request)
    {
        // Validación de los campos obligatorios
        if (request == null || request.Items == null || request.Items.Count == 0)
        {
            return BadRequest(new { Error = "La solicitud y la lista de ítems no pueden estar vacías." });
        }

        string basePath = Path.Combine(AppContext.BaseDirectory, "Recursos");
        NativeLibrary.Load(Path.Combine(basePath, "icuuc50.dll"));
        NativeLibrary.Load(Path.Combine(basePath, "icudt50.dll"));
        NativeLibrary.Load(Path.Combine(basePath, "icuin50.dll"));

        var settings = new Dictionary<string, string>
        {
            {"ashost", "10.45.4.163"},
            {"sysnr", "01"},
            {"client", "200"},
            {"user", "USU_INTEGRAC"},
            {"passwd", "Rocio*25"},
            {"lang", "ES"}
        };

        var connectionBuilder = new ConnectionBuilder(settings);
        var connFunc = connectionBuilder.Build();

        using (var context = new RfcContext(connFunc))
        {
            try
            {
                // Asignación de valores predeterminados para los campos
                string DATBI_CAB = string.IsNullOrEmpty(request.DATBI_CAB) ? "" : request.DATBI_CAB;
                string VBELN_CAB = string.IsNullOrEmpty(request.VBELN_CAB) ? "" : request.VBELN_CAB;
                string BUDAT_CAB = string.IsNullOrEmpty(request.BUDAT_CAB) ? "" : request.BUDAT_CAB;
 

                // Llamada a la función de SAP
                var result = await context.CallFunction("ZSD_FM_CREA_ENTREGA",
                    Input: f => f.SetStructure("ES_CAB_DESPA", s => s
                                   .SetField("DATBI", DateTime.ParseExact(DATBI_CAB, "dd.MM.yyyy", null))
                                   .SetField("VBELN", FormatVbeln(VBELN_CAB))
                                   .SetField("BUDAT", DateTime.ParseExact(DATBI_CAB, "dd.MM.yyyy", null)))
                        .SetTable("T_DET_DESP", request.Items, (structure, item) => structure
                            .SetField("POSNR", item.POSNR_DET)
                            .SetField("KWMENG", item.KWMENG_DET)
                            .SetField("VRKME", item.VRKME_DET == "UN" ? "ST" : item.VRKME_DET)
                            .SetField("CHARG", item.CHARG_DET)),


                    Output: f => (
                        from E_VBELN_ENT in f.GetField<string>("E_VBELN_ENT")
                        from E_MBLNR in f.GetField<string>("E_MBLNR")
                        from T_RETURN in f.MapTable("T_RETURN", r =>
                                    from TYPE in r.GetField<string>("TYPE")
                                    from ID in r.GetField<string>("ID")
                                    from NUMBER in r.GetField<string>("NUMBER")
                                    from MESSAGE in r.GetField<string>("MESSAGE")
                                    from LOG_NO in r.GetField<string>("LOG_NO")
                                    from LOG_MSG_NO in r.GetField<decimal>("LOG_MSG_NO")
                                    from MESSAGE_V1 in r.GetField<string>("MESSAGE_V1")
                                    from MESSAGE_V2 in r.GetField<string>("MESSAGE_V2")
                                    from MESSAGE_V3 in r.GetField<string>("MESSAGE_V3")
                                    from MESSAGE_V4 in r.GetField<string>("MESSAGE_V4")
                                    from PARAMETER in r.GetField<string>("PARAMETER")
                                    from ROW in r.GetField<string>("ROW")
                                    from FIELD in r.GetField<string>("FIELD")
                                    from SYSTEM in r.GetField<string>("SYSTEM")
                                    select new
                                    {
                                        TYPE,
                                        ID,
                                        NUMBER,
                                        MESSAGE,
                                        LOG_NO,
                                        LOG_MSG_NO,
                                        MESSAGE_V1,
                                        MESSAGE_V2,
                                        MESSAGE_V3,
                                        MESSAGE_V4,
                                        PARAMETER,
                                        ROW,
                                        FIELD,
                                        SYSTEM
                                    })
                        select new
                        {
                            E_VBELN_ENT,
                            E_MBLNR,
                            T_RETURN
                        }
                    )
                );

               return Ok(new { Data = result.Case });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
