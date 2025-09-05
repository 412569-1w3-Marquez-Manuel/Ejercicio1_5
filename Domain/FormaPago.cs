using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto01.Domain
{
    public class FormaPago
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public FormaPago() { }

        public FormaPago(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        public override string ToString() =>
            $"{Id} - {Nombre}";
    }
}
