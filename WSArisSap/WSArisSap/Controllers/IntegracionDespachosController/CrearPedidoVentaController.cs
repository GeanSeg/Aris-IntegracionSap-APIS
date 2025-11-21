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
public class CrearPedidoVentaController : ControllerBase
{
    public class NotConsOiItemCrearPedidoVenta
    {
        public string POSNR_DET { get; set; }
        public string MATNR_DET { get; set; }
        public string WERKS_DET { get; set; }
        public string DZMENG_DET { get; set; }
        public string DZIEME_DET { get; set; }
        public string PSTYV_DET { get; set; }
        public string NETWR_DET { get; set; }
        public string WAERK_DET { get; set; }
        public string PROFIT_CTR_DET { get; set; }
    }

    public class CrearPedidoVentaRequest
    {
        public string TP_TRAT_CAB { get; set; }
        public string VBELN_CAB { get; set; }
        public string AUART_CAB { get; set; }
        public string PURCH_NO_C_CAB { get; set; }
        public string PURCH_DATE_CAB { get; set; }
        public string VDATU_CAB { get; set; }
        public string CURRENCY_CAB { get; set; }
        public string VKORG_CAB { get; set; }
        public string VTWEG_CAB { get; set; }
        public string SPART_CAB { get; set; }
        public string VKGRP_CAB { get; set; }
        public string VKBUR_CAB { get; set; }
        public string ZTERM_CAB { get; set; }
        public string AUGRU_CAB { get; set; }
        public string BZIRK_CAB { get; set; }
        public string KUNNR_CAB { get; set; }
        public string KUNWE_CAB { get; set; }
        public string NUM_PED_ARIS_CAB { get; set; }
        public string USER_CREA_ARIS_CAB { get; set; }
        public string CREA_ORG_ARIS_CAB { get; set; }
        public string USER_EDIT_ARIS_CAB { get; set; }
        public List<NotConsOiItemCrearPedidoVenta> Items { get; set; }
    }

    private readonly IConfiguration _configuration;

    public CrearPedidoVentaController(IConfiguration configuration)
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

    private string FormatKnnr(string KnnrInput)
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


    [HttpPost("CrearPedidoVentaController")]
    public async Task<IActionResult> CrearPedidoVenta([FromBody] CrearPedidoVentaRequest request)
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
                string TP_TRAT_CAB = string.IsNullOrEmpty(request.TP_TRAT_CAB) ? "" : request.TP_TRAT_CAB;
                string VBELN_CAB = string.IsNullOrEmpty(request.VBELN_CAB) ? "" : request.VBELN_CAB;
                string AUART_CAB = string.IsNullOrEmpty(request.AUART_CAB) ? "" : request.AUART_CAB;
                string PURCH_NO_C_CAB = string.IsNullOrEmpty(request.PURCH_NO_C_CAB) ? "" : request.PURCH_NO_C_CAB;
                string PURCH_DATE_CAB = string.IsNullOrEmpty(request.PURCH_DATE_CAB) ? "" : request.PURCH_DATE_CAB;
                string VDATU_CAB = string.IsNullOrEmpty(request.VDATU_CAB) ? "" : request.VDATU_CAB;
                string CURRENCY_CAB = string.IsNullOrEmpty(request.CURRENCY_CAB) ? "" : request.CURRENCY_CAB;
                string VKORG_CAB = string.IsNullOrEmpty(request.VKORG_CAB) ? "" : request.VKORG_CAB;
                string VTWEG_CAB = string.IsNullOrEmpty(request.VTWEG_CAB) ? "" : request.VTWEG_CAB;
                string SPART_CAB = string.IsNullOrEmpty(request.SPART_CAB) ? "" : request.SPART_CAB;
                string VKGRP_CAB = string.IsNullOrEmpty(request.VKGRP_CAB) ? "" : request.VKGRP_CAB;
                string VKBUR_CAB = string.IsNullOrEmpty(request.VKBUR_CAB) ? "" : request.VKBUR_CAB;
                string ZTERM_CAB = string.IsNullOrEmpty(request.ZTERM_CAB) ? "" : request.ZTERM_CAB;
                string AUGRU_CAB = string.IsNullOrEmpty(request.AUGRU_CAB) ? "" : request.AUGRU_CAB;
                string BZIRK_CAB = string.IsNullOrEmpty(request.BZIRK_CAB) ? "" : request.BZIRK_CAB;
                string KUNNR_CAB = string.IsNullOrEmpty(request.KUNNR_CAB) ? "" : request.KUNNR_CAB;
                string KUNWE_CAB = string.IsNullOrEmpty(request.KUNWE_CAB) ? "" : request.KUNWE_CAB;
                string NUM_PED_ARIS_CAB = string.IsNullOrEmpty(request.NUM_PED_ARIS_CAB) ? "" : request.NUM_PED_ARIS_CAB;
                string USER_CREA_ARIS_CAB = string.IsNullOrEmpty(request.USER_CREA_ARIS_CAB) ? "" : request.USER_CREA_ARIS_CAB;
                string CREA_ORG_ARIS_CAB = string.IsNullOrEmpty(request.CREA_ORG_ARIS_CAB) ? "" : request.CREA_ORG_ARIS_CAB;
                string USER_EDIT_ARIS_CAB = string.IsNullOrEmpty(request.USER_EDIT_ARIS_CAB) ? "" : request.USER_EDIT_ARIS_CAB;

