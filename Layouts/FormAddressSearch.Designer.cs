using System.Windows.Forms;
using Siticone.Desktop.UI.WinForms;

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
            panelMain = new SiticonePanel();
            lblTitle = new Label();
            lblAvenue = new Label();
            txtAvenue = new SiticoneTextBox();
            lstAvenues = new ListView();
            lblQuartier = new Label();
            txtQuartier = new SiticoneTextBox();
            lblCommune = new Label();
            txtCommune = new SiticoneTextBox();
            lblVille = new Label();
            txtVille = new SiticoneTextBox();
            lblProvince = new Label();
            txtProvince = new SiticoneTextBox();
            btnOK = new SiticoneButton();
            btnCancel = new SiticoneButton();
            btnManualEntry = new SiticoneButton();
            lblInfo = new Label();
            panelMain.SuspendLayout();
            SuspendLayout();
            // 
            // panelMain
            // 
            panelMain.BackColor = Color.Transparent;
            panelMain.BorderRadius = 20;
            panelMain.Controls.Add(lblTitle);
            panelMain.Controls.Add(lblAvenue);
            panelMain.Controls.Add(txtAvenue);
            panelMain.Controls.Add(lstAvenues);
            panelMain.Controls.Add(lblQuartier);
            panelMain.Controls.Add(txtQuartier);
            panelMain.Controls.Add(lblCommune);
            panelMain.Controls.Add(txtCommune);
            panelMain.Controls.Add(lblVille);
            panelMain.Controls.Add(txtVille);
            panelMain.Controls.Add(lblProvince);
            panelMain.Controls.Add(txtProvince);
            panelMain.Controls.Add(btnOK);
            panelMain.Controls.Add(btnCancel);
            panelMain.Controls.Add(btnManualEntry);
            panelMain.Controls.Add(lblInfo);
            panelMain.FillColor = Color.White;
            panelMain.Location = new Point(20, 20);
            panelMain.Name = "panelMain";
            panelMain.ShadowDecoration.BorderRadius = 20;
            panelMain.ShadowDecoration.Enabled = true;
            panelMain.Size = new Size(620, 480);
            panelMain.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(94, 148, 255);
            lblTitle.Location = new Point(20, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(580, 45);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Recherche Adresse";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblAvenue
            // 
            lblAvenue.AutoSize = true;
            lblAvenue.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblAvenue.ForeColor = Color.FromArgb(64, 64, 64);
            lblAvenue.Location = new Point(20, 70);
            lblAvenue.Name = "lblAvenue";
            lblAvenue.Size = new Size(64, 23);
            lblAvenue.TabIndex = 1;
            lblAvenue.Text = "Avenue";
            // 
            // txtAvenue
            // 
            txtAvenue.BorderColor = Color.FromArgb(5, 21, 48);
            txtAvenue.BorderRadius = 14;
            txtAvenue.DefaultText = "";
            txtAvenue.Font = new Font("Segoe UI", 9F);
            txtAvenue.Location = new Point(20, 95);
            txtAvenue.Margin = new Padding(3, 4, 3, 4);
            txtAvenue.Name = "txtAvenue";
            txtAvenue.PasswordChar = '\0';
            txtAvenue.PlaceholderText = "Tapez pour rechercher une avenue...";
            txtAvenue.SelectedText = "";
            txtAvenue.Size = new Size(350, 36);
            txtAvenue.TabIndex = 2;
            txtAvenue.TextChanged += new EventHandler(txtAvenue_TextChanged);
            // 
            // lstAvenues
            // 
            lstAvenues.Font = new Font("Segoe UI", 9F);
            lstAvenues.FullRowSelect = true;
            lstAvenues.GridLines = true;
            lstAvenues.HideSelection = false;
            lstAvenues.Location = new Point(20, 140);
            lstAvenues.MultiSelect = false;
            lstAvenues.Name = "lstAvenues";
            lstAvenues.Size = new Size(580, 150);
            lstAvenues.TabIndex = 3;
            lstAvenues.UseCompatibleStateImageBehavior = false;
            lstAvenues.View = View.List;
            lstAvenues.SelectedIndexChanged += new EventHandler(lstAvenues_SelectedIndexChanged);
            // 
            // lblQuartier
            // 
            lblQuartier.AutoSize = true;
            lblQuartier.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblQuartier.ForeColor = Color.FromArgb(64, 64, 64);
            lblQuartier.Location = new Point(390, 70);
            lblQuartier.Name = "lblQuartier";
            lblQuartier.Size = new Size(74, 23);
            lblQuartier.TabIndex = 4;
            lblQuartier.Text = "Quartier";
            // 
            // txtQuartier
            // 
            txtQuartier.BorderColor = Color.FromArgb(213, 218, 223);
            txtQuartier.BorderRadius = 14;
            txtQuartier.DefaultText = "";
            txtQuartier.FillColor = Color.FromArgb(245, 245, 245);
            txtQuartier.Font = new Font("Segoe UI", 9F);
            txtQuartier.ForeColor = Color.FromArgb(68, 88, 112);
            txtQuartier.Location = new Point(390, 95);
            txtQuartier.Margin = new Padding(3, 4, 3, 4);
            txtQuartier.Name = "txtQuartier";
            txtQuartier.PasswordChar = '\0';
            txtQuartier.PlaceholderText = "";
            txtQuartier.ReadOnly = true;
            txtQuartier.SelectedText = "";
            txtQuartier.Size = new Size(210, 36);
            txtQuartier.TabIndex = 5;
            // 
            // lblCommune
            // 
            lblCommune.AutoSize = true;
            lblCommune.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCommune.ForeColor = Color.FromArgb(64, 64, 64);
            lblCommune.Location = new Point(20, 300);
            lblCommune.Name = "lblCommune";
            lblCommune.Size = new Size(89, 23);
            lblCommune.TabIndex = 6;
            lblCommune.Text = "Commune";
            // 
            // txtCommune
            // 
            txtCommune.BorderColor = Color.FromArgb(213, 218, 223);
            txtCommune.BorderRadius = 14;
            txtCommune.DefaultText = "";
            txtCommune.FillColor = Color.FromArgb(245, 245, 245);
            txtCommune.Font = new Font("Segoe UI", 9F);
            txtCommune.ForeColor = Color.FromArgb(68, 88, 112);
            txtCommune.Location = new Point(20, 325);
            txtCommune.Margin = new Padding(3, 4, 3, 4);
            txtCommune.Name = "txtCommune";
            txtCommune.PasswordChar = '\0';
            txtCommune.PlaceholderText = "";
            txtCommune.ReadOnly = true;
            txtCommune.SelectedText = "";
            txtCommune.Size = new Size(180, 36);
            txtCommune.TabIndex = 7;
            // 
            // lblVille
            // 
            lblVille.AutoSize = true;
            lblVille.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblVille.ForeColor = Color.FromArgb(64, 64, 64);
            lblVille.Location = new Point(220, 300);
            lblVille.Name = "lblVille";
            lblVille.Size = new Size(43, 23);
            lblVille.TabIndex = 8;
            lblVille.Text = "Ville";
            // 
            // txtVille
            // 
            txtVille.BorderColor = Color.FromArgb(213, 218, 223);
            txtVille.BorderRadius = 14;
            txtVille.DefaultText = "";
            txtVille.FillColor = Color.FromArgb(245, 245, 245);
            txtVille.Font = new Font("Segoe UI", 9F);
            txtVille.ForeColor = Color.FromArgb(68, 88, 112);
            txtVille.Location = new Point(220, 325);
            txtVille.Margin = new Padding(3, 4, 3, 4);
            txtVille.Name = "txtVille";
            txtVille.PasswordChar = '\0';
            txtVille.PlaceholderText = "";
            txtVille.ReadOnly = true;
            txtVille.SelectedText = "";
            txtVille.Size = new Size(180, 36);
            txtVille.TabIndex = 9;
            // 
            // lblProvince
            // 
            lblProvince.AutoSize = true;
            lblProvince.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblProvince.ForeColor = Color.FromArgb(64, 64, 64);
            lblProvince.Location = new Point(420, 300);
            lblProvince.Name = "lblProvince";
            lblProvince.Size = new Size(78, 23);
            lblProvince.TabIndex = 10;
            lblProvince.Text = "Province";
            // 
            // txtProvince
            // 
            txtProvince.BorderColor = Color.FromArgb(213, 218, 223);
            txtProvince.BorderRadius = 14;
            txtProvince.DefaultText = "";
            txtProvince.FillColor = Color.FromArgb(245, 245, 245);
            txtProvince.Font = new Font("Segoe UI", 9F);
            txtProvince.ForeColor = Color.FromArgb(68, 88, 112);
            txtProvince.Location = new Point(420, 325);
            txtProvince.Margin = new Padding(3, 4, 3, 4);
            txtProvince.Name = "txtProvince";
            txtProvince.PasswordChar = '\0';
            txtProvince.PlaceholderText = "";
            txtProvince.ReadOnly = true;
            txtProvince.SelectedText = "";
            txtProvince.Size = new Size(180, 36);
            txtProvince.TabIndex = 11;
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            lblInfo.ForeColor = Color.FromArgb(108, 117, 125);
            lblInfo.Location = new Point(20, 370);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(350, 19);
            lblInfo.TabIndex = 18;
            lblInfo.Text = "Tapez au moins 2 caractères pour lancer la recherche.";
            // 
            // btnManualEntry
            // 
            btnManualEntry.BorderRadius = 10;
            btnManualEntry.FillColor = Color.FromArgb(255, 193, 7);
            btnManualEntry.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnManualEntry.ForeColor = Color.Black;
            btnManualEntry.Location = new Point(20, 410);
            btnManualEntry.Name = "btnManualEntry";
            btnManualEntry.Size = new Size(150, 45);
            btnManualEntry.TabIndex = 14;
            btnManualEntry.Text = "Saisie manuelle";
            btnManualEntry.Click += new EventHandler(btnAjouterAdresse_Click);
            // 
            // btnOK
            // 
            btnOK.BorderRadius = 10;
            btnOK.Enabled = false;
            btnOK.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnOK.ForeColor = Color.White;
            btnOK.Location = new Point(350, 410);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(120, 45);
            btnOK.TabIndex = 12;
            btnOK.Text = "OK";
            btnOK.Click += new EventHandler(btnOK_Click);
            // 
            // btnCancel
            // 
            btnCancel.BorderRadius = 10;
            btnCancel.FillColor = Color.Silver;
            btnCancel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(480, 410);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(120, 45);
            btnCancel.TabIndex = 13;
            btnCancel.Text = "Annuler";
            btnCancel.Click += new EventHandler(btnCancel_Click);
            // 
            // FormAddressSearch
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 242, 245);
            ClientSize = new Size(660, 520);
            Controls.Add(panelMain);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormAddressSearch";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Recherche d'Adresse";
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SiticonePanel panelMain;
        private Label lblTitle;
        private Label lblAvenue;
        private SiticoneTextBox txtAvenue;
        private ListView lstAvenues;
        private Label lblQuartier;
        private SiticoneTextBox txtQuartier;
        private Label lblCommune;
        private SiticoneTextBox txtCommune;
        private Label lblVille;
        private SiticoneTextBox txtVille;
        private Label lblProvince;
        private SiticoneTextBox txtProvince;
        private SiticoneButton btnOK;
        private SiticoneButton btnCancel;
        private SiticoneButton btnManualEntry;
        private Label lblInfo;
    }
}
