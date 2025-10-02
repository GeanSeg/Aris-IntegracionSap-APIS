using Dbosoft.YaNco;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/")]
public class MovimientosHuevosIncubacionController : ControllerBase
{
    public class NotConsOiItemMovimientosHuevos
    {
        public string MATNR_DET { get; set; }
        public string WERKS_DET { get; set; }
        public string LGORT_DET { get; set; }
        public string CHARG_DET { get; set; }
        public string ENTRY_QNT_DET { get; set; }
        public string ENTRY_UOM_DET { get; set; }
    }

    public class MovimientosHuevosRequest
    {
        public string MATNR_CAB { get; set; }
        public string WERKS_CAB { get; set; }
        public string PLWERK_CAB { get; set; }
        public string VERID_CAB { get; set; }
        public string BUDAT_CAB { get; set; }
        public string BLDAT_CAB { get; set; }
        public string REFMG_CAB { get; set; }
        public string ERFME_CAB { get; set; }
        public string USER_CREA_ARIS_CAB { get; set; }
        public string USER_MODIF_ARIS_CAB { get; set; }
        public List<NotConsOiItemMovimientosHuevos> Items { get; set; }
    }

    private readonly IConfiguration _configuration;

    public MovimientosHuevosIncubacionController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string FormatMatnr(string matnrInput)
    {
        const int matnrLength = 18;

        // Si el valor es nulo o vacío, devolver cadena vacía
        if (string.IsNullOrWhiteSpace(matnrInput))
        {
            return "";
        }

        string trimmedMatnr = matnrInput.Trim();

        // Verificar si el valor comienza con una letra
        if (char.IsLetter(trimmedMatnr[0]))
        {
            return trimmedMatnr;
        }

        // Verificar si el valor es numérico
        if (trimmedMatnr.All(char.IsDigit))
        {
     
            string formattedMatnr = trimmedMatnr.PadLeft(matnrLength, '0');
            return formattedMatnr;
        }
        return trimmedMatnr;
    }

    [HttpPost("MovimientosHuevosIncubacion")]
    public async Task<IActionResult> MovimientosHuevosIncubacion([FromBody] MovimientosHuevosRequest request)
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

        var sapSettings = _configuration.GetSection("SapSettings");

        var settings = new Dictionary<string, string>
            {
                {"ashost", sapSettings["AppServerHost"]},
                {"sysnr", sapSettings["SystemNumber"]},
                {"client", sapSettings["Client"]},
                {"user", sapSettings["User"]},
                {"passwd", sapSettings["Password"]},
                {"lang", sapSettings["Language"]}
            };

        var connectionBuilder = new ConnectionBuilder(settings);
        var connFunc = connectionBuilder.Build();

        using (var context = new RfcContext(connFunc))
        {
            try
            {
                // Asignación de valores predeterminados para los campos
                string MATNR_CAB = string.IsNullOrEmpty(request.MATNR_CAB) ? "" : request.MATNR_CAB;
                string WERKS_CAB = string.IsNullOrEmpty(request.WERKS_CAB) ? "" : request.WERKS_CAB;
                string PLWERK_CAB = string.IsNullOrEmpty(request.PLWERK_CAB) ? "" : request.PLWERK_CAB;
                string VERID_CAB = string.IsNullOrEmpty(request.VERID_CAB) ? "" : request.VERID_CAB;
                string BUDAT_CAB = string.IsNullOrEmpty(request.BUDAT_CAB) ? "" : request.BUDAT_CAB;
                string BLDAT_CAB = string.IsNullOrEmpty(request.BLDAT_CAB) ? "" : request.BLDAT_CAB;
                string REFMG_CAB = string.IsNullOrEmpty(request.REFMG_CAB) ? "" : request.REFMG_CAB;
                string ERFME_CAB = string.IsNullOrEmpty(request.ERFME_CAB) ? "" : request.ERFME_CAB;
                string USER_CREA_ARIS_CAB = string.IsNullOrEmpty(request.USER_CREA_ARIS_CAB) ? "" : request.USER_CREA_ARIS_CAB;
                string USER_MODIF_ARIS_CAB = string.IsNullOrEmpty(request.USER_MODIF_ARIS_CAB) ? "" : request.USER_MODIF_ARIS_CAB;

                // Llamada a la función de SAP
                var result = await context.CallFunction("ZPP_FM_NOTIF_CONS_ORDEN_FAB",
                    Input: f => f.SetStructure("IST_ORDER_FAB", s => s
                            .SetField("MATNR", FormatMatnr(MATNR_CAB))
                            .SetField("WERKS", WERKS_CAB)
                            .SetField("PLWERK", PLWERK_CAB)
                            .SetField("VERID", VERID_CAB)
                            .SetField("BUDAT", DateTime.ParseExact(BUDAT_CAB, "dd.MM.yyyy", null))
                            .SetField("BLDAT", DateTime.ParseExact(BLDAT_CAB, "dd.MM.yyyy", null))
                            .SetField("REFMG", string.IsNullOrWhiteSpace(REFMG_CAB) ? 0.000m : Convert.ToDecimal(REFMG_CAB, System.Globalization.CultureInfo.InvariantCulture))
                            .SetField("ERFME", ERFME_CAB == "UN" ? "ST" : ERFME_CAB)
                            .SetField("USER_CREA_ARIS", USER_CREA_ARIS_CAB)
                            .SetField("USER_MODIF_ARIS", USER_MODIF_ARIS_CAB))
                        .SetTable("IT_COMPONENTS", request.Items, (structure, item) => structure
                            .SetField("MATNR", item.MATNR_DET)
                            .SetField("WERKS", item.WERKS_DET)
                            .SetField("LGORT", item.LGORT_DET)
                            .SetField("CHARG", item.CHARG_DET)
                            .SetField("ENTRY_QNT", string.IsNullOrWhiteSpace(item.ENTRY_QNT_DET) ? 0.000m : Convert.ToDecimal(item.ENTRY_QNT_DET, System.Globalization.CultureInfo.InvariantCulture))
                            .SetField("ENTRY_UOM", item.ENTRY_UOM_DET == "UN" ? "ST" : item.ENTRY_UOM_DET)),
                    Output: f => (
                        from E_NUMB_NOTIF in f.GetField<string>("E_NUMB_NOTIF")
                        from E_DOC_MATNR in f.GetField<string>("E_DOC_MATNR")
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
                            E_NUMB_NOTIF,
                            E_DOC_MATNR,
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
