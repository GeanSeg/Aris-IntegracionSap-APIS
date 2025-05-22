using Domain.Entidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEntregaService
    {
        Task<bool> CrearEntregaSAP(ReplicaOdooEntrega replicaOdooEntrega);
    }
}
