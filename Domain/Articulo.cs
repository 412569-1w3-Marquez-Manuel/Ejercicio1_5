using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto01.Domain
{
    public class Articulo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioUnitario { get; set; }

        public Articulo() { }

        public Articulo(int id, string nombre, decimal precioUnitario)
        {
            Id = id;
            Nombre = nombre;
            PrecioUnitario = precioUnitario;
        }

        public override string ToString() =>
            $"{Id} - {Nombre} @ {PrecioUnitario:C}";
    }
}
