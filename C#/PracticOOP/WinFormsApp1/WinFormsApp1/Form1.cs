using System;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent(); // Это работает только здесь!

            // Ваш код для textBoxFloat
            textBoxFloat.KeyPress += TextBoxFloat_KeyPress;
        }

        private void TextBoxFloat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 67)
            {

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("LoudOn");
        }

        private void button1_Click(object sender, EventArgs e)
        {

            calculate(textBoxFloat.Text);

            if (textBoxFloat.Text == "67")
            {
                Close();
            }
        }



    }
}