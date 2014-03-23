using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using ChatLib;

namespace ChatClient2
{
    public partial class Form1 : Form
    {
        public delegate void deleteAddLine(string method);

        ChatClient2 chatClient;
        public deleteAddLine myDelegate;

        public void addLine(string message)
        {
            messageBox.AppendText("\r\n" + message);
        }

        public Form1()
        {
            InitializeComponent();
            chatClient = new ChatClient2(this);
            myDelegate = new deleteAddLine(addLine);
        }

        public void showBox(string message)
        {
            MessageBox.Show(message);
        }

        private void portNumberBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            if (portNumberBox.TextLength == 0 || usernameBox.TextLength == 0)
            {
                MessageBox.Show("ERROR: Please insert port number and username.");
            }
            else
            {
                chatClient.setPortNumber(Convert.ToInt32(portNumberBox.Text));
                chatClient.registerToServer(usernameBox.Text, portNumberBox.Text);
                messageBox.AppendText("You were added to the conversation.");
            }
        }

        private void usernameBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void sendMessageButton_Click(object sender, EventArgs e)
        {
            chatClient.sendMessageToServer(writeMessageBox.Text);
            messageBox.AppendText("\r\n" + chatClient.getNick() + " > " + writeMessageBox.Text);
            writeMessageBox.Clear();
        }
    }
}
