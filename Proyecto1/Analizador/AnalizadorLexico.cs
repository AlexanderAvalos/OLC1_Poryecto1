using Proyecto1.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1.Analizador
{
    class AnalizadorLexico
    {
        private string lexema = "";
        private bool cierre = false;
        private int filaL = 0;
        private int estado = 0;
        private bool continua = false;
        private int columna;
        public List<Token> lst_token = new List<Token>();
        public List<Errores> lst_errores = new List<Errores>();

        public void analizadorlexico(string linea, int fila)
        {
            filaL = fila;
            char[] caracter;
            char codCaracter;
            int carac = 0;
            for (columna = 0; columna < linea.Length; columna++)
            {
                caracter = linea.ToCharArray();
                codCaracter = caracter[columna];
                carac = (int)caracter[columna];
                if (estado == 0 && continua == false)
                    estado = iniciales(codCaracter);
                switch (estado)
                {
                    case 1:
                        if (char.IsLetterOrDigit(codCaracter))
                        {
                            lexema = lexema + codCaracter;
                            estado = 1;
                        }
                        else
                        {
                            if (reservada(lexema) == true)
                            {
                                lst_token.Add(new Token(Tipo.RESERVADA, lexema.ToUpper(), filaL, columna));
                                lexema = "";
                            }
                            else
                            {
                                lst_token.Add(new Token(Tipo.ID, lexema, filaL, columna));
                                lexema = "";
                            }
                            columna--;
                            estado = 0;
                        }
                        break;
                    case 2:
                        if (char.IsDigit(codCaracter))
                        {
                            lexema = lexema + codCaracter;
                            estado = 2;
                        }
                        else
                        {
                            if (codCaracter != '.')
                            {
                                lst_token.Add(new Token(Tipo.ENTERO, lexema, filaL, columna));
                                lexema = "";
                                estado = iniciales(codCaracter);
                            }
                            else
                            {
                                lexema = lexema + codCaracter;
                                estado = 8;
                            }
                        }
                        break;
                    case 3:
                        if (codCaracter == '\n' && cierre == true)
                        {
                            lexema = lexema + codCaracter;
                            lst_token.Add(new Token(Tipo.COMENTARIO, lexema, filaL, columna));
                            lexema = "";
                            estado = iniciales(codCaracter);
                            continua = false;
                            cierre = false;
                        }
                        else
                        {
                            lexema = lexema + codCaracter;
                            estado = 3;
                            cierre = true;
                            continua = true;
                        }
                        break;
                    case 4:
                        if (codCaracter == '/' && cierre == true)
                        {
                            lexema = lexema + codCaracter;
                            lst_token.Add(new Token(Tipo.COMENTARIO, lexema, filaL, columna));
                            lexema = "";
                            estado = iniciales(codCaracter);
                            continua = false;
                            cierre = false;
                        }
                        else
                        {
                            lexema = lexema + codCaracter;
                            estado = 4;
                            cierre = true;
                            continua = true;
                        }
                        break;
                    case 5:
                        if (codCaracter == '=')
                        {
                            lexema = lexema + codCaracter;
                            lst_token.Add(new Token(Tipo.SIMBOLO_MAYORIGUAL, lexema, filaL, columna));
                            lexema = "";
                            estado = 0;
                        }
                        else
                        {
                            lst_token.Add(new Token(Tipo.SIMBOLO_MAYOR, lexema, filaL, columna));
                            estado = 0;
                        }
                        break;
                    case 6:
                        if (codCaracter == '\"' && cierre == true)
                        {
                            lexema = lexema + codCaracter;
                            estado = 10;
                            continua = false;
                            cierre = false;
                        }
                        else
                        {
                            lexema = lexema + codCaracter;
                            estado = 6;
                            cierre = true;
                            continua = true;
                        }
                        break;
                    case 7:
                        if (char.IsDigit(codCaracter))
                        {
                            lexema = lexema + codCaracter;
                            estado = 11;
                        }
                        break;
                    case 8:
                        lexema = lexema + codCaracter;
                        estado = 12;
                        break;
                    case 10:
                        lst_token.Add(new Token(Tipo.CADENA, lexema, filaL, columna));
                        lexema = "";
                        estado = 0;
                        break;
                    case 11:
                        if (char.IsDigit(codCaracter))
                        {
                            lexema = lexema + codCaracter;
                            estado = 13;
                        }
                        break;
                    case 12:
                        if (char.IsDigit(codCaracter))
                        {
                            lexema = lexema + codCaracter;
                            estado = 12;
                        }
                        else
                        {
                            lst_token.Add(new Token(Tipo.FLOTANTE, lexema, filaL, columna));
                            lexema = "";
                            estado = iniciales(codCaracter);

                        }
                        break;
                    case 13:
                        if (codCaracter == '/')
                        {
                            lexema = lexema + codCaracter;
                            estado = 14;
                        }
                        break;
                    case 14:
                        if (char.IsDigit(codCaracter))
                        {
                            lexema = lexema + codCaracter;
                            estado = 15;
                        }
                        break;
                    case 15:
                        if (char.IsDigit(codCaracter))
                        {
                            lexema = lexema + codCaracter;
                            estado = 16;
                        }
                        break;
                    case 16:
                        if (codCaracter == '/')
                        {
                            lexema = lexema + codCaracter;
                            estado = 17;
                        }
                        break;
                    case 17:
                        if (char.IsDigit(codCaracter))
                        {
                            lexema = lexema + codCaracter;
                            estado = 18;
                        }
                        break;
                    case 18:
                        if (char.IsDigit(codCaracter))
                        {
                            lexema = lexema + codCaracter;
                            estado = 19;
                        }
                        break;
                    case 19:
                        if (char.IsDigit(codCaracter))
                        {
                            lexema = lexema + codCaracter;
                            estado = 20;
                        }
                        break;
                    case 20:
                        if (char.IsDigit(codCaracter))
                        {
                            lexema = lexema + codCaracter;
                            estado = 21;
                        }
                        break;
                    case 21:
                        if (codCaracter == '/')
                        {
                            lexema = lexema + codCaracter;
                            estado = 22;
                        }
                        break;
                    case 22:
                        lst_token.Add(new Token(Tipo.FECHA, lexema, filaL, columna));
                        lexema = "";
                        estado = 0;
                        break;
                    case 23:
                        if (codCaracter == '=')
                        {
                            lexema = lexema + codCaracter;
                            lst_token.Add(new Token(Tipo.SIMBOLO_MENORIGUAL, lexema, filaL, columna));
                            lexema = "";
                            estado = 0;
                        }
                        else
                        {
                            lst_token.Add(new Token(Tipo.SIMBOLO_MENOR, lexema, filaL, columna));
                            estado = 0;
                        }
                        break;
                    case 24:
                        if (codCaracter == '=')
                        {
                            lexema = lexema + codCaracter;
                            lst_token.Add(new Token(Tipo.SIMBOLO_DIFERENTE, lexema, filaL, columna));
                            lexema = "";
                            estado = 0;
                        }
                        break;
                }
            }
        }

        private int iniciales(char codCaracter)
        {
            int caracter = (int)codCaracter;
            if (char.IsLetter(codCaracter))
            {
                return 1;
            }
            else if (char.IsDigit(codCaracter))
            {
                return 2;
            }
            else if (codCaracter == '-')
            {
                lexema = lexema + codCaracter;
                if (lexema.Equals("--"))
                {
                    return 3;
                }
                else
                {
                    return 0;
                }
            }
            else if (codCaracter == '/')
            {
                return 4;
            }
            else if (codCaracter == '\"')
            {
                return 6;
            }
            else if (codCaracter == '\'')
            {
                lexema = lexema + codCaracter;
                return 7;
            }
            else if (codCaracter == '.')
            {
                lst_token.Add(new Token(Tipo.SIMBOLO_PUNTO, lexema, filaL, columna));
                return 0;
            }
            else if (codCaracter == ',')
            {
                lst_token.Add(new Token(Tipo.SIMBOLO_COMA, lexema, filaL, columna));
                return 0;
            }
            else if (codCaracter == '*')
            {
                lst_token.Add(new Token(Tipo.SIMBOLO_ASTERISCO, lexema, filaL, columna));
                return 0;
            }
            else if (codCaracter == '(')
            {
                lst_token.Add(new Token(Tipo.SIMBOLO_PARENTESISIZQ, lexema, filaL, columna));
                return 0;
            }
            else if (codCaracter == ')')
            {
                lst_token.Add(new Token(Tipo.SIMBOLO_PARENTESISDER, lexema, filaL, columna));
                return 0;
            }
            else if (codCaracter == '=')
            {
                lst_token.Add(new Token(Tipo.SIMBOLO_IGUAL, lexema, filaL, columna));
                return 0;
            }
            else if (codCaracter == '<')
            {
                lexema = lexema + codCaracter;
                return 0;
            }
            else if (codCaracter == '>')
            {
                lexema = lexema + codCaracter;
                return 5;
            }
            else if (codCaracter == '!')
            {
                lexema = lexema + codCaracter;
                return 0;
            }
            else if (caracter == 9 || caracter == 13 || caracter == 32 || caracter == 10)
            {
                return 0;
            }
            else
            {
                lst_errores.Add(new Errores("Lexico", "Caracter no pertenece al lenguaje", filaL,columna));
                return 0;
            }
        }

        private bool reservada(String lexema)
        {
            if (lexema.Equals("crear"))
            {
                return true;
            }
            else if (lexema.Equals("tabla"))
            {
                return true;
            }
            else if (lexema.Equals("insertar"))
            {
                return true;
            }
            else if (lexema.Equals("en"))
            {
                return true;
            }
            else if (lexema.Equals("valores"))
            {
                return true;
            }
            else if (lexema.Equals("seleccionar"))
            {
                return true;
            }
            else if (lexema.Equals("de"))
            {
                return true;
            }
            else if (lexema.Equals("donde"))
            {
                return true;
            }
            else if (lexema.Equals("y"))
            {
                return true;
            }
            else if (lexema.Equals("o"))
            {
                return true;
            }
            else if (lexema.Equals("eliminar"))
            {
                return true;
            }
            else if (lexema.Equals("actualizar"))
            {
                return true;
            }
            else if (lexema.Equals("establecer"))
            {
                return true;
            }
            else
                return false;
        }

    }
}
