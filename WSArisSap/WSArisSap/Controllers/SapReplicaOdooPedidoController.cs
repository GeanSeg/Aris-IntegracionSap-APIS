using Application.Interfaces;
using Dbosoft.YaNco;
using Domain.Entidad;
using Dominio.Entidad;
using Infrastructure.Services;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.SomeHelp;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using SapNwRfc;
using System;
using System.Runtime.InteropServices;

namespace WSpruebaArisSap.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class SapReplicaOdooPedidoController (ILibraryInitializer libraryInitializer,
        IInitializerContextSAP initializerContextSAP,SapResultadoPedido sapResultado,
        ILoggingService loggingService) : Controller
    {
        [HttpPost(Name = "SapReplicaOdooPedido")]
        public async Task<IActionResult> ReplicaOdooPedido([FromBody] ReplicaOdooPedido replicaOdooPedido)
        {
            try
            {
                loggingService.LogInfo("ReplicaOdooPedido : Inicializando Librería");

                bool resLibraryInitializer = libraryInitializer.InitializeLibrary();

                if (!resLibraryInitializer) {
                    throw new Exception("No se pudo cargar librerías necesarias");
                }

                string connectionString = initializerContextSAP.InitializeContextConnSap();

                loggingService.LogInfo("ReplicaOdooPedido : Conectando con SAP");
                loggingService.LogInfo($"ReplicaOdooPedido : conn => {connectionString}");

                using var connection = new SapConnection(connectionString);
                connection.Connect();

                loggingService.LogInfo("ReplicaOdooPedido : Consumiendo RFC ZSD_REPLICA_ODOO_PEDIDO");

                loggingService.LogInfo($"AUART: {replicaOdooPedido.AUART}");
                loggingService.LogInfo($"PURCH_NO_C: {replicaOdooPedido.PURCH_NO_C.PadLeft(10, '0')}");
                loggingService.LogInfo($"PURCH_DATE: {replicaOdooPedido.PURCH_DATE}");
                loggingService.LogInfo($"VDATU: {replicaOdooPedido.VDATU}");
                loggingService.LogInfo($"CURRENCY: {replicaOdooPedido.CURRENCY}");
                loggingService.LogInfo($"VKORG: {replicaOdooPedido.VKORG}");
                loggingService.LogInfo($"VTWEG: {replicaOdooPedido.VTWEG}");
                loggingService.LogInfo($"SPART: {replicaOdooPedido.SPART.PadLeft(2, '0')}");
                loggingService.LogInfo($"VKGRP: {replicaOdooPedido.VKGRP}");
                loggingService.LogInfo($"VKBUR: {replicaOdooPedido.VKBUR}");
                loggingService.LogInfo($"ZTERM: {replicaOdooPedido.ZTERM}");
                loggingService.LogInfo($"AUGRU: {replicaOdooPedido.AUGRU}");
                loggingService.LogInfo($"BZIRK: {replicaOdooPedido.BZIRK}");
                loggingService.LogInfo($"KUNNR: {replicaOdooPedido.KUNNR.PadLeft(10, '0')}");

                foreach (var detalle in replicaOdooPedido.replicaOdooPedidoDetalle)
                {
                    loggingService.LogInfo("--- Detalle ---");
                    loggingService.LogInfo($"POSNR: {detalle.POSNR.PadLeft(6, '0')}");
                    loggingService.LogInfo($"MATNR: {detalle.MATNR}");
                    loggingService.LogInfo($"WERKS: {detalle.WERKS}");
                    loggingService.LogInfo($"DZMENG: {detalle.DZMENG}");
                    loggingService.LogInfo($"DZIEME: {detalle.DZIEME}");
                    loggingService.LogInfo($"PSTYV: {detalle.PSTYV}");
                    loggingService.LogInfo($"NETWR: {detalle.NETWR}");
                    loggingService.LogInfo($"WAERK: {detalle.WAERK}");
                    loggingService.LogInfo($"PROFIT_CTR: {detalle.PROFIT_CTR}");
                    loggingService.LogInfo($"LGORT: {detalle.LGORT}");
                }

                using var someFunction = connection.CreateFunction("ZSD_REPLICA_ODOO_PEDIDO");

                var result = someFunction.Invoke<PedidoResult>(new PedidoParameters
                {
                    CAB = new PedidoResultItemCAB
                    {
                        AUART = replicaOdooPedido.AUART,
                        PURCH_NO_C = replicaOdooPedido.PURCH_NO_C.PadLeft(10, '0'),
                        PURCH_DATE = DateTime.ParseExact(replicaOdooPedido.PURCH_DATE, "dd.MM.yyyy", null),
                        VDATU = DateTime.ParseExact(replicaOdooPedido.VDATU, "dd.MM.yyyy", null),
                        CURRENCY = replicaOdooPedido.CURRENCY,
                        VKORG = replicaOdooPedido.VKORG,
                        VTWEG = replicaOdooPedido.VTWEG,
                        SPART = replicaOdooPedido.SPART.PadLeft(2, '0'),
                        VKGRP = replicaOdooPedido.VKGRP,
                        VKBUR = replicaOdooPedido.VKBUR,
                        ZTERM = replicaOdooPedido.ZTERM,
                        AUGRU = replicaOdooPedido.AUGRU,
                        BZIRK = replicaOdooPedido.BZIRK,
                        KUNNR = replicaOdooPedido.KUNNR.PadLeft(10, '0'),

                    },
                    DET = replicaOdooPedido.replicaOdooPedidoDetalle
                        .Select(detalle => new PedidoResultItemDET
                        {
                            POSNR = detalle.POSNR.PadLeft(6, '0'),
                            MATNR = detalle.MATNR,
                            WERKS = detalle.WERKS,
                            DZMENG = detalle.DZMENG,
                            DZIEME = detalle.DZIEME,
                            PSTYV = detalle.PSTYV,
                            NETWR = detalle.NETWR,
                            WAERK = detalle.WAERK,
                            PROFIT_CTR = detalle.PROFIT_CTR,
                            LGORT = detalle.LGORT
                        })
                        .ToArray()
                });

                loggingService.LogInfo("ReplicaOdooPedido : Fin Consumiendo RFC ZSD_REPLICA_ODOO_PEDIDO");
                return Ok(result);

            }

            catch (Exception ex)
            {
                loggingService.LogError($"ReplicaOdooPedido : {ex.Message}");
                return StatusCode(500, new
                {
                    Error = $"Error {ex.Message}"
                });
            }
        }
    }
}
