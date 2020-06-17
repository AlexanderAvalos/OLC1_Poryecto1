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
        private bool correcto = true;
        Stack<Nodo> stack = new Stack<Nodo>();
        private int index = 0;

        public AnalizadorSintactico(List<Token> token_inicial, List<Errores> lst_error)
        {
            this.lst_token.Add(new Token(Tipo.S, "", 0, 0));
            this.lst_token.AddRange(token_inicial);
            this.lst_error = lst_error;
        }
        private int getIndex()
        {
            index++;
            return index;
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
            //mandar a graficar tu arbol
            Nodo raiz = stack.Pop();




        }
        public void S()
        {
            sentencias();
            Nodo nodo = new Nodo(getIndex(), "S", new List<Nodo>());
            nodo.Childs.Add(stack.Pop());
            stack.Push(nodo);
        }
        private void sentencias()
        {
            sentecia();
            sentenciasP();

            Nodo sentenciasPrima = stack.Pop();
            Nodo sent = stack.Pop();
            Nodo nodo = new Nodo(getIndex(), "sentencias", new List<Nodo>());
            Nodo nodo2 = new Nodo(getIndex(), "sentencia", new List<Nodo>());
            nodo2.Childs.Add(sent);
            nodo.Childs.Add(nodo2);
            nodo.Childs.Add(sentenciasPrima);
            stack.Push(nodo);

        }

        private void sentenciasP()
        {
            if (sentecia())
            {
                sentenciasP();
                Nodo sentenciasPrima = stack.Pop();
                Nodo sent = stack.Pop();
                Nodo nodo = new Nodo(getIndex(), "sentenciasP", new List<Nodo>());
                Nodo nodo2 = new Nodo(getIndex(), "sentencia", new List<Nodo>());
                nodo2.Childs.Add(sent);
                nodo.Childs.Add(nodo2);
                nodo.Childs.Add(sentenciasPrima);
                stack.Push(nodo);

            }
            else
            {
                Nodo nodo = new Nodo(getIndex(), "epsilon", null);
                stack.Push(nodo);

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
            Nodo nodo = new Nodo(getIndex(), "create", new List<Nodo>());
            nodo.Childs.Add(new Nodo(getIndex(), "CREAR", null));
            match(Tipo.CREAR);
            nodo.Childs.Add(new Nodo(getIndex(), "TABLA", null));
            match(Tipo.TABLA);
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.ID);
            nodo.Childs.Add(new Nodo(getIndex(), "(", null));
            match(Tipo.SIMBOLO_PARENTESISIZQ);
            parametros();
            Nodo param = stack.Pop();
            Nodo nodo2 = new Nodo(getIndex(), "PARAMETROS", new List<Nodo>());
            nodo2.Childs.Add(param);
            nodo.Childs.Add(nodo2);
            nodo.Childs.Add(new Nodo(getIndex(), ")", null));
            match(Tipo.SIMBOLO_PARENTESISDER);
            nodo.Childs.Add(new Nodo(getIndex(), ";", null));
            match(Tipo.SIMBOLO_PUNTOYCOMA);
            stack.Push(nodo);
        }

        private void parametros()
        {
            parametro();
            parametrosP();
            Nodo parametroPrima = stack.Pop();
            Nodo param = stack.Pop();
            Nodo nodo = new Nodo(getIndex(), "parametros", new List<Nodo>());
            Nodo nodo2 = new Nodo(getIndex(), "parametro", new List<Nodo>());
            nodo2.Childs.Add(param);
            nodo.Childs.Add(nodo2);
            nodo.Childs.Add(parametroPrima);
            stack.Push(nodo);
        }

        private void parametrosP()
        {

            Nodo nodo = new Nodo(getIndex(), "parametrosP", new List<Nodo>());
            Nodo nodo2 = new Nodo(getIndex(), "parametro", new List<Nodo>());
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.SIMBOLO_COMA);
            parametro();
            parametrosP();
            Nodo parametroPrima = stack.Pop();
            Nodo param = stack.Pop();
            nodo2.Childs.Add(param);
            nodo.Childs.Add(nodo2);
            nodo.Childs.Add(parametroPrima);
            stack.Push(nodo);
        }

        private void parametro()
        {
            Nodo nodo = new Nodo(getIndex(), "parametro", new List<Nodo>());
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.ID);
            tipo();
            Nodo tipoD = stack.Pop();
            Nodo nodo2 = new Nodo(getIndex(), "Tipo", new List<Nodo>());
            nodo2.Childs.Add(tipoD);
            nodo.Childs.Add(nodo2);
            stack.Push(nodo);
        }

        private void tipo()
        {
            Nodo nodo = new Nodo(getIndex(), "tipo", new List<Nodo>());
            if (preanalisis == Tipo.TIPO_ENTERO)
            {
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.TIPO_ENTERO);
            }
            else if (preanalisis == Tipo.TIPO_FLOTANTE)
            {
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.TIPO_FLOTANTE);
            }
            else if (preanalisis == Tipo.TIPO_CADENA)
            {
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.TIPO_CADENA);
            }
            else if (preanalisis == Tipo.TIPO_FECHA)
            {
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.TIPO_FECHA);
            }
            stack.Push(nodo);
        }
        private void insert()
        {
            Nodo nodo = new Nodo(getIndex(), "insert", new List<Nodo>());
            nodo.Childs.Add(new Nodo(getIndex(), "INSERTAR", null));
            match(Tipo.INSERTAR);
            nodo.Childs.Add(new Nodo(getIndex(), "EN", null));
            match(Tipo.EN);
            nodo.Childs.Add(new Nodo(getIndex(), "ID", null));
            match(Tipo.ID);
            nodo.Childs.Add(new Nodo(getIndex(), "VALORES", null));
            match(Tipo.VALORES);
            nodo.Childs.Add(new Nodo(getIndex(), "(", null));
            match(Tipo.SIMBOLO_PARENTESISIZQ);
            valores();
            Nodo val = stack.Pop();
            Nodo nodo2 = new Nodo(getIndex(), "valores", new List<Nodo>());
            nodo2.Childs.Add(val);
            nodo.Childs.Add(nodo2);
            nodo.Childs.Add(new Nodo(getIndex(), ")", null));
            match(Tipo.SIMBOLO_PARENTESISDER);
            stack.Push(nodo);
        }

        private void valores()
        {
            valor();
            valoresP();
            Nodo valoresPrima = stack.Pop();
            Nodo val = stack.Pop();
            Nodo nodo = new Nodo(getIndex(), "valores", new List<Nodo>());
            Nodo nodo2 = new Nodo(getIndex(), "valor", new List<Nodo>());
            nodo2.Childs.Add(val);
            nodo.Childs.Add(nodo2);
            nodo.Childs.Add(valoresPrima);
            stack.Push(nodo);
        }

        private void valoresP()
        {

            if (preanalisis == Tipo.SIMBOLO_COMA)
            {
                Nodo nodo = new Nodo(getIndex(), "valoresP", new List<Nodo>());
                Nodo nodo2 = new Nodo(getIndex(), "valor", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.SIMBOLO_COMA);
                valor();
                valoresP();
                Nodo valoresPrima = stack.Pop();
                Nodo val = stack.Pop();
                nodo2.Childs.Add(val);
                nodo.Childs.Add(nodo2);
                nodo.Childs.Add(valoresPrima);
                stack.Push(nodo);
            }
            else
            {
                Nodo nodo = new Nodo(getIndex(), "epsilon", null);
                stack.Push(nodo);
            }
        }

        private void valor()
        {
            Nodo nodo = new Nodo(getIndex(), "valor", new List<Nodo>());
            if (preanalisis == Tipo.ENTERO)
            {
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.ENTERO);
            }
            else if (preanalisis == Tipo.FLOTANTE)
            {
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.FLOTANTE);
            }
            else if (preanalisis == Tipo.CADENA)
            {
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.CADENA);

            }
            else if (preanalisis == Tipo.FECHA)
            {
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.FECHA);
            }
        }

        private void select()
        {
            Nodo nodo = new Nodo(getIndex(), "select", new List<Nodo>());
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.SELECCIONAR);
            campos();
            Nodo camposs = stack.Pop();
            Nodo nodo2 = new Nodo(getIndex(), "campos", new List<Nodo>());
            nodo2.Childs.Add(camposs);
            nodo.Childs.Add(nodo2);
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.DE);
            tablas();
            Nodo tabl = stack.Pop();
            Nodo nodo3 = new Nodo(getIndex(), "tablas", new List<Nodo>());
            nodo3.Childs.Add(tabl);
            nodo.Childs.Add(nodo3);
            where();
            Nodo whe = stack.Pop();
            Nodo nodo4 = new Nodo(getIndex(), "where", new List<Nodo>());
            nodo4.Childs.Add(whe);
            nodo.Childs.Add(nodo4);
            stack.Push(nodo);
        }

        private void campos()
        {
            campo();
            camposP();
            Nodo camposPrima = stack.Pop();
            Nodo camposs = stack.Pop();
            Nodo nodo = new Nodo(getIndex(), "campos", new List<Nodo>());
            Nodo nodo2 = new Nodo(getIndex(), "campo", new List<Nodo>());
            nodo2.Childs.Add(camposs);
            nodo.Childs.Add(nodo2);
            nodo.Childs.Add(camposPrima);
            stack.Push(nodo);
        }

        private void camposP()
        {
            if (preanalisis == Tipo.SIMBOLO_COMA)
            {
                Nodo nodo = new Nodo(getIndex(), "camposP", new List<Nodo>());

                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.SIMBOLO_COMA);
                campo();
                camposP();
                Nodo camposPrima = stack.Pop();
                Nodo nodo2 = new Nodo(getIndex(), "campo", new List<Nodo>());
                Nodo camp = stack.Pop();
                nodo2.Childs.Add(camp);
                nodo.Childs.Add(nodo2);
                nodo.Childs.Add(camposPrima);
                stack.Push(nodo);
            }
            else
            {
                Nodo nodo = new Nodo(getIndex(), "Epsilon", new List<Nodo>());
                stack.Push(nodo);
            }
        }
        private void campo()
        {

            if (preanalisis == Tipo.ID)
            {
                Nodo nodo = new Nodo(getIndex(), "campo", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.ID);
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.SIMBOLO_PUNTO);
                campoP();
                Nodo camp = stack.Pop();
                Nodo nodo2 = new Nodo(getIndex(), "camposP", new List<Nodo>());
                nodo2.Childs.Add(camp);
                nodo.Childs.Add(nodo2);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.ID)
            {
                Nodo nodo = new Nodo(getIndex(), "campo", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.ID);
                como();
                Nodo co = stack.Pop();
                Nodo nodo2 = new Nodo(getIndex(), "como", new List<Nodo>());
                nodo2.Childs.Add(co);
                nodo.Childs.Add(nodo2);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.SIMBOLO_ASTERISCO)
            {
                Nodo nodo = new Nodo(getIndex(), "campo", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.SIMBOLO_ASTERISCO);
                stack.Push(nodo);
            }
        }
        private void campoP()
        {
            if (preanalisis == Tipo.ID)
            {
                Nodo nodo = new Nodo(getIndex(), "campoP", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.ID);
                como();
                Nodo co = stack.Pop();
                Nodo nodo2 = new Nodo(getIndex(), "como", new List<Nodo>());
                nodo2.Childs.Add(co);
                nodo.Childs.Add(nodo2);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.SIMBOLO_ASTERISCO)
            {
                Nodo nodo = new Nodo(getIndex(), "campo", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.SIMBOLO_ASTERISCO);
                stack.Push(nodo);
            }
        }
        private void como()
        {
            if (preanalisis == Tipo.COMO)
            {
                Nodo nodo = new Nodo(getIndex(), "como", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.COMO);
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.ID);
                stack.Push(nodo);
            }
            else
            {
                Nodo nodo = new Nodo(getIndex(), "Epsilon", new List<Nodo>());
                stack.Push(nodo);
            }
        }

        private void tablas()
        {
            Nodo nodo = new Nodo(getIndex(), "tablas", new List<Nodo>());
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.ID);
            tablasP();
            Nodo tablasPrima = stack.Pop();
            Nodo nodo2 = new Nodo(getIndex(), "tablasP", new List<Nodo>());
            nodo2.Childs.Add(tablasPrima);
            nodo.Childs.Add(nodo2);
            stack.Push(nodo);
        }
        private void tablasP()
        {
            if (preanalisis == Tipo.SIMBOLO_COMA)
            {
                Nodo nodo = new Nodo(getIndex(), "tablasP", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.SIMBOLO_COMA);
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.ID);
                tablasP();
                Nodo tablasPrima = stack.Pop();
                Nodo nodo2 = new Nodo(getIndex(), "tablasP", new List<Nodo>());
                nodo2.Childs.Add(tablasPrima);
                nodo.Childs.Add(nodo2);
                stack.Push(nodo);
            }
            else
            {
                Nodo nodo = new Nodo(getIndex(), "Epsilon", new List<Nodo>());
                stack.Push(nodo);
            }
        }
        private void where()
        {
            if (preanalisis == Tipo.DONDE)
            {
                Nodo nodo = new Nodo(getIndex(), "where", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.DONDE);
                condiciones();
                Nodo condicio = stack.Pop();
                Nodo nodo2 = new Nodo(getIndex(), "condicones", new List<Nodo>());
                nodo2.Childs.Add(condicio);
                nodo.Childs.Add(nodo2);
                stack.Push(nodo);
            }
            else
            {
                Nodo nodo = new Nodo(getIndex(), "Epsilon", new List<Nodo>());
                stack.Push(nodo);
            }

        }

        private void condiciones()
        {
            condicion();
            condicionesP();
            Nodo condicionPrima = stack.Pop();
            Nodo cond = stack.Pop();
            Nodo nodo = new Nodo(getIndex(), "condiciones", new List<Nodo>());
            Nodo nodo2 = new Nodo(getIndex(), "condicion", new List<Nodo>());
            nodo2.Childs.Add(cond);
            nodo.Childs.Add(nodo2);
            nodo.Childs.Add(condicionPrima);
            stack.Push(nodo);
        }

        private void condicionesP()
        {
            if (preanalisis == Tipo.Y)
            {
                Nodo nodo = new Nodo(getIndex(), "condicionesP", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.Y);
                condicion();
                condicionesP();
                Nodo condicionPrima = stack.Pop();
                Nodo cond = stack.Pop();
                Nodo nodo2 = new Nodo(getIndex(), "condicion", new List<Nodo>());
                nodo2.Childs.Add(cond);
                nodo.Childs.Add(nodo2);
                nodo.Childs.Add(condicionPrima);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.O)
            {
                Nodo nodo = new Nodo(getIndex(), "condicionesP", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.O);
                condicion();
                condicionesP();
                Nodo condicionPrima = stack.Pop();
                Nodo cond = stack.Pop();
                Nodo nodo2 = new Nodo(getIndex(), "condicion", new List<Nodo>());
                nodo2.Childs.Add(cond);
                nodo.Childs.Add(nodo2);
                nodo.Childs.Add(condicionPrima);
                stack.Push(nodo);
            }
            else
            {
                Nodo nodo = new Nodo(getIndex(), "Epsilon", new List<Nodo>());
                stack.Push(nodo);
            }
        }

        private void condicion()
        {
            Nodo nodo = new Nodo(getIndex(), "condicion", new List<Nodo>());
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.ID);
            simbolo();
            valor();
            Nodo simb = stack.Pop();
            Nodo val = stack.Pop();
            Nodo nodo3 = new Nodo(getIndex(), "simbolo", new List<Nodo>());
            Nodo nodo2 = new Nodo(getIndex(), "valor", new List<Nodo>());
            nodo2.Childs.Add(val);
            nodo3.Childs.Add(simb);
            nodo.Childs.Add(nodo2);
            nodo.Childs.Add(nodo3);
            stack.Push(nodo);
        }

        private void simbolo()
        {
            if (preanalisis == Tipo.SIMBOLO_MAYORIGUAL)
            {
                Nodo nodo = new Nodo(getIndex(), "simbolo", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.SIMBOLO_MAYORIGUAL);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.SIMBOLO_MENORIGUAL)
            {
                Nodo nodo = new Nodo(getIndex(), "simbolo", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.SIMBOLO_MENORIGUAL);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.SIMBOLO_DIFERENTE)
            {
                Nodo nodo = new Nodo(getIndex(), "simbolo", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.SIMBOLO_DIFERENTE);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.SIMBOLO_IGUAL)
            {
                Nodo nodo = new Nodo(getIndex(), "simbolo", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.SIMBOLO_IGUAL);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.SIMBOLO_MAYOR)
            {
                Nodo nodo = new Nodo(getIndex(), "simbolo", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.SIMBOLO_MAYOR);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.SIMBOLO_MENOR)
            {
                Nodo nodo = new Nodo(getIndex(), "simbolo", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.SIMBOLO_MENOR);
                stack.Push(nodo);
            }
        }
        private void delete()
        {
            Nodo nodo = new Nodo(getIndex(), "delete", new List<Nodo>());
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.ELIMINAR);
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.DE);
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.ID);
            where();
            Nodo whe = stack.Pop();
            Nodo nodo2 = new Nodo(getIndex(), "where", new List<Nodo>());
            nodo2.Childs.Add(whe);
            nodo.Childs.Add(nodo2);
            stack.Push(nodo);
        }

        private void update()
        {
            Nodo nodo = new Nodo(getIndex(), "update", new List<Nodo>());
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.ACTUALIZAR);
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.ID);
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.ESTABLECER);
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.SIMBOLO_PARENTESISIZQ);
            asignaciones();
            Nodo asig = stack.Pop();
            Nodo nodo3 = new Nodo(getIndex(), "asignaciones", new List<Nodo>());
            nodo3.Childs.Add(asig);
            nodo.Childs.Add(nodo3);
            stack.Push(nodo);
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.SIMBOLO_PARENTESISDER);
            where();
            Nodo co = stack.Pop();
            Nodo nodo2 = new Nodo(getIndex(), "where", new List<Nodo>());
            nodo2.Childs.Add(co);
            nodo.Childs.Add(nodo2);
            stack.Push(nodo);
        }

        private void asignaciones()
        {
            asignacion();
            asignacionesP();
            Nodo asignacionPrima = stack.Pop();
            Nodo asig = stack.Pop();
            Nodo nodo = new Nodo(getIndex(), "asignaciones", new List<Nodo>());
            Nodo nodo2 = new Nodo(getIndex(), "asig", new List<Nodo>());
            nodo2.Childs.Add(asig);
            nodo.Childs.Add(nodo2);
            nodo.Childs.Add(asignacionPrima);
            stack.Push(nodo);
        }

        private void asignacionesP()
        {
            if (preanalisis == Tipo.SIMBOLO_COMA)
            {
                Nodo nodo = new Nodo(getIndex(), "asignacionesP", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.SIMBOLO_COMA);
                asignacion();
                asignacionesP();
                Nodo asignacionPrima = stack.Pop();
                Nodo asig = stack.Pop();
                Nodo nodo2 = new Nodo(getIndex(), "asig", new List<Nodo>());
                nodo2.Childs.Add(asig);
                nodo.Childs.Add(nodo2);
                nodo.Childs.Add(asignacionPrima);
                stack.Push(nodo);
            }
            else
            {
                Nodo nodo = new Nodo(getIndex(), "Epsilon", new List<Nodo>());
                stack.Push(nodo);
            }
        }

        private void asignacion()
        {
            Nodo nodo = new Nodo(getIndex(), "asignacion", new List<Nodo>());
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.SIMBOLO_COMA);
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.ID);
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            match(Tipo.SIMBOLO_IGUAL);
            valor();
            Nodo val = stack.Pop();
            Nodo nodo2 = new Nodo(getIndex(), "valor", new List<Nodo>());
            nodo2.Childs.Add(val);
            nodo.Childs.Add(nodo2);
            stack.Push(nodo);
        }
    }
}
