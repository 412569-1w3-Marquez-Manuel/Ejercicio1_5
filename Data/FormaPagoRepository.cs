using Proyecto01.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto01.Data
{
    public class FormaPagoRepository : IFormaPagoRepository
    {
        public List<FormaPago> GetAll()
        {
            var lst = new List<FormaPago>();
            var dt = DataHelper.GetInstance().ExecuteSPQuery("SP_RECUPERAR_FORMAS_PAGO");
            foreach (DataRow row in dt.Rows)
            {
                lst.Add(new FormaPago((int)row["Id"], (string)row["Nombre"]));
            }
            return lst;
        }

        public FormaPago? GetById(int id)
        {
            var ps = new List<ParametroSP> { new ParametroSP { Name = "@Id", Valor = id } };
            var dt = DataHelper.GetInstance()
                .ExecuteSPQuery("SP_RECUPERAR_FORMA_PAGO_POR_ID", ps);
            if (dt.Rows.Count == 0) return null;
            var r = dt.Rows[0];
            return new FormaPago((int)r["Id"], (string)r["Nombre"]);
        }
    }
}
