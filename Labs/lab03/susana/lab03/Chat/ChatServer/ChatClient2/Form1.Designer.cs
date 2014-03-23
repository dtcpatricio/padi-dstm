namespace ChatClient2
{
    partial class Form1
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
            this.portNumberBox = new System.Windows.Forms.TextBox();
            this.usernameBox = new System.Windows.Forms.TextBox();
            this.registerButton = new System.Windows.Forms.Button();
            this.writeMessageBox = new System.Windows.Forms.TextBox();
            this.sendMessageButton = new System.Windows.Forms.Button();
            this.messageLabel = new System.Windows.Forms.Label();
            this.messageBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // portNumberBox
            // 
            this.portNumberBox.Location = new System.Drawing.Point(114, 12);
            this.portNumberBox.Name = "portNumberBox";
            this.portNumberBox.Size = new System.Drawing.Size(63, 20);
            this.portNumberBox.TabIndex = 0;
            this.portNumberBox.TextChanged += new System.EventHandler(this.portNumberBox_TextChanged);
            // 
            // usernameBox
            // 
            this.usernameBox.Location = new System.Drawing.Point(12, 50);
            this.usernameBox.Name = "usernameBox";
            this.usernameBox.Size = new System.Drawing.Size(141, 20);
            this.usernameBox.TabIndex = 1;
            this.usernameBox.TextChanged += new System.EventHandler(this.usernameBox_TextChanged);
            // 
            // registerButton
            // 
            this.registerButton.Location = new System.Drawing.Point(187, 47);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(85, 23);
            this.registerButton.TabIndex = 2;
            this.registerButton.Text = "Register";
            this.registerButton.UseVisualStyleBackColor = true;
            this.registerButton.Click += new System.EventHandler(this.registerButton_Click);
            // 
            // writeMessageBox
            // 
            this.writeMessageBox.Location = new System.Drawing.Point(12, 87);
            this.writeMessageBox.Name = "writeMessageBox";
            this.writeMessageBox.Size = new System.Drawing.Size(141, 20);
            this.writeMessageBox.TabIndex = 3;
            this.writeMessageBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // sendMessageButton
            // 
            this.sendMessageButton.Location = new System.Drawing.Point(187, 85);
            this.sendMessageButton.Name = "sendMessageButton";
            this.sendMessageButton.Size = new System.Drawing.Size(85, 23);
            this.sendMessageButton.TabIndex = 4;
            this.sendMessageButton.Text = "Send";
            this.sendMessageButton.UseVisualStyleBackColor = true;
            this.sendMessageButton.Click += new System.EventHandler(this.sendMessageButton_Click);
            // 
            // messageLabel
            // 
            this.messageLabel.AutoSize = true;
            this.messageLabel.Location = new System.Drawing.Point(118, 127);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(55, 13);
            this.messageLabel.TabIndex = 6;
            this.messageLabel.Text = "Messages";
            this.messageLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // messageBox
            // 
            this.messageBox.Location = new System.Drawing.Point(12, 152);
            this.messageBox.Multiline = true;
            this.messageBox.Name = "messageBox";
            this.messageBox.Size = new System.Drawing.Size(260, 97);
            this.messageBox.TabIndex = 5;
            this.messageBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.messageLabel);
            this.Controls.Add(this.messageBox);
            this.Controls.Add(this.sendMessageButton);
            this.Controls.Add(this.writeMessageBox);
            this.Controls.Add(this.registerButton);
            this.Controls.Add(this.usernameBox);
            this.Controls.Add(this.portNumberBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox portNumberBox;
        private System.Windows.Forms.TextBox usernameBox;
        private System.Windows.Forms.Button registerButton;
        private System.Windows.Forms.TextBox writeMessageBox;
        private System.Windows.Forms.Button sendMessageButton;
        private System.Windows.Forms.Label messageLabel;
        private System.Windows.Forms.TextBox messageBox;
    }
}

