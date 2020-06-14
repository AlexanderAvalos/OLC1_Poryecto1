using Proyecto1.Analizador;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        public Form1()
        {
            InitializeComponent();
        }

        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
                analizador.analizadorlexico(cadena[posfila].ToLower() + '\n', posfila);
            }
        }
        private void guardar()
        {
            try
            {
                SaveFileDialog Dialog = new SaveFileDialog();
                Dialog.Filter = "Archivos con extension SQLE|*.sqle";
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

        private void ejecutarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            analizador_lexico();
        }
    }
}
