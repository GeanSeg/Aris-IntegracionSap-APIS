using Newtonsoft.Json;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entidad.PlantaAlimentoDespacho
{
    public class PlantaAlimentoDespachoInsuMa
    {
        [JsonProperty("FECHA_RECEPCION")]
        public string FECHA_RECEPCION { get; set; }
        public string FECHA_TRANSFERENCIA { get; set; }
        public string CLASE_MOVIMIENTO { get; set; }
        public string RECEPCION_ID { get; set; }
        public List<PlantaAlimentoDespachoInsuMaDetalle> _PlantaAlimentoDespachoInsuMaDetalle { get; set; }
    }
    public class PlantaAlimentoDespachoInsuMaDetalle
    {
        public string MATERIAL_ID { get; set; }      
        public decimal PESO_SALIDA { get; set; }    
        public string UNIDAD_MEDIDA { get; set; }    
        public string CLASE_MOVIMIENTO { get; set; }
        public string CENTRO_ID { get; set; }       
        public string ALMACEN_ID { get; set; }     
        public string PROVEEDOR_ID { get; set; }   
        public string LOTE { get; set; }             
    }

    public class PlantaAlimentoDespachoInsuMaParameters
    {
        [SapName("CABECERA_SAP")]
        public PlantaAlimentoDespachoInsuMaParametersItem CABECERA_SAP { get; set; }

        [SapName("DETALLE_SAP")]
        public PlantaAlimentoDespachoInsuMaParametersItems[] DETALLE_SAP { get; set; }
    }

    public class PlantaAlimentoDespachoInsuMaResult
    {
        [SapName("RESPUESTA_SAP")]
        public PlantaAlimentoDespachoInsuMaResultItems RESPUESTA_SAP { get; set; }

    }

    public class PlantaAlimentoDespachoInsuMaParametersItem
    {
        [SapName("FECHA_RECEPCION")]
        public DateTime FECHA_RECEPCION { get; set; }

        [SapName("FECHA_TRANSFERENCIA")]
        public DateTime FECHA_TRANSFERENCIA { get; set; }

        [SapName("CLASE_MOVIMIENTO")]
        public string CLASE_MOVIMIENTO { get; set; }

        [SapName("RECEPCION_ID")]
        public string RECEPCION_ID { get; set; }
    }

    public class PlantaAlimentoDespachoInsuMaParametersItems
    {
        [SapName("MATERIAL_ID")]
        public string MATERIAL_ID { get; set; }

        [SapName("PESO_SALIDA")]
        public decimal PESO_SALIDA { get; set; }

        [SapName("UNIDAD_MEDIDA")]
        public string UNIDAD_MEDIDA { get; set; }

        [SapName("CLASE_MOVIMIENTO")]
        public string CLASE_MOVIMIENTO { get; set; }

        [SapName("CENTRO_ID")]
        public string CENTRO_ID { get; set; }

        [SapName("ALMACEN_ID")]
        public string ALMACEN_ID { get; set; }

        [SapName("PROVEEDOR_ID")]
        public string PROVEEDOR_ID { get; set; }

        [SapName("LOTE")]
        public string LOTE { get; set; }
    }

    public class PlantaAlimentoDespachoInsuMaResultItems
    {
        [SapName("MENSAJE")]
        public string MENSAJE { get; set; }

        [SapName("MBLNR")]
        public string MBLNR { get; set; }

        [SapName("MJAHR")]
        public string MJAHR { get; set; }
    }

}
