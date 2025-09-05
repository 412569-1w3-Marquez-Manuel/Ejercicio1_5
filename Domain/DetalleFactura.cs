using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto01.Domain
{
    public class DetalleFactura
    {
        public int Id { get; set; }
        public int NumeroFactura { get; set; }
        public Articulo Articulo { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public decimal SubTotal => Cantidad * PrecioUnitario;

        public DetalleFactura() { }

        public DetalleFactura(int id, Articulo articulo, int cantidad, decimal precioUnitario)
        {
            Id = id;
            Articulo = articulo ?? throw new ArgumentNullException(nameof(articulo));
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
        }

        public override string ToString() =>
            $"{Articulo.Nombre} x {Cantidad} = {SubTotal:C}";
    }
}
