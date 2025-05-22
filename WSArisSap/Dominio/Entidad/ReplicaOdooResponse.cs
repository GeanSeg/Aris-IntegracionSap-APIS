using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidad
{
    public class SapResultadoPedido
    {
        public string? EstadoPedidoSAP { get; set; }
        public string? E_VBELN { get; set; }
        public List<ReplicaOdooResponse> replicaOdooResponses { get; set; }
    }

    public class SapResultadoEntrega
    {
        public string? EstadoEntregaSAP { get; set; }
        public string? E_VBELN_ENT { get; set; }
        public List<ReplicaOdooResponse> replicaOdooResponses { get; set; }
    }

    public class SapResultadoFactura
    {
        public string? EstadoFactureSAP { get; set; }
        public string? E_FACTURA { get; set; }
        public List<ReplicaOdooResponse> replicaOdooResponses { get; set; }
    }

    public class ReplicaOdooResponse
    {
        public string RTYPE { get; set; }
        public string RID { get; set; }
        public string RNUMBER { get; set; }
        public string RMESSAGE { get; set; }
    }
}
