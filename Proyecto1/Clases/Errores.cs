using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1.Clases
{
    class Errores
    {
        private string tipo;
        private string descripcion;
        private int fila;
        private int columna;

        public Errores(string tipo, string descripcion, int fila, int columna)
        {
            this.tipo = tipo ;
            this.descripcion = descripcion ;
            this.fila = fila;
            this.columna = columna;
        }

        public string Tipo { get => tipo; set => tipo = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }
        public int Fila { get => fila; set => fila = value; }
        public int Columna { get => columna; set => columna = value; }
    }
}
