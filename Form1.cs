using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_final_hans
{
    // ==========================================
    // MODELOS DE DATOS
    // ==========================================

    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public bool Rol { get; set; } // true = Admin, false = Cliente
    }

    public class Vehiculo
    {
        [Key]
        public string Placa { get; set; }
        [Required]
        public string Marca { get; set; }
        [Required]
        public string Modelo { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioPorDia { get; set; }
        public string RutaImagen { get; set; }
        public bool Disponible { get; set; }
    }

    public class Renta
    {
        [Key]
        public int IdRenta { get; set; }
        public string PlacaVehiculo { get; set; }
        public string Username { get; set; }
        public DateTime Fecha { get; set; }
        public int Dias { get; set; }
        public decimal Total { get; set; }
    }

    // ==========================================
    // CONTEXTO DE BASE DE DATOS (EF CORE)
    // ==========================================

    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Renta> Rentas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=.;Database=proyectoFinal;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }
    }

    // ==========================================
    // FORMULARIO DE LOGIN (FORM1)
    // ==========================================

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string userStr = txtUser.Text.Trim();
            string passStr = txtPass.Text.Trim();

            // Validación de campos vacíos
            if (string.IsNullOrEmpty(userStr) || string.IsNullOrEmpty(passStr))
            {
                MessageBox.Show("Por favor, complete todos los campos de acceso.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                try
                {
                    var user = db.Usuarios.FirstOrDefault(u => u.Username == userStr && u.Password == passStr);

                    if (user != null)
                    {
                        Form2 mainApp = new Form2(user);
                        this.Hide();
                        mainApp.Show();
                    }
                    else
                    {
                        MessageBox.Show("Credenciales inválidas. Verifique su usuario y contraseña.", "Error de Acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error crítico de conexión: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }
    }
}
