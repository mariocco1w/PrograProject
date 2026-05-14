using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Layout.Borders;

namespace Proyecto_final_hans
{
    public partial class Form2 : Form
    {
        private Usuario _usuarioActual;
        private string _facturasPath;

        public Form2(Usuario user)
        {
            InitializeComponent();
            _usuarioActual = user;
            _facturasPath = Path.Combine(Application.StartupPath, "Facturas");
            Directory.CreateDirectory(_facturasPath);
            ConfigurarSeguridad();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            CargarDatos();
            ActualizarDashboard();
        }

        private void ConfigurarSeguridad()
        {
            if (!_usuarioActual.Rol)
            {
                this.Text = $"Portal Cliente - {_usuarioActual.Username}";
                // Ocultar elementos de administración
                if (panelAdmin != null) panelAdmin.Visible = false;
                if (tabAdmin != null) tabControlPrincipal.TabPages.Remove(tabAdmin);
            }
            else
            {
                this.Text = "Panel Administrativo - MODO ROOT";
            }
        }

        // ==========================================
        // LÓGICA DE DATOS Y FILTROS
        // ==========================================

        private void CargarDatos()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    string busqueda = txtBuscar.Text.Trim().ToLower();
                    var query = db.Vehiculos.AsQueryable();

                    // Filtros de búsqueda en tiempo real
                    if (!string.IsNullOrEmpty(busqueda))
                    {
                        query = query.Where(v => v.Marca.ToLower().Contains(busqueda) || 
                                               v.Modelo.ToLower().Contains(busqueda) || 
                                               v.Placa.ToLower().Contains(busqueda));
                    }

                    // Filtros por estado
                    if (rbDisponibles.Checked) query = query.Where(v => v.Disponible);
                    else if (rbRentados.Checked) query = query.Where(v => !v.Disponible);

                    dgvVehiculos.DataSource = query.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e) => CargarDatos();
        private void rbFiltro_CheckedChanged(object sender, EventArgs e) => CargarDatos();

        // ==========================================
        // LÓGICA DE RENTA Y PDF
        // ==========================================

