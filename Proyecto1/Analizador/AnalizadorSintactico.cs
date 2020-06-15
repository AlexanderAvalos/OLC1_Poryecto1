using Proyecto1.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1.Analizador
{
    class AnalizadorSintactico
    {
        private Tipo preanalisis = Tipo.S;
        private List<Token> lst_token = new List<Token>();
        private List<Errores> lst_error = new List<Errores>();
        private int indice = 0;
        private bool con = false;
        private bool correcto = true;

        public AnalizadorSintactico(List<Token> token_inicial, List<Errores> lst_error)
        {
            this.lst_token.Add(new Token(Tipo.S, "", 0, 0));
            this.lst_token.AddRange(token_inicial);
            this.lst_error = lst_error;
        }
        public void match(Tipo token)
        {
            if (token == preanalisis)
            {
                preanalisis = siguiente();
            }
            else
            {
                if (correcto == true)
                {
                    lst_error.Add(new Errores("Sintatico", "se esperaba otro tipo de valor en vez de " + lst_token.ElementAt(indice).Lexema, lst_token.ElementAt(indice).Fila, lst_token.ElementAt(indice).Columna));
                    recuperar();
                    correcto = true;
                }
            }
        }
        private void recuperar()
        {
            bool salir = false;
            while (salir)
            {
                preanalisis = siguiente();
                if (preanalisis == Tipo.SIMBOLO_PUNTOYCOMA)
                {
                    salir = true;
                }
            }
            correcto = false;
        }
        private Tipo siguiente()
        {
            if ((lst_token.Count - 1) != indice)
            {
                indice++;
            }
            return lst_token.ElementAt(indice).Tipo;
        }
        public void parser()
        {
            match(Tipo.S);
            S();
            Console.WriteLine("terminado");
        }
        public void S()
        {
            sentencias();
        }
        private void sentencias()
        {
            sentecia();
            sentenciasP();

        }

        private void sentenciasP()
        {
            if (sentecia())
            {
                sentenciasP();
            }
            else
            {

            }
        }

        private bool sentecia()
        {
            if (preanalisis == Tipo.CREAR)
            {
                create();
                return true;
            }
            else if (preanalisis == Tipo.INSERTAR)
            {
                insert();
                return true;
            }
            else if (preanalisis == Tipo.ELIMINAR)
            {
                delete();
                return true;
            }
            else if (preanalisis == Tipo.SELECCIONAR)
            {
                select();
                return true;
            }
            else if (preanalisis == Tipo.ACTUALIZAR)
            {
                update();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void create()
        {
            match(Tipo.CREAR);
            match(Tipo.TABLA);
            match(Tipo.ID);
            match(Tipo.SIMBOLO_PARENTESISIZQ);
            parametros();
            match(Tipo.SIMBOLO_PARENTESISDER);
            match(Tipo.SIMBOLO_PUNTOYCOMA);
        }

        private void parametros()
        {
            parametro();
            parametrosP();
        }

        private void parametrosP()
        {
            match(Tipo.SIMBOLO_COMA);
            parametro();
            parametrosP();
        }

        private void parametro()
        {
            match(Tipo.ID);
            tipo();
        }

        private void tipo()
        {
            if (preanalisis == Tipo.ENTERO)
            {
                match(Tipo.ENTERO);
            }
            else if (preanalisis == Tipo.FLOTANTE)
            {
                match(Tipo.FLOTANTE);
            }
            else if (preanalisis == Tipo.CADENA)
            {
                match(Tipo.CADENA);
            }
            else if (preanalisis == Tipo.FECHA)
            {
                match(Tipo.FECHA);
            }
        }
        private void insert()
        {
            match(Tipo.INSERTAR);
            match(Tipo.EN);
            match(Tipo.ID);
            match(Tipo.VALORES);
            match(Tipo.SIMBOLO_PARENTESISIZQ);
            valores();
            match(Tipo.SIMBOLO_PARENTESISDER);
        }

        private void valores()
        {
            valor();
            valoresP();
        }

        private void valoresP()
        {

            if (preanalisis == Tipo.SIMBOLO_COMA)
            {
                match(Tipo.SIMBOLO_COMA);
                valor();
                valoresP();
            }
            else
            {

            }
        }

        private void valor()
        {
            if (preanalisis == Tipo.ENTERO)
            {
                match(Tipo.ENTERO);

            }
            else if (preanalisis == Tipo.FLOTANTE)
            {
                match(Tipo.FLOTANTE);

            }
            else if (preanalisis == Tipo.CADENA)
            {
                match(Tipo.CADENA);

            }
            else if (preanalisis == Tipo.FECHA)
            {
                match(Tipo.FECHA);

            }
        }

        private void select()
        {
            match(Tipo.SELECCIONAR);
            campos();
            match(Tipo.DE);
            tablas();
            where();
        }

        private void campos()
        {
            campo();
            camposP();
        }

        private void camposP()
        {
            if (preanalisis == Tipo.SIMBOLO_COMA)
            {
                match(Tipo.SIMBOLO_COMA);
                campo();
                camposP();
            }
            else { }
        }
        private void campo()
        {
            match(Tipo.TABLA);
            if (preanalisis == Tipo.SIMBOLO_PUNTO)
            {
                match(Tipo.SIMBOLO_PUNTO);
                match(Tipo.ID);
                como();
            }
            else if (preanalisis == Tipo.ID)
            {
                match(Tipo.ID);
                como();
            }
            else if (preanalisis == Tipo.SIMBOLO_ASTERISCO)
            {
                match(Tipo.SIMBOLO_ASTERISCO);
            }
        }

        private void como()
        {
            if (preanalisis == Tipo.COMO)
            {
                match(Tipo.COMO);
                match(Tipo.ID);
            }
            else { }
        }

        private void tablas()
        {
            match(Tipo.ID);
            tablasP();
        }
        private void tablasP()
        {
            if (preanalisis == Tipo.SIMBOLO_COMA)
            {
                match(Tipo.SIMBOLO_COMA);
                match(Tipo.ID);
                tablasP();
            }
            else { }
        }
        private void where()
        {
            if (preanalisis == Tipo.DONDE)
            {
                match(Tipo.DONDE);
                condiciones();
            }
            else { }

        }

        private void condiciones()
        {
            condicion();
            condicionesP();
        }

        private void condicionesP()
        {
            if (preanalisis == Tipo.Y)
            {
                match(Tipo.Y);
                condicion();
                condicionesP();
            }
            else if (preanalisis == Tipo.O)
            {
                match(Tipo.O);
                condicion();
                condicionesP();
            }
            else { }
        }

        private void condicion()
        {
            match(Tipo.ID);
            simbolo();
            valor();
        }

        private void simbolo()
        {
            if (preanalisis == Tipo.SIMBOLO_MAYORIGUAL)
            {
                match(Tipo.SIMBOLO_MAYORIGUAL);
            }
            else if (preanalisis == Tipo.SIMBOLO_MENORIGUAL)
            {
                match(Tipo.SIMBOLO_MENORIGUAL);
            }
            else if (preanalisis == Tipo.SIMBOLO_DIFERENTE)
            {
                match(Tipo.SIMBOLO_DIFERENTE);
            }
            else if (preanalisis == Tipo.SIMBOLO_IGUAL)
            {
                match(Tipo.SIMBOLO_IGUAL);
            }
            else if (preanalisis == Tipo.SIMBOLO_MAYOR)
            {
                match(Tipo.SIMBOLO_MAYOR);
            }
            else if (preanalisis == Tipo.SIMBOLO_MENOR)
            {
                match(Tipo.SIMBOLO_MENOR);
            }
        }
        private void delete()
        {
            match(Tipo.ELIMINAR);
            match(Tipo.DE);
            match(Tipo.ID);
            where();
        }

        private void update()
        {
            match(Tipo.ACTUALIZAR);
            match(Tipo.ID);
            match(Tipo.ESTABLECER);
            match(Tipo.SIMBOLO_PARENTESISIZQ);
            asignaciones();
            match(Tipo.SIMBOLO_PARENTESISDER);
            where();
        }

        private void asignaciones()
        {
            asignacion();
            asignacionesP();
        }

        private void asignacionesP()
        {
            if (preanalisis == Tipo.SIMBOLO_COMA)
            {
                match(Tipo.SIMBOLO_COMA);
                asignacion();
                asignacionesP();
            }
            else { }
        }

        private void asignacion()
        {
            match(Tipo.ID);
            match(Tipo.SIMBOLO_IGUAL);
            valor();
        }
    }
}
