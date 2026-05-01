namespace GUI_proyecto
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            sistemaToolStripMenuItem = new ToolStripMenuItem();
            cerrarSesionToolStripMenuItem = new ToolStripMenuItem();
            salirDeLaAplicacionToolStripMenuItem = new ToolStripMenuItem();
            herramientasToolStripMenuItem = new ToolStripMenuItem();
            importarFlotaDesdeCsvToolStripMenuItem = new ToolStripMenuItem();
            exportarReporteACSVToolStripMenuItem = new ToolStripMenuItem();
            ayudaToolStripMenuItem = new ToolStripMenuItem();
            acercaDeToolStripMenuItem = new ToolStripMenuItem();
            hansAlvaradoToolStripMenuItem = new ToolStripMenuItem();
            marioMonroyToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { sistemaToolStripMenuItem, herramientasToolStripMenuItem, ayudaToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            menuStrip1.ItemClicked += menuStrip1_ItemClicked;
            // 
            // sistemaToolStripMenuItem
            // 
            sistemaToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cerrarSesionToolStripMenuItem, salirDeLaAplicacionToolStripMenuItem });
            sistemaToolStripMenuItem.Name = "sistemaToolStripMenuItem";
            sistemaToolStripMenuItem.Size = new Size(75, 24);
            sistemaToolStripMenuItem.Text = "Sistema";
            // 
            // cerrarSesionToolStripMenuItem
            // 
            cerrarSesionToolStripMenuItem.Name = "cerrarSesionToolStripMenuItem";
            cerrarSesionToolStripMenuItem.Size = new Size(230, 26);
            cerrarSesionToolStripMenuItem.Text = "Cerrar sesion";
            cerrarSesionToolStripMenuItem.Click += cerrarSesionToolStripMenuItem_Click;
            // 
            // salirDeLaAplicacionToolStripMenuItem
            // 
            salirDeLaAplicacionToolStripMenuItem.Name = "salirDeLaAplicacionToolStripMenuItem";
            salirDeLaAplicacionToolStripMenuItem.Size = new Size(230, 26);
            salirDeLaAplicacionToolStripMenuItem.Text = "Salir de la aplicacion";
            salirDeLaAplicacionToolStripMenuItem.Click += salirDeLaAplicacionToolStripMenuItem_Click;
            // 
            // herramientasToolStripMenuItem
            // 
            herramientasToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { importarFlotaDesdeCsvToolStripMenuItem, exportarReporteACSVToolStripMenuItem });
            herramientasToolStripMenuItem.Name = "herramientasToolStripMenuItem";
            herramientasToolStripMenuItem.Size = new Size(112, 24);
            herramientasToolStripMenuItem.Text = "Herramientas";
            // 
            // importarFlotaDesdeCsvToolStripMenuItem
            // 
            importarFlotaDesdeCsvToolStripMenuItem.Name = "importarFlotaDesdeCsvToolStripMenuItem";
            importarFlotaDesdeCsvToolStripMenuItem.Size = new Size(253, 26);
            importarFlotaDesdeCsvToolStripMenuItem.Text = "Importar flota desde csv";
            // 
            // exportarReporteACSVToolStripMenuItem
            // 
            exportarReporteACSVToolStripMenuItem.Name = "exportarReporteACSVToolStripMenuItem";
            exportarReporteACSVToolStripMenuItem.Size = new Size(253, 26);
            exportarReporteACSVToolStripMenuItem.Text = "Exportar reporte a CSV";
            // 
            // ayudaToolStripMenuItem
            // 
            ayudaToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { acercaDeToolStripMenuItem });
            ayudaToolStripMenuItem.Name = "ayudaToolStripMenuItem";
            ayudaToolStripMenuItem.Size = new Size(65, 24);
            ayudaToolStripMenuItem.Text = "Ayuda";
            // 
            // acercaDeToolStripMenuItem
            // 
            acercaDeToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { hansAlvaradoToolStripMenuItem, marioMonroyToolStripMenuItem });
            acercaDeToolStripMenuItem.Name = "acercaDeToolStripMenuItem";
            acercaDeToolStripMenuItem.Size = new Size(224, 26);
            acercaDeToolStripMenuItem.Text = "Acerca de";
            // 
            // hansAlvaradoToolStripMenuItem
            // 
            hansAlvaradoToolStripMenuItem.Name = "hansAlvaradoToolStripMenuItem";
            hansAlvaradoToolStripMenuItem.Size = new Size(224, 26);
            hansAlvaradoToolStripMenuItem.Text = "Hans Alvarado";
            hansAlvaradoToolStripMenuItem.Click += hansAlvaradoToolStripMenuItem_Click;
            // 
            // marioMonroyToolStripMenuItem
            // 
            marioMonroyToolStripMenuItem.Name = "marioMonroyToolStripMenuItem";
            marioMonroyToolStripMenuItem.Size = new Size(224, 26);
            marioMonroyToolStripMenuItem.Text = "Mario Monroy";
            marioMonroyToolStripMenuItem.Click += marioMonroyToolStripMenuItem_Click;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form2";
            Text = "Form2";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem sistemaToolStripMenuItem;
        private ToolStripMenuItem cerrarSesionToolStripMenuItem;
        private ToolStripMenuItem salirDeLaAplicacionToolStripMenuItem;
        private ToolStripMenuItem herramientasToolStripMenuItem;
        private ToolStripMenuItem importarFlotaDesdeCsvToolStripMenuItem;
        private ToolStripMenuItem exportarReporteACSVToolStripMenuItem;
        private ToolStripMenuItem ayudaToolStripMenuItem;
        private ToolStripMenuItem acercaDeToolStripMenuItem;
        private ToolStripMenuItem hansAlvaradoToolStripMenuItem;
        private ToolStripMenuItem marioMonroyToolStripMenuItem;
    }
}