        private void btnRentar_Click(object sender, EventArgs e)
        {
            if (dgvVehiculos.CurrentRow == null) return;

            string placa = dgvVehiculos.CurrentRow.Cells["Placa"].Value.ToString();
            
            if (!int.TryParse(txtDiasRenta.Text, out int dias) || dias <= 0)
            {
                MessageBox.Show("Ingrese una cantidad de días válida (mayor a 0).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                var v = db.Vehiculos.Find(placa);
                if (v == null || !v.Disponible)
                {
                    MessageBox.Show("El vehículo ya no está disponible.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal total = v.PrecioPorDia * dias;
                
                if (MessageBox.Show($"Total a pagar: Q{total:F2}\n¿Confirmar renta?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    v.Disponible = false;
                    
                    var renta = new Renta {
                        PlacaVehiculo = v.Placa,
                        Username = _usuarioActual.Username,
                        Fecha = DateTime.Now,
                        Dias = dias,
                        Total = total
                    };

                    db.Rentas.Add(renta);
                    db.SaveChanges();

                    GenerarFacturaPDF(v, renta);
                    
                    MessageBox.Show("Renta exitosa. El PDF ha sido generado en la carpeta 'Facturas'.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatos();
                    ActualizarDashboard();
                }
            }
        }

        private void GenerarFacturaPDF(Vehiculo v, Renta r)
        {
            string filePath = Path.Combine(_facturasPath, $"Factura_{v.Placa}_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
            
            using (PdfWriter writer = new PdfWriter(filePath))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document doc = new Document(pdf);
                    
                    // Diseño Profesional
                    doc.Add(new Paragraph("FACTURA DE RENTA DE VEHÍCULO")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(20)
                        .SetBold()
                        .SetFontColor(ColorConstants.BLUE));

                    doc.Add(new Paragraph($"Fecha de Emisión: {r.Fecha:dd/MM/yyyy HH:mm:ss}"));
                    doc.Add(new Paragraph($"Cliente: {r.Username}").SetBold());
                    doc.Add(new Paragraph("\n"));

                    Table table = new Table(UnitValue.CreatePercentArray(new float[] { 40, 60 })).UseAllAvailableWidth();
                    table.AddHeaderCell("Concepto");
                    table.AddHeaderCell("Detalle");

                    table.AddCell("Vehículo"); table.AddCell($"{v.Marca} {v.Modelo}");
                    table.AddCell("Placa"); table.AddCell(v.Placa);
                    table.AddCell("Días de Renta"); table.AddCell(r.Dias.ToString());
                    table.AddCell("Precio por Día"); table.AddCell($"Q{v.PrecioPorDia:F2}");
                    
                    Cell totalCell = new Cell(1, 2).Add(new Paragraph($"TOTAL PAGADO: Q{r.Total:F2}")
                        .SetBold()
                        .SetFontSize(14)
                        .SetTextAlignment(TextAlignment.RIGHT))
                        .SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    table.AddFooterCell(totalCell);

                    doc.Add(table);
                    doc.Add(new Paragraph("\nGracias por confiar en nosotros.")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetItalic());
                }
            }
        }

        // ==========================================
        // DEVOLUCIÓN DE VEHÍCULO
        // ==========================================

        private void btnDevolver_Click(object sender, EventArgs e)
        {
            string placaInput = txtPlacaDevolver.Text.Trim().ToUpper();

            if (string.IsNullOrEmpty(placaInput))
            {
                MessageBox.Show("Ingrese la placa del vehículo a devolver.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                var v = db.Vehiculos.FirstOrDefault(veh => veh.Placa == placaInput);

                if (v == null)
                {
                    MessageBox.Show("El vehículo con esa placa no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (v.Disponible)
                {
                    MessageBox.Show("El vehículo ya se encuentra en inventario (Disponible).", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (MessageBox.Show($"¿Confirmar devolución de {v.Marca} {v.Modelo}?", "Devolución", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    v.Disponible = true;
                    db.SaveChanges();

                    MessageBox.Show("Vehículo devuelto exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtPlacaDevolver.Clear();
                    CargarDatos();
                    ActualizarDashboard();
                }
            }
        }

        // ==========================================
        // DASHBOARD ESTADÍSTICO
        // ==========================================

        private void ActualizarDashboard()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    lblTotalVehiculos.Text = db.Vehiculos.Count().ToString();
                    lblDisponibles.Text = db.Vehiculos.Count(v => v.Disponible).ToString();
                    lblRentados.Text = db.Vehiculos.Count(v => !v.Disponible).ToString();
                    lblTotalUsuarios.Text = db.Usuarios.Count().ToString();
                    
                    decimal totalGenerado = db.Rentas.Sum(r => (decimal?)r.Total) ?? 0;
                    lblTotalVentas.Text = $"Q{totalGenerado:N2}";
                }
            }
            catch { /* Silencioso para evitar interrupciones visuales */ }
        }

        // ==========================================
        // MANTENIMIENTO DE VEHÍCULOS (ADMIN)
        // ==========================================

        private void btnGuardarVehiculo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPlaca.Text) || string.IsNullOrWhiteSpace(txtMarca.Text))
            {
                MessageBox.Show("Complete todos los campos del vehículo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("El precio debe ser un número mayor a 0.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                string placa = txtPlaca.Text.Trim().ToUpper();
                if (db.Vehiculos.Any(v => v.Placa == placa))
                {
                    MessageBox.Show("Error: La placa ya está registrada.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                db.Vehiculos.Add(new Vehiculo {
                    Placa = placa,
                    Marca = txtMarca.Text.Trim(),
                    Modelo = txtModelo.Text.Trim(),
                    PrecioPorDia = precio,
                    Disponible = true,
                    RutaImagen = ""
                });

                db.SaveChanges();
                MessageBox.Show("Vehículo agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarDatos();
                ActualizarDashboard();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvVehiculos.CurrentRow == null) return;

            if (MessageBox.Show("¿Está seguro de eliminar este registro permanentemente?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string placa = dgvVehiculos.CurrentRow.Cells["Placa"].Value.ToString();
                using (var db = new AppDbContext())
                {
                    var v = db.Vehiculos.Find(placa);
                    if (v != null)
                    {
                        db.Vehiculos.Remove(v);
                        db.SaveChanges();
                        CargarDatos();
                        ActualizarDashboard();
                    }
                }
            }
        }
        // ==========================================
        // IMPORTACIÓN Y EXPORTACIÓN CSV (ADMIN)
        // ==========================================

        private void btnImportarCSV_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog { Filter = "Archivos CSV (*.csv)|*.csv" };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var db = new AppDbContext())
                    {
                        string[] lineas = File.ReadAllLines(openFile.FileName);
                        int importados = 0;

                        foreach (string linea in lineas.Skip(1)) // Saltar encabezado
                        {
                            if (string.IsNullOrWhiteSpace(linea)) continue;
                            var datos = linea.Split(',');

                            if (datos.Length < 4) continue;

                            string placa = datos[0].Trim().ToUpper();
                            
                            // Validación: No permitir placas repetidas
                            if (!db.Vehiculos.Any(v => v.Placa == placa))
                            {
                                if (decimal.TryParse(datos[3].Trim(), out decimal precio))
                                {
                                    db.Vehiculos.Add(new Vehiculo
                                    {
                                        Placa = placa,
                                        Marca = datos[1].Trim(),
                                        Modelo = datos[2].Trim(),
                                        PrecioPorDia = precio,
                                        Disponible = true,
                                        RutaImagen = ""
                                    });
                                    importados++;
                                }
                            }
                        }
                        db.SaveChanges();
                        MessageBox.Show($"{importados} vehículos importados exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarDatos();
                        ActualizarDashboard();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al importar CSV: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnExportarCSV_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    // Reporte de vehículos actualmente RENTADOS (Disponible = false)
                    var rentados = db.Vehiculos.Where(v => !v.Disponible).ToList();

                    if (rentados.Count == 0)
                    {
                        MessageBox.Show("No hay vehículos rentados para exportar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    string reportePath = Path.Combine(Application.StartupPath, "Reporte_Vehiculos_Rentados.csv");
                    using (StreamWriter sw = new StreamWriter(reportePath, false, Encoding.UTF8))
                    {
                        sw.WriteLine("Placa,Marca,Modelo,PrecioPorDia");
                        foreach (var v in rentados)
                        {
                            sw.WriteLine($"{v.Placa},{v.Marca},{v.Modelo},{v.PrecioPorDia}");
                        }
                    }
                    MessageBox.Show($"Reporte CSV generado en:\n{reportePath}", "Exportación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar CSV: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
