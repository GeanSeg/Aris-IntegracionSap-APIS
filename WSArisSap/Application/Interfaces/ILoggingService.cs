using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ILoggingService
    {
        void LogInfo(string mensaje);
        void LogError(string mensaje);
    }
}
