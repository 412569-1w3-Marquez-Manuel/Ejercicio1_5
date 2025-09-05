using Microsoft.Data.SqlClient;
using Proyecto01.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Proyecto01.Data
{
    public class DetalleFacturaRepository : IDetalleFacturaRepository
    {
        private readonly string _conexion;

        public DetalleFacturaRepository(string cadenaConexion)
        {
            _conexion = cadenaConexion;
        }

        public List<DetalleFactura> GetByFactura(int numeroFactura)
        {
            var detalles = new List<DetalleFactura>();

            using (var conn = new SqlConnection(_conexion))
            {
                try
                {
                    conn.Open();

                    using var cmd = new SqlCommand("SP_RECUPERAR_DETALLES_POR_FACTURA", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@NumeroFactura", numeroFactura);

                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var articulo = new Articulo
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CodigoArticulo")),
                            Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                            PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")) // nuevo campo del SP
                        };

                        var detalle = new DetalleFactura
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("IdDetalle")),
                            NumeroFactura = reader.GetInt32(reader.GetOrdinal("NumeroFactura")),
                            Articulo = articulo,
                            Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                            PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("Precio"))
                        };

                        detalles.Add(detalle);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error al recuperar detalles: {ex.Message}");
                }
            }

            return detalles;
        }

        public bool Save(DetalleFactura detalle)
        {
            using (var conn = new SqlConnection(_conexion))
            {
                try
                {
                    conn.Open();
                    using var cmd = new SqlCommand("SP_INSERTAR_DETALLE_FACTURA", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@NumeroFactura", detalle.NumeroFactura);
                    cmd.Parameters.AddWithValue("@CodigoArticulo", detalle.Articulo.Id);
                    cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                    cmd.Parameters.AddWithValue("@Precio", detalle.PrecioUnitario);

                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error al guardar detalle: {ex.Message}");
                    return false;
                }
            }
        }
    }
}
