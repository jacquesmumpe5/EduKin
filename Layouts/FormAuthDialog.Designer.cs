using Siticone.Desktop.UI.WinForms;

namespace EduKin.Layouts
{
    partial class FormAuthDialog
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Label lblMessage;
        private Label lblUsername;
        private SiticoneTextBox txtUsername;
        private Label lblPassword;
        private SiticoneTextBox txtPassword;
        private SiticoneButton btnAuthenticate;
        private SiticoneButton btnCancel;
        private Label lblError;
        private Panel panelMain;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelMain = new Panel();
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
            panelMain.BackColor = Color.White;
            panelMain.Controls.Add(lblTitle);
            panelMain.Controls.Add(lblMessage);
            panelMain.Controls.Add(lblUsername);
            panelMain.Controls.Add(txtUsername);
            panelMain.Controls.Add(lblPassword);
            panelMain.Controls.Add(txtPassword);
            panelMain.Controls.Add(btnAuthenticate);
            panelMain.Controls.Add(btnCancel);
            panelMain.Controls.Add(lblError);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(0, 0);
            panelMain.Margin = new Padding(3, 4, 3, 4);
            panelMain.Name = "panelMain";
            panelMain.Padding = new Padding(23, 27, 23, 27);
            panelMain.Size = new Size(457, 400);
            panelMain.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(64, 64, 64);
            lblTitle.Location = new Point(23, 27);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(291, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Authentification requise";
            // 
            // lblMessage
            // 
            lblMessage.Location = new Point(23, 73);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(411, 53);
            lblMessage.TabIndex = 1;
            lblMessage.Text = "Seuls les Super Administrateurs peuvent créer des écoles.\nVeuillez vous authentifier :";
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Font = new Font("Segoe UI", 9F);
            lblUsername.ForeColor = Color.FromArgb(64, 64, 64);
            lblUsername.Location = new Point(23, 140);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(130, 20);
            lblUsername.TabIndex = 2;
            lblUsername.Text = "Nom d'utilisateur :";
            // 
            // txtUsername
            // 
            txtUsername.BorderRadius = 14;
            txtUsername.Cursor = Cursors.IBeam;
            txtUsername.DefaultText = "";
            txtUsername.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtUsername.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtUsername.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtUsername.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtUsername.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtUsername.Font = new Font("Segoe UI", 9F);
            txtUsername.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtUsername.Location = new Point(23, 167);
            txtUsername.Margin = new Padding(3, 5, 3, 5);
            txtUsername.Name = "txtUsername";
            txtUsername.PasswordChar = '\0';
            txtUsername.PlaceholderText = "Saisissez votre nom d'utilisateur";
            txtUsername.SelectedText = "";
            txtUsername.Size = new Size(411, 48);
            txtUsername.TabIndex = 3;
            txtUsername.KeyPress += txtUsername_KeyPress;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Font = new Font("Segoe UI", 9F);
            lblPassword.ForeColor = Color.FromArgb(64, 64, 64);
            lblPassword.Location = new Point(23, 233);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(105, 20);
            lblPassword.TabIndex = 4;
            lblPassword.Text = "Mot de passe :";
            // 
            // txtPassword
            // 
            txtPassword.BorderRadius = 14;
            txtPassword.Cursor = Cursors.IBeam;
            txtPassword.DefaultText = "";
            txtPassword.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtPassword.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtPassword.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtPassword.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtPassword.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtPassword.Font = new Font("Segoe UI", 9F);
            txtPassword.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtPassword.Location = new Point(23, 260);
            txtPassword.Margin = new Padding(3, 5, 3, 5);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '●';
            txtPassword.PlaceholderText = "Saisissez votre mot de passe";
            txtPassword.SelectedText = "";
            txtPassword.Size = new Size(411, 48);
            txtPassword.TabIndex = 5;
            txtPassword.KeyPress += txtPassword_KeyPress;
            // 
            // btnAuthenticate
            // 
            btnAuthenticate.BorderRadius = 14;
            btnAuthenticate.DisabledState.BorderColor = Color.DarkGray;
            btnAuthenticate.DisabledState.CustomBorderColor = Color.DarkGray;
            btnAuthenticate.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnAuthenticate.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnAuthenticate.FillColor = Color.FromArgb(0, 122, 204);
            btnAuthenticate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAuthenticate.ForeColor = Color.White;
            btnAuthenticate.Location = new Point(200, 333);
            btnAuthenticate.Margin = new Padding(3, 4, 3, 4);
            btnAuthenticate.Name = "btnAuthenticate";
            btnAuthenticate.Size = new Size(132, 47);
            btnAuthenticate.TabIndex = 6;
            btnAuthenticate.Text = "Authentifier";
            btnAuthenticate.Click += btnAuthenticate_Click;
            // 
            // btnCancel
            // 
            btnCancel.BorderRadius = 14;
            btnCancel.DisabledState.BorderColor = Color.DarkGray;
            btnCancel.DisabledState.CustomBorderColor = Color.DarkGray;
            btnCancel.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnCancel.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnCancel.FillColor = Color.FromArgb(108, 117, 125);
            btnCancel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(343, 333);
            btnCancel.Margin = new Padding(3, 4, 3, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(91, 47);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Annuler";
            btnCancel.Click += btnCancel_Click;
            // 
            // lblError
            // 
            lblError.Font = new Font("Segoe UI", 8.25F);
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(23, 313);
            lblError.Name = "lblError";
            lblError.Size = new Size(411, 16);
            lblError.TabIndex = 8;
            lblError.Visible = false;
            // 
            // FormAuthDialog
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(457, 400);
            Controls.Add(panelMain);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormAuthDialog";
            this.AcceptButton = btnAuthenticate;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Authentification Administrateur";
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            ResumeLayout(false);
        }
    }
}