using System.Windows.Forms;
using Siticone.Desktop.UI.WinForms;

namespace EduKin.Layouts
{
    partial class FormAffectSection
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
            lblInfo = new Label();
            panelOptions = new SiticonePanel();
            lblNoOptions = new Label();
            CmbSection = new SiticoneComboBox();
            label3 = new Label();
            BtnRefresh = new SiticoneButton();
            BtnFermer = new SiticoneButton();
            BtnAffecterOptions = new SiticoneButton();
            lblTitle = new Label();
            panelMain.SuspendLayout();
            panelOptions.SuspendLayout();
            SuspendLayout();
            // 
            // panelMain
            // 
            panelMain.BackColor = Color.Transparent;
            panelMain.BorderRadius = 20;
            panelMain.Controls.Add(lblInfo);
            panelMain.Controls.Add(panelOptions);
            panelMain.Controls.Add(CmbSection);
            panelMain.Controls.Add(label3);
            panelMain.Controls.Add(BtnRefresh);
            panelMain.Controls.Add(BtnFermer);
            panelMain.Controls.Add(BtnAffecterOptions);
            panelMain.Controls.Add(lblTitle);
            panelMain.FillColor = Color.White;
            panelMain.Location = new Point(33, 38);
            panelMain.Name = "panelMain";
            panelMain.ShadowDecoration.BorderRadius = 20;
            panelMain.ShadowDecoration.Enabled = true;
            panelMain.Size = new Size(946, 650);
            panelMain.TabIndex = 0;
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblInfo.ForeColor = Color.FromArgb(108, 117, 125);
            lblInfo.Location = new Point(30, 550);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(325, 20);
            lblInfo.TabIndex = 18;
            lblInfo.Text = "Sélectionnez une section pour afficher ses options.";
            // 
            // panelOptions
            // 
            panelOptions.AutoScroll = true;
            panelOptions.BackColor = Color.Transparent;
            panelOptions.BorderColor = Color.FromArgb(213, 218, 223);
            panelOptions.BorderRadius = 14;
            panelOptions.BorderThickness = 1;
            panelOptions.Controls.Add(lblNoOptions);
            panelOptions.FillColor = Color.FromArgb(248, 249, 250);
            panelOptions.Location = new Point(30, 170);
            panelOptions.Name = "panelOptions";
            panelOptions.Padding = new Padding(10);
            panelOptions.Size = new Size(873, 360);
            panelOptions.TabIndex = 19;
            // 
            // lblNoOptions
            // 
            lblNoOptions.Font = new Font("Segoe UI", 12F, FontStyle.Italic);
            lblNoOptions.ForeColor = Color.FromArgb(108, 117, 125);
            lblNoOptions.Location = new Point(298, 68);
            lblNoOptions.Name = "lblNoOptions";
            lblNoOptions.Size = new Size(259, 39);
            lblNoOptions.TabIndex = 0;
            lblNoOptions.Text = "Aucune option à afficher";
            lblNoOptions.TextAlign = ContentAlignment.MiddleCenter;
            lblNoOptions.Click += lblNoOptions_Click;
            // 
            // CmbSection
            // 
            CmbSection.BackColor = Color.Transparent;
            CmbSection.BorderColor = Color.FromArgb(213, 218, 223);
            CmbSection.BorderRadius = 10;
            CmbSection.DrawMode = DrawMode.OwnerDrawFixed;
            CmbSection.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbSection.FocusedColor = Color.FromArgb(94, 148, 255);
            CmbSection.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            CmbSection.Font = new Font("Segoe UI", 10F);
            CmbSection.ForeColor = Color.FromArgb(68, 88, 112);
            CmbSection.ItemHeight = 30;
            CmbSection.Location = new Point(30, 110);
            CmbSection.Name = "CmbSection";
            CmbSection.Size = new Size(350, 36);
            CmbSection.TabIndex = 16;
            CmbSection.SelectedIndexChanged += CmbSection_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(64, 64, 64);
            label3.Location = new Point(30, 85);
            label3.Name = "label3";
            label3.Size = new Size(78, 25);
            label3.TabIndex = 15;
            label3.Text = "Section";
            // 
            // BtnRefresh
            // 
            BtnRefresh.BorderRadius = 10;
            BtnRefresh.FillColor = Color.FromArgb(255, 193, 7);
            BtnRefresh.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            BtnRefresh.ForeColor = Color.White;
            BtnRefresh.Location = new Point(420, 110);
            BtnRefresh.Name = "BtnRefresh";
            BtnRefresh.Size = new Size(120, 36);
            BtnRefresh.TabIndex = 14;
            BtnRefresh.Text = "Actualiser";
            BtnRefresh.Click += BtnRefresh_Click;
            // 
            // BtnFermer
            // 
            BtnFermer.BorderRadius = 10;
            BtnFermer.FillColor = Color.Silver;
            BtnFermer.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            BtnFermer.ForeColor = Color.White;
            BtnFermer.Location = new Point(580, 580);
            BtnFermer.Name = "BtnFermer";
            BtnFermer.Size = new Size(191, 45);
            BtnFermer.TabIndex = 14;
            BtnFermer.Text = "Fermer";
            BtnFermer.Click += BtnFermer_Click;
            // 
            // BtnAffecterOptions
            // 
            BtnAffecterOptions.BorderRadius = 10;
            BtnAffecterOptions.Enabled = false;
            BtnAffecterOptions.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            BtnAffecterOptions.ForeColor = Color.White;
            BtnAffecterOptions.Location = new Point(224, 580);
            BtnAffecterOptions.Name = "BtnAffecterOptions";
            BtnAffecterOptions.Size = new Size(191, 45);
            BtnAffecterOptions.TabIndex = 9;
            BtnAffecterOptions.Text = "Affecter Options";
            BtnAffecterOptions.Click += BtnAffecterOptions_Click;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(94, 148, 255);
            lblTitle.Location = new Point(30, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(740, 45);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Affectation des Options aux Sections";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FormAffectSection
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 242, 245);
            ClientSize = new Size(1008, 726);
            Controls.Add(panelMain);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormAffectSection";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Affectation Options Section";
            Load += FormAffectSection_Load;
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            panelOptions.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SiticonePanel panelMain;
        private Label lblTitle;
        private SiticoneButton BtnAffecterOptions;
        private SiticoneButton BtnFermer;
        private SiticoneButton BtnRefresh;
        private SiticoneComboBox CmbSection;
        private Label label3;
        private SiticonePanel panelOptions;
        private Label lblNoOptions;
        private Label lblInfo;
    }
}