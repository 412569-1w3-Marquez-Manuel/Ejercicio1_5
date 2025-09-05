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
    public class ArticuloRepository : IArticuloRepository
    {
        private readonly string _conexion;

        public ArticuloRepository(string cadenaConexion)
        {
            _conexion = cadenaConexion;
        }

        public int InsertarArticulo(string nombre, decimal precioUnitario)
        {
            using (var conn = new SqlConnection(_conexion))
            using (var cmd = new SqlCommand("dbo.SP_INSERTAR_ARTICULO", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@PrecioUnitario", precioUnitario);

                var output = new SqlParameter("@NuevoCodigo", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(output);

                conn.Open();
                cmd.ExecuteNonQuery();

                return (int)output.Value;
            }
        }
        public List<Articulo> GetAll()
        {
            var lst = new List<Articulo>();
            var dt = DataHelper.GetInstance().ExecuteSPQuery("SP_RECUPERAR_ARTICULOS");
            foreach (DataRow row in dt.Rows)
            {
                lst.Add(new Articulo(
                    (int)row["Id"],
                    (string)row["Nombre"],
                    (decimal)row["PrecioUnitario"]
                ));
            }
            return lst;
        }

        public Articulo? GetById(int id)
        {
            var ps = new List<ParametroSP> { new ParametroSP { Name = "@Codigo", Valor = id } };
            var dt = DataHelper.GetInstance()
                .ExecuteSPQuery("SP_RECUPERAR_ARTICULO_POR_ID", ps);

            if (dt == null || dt.Rows.Count == 0)
                return null;

            var r = dt.Rows[0];
            return new Articulo((int)r["Codigo"], (string)r["Nombre"], (decimal)r["PrecioUnitario"]);

        }

        public int SaveAndGetId(Articulo articulo)
        {
            using var conn = new SqlConnection(_conexion);
            using var cmd = new SqlCommand("SP_INSERTAR_ARTICULO", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Parámetros de entrada
            cmd.Parameters.AddWithValue("@Nombre", articulo.Nombre);
            cmd.Parameters.AddWithValue("@PrecioUnitario", articulo.PrecioUnitario);

            // Parámetro output
            var outputParam = new SqlParameter("@NuevoCodigo", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputParam);

            conn.Open();
            cmd.ExecuteNonQuery();

            // Convertir y retornar el nuevo ID
            return (int)outputParam.Value;
        }


        public bool Save(Articulo articulo)
        {
            var parametros = new List<ParametroSP>
    {
        new ParametroSP { Name = "@Nombre", Valor = articulo.Nombre },
        new ParametroSP { Name = "@PrecioUnitario", Valor = articulo.PrecioUnitario }
    };
            var dt = DataHelper.GetInstance().ExecuteSPQuery("SP_INSERTAR_ARTICULO", parametros);
            // Si el SP retorna filas, asumimos éxito
            return dt != null;
        }
        public bool Delete(int id) => throw new NotImplementedException();
    }
}
