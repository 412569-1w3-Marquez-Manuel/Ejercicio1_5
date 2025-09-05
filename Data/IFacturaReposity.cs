using Proyecto01.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto01.Data
{
    public interface IFacturaRepository
    {
        List<Factura> GetAll();
        Factura? GetByNumero(int numero);
        bool Save(Factura factura);
    }
}
