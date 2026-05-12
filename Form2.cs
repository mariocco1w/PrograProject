using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static Proyecto_final_hans.Form1;

namespace Proyecto_final_hans
{

    public partial class Form2 : Form
    {
        private Form1.Usuario usuarioActual;
        public Form2(Form1.Usuario user)
        {
            InitializeComponent();
            usuarioActual = user;
            ConfigurarInterfaz();
            string resourcesFolder = Path.Combine(Application.StartupPath, "Resources");
            Directory.CreateDirectory(resourcesFolder);
        }


        private void ConfigurarInterfaz()
        {
            // Si el rol es false (0), ocultamos botones de edición porque es un usuario normal
            if (!usuarioActual.Rol)
            {
                this.Text = $"Portal de Rentas - Bienvenido {usuarioActual.Username}";
                herramientasToolStripMenuItem.Visible = false;
                tabControl1.TabPages.Remove(tabPage2);
                tabControl1.TabPages.Remove(tabPage3);

            }
            else
            {
                this.Text = "Panel de Administración - MODO ROOT";

            }
        }

        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 login = new Form1();
            login.Show();
            this.Close(); // Esto destruye esta instancia y limpia la memoria
        }

        private void salirDeLaAplicacionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                // conexion a la base de datos
                using (var db = new AppDbContext())
                {
                    // lista
                    var listaCarros = db.Vehiculos.ToList();


                    dataGridView1.DataSource = listaCarros;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message);
            }


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            try
            {
                using (var db = new AppDbContext())
                {

                    string busqueda = textBox1.Text.Trim();


                    // Busca todos los registros que coincden
                    var resultados = db.Vehiculos
                        .Where(x => x.Placa.Contains(busqueda))
                        .ToList();


                    dataGridView1.DataSource = resultados;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error en búsqueda: " + ex.Message);
            }



        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    // Obtener la fila seleccionada
                    DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];

                    // Obtener la ruta guardada en la base de datos
                    string imagePath = fila.Cells["rutaImagen"].Value?.ToString();

                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        // Si guardaste ruta relativa, la convertimos a absoluta
                        string fullPath = Path.Combine(Application.StartupPath, imagePath);

                        if (File.Exists(fullPath))
                        {
                            pictureBox1.ImageLocation = fullPath;
                        }
                        else
                        {
                            MessageBox.Show("La imagen no existe en la carpeta.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar imagen: " + ex.Message);
                }
            }
        }
    }
}

