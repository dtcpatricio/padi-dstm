namespace Client
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
            this.uidCreate = new System.Windows.Forms.TextBox();
            this.createButton = new System.Windows.Forms.Button();
            this.uidAccess = new System.Windows.Forms.TextBox();
            this.accessButton = new System.Windows.Forms.Button();
            this.readButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // uidCreate
            // 
            this.uidCreate.Location = new System.Drawing.Point(12, 111);
            this.uidCreate.Name = "uidCreate";
            this.uidCreate.Size = new System.Drawing.Size(100, 20);
            this.uidCreate.TabIndex = 1;
            // 
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point(171, 111);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(75, 23);
            this.createButton.TabIndex = 2;
            this.createButton.Text = "Create";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // uidAccess
            // 
            this.uidAccess.Location = new System.Drawing.Point(12, 154);
            this.uidAccess.Name = "uidAccess";
            this.uidAccess.Size = new System.Drawing.Size(100, 20);
            this.uidAccess.TabIndex = 3;
            // 
            // accessButton
            // 
            this.accessButton.Location = new System.Drawing.Point(171, 151);
            this.accessButton.Name = "accessButton";
            this.accessButton.Size = new System.Drawing.Size(75, 23);
            this.accessButton.TabIndex = 4;
            this.accessButton.Text = "Access";
            this.accessButton.UseVisualStyleBackColor = true;
            this.accessButton.Click += new System.EventHandler(this.accessButton_Click);
            // 
            // readButton
            // 
            this.readButton.Location = new System.Drawing.Point(171, 194);
            this.readButton.Name = "readButton";
            this.readButton.Size = new System.Drawing.Size(75, 23);
            this.readButton.TabIndex = 6;
            this.readButton.Text = "Read";
            this.readButton.UseVisualStyleBackColor = true;
            this.readButton.Click += new System.EventHandler(this.readButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.readButton);
            this.Controls.Add(this.accessButton);
            this.Controls.Add(this.uidAccess);
            this.Controls.Add(this.createButton);
            this.Controls.Add(this.uidCreate);
            this.Name = "Form1";
            this.Text = "PADI-DSTM";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox uidCreate;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.TextBox uidAccess;
        private System.Windows.Forms.Button accessButton;
        private System.Windows.Forms.Button readButton;
    }
}

