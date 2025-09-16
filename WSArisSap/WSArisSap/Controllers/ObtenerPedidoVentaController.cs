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
    public class ObtenerPedidoVentaController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ObtenerPedidoVentaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("ObtenerPedidoVentaController")]
        public async Task<IActionResult> GetObtenerPedidoVenta(string FEC_CREA_INICIO= "", string FEC_CREA_FIN = "", string NRO_PEDIDO = "", string SOCIEDAD = "")
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

                    FEC_CREA_INICIO = string.IsNullOrEmpty(FEC_CREA_INICIO) ? "" : FEC_CREA_INICIO;
                    FEC_CREA_FIN = string.IsNullOrEmpty(FEC_CREA_FIN) ? "" : FEC_CREA_FIN;
                    NRO_PEDIDO = string.IsNullOrEmpty(NRO_PEDIDO) ? "" : NRO_PEDIDO;
                    SOCIEDAD = string.IsNullOrEmpty(SOCIEDAD) ? "" : SOCIEDAD;





                    var result = await context.CallFunction("ZSD_PEDIDO_VENTA",
                        Input: f => f
                                        .SetField("FEC_CREA_INICIO", DateTime.ParseExact(FEC_CREA_INICIO,"dd.MM.yyyy", null))
                                        .SetField("FEC_CREA_FIN", DateTime.ParseExact(FEC_CREA_FIN,"dd.MM.yyyy", null))
                                        .SetField("NRO_PEDIDO", string.IsNullOrWhiteSpace(NRO_PEDIDO) ?"": "00000" + NRO_PEDIDO)//22154  no le ingrso nada
                                        .SetField("SOCIEDAD", SOCIEDAD),


                        Output: f => 
                        (
                    from ET_CABECERA in f.MapTable("ET_CABECERA", s =>
                         from BEZEI in s.GetField<string>("BEZEI")    // CHAR
                         from VBELN in s.GetField<string>("VBELN")
                         from KUNNR in s.GetField<string>("KUNNR")
                         from KUNWE in s.GetField<string>("KUNWE")
                         from BSTNK in s.GetField<string>("BSTNK")
                         from NETWR in s.GetField<string>("NETWR")
                         from WAERK in s.GetField<string>("WAERK")
                         from BSTDK in s.GetField<string>("BSTDK")
                         from VDATU in s.GetField<DateTime>("VDATU")
                         from VSBED in s.GetField<string>("VSBED")
                         from BRGEW in s.GetField<string>("BRGEW")
                         from GEWEI in s.GetField<string>("GEWEI")
                         from NTGEW in s.GetField<string>("NTGEW")
                         from AUART in s.GetField<string>("AUART")
                         from AUGRU in s.GetField<string>("AUGRU")
                         from VKORG in s.GetField<string>("VKORG")
                         from VTWEG in s.GetField<string>("VTWEG")    // CHAR
                         from SPART in s.GetField<string>("SPART")
                         from VKBUR in s.GetField<string>("VKBUR")
                         from VKGRP in s.GetField<string>("VKGRP")
                         from AUDAT in s.GetField<DateTime>("AUDAT")
                         from ERNAM in s.GetField<string>("ERNAM")
                         from AEDAT in s.GetField<string>("AEDAT")
                         from ERDAT in s.GetField<DateTime>("ERDAT")
                         from ERZET in s.GetField<DateTime>("ERZET")
                         from WAERK_3 in s.GetField<string>("WAERK_3")
                         from KURSK in s.GetField<string>("KURSK")
                         from KALSM in s.GetField<string>("KALSM")
                         from KONDA in s.GetField<string>("KONDA")
                         from PRSDT in s.GetField<string>("PRSDT")
                         from KDGRP in s.GetField<string>("KDGRP")
                         from BZIRK in s.GetField<string>("BZIRK")    // CHAR
                         from BRGEW_2 in s.GetField<string>("BRGEW_2")
                         from WAERK_2 in s.GetField<string>("WAERK_2")
                         from KUNRG_ANA in s.GetField<string>("KUNRG_ANA")
                         from ZTERM in s.GetField<string>("ZTERM")
                         from FKDAT in s.GetField<DateTime>("FKDAT")
                         from BUKRS_VF in s.GetField<string>("BUKRS_VF")
                         from AKPRZ in s.GetField<string>("AKPRZ")
                         from DDTEXT_GBSTK in s.GetField<string>("DDTEXT_GBSTK")
                         from DDTEXT_ABSTK in s.GetField<string>("DDTEXT_ABSTK")
                         from DDTEXT_LFSTK in s.GetField<string>("DDTEXT_LFSTK")
                         from DDTEXT_CMGST in s.GetField<string>("DDTEXT_CMGST")
                         from DDTEXT_FSSTK in s.GetField<string>("DDTEXT_FSSTK")
                         from KTGRD in s.GetField<string>("KTGRD")
                         from KTGRD_2 in s.GetField<string>("KTGRD_2")
                         from IMPUESTO in s.GetField<string>("IMPUESTO")
                         from PARVW in s.GetField<string>("PARVW")
                         from KUNNR_3 in s.GetField<string>("KUNNR_3")
                         from ASSIGNED_BP in s.GetField<string>("ASSIGNED_BP")
                         from NAME1 in s.GetField<string>("NAME1")
                         from STRAS in s.GetField<string>("STRAS")
                         from PSTLZ in s.GetField<string>("PSTLZ")
                         from ORT01 in s.GetField<string>("ORT01")
                         from ACTIVIDAD in s.GetField<string>("ACTIVIDAD")
                         from MOTIVO_RESCISION in s.GetField<string>("MOTIVO_RESCISION")
                         from DOCUMENTO_RESCISION in s.GetField<string>("DOCUMENTO_RESCISION")
                         select new
                        {
                             BEZEI,
                             VBELN,
                             KUNNR,
                             KUNWE,
                             BSTNK,
                             NETWR,
                             WAERK,
                             BSTDK,
                             VDATU,
                             VSBED,
                             BRGEW,
                             GEWEI,
                             NTGEW,
                             AUART,
                             AUGRU,
                             VKORG,
                             VTWEG,
                             SPART,
                             VKBUR,
                             VKGRP,
                             AUDAT,
                             ERNAM,
                             AEDAT,
                             ERDAT,
                             ERZET,
                             WAERK_3,
                             KURSK,
                             KALSM,
                             KONDA,
                             PRSDT,
                             KDGRP,
                             BZIRK,
                             BRGEW_2,
                             WAERK_2,
                             KUNRG_ANA,
                             ZTERM,
                             FKDAT,
                             BUKRS_VF,
                             AKPRZ,
                             DDTEXT_GBSTK,
                             DDTEXT_ABSTK,
                             DDTEXT_LFSTK,
                             DDTEXT_CMGST,
                             DDTEXT_FSSTK,
                             KTGRD,
                             KTGRD_2,
                             IMPUESTO,
                             PARVW,
                             KUNNR_3,
                             ASSIGNED_BP,
                             NAME1,
                             STRAS,
                             PSTLZ,
                             ORT01,
                             ACTIVIDAD,
                             MOTIVO_RESCISION,
                             DOCUMENTO_RESCISION
                         })

                    from ET_DETALLE in f.MapTable("ET_DETALLE", s =>
                      from VBELN in s.GetField<string>("VBELN")
                      from POSNR in s.GetField<string>("POSNR")
                      from MATNR in s.GetField<string>("MATNR")
                      from SGT_RCAT in s.GetField<string>("SGT_RCAT")
                      from KWMENG in s.GetField<string>("KWMENG")
                      from VRKME in s.GetField<string>("VRKME")
                      from ARKTX in s.GetField<string>("ARKTX")
                      from VPRGR in s.GetField<string>("VPRGR")
                      from PSTYV in s.GetField<string>("PSTYV")
                      from UEPOS in s.GetField<string>("UEPOS")
                      from ERDAT in s.GetField<string>("ERDAT")
                      from WERKS in s.GetField<string>("WERKS")
                      from BMENG in s.GetField<string>("BMENG")
                      from ETTYP in s.GetField<string>("ETTYP")
                      from BEDAE in s.GetField<string>("BEDAE")
                      from MBDAT in s.GetField<string>("MBDAT")
                      from KSCHL in s.GetField<string>("KSCHL")
                      from KBETR in s.GetField<string>("KBETR")
                      from WAERS in s.GetField<string>("WAERS")
                      from NETPR in s.GetField<string>("NETPR")
                      from KPEIN in s.GetField<string>("KPEIN")
                      from KMEIN in s.GetField<string>("KMEIN")
                      from NETWR in s.GetField<string>("NETWR")
                      from WAERK in s.GetField<string>("WAERK")
                      from PRODH in s.GetField<string>("PRODH")
                      from PRCTR in s.GetField<string>("PRCTR")
                      from KZTLF in s.GetField<string>("KZTLF")
                      from ZTERM in s.GetField<string>("ZTERM")
                      from DDTEXT_GBSTK in s.GetField<string>("DDTEXT_GBSTK")
                      from ARKTX_2 in s.GetField<string>("ARKTX_2")
                      from KWMENG_2 in s.GetField<string>("KWMENG_2")
                      select new
                      {
                          VBELN,
                          POSNR,
                          MATNR,
                          SGT_RCAT,
                          KWMENG,
                          VRKME = VRKME == "ST" ? "UN" : VRKME,
                          ARKTX,
                          VPRGR,
                          PSTYV,
                          UEPOS,
                          ERDAT,
                          WERKS,
                          BMENG,
                          ETTYP,
                          BEDAE,
                          MBDAT,
                          KSCHL,
                          KBETR,
                          WAERS,
                          NETPR,
                          KPEIN,
                          KMEIN = KMEIN == "ST" ? "UN" : KMEIN,
                          NETWR,
                          WAERK,
                          PRODH,
                          PRCTR,
                          KZTLF,
                          ZTERM,
                          DDTEXT_GBSTK,
                          ARKTX_2,
                          KWMENG_2
                      })

                    from ET_RETURN in f.MapTable("ET_RETURN", s =>
                     from TYPE in s.GetField<string>("TYPE")
                     from ID in s.GetField<string>("ID")
                     from NUMBER in s.GetField<string>("NUMBER")
                     from MESSAGE in s.GetField<string>("MESSAGE")
                     from LOG_NO in s.GetField<string>("LOG_NO")
                     from LOG_MSG_NO in s.GetField<decimal>("LOG_MSG_NO")
                     from MESSAGE_V1 in s.GetField<string>("MESSAGE_V1")
                     from MESSAGE_V2 in s.GetField<string>("MESSAGE_V2")
                     from MESSAGE_V3 in s.GetField<string>("MESSAGE_V3")
                     from MESSAGE_V4 in s.GetField<string>("MESSAGE_V4")
                     from PARAMETER in s.GetField<string>("PARAMETER")
                     from ROW in s.GetField<string>("ROW")
                     from FIELD in s.GetField<string>("FIELD")
                     from SYSTEM in s.GetField<string>("SYSTEM")
                    

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
                        ET_CABECERA,
                        ET_DETALLE,
                        ET_RETURN
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