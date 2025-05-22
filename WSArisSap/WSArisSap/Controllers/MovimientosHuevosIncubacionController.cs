using Dbosoft.YaNco;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

[ApiController]
[Route("api/")]
public class MovimientosHuevosIncubacionController : ControllerBase
{
    public class NotConsOiItem
    {
        public string MATNR { get; set; }
        public string WERKS { get; set; }
        public string LGORT { get; set; }
        public string CHARG { get; set; }
        public string ENTRY_QNT { get; set; }
        public string ENTRY_UOM { get; set; }
    }

    private readonly IConfiguration _configuration;

    public MovimientosHuevosIncubacionController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("MovimientosHuevosIncubacion")]
    public async Task<IActionResult> MovimientosHuevosIncubacion(
        string MATNR = "",
        string WERKS = "",
        string PLWERK = "",
        string VERID = "",
        string BUDAT = "",
        string BLDAT = "",
        string REFMG = "",
        string ERFME = "",
        string USER_CREA_ARIS = "",
        string USER_MODIF_ARIS = "",
        string matnr = "",
        string werks = "",
        string lgort = "",
        string charg = "",
        string entry_qnt = "",
        string entry_uom = ""
        )
    {
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
            {"passwd","Rocio*25"},
            {"lang", "ES"}
        };

        var connectionBuilder = new ConnectionBuilder(settings);
        var connFunc = connectionBuilder.Build();

        using (var context = new RfcContext(connFunc))
        {
            try
            {
                MATNR = string.IsNullOrEmpty(MATNR) ? "" : MATNR;
                WERKS = string.IsNullOrEmpty(WERKS) ? "" : WERKS;
                PLWERK = string.IsNullOrEmpty(PLWERK) ? "" : PLWERK;
                VERID = string.IsNullOrEmpty(VERID) ? "" : VERID;
                BUDAT = string.IsNullOrEmpty(BUDAT) ? "" : BUDAT;
                BLDAT = string.IsNullOrEmpty(BLDAT) ? "" : BLDAT;
                REFMG = string.IsNullOrEmpty(REFMG) ? "" : REFMG;
                ERFME = string.IsNullOrEmpty(ERFME) ? "" : ERFME;
                USER_CREA_ARIS = string.IsNullOrEmpty(USER_CREA_ARIS) ? "" : USER_CREA_ARIS;
                USER_MODIF_ARIS = string.IsNullOrEmpty(USER_MODIF_ARIS) ? "" : USER_MODIF_ARIS;
                matnr = string.IsNullOrEmpty(matnr) ? "" : matnr;
                werks = string.IsNullOrEmpty(werks) ? "" : werks;
                lgort = string.IsNullOrEmpty(lgort) ? "" : lgort;
                charg = string.IsNullOrEmpty(charg) ? "" : charg;
                entry_qnt = string.IsNullOrEmpty(entry_qnt) ? "" : entry_qnt;
                entry_uom = string.IsNullOrEmpty(entry_uom) ? "" : entry_uom;


                var items = new List<NotConsOiItem>
{
                            new NotConsOiItem
                            {
                                MATNR = matnr,
                                WERKS = werks,
                                LGORT = lgort,
                                CHARG = charg,
                                ENTRY_QNT = entry_qnt,
                                ENTRY_UOM = entry_uom
                            }

                    };
                var result = await context.CallFunction("ZPP_FM_NOTIF_CONS_ORDEN_FAB", // Ajusta el nombre si es diferente
                    Input: f => f
                        .SetStructure("IST_ORDER_FAB", s => s
                            .SetField("MATNR", MATNR)
                            .SetField("WERKS", WERKS)
                            .SetField("PLWERK", PLWERK)
                            .SetField("VERID", VERID)
                            .SetField("BUDAT", DateTime.ParseExact(BUDAT, "dd.MM.yyyy", null))
                            .SetField("BLDAT", DateTime.ParseExact(BLDAT, "dd.MM.yyyy", null))
                            .SetField("REFMG", string.IsNullOrWhiteSpace(REFMG) ? 0.000m : Convert.ToDecimal(REFMG, System.Globalization.CultureInfo.InvariantCulture))
                            .SetField("ERFME", ERFME)
                            .SetField("USER_CREA_ARIS", USER_CREA_ARIS)
                            .SetField("USER_MODIF_ARIS", USER_MODIF_ARIS)
                        )
                        .SetTable("IT_COMPONENTS", items,
                                    (structure, items) => structure
                            .SetField("MATNR", items.MATNR)
                            .SetField("WERKS", items.WERKS)
                            .SetField("LGORT", items.LGORT)
                            .SetField("CHARG", items.CHARG)
                            .SetField("ENTRY_QNT", string.IsNullOrWhiteSpace(items.ENTRY_QNT) ? 0.000m : Convert.ToDecimal(items.ENTRY_QNT, System.Globalization.CultureInfo.InvariantCulture))
                            .SetField("ENTRY_UOM", items.ENTRY_UOM)
                        ),
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