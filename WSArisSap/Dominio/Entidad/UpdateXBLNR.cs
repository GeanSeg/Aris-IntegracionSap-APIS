using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entidad.UpdateXBLNR
{
    public class UpdateXBLNR
    {
        public string I_VBELN { get; set; } 
        public string I_XBLNR { get; set; } 
    }

    public class UpdateXBLNR_Parameters
    {
        [SapName("I_VBELN")]
        public string? I_VBELN { get; set; }

        [SapName("I_XBLNR")]
        public string? I_XBLNR { get; set; }
    }
}
