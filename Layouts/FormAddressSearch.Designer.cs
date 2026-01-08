namespace EduKin.Layouts
{
    partial class FormAddressSearch
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblAvenue = new System.Windows.Forms.Label();
            this.txtAvenue = new System.Windows.Forms.TextBox();
            this.lstAvenues = new System.Windows.Forms.ListView();
            this.lblQuartier = new System.Windows.Forms.Label();
            this.txtQuartier = new System.Windows.Forms.TextBox();
            this.lblCommune = new System.Windows.Forms.Label();
            this.txtCommune = new System.Windows.Forms.TextBox();
            this.lblVille = new System.Windows.Forms.Label();
            this.txtVille = new System.Windows.Forms.TextBox();
            this.lblProvince = new System.Windows.Forms.Label();
            this.txtProvince = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnManualEntry = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(200, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Recherche Adresse";
            // 
            // lblAvenue
            // 
            this.lblAvenue.AutoSize = true;
            this.lblAvenue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblAvenue.Location = new System.Drawing.Point(12, 60);
            this.lblAvenue.Name = "lblAvenue";
            this.lblAvenue.Size = new System.Drawing.Size(54, 20);
            this.lblAvenue.TabIndex = 1;
            this.lblAvenue.Text = "Avenue";
            // 
            // txtAvenue
            // 
            this.txtAvenue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtAvenue.Location = new System.Drawing.Point(12, 83);
            this.txtAvenue.Name = "txtAvenue";
            this.txtAvenue.PlaceholderText = "Tapez pour rechercher une avenue...";
            this.txtAvenue.Size = new System.Drawing.Size(350, 27);
            this.txtAvenue.TabIndex = 2;
            this.txtAvenue.TextChanged += new System.EventHandler(this.txtAvenue_TextChanged);
            // 
            // lstAvenues
            // 
            this.lstAvenues.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lstAvenues.FullRowSelect = true;
            this.lstAvenues.GridLines = true;
            this.lstAvenues.HideSelection = false;
            this.lstAvenues.Location = new System.Drawing.Point(12, 116);
            this.lstAvenues.MultiSelect = false;
            this.lstAvenues.Name = "lstAvenues";
            this.lstAvenues.Size = new System.Drawing.Size(560, 150);
            this.lstAvenues.TabIndex = 3;
            this.lstAvenues.UseCompatibleStateImageBehavior = false;
            this.lstAvenues.View = System.Windows.Forms.View.List;
            this.lstAvenues.SelectedIndexChanged += new System.EventHandler(this.lstAvenues_SelectedIndexChanged);
            // 
            // lblQuartier
            // 
            this.lblQuartier.AutoSize = true;
            this.lblQuartier.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblQuartier.Location = new System.Drawing.Point(380, 60);
            this.lblQuartier.Name = "lblQuartier";
            this.lblQuartier.Size = new System.Drawing.Size(63, 20);
            this.lblQuartier.TabIndex = 4;
            this.lblQuartier.Text = "Quartier";
            // 
            // txtQuartier
            // 
            this.txtQuartier.BackColor = System.Drawing.SystemColors.Control;
            this.txtQuartier.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtQuartier.Location = new System.Drawing.Point(380, 83);
            this.txtQuartier.Name = "txtQuartier";
            this.txtQuartier.ReadOnly = true;
            this.txtQuartier.Size = new System.Drawing.Size(192, 27);
            this.txtQuartier.TabIndex = 5;
            // 
            // lblCommune
            // 
            this.lblCommune.AutoSize = true;
            this.lblCommune.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCommune.Location = new System.Drawing.Point(12, 280);
            this.lblCommune.Name = "lblCommune";
            this.lblCommune.Size = new System.Drawing.Size(72, 20);
            this.lblCommune.TabIndex = 6;
            this.lblCommune.Text = "Commune";
            // 
            // txtCommune
            // 
            this.txtCommune.BackColor = System.Drawing.SystemColors.Control;
            this.txtCommune.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtCommune.Location = new System.Drawing.Point(12, 303);
            this.txtCommune.Name = "txtCommune";
            this.txtCommune.ReadOnly = true;
            this.txtCommune.Size = new System.Drawing.Size(180, 27);
            this.txtCommune.TabIndex = 7;
            // 
            // lblVille
            // 
            this.lblVille.AutoSize = true;
            this.lblVille.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblVille.Location = new System.Drawing.Point(210, 280);
            this.lblVille.Name = "lblVille";
            this.lblVille.Size = new System.Drawing.Size(36, 20);
            this.lblVille.TabIndex = 8;
            this.lblVille.Text = "Ville";
            // 
            // txtVille
            // 
            this.txtVille.BackColor = System.Drawing.SystemColors.Control;
            this.txtVille.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtVille.Location = new System.Drawing.Point(210, 303);
            this.txtVille.Name = "txtVille";
            this.txtVille.ReadOnly = true;
            this.txtVille.Size = new System.Drawing.Size(180, 27);
            this.txtVille.TabIndex = 9;
            // 
            // lblProvince
            // 
            this.lblProvince.AutoSize = true;
            this.lblProvince.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblProvince.Location = new System.Drawing.Point(408, 280);
            this.lblProvince.Name = "lblProvince";
            this.lblProvince.Size = new System.Drawing.Size(64, 20);
            this.lblProvince.TabIndex = 10;
            this.lblProvince.Text = "Province";
            // 
            // txtProvince
            // 
            this.txtProvince.BackColor = System.Drawing.SystemColors.Control;
            this.txtProvince.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtProvince.Location = new System.Drawing.Point(408, 303);
            this.txtProvince.Name = "txtProvince";
            this.txtProvince.ReadOnly = true;
            this.txtProvince.Size = new System.Drawing.Size(164, 27);
            this.txtProvince.TabIndex = 11;
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnOK.Enabled = false;
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnOK.ForeColor = System.Drawing.Color.White;
            this.btnOK.Location = new System.Drawing.Point(380, 350);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 35);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(482, 350);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 35);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Annuler";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnManualEntry
            // 
            this.btnManualEntry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7)))));
            this.btnManualEntry.FlatAppearance.BorderSize = 0;
            this.btnManualEntry.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManualEntry.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnManualEntry.ForeColor = System.Drawing.Color.Black;
            this.btnManualEntry.Location = new System.Drawing.Point(12, 350);
            this.btnManualEntry.Name = "btnManualEntry";
            this.btnManualEntry.Size = new System.Drawing.Size(120, 35);
            this.btnManualEntry.TabIndex = 14;
            this.btnManualEntry.Text = "Saisie manuelle";
            this.btnManualEntry.UseVisualStyleBackColor = false;
            this.btnManualEntry.Click += new System.EventHandler(this.btnManualEntry_Click);
            // 
            // FormAddressSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(584, 401);
            this.Controls.Add(this.btnManualEntry);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtProvince);
            this.Controls.Add(this.lblProvince);
            this.Controls.Add(this.txtVille);
            this.Controls.Add(this.lblVille);
            this.Controls.Add(this.txtCommune);
            this.Controls.Add(this.lblCommune);
            this.Controls.Add(this.txtQuartier);
            this.Controls.Add(this.lblQuartier);
            this.Controls.Add(this.lstAvenues);
            this.Controls.Add(this.txtAvenue);
            this.Controls.Add(this.lblAvenue);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAddressSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Recherche d\'Adresse";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblAvenue;
        private System.Windows.Forms.TextBox txtAvenue;
        private System.Windows.Forms.ListView lstAvenues;
        private System.Windows.Forms.Label lblQuartier;
        private System.Windows.Forms.TextBox txtQuartier;
        private System.Windows.Forms.Label lblCommune;
        private System.Windows.Forms.TextBox txtCommune;
        private System.Windows.Forms.Label lblVille;
        private System.Windows.Forms.TextBox txtVille;
        private System.Windows.Forms.Label lblProvince;
        private System.Windows.Forms.TextBox txtProvince;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnManualEntry;
    }
}