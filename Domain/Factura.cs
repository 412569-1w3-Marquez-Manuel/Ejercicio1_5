using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto01.Domain
{
    public class Factura
    {
        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public FormaPago FormaPago { get; set; }
        public List<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();

        public decimal Total => Detalles.Sum(d => d.SubTotal);

        public Factura() { }

        public Factura(int numero, DateTime fecha, string cliente, FormaPago formaPago)
        {
            Numero = numero;
            Fecha = fecha;
            Cliente = cliente;
            FormaPago = formaPago ?? throw new ArgumentNullException(nameof(formaPago));
        }

        public void AgregarDetalle(DetalleFactura detalle)
        {
            if (detalle == null) throw new ArgumentNullException(nameof(detalle));

            var existente = Detalles
                .FirstOrDefault(d => d.Articulo.Id == detalle.Articulo.Id);

            if (existente != null)
                existente.Cantidad += detalle.Cantidad;
            else
                Detalles.Add(detalle);
        }

        public override string ToString()
        {
            var lineas = Detalles.Select(d => d.ToString());
            var detallesStr = string.Join(Environment.NewLine, lineas);
            return
                $"Factura Nº {Numero} – {Fecha:dd/MM/yyyy}\n" +
                $"Cliente: {Cliente}\n" +
                $"Forma Pago: {FormaPago.Nombre}\n" +
                $"Detalles:\n{detallesStr}\n" +
                $"Total: {Total:C}";
        }
    }
}
