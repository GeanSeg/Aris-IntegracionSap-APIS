using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entidad.GestionPedidosCobranzas
{
    public class EstadoCuentaAris
    {
        public string DEUDOR { get; set; } 
        public string SOCIEDAD { get; set; } 
        public string FECHA_ABI_FEC_CLAVE { get; set; }
        public string NRO_FACTURA { get; set; } = "";
    }

    public class EstadoCuentaArisParameters
    {
        [SapName("I_FACT_PEND_PAGO")]
        public EstadoCuentaArisParametersItem FACT_PEND_PAGO { get; set; }

        [SapName("I_NRO_FACTURA")]
        public string? NRO_FACTURA { get; set; }
    }

    public class EstadoCuentaArisResult
    {
        [SapName("E_FACT_PEND_PAGO_R")]
        public EstadoCuentaArisResultItems[] E_FACT_PEND_PAGO_R { get; set; }

        [SapName("E_SALDO_CLIENTE")]
        public EstadoCuentaArisResultItem E_SALDO_CLIENTE { get; set; }

        [SapName("E_SERIE_CORRE_FACT")]
        public EstadoCuentaArisResultItems1[] E_SERIE_CORRE_FACT { get; set; }

        [SapName("E_MONTO_INICIAL")]
        public EstadoCuentaArisResultItems2[] E_MONTO_INICIAL { get; set; }

        [SapName("ET_RETURN")]
        public EstadoCuentaArisResultItems3[] ET_RETURN { get; set; }
    }
  
    public class EstadoCuentaArisParametersItem
    {
        [SapName("DEUDOR")]
        public string DEUDOR { get; set; }

        [SapName("SOCIEDAD")]
        public string SOCIEDAD { get; set; }

        [SapName("FECHA_ABI_FEC_CLAVE")]
        public DateTime FECHA_ABI_FEC_CLAVE { get; set; }
    }

    public class EstadoCuentaArisResultItem
    {
        [SapName("SALDO_SOLES")]
        public string SALDO_SOLES { get; set; }

        [SapName("SALDO_DOLARES")]
        public string SALDO_DOLARES { get; set; }
    }
    public class EstadoCuentaArisResultItems
    {
        [SapName("SOCIEDAD")]
        public string SOCIEDAD { get; set; }

        [SapName("DOC_FACTURACION")]
        public string DOC_FACTURACION { get; set; }

        [SapName("REFERENCIA")]
        public string REFERENCIA { get; set; }

        [SapName("VENC_NETO")]
        public string VENC_NETO { get; set; }

        [SapName("IMPORTE_MD")]
        public string IMPORTE_MD { get; set; }

        [SapName("MONEDA_DOC")]
        public string MONEDA_DOC { get; set; }

        [SapName("CUENTA")]
        public string CUENTA { get; set; }

        [SapName("NOMB_INTERLOCUTOR")]
        public string NOMB_INTERLOCUTOR { get; set; }

    }
    public class EstadoCuentaArisResultItems1
    {
        [SapName("VBELN")]
        public string VBELN { get; set; }

        [SapName("BELNR")]
        public string BELNR { get; set; }

        [SapName("TIPO_DOC")]
        public string TIPO_DOC { get; set; }

        [SapName("SERIE_DOC")]
        public string SERIE_DOC { get; set; }

        [SapName("NUM_DOC")]
        public string NUM_DOC { get; set; }
    }

    public class EstadoCuentaArisResultItems2
    {

        [SapName("VBELN")]
        public string VBELN { get; set; }

        [SapName("BELNR")]
        public string BELNR { get; set; }

        [SapName("NETWR")]
        public string NETWR { get; set; }

        [SapName("MONTO_IMPUESTO")]
        public string MONTO_IMPUESTO { get; set; }

        [SapName("WAERK")]
        public string WAERK { get; set; }
    }
    public class EstadoCuentaArisResultItems3
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
