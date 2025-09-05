using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient; // Cambiado de System.Data.SqlClient a Microsoft.Data.SqlClient
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Collections.Generic;

namespace Proyecto01.Data
{
    public class DataHelper
    {
        private static DataHelper _instance;
        private readonly SqlConnection _connection;
        private readonly string _connectionString; // Agregado: define la variable de cadena de cone

        private DataHelper()
        {
            // Usá tu cadena real aquí
            string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=db_facturacion;Integrated Security=True;TrustServerCertificate=True ";
            _connection = new SqlConnection(connectionString);
        }

        public static DataHelper GetInstance()
        {
            if (_instance == null)
                _instance = new DataHelper();
            return _instance;
        }

        public DataTable ExecuteSPQuery(string spName, List<ParametroSP>? parametros = null)
        {
            var dt = new DataTable();
            try
            {
                _connection.Open();
                using var cmd = new SqlCommand(spName, _connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                if (parametros != null)
                    parametros.ForEach(p =>
                        cmd.Parameters.AddWithValue(p.Name, p.Valor ?? DBNull.Value));

                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {
                // Aquí puedes loguear ex.Message para depurar
                // dt NO se asigna a null, dejamos el DataTable vacío
            }
            finally
            {
                _connection.Close();
            }
            return dt;
        }


        public SqlConnection GetConnection() => _connection;
    }
}
