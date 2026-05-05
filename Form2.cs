using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GUI_proyecto
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();

        }

        private void salirDeLaAplicacionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void hansAlvaradoToolStripMenuItem_Click(object sen der, EventArgs e)
        {
            MessageBox.Show("Carne No.2500476", "Informacion del estudiante", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void marioMonroyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Carne No. 23000140", "Informacion del estudiante", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
