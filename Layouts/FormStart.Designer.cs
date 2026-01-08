namespace EduKin.Layouts
{
    partial class FormStart
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
            lblStatus = new Label();
            progressBar = new Siticone.Desktop.UI.WinForms.SiticoneProgressBar();
            lblAppName = new Label();
            pictureBoxLogo = new PictureBox();
            panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).BeginInit();
            SuspendLayout();
            // 
            // panelMain
            // 
            panelMain.BackColor = Color.Transparent;
            panelMain.BorderRadius = 15;
            panelMain.Controls.Add(lblStatus);
            panelMain.Controls.Add(progressBar);
            panelMain.Controls.Add(lblAppName);
            panelMain.Controls.Add(pictureBoxLogo);
            panelMain.FillColor = Color.White;
            panelMain.Location = new Point(50, 80);
            panelMain.Name = "panelMain";
            panelMain.ShadowDecoration.BorderRadius = 15;
            panelMain.ShadowDecoration.Depth = 20;
            panelMain.ShadowDecoration.Enabled = true;
            panelMain.ShadowDecoration.Shadow = new Padding(0, 0, 10, 10);
            panelMain.Size = new Size(500, 300);
            panelMain.TabIndex = 0;
            panelMain.Paint += panelMain_Paint;
            // 
            // lblStatus
            // 
            lblStatus.BackColor = Color.Transparent;
            lblStatus.Font = new Font("Segoe UI", 10F);
            lblStatus.ForeColor = Color.FromArgb(100, 100, 100);
            lblStatus.Location = new Point(50, 210);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(400, 25);
            lblStatus.TabIndex = 3;
            lblStatus.Text = "Initialisation...";
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // progressBar
            // 
            progressBar.BorderRadius = 10;
            progressBar.FillColor = Color.FromArgb(230, 230, 230);
            progressBar.Location = new Point(50, 180);
            progressBar.Name = "progressBar";
            progressBar.ProgressColor = Color.FromArgb(94, 148, 255);
            progressBar.ProgressColor2 = Color.FromArgb(71, 71, 135);
            progressBar.Size = new Size(400, 20);
            progressBar.TabIndex = 2;
            progressBar.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            // 
            // lblAppName
            // 
            lblAppName.BackColor = Color.Transparent;
            lblAppName.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblAppName.ForeColor = Color.FromArgb(94, 148, 255);
            lblAppName.Location = new Point(150, 120);
            lblAppName.Name = "lblAppName";
            lblAppName.Size = new Size(200, 35);
            lblAppName.TabIndex = 1;
            lblAppName.Text = "EduKin";
            lblAppName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pictureBoxLogo
            // 
            pictureBoxLogo.BackColor = Color.Transparent;
            pictureBoxLogo.Image = Properties.Resources.ExtatbeltLogo;
            pictureBoxLogo.Location = new Point(175, 30);
            pictureBoxLogo.Name = "pictureBoxLogo";
            pictureBoxLogo.Size = new Size(150, 80);
            pictureBoxLogo.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxLogo.TabIndex = 0;
            pictureBoxLogo.TabStop = false;
            // 
            // FormStart
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 242, 245);
            ClientSize = new Size(600, 450);
            Controls.Add(panelMain);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormStart";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "EduKin - Démarrage";
            Load += FormStart_Load;
            panelMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Siticone.Desktop.UI.WinForms.SiticonePanel panelMain;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Label lblAppName;
        private Siticone.Desktop.UI.WinForms.SiticoneProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
    }
}
