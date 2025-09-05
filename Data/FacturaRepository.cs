using Proyecto01.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Transactions; // <-- Agregar si usas transacciones distribuidas






namespace Proyecto01.Data
{
    public class FacturaRepository : IFacturaRepository
    {
        private readonly IDetalleFacturaRepository _detalleRepo;
        private readonly string _conexion;

        public FacturaRepository(string cadenaConexion)
        {
            _conexion = cadenaConexion;
            _detalleRepo = new DetalleFacturaRepository(cadenaConexion);
        }

        public List<Factura> GetAll()
        {
            var facturas = new List<Factura>();

            using (var conn = new SqlConnection(_conexion))
            {
                try
                {
                    conn.Open();

                    using var cmd = new SqlCommand("SP_RECUPERAR_FACTURAS", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var factura = new Factura
                        {
                            Numero = reader.GetInt32(reader.GetOrdinal("NumeroFactura")),
                            Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                            Cliente = reader.GetString(reader.GetOrdinal("Cliente")),
                            FormaPago = new FormaPago
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("FormaPago")),
                                    Nombre = reader.GetString(reader.GetOrdinal("FormaPagoNombre"))
                            }
                        };

                        facturas.Add(factura);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error al recuperar facturas: {ex.Message}");
                }
            }

            // 2. Obtener detalles por factura
            foreach (var factura in facturas)
            {
                factura.Detalles = _detalleRepo.GetByFactura(factura.Numero);
            }

            return facturas;
        }

        public Factura? GetByNumero(int numero)
        {
            throw new NotImplementedException();
        }

        public bool Save(Factura factura)
        {
            bool ok = true;

            using (var conn = new SqlConnection(_conexion))
            {
                SqlTransaction tx = null;

                try
                {
                    conn.Open();
                    tx = conn.BeginTransaction();

                    using var cmd = new SqlCommand("SP_INSERTAR_FACTURA", conn, tx)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@Fecha", factura.Fecha);
                    cmd.Parameters.AddWithValue("@Cliente", factura.Cliente);
                    cmd.Parameters.AddWithValue("@FormaPago", factura.FormaPago.Id);

                    var outNum = new SqlParameter("@NumeroFactura", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outNum);
                    cmd.ExecuteNonQuery();

                    int numeroFactura = (int)outNum.Value;

                    foreach (var det in factura.Detalles)
                    {
                        using var cmdDet = new SqlCommand("SP_INSERTAR_DETALLE_FACTURA", conn, tx)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        cmdDet.Parameters.AddWithValue("@NumeroFactura", numeroFactura);
                        cmdDet.Parameters.AddWithValue("@Cantidad", det.Cantidad);
                        cmdDet.Parameters.AddWithValue("@CodigoArticulo", det.Articulo.Id);
                        cmdDet.Parameters.AddWithValue("@Precio", det.PrecioUnitario);

                        cmdDet.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
                catch (Exception ex)
                {
                    tx?.Rollback();
                    Console.WriteLine($"❌ Error al guardar la factura: {ex.Message}");
                    ok = false;
                }
            }

            return ok;
        }
    }
}
