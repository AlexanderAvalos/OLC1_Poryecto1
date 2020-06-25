using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1.Clases
{
    class Tabla
    {
        private string nombre_tabla;
        private List<Columna> columnas = new List<Columna>();
        private List<Dictionary<string, object>> filas = new List<Dictionary<string, object>>();

        public Tabla()
        {
            this.columnas = new List<Columna>();
            this.Filas = new List<Dictionary<string, object>>();
        }
        public void agregarcolumna(int index,string llave, string valor) {
            if (filas.Count > index + 1)
                filas.ElementAt(index).Add(llave, valor);
            else
            {
                Dictionary<string, object> aux = new Dictionary<string, object>();
                aux.Add(llave, valor);
                filas.Add(aux);
            }
        }
        public string Nombre_tabla { get => nombre_tabla; set => nombre_tabla = value; }
        public List<Dictionary<string, object>> Filas { get => filas; set => filas = value; }
        internal List<Columna> Columnas { get => columnas; set => columnas = value; }
    }
}
