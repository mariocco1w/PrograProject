namespace GUI_proyecto
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            nameBox = new TextBox();
            passBox = new TextBox();
            validar = new Button();
            checkBox1 = new CheckBox();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // nameBox
            // 
            nameBox.Location = new Point(266, 87);
            nameBox.Name = "nameBox";
            nameBox.Size = new Size(135, 27);
            nameBox.TabIndex = 0;
            // 
            // passBox
            // 
            passBox.Location = new Point(266, 198);
            passBox.Name = "passBox";
            passBox.Size = new Size(135, 27);
            passBox.TabIndex = 1;
            passBox.UseSystemPasswordChar = true;
            // 
            // validar
            // 
            validar.Location = new Point(266, 320);
            validar.Name = "validar";
            validar.Size = new Size(137, 30);
            validar.TabIndex = 2;
            validar.Text = "Ingresar";
            validar.UseVisualStyleBackColor = true;
            validar.Click += validar_Click;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(442, 324);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(128, 24);
            checkBox1.TabIndex = 3;
            checkBox1.Text = "Mostrar contra";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(270, 62);
            label1.Name = "label1";
            label1.Size = new Size(131, 20);
            label1.TabIndex = 4;
            label1.Text = "Ingrese su nombre";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(266, 175);
            label2.Name = "label2";
            label2.Size = new Size(121, 20);
            label2.TabIndex = 5;
            label2.Text = "Ingrese su contra";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(checkBox1);
            Controls.Add(validar);
            Controls.Add(passBox);
            Controls.Add(nameBox);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox nameBox;
        private TextBox passBox;
        private Button validar;
        private CheckBox checkBox1;
        private Label label1;
        private Label label2;
    }
}
