using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dbosoft.YaNco.TypeMapping;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.InteropServices;

namespace WSpruebaArisSap.Controllers
{
    [ApiController]
    [Route("api/")]
    public class ObtenerMaterialController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ObtenerMaterialController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("ObtenerMaterialController")]
        public async Task<IActionResult> GetObtenerMaterial(string FEC_CREA_INICIO, string FEC_CREA_FIN, string TIPO_DOCU, string SOCIEDAD, string NRO_PEDIDO = "")
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
                {"lang", "EN"}
            };

            var connectionBuilder = new ConnectionBuilder(settings);
            var connFunc = connectionBuilder.Build();

            using (var context = new RfcContext(connFunc))
            {
                try
                {

                    NRO_PEDIDO = string.IsNullOrEmpty(NRO_PEDIDO) ? "" : NRO_PEDIDO;
                
                    var result = await context.CallFunction("ZMM_PEDIDO_TRASLADO",
                        Input: f => f
                                        .SetField("FEC_CREA_INICIO", DateTime.ParseExact(FEC_CREA_INICIO, "dd.MM.yyyy", null))
                                        .SetField("FEC_CREA_FIN", DateTime.ParseExact(FEC_CREA_FIN, "dd.MM.yyyy", null))
                                        .SetField("TIPO_DOCU", TIPO_DOCU)
                                        .SetField("NRO_PEDIDO", NRO_PEDIDO)
                                        .SetField("SOCIEDAD", SOCIEDAD),


                        Output: f => (
                    from ET_CABECERA in f.MapTable("ET_CABECERA", s =>
                        from EBELN in s.GetField<string>("EBELN")
                        from BSART in s.GetField<string>("BSART")
                        from CENTRO_SUMINISTRADOR in s.GetField<string>("CENTRO_SUMINISTRADOR")
                        from BEDAT in s.GetField<DateTime>("BEDAT")
                        from EKORG in s.GetField<string>("EKORG")
                        from EKGRP in s.GetField<string>("EKGRP")
                        from BUKRS in s.GetField<string>("BUKRS")
                        from AEDAT in s.GetField<DateTime>("AEDAT")
                        from ERNAM in s.GetField<string>("ERNAM")
                        from FEC_MODIF in s.GetField<string>("FEC_MODIF")
                        from USU_MODIF in s.GetField<string>("USU_MODIF")
                        from ZZ_LUG_ENTR in s.GetField<string>("ZZ_LUG_ENTR")
                        from CAMPO_NO_UTILIZADO in s.GetField<string>("CAMPO_NO_UTILIZADO")
                        from SUMINISTRO_COMPLETO in s.GetField<string>("SUMINISTRO_COMPLETO")
                        from MENSAJE_EM in s.GetField<string>("MENSAJE_EM")
                        select new
                        {
                            EBELN,
                            BSART,
                            CENTRO_SUMINISTRADOR,
                            BEDAT,
                            EKORG,
                            EKGRP,
                            BUKRS,
                            AEDAT,
                            ERNAM,
                            FEC_MODIF,
                            USU_MODIF,
                            ZZ_LUG_ENTR,
                            CAMPO_NO_UTILIZADO,
                            SUMINISTRO_COMPLETO,
                            MENSAJE_EM,
                        })

                    from ET_DETALLE in f.MapTable("ET_DETALLE", s =>
                        from EBELN in s.GetField<decimal>("EBELN")
                        from EBELP in s.GetField<string>("EBELP")
                        from PSTYP in s.GetField<string>("PSTYP")
                        from TXZ01 in s.GetField<string>("TXZ01")
                        from MENGE in s.GetField<string>("MENGE")
                        select new
                        {
                            EBELN,
                            EBELP,
                            PSTYP,
                            TXZ01,
                            MENGE
                        })

                    from ET_HISTORIAL in f.MapTable("ET_HISTORIAL", s =>
                        from EBELN in s.GetField<string>("EBELN")
                        from EBELP in s.GetField<string>("EBELP")
                        from BWART in s.GetField<string>("BWART")
                        from BELNR in s.GetField<string>("BELNR")
                        select new
                        {
                            EBELN,
                            EBELP,
                            BWART,
                            BELNR
                        })

                    select new
                    {
                        ET_CABECERA,
                        ET_DETALLE,
                        ET_HISTORIAL
                    })
            );
                    return Ok(new
                    {
                        Data = result.Case
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Error = ex.Message });
                }
            }
        }
    }
}