                // Llamada a la función de SAP
                var result = await context.CallFunction("ZSD_FM_CREA_PED_VENTA",
                    Input: f => f.SetStructure("ES_CAB_PED", s => s
                                   .SetField("TP_TRAT", TP_TRAT_CAB)
                                   .SetField("VBELN", VBELN_CAB)
                                   .SetField("AUART", AUART_CAB)
                                   .SetField("PURCH_NO_C", PURCH_NO_C_CAB)
                                //.SetField("PURCH_DATE", string.IsNullOrEmpty(PURCH_DATE_CAB) ? default(DateTime) : DateTime.ParseExact(PURCH_DATE_CAB, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture))
                                .SetField("PURCH_DATE", string.IsNullOrEmpty(PURCH_DATE_CAB) ? DateTime.Now : DateTime.ParseExact(PURCH_DATE_CAB, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture))
                                .SetField("VDATU", string.IsNullOrEmpty(VDATU_CAB) ? DateTime.Now : DateTime.ParseExact(VDATU_CAB, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture))
                                   .SetField("CURRENCY", CURRENCY_CAB)
                                   .SetField("VKORG", VKORG_CAB)
                                   .SetField("VTWEG", VTWEG_CAB)
                                   .SetField("SPART", SPART_CAB)
                                   .SetField("VKGRP", VKGRP_CAB)
                                   .SetField("VKBUR", VKBUR_CAB)
                                   .SetField("ZTERM", ZTERM_CAB)
                                   .SetField("AUGRU", AUGRU_CAB)
                                   .SetField("BZIRK", BZIRK_CAB)
                                   .SetField("KUNNR", FormatKnnr(KUNNR_CAB))
                                   .SetField("KUNWE", FormatKnnr(KUNWE_CAB))
                                   .SetField("NUM_PED_ARIS", NUM_PED_ARIS_CAB)
                                   .SetField("USER_CREA_ARIS", USER_CREA_ARIS_CAB)
                                   .SetField("CREA_ORG_ARIS", CREA_ORG_ARIS_CAB)
                                   .SetField("USER_EDIT_ARIS", USER_EDIT_ARIS_CAB))

                        .SetTable("T_DET_PED", request.Items, (structure, item) => structure
                            .SetField("POSNR", item.POSNR_DET)
                            .SetField("MATNR", FormatMatnr(item.MATNR_DET))
                            .SetField("WERKS", item.WERKS_DET)
                            .SetField("DZMENG", string.IsNullOrWhiteSpace(item.DZMENG_DET) ? 0.000m : Convert.ToDecimal(item.DZMENG_DET, System.Globalization.CultureInfo.InvariantCulture))
                            .SetField("DZIEME", item.DZIEME_DET)
                            .SetField("PSTYV", item.PSTYV_DET)
                            .SetField("NETWR", string.IsNullOrWhiteSpace(item.NETWR_DET) ? 0.000m : Math.Round(Convert.ToDecimal(item.NETWR_DET, System.Globalization.CultureInfo.InvariantCulture), 9))
                            .SetField("WAERK", item.WAERK_DET)
                            .SetField("PROFIT_CTR", item.PROFIT_CTR_DET)),
                    Output: f => (
                        from E_VBELN in f.GetField<string>("E_VBELN")
                        from T_STATUS in f.MapTable("T_STATUS", r =>
                                from VBELN in r.GetField<string>("VBELN")
                                from POSNR in r.GetField<string>("POSNR")
                                from MATNR in r.GetField<string>("MATNR")
                                from ABGRU in r.GetField<string>("ABGRU")
                                from BEZEI in r.GetField<string>("BEZEI")
                                from GBSTA in r.GetField<string>("GBSTA")
                                from GBSTA_T in r.GetField<string>("GBSTA_T")
                                from LFGSA in r.GetField<string>("LFGSA")
                                from LFGSA_T in r.GetField<string>("LFGSA_T")
                                from LFSTA in r.GetField<string>("LFSTA")
                                from LFSTA_T in r.GetField<string>("LFSTA_T")
                                select new
                                {
                                        VBELN,  
                                        POSNR,  
                                        MATNR,  
                                        ABGRU,  
                                        BEZEI,  
                                        GBSTA,  
                                        GBSTA_T,
                                        LFGSA,  
                                        LFGSA_T,
                                        LFSTA,  
                                        LFSTA_T,
                                })
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
                            E_VBELN,
                            T_STATUS,
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
