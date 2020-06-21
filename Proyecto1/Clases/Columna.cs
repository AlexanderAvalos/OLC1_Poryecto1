using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1.Clases
{
    class Columna
    {
        private string nombre;
        private string tipo;

        public Columna() {
            this.nombre = "";
            this.tipo = "";
        }
        public Columna(string nombre, string tipo)
        {
            this.nombre = nombre;
            this.tipo = tipo ;
        }

        public string Nombre { get => nombre; set => nombre = value; }
        public string Tipo { get => tipo; set => tipo = value; }
    }
}
