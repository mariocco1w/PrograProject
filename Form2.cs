using Azure.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Text.Json;
using static Proyecto_final_hans.Form1;

namespace Proyecto_final_hans
{

    public partial class Form2 : Form
    {
        private Form1.Usuario usuarioActual;
        decimal precioRenta;
        bool disponibilidad;
        string rutaArchivo;

        // Nuevos controles para CRUD de Usuarios
        private TabPage tabPageUsuarios;
        private DataGridView dgvUsuarios;
        private TextBox txtUserUsername;
        private TextBox txtUserPassword;
        private CheckBox chkUserRol;
        private Button btnUserCreate;
        private Button btnUserUpdate;
        private Button btnUserDelete;

        public Form2(Form1.Usuario user)
        {
            InitializeComponent();
            usuarioActual = user;
            SetupExtraFeatures();
            ConfigurarInterfaz();
            string resourcesFolder = Path.Combine(Application.StartupPath, "rutasImagenes");
            Directory.CreateDirectory(resourcesFolder);

        }

        private void SetupExtraFeatures()
        {
            // Solo agregar para administradores
            if (usuarioActual.Rol)
            {
                // 1. Crear TabPage de Usuarios
                tabPageUsuarios = new TabPage("Gestión de Usuarios");
                
                dgvUsuarios = new DataGridView();
                dgvUsuarios.Location = new Point(10, 10);
                dgvUsuarios.Size = new Size(400, 300);
                dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvUsuarios.CellClick += DgvUsuarios_CellClick;

                Label lblUser = new Label { Text = "Usuario:", Location = new Point(420, 10), AutoSize = true };
                txtUserUsername = new TextBox { Location = new Point(420, 30), Width = 150 };

                Label lblPass = new Label { Text = "Contraseña:", Location = new Point(420, 60), AutoSize = true };
                txtUserPassword = new TextBox { Location = new Point(420, 80), Width = 150, UseSystemPasswordChar = true };

                chkUserRol = new CheckBox { Text = "Es Administrador", Location = new Point(420, 110), AutoSize = true };

                btnUserCreate = new Button { Text = "Crear", Location = new Point(420, 140), Width = 70 };
                btnUserCreate.Click += BtnUserCreate_Click;

                btnUserUpdate = new Button { Text = "Actualizar", Location = new Point(500, 140), Width = 70 };
                btnUserUpdate.Click += BtnUserUpdate_Click;

                btnUserDelete = new Button { Text = "Eliminar", Location = new Point(420, 175), Width = 150 };
                btnUserDelete.Click += BtnUserDelete_Click;

                tabPageUsuarios.Controls.AddRange(new Control[] { 
                    dgvUsuarios, lblUser, txtUserUsername, lblPass, txtUserPassword, 
                    chkUserRol, btnUserCreate, btnUserUpdate, btnUserDelete 
                });

                tabControl1.TabPages.Add(tabPageUsuarios);
                CargarUsuarios();

                // 2. Agregar opciones de Importación/Exportación al MenuStrip
                var menuImportar = new ToolStripMenuItem("Importar Datos JSON");
                menuImportar.Click += MenuImportar_Click;
                
                var menuExportar = new ToolStripMenuItem("Exportar Datos JSON");
                menuExportar.Click += MenuExportar_Click;

                herramientasToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
                herramientasToolStripMenuItem.DropDownItems.Add(menuImportar);
                herramientasToolStripMenuItem.DropDownItems.Add(menuExportar);
            }
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

        // ==========================================
        // LÓGICA DE GESTIÓN DE USUARIOS (CRUD)
        // ==========================================

        private void CargarUsuarios()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    dgvUsuarios.DataSource = db.Usuarios.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message);
            }
        }

        private void DgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvUsuarios.Rows[e.RowIndex];
                txtUserUsername.Text = row.Cells["Username"].Value.ToString();
                txtUserPassword.Text = row.Cells["Password"].Value.ToString();
                chkUserRol.Checked = Convert.ToBoolean(row.Cells["Rol"].Value);
            }
        }

        private void BtnUserCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserUsername.Text) || string.IsNullOrEmpty(txtUserPassword.Text))
            {
                MessageBox.Show("Complete los campos de usuario y contraseña.");
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    if (db.Usuarios.Any(u => u.Username == txtUserUsername.Text))
                    {
                        MessageBox.Show("El nombre de usuario ya existe.");
                        return;
                    }

                    var nuevoUsuario = new Usuario
                    {
                        Username = txtUserUsername.Text,
                        Password = txtUserPassword.Text,
                        Rol = chkUserRol.Checked
                    };

                    db.Usuarios.Add(nuevoUsuario);
                    db.SaveChanges();
                    MessageBox.Show("Usuario creado con éxito.");
                    CargarUsuarios();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear usuario: " + ex.Message);
            }
        }

        private void BtnUserUpdate_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null) return;

            int id = (int)dgvUsuarios.CurrentRow.Cells["IdUsuario"].Value;

            try
            {
                using (var db = new AppDbContext())
                {
                    var usuario = db.Usuarios.Find(id);
                    if (usuario != null)
                    {
                        usuario.Username = txtUserUsername.Text;
                        usuario.Password = txtUserPassword.Text;
                        usuario.Rol = chkUserRol.Checked;

                        db.SaveChanges();
                        MessageBox.Show("Usuario actualizado con éxito.");
                        CargarUsuarios();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar usuario: " + ex.Message);
            }
        }

        private void BtnUserDelete_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null) return;

            int id = (int)dgvUsuarios.CurrentRow.Cells["IdUsuario"].Value;
            string userToDelete = dgvUsuarios.CurrentRow.Cells["Username"].Value.ToString();

            if (userToDelete == usuarioActual.Username)
            {
                MessageBox.Show("No puedes eliminarte a ti mismo.");
                return;
            }

            if (MessageBox.Show($"¿Seguro que desea eliminar al usuario {userToDelete}?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (var db = new AppDbContext())
                    {
                        var usuario = db.Usuarios.Find(id);
                        if (usuario != null)
                        {
                            db.Usuarios.Remove(usuario);
                            db.SaveChanges();
                            CargarUsuarios();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar usuario: " + ex.Message);
                }
            }
        }

        // ==========================================
        // HERRAMIENTAS DE IMPORTACIÓN Y EXPORTACIÓN
        // ==========================================

        private class DataPackage
        {
            public List<Usuario> Usuarios { get; set; }
            public List<Vehiculo> Vehiculos { get; set; }
        }

        private void MenuExportar_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivos JSON|*.json";
            sfd.FileName = "respaldo_datos.json";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var db = new AppDbContext())
                    {
                        var package = new DataPackage
                        {
                            Usuarios = db.Usuarios.ToList(),
                            Vehiculos = db.Vehiculos.ToList()
                        };

                        string json = JsonSerializer.Serialize(package, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(sfd.FileName, json);
                        MessageBox.Show("Datos exportados exitosamente a JSON.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar: " + ex.Message);
                }
            }
        }

        private void MenuImportar_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivos JSON|*.json";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string json = File.ReadAllText(ofd.FileName);
                    var package = JsonSerializer.Deserialize<DataPackage>(json);

                    if (package != null)
                    {
                        using (var db = new AppDbContext())
                        {
                            // Limpiar y cargar usuarios (evitando duplicar IDs si es posible, o simplemente añadir nuevos)
                            if (package.Usuarios != null)
                            {
                                foreach (var u in package.Usuarios)
                                {
                                    if (!db.Usuarios.Any(existente => existente.Username == u.Username))
                                    {
                                        u.IdUsuario = 0; // Dejar que la BD genere nuevo ID
                                        db.Usuarios.Add(u);
                                    }
                                }
                            }

                            // Limpiar y cargar vehículos
                            if (package.Vehiculos != null)
                            {
                                foreach (var v in package.Vehiculos)
                                {
                                    if (!db.Vehiculos.Any(existente => existente.Placa == v.Placa))
                                    {
                                        db.Vehiculos.Add(v);
                                    }
                                }
                            }

                            db.SaveChanges();
                            MessageBox.Show("Datos importados (solo registros nuevos añadidos).");
                            Form2_Load(null, null); // Recargar vehículos
                            CargarUsuarios();      // Recargar usuarios
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al importar: " + ex.Message);
                }
            }
        }
    }
}

