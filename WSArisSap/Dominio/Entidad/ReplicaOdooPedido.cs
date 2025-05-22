using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidad
{
    public class ReplicaOdooPedido
    {
        public string AUART { get; set; }
        public string PURCH_NO_C { get; set; }
        public string PURCH_DATE { get; set; }
        public string VDATU { get; set; }
        public string CURRENCY { get; set; }
        public string VKORG { get; set; }
        public string VTWEG { get; set; }
        public string SPART { get; set; }
        public string? VKGRP { get; set; }
        public string? VKBUR { get; set; }
        public string ZTERM { get; set; }
        public string? AUGRU { get; set; }
        public string BZIRK { get; set; }
        public string KUNNR { get; set; }
        public List<ReplicaOdooPedidoDetalle> replicaOdooPedidoDetalle { get; set; }
    }
    public class ReplicaOdooPedidoDetalle
    {
        public string POSNR { get; set; }
        public string MATNR { get; set; }
        public string WERKS { get; set; }
        public decimal DZMENG { get; set; }
        public string DZIEME { get; set; }
        public string PSTYV { get; set; }
        public decimal NETWR { get; set; }
        public string WAERK { get; set; }
        public string PROFIT_CTR { get; set; }
        public string LGORT { get; set; }
    }

    public class PedidoParameters
    {
        [SapName("ES_CAB_PED")]
        public PedidoResultItemCAB CAB { get; set; }

        [SapName("T_DET_PED")]
        public PedidoResultItemDET[] DET { get; set; }

    }

    public class PedidoResult
    {
        [SapName("E_VBELN")]
        public string E_VBELN_ENT { get; set; }

        [SapName("T_RETURN")]
        public PedidoResultItemTReturn[] T_RETURN { get; set; }
    }
    public class PedidoResultItemTReturn
    {
        [SapName("TYPE")]
        public string TYPE { get; set; }

        [SapName("ID")]
        public string ID { get; set; }

        [SapName("NUMBER")]
        public string NUMBER { get; set; }

        [SapName("MESSAGE")]
        public string MESSAGE { get; set; }
    }

    public class PedidoResultItemCAB
    {
        [SapName("AUART")]
        public string AUART { get; set; }

        [SapName("PURCH_NO_C")]
        public string PURCH_NO_C { get; set; }

        [SapName("PURCH_DATE")]
        public DateTime PURCH_DATE { get; set; }

        [SapName("VDATU")]
        public DateTime VDATU { get; set; }

        [SapName("CURRENCY")]
        public string CURRENCY { get; set; }

        [SapName("VKORG")]
        public string VKORG { get; set; }

        [SapName("VTWEG")]
        public string VTWEG { get; set; }

        [SapName("SPART")]
        public string SPART { get; set; }

        [SapName("VKGRP")]
        public string? VKGRP { get; set; }

        [SapName("VKBUR")]
        public string? VKBUR { get; set; }

        [SapName("ZTERM")]
        public string ZTERM { get; set; }

        [SapName("AUGRU")]
        public string? AUGRU { get; set; }

        [SapName("BZIRK")]
        public string BZIRK { get; set; }

        [SapName("KUNNR")]
        public string KUNNR { get; set; }
    }

    public class PedidoResultItemDET
    {
        [SapName("POSNR")]
        public string POSNR { get; set; }

        [SapName("MATNR")]
        public string MATNR { get; set; }

        [SapName("WERKS")]
        public string WERKS { get; set; }

        [SapName("DZMENG")]
        public decimal DZMENG { get; set; }

        [SapName("DZIEME")]
        public string DZIEME { get; set; }

        [SapName("PSTYV")]
        public string PSTYV { get; set; }

        [SapName("NETWR")]
        public decimal NETWR { get; set; }

        [SapName("WAERK")]
        public string WAERK { get; set; }

        [SapName("PROFIT_CTR")]
        public string PROFIT_CTR { get; set; }

        [SapName("LGORT")]
        public string LGORT { get; set; }

    }

}
