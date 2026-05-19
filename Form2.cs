using Azure.Core;
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
        decimal precioRenta;
        bool disponibilidad;
        string rutaArchivo;
        public Form2(Form1.Usuario user)
        {
            InitializeComponent();
            usuarioActual = user;
            ConfigurarInterfaz();
            string resourcesFolder = Path.Combine(Application.StartupPath, "rutasImagenes");
            Directory.CreateDirectory(resourcesFolder);

        }

        //metodo que quita las tab pages si es user 
        private void ConfigurarInterfaz()
        {
            // Si el rol es false (0), ocultamos botones de edición porque es un usuario normal
            if (!usuarioActual.Rol)
            {
                string rolUser = "Cliente";
                this.Text = $"Portal de Rentas - Bienvenido {usuarioActual.Username}, {rolUser}";
                herramientasToolStripMenuItem.Visible = false;
                tabControl1.TabPages.Remove(tabPage2);
                tabControl1.TabPages.Remove(tabPage3);

            }
            else
            {
                string rolUser = "Administrador";
                this.Text = $"Panel de Administración - Bienvenido {usuarioActual.Username}, {rolUser}";

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

        /// <summary>
        /// metodo para actualizar la disponibilidad del carro despues de confirmar renta, lo que hace es buscar el carro por placa y cambiar su estado a no disponible
        /// </summary>
        private void actualizarDisp()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var vehiculo = db.Vehiculos.FirstOrDefault(v => v.Placa == dataGridView1.CurrentRow.Cells["Placa"].Value.ToString());
                    if (vehiculo != null)
                    {
                        vehiculo.Disponible = false; // Marcar como no disponible
                        db.SaveChanges();
                        Form2_Load(null, null); // Recargar datos para reflejar el cambio
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar disponibilidad: " + ex.Message);
            }
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

                    // 2. Cargar el ComboBox
                    selectCar.DataSource = null;
                    selectCar.DisplayMember = "placa";
                    selectCar.ValueMember = "Id";
                    selectCar.DataSource = listaCarros;

                    //  empezar sin nada seleccionado
                    selectCar.SelectedIndex = -1;



                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message);
            }


        }
        //metodo para buscar por placa, lo que hace es filtrar la lista de carros por los que contengan el texto ingresado en el textbox, y luego actualizar el datagridview con los resultados
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

        //metodo para mostrar la imagen del carro seleccionado, lo que hace es obtener la ruta de la imagen guardada en la base de datos y cargarla en el picturebox
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    // Obtener la fila seleccionada
                    DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];
                    precioRenta = Convert.ToDecimal(fila.Cells["precioPorDia"].Value);
                    disponibilidad = Convert.ToBoolean(fila.Cells["disponible"].Value);



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
            //--------------------------------------------------------------------



        }



        //NO TOCAR ESTO, ES SOLO PARA PRUEBAS DE INTERFAZ, SI LO TOCAS SE ROMPE TODO
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        //NO TOCAR
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        //metodo para confirmar la renta, lo que hace es calcular el total a pagar segun la opcion seleccionada en el combo box y mostrarlo en un label, ademas de actualizar la disponibilidad del carro
        private void confirmarRenta_Click(object sender, EventArgs e)
        {


            if (precioRenta > 0)
            {
                if (!disponibilidad)
                {
                    MessageBox.Show("Este vehículo no está disponible para renta.");
                    return;
                }


                if (comboBox1.Text.Trim() == "1 dia")
                {
                    decimal totalRenta = precioRenta * 1;
                    label3.Text = $"Total a pagar: ${totalRenta:N2}";
                    actualizarDisp();

                }
                else if (comboBox1.Text.Trim() == "3 dias")
                {
                    decimal totalRenta = precioRenta * 3;
                    label3.Text = $"Total a pagar: ${totalRenta:N2}";
                    actualizarDisp();
                }
                else if (comboBox1.Text.Trim() == "1 semana")
                {
                    decimal totalRenta = precioRenta * 7;
                    label3.Text = $"Total a pagar: ${totalRenta:N2}";
                    actualizarDisp();
                }
                else
                {
                    MessageBox.Show("Por favor, selecciona una opción de la lista.");
                }
            }
            else
            {
                MessageBox.Show("Primero selecciona un vehículo haciendo clic en la tabla.");
            }

        }

        //metodo para mostrar el precio del carro seleccionado en el combo box, lo que hace es obtener el precio del carro seleccionado y mostrarlo en un textbox para que el admin pueda editarlo si lo desea
        private void selectCar_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (var db = new AppDbContext())
            {

                if (selectCar.SelectedItem != null)
                {
                    precioBox.Text = ((Vehiculo)selectCar.SelectedItem).PrecioPorDia.ToString("N2");

                }






            }


        }

        // metodo para forzar la disponibilidad del carro seleccionado, lo que hace es buscar el carro por placa y cambiar su estado a disponible, esto es util para cuando un admin quiere corregir un error o un cliente devuelve el carro sin avisar
        private void forzarDisponibilidad_Click(object sender, EventArgs e)
        {

            if (selectCar.SelectedItem != null && selectCar.Text != "nuevo carro")
            {
                try
                {

                    string placaSeleccionada = selectCar.Text;

                    using (var db = new AppDbContext())
                    {


                        var vehiculo = db.Vehiculos.FirstOrDefault(v => v.Placa == placaSeleccionada);

                        if (vehiculo != null)
                        {

                            if (vehiculo.Disponible)
                            {
                                MessageBox.Show($"El vehículo {placaSeleccionada} ya se encuentra disponible.");
                            }
                            else
                            {

                                vehiculo.Disponible = true;
                                db.SaveChanges();

                                MessageBox.Show($"El vehículo {placaSeleccionada} ahora está disponible.");


                                Form2_Load(null, null);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el vehículo en la base de datos.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Seleccione una placa válida.");
            }
        }

        //metodo para editar el precio del carro seleccionado, lo que hace es buscar el carro por placa y cambiar su precio por el valor ingresado en el textbox, ademas de mostrar un mensaje de confirmacion antes de guardar los cambios
        private void editPrecio_Click(object sender, EventArgs e)
        {
            if (selectCar.SelectedItem != null && selectCar.Text != "nuevo carro")
            {
                string placaSeleccionada = selectCar.Text;
                using (var db = new AppDbContext())
                {

                    var vehiculo = db.Vehiculos.FirstOrDefault(v => v.Placa == placaSeleccionada); // Buscamos el registro original

                    if (vehiculo != null)
                    {
                        try
                        {
                            vehiculo.PrecioPorDia = decimal.Parse(precioBox.Text);
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("Ingrese un precio válido.");
                            return;
                        }

                        if (MessageBox.Show("Confirmar cambios?", "AVISO", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                        {
                            MessageBox.Show($"Precio editado con exito", vehiculo.Placa, MessageBoxButtons.OK);
                            db.SaveChanges();
                            Form2_Load(null, null);
                        }
                        else
                        {
                            return;
                        }

                    }
                }

            }
            else
            {
                MessageBox.Show("Seleccione una placa válida para editar su precio.");
            }





        }

        //metodo para agregar un nuevo carro, lo que hace es crear un nuevo objeto de tipo vehiculo con los datos ingresados en los textbox y guardarlo en la base de datos, ademas de mostrar un mensaje de confirmacion antes de guardar los cambios
        private void nuevoCarro_Click(object sender, EventArgs e)
        {
            using (var db = new AppDbContext())
            {
                decimal precioNuevo;
                //verificar si precioBox puede hacer parse
                try
                {
                    precioNuevo = decimal.Parse(precioBox.Text);
                }
                catch (FormatException) { MessageBox.Show("Ingrese un precio válido."); return; }

                //veficar si hay imagen seleccionada 
                try
                {
                    if (string.IsNullOrEmpty(rutaArchivo))
                    {
                        MessageBox.Show("Seleccione una imagen para el vehículo.");
                        return;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar imagen: " + ex.Message);
                    return;
                }



                // Creamos el objeto con los datos de los TextBox
                Vehiculo nuevoVehiculo = new Vehiculo
                {
                    Placa = placaBox.Text,
                    Modelo = modeloBox.Text,
                    Marca = maraBox.Text,
                    PrecioPorDia = precioNuevo,
                    RutaImagen = rutaArchivo,
                    Disponible = true
                };
                MessageBox.Show(rutaArchivo);

                db.Vehiculos.Add(nuevoVehiculo); // Equivale al INSERT INTO
                db.SaveChanges(); // IMPORTANTE: Esto ejecuta la operación en SQL
                Form2_Load(null, null);



            }


        }


        //metodo para cargar la imagen del carro, lo que hace es abrir un cuadro de dialogo para seleccionar una imagen y luego copiarla a la carpeta de recursos del proyecto, ademas de guardar la ruta relativa en una variable para luego guardarla en la base de datos
        private void cargarImagen_Click(object sender, EventArgs e)
        {

            OpenFileDialog buscador = new OpenFileDialog();


            buscador.Filter = "Imágenes JPG|*.jpg|Imágenes PNG|*.png";
            buscador.Title = "Seleccionar imagen del vehículo";

            if (buscador.ShowDialog() == DialogResult.OK)
            {

                string rutaOrigen = buscador.FileName;
                rutaArchivo = buscador.SafeFileName;
                rutaArchivo = Path.Combine(Application.StartupPath, "rutasImagenes", rutaArchivo);
                File.Copy(rutaOrigen, rutaArchivo, true);

                MessageBox.Show($"Imagen cargada exitosamente '{rutaArchivo}'.");

            }


        }

        //metodo para eliminar un carro, lo que hace es buscar el carro por placa y eliminarlo de la base de datos, ademas de mostrar un mensaje de confirmacion antes de eliminar el registro
        private void eliminarCarro_Click(object sender, EventArgs e)
        {

            using (var db = new AppDbContext())
            {

                if (selectCar.SelectedItem != null)
                {
                    string placaSeleccionada = selectCar.Text;
                    var carroEliminado = db.Vehiculos.FirstOrDefault(v => v.Placa == placaSeleccionada);

                    MessageBox.Show($"Seguro que desea eliminar este carro? [{carroEliminado.Placa}]", "ADVERTENCIA", MessageBoxButtons.OKCancel);
                    db.Vehiculos.Remove(carroEliminado); // Equivale al DELETE FROM... WHERE ID = id
                    db.SaveChanges();

                }


            }



            //--------------------------
        }
    }
}

