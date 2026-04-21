namespace Hotcakes_Desktop
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
            lbOrders = new ListBox();
            dgvItems = new DataGridView();
            btnLoad = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvItems).BeginInit();
            SuspendLayout();
            // 
            // lbOrders
            // 
            lbOrders.FormattingEnabled = true;
            lbOrders.Location = new Point(39, 147);
            lbOrders.Name = "lbOrders";
            lbOrders.Size = new Size(363, 708);
            lbOrders.TabIndex = 0;
            // 
            // dgvItems
            // 
            dgvItems.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            dgvItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvItems.Location = new Point(474, 35);
            dgvItems.Name = "dgvItems";
            dgvItems.RowHeadersWidth = 82;
            dgvItems.Size = new Size(1295, 820);
            dgvItems.TabIndex = 1;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(39, 35);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(150, 46);
            btnLoad.TabIndex = 2;
            btnLoad.Text = "Betöltés";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1798, 920);
            Controls.Add(btnLoad);
            Controls.Add(dgvItems);
            Controls.Add(lbOrders);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dgvItems).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ListBox lbOrders;
        private DataGridView dgvItems;
        private Button btnLoad;
    }
}
