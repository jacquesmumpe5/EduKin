using System.Windows.Forms;
using Siticone.Desktop.UI.WinForms;

namespace EduKin.Layouts
{
    partial class FormAuthDialog
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
            lblMessage = new Label();
            lblUsername = new Label();
            txtUsername = new SiticoneTextBox();
            lblPassword = new Label();
            txtPassword = new SiticoneTextBox();
            btnAuthenticate = new SiticoneButton();
            btnCancel = new SiticoneButton();
            lblError = new Label();
            panelMain.SuspendLayout();
            SuspendLayout();
            // 
            // panelMain
            // 
            panelMain.BackColor = Color.Transparent;
            panelMain.BorderRadius = 20;
            panelMain.Controls.Add(lblTitle);
            panelMain.Controls.Add(lblMessage);
            panelMain.Controls.Add(lblUsername);
            panelMain.Controls.Add(txtUsername);
            panelMain.Controls.Add(lblPassword);
            panelMain.Controls.Add(txtPassword);
            panelMain.Controls.Add(btnAuthenticate);
            panelMain.Controls.Add(btnCancel);
            panelMain.Controls.Add(lblError);
            panelMain.FillColor = Color.White;
            panelMain.Location = new Point(25, 25);
            panelMain.Name = "panelMain";
            panelMain.ShadowDecoration.BorderRadius = 20;
            panelMain.ShadowDecoration.Enabled = true;
            panelMain.Size = new Size(450, 400);
            panelMain.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(94, 148, 255);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(410, 45);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Authentification requise";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblMessage
            // 
            lblMessage.Font = new Font("Segoe UI", 9F);
            lblMessage.ForeColor = Color.FromArgb(64, 64, 64);
            lblMessage.Location = new Point(25, 70);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(400, 45);
            lblMessage.TabIndex = 1;
            lblMessage.Text = "Seuls les Super Administrateurs peuvent effectuer cette action.\nVeuillez vous authentifier :";
            lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUsername.ForeColor = Color.FromArgb(64, 64, 64);
            lblUsername.Location = new Point(25, 130);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(144, 23);
            lblUsername.TabIndex = 2;
            lblUsername.Text = "Nom d'utilisateur";
            // 
            // txtUsername
            // 
            txtUsername.BorderColor = Color.FromArgb(5, 21, 48);
            txtUsername.BorderRadius = 14;
            txtUsername.Cursor = Cursors.IBeam;
            txtUsername.DefaultText = "";
            txtUsername.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtUsername.Font = new Font("Segoe UI", 9F);
            txtUsername.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtUsername.Location = new Point(25, 155);
            txtUsername.Margin = new Padding(3, 4, 3, 4);
            txtUsername.Name = "txtUsername";
            txtUsername.PasswordChar = '\0';
            txtUsername.PlaceholderText = "Saisissez votre nom d'utilisateur";
            txtUsername.SelectedText = "";
            txtUsername.Size = new Size(400, 36);
            txtUsername.TabIndex = 3;
            txtUsername.KeyPress += txtUsername_KeyPress;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPassword.ForeColor = Color.FromArgb(64, 64, 64);
            lblPassword.Location = new Point(25, 205);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(114, 23);
            lblPassword.TabIndex = 4;
            lblPassword.Text = "Mot de passe";
            // 
            // txtPassword
            // 
            txtPassword.BorderColor = Color.FromArgb(5, 21, 48);
            txtPassword.BorderRadius = 14;
            txtPassword.Cursor = Cursors.IBeam;
            txtPassword.DefaultText = "";
            txtPassword.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtPassword.Font = new Font("Segoe UI", 9F);
            txtPassword.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtPassword.Location = new Point(25, 230);
            txtPassword.Margin = new Padding(3, 4, 3, 4);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '\u25CF';
            txtPassword.PlaceholderText = "Saisissez votre mot de passe";
            txtPassword.SelectedText = "";
            txtPassword.Size = new Size(400, 36);
            txtPassword.TabIndex = 5;
            txtPassword.KeyPress += txtPassword_KeyPress;
            // 
            // lblError
            // 
            lblError.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(25, 280);
            lblError.Name = "lblError";
            lblError.Size = new Size(400, 20);
            lblError.TabIndex = 8;
            lblError.Visible = false;
            // 
            // btnAuthenticate
            // 
            btnAuthenticate.BorderRadius = 10;
            btnAuthenticate.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnAuthenticate.ForeColor = Color.White;
            btnAuthenticate.Location = new Point(150, 320);
            btnAuthenticate.Name = "btnAuthenticate";
            btnAuthenticate.Size = new Size(140, 45);
            btnAuthenticate.TabIndex = 6;
            btnAuthenticate.Text = "Authentifier";
            btnAuthenticate.Click += btnAuthenticate_Click;
            // 
            // btnCancel
            // 
            btnCancel.BorderRadius = 10;
            btnCancel.FillColor = Color.Silver;
            btnCancel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(300, 320);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(120, 45);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Annuler";
            btnCancel.Click += btnCancel_Click;
            // 
            // FormAuthDialog
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 242, 245);
            ClientSize = new Size(500, 450);
            Controls.Add(panelMain);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormAuthDialog";
            AcceptButton = btnAuthenticate;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Authentification Administrateur";
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SiticonePanel panelMain;
        private Label lblTitle;
        private Label lblMessage;
        private Label lblUsername;
        private SiticoneTextBox txtUsername;
        private Label lblPassword;
        private SiticoneTextBox txtPassword;
        private SiticoneButton btnAuthenticate;
        private SiticoneButton btnCancel;
        private Label lblError;
    }
}
