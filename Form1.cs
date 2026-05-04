using System;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GUI_proyecto
{
    // --- CLASES DE DATOS ---
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; } // 'Admin' o 'Cliente'
    }

    public class Vehiculo
    {
        [Key]
        public string Placa { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public decimal PrecioPorDia { get; set; }
        public string RutaImagen { get; set; }
        public bool Disponible { get; set; }
    }

    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Esta es la concexion a la base de datos creada
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=RentaVehiculos;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    // --- FORMULARIO DE LOGIN ---
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void validar_Click(object sender, EventArgs e)
        {
            string nombre = nameBox.Text;
            string pass = passBox.Text;

            // 1. Validación de Regex (Utilizaremos el mismo formato que ya creaste? o lo cambiamos a admin123 y cliente123 para que se mire un poco mas profesional?)
            // string contraReg = @"\d{4}"; // Ejemplo: solo 4 números

            using (var db = new AppDbContext())
            {
                try
                {
                    // 2. Consulta ORM: Buscar usuario en la base de datos
                    var user = db.Usuarios.FirstOrDefault(u => u.Username == nombre && u.Password == pass);

                    if (user != null)
                    {
                        MessageBox.Show($"Bienvenido {user.Username} ({user.Rol})");

                        // 3. Pasar el usuario al Form2 (Dashboard)
                        // Es vital pasar el objeto 'user' para saber qué botones ocultar/mostrar
                        Form2 dashboard = new Form2(user);
                        this.Hide();
                        dashboard.Show();
                    }
                    else
                    {
                        MessageBox.Show("Usuario o contraseña incorrectos.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error de conexión: " + ex.Message);
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            passBox.UseSystemPasswordChar = !checkBox1.Checked;
        }
    }
}
