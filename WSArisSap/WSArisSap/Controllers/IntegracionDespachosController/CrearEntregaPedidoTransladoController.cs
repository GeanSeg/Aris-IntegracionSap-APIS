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
public class CrearEntregaPedidoTransladoController : ControllerBase
{
    public class NotConsOiItemCrearEntregaPedidoTransladoController
    {
        public string EBELP_DET       { get; set; }
        public string LGORT_DET       { get; set; }
        public string CHARG_DET       { get; set; }
        //public string MATERIAL_DET    { get; set; }
        public string DLV_QTY_DET     { get; set; }
        public string SALES_UNIT_DET { get; set; }
    }

    public class CrearEntregaPedidoTransladoControllerRequest
    {
        public string EBELN_CAB { get; set; }
        public string BUDAT_CAB { get; set; }
        public string I_CONTAB_CAB { get; set; }
        
        public List<NotConsOiItemCrearEntregaPedidoTransladoController> Items { get; set; }
    }

    private readonly IConfiguration _configuration;

    public CrearEntregaPedidoTransladoController(IConfiguration configuration)
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


    [HttpPost("CrearEntregaPedidoTransladoController")]
    public async Task<IActionResult> CrearPedidoVenta([FromBody] CrearEntregaPedidoTransladoControllerRequest request)
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
                string EBELN_CAB = string.IsNullOrEmpty(request.EBELN_CAB) ? "" : request.EBELN_CAB;
                string BUDAT_CAB = string.IsNullOrEmpty(request.BUDAT_CAB) ? "" : request.BUDAT_CAB;
                string I_CONTAB_CAB = string.IsNullOrEmpty(request.I_CONTAB_CAB) ? "" : request.I_CONTAB_CAB;
                
                // Llamada a la función de SAP
                var result = await context.CallFunction("ZSD_FM_CREA_ENTREGA_PED_TRAS",
                    Input: f => f.SetStructure("ES_CAB_PED_TRAS", s => s
                                   .SetField("EBELN", EBELN_CAB)
                                   .SetField("BUDAT", DateTime.ParseExact(BUDAT_CAB, "dd.MM.yyyy", null)))
                                   .SetField("I_CONTAB", I_CONTAB_CAB)

                        .SetTable("IT_DET_PED_TRAS", request.Items, (structure, item) => structure
                            .SetField("EBELP", item.EBELP_DET)
                            .SetField("LGORT", item.LGORT_DET)
                            .SetField("CHARG", item.CHARG_DET)
                            //.SetField("MATERIAL", FormatMatnr(item.MATERIAL_DET))
                            .SetField("DLV_QTY", item.DLV_QTY_DET)
                            .SetField("SALES_UNIT", item.SALES_UNIT_DET)),
                    Output: f => (
                        from E_VBELN in f.GetField<string>("E_VBELN")
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
                            E_VBELN,
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
