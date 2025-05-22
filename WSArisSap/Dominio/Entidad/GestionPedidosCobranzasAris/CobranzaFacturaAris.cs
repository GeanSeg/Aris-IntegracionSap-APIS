using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entidad.GestionPedidosCobranzasAris
{
    public class CobranzaFacturaAris
    {
        public string BLDAT { get; set; }
        public string BUDAT { get; set; }
        public string BLART { get; set; }
        public string BUKRS { get; set; }
        public string WAERS { get; set; }
        public string BKTXT { get; set; }
        public string KONTO { get; set; }
        public string VALUT { get; set; }
        public string AGKON { get; set; }

        public List<CobranzaFacturaArisDetalle> cobranzaFacturaArisDetalle { get; set; }
    }

    public class CobranzaFacturaArisDetalle
    {
        public string NRO_FACTURA { get; set; }
        public decimal IMPORTE_PAGO { get; set; }
        public string REFERENCIA { get; set; }
        public string SGTXT { get; set; }
        public string ZUONR { get; set; }
        public string SGTXT_COMP { get; set; }
    }

    public class CobranzaFacturaArisParameters
    {
        [SapName("I_COBRANZA_CAB")]
        public CobranzaFacturaArisParametersItem I_COBRANZA_CAB { get; set; }

        [SapName("T_COBRANZA_DET")]
        public CobranzaFacturaArisParametersItems[] T_COBRANZA_DET { get; set; }
    }

    public class CobranzaFacturaArisResult
    {
        [SapName("T_RETURN")]
        public CobranzaFacturaArisResultItems[] T_RETURN { get; set; }
    }

    public class CobranzaFacturaArisParametersItem
    {
        [SapName("BLDAT")]
        public DateTime BLDAT { get; set; }

        [SapName("BUDAT")]
        public DateTime BUDAT { get; set; }

        [SapName("BLART")]
        public string BLART { get; set; }

        [SapName("BUKRS")]
        public string BUKRS { get; set; }

        [SapName("WAERS")]
        public string WAERS { get; set; }

        [SapName("BKTXT")]
        public string BKTXT { get; set; }

        [SapName("KONTO")]
        public string KONTO { get; set; }

        [SapName("VALUT")]
        public DateTime VALUT { get; set; }

        [SapName("AGKON")]
        public string AGKON { get; set; }
    }

    public class CobranzaFacturaArisParametersItems
    {
        [SapName("NRO_FACTURA")]
        public string NRO_FACTURA { get; set; }

        [SapName("IMPORTE_PAGO")]
        public decimal IMPORTE_PAGO { get; set; }
        
        [SapName("REFERENCIA")]
        public string REFERENCIA { get; set; }

        [SapName("SGTXT")]
        public string SGTXT { get; set; }

        [SapName("ZUONR")]
        public string ZUONR { get; set; }

        [SapName("SGTXT_COMP")]
        public string SGTXT_COMP { get; set; }
    }

    public class CobranzaFacturaArisResultItems
    {
        [SapName("NRO_FACTURA")]
        public string NRO_FACTURA { get; set; }

        [SapName("BELNR_NEW")]
        public string BELNR_NEW { get; set; }

        [SapName("AUGBL_NEW")]
        public string AUGBL_NEW { get; set; }

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
