﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1.Analizador
{
    class Nodo
    {
        int index;
        string nombre;
        List<Nodo> childs;

        public Nodo(int index, string nombre, List<Nodo> lst )
        {
            this.index = index;
            this.Nombre = nombre;
            Childs = lst;
        }

        public string Nombre { get => nombre; set => nombre = value; }
        public List<Nodo> Childs { get => childs; set => childs = value; }
    }
}