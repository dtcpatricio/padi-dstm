using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using padi_dstm_library;

namespace Client
{
    public partial class Form1 : Form
    {
        PadiClient padiClient;

        public delegate void showMessageBox(string method);
        public showMessageBox myDelegate;

        public Form1()
        {
            InitializeComponent();
            padiClient = new PadiClient(this);
            myDelegate = new showMessageBox(show);
        }

        public void show(string message)
        {
            MessageBox.Show(message);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (uidCreate.TextLength == 0)
            {
                MessageBox.Show("ERROR: Please insert requested uid");
            }
            else
            {
                padiClient.setChannel();
                PadInt pi = padiClient.CreatePadInt(Convert.ToInt32(uidCreate.Text));
                if (pi == null)
                {
                    MessageBox.Show("PadInt not created, please insert again");
                }
                else
                {
                    MessageBox.Show("PadInt with uid " + pi.getUid() + " and value: " + pi.getValue() + " created successfully");
                }
            }
        }

        private void accessButton_Click(object sender, EventArgs e)
        {
            if (uidAccess.TextLength == 0)
            {
                MessageBox.Show("ERROR: Please insert requested uid");
            }
            else
            {
                padiClient.setChannel();
                PadInt pi = padiClient.AccessPadInt(Convert.ToInt32(uidAccess.Text));
                if (pi == null)
                {
                    MessageBox.Show("PadInt not accessed, please insert again");
                }
                else
                {
                    MessageBox.Show("PadInt with uid " + pi.getUid() + " and value: " + pi.getValue() + " accessed successfully");
                }
            }
        }

        // Para leitura devemos sempre fazer access primeiro para termos o valor mais recente do PadInt
        // Se nao, como criar um proxy remoto do padInt?
        private void readButton_Click(object sender, EventArgs e)
        {
            int pi = padiClient.ReadPadInt();
            MessageBox.Show("Read result: " + pi);
        }
    }
}
