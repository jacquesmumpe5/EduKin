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
            this.panelMain = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            this.panelLeft = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblEcoleName = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelRight = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblEcoleInfo = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            this.lblError = new System.Windows.Forms.Label();
            this.btnLogin = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            this.btnCancel = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            this.lnkReconfigure = new System.Windows.Forms.LinkLabel();
            this.panelMain.SuspendLayout();
            this.panelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.White;
            this.panelMain.BorderRadius = 20;
            this.panelMain.Controls.Add(this.panelRight);
            this.panelMain.Controls.Add(this.panelLeft);
            this.panelMain.FillColor = System.Drawing.Color.White;
            this.panelMain.Location = new System.Drawing.Point(40, 40);
            this.panelMain.Name = "panelMain";
            this.panelMain.ShadowDecoration.BorderRadius = 20;
            this.panelMain.ShadowDecoration.Depth = 30;
            this.panelMain.ShadowDecoration.Enabled = true;
            this.panelMain.Size = new System.Drawing.Size(920, 620);
            this.panelMain.TabIndex = 0;
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.Transparent;
            this.panelLeft.BorderRadius = 20;
            this.panelLeft.Controls.Add(this.pictureBox1);
            this.panelLeft.Controls.Add(this.lblEcoleName);
            this.panelLeft.Controls.Add(this.lblWelcome);
            this.panelLeft.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(400, 620);
            this.panelLeft.TabIndex = 0;
            // 
            // lblWelcome
            // 
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.ForeColor = System.Drawing.Color.White;
            this.lblWelcome.Location = new System.Drawing.Point(30, 350);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(340, 50);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Bienvenue";
            this.lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEcoleName
            // 
            this.lblEcoleName.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblEcoleName.ForeColor = System.Drawing.Color.White;
            this.lblEcoleName.Location = new System.Drawing.Point(30, 410);
            this.lblEcoleName.Name = "lblEcoleName";
            this.lblEcoleName.Size = new System.Drawing.Size(340, 80);
            this.lblEcoleName.TabIndex = 1;
            this.lblEcoleName.Text = "École";
            this.lblEcoleName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Location = new System.Drawing.Point(100, 20);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(200, 200);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // panelRight
            // 
            this.panelRight.BackColor = System.Drawing.Color.Transparent;
            this.panelRight.Controls.Add(this.lnkReconfigure);
            this.panelRight.Controls.Add(this.btnCancel);
            this.panelRight.Controls.Add(this.btnLogin);
            this.panelRight.Controls.Add(this.lblError);
            this.panelRight.Controls.Add(this.txtPassword);
            this.panelRight.Controls.Add(this.lblPassword);
            this.panelRight.Controls.Add(this.txtUsername);
            this.panelRight.Controls.Add(this.lblUsername);
            this.panelRight.Controls.Add(this.lblEcoleInfo);
            this.panelRight.Controls.Add(this.lblTitle);
            this.panelRight.Location = new System.Drawing.Point(400, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(520, 620);
            this.panelRight.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.lblTitle.Location = new System.Drawing.Point(40, 80);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(440, 45);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Connexion";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEcoleInfo
            // 
            this.lblEcoleInfo.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblEcoleInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblEcoleInfo.Location = new System.Drawing.Point(40, 130);
            this.lblEcoleInfo.Name = "lblEcoleInfo";
            this.lblEcoleInfo.Size = new System.Drawing.Size(440, 30);
            this.lblEcoleInfo.TabIndex = 1;
            this.lblEcoleInfo.Text = "École: ";
            this.lblEcoleInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUsername
            // 
            this.lblUsername.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblUsername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblUsername.Location = new System.Drawing.Point(60, 200);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(150, 25);
            this.lblUsername.TabIndex = 2;
            this.lblUsername.Text = "Nom d'utilisateur:";
            this.lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtUsername
            // 
            this.txtUsername.BorderRadius = 10;
            this.txtUsername.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtUsername.DefaultText = "";
            this.txtUsername.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtUsername.Location = new System.Drawing.Point(60, 230);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.PasswordChar = '\0';
            this.txtUsername.PlaceholderText = "Entrez votre nom d'utilisateur";
            this.txtUsername.SelectedText = "";
            this.txtUsername.Size = new System.Drawing.Size(400, 40);
            this.txtUsername.TabIndex = 3;
            this.txtUsername.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtUsername_KeyPress);
            // 
            // lblPassword
            // 
            this.lblPassword.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblPassword.Location = new System.Drawing.Point(60, 290);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(150, 25);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "Mot de passe:";
            this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPassword
            // 
            this.txtPassword.BorderRadius = 10;
            this.txtPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPassword.DefaultText = "";
            this.txtPassword.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtPassword.Location = new System.Drawing.Point(60, 320);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.PlaceholderText = "Entrez votre mot de passe";
            this.txtPassword.SelectedText = "";
            this.txtPassword.Size = new System.Drawing.Size(400, 40);
            this.txtPassword.TabIndex = 5;
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPassword_KeyPress);
            // 
            // lblError
            // 
            this.lblError.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(87)))), ((int)(((byte)(87)))));
            this.lblError.Location = new System.Drawing.Point(60, 370);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(400, 40);
            this.lblError.TabIndex = 6;
            this.lblError.Text = "Message d'erreur";
            this.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblError.Visible = false;
            // 
            // btnLogin
            // 
            this.btnLogin.BorderRadius = 10;
            this.btnLogin.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(60, 430);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(400, 45);
            this.btnLogin.TabIndex = 7;
            this.btnLogin.Text = "Se connecter";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BorderRadius = 10;
            this.btnCancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(60, 490);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(400, 40);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Annuler";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lnkReconfigure
            // 
            this.lnkReconfigure.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lnkReconfigure.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.lnkReconfigure.Location = new System.Drawing.Point(60, 545);
            this.lnkReconfigure.Name = "lnkReconfigure";
            this.lnkReconfigure.Size = new System.Drawing.Size(400, 20);
            this.lnkReconfigure.TabIndex = 9;
            this.lnkReconfigure.TabStop = true;
            this.lnkReconfigure.Text = "Reconfigurer l'école";
            this.lnkReconfigure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkReconfigure.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkReconfigure_LinkClicked);
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Controls.Add(this.panelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connexion - EduKin";
            this.Load += new System.EventHandler(this.FormLogin_Load);
            this.panelMain.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelRight.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Siticone.Desktop.UI.WinForms.SiticonePanel panelMain;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelLeft;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label lblEcoleName;
        private System.Windows.Forms.PictureBox pictureBox1;
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
    }
}
