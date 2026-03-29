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
            lblFullName = new Label();
            txtFullName = new TextBox();
            btnAdd = new Button();
            btnSave = new Button();
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
            // lblFullName
            // 
            lblFullName.AutoSize = true;
            lblFullName.Location = new Point(361, 193);
            lblFullName.Name = "lblFullName";
            lblFullName.Size = new Size(78, 32);
            lblFullName.TabIndex = 2;
            lblFullName.Text = "label2";
            // 
            // txtFullName
            // 
            txtFullName.Location = new Point(494, 190);
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(200, 39);
            txtFullName.TabIndex = 4;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(361, 273);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(333, 46);
            btnAdd.TabIndex = 5;
            btnAdd.Text = "button1";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(361, 334);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(333, 46);
            btnSave.TabIndex = 6;
            btnSave.Text = "buttonSave";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1254, 642);
            Controls.Add(btnSave);
            Controls.Add(btnAdd);
            Controls.Add(txtFullName);
            Controls.Add(lblFullName);
            Controls.Add(listUsers);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listUsers;
        private Label lblFullName;
        private TextBox txtFullName;
        private Button btnAdd;
        private Button btnSave;
    }
}
