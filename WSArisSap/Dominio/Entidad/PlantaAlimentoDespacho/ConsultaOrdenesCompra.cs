using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entidad.PlantaAlimentoDespacho
{
     public class ConsultaOrdenesCompra
    {
        public string I_FEC_CREA_INICIO { get; set; }
        public string I_FEC_CREA_FIN { get; set; }
        public string VC_CLASE_DOCUMENTO { get; set; }
        public string I_NRO_PEDIDO { get; set; }
        public string I_SOCIEDAD { get; set; }
    }

    public class ConsultaOrdenesCompraParameters
    {
        [SapName("I_FEC_CREA_INICIO")]
        public DateTime I_FEC_CREA_INICIO { get; set; }

        [SapName("I_FEC_CREA_FIN")]
        public DateTime I_FEC_CREA_FIN { get; set; }

        [SapName("I_NRO_PEDIDO")]
        public string I_NRO_PEDIDO { get; set; }

        [SapName("I_SOCIEDAD")]
        public string I_SOCIEDAD { get; set; }
    }

    public class ConsultaOrdenesCompraResult
    {
        [SapName("ET_CABECERA")]
        public ConsultaOrdenesCompraResultItems[] ET_CABECERA { get; set; }

        [SapName("ET_DETALLE")]
        public ConsultaOrdenesCompraResultItems2[] ET_DETALLE { get; set; }

        [SapName("ET_RETURN")]
        public ConsultaOrdenesCompraResultItems3[] ET_RETURN { get; set; }
    }


    public class ConsultaOrdenesCompraResultItems
    {
        [SapName("EBELN")]
        public string EBELN { get; set; }

        [SapName("BSART")]
        public string BSART { get; set; }

        [SapName("BEDAT")]
        public string BEDAT { get; set; }

        [SapName("FRGKE")]
        public string FRGKE { get; set; }

        [SapName("LIFNR")]
        public string LIFNR { get; set; }

        [SapName("EKORG")]
        public string EKORG { get; set; }

        [SapName("EKGRP")]
        public string EKGRP { get; set; }

        [SapName("BUKRS")]
        public string BUKRS { get; set; }

        [SapName("AEDAT")]
        public string AEDAT { get; set; }

        [SapName("ERNAM")]
        public string ERNAM { get; set; }

        [SapName("FEC_MODIF")]
        public string FEC_MODIF { get; set; }

        [SapName("USU_MODIF")]
        public string USU_MODIF { get; set; }

        [SapName("ZZ_LUG_ENTR")]
        public string ZZ_LUG_ENTR { get; set; }

        [SapName("CAMPO_NO_UTILIZADO")]
        public string CAMPO_NO_UTILIZADO { get; set; }

        [SapName("SUMINISTRO_COMPLETO")]
        public string SUMINISTRO_COMPLETO { get; set; }

        [SapName("MENSAJE_EM")]
        public string MENSAJE_EM { get; set; }
    }

    public class ConsultaOrdenesCompraResultItems2
    {
        [SapName("EBELN")]
        public string EBELN { get; set; }

        [SapName("EBELP")]
        public string EBELP { get; set; }
        [SapName("PSTYP")]
        public string PSTYP { get; set; }

        [SapName("MATNR")]
        public string MATNR { get; set; }

        [SapName("TXZ01")]
        public string TXZ01 { get; set; }

        [SapName("MENGE")]
        public string MENGE { get; set; }

        [SapName("MEINS")]
        public string MEINS { get; set; }

        [SapName("LPEIN")]
        public string LPEIN { get; set; }

        [SapName("EINDT")]
        public string EINDT { get; set; }

        [SapName("NETPR")]
        public string NETPR { get; set; }

        [SapName("WAERS")]
        public string WAERS { get; set; }

        [SapName("PEINH")]
        public string PEINH { get; set; }

        [SapName("MATKL")]
        public string MATKL { get; set; }

        [SapName("WERKS")]
        public string WERKS { get; set; }

        [SapName("LGORT")]
        public string LGORT { get; set; }

        [SapName("CHARG")]
        public string CHARG { get; set; }

        [SapName("GRATIS")]
        public string GRATIS { get; set; }

        [SapName("AFNAM")]
        public string AFNAM { get; set; }

        [SapName("BANFN")]
        public string BANFN { get; set; }

        [SapName("BNFPO")]
        public string BNFPO { get; set; }
    }

    public class ConsultaOrdenesCompraResultItems3
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
}
