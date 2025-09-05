using Proyecto01.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto01.Data
{
    public interface IFormaPagoRepository
    {
        List<FormaPago> GetAll();
        FormaPago? GetById(int id);
    }
}
