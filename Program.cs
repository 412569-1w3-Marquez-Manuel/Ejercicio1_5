// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Proyecto01.Domain;
using Proyecto01.Services;
using System;


class Program
{
    static void Main()
    {
        
        var service = new FacturaService("Data Source=.\\SQLEXPRESS;Initial Catalog=db_facturacion;Integrated Security=True;TrustServerCertificate=True");

        Console.WriteLine("Listado de Facturas:");
        var facturas = service.GetAllFacturas();
        if (facturas.Count > 0)
        {
            facturas.ForEach(f => Console.WriteLine(f));
        }
        else
        {
            Console.WriteLine("No hay facturas registradas.");
        }

        Console.WriteLine("\nCreando nueva factura de prueba...");
        var fp = new FormaPago(1, "Tarjeta");
        var art = new Articulo(1, "Lapicera", 150m);
        var nueva = new Factura(0, DateTime.Today, "ACME S.A.", fp);

        // Agregamos el detalle usando el constructor original (4 parámetros)
        nueva.AgregarDetalle(
            new DetalleFactura(
                id: 0,
                articulo: art,
                cantidad: 2,
                precioUnitario: art.PrecioUnitario
            )
        );

        bool ok = service.SaveFactura(nueva);
        Console.WriteLine(ok
            ? "✅ Factura registrada con éxito."
            : "❌ Error al registrar la factura.");
    }
}