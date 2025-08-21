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
public class CrearPedidoTransladoController : ControllerBase
{
    public class NotConsOiItemCrearPedidoTranslado
    {
        public string EBELP_DET { get; set; }
        public string MATNR_DET { get; set; }
        public string WERKS_DET { get; set; }
        public string LGORTV_DET { get; set; }
        public string QUANTITY_DET { get; set; }
        public string PO_UNIT_DET { get; set; }
        public string AFNAM_DET { get; set; }
        public string DEL_DATCAT_EXT_DET { get; set; }
        public string DELIVERY_DATE_DET { get; set; }
        public string NUM_PED_ARIS_DET { get; set; }
        public string USER_CREA_ARIS_DET { get; set; }
        public string CREA_ORG_ARIS_DET { get; set; }
        public string USER_EDIT_ARIS_DET { get; set; }
    }

    public class CrearPedidoTransladoRequest
    {
        public string TP_TRAT_CAB { get; set; }
        public string EBELN_CAB { get; set; }
        public string BUKRS_CAB { get; set; }
        public string EKORG_CAB { get; set; }
        public string BKGRP_CAB { get; set; }
        public string RESWK_CAB { get; set; }
        public string ZZ_LUG_ENTR_CAB { get; set; }
        public List<NotConsOiItemCrearPedidoTranslado> Items { get; set; }
    }

    private readonly IConfiguration _configuration;

    public CrearPedidoTransladoController(IConfiguration configuration)
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


    [HttpPost("CrearPedidoTransladoController")]
    public async Task<IActionResult> CrearPedidoVenta([FromBody] CrearPedidoTransladoRequest request)
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

                
                string TP_TRAT_CAB = string.IsNullOrEmpty(request.TP_TRAT_CAB) ? "" : request.TP_TRAT_CAB;
                string EBELN_CAB = string.IsNullOrEmpty(request.EBELN_CAB) ? "" : request.EBELN_CAB;
                string BUKRS_CAB = string.IsNullOrEmpty(request.BUKRS_CAB) ? "" : request.BUKRS_CAB;
                string EKORG_CAB = string.IsNullOrEmpty(request.EKORG_CAB) ? "" : request.EKORG_CAB;
                string BKGRP_CAB = string.IsNullOrEmpty(request.BKGRP_CAB) ? "" : request.BKGRP_CAB;
                string RESWK_CAB = string.IsNullOrEmpty(request.RESWK_CAB) ? "" : request.RESWK_CAB;
                string ZZ_LUG_ENTR_CAB = string.IsNullOrEmpty(request.ZZ_LUG_ENTR_CAB) ? "" : request.ZZ_LUG_ENTR_CAB;


                // Llamada a la función de SAP
                var result = await context.CallFunction("ZSD_FM_CREA_PED_TRAS",
                    Input: f => f.SetStructure("ES_CAB_PED_TR", s => s
                                   .SetField("TP_TRAT", TP_TRAT_CAB)
                                   .SetField("EBELN", EBELN_CAB)
                                   .SetField("BUKRS", BUKRS_CAB)
                                   .SetField("EKORG", EKORG_CAB)
                                   .SetField("BKGRP", BKGRP_CAB)
                                   .SetField("RESWK", RESWK_CAB)
                                   .SetField("ZZ_LUG_ENTR", ZZ_LUG_ENTR_CAB)
                               )

                        .SetTable("T_DET_PED_TR", request.Items, (structure, item) => structure
                        .SetField("EBELP", item.EBELP_DET)
                        .SetField("MATNR", FormatMatnr( item.MATNR_DET))
                        .SetField("WERKS", item.WERKS_DET)
                        .SetField("LGORT", item.LGORTV_DET)
                        .SetField("QUANTITY", item.QUANTITY_DET)
                        .SetField("PO_UNIT", item.PO_UNIT_DET == "UN" ? "ST" : item.PO_UNIT_DET)
                        .SetField("AFNAM", item.AFNAM_DET)
                        .SetField("DEL_DATCAT_EXT", item.DEL_DATCAT_EXT_DET)
                        .SetField("DELIVERY_DATE", item.DELIVERY_DATE_DET)
                        .SetField("NUM_PED_ARIS", item.NUM_PED_ARIS_DET)
                        .SetField("USER_CREA_ARIS", item.USER_CREA_ARIS_DET)
                        .SetField("CREA_ORG_ARIS", item.CREA_ORG_ARIS_DET)
                        .SetField("USER_EDIT_ARIS", item.USER_EDIT_ARIS_DET)),
                    Output: f => (
                        from E_EBELN in f.GetField<string>("E_EBELN")
                        
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
                            E_EBELN,
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
