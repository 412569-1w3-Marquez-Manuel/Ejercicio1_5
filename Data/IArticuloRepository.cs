using Proyecto01.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Collections.Generic;

namespace Proyecto01.Data
{
    public interface IArticuloRepository
    {
        int InsertarArticulo(string nombre, decimal precioUnitario);
        List<Articulo> GetAll();
        Articulo? GetById(int id);
        bool Save(Articulo articulo);
        bool Delete(int id);
    }
}
