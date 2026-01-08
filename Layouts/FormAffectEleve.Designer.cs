using System.Windows.Forms;
using Siticone.Desktop.UI.WinForms;

namespace EduKin.Layouts
{
    partial class FormAffectEleve
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
            TxtNbreElevePromotion = new SiticoneTextBox();
            TxtIndicePromotion = new SiticoneTextBox();
            CmbAnneeScolaire = new SiticoneComboBox();
            label5 = new Label();
            label4 = new Label();
            label1 = new Label();
            CmbSection = new SiticoneComboBox();
            label3 = new Label();
            CmbOption = new SiticoneComboBox();
            label2 = new Label();
            CmbPromotion = new SiticoneComboBox();
            lblService = new Label();
            BtnQuitter = new SiticoneButton();
            BtnAffectEleve = new SiticoneButton();
            lblTitle = new Label();
            panelMain.SuspendLayout();
            SuspendLayout();
            // 
            // panelMain
            // 
            panelMain.BackColor = Color.Transparent;
            panelMain.BorderRadius = 20;
            panelMain.Controls.Add(lblInfo);
            panelMain.Controls.Add(TxtNbreElevePromotion);
            panelMain.Controls.Add(TxtIndicePromotion);
            panelMain.Controls.Add(CmbAnneeScolaire);
            panelMain.Controls.Add(label5);
            panelMain.Controls.Add(label4);
            panelMain.Controls.Add(label1);
            panelMain.Controls.Add(CmbSection);
            panelMain.Controls.Add(label3);
            panelMain.Controls.Add(CmbOption);
            panelMain.Controls.Add(label2);
            panelMain.Controls.Add(CmbPromotion);
            panelMain.Controls.Add(lblService);
            panelMain.Controls.Add(BtnQuitter);
            panelMain.Controls.Add(BtnAffectEleve);
            panelMain.Controls.Add(lblTitle);
            panelMain.FillColor = Color.White;
            panelMain.Location = new Point(33, 38);
            panelMain.Name = "panelMain";
            panelMain.ShadowDecoration.BorderRadius = 20;
            panelMain.ShadowDecoration.Enabled = true;
            panelMain.Size = new Size(641, 490);
            panelMain.TabIndex = 0;
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            lblInfo.ForeColor = Color.FromArgb(108, 117, 125);
            lblInfo.Location = new Point(117, 368);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(385, 19);
            lblInfo.TabIndex = 18;
            lblInfo.Text = "Sélectionnez l'année scolaire pour activer les autres champs.";
            // 
            // TxtNbreElevePromotion
            // 
            TxtNbreElevePromotion.BorderColor = Color.FromArgb(5, 21, 48);
            TxtNbreElevePromotion.BorderRadius = 14;
            TxtNbreElevePromotion.DefaultText = "";
            TxtNbreElevePromotion.Font = new Font("Segoe UI", 9F);
            TxtNbreElevePromotion.Location = new Point(354, 308);
            TxtNbreElevePromotion.Margin = new Padding(3, 4, 3, 4);
            TxtNbreElevePromotion.Name = "TxtNbreElevePromotion";
            TxtNbreElevePromotion.PasswordChar = '\0';
            TxtNbreElevePromotion.PlaceholderText = "Ex. 350";
            TxtNbreElevePromotion.SelectedText = "";
            TxtNbreElevePromotion.Size = new Size(259, 36);
            TxtNbreElevePromotion.TabIndex = 17;
            // 
            // TxtIndicePromotion
            // 
            TxtIndicePromotion.BorderColor = Color.FromArgb(5, 21, 48);
            TxtIndicePromotion.BorderRadius = 14;
            TxtIndicePromotion.DefaultText = "";
            TxtIndicePromotion.Font = new Font("Segoe UI", 9F);
            TxtIndicePromotion.Location = new Point(354, 217);
            TxtIndicePromotion.Margin = new Padding(3, 4, 3, 4);
            TxtIndicePromotion.Name = "TxtIndicePromotion";
            TxtIndicePromotion.PasswordChar = '\0';
            TxtIndicePromotion.PlaceholderText = "Ex. 350";
            TxtIndicePromotion.SelectedText = "";
            TxtIndicePromotion.Size = new Size(259, 36);
            TxtIndicePromotion.TabIndex = 17;
            // 
            // CmbAnneeScolaire
            // 
            CmbAnneeScolaire.BackColor = Color.Transparent;
            CmbAnneeScolaire.BorderColor = Color.FromArgb(213, 218, 223);
            CmbAnneeScolaire.BorderRadius = 10;
            CmbAnneeScolaire.DrawMode = DrawMode.OwnerDrawFixed;
            CmbAnneeScolaire.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbAnneeScolaire.FocusedColor = Color.FromArgb(94, 148, 255);
            CmbAnneeScolaire.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            CmbAnneeScolaire.Font = new Font("Segoe UI", 10F);
            CmbAnneeScolaire.ForeColor = Color.FromArgb(68, 88, 112);
            CmbAnneeScolaire.ItemHeight = 30;
            CmbAnneeScolaire.Location = new Point(354, 128);
            CmbAnneeScolaire.Name = "CmbAnneeScolaire";
            CmbAnneeScolaire.Size = new Size(259, 36);
            CmbAnneeScolaire.TabIndex = 16;
            CmbAnneeScolaire.SelectedIndexChanged += CmbAnneeScolaire_SelectedIndexChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label5.ForeColor = Color.FromArgb(64, 64, 64);
            label5.Location = new Point(354, 283);
            label5.Name = "label5";
            label5.Size = new Size(220, 23);
            label5.TabIndex = 15;
            label5.Text = "Effectif dans la Promotion";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label4.ForeColor = Color.FromArgb(64, 64, 64);
            label4.Location = new Point(354, 192);
            label4.Name = "label4";
            label4.Size = new Size(148, 23);
            label4.TabIndex = 15;
            label4.Text = "Indice Promotion";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(64, 64, 64);
            label1.Location = new Point(354, 103);
            label1.Name = "label1";
            label1.Size = new Size(130, 23);
            label1.TabIndex = 15;
            label1.Text = "Année-Scolaire";
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
            CmbSection.Location = new Point(19, 128);
            CmbSection.Name = "CmbSection";
            CmbSection.Size = new Size(259, 36);
            CmbSection.TabIndex = 16;
            CmbSection.SelectedIndexChanged += CmbSection_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(64, 64, 64);
            label3.Location = new Point(19, 103);
            label3.Name = "label3";
            label3.Size = new Size(69, 23);
            label3.TabIndex = 15;
            label3.Text = "Section";
            // 
            // CmbOption
            // 
            CmbOption.BackColor = Color.Transparent;
            CmbOption.BorderColor = Color.FromArgb(213, 218, 223);
            CmbOption.BorderRadius = 10;
            CmbOption.DrawMode = DrawMode.OwnerDrawFixed;
            CmbOption.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbOption.FocusedColor = Color.FromArgb(94, 148, 255);
            CmbOption.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            CmbOption.Font = new Font("Segoe UI", 10F);
            CmbOption.ForeColor = Color.FromArgb(68, 88, 112);
            CmbOption.ItemHeight = 30;
            CmbOption.Location = new Point(19, 217);
            CmbOption.Name = "CmbOption";
            CmbOption.Size = new Size(259, 36);
            CmbOption.TabIndex = 16;
            CmbOption.SelectedIndexChanged += CmbOption_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(64, 64, 64);
            label2.Location = new Point(19, 192);
            label2.Name = "label2";
            label2.Size = new Size(66, 23);
            label2.TabIndex = 15;
            label2.Text = "Option";
            // 
            // CmbPromotion
            // 
            CmbPromotion.BackColor = Color.Transparent;
            CmbPromotion.BorderColor = Color.FromArgb(213, 218, 223);
            CmbPromotion.BorderRadius = 10;
            CmbPromotion.DrawMode = DrawMode.OwnerDrawFixed;
            CmbPromotion.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbPromotion.FocusedColor = Color.FromArgb(94, 148, 255);
            CmbPromotion.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            CmbPromotion.Font = new Font("Segoe UI", 10F);
            CmbPromotion.ForeColor = Color.FromArgb(68, 88, 112);
            CmbPromotion.ItemHeight = 30;
            CmbPromotion.Location = new Point(19, 308);
            CmbPromotion.Name = "CmbPromotion";
            CmbPromotion.Size = new Size(259, 36);
            CmbPromotion.TabIndex = 16;
            CmbPromotion.SelectedIndexChanged += CmbPromotion_SelectedIndexChanged;
            // 
            // lblService
            // 
            lblService.AutoSize = true;
            lblService.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblService.ForeColor = Color.FromArgb(64, 64, 64);
            lblService.Location = new Point(19, 283);
            lblService.Name = "lblService";
            lblService.Size = new Size(95, 23);
            lblService.TabIndex = 15;
            lblService.Text = "Promotion";
            // 
            // BtnQuitter
            // 
            BtnQuitter.BorderRadius = 10;
            BtnQuitter.FillColor = Color.Silver;
            BtnQuitter.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            BtnQuitter.ForeColor = Color.White;
            BtnQuitter.Location = new Point(354, 416);
            BtnQuitter.Name = "BtnQuitter";
            BtnQuitter.Size = new Size(191, 45);
            BtnQuitter.TabIndex = 14;
            BtnQuitter.Text = "Fermer";
            BtnQuitter.Click += BtnQuitter_Click;
            // 
            // BtnAffectEleve
            // 
            BtnAffectEleve.BorderRadius = 10;
            BtnAffectEleve.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            BtnAffectEleve.ForeColor = Color.White;
            BtnAffectEleve.Location = new Point(69, 416);
            BtnAffectEleve.Name = "BtnAffectEleve";
            BtnAffectEleve.Size = new Size(191, 45);
            BtnAffectEleve.TabIndex = 9;
            BtnAffectEleve.Text = "Affecter";
            BtnAffectEleve.Click += BtnAffectEleve_Click;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(94, 148, 255);
            lblTitle.Location = new Point(19, 12);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(610, 45);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Affectation de l'Eleve";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FormAffectEleve
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 242, 245);
            ClientSize = new Size(715, 563);
            Controls.Add(panelMain);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormAffectEleve";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Affectation Agent";
            Load += FormAffectEleve_Load;
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Siticone.Desktop.UI.WinForms.SiticonePanel panelMain;
        private System.Windows.Forms.Label lblTitle;
        private Siticone.Desktop.UI.WinForms.SiticoneButton BtnAffectEleve;
        private SiticoneButton BtnQuitter;
        private SiticoneTextBox TxtIndicePromotion;
        private SiticoneComboBox CmbPromotion;
        private Label lblService;
        private SiticoneComboBox CmbAnneeScolaire;
        private Label label1;
        private SiticoneTextBox TxtNbreElevePromotion;
        private SiticoneComboBox CmbSection;
        private Label label3;
        private SiticoneComboBox CmbOption;
        private Label label2;
        private Label label5;
        private Label label4;
        private Label lblInfo;
    }
}
