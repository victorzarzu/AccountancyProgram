namespace Nova_Tools
{
    partial class Iesiri
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.plati_button = new System.Windows.Forms.Button();
            this.creeaza_button = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.plati_button, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.creeaza_button, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(391, 165);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // plati_button
            // 
            this.plati_button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.plati_button.BackColor = System.Drawing.Color.CornflowerBlue;
            this.plati_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.plati_button.ForeColor = System.Drawing.Color.White;
            this.plati_button.Location = new System.Drawing.Point(81, 107);
            this.plati_button.Name = "plati_button";
            this.plati_button.Size = new System.Drawing.Size(228, 33);
            this.plati_button.TabIndex = 3;
            this.plati_button.Text = "Facturi";
            this.plati_button.UseVisualStyleBackColor = false;
            this.plati_button.Click += new System.EventHandler(this.plati_button_Click);
            // 
            // creeaza_button
            // 
            this.creeaza_button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.creeaza_button.BackColor = System.Drawing.Color.CornflowerBlue;
            this.creeaza_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.creeaza_button.ForeColor = System.Drawing.Color.White;
            this.creeaza_button.Location = new System.Drawing.Point(81, 24);
            this.creeaza_button.Name = "creeaza_button";
            this.creeaza_button.Size = new System.Drawing.Size(228, 33);
            this.creeaza_button.TabIndex = 2;
            this.creeaza_button.Text = "Creeaza";
            this.creeaza_button.UseVisualStyleBackColor = false;
            this.creeaza_button.Click += new System.EventHandler(this.creeaza_button_Click);
            // 
            // Iesiri
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightBlue;
            this.ClientSize = new System.Drawing.Size(391, 165);
            this.Controls.Add(this.tableLayoutPanel1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "Iesiri";
            this.Text = "Iesiri";
            this.Load += new System.EventHandler(this.Iesiri_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button creeaza_button;
        private System.Windows.Forms.Button plati_button;
    }
}