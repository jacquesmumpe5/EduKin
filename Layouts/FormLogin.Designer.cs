namespace EduKin.Layouts
{
    partial class FormLogin
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
            panelMain = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            panelRight = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            lnkReconfigure = new LinkLabel();
            btnCancel = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            btnLogin = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            lblError = new Label();
            txtPassword = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            lblPassword = new Label();
            txtUsername = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            lblUsername = new Label();
            lblEcoleInfo = new Label();
            lblTitle = new Label();
            panelLeft = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            picLogoEcole = new Siticone.Desktop.UI.WinForms.SiticonePictureBox();
            lblEcoleName = new Label();
            lblWelcome = new Label();
            panelMain.SuspendLayout();
            panelRight.SuspendLayout();
            panelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogoEcole).BeginInit();
            SuspendLayout();
            // 
            // panelMain
            // 
            panelMain.BackColor = Color.Transparent;
            panelMain.BorderRadius = 20;
            panelMain.Controls.Add(panelRight);
            panelMain.Controls.Add(panelLeft);
            panelMain.FillColor = Color.White;
            panelMain.Location = new Point(40, 40);
            panelMain.Name = "panelMain";
            panelMain.ShadowDecoration.BorderRadius = 20;
            panelMain.ShadowDecoration.Enabled = true;
            panelMain.Size = new Size(920, 620);
            panelMain.TabIndex = 0;
            // 
            // panelRight
            // 
            panelRight.BackColor = Color.Transparent;
            panelRight.Controls.Add(lnkReconfigure);
            panelRight.Controls.Add(btnCancel);
            panelRight.Controls.Add(btnLogin);
            panelRight.Controls.Add(lblError);
            panelRight.Controls.Add(txtPassword);
            panelRight.Controls.Add(lblPassword);
            panelRight.Controls.Add(txtUsername);
            panelRight.Controls.Add(lblUsername);
            panelRight.Controls.Add(lblEcoleInfo);
            panelRight.Controls.Add(lblTitle);
            panelRight.Location = new Point(400, 0);
            panelRight.Name = "panelRight";
            panelRight.Size = new Size(520, 620);
            panelRight.TabIndex = 1;
            // 
            // lnkReconfigure
            // 
            lnkReconfigure.Font = new Font("Segoe UI", 9F);
            lnkReconfigure.LinkColor = Color.FromArgb(94, 148, 255);
            lnkReconfigure.Location = new Point(60, 545);
            lnkReconfigure.Name = "lnkReconfigure";
            lnkReconfigure.Size = new Size(400, 20);
            lnkReconfigure.TabIndex = 9;
            lnkReconfigure.TabStop = true;
            lnkReconfigure.Text = "Reconfigurer l'école";
            lnkReconfigure.TextAlign = ContentAlignment.MiddleCenter;
            lnkReconfigure.LinkClicked += lnkReconfigure_LinkClicked;
            // 
            // btnCancel
            // 
            btnCancel.BorderRadius = 10;
            btnCancel.FillColor = Color.FromArgb(200, 200, 200);
            btnCancel.Font = new Font("Segoe UI", 11F);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(60, 490);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(400, 40);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Annuler";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnLogin
            // 
            btnLogin.BorderRadius = 10;
            btnLogin.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(60, 430);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(400, 45);
            btnLogin.TabIndex = 7;
            btnLogin.Text = "Se connecter";
            btnLogin.Click += btnLogin_Click;
            // 
            // lblError
            // 
            lblError.Font = new Font("Segoe UI", 9F);
            lblError.ForeColor = Color.FromArgb(255, 87, 87);
            lblError.Location = new Point(60, 370);
            lblError.Name = "lblError";
            lblError.Size = new Size(400, 40);
            lblError.TabIndex = 6;
            lblError.Text = "Message d'erreur";
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            lblError.Visible = false;
            // 
            // txtPassword
            // 
            txtPassword.BorderRadius = 10;
            txtPassword.Cursor = Cursors.IBeam;
            txtPassword.DefaultText = "";
            txtPassword.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtPassword.Font = new Font("Segoe UI", 11F);
            txtPassword.Location = new Point(60, 320);
            txtPassword.Margin = new Padding(3, 4, 3, 4);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '●';
            txtPassword.PlaceholderText = "Entrez votre mot de passe";
            txtPassword.SelectedText = "";
            txtPassword.Size = new Size(400, 40);
            txtPassword.TabIndex = 5;
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.KeyPress += txtPassword_KeyPress;
            // 
            // lblPassword
            // 
            lblPassword.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPassword.ForeColor = Color.FromArgb(33, 37, 41);
            lblPassword.Location = new Point(60, 290);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(150, 25);
            lblPassword.TabIndex = 4;
            lblPassword.Text = "Mot de passe:";
            lblPassword.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtUsername
            // 
            txtUsername.BorderRadius = 10;
            txtUsername.Cursor = Cursors.IBeam;
            txtUsername.DefaultText = "";
            txtUsername.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtUsername.Font = new Font("Segoe UI", 11F);
            txtUsername.Location = new Point(60, 230);
            txtUsername.Margin = new Padding(3, 4, 3, 4);
            txtUsername.Name = "txtUsername";
            txtUsername.PasswordChar = '\0';
            txtUsername.PlaceholderText = "Entrez votre nom d'utilisateur";
            txtUsername.SelectedText = "";
            txtUsername.Size = new Size(400, 40);
            txtUsername.TabIndex = 3;
            txtUsername.KeyPress += txtUsername_KeyPress;
            // 
            // lblUsername
            // 
            lblUsername.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUsername.ForeColor = Color.FromArgb(33, 37, 41);
            lblUsername.Location = new Point(60, 200);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(150, 25);
            lblUsername.TabIndex = 2;
            lblUsername.Text = "Nom d'utilisateur:";
            lblUsername.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblEcoleInfo
            // 
            lblEcoleInfo.Font = new Font("Segoe UI", 11F);
            lblEcoleInfo.ForeColor = Color.FromArgb(100, 100, 100);
            lblEcoleInfo.Location = new Point(40, 130);
            lblEcoleInfo.Name = "lblEcoleInfo";
            lblEcoleInfo.Size = new Size(440, 30);
            lblEcoleInfo.TabIndex = 1;
            lblEcoleInfo.Text = "École: ";
            lblEcoleInfo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(94, 148, 255);
            lblTitle.Location = new Point(40, 80);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(440, 45);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Connexion";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelLeft
            // 
            panelLeft.BackColor = Color.Transparent;
            panelLeft.BorderRadius = 20;
            panelLeft.Controls.Add(picLogoEcole);
            panelLeft.Controls.Add(lblEcoleName);
            panelLeft.Controls.Add(lblWelcome);
            panelLeft.FillColor = Color.FromArgb(94, 148, 255);
            panelLeft.Location = new Point(0, 0);
            panelLeft.Name = "panelLeft";
            panelLeft.Size = new Size(400, 620);
            panelLeft.TabIndex = 0;
            // 
            // picLogoEcole
            // 
            picLogoEcole.BackColor = Color.Transparent;
            picLogoEcole.BorderRadius = 14;
            picLogoEcole.ImageRotate = 0F;
            picLogoEcole.Location = new Point(100, 147);
            picLogoEcole.Name = "picLogoEcole";
            picLogoEcole.Size = new Size(200, 200);
            picLogoEcole.SizeMode = PictureBoxSizeMode.Zoom;
            picLogoEcole.TabIndex = 2;
            picLogoEcole.TabStop = false;
            // 
            // lblEcoleName
            // 
            lblEcoleName.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblEcoleName.ForeColor = Color.White;
            lblEcoleName.Location = new Point(30, 410);
            lblEcoleName.Name = "lblEcoleName";
            lblEcoleName.Size = new Size(340, 80);
            lblEcoleName.TabIndex = 1;
            lblEcoleName.Text = "École";
            lblEcoleName.TextAlign = ContentAlignment.TopCenter;
            // 
            // lblWelcome
            // 
            lblWelcome.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblWelcome.ForeColor = Color.White;
            lblWelcome.Location = new Point(30, 350);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(340, 50);
            lblWelcome.TabIndex = 0;
            lblWelcome.Text = "Bienvenue";
            lblWelcome.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FormLogin
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 242, 245);
            ClientSize = new Size(1000, 700);
            Controls.Add(panelMain);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Connexion - EduKin";
            Load += FormLogin_Load;
            panelMain.ResumeLayout(false);
            panelRight.ResumeLayout(false);
            panelLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picLogoEcole).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Siticone.Desktop.UI.WinForms.SiticonePanel panelMain;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelLeft;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label lblEcoleName;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelRight;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblEcoleInfo;
        private System.Windows.Forms.Label lblUsername;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txtPassword;
        private System.Windows.Forms.Label lblError;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnLogin;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnCancel;
        private System.Windows.Forms.LinkLabel lnkReconfigure;
        private Siticone.Desktop.UI.WinForms.SiticonePictureBox picLogoEcole;
    }
}
