using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configuration
{
    public class SapSettings
    {
        public string AppServerHost { get; set; }
        public string SystemNumber { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Client { get; set; }
        public string Language { get; set; }
        public string PoolSize { get; set; }
        public string Trace { get; set; }
    }
}
