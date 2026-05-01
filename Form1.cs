using System.Text.RegularExpressions;
using static System.Windows.Forms.DataFormats;

namespace GUI_proyecto
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //despues se cambia cuando se conecte con el sql, esto es de prueba
        string usuario1 = "admin";
        string contra1 = "1234";
        string usuario2 = "recepcion";
        string contra2 = "5678";


        //esto debe quedarse
        string contraReg = @"\d{4}";
        Form2 consultas = new Form2();










        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void validar_Click(object sender, EventArgs e)
        {
            //

            if (Regex.IsMatch(passBox.Text, contraReg))
            {
                if (usuario1 == nameBox.Text && contra1 == passBox.Text)
                {

                    consultas.Show();
                }
                if (usuario2 == nameBox.Text && contra2 == nameBox.Text)
                {
                    //en esta condicion deberia ir el menu que solo pueden ver los clientes

                    consultas.Show();

                }
            }

            //
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //

            if (checkBox1.Checked)
            {
                passBox.UseSystemPasswordChar = false;
            }
            else
            {
                passBox.UseSystemPasswordChar = true;

            }

            //
        }


        //este evento no debe hacer nada xd
        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
