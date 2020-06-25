using Proyecto1.Analizador;
using Proyecto1.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto1
{
    public partial class Form1 : Form
    {
        private string[] cadena;
        private AnalizadorLexico analizador = new AnalizadorLexico();
        private AnalizadorSintactico sintactico;
        public Form1()
        {
            InitializeComponent();
            Controlador.Start();
        }

        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Panel.Text = "";
            analizador.lst_token.Clear();
            int val = analizador.lst_token.Count();
            for (int i = 0; i < val - 1; i++)
            {
                analizador.lst_token.RemoveAt(i);
            }
            analizador.lst_tokens.Clear();
            int val2 = analizador.lst_tokens.Count();
            for (int i = 0; i < val2 - 1; i++)
            {
                analizador.lst_tokens.RemoveAt(i);
            }
        }



        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            abrir();
        }

        private void guardarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guardar();
        }
        private void abrir()
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.Filter = "Archivos con extension SQLE|*.sqle|Archivos con extension RAAS|*.raas";
            Dialog.Title = "Seleccione el archivo";
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                string url = Dialog.FileName;
                try
                {
                    TextReader Leer = new StreamReader(url);
                    Panel.Text = Leer.ReadToEnd();
                }
                catch
                {
                    MessageBox.Show("Error De Carga");
                }
            }
        }
        private void analizador_lexico()
        {
            cadena = Panel.Lines;
            for (int posfila = 0; posfila < cadena.Length; posfila++)
            {
                analizador.analizadorlexico(cadena[posfila].ToLower() + '\n', posfila, Panel);
            }
            analizador.vacio = true;
        }
        private void anali()
        {
            Console.WriteLine();
            string linea = Panel.SelectedText;
            string line = linea.ToLower();
            char[] caracter = new char[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                caracter[i] = line.ElementAt(i);
            }
            cadena = Panel.Lines;
            for (int posfila = 0; posfila < caracter.Length; posfila++)
            {
                analizador.analizadorlexico(caracter[posfila].ToString(), posfila, Panel);
            }
        }


        private void analizador_sintactico()
        {
            sintactico = new AnalizadorSintactico(analizador.lst_tokens, analizador.lst_errores);
            sintactico.parser();
        }
        private void guardar()
        {
            try
            {
                SaveFileDialog Dialog = new SaveFileDialog();
                Dialog.Filter = "Archivos con extension SQLE| *.sqle | Archivos con extension RAAS | *.raas";
                Dialog.Title = "Guardar";
                string rutah = Dialog.FileName;
                if (Dialog.ShowDialog() == DialogResult.OK)
                {
                    Panel.SaveFile(Dialog.FileName, RichTextBoxStreamType.PlainText);
                    MessageBox.Show("Archivo Guardado");
                }
            }
            catch
            {
                MessageBox.Show("Error al guardar");
            }
        }
        private void generarToken()
        {


            string ruta = @"Lista Token.htm";
            using (FileStream fs = new FileStream(ruta, FileMode.Create))
            {
                using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                {
                    w.WriteLine("<Center><TABLE border = 2 ></Center>");
                    w.WriteLine("<TR>");
                    w.WriteLine("<Center><TH COLSPAN = 6 > Tabla en HTML</TH></Center>");
                    w.WriteLine("</TR>");
                    w.WriteLine("<TR>");
                    w.WriteLine("<TH> Token </TH>");
                    w.WriteLine("<TH> Tipo </TH>");
                    w.WriteLine("<TH> Lexema </TH>");
                    w.WriteLine("<TH> Linea </TH>");
                    w.WriteLine("<TH> Columna </TH>");
                    w.WriteLine("</TR>");
                    foreach (Token token in analizador.lst_token)
                    {
                        w.WriteLine("<TR>");
                        w.WriteLine("<TH>" + token.Tipo.GetHashCode() + "</TH>");
                        w.WriteLine("<TH>" + token.Tipo.ToString() + "</TH>");
                        w.WriteLine("<TH>" + token.Lexema + "</TH>");
                        w.WriteLine("<TH>" + token.Fila + "</TH>");
                        w.WriteLine("<TH>" + token.Columna + "</TH>");
                        w.WriteLine("</TR>");
                    }
                }
            }
        }

        private void generarError()
        {
            String ruta = "Lista Errores.html";
            using (FileStream fs = new FileStream(ruta, FileMode.Create))
            {
                using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                {
                    w.WriteLine("<Center><TABLE border = 2 ></Center>");
                    w.WriteLine("<TR>");
                    w.WriteLine("<Center><TH COLSPAN = 5 > Tabla en HTML</TH></Center>");
                    w.WriteLine("</TR>");
                    w.WriteLine("<TR>");
                    w.WriteLine("<TH> Tipo </TH>");
                    w.WriteLine("<TH> Descripcion </TH>");
                    w.WriteLine("<TH> Linea </TH>");
                    w.WriteLine("<TH> columna </TH>");
                    w.WriteLine("</TR>");
                    foreach (Errores error in analizador.lst_errores)
                    {
                        w.WriteLine("<TR>");
                        w.WriteLine("<TH>" + error.Tipo + "</TH>");
                        w.WriteLine("<TH>" + error.Descripcion + "</TH>");
                        w.WriteLine("<TH>" + error.Fila + "</TH>");
                        w.WriteLine("<TH>" + error.Columna + "</TH>");
                        w.WriteLine("</TR>");
                    }
                }
            }
        }

        private void ejecutarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            anali();
            //analizador_lexico();
            analizador_sintactico();
        }

        private void mostrarTokenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            generarToken();
        }

        private void mostrarErroresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            generarError();
        }

        private void Controlador_Tick(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int caracter = 0;
            int altura = Panel.GetPositionFromCharIndex(0).Y;
            if (Panel.Lines.Length > 0)
            {
                for (int i = 0; i < Panel.Lines.Length; i++)
                {
                    e.Graphics.DrawString((i + 1).ToString(), Panel.Font, Brushes.Cyan, pictureBox1.Width - (e.Graphics.MeasureString((i + 1).ToString(), Panel.Font).Width + 10), altura);
                    caracter += Panel.Lines[i].Length + 1;
                    altura = Panel.GetPositionFromCharIndex(caracter).Y;
                }
            }
            else
            {
                e.Graphics.DrawString((1).ToString(), Panel.Font, Brushes.Cyan, pictureBox1.Width - (e.Graphics.MeasureString((1).ToString(), Panel.Font).Width + 10), altura);
            }
        }

        private void verTablasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sintactico.vertablar();
        }

        private void mostrarArbolDeDerivacionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sintactico = new AnalizadorSintactico(analizador.lst_tokens, analizador.lst_errores);
            sintactico.parser();
            sintactico.imprirarbol();
        }

        private void cargarTablasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            abrir();
        }

        private void ejecutarTodoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            analizador_lexico();
            analizador_sintactico();


        }

        private void manualDeUsuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("Manual de Usuario.pdf");
        }

        private void manualTecnicoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("Manual técnico.pdf");
        }
    }
}
