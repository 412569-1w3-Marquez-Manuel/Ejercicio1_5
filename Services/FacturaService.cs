using Proyecto01.Data;
using Proyecto01.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;






namespace Proyecto01.Services
{
    public class FacturaService
    {
        private readonly IFacturaRepository _repo;
        private readonly ArticuloRepository _artRepo;

        public FacturaService(string cadenaConexion)
        {
            _repo = new FacturaRepository(cadenaConexion);
            _artRepo = new ArticuloRepository(cadenaConexion);
        }

        public List<Factura> GetAllFacturas() => _repo.GetAll();

        public Factura? GetFacturaByNumero(int numero) => _repo.GetByNumero(numero);

        public bool SaveFactura(Factura factura)
        {
            // No guardamos facturas sin detalles
            if (factura.Detalles.Count == 0)
                return false;

            // Validamos cada detalle contra la tabla Articulo
            foreach (var detalle in factura.Detalles)
            {
                var articuloEnBd = _artRepo.GetById(detalle.Articulo.Id);
                if (articuloEnBd == null)
                {
                    Console.WriteLine($"❌ El artículo con ID {detalle.Articulo.Id} no existe.");
                    return false;
                }

                // Sobrescribimos precio y referencia de Articulo
                detalle.PrecioUnitario = articuloEnBd.PrecioUnitario;
                detalle.Articulo = articuloEnBd;
            }

            // Si todos los artículos existen, guardamos la factura
            return _repo.Save(factura);
        }
    }
}
