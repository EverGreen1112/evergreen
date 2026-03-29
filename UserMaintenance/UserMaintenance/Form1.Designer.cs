namespace UserMaintenance
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
            listUsers = new ListBox();
            lblFirstName = new Label();
            lblLastName = new Label();
            txtLastName = new TextBox();
            txtFirstName = new TextBox();
            btnAdd = new Button();
            SuspendLayout();
            // 
            // listUsers
            // 
            listUsers.FormattingEnabled = true;
            listUsers.Location = new Point(61, 107);
            listUsers.Name = "listUsers";
            listUsers.Size = new Size(275, 292);
            listUsers.TabIndex = 0;
            // 
            // lblFirstName
            // 
            lblFirstName.AutoSize = true;
            lblFirstName.Location = new Point(387, 128);
            lblFirstName.Name = "lblFirstName";
            lblFirstName.Size = new Size(78, 32);
            lblFirstName.TabIndex = 1;
            lblFirstName.Text = "label1";
            // 
            // lblLastName
            // 
            lblLastName.AutoSize = true;
            lblLastName.Location = new Point(387, 193);
            lblLastName.Name = "lblLastName";
            lblLastName.Size = new Size(78, 32);
            lblLastName.TabIndex = 2;
            lblLastName.Text = "label2";
            // 
            // txtLastName
            // 
            txtLastName.Location = new Point(494, 128);
            txtLastName.Name = "txtLastName";
            txtLastName.Size = new Size(200, 39);
            txtLastName.TabIndex = 3;
            // 
            // txtFirstName
            // 
            txtFirstName.Location = new Point(494, 190);
            txtFirstName.Name = "txtFirstName";
            txtFirstName.Size = new Size(200, 39);
            txtFirstName.TabIndex = 4;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(387, 274);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(307, 46);
            btnAdd.TabIndex = 5;
            btnAdd.Text = "button1";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1254, 642);
            Controls.Add(btnAdd);
            Controls.Add(txtFirstName);
            Controls.Add(txtLastName);
            Controls.Add(lblLastName);
            Controls.Add(lblFirstName);
            Controls.Add(listUsers);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listUsers;
        private Label lblFirstName;
        private Label lblLastName;
        private TextBox txtLastName;
        private TextBox txtFirstName;
        private Button btnAdd;
    }
}
