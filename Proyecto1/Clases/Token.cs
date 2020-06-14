﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1.Clases
{
    class Token
    {
        private Tipo tipo;
        private string lexema;
        private int fila;
        private int columna;

        public Token( Tipo tipo, string lexema, int fila, int columna)
        {
            this.tipo = tipo;
            this.lexema = lexema;
            this.fila = fila;
            this.columna = columna;
        }
        public string Lexema { get => lexema; set => lexema = value; }
        public int Fila { get => fila; set => fila = value; }
        public int Columna { get => columna; set => columna = value; }
        internal Tipo Tipo { get => tipo; set => tipo = value; }
    }
}