using System.Windows.Forms;
using Siticone.Desktop.UI.WinForms;

namespace EduKin.Layouts
{
    partial class FormNewAddress
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
            lblProvince = new Label();
            cmbProvince = new SiticoneComboBox();
            lblVille = new Label();
            cmbVille = new SiticoneComboBox();
            lblCommune = new Label();
            cmbCommune = new SiticoneComboBox();
            lblQuartier = new Label();
            cmbQuartier = new SiticoneComboBox();
            lblInfo = new Label();
            btnSave = new SiticoneButton();
            btnCancel = new SiticoneButton();
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
            panelMain.Controls.Add(lblProvince);
            panelMain.Controls.Add(cmbProvince);
            panelMain.Controls.Add(lblVille);
            panelMain.Controls.Add(cmbVille);
            panelMain.Controls.Add(lblCommune);
            panelMain.Controls.Add(cmbCommune);
            panelMain.Controls.Add(lblQuartier);
            panelMain.Controls.Add(cmbQuartier);
            panelMain.Controls.Add(lblInfo);
            panelMain.Controls.Add(btnSave);
            panelMain.Controls.Add(btnCancel);
            panelMain.FillColor = Color.White;
            panelMain.Location = new Point(30, 30);
            panelMain.Name = "panelMain";
            panelMain.ShadowDecoration.BorderRadius = 20;
            panelMain.ShadowDecoration.Enabled = true;
            panelMain.Size = new Size(580, 450);
            panelMain.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(94, 148, 255);
            lblTitle.Location = new Point(20, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(540, 45);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Ajouter une nouvelle adresse";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblAvenue
            // 
            lblAvenue.AutoSize = true;
            lblAvenue.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblAvenue.ForeColor = Color.FromArgb(64, 64, 64);
            lblAvenue.Location = new Point(30, 75);
            lblAvenue.Name = "lblAvenue";
            lblAvenue.Size = new Size(69, 23);
            lblAvenue.TabIndex = 1;
            lblAvenue.Text = "Avenue";
            // 
            // txtAvenue
            // 
            txtAvenue.BorderColor = Color.FromArgb(5, 21, 48);
            txtAvenue.BorderRadius = 14;
            txtAvenue.DefaultText = "";
            txtAvenue.Font = new Font("Segoe UI", 9F);
            txtAvenue.Location = new Point(30, 100);
            txtAvenue.Margin = new Padding(3, 4, 3, 4);
            txtAvenue.Name = "txtAvenue";
            txtAvenue.PasswordChar = '\0';
            txtAvenue.PlaceholderText = "Nom de l'avenue";
            txtAvenue.SelectedText = "";
            txtAvenue.Size = new Size(520, 36);
            txtAvenue.TabIndex = 2;
            // 
            // lblProvince
            // 
            lblProvince.AutoSize = true;
            lblProvince.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblProvince.ForeColor = Color.FromArgb(64, 64, 64);
            lblProvince.Location = new Point(30, 150);
            lblProvince.Name = "lblProvince";
            lblProvince.Size = new Size(78, 23);
            lblProvince.TabIndex = 3;
            lblProvince.Text = "Province";
            // 
            // cmbProvince
            // 
            cmbProvince.BackColor = Color.Transparent;
            cmbProvince.BorderColor = Color.FromArgb(213, 218, 223);
            cmbProvince.BorderRadius = 10;
            cmbProvince.DrawMode = DrawMode.OwnerDrawFixed;
            cmbProvince.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProvince.FocusedColor = Color.FromArgb(94, 148, 255);
            cmbProvince.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cmbProvince.Font = new Font("Segoe UI", 10F);
            cmbProvince.ForeColor = Color.FromArgb(68, 88, 112);
            cmbProvince.ItemHeight = 30;
            cmbProvince.Location = new Point(30, 175);
            cmbProvince.Name = "cmbProvince";
            cmbProvince.Size = new Size(250, 36);
            cmbProvince.TabIndex = 4;
            cmbProvince.SelectedIndexChanged += cmbProvince_SelectedIndexChanged;
            // 
            // lblVille
            // 
            lblVille.AutoSize = true;
            lblVille.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblVille.ForeColor = Color.FromArgb(64, 64, 64);
            lblVille.Location = new Point(300, 150);
            lblVille.Name = "lblVille";
            lblVille.Size = new Size(45, 23);
            lblVille.TabIndex = 5;
            lblVille.Text = "Ville";
            // 
            // cmbVille
            // 
            cmbVille.BackColor = Color.Transparent;
            cmbVille.BorderColor = Color.FromArgb(213, 218, 223);
            cmbVille.BorderRadius = 10;
            cmbVille.DrawMode = DrawMode.OwnerDrawFixed;
            cmbVille.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbVille.FocusedColor = Color.FromArgb(94, 148, 255);
            cmbVille.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cmbVille.Font = new Font("Segoe UI", 10F);
            cmbVille.ForeColor = Color.FromArgb(68, 88, 112);
            cmbVille.ItemHeight = 30;
            cmbVille.Location = new Point(300, 175);
            cmbVille.Name = "cmbVille";
            cmbVille.Size = new Size(250, 36);
            cmbVille.TabIndex = 6;
            cmbVille.SelectedIndexChanged += cmbVille_SelectedIndexChanged;
            // 
            // lblCommune
            // 
            lblCommune.AutoSize = true;
            lblCommune.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCommune.ForeColor = Color.FromArgb(64, 64, 64);
            lblCommune.Location = new Point(30, 225);
            lblCommune.Name = "lblCommune";
            lblCommune.Size = new Size(92, 23);
            lblCommune.TabIndex = 7;
            lblCommune.Text = "Commune";
            // 
            // cmbCommune
            // 
            cmbCommune.BackColor = Color.Transparent;
            cmbCommune.BorderColor = Color.FromArgb(213, 218, 223);
            cmbCommune.BorderRadius = 10;
            cmbCommune.DrawMode = DrawMode.OwnerDrawFixed;
            cmbCommune.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCommune.FocusedColor = Color.FromArgb(94, 148, 255);
            cmbCommune.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cmbCommune.Font = new Font("Segoe UI", 10F);
            cmbCommune.ForeColor = Color.FromArgb(68, 88, 112);
            cmbCommune.ItemHeight = 30;
            cmbCommune.Location = new Point(30, 250);
            cmbCommune.Name = "cmbCommune";
            cmbCommune.Size = new Size(250, 36);
            cmbCommune.TabIndex = 8;
            cmbCommune.SelectedIndexChanged += cmbCommune_SelectedIndexChanged;
            // 
            // lblQuartier
            // 
            lblQuartier.AutoSize = true;
            lblQuartier.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblQuartier.ForeColor = Color.FromArgb(64, 64, 64);
            lblQuartier.Location = new Point(300, 225);
            lblQuartier.Name = "lblQuartier";
            lblQuartier.Size = new Size(77, 23);
            lblQuartier.TabIndex = 9;
            lblQuartier.Text = "Quartier";
            // 
            // cmbQuartier
            // 
            cmbQuartier.BackColor = Color.Transparent;
            cmbQuartier.BorderColor = Color.FromArgb(213, 218, 223);
            cmbQuartier.BorderRadius = 10;
            cmbQuartier.DrawMode = DrawMode.OwnerDrawFixed;
            cmbQuartier.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbQuartier.FocusedColor = Color.FromArgb(94, 148, 255);
            cmbQuartier.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cmbQuartier.Font = new Font("Segoe UI", 10F);
            cmbQuartier.ForeColor = Color.FromArgb(68, 88, 112);
            cmbQuartier.ItemHeight = 30;
            cmbQuartier.Location = new Point(300, 250);
            cmbQuartier.Name = "cmbQuartier";
            cmbQuartier.Size = new Size(250, 36);
            cmbQuartier.TabIndex = 10;
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            lblInfo.ForeColor = Color.FromArgb(108, 117, 125);
            lblInfo.Location = new Point(30, 305);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(618, 19);
            lblInfo.TabIndex = 18;
            lblInfo.Text = "Les champs Province, Ville, Commune et Quartier sont liés et se mettent a jour automatiquement.";
            // 
            // btnSave
            // 
            btnSave.BorderRadius = 10;
            btnSave.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(126, 370);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(130, 45);
            btnSave.TabIndex = 11;
            btnSave.Text = "Enregistrer";
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.BorderRadius = 10;
            btnCancel.FillColor = Color.Silver;
            btnCancel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(355, 370);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(110, 45);
            btnCancel.TabIndex = 12;
            btnCancel.Text = "Annuler";
            btnCancel.Click += btnCancel_Click;
            // 
            // FormNewAddress
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 242, 245);
            ClientSize = new Size(640, 510);
            Controls.Add(panelMain);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormNewAddress";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Ajouter une adresse";
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SiticonePanel panelMain;
        private Label lblTitle;
        private Label lblAvenue;
        private SiticoneTextBox txtAvenue;
        private Label lblProvince;
        private SiticoneComboBox cmbProvince;
        private Label lblVille;
        private SiticoneComboBox cmbVille;
        private Label lblCommune;
        private SiticoneComboBox cmbCommune;
        private Label lblQuartier;
        private SiticoneComboBox cmbQuartier;
        private Label lblInfo;
        private SiticoneButton btnSave;
        private SiticoneButton btnCancel;
    }
}
