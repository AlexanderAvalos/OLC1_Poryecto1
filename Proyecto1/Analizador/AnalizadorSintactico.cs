using Proyecto1.Clases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1.Analizador
{
    class AnalizadorSintactico
    {
        private static StringBuilder print;
        public string rpng, ruta;
        private Tipo preanalisis = Tipo.S;
        private List<Token> lst_token = new List<Token>();
        private List<Errores> lst_error = new List<Errores>();
        private List<Tabla> lst_tablas = new List<Tabla>();
        private int indice = 0;
        private bool correcto = true;
        private Stack<Nodo> stack = new Stack<Nodo>();
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
            Nodo raiz = stack.Pop();
            graficar(raiz);
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
            Nodo sentenci = stack.Pop();
            Nodo sent = stack.Pop();
            Nodo nodo = new Nodo(getIndex(), "sentencias", new List<Nodo>());
            Nodo nodo2 = new Nodo(getIndex(), "sentencia", new List<Nodo>());
            nodo2.Childs.Add(sent);
            nodo.Childs.Add(nodo2);
            nodo.Childs.Add(sentenci);
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
            Tabla actual = new Tabla();
            string nombretabla;
            Nodo nodo = new Nodo(getIndex(), "create", new List<Nodo>());
            nodo.Childs.Add(new Nodo(getIndex(), "CREAR", null));
            match(Tipo.CREAR);
            nodo.Childs.Add(new Nodo(getIndex(), "TABLA", null));
            match(Tipo.TABLA);
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            nombretabla = lst_token.ElementAt(indice).Lexema;
            Console.WriteLine(nombretabla);
           
            actual.Nombre_tabla = nombretabla;
            match(Tipo.ID);
            nodo.Childs.Add(new Nodo(getIndex(), "(", null));
            match(Tipo.SIMBOLO_PARENTESISIZQ);
            List<Columna> lst = parametros();
            Nodo param = stack.Pop();
            nodo.Childs.Add(param);
            nodo.Childs.Add(new Nodo(getIndex(), ")", null));
            match(Tipo.SIMBOLO_PARENTESISDER);
            nodo.Childs.Add(new Nodo(getIndex(), ";", null));
            match(Tipo.SIMBOLO_PUNTOYCOMA);
            stack.Push(nodo);
            actual.Nombre_tabla = nombretabla;
            actual.Columnas = lst;
            lst_tablas.Add(actual);
        }

        private List<Columna> parametros()
        {
            Columna val = parametro();
            List<Columna> lst = parametrosP();
            lst.Add(val);
            Nodo paramet = stack.Pop();
            Nodo param = stack.Pop();
            Nodo nodo = new Nodo(getIndex(), "parametros", new List<Nodo>());
            nodo.Childs.Add(param);
            nodo.Childs.Add(paramet);
            stack.Push(nodo);
            return lst;
        }

        private List<Columna> parametrosP()
        {

            if (preanalisis == Tipo.SIMBOLO_COMA)
            {
                Nodo nodo = new Nodo(getIndex(), "parametrosP", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                match(Tipo.SIMBOLO_COMA);
                Columna val = parametro();
                List<Columna> lst = parametrosP();
                lst.Add(val);
                Nodo parametroPrima = stack.Pop();
                Nodo param = stack.Pop();
                nodo.Childs.Add(param);
                nodo.Childs.Add(parametroPrima);
                stack.Push(nodo);
                return lst;
            }
            else
            {
                Nodo nodo = new Nodo(getIndex(), "epsilon", null);
                stack.Push(nodo);
                return new List<Columna>();
            }
        }

        private Columna parametro()
        {
            string id;
            Nodo nodo = new Nodo(getIndex(), "parametro", new List<Nodo>());
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            id = lst_token.ElementAt(indice).Lexema;
            match(Tipo.ID);
            string tip = tipo();
            Console.WriteLine(id+" "+ tip);
            Columna columna_actual = new Columna(id, tip);
            Nodo tipoD = stack.Pop();
            Nodo nodo2 = new Nodo(getIndex(), "Tipo", new List<Nodo>());
            nodo2.Childs.Add(tipoD);
            nodo.Childs.Add(nodo2);
            stack.Push(nodo);
            return columna_actual;
        }
    
        private string tipo()
        {
            String tip;
            if (preanalisis == Tipo.TIPO_ENTERO)
            {
                Nodo nodo = new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null);
                tip = lst_token.ElementAt(indice).Lexema;
                match(Tipo.TIPO_ENTERO);
                stack.Push(nodo);
                return tip;
            }
            else if (preanalisis == Tipo.TIPO_FLOTANTE)
            {
                Nodo nodo = new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null);
                tip = lst_token.ElementAt(indice).Lexema;
                match(Tipo.TIPO_FLOTANTE);
                stack.Push(nodo);
                return tip;
            }
            else if (preanalisis == Tipo.TIPO_CADENA)
            {
                Nodo nodo = new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null);
                tip = lst_token.ElementAt(indice).Lexema;
                match(Tipo.TIPO_CADENA);
                stack.Push(nodo);
                return tip;
            }
            else if (preanalisis == Tipo.TIPO_FECHA)
            {
                Nodo nodo = new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null);
                tip = lst_token.ElementAt(indice).Lexema;
                match(Tipo.TIPO_FECHA);
                stack.Push(nodo);
                return tip;
            }
            else
            {
                return "";
            }

        }
        private void insert()
        {
            string nombretabla;
            Nodo nodo = new Nodo(getIndex(), "insert", new List<Nodo>());
            nodo.Childs.Add(new Nodo(getIndex(), "INSERTAR", null));
            match(Tipo.INSERTAR);
            nodo.Childs.Add(new Nodo(getIndex(), "EN", null));
            match(Tipo.EN);
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            nombretabla = lst_token.ElementAt(indice).Lexema;
            Tabla actual = buscar(nombretabla);
            match(Tipo.ID);
            nodo.Childs.Add(new Nodo(getIndex(), "VALORES", null));
            match(Tipo.VALORES);
            nodo.Childs.Add(new Nodo(getIndex(), "(", null));
            match(Tipo.SIMBOLO_PARENTESISIZQ);
            List<string> lst_valores = valores();
            if (actual.Columnas.Count == lst_valores.Count)
            {
                Dictionary<string, object> aux = new Dictionary<string, object>();
                for (int i = 0; i < actual.Columnas.Count; i++)
                {
                    string llave = Convert.ToString(actual.Columnas.ElementAt(i).Nombre);
                    aux.Add(llave, lst_valores.ElementAt(i));

                }
                actual.Filas.Add(aux);
            }
            Nodo val = stack.Pop();
            nodo.Childs.Add(val);
            nodo.Childs.Add(new Nodo(getIndex(), ")", null));
            match(Tipo.SIMBOLO_PARENTESISDER);
            nodo.Childs.Add(new Nodo(getIndex(), ";", null));
            match(Tipo.SIMBOLO_PUNTOYCOMA);
            stack.Push(nodo);
        }

        private Tabla buscar(string nombretabla)
        {
            foreach (Tabla item in lst_tablas)
            {
                if (nombretabla == item.Nombre_tabla)
                {
                    return item;
                }
            }
            return null;
        }
        private List<string> valores()
        {
            string valo = valor();
            List<String> lst_valores = valoresP();
            lst_valores.Add(valo);
            Nodo valoresPrima = stack.Pop();
            Nodo val = stack.Pop();
            Nodo nodo = new Nodo(getIndex(), "valores", new List<Nodo>());
            Nodo nodo2 = new Nodo(getIndex(), "valor", new List<Nodo>());
            nodo2.Childs.Add(val);
            nodo.Childs.Add(nodo2);
            nodo.Childs.Add(valoresPrima);
            stack.Push(nodo);
            return lst_valores;
        }

        private List<string> valoresP()
        {

            if (preanalisis == Tipo.SIMBOLO_COMA)
            {
                Nodo nodo = new Nodo(getIndex(), "valoresP", new List<Nodo>());
                Nodo nodo2 = new Nodo(getIndex(), "valor", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), ",", null));
                match(Tipo.SIMBOLO_COMA);
                string valo = valor();
                List<String> lst_valores = valoresP();
                lst_valores.Add(valo);
                Nodo valoresPrima = stack.Pop();
                Nodo val = stack.Pop();
                nodo2.Childs.Add(val);
                nodo.Childs.Add(nodo2);
                nodo.Childs.Add(valoresPrima);
                stack.Push(nodo);
                return lst_valores;
            }
            else
            {
                Nodo nodo = new Nodo(getIndex(), "epsilon", null);
                stack.Push(nodo);
                return new List<string>();
            }
        }

        private string valor()
        {
            string valore = "";
            if (preanalisis == Tipo.ENTERO)
            {
                Nodo nodo = new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null);
                valore = lst_token.ElementAt(indice).Lexema;
                match(Tipo.ENTERO);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.FLOTANTE)
            {
                Nodo nodo = new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null);
                valore = lst_token.ElementAt(indice).Lexema;
                match(Tipo.FLOTANTE);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.CADENA)
            {
                Nodo nodo = new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema.ToString().Replace('"', ' ').Trim(), null);
                valore = lst_token.ElementAt(indice).Lexema;
                match(Tipo.CADENA);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.FECHA)
            {
                Nodo nodo = new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null);
                valore = lst_token.ElementAt(indice).Lexema;
                match(Tipo.FECHA);
                stack.Push(nodo);
            }
           
            return valore;
        }

        private void select()
        {
            Nodo nodo = new Nodo(getIndex(), "select", new List<Nodo>());
            nodo.Childs.Add(new Nodo(getIndex(), "SELECCIONAR", null));
            match(Tipo.SELECCIONAR);
            campos();
            Nodo camposs = stack.Pop();
            Nodo nodo2 = new Nodo(getIndex(), "campos", new List<Nodo>());
            nodo2.Childs.Add(camposs);
            nodo.Childs.Add(nodo2);
            nodo.Childs.Add(new Nodo(getIndex(), "DE", null));
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
            match(Tipo.SIMBOLO_PUNTOYCOMA);
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
                nodo.Childs.Add(new Nodo(getIndex(), ",", null));
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
                string id = lst_token.ElementAt(indice).Lexema;
                Console.Write(id+ "  ");
                match(Tipo.ID);
                campoP();
                Nodo campp = stack.Pop();
                nodo.Childs.Add(campp);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.SIMBOLO_ASTERISCO)
            {
                Nodo nodo = new Nodo(getIndex(), "*", null);
                string todo = lst_token.ElementAt(indice).Lexema;
                Console.WriteLine(todo);
                match(Tipo.SIMBOLO_ASTERISCO);
                stack.Push(nodo);
            }
        }
        private void campoP()
        {
            if (preanalisis == Tipo.SIMBOLO_PUNTO)
            {
                Nodo nodo = new Nodo(getIndex(), "campoP", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), ".", null));
                match(Tipo.SIMBOLO_PUNTO);
                campoB();
                Nodo campB = stack.Pop();
                nodo.Childs.Add(campB);
                stack.Push(nodo);
            }
            else if (como())
            {
                Nodo nodo = new Nodo(getIndex(), "como", new List<Nodo>());
                Nodo com = stack.Pop();
                nodo.Childs.Add(com);
                stack.Push(nodo);
            }
        }
        private void campoB()
        {
            if (preanalisis == Tipo.ID)
            {
                Nodo nodo = new Nodo(getIndex(), "campoB", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                string id = lst_token.ElementAt(indice).Lexema;
                Console.WriteLine(id);
                match(Tipo.ID);
                como();
                Nodo com = stack.Pop();
                nodo.Childs.Add(com);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.SIMBOLO_ASTERISCO)
            {
                Nodo nodo = new Nodo(getIndex(), "*", null);
                match(Tipo.SIMBOLO_ASTERISCO);
                string todo = lst_token.ElementAt(indice).Lexema;
                Console.WriteLine(todo);
                stack.Push(nodo);
            }
        }
        private bool como()
        {
            if (preanalisis == Tipo.COMO)
            {
                Nodo nodo = new Nodo(getIndex(), "como", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), "COMO", null));
                match(Tipo.COMO);
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                string alias = lst_token.ElementAt(indice).Lexema;
                Console.WriteLine(alias);
                match(Tipo.ID);
                stack.Push(nodo);
                return true;
            }
            else
            {
                Nodo nodo = new Nodo(getIndex(), "Epsilon", new List<Nodo>());
                stack.Push(nodo);
                return false;
            }
        }

        private void tablas()
        {
            Nodo nodo = new Nodo(getIndex(), "tablas", new List<Nodo>());
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            string tabla = lst_token.ElementAt(indice).Lexema;
            Console.WriteLine(tabla);
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
                nodo.Childs.Add(new Nodo(getIndex(), ",", null));
                match(Tipo.SIMBOLO_COMA);
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                string ta = lst_token.ElementAt(indice).Lexema;
                Console.WriteLine(ta);
                match(Tipo.ID);
                tablasP();
                Nodo tablasPrima = stack.Pop();
                nodo.Childs.Add(tablasPrima);
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
                nodo.Childs.Add(new Nodo(getIndex(), "DONDE", null));
                match(Tipo.DONDE);
                condiciones();
                Nodo condicio = stack.Pop();
                nodo.Childs.Add(condicio);
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
            nodo.Childs.Add(cond);
            nodo.Childs.Add(condicionPrima);
            stack.Push(nodo);
        }

        private void condicionesP()
        {
            if (preanalisis == Tipo.Y)
            {
                Nodo nodo = new Nodo(getIndex(), "condicionesP", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), "Y", null));
                match(Tipo.Y);
                condicion();
                condicionesP();
                Nodo condicionPrima = stack.Pop();
                Nodo cond = stack.Pop();
                nodo.Childs.Add(cond);
                nodo.Childs.Add(condicionPrima);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.O)
            {
                Nodo nodo = new Nodo(getIndex(), "condicionesP", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), "O", null));
                match(Tipo.O);
                condicion();
                condicionesP();
                Nodo condicionPrima = stack.Pop();
                Nodo cond = stack.Pop();
                nodo.Childs.Add(cond);
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
            string todo = lst_token.ElementAt(indice).Lexema;
            Console.WriteLine(todo);
            match(Tipo.ID);
            condicionP();
            Nodo condicionPrima = stack.Pop();
            nodo.Childs.Add(condicionPrima);
            stack.Push(nodo);
        }

        private void condicionP()
        {
            if (preanalisis == Tipo.SIMBOLO_PUNTO)
            {
                Nodo nodo = new Nodo(getIndex(), "condicionP", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), ".", null));
                match(Tipo.SIMBOLO_PUNTO);
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                string todo = lst_token.ElementAt(indice).Lexema;
                Console.WriteLine(todo);
                match(Tipo.ID);
                simbolo();
                valorP();
                Nodo simb = stack.Pop();
                Nodo val = stack.Pop();
                Nodo nodo3 = new Nodo(getIndex(), "simbolo", new List<Nodo>());
                Nodo nodo2 = new Nodo(getIndex(), "valorP", new List<Nodo>());
                nodo2.Childs.Add(val);
                nodo3.Childs.Add(simb);
                nodo.Childs.Add(nodo2);
                nodo.Childs.Add(nodo3);
                nodo.Childs.Add(new Nodo(getIndex(), ";", null));
                match(Tipo.SIMBOLO_PUNTOYCOMA);
                stack.Push(nodo);
            }
            else
            {
                Nodo nodo = new Nodo(getIndex(), "condicionP", new List<Nodo>());
                simbolo();
                valorP();
                Nodo simb = stack.Pop();
                Nodo val = stack.Pop();
                nodo.Childs.Add(val);
                nodo.Childs.Add(simb);
                nodo.Childs.Add(new Nodo(getIndex(), ";", null));
                match(Tipo.SIMBOLO_PUNTOYCOMA);
                stack.Push(nodo);
            }
        }
        private void valorP()
        {
            if (preanalisis == Tipo.ID)
            {
                Nodo nodo = new Nodo(getIndex(), "valorP", new List<Nodo>());
                nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                string todo = lst_token.ElementAt(indice).Lexema;
                Console.WriteLine(todo);
                match(Tipo.ID);
                if (preanalisis == Tipo.SIMBOLO_PUNTO)
                {
                    nodo.Childs.Add(new Nodo(getIndex(), ".", null));
                    match(Tipo.SIMBOLO_PUNTO);
                    string id = lst_token.ElementAt(indice).Lexema;
                    Console.WriteLine(id);
                    nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
                    match(Tipo.ID);
                }
                stack.Push(nodo);
            }
            else
            {
                valor();
                Nodo nodo = new Nodo(getIndex(), "valorP", new List<Nodo>());
                Nodo nodo2 = new Nodo(getIndex(), "valor", new List<Nodo>());
                Nodo condicionPrima = stack.Pop();
                nodo2.Childs.Add(condicionPrima);
                nodo.Childs.Add(nodo2);
                stack.Push(nodo);
            }
        }
        private void simbolo()
        {
            string todo= "";
            if (preanalisis == Tipo.SIMBOLO_MAYORIGUAL)
            {
                Nodo nodo = new Nodo(getIndex(), ">=", null);
                 todo = lst_token.ElementAt(indice).Lexema;
                
                match(Tipo.SIMBOLO_MAYORIGUAL);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.SIMBOLO_MENORIGUAL)
            {
                Nodo nodo = new Nodo(getIndex(), "<=", null);
                todo = lst_token.ElementAt(indice).Lexema;
                match(Tipo.SIMBOLO_MENORIGUAL);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.SIMBOLO_DIFERENTE)
            {
                Nodo nodo = new Nodo(getIndex(), "!=", null);
                todo = lst_token.ElementAt(indice).Lexema;
                match(Tipo.SIMBOLO_DIFERENTE);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.SIMBOLO_IGUAL)
            {
                Nodo nodo = new Nodo(getIndex(), "=", null);
                todo = lst_token.ElementAt(indice).Lexema;
                match(Tipo.SIMBOLO_IGUAL);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.SIMBOLO_MAYOR)
            {
                Nodo nodo = new Nodo(getIndex(), ">", null);
                todo = lst_token.ElementAt(indice).Lexema;
                match(Tipo.SIMBOLO_MAYOR);
                stack.Push(nodo);
            }
            else if (preanalisis == Tipo.SIMBOLO_MENOR)
            {
                Nodo nodo = new Nodo(getIndex(), "<", null);
                todo = lst_token.ElementAt(indice).Lexema;
                match(Tipo.SIMBOLO_MENOR);
                stack.Push(nodo);
            }
            Console.WriteLine(todo);
        }
        private void delete()
        {
            Nodo nodo = new Nodo(getIndex(), "delete", new List<Nodo>());
            nodo.Childs.Add(new Nodo(getIndex(), "ELIMINAR", null));
            match(Tipo.ELIMINAR);
            nodo.Childs.Add(new Nodo(getIndex(), "DE", null));
            match(Tipo.DE);
            string id = lst_token.ElementAt(indice).Lexema;
            Console.WriteLine(id);
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
            nodo.Childs.Add(new Nodo(getIndex(), "ACTUALIZAR", null));
            match(Tipo.ACTUALIZAR);
            nodo.Childs.Add(new Nodo(getIndex(), lst_token.ElementAt(indice).Lexema, null));
            string id = lst_token.ElementAt(indice).Lexema;
            Console.WriteLine(id);
            match(Tipo.ID);
            nodo.Childs.Add(new Nodo(getIndex(), "ESTABLECER", null));
            match(Tipo.ESTABLECER);
            nodo.Childs.Add(new Nodo(getIndex(), "(", null));
            match(Tipo.SIMBOLO_PARENTESISIZQ);
            asignaciones();
            Nodo asig = stack.Pop();
            nodo.Childs.Add(asig);
            stack.Push(nodo);
            nodo.Childs.Add(new Nodo(getIndex(), ")", null));
            match(Tipo.SIMBOLO_PARENTESISDER);
            where();
            Nodo co = stack.Pop();
            nodo.Childs.Add(co);
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
                nodo.Childs.Add(new Nodo(getIndex(), ",", null));
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
            string id = lst_token.ElementAt(indice).Lexema;
            Console.WriteLine(id);
            match(Tipo.ID);
            nodo.Childs.Add(new Nodo(getIndex(), "=", null));
            match(Tipo.SIMBOLO_IGUAL);
            valorP();
            Nodo val = stack.Pop();
            nodo.Childs.Add(val);
            stack.Push(nodo);
        }
        public void imprimir()
        {
            foreach (Tabla item in lst_tablas)
            {
                Console.WriteLine(item.Nombre_tabla);
                foreach (var item2 in item.Columnas)
                {
                    Console.Write(item2.Nombre + "  ");
                }
                Console.WriteLine();
                foreach (var item2 in item.Filas)
                {
                    for (int i = item2.Count - 1; i >= 0; i--)
                    {
                        Console.Write(item2.ElementAt(i).Value + "  ");
                    }
                    Console.WriteLine();
                }

            }

        }

        public void graficar(Nodo nodo)
        {
            String rdot = "_arbol.dot";
            rpng = "arbol.png";
            print = new StringBuilder("digraph { ");
            imprimirNodos(nodo, print);
            grafic(nodo, print);
            print.Append("}");
            this.generatedot(rdot, rpng);
        }

        private void imprimirNodos(Nodo nodo, StringBuilder print)
        {
            print.Append(nodo.Index + "[style = \"filled\" ; label = \"" + nodo.Nombre + "\"] \n");
            if (nodo.Childs != null)
            {
                foreach (Nodo child in nodo.Childs)
                {
                    imprimirNodos(child, print);
                }
            }
        }

        private void grafic(Nodo nodo, StringBuilder print)
        {
            if (nodo.Childs != null)
            {
                foreach (Nodo child in nodo.Childs)
                {
                    print.Append(nodo.Index + "->" + child.Index + "; \n");
                    grafic(child, print);
                }
            }
        }
        private void generatedot(String rdot, String rpng)
        {
            File.WriteAllText(rdot, print.ToString());
            string comanDot = "dot -Tpng " + rdot + " -o " + rpng + "";
            var comand = string.Format(comanDot);
            System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + comand);
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = false;
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            proc.WaitForExit();
            string result = proc.StandardOutput.ReadToEnd();
            Console.WriteLine(result);
        }
    }
}
