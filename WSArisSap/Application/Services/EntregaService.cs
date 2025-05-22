using Application.Interfaces;
using Domain.Entidad;
using LanguageExt.ClassInstances.Pred;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public sealed class EntregaService (IEntregaService entregaService)
    {
        public async Task<bool> CrearEntrega(ReplicaOdooEntrega replicaOdooEntrega)
        {
            var a  = await entregaService.CrearEntregaSAP(replicaOdooEntrega);
            return true;
        }
    }
}
