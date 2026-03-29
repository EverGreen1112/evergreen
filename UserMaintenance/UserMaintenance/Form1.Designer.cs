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
            listBox1 = new ListBox();
            label1 = new Label();
            label2 = new Label();
            lblLastName = new TextBox();
            lblFirstName = new TextBox();
            btnAdd = new Button();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(61, 107);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(275, 292);
            listBox1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(387, 128);
            label1.Name = "label1";
            label1.Size = new Size(78, 32);
            label1.TabIndex = 1;
            label1.Text = "label1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(387, 193);
            label2.Name = "label2";
            label2.Size = new Size(78, 32);
            label2.TabIndex = 2;
            label2.Text = "label2";
            // 
            // lblLastName
            // 
            lblLastName.Location = new Point(494, 128);
            lblLastName.Name = "lblLastName";
            lblLastName.Size = new Size(200, 39);
            lblLastName.TabIndex = 3;
            // 
            // lblFirstName
            // 
            lblFirstName.Location = new Point(494, 190);
            lblFirstName.Name = "lblFirstName";
            lblFirstName.Size = new Size(200, 39);
            lblFirstName.TabIndex = 4;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(387, 274);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(307, 46);
            btnAdd.TabIndex = 5;
            btnAdd.Text = "button1";
            btnAdd.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1254, 642);
            Controls.Add(btnAdd);
            Controls.Add(lblFirstName);
            Controls.Add(lblLastName);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(listBox1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBox1;
        private Label label1;
        private Label label2;
        private TextBox lblLastName;
        private TextBox lblFirstName;
        private Button btnAdd;
    }
}
