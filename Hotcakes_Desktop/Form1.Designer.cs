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
            txtSku = new TextBox();
            txtQuantity = new TextBox();
            btnPack = new Button();
            btnFinish = new Button();
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
            dgvItems.Size = new Size(1295, 642);
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
            // txtSku
            // 
            txtSku.Location = new Point(474, 736);
            txtSku.Name = "txtSku";
            txtSku.Size = new Size(403, 39);
            txtSku.TabIndex = 3;
            // 
            // txtQuantity
            // 
            txtQuantity.Location = new Point(932, 736);
            txtQuantity.Name = "txtQuantity";
            txtQuantity.Size = new Size(119, 39);
            txtQuantity.TabIndex = 4;
            // 
            // btnPack
            // 
            btnPack.Location = new Point(1104, 736);
            btnPack.Name = "btnPack";
            btnPack.Size = new Size(150, 46);
            btnPack.TabIndex = 5;
            btnPack.Text = "Hozzáadás";
            btnPack.UseVisualStyleBackColor = true;
            btnPack.Click += btnPack_Click;
            // 
            // btnFinish
            // 
            btnFinish.Location = new Point(1574, 819);
            btnFinish.Name = "btnFinish";
            btnFinish.Size = new Size(180, 77);
            btnFinish.TabIndex = 6;
            btnFinish.Text = "Kész";
            btnFinish.UseVisualStyleBackColor = true;
            btnFinish.Click += btnFinish_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1798, 920);
            Controls.Add(btnFinish);
            Controls.Add(btnPack);
            Controls.Add(txtQuantity);
            Controls.Add(txtSku);
            Controls.Add(btnLoad);
            Controls.Add(dgvItems);
            Controls.Add(lbOrders);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dgvItems).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox lbOrders;
        private DataGridView dgvItems;
        private Button btnLoad;
        private TextBox txtSku;
        private TextBox txtQuantity;
        private Button btnPack;
        private Button btnFinish;
    }
}
