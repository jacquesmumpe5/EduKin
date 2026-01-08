using System.Windows.Forms;
using Siticone.Desktop.UI.WinForms;

namespace EduKin.Layouts
{
    partial class FormAffectAgent
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
            TxtSalaireNetAgent = new SiticoneTextBox();
            TxtCNSSAgent = new SiticoneTextBox();
            TxtSalaireBrutAgent = new SiticoneTextBox();
            TxtIPRAgent = new SiticoneTextBox();
            TxtPrimeAgent = new SiticoneTextBox();
            TxtSalaireBase = new SiticoneTextBox();
            CmbRoleAgent = new SiticoneComboBox();
            label5 = new Label();
            label4 = new Label();
            lblRole = new Label();
            label6 = new Label();
            CmbGradeAgent = new SiticoneComboBox();
            label3 = new Label();
            lblGrade = new Label();
            CmbFonctionAgent = new SiticoneComboBox();
            label2 = new Label();
            lblFonction = new Label();
            CmbServiceAgent = new SiticoneComboBox();
            label1 = new Label();
            lblService = new Label();
            btnCancel = new SiticoneButton();
            btnSave = new SiticoneButton();
            lblTitle = new Label();
            panelMain.SuspendLayout();
            SuspendLayout();
            // 
            // panelMain
            // 
            panelMain.BackColor = Color.Transparent;
            panelMain.BorderRadius = 20;
            panelMain.Controls.Add(TxtSalaireNetAgent);
            panelMain.Controls.Add(TxtCNSSAgent);
            panelMain.Controls.Add(TxtSalaireBrutAgent);
            panelMain.Controls.Add(TxtIPRAgent);
            panelMain.Controls.Add(TxtPrimeAgent);
            panelMain.Controls.Add(TxtSalaireBase);
            panelMain.Controls.Add(CmbRoleAgent);
            panelMain.Controls.Add(label5);
            panelMain.Controls.Add(label4);
            panelMain.Controls.Add(lblRole);
            panelMain.Controls.Add(label6);
            panelMain.Controls.Add(CmbGradeAgent);
            panelMain.Controls.Add(label3);
            panelMain.Controls.Add(lblGrade);
            panelMain.Controls.Add(CmbFonctionAgent);
            panelMain.Controls.Add(label2);
            panelMain.Controls.Add(lblFonction);
            panelMain.Controls.Add(CmbServiceAgent);
            panelMain.Controls.Add(label1);
            panelMain.Controls.Add(lblService);
            panelMain.Controls.Add(btnCancel);
            panelMain.Controls.Add(btnSave);
            panelMain.Controls.Add(lblTitle);
            panelMain.FillColor = Color.White;
            panelMain.Location = new Point(40, 40);
            panelMain.Name = "panelMain";
            panelMain.ShadowDecoration.BorderRadius = 20;
            panelMain.ShadowDecoration.Enabled = true;
            panelMain.Size = new Size(650, 620);
            panelMain.TabIndex = 0;
            // 
            // TxtSalaireNetAgent
            // 
            TxtSalaireNetAgent.BorderColor = Color.FromArgb(5, 21, 48);
            TxtSalaireNetAgent.BorderRadius = 14;
            TxtSalaireNetAgent.DefaultText = "";
            TxtSalaireNetAgent.Font = new Font("Segoe UI", 9F);
            TxtSalaireNetAgent.Location = new Point(334, 444);
            TxtSalaireNetAgent.Margin = new Padding(3, 4, 3, 4);
            TxtSalaireNetAgent.Name = "TxtSalaireNetAgent";
            TxtSalaireNetAgent.PasswordChar = '\0';
            TxtSalaireNetAgent.PlaceholderText = "Ex. 350";
            TxtSalaireNetAgent.ReadOnly = true;
            TxtSalaireNetAgent.SelectedText = "";
            TxtSalaireNetAgent.Size = new Size(241, 36);
            TxtSalaireNetAgent.TabIndex = 11;
            // 
            // TxtCNSSAgent
            // 
            TxtCNSSAgent.BorderColor = Color.FromArgb(5, 21, 48);
            TxtCNSSAgent.BorderRadius = 14;
            TxtCNSSAgent.DefaultText = "";
            TxtCNSSAgent.Font = new Font("Segoe UI", 9F);
            TxtCNSSAgent.Location = new Point(334, 357);
            TxtCNSSAgent.Margin = new Padding(3, 4, 3, 4);
            TxtCNSSAgent.Name = "TxtCNSSAgent";
            TxtCNSSAgent.PasswordChar = '\0';
            TxtCNSSAgent.PlaceholderText = "Ex. 350";
            TxtCNSSAgent.SelectedText = "";
            TxtCNSSAgent.Size = new Size(241, 36);
            TxtCNSSAgent.TabIndex = 11;
            // 
            // TxtSalaireBrutAgent
            // 
            TxtSalaireBrutAgent.BorderColor = Color.FromArgb(5, 21, 48);
            TxtSalaireBrutAgent.BorderRadius = 14;
            TxtSalaireBrutAgent.DefaultText = "";
            TxtSalaireBrutAgent.Font = new Font("Segoe UI", 9F);
            TxtSalaireBrutAgent.Location = new Point(334, 195);
            TxtSalaireBrutAgent.Margin = new Padding(3, 4, 3, 4);
            TxtSalaireBrutAgent.Name = "TxtSalaireBrutAgent";
            TxtSalaireBrutAgent.PasswordChar = '\0';
            TxtSalaireBrutAgent.PlaceholderText = "Ex. 350";
            TxtSalaireBrutAgent.ReadOnly = true;
            TxtSalaireBrutAgent.SelectedText = "";
            TxtSalaireBrutAgent.Size = new Size(241, 36);
            TxtSalaireBrutAgent.TabIndex = 11;
            // 
            // TxtIPRAgent
            // 
            TxtIPRAgent.BorderColor = Color.FromArgb(5, 21, 48);
            TxtIPRAgent.BorderRadius = 14;
            TxtIPRAgent.DefaultText = "";
            TxtIPRAgent.Font = new Font("Segoe UI", 9F);
            TxtIPRAgent.Location = new Point(334, 277);
            TxtIPRAgent.Margin = new Padding(3, 4, 3, 4);
            TxtIPRAgent.Name = "TxtIPRAgent";
            TxtIPRAgent.PasswordChar = '\0';
            TxtIPRAgent.PlaceholderText = "Ex. 350";
            TxtIPRAgent.SelectedText = "";
            TxtIPRAgent.Size = new Size(241, 36);
            TxtIPRAgent.TabIndex = 11;
            // 
            // TxtPrimeAgent
            // 
            TxtPrimeAgent.BorderColor = Color.FromArgb(5, 21, 48);
            TxtPrimeAgent.BorderRadius = 14;
            TxtPrimeAgent.DefaultText = "";
            TxtPrimeAgent.Font = new Font("Segoe UI", 9F);
            TxtPrimeAgent.Location = new Point(334, 115);
            TxtPrimeAgent.Margin = new Padding(3, 4, 3, 4);
            TxtPrimeAgent.Name = "TxtPrimeAgent";
            TxtPrimeAgent.PasswordChar = '\0';
            TxtPrimeAgent.PlaceholderText = "Ex. 350";
            TxtPrimeAgent.SelectedText = "";
            TxtPrimeAgent.Size = new Size(241, 36);
            TxtPrimeAgent.TabIndex = 11;
            // 
            // TxtSalaireBase
            // 
            TxtSalaireBase.BorderColor = Color.FromArgb(5, 21, 48);
            TxtSalaireBase.BorderRadius = 14;
            TxtSalaireBase.DefaultText = "";
            TxtSalaireBase.Font = new Font("Segoe UI", 9F);
            TxtSalaireBase.Location = new Point(17, 444);
            TxtSalaireBase.Margin = new Padding(3, 4, 3, 4);
            TxtSalaireBase.Name = "TxtSalaireBase";
            TxtSalaireBase.PasswordChar = '\0';
            TxtSalaireBase.PlaceholderText = "Ex. 350";
            TxtSalaireBase.SelectedText = "";
            TxtSalaireBase.Size = new Size(259, 36);
            TxtSalaireBase.TabIndex = 11;
            // 
            // CmbRoleAgent
            // 
            CmbRoleAgent.BackColor = Color.Transparent;
            CmbRoleAgent.BorderColor = Color.FromArgb(213, 218, 223);
            CmbRoleAgent.BorderRadius = 10;
            CmbRoleAgent.DrawMode = DrawMode.OwnerDrawFixed;
            CmbRoleAgent.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbRoleAgent.FocusedColor = Color.FromArgb(94, 148, 255);
            CmbRoleAgent.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            CmbRoleAgent.Font = new Font("Segoe UI", 10F);
            CmbRoleAgent.ForeColor = Color.FromArgb(68, 88, 112);
            CmbRoleAgent.ItemHeight = 30;
            CmbRoleAgent.Location = new Point(17, 355);
            CmbRoleAgent.Name = "CmbRoleAgent";
            CmbRoleAgent.Size = new Size(259, 36);
            CmbRoleAgent.TabIndex = 8;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label5.ForeColor = Color.FromArgb(64, 64, 64);
            label5.Location = new Point(334, 406);
            label5.Name = "label5";
            label5.Size = new Size(98, 23);
            label5.TabIndex = 7;
            label5.Text = "Salaire Net";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label4.ForeColor = Color.FromArgb(64, 64, 64);
            label4.Location = new Point(334, 330);
            label4.Name = "label4";
            label4.Size = new Size(54, 23);
            label4.TabIndex = 7;
            label4.Text = "CNSS";
            // 
            // lblRole
            // 
            lblRole.AutoSize = true;
            lblRole.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblRole.ForeColor = Color.FromArgb(64, 64, 64);
            lblRole.Location = new Point(17, 330);
            lblRole.Name = "lblRole";
            lblRole.Size = new Size(50, 23);
            lblRole.TabIndex = 7;
            lblRole.Text = "Rôle:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label6.ForeColor = Color.FromArgb(64, 64, 64);
            label6.Location = new Point(334, 168);
            label6.Name = "label6";
            label6.Size = new Size(104, 23);
            label6.TabIndex = 5;
            label6.Text = "Salaire Brut";
            // 
            // CmbGradeAgent
            // 
            CmbGradeAgent.BackColor = Color.Transparent;
            CmbGradeAgent.BorderColor = Color.FromArgb(213, 218, 223);
            CmbGradeAgent.BorderRadius = 10;
            CmbGradeAgent.DrawMode = DrawMode.OwnerDrawFixed;
            CmbGradeAgent.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbGradeAgent.FocusedColor = Color.FromArgb(94, 148, 255);
            CmbGradeAgent.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            CmbGradeAgent.Font = new Font("Segoe UI", 10F);
            CmbGradeAgent.ForeColor = Color.FromArgb(68, 88, 112);
            CmbGradeAgent.ItemHeight = 30;
            CmbGradeAgent.Location = new Point(17, 275);
            CmbGradeAgent.Name = "CmbGradeAgent";
            CmbGradeAgent.Size = new Size(259, 36);
            CmbGradeAgent.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(64, 64, 64);
            label3.Location = new Point(334, 250);
            label3.Name = "label3";
            label3.Size = new Size(36, 23);
            label3.TabIndex = 5;
            label3.Text = "IPR";
            // 
            // lblGrade
            // 
            lblGrade.AutoSize = true;
            lblGrade.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblGrade.ForeColor = Color.FromArgb(64, 64, 64);
            lblGrade.Location = new Point(17, 250);
            lblGrade.Name = "lblGrade";
            lblGrade.Size = new Size(63, 23);
            lblGrade.TabIndex = 5;
            lblGrade.Text = "Grade:";
            // 
            // CmbFonctionAgent
            // 
            CmbFonctionAgent.BackColor = Color.Transparent;
            CmbFonctionAgent.BorderColor = Color.FromArgb(213, 218, 223);
            CmbFonctionAgent.BorderRadius = 10;
            CmbFonctionAgent.DrawMode = DrawMode.OwnerDrawFixed;
            CmbFonctionAgent.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbFonctionAgent.FocusedColor = Color.FromArgb(94, 148, 255);
            CmbFonctionAgent.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            CmbFonctionAgent.Font = new Font("Segoe UI", 10F);
            CmbFonctionAgent.ForeColor = Color.FromArgb(68, 88, 112);
            CmbFonctionAgent.ItemHeight = 30;
            CmbFonctionAgent.Location = new Point(17, 195);
            CmbFonctionAgent.Name = "CmbFonctionAgent";
            CmbFonctionAgent.Size = new Size(259, 36);
            CmbFonctionAgent.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(64, 64, 64);
            label2.Location = new Point(334, 88);
            label2.Name = "label2";
            label2.Size = new Size(64, 23);
            label2.TabIndex = 3;
            label2.Text = "Primes";
            // 
            // lblFonction
            // 
            lblFonction.AutoSize = true;
            lblFonction.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblFonction.ForeColor = Color.FromArgb(64, 64, 64);
            lblFonction.Location = new Point(17, 170);
            lblFonction.Name = "lblFonction";
            lblFonction.Size = new Size(84, 23);
            lblFonction.TabIndex = 3;
            lblFonction.Text = "Fonction:";
            // 
            // CmbServiceAgent
            // 
            CmbServiceAgent.BackColor = Color.Transparent;
            CmbServiceAgent.BorderColor = Color.FromArgb(213, 218, 223);
            CmbServiceAgent.BorderRadius = 10;
            CmbServiceAgent.DrawMode = DrawMode.OwnerDrawFixed;
            CmbServiceAgent.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbServiceAgent.FocusedColor = Color.FromArgb(94, 148, 255);
            CmbServiceAgent.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            CmbServiceAgent.Font = new Font("Segoe UI", 10F);
            CmbServiceAgent.ForeColor = Color.FromArgb(68, 88, 112);
            CmbServiceAgent.ItemHeight = 30;
            CmbServiceAgent.Location = new Point(17, 115);
            CmbServiceAgent.Name = "CmbServiceAgent";
            CmbServiceAgent.Size = new Size(259, 36);
            CmbServiceAgent.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(64, 64, 64);
            label1.Location = new Point(17, 417);
            label1.Name = "label1";
            label1.Size = new Size(135, 23);
            label1.TabIndex = 1;
            label1.Text = "Salaire de  base";
            // 
            // lblService
            // 
            lblService.AutoSize = true;
            lblService.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblService.ForeColor = Color.FromArgb(64, 64, 64);
            lblService.Location = new Point(17, 90);
            lblService.Name = "lblService";
            lblService.Size = new Size(73, 23);
            lblService.TabIndex = 1;
            lblService.Text = "Service:";
            // 
            // btnCancel
            // 
            btnCancel.BorderRadius = 10;
            btnCancel.FillColor = Color.FromArgb(224, 224, 224);
            btnCancel.Font = new Font("Segoe UI", 11F);
            btnCancel.ForeColor = Color.FromArgb(64, 64, 64);
            btnCancel.Location = new Point(368, 514);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(191, 40);
            btnCancel.TabIndex = 10;
            btnCancel.Text = "Annuler";
            // 
            // btnSave
            // 
            btnSave.BorderRadius = 10;
            btnSave.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(53, 509);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(191, 45);
            btnSave.TabIndex = 9;
            btnSave.Text = "Enregistrer";
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(94, 148, 255);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(610, 45);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Affectation de l'agent";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FormAffectAgent
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 242, 245);
            ClientSize = new Size(733, 700);
            Controls.Add(panelMain);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormAffectAgent";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Affectation Agent";
            Load += FormAffectAgent_Load;
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Siticone.Desktop.UI.WinForms.SiticonePanel panelMain;
        private System.Windows.Forms.Label lblTitle;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnSave;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnCancel;
        
        private Siticone.Desktop.UI.WinForms.SiticoneComboBox CmbServiceAgent;
        private System.Windows.Forms.Label lblService;
        
        private Siticone.Desktop.UI.WinForms.SiticoneComboBox CmbFonctionAgent;
        private System.Windows.Forms.Label lblFonction;
        
        private Siticone.Desktop.UI.WinForms.SiticoneComboBox CmbGradeAgent;
        private System.Windows.Forms.Label lblGrade;
        
        private Siticone.Desktop.UI.WinForms.SiticoneComboBox CmbRoleAgent;
        private System.Windows.Forms.Label lblRole;
        private Label label3;
        private Label label2;
        private Label label1;
        private Label label5;
        private Label label4;
        private SiticoneTextBox TxtSalaireNetAgent;
        private SiticoneTextBox TxtCNSSAgent;
        private SiticoneTextBox TxtIPRAgent;
        private SiticoneTextBox TxtPrimeAgent;
        private SiticoneTextBox TxtSalaireBase;
        private SiticoneTextBox TxtSalaireBrutAgent;
        private Label label6;
    }
}
