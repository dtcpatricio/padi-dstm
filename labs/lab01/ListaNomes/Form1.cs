using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ListaNomesLib;

namespace ListaNomesApp
{
    public partial class Form1 : Form
    {
        ListaNomesLib.ListaNomes list;

        public Form1() {
            list = new ListaNomesLib.ListaNomes();
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.TextLength == 0)
            {
                MessageBox.Show("Error: No name added");
            }
            else
            {
                list.addName(textBox1.Text);
                MessageBox.Show("Added: " + textBox1.Text);
                textBox1.Clear();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string names = "";
            foreach (string n in list.getNames())
                names = names + " " + n;
            MessageBox.Show("Names: " + names);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            list.clean();
        }
    }
}
