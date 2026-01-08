using System.Drawing;
using System.Windows.Forms;

namespace EduKin.Layouts
{
    partial class FormPictureManagerExample
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
            this.cardMain = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.cardControls = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            this.btnClear = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            this.btnCaptureWebcam = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            this.btnBrowse = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            this.btnLoadFromFile = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            this.lblPath = new System.Windows.Forms.Label();
            this.txtImagePath = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cardMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.cardControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // cardMain
            // 
            this.cardMain.BackColor = System.Drawing.Color.Transparent;
            this.cardMain.BorderRadius = 15;
            this.cardMain.Controls.Add(this.pictureBox);
            this.cardMain.FillColor = System.Drawing.Color.White;
            this.cardMain.Location = new System.Drawing.Point(20, 20);
            this.cardMain.Name = "cardMain";
            this.cardMain.Padding = new System.Windows.Forms.Padding(15);
            this.cardMain.ShadowDecoration.BorderRadius = 15;
            this.cardMain.ShadowDecoration.Depth = 10;
            this.cardMain.ShadowDecoration.Enabled = true;
            this.cardMain.Size = new System.Drawing.Size(430, 430);
            this.cardMain.TabIndex = 0;
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(15, 15);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(398, 398);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // cardControls
            // 
            this.cardControls.BackColor = System.Drawing.Color.Transparent;
            this.cardControls.BorderRadius = 15;
            this.cardControls.Controls.Add(this.btnClear);
            this.cardControls.Controls.Add(this.btnCaptureWebcam);
            this.cardControls.Controls.Add(this.btnBrowse);
            this.cardControls.Controls.Add(this.btnLoadFromFile);
            this.cardControls.FillColor = System.Drawing.Color.White;
            this.cardControls.Location = new System.Drawing.Point(470, 20);
            this.cardControls.Name = "cardControls";
            this.cardControls.Padding = new System.Windows.Forms.Padding(15);
            this.cardControls.ShadowDecoration.BorderRadius = 15;
            this.cardControls.ShadowDecoration.Depth = 10;
            this.cardControls.ShadowDecoration.Enabled = true;
            this.cardControls.Size = new System.Drawing.Size(250, 430);
            this.cardControls.TabIndex = 1;
            // 
            // btnLoadFromFile
            // 
            this.btnLoadFromFile.BorderRadius = 10;
            this.btnLoadFromFile.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.btnLoadFromFile.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLoadFromFile.ForeColor = System.Drawing.Color.White;
            this.btnLoadFromFile.Location = new System.Drawing.Point(15, 15);
            this.btnLoadFromFile.Name = "btnLoadFromFile";
            this.btnLoadFromFile.Size = new System.Drawing.Size(220, 45);
            this.btnLoadFromFile.TabIndex = 0;
            this.btnLoadFromFile.Text = "📁 Charger depuis fichier";
            this.btnLoadFromFile.Click += new System.EventHandler(this.BtnLoadFromFile_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.BorderRadius = 10;
            this.btnBrowse.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.btnBrowse.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnBrowse.ForeColor = System.Drawing.Color.White;
            this.btnBrowse.Location = new System.Drawing.Point(15, 75);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(220, 45);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "🔍 Parcourir...";
            this.btnBrowse.Click += new System.EventHandler(this.BtnBrowse_Click);
            // 
            // btnCaptureWebcam
            // 
            this.btnCaptureWebcam.BorderRadius = 10;
            this.btnCaptureWebcam.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.btnCaptureWebcam.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCaptureWebcam.ForeColor = System.Drawing.Color.White;
            this.btnCaptureWebcam.Location = new System.Drawing.Point(15, 135);
            this.btnCaptureWebcam.Name = "btnCaptureWebcam";
            this.btnCaptureWebcam.Size = new System.Drawing.Size(220, 45);
            this.btnCaptureWebcam.TabIndex = 2;
            this.btnCaptureWebcam.Text = "📷 Capturer webcam";
            this.btnCaptureWebcam.Click += new System.EventHandler(this.BtnCaptureWebcam_Click);
            // 
            // btnClear
            // 
            this.btnClear.BorderRadius = 10;
            this.btnClear.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(87)))), ((int)(((byte)(87)))));
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Location = new System.Drawing.Point(15, 195);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(220, 45);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "🗑 Effacer";
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // lblPath
            // 
            this.lblPath.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblPath.Location = new System.Drawing.Point(20, 470);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(150, 20);
            this.lblPath.TabIndex = 2;
            this.lblPath.Text = "Chemin de l'image:";
            // 
            // txtImagePath
            // 
            this.txtImagePath.BorderRadius = 8;
            this.txtImagePath.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtImagePath.DefaultText = "";
            this.txtImagePath.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtImagePath.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtImagePath.Location = new System.Drawing.Point(20, 495);
            this.txtImagePath.Name = "txtImagePath";
            this.txtImagePath.PasswordChar = '\0';
            this.txtImagePath.PlaceholderText = "Chemin du fichier image...";
            this.txtImagePath.ReadOnly = true;
            this.txtImagePath.SelectedText = "";
            this.txtImagePath.Size = new System.Drawing.Size(700, 36);
            this.txtImagePath.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblStatus.Location = new System.Drawing.Point(20, 545);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(700, 20);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Prêt";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormPictureManagerExample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.ClientSize = new System.Drawing.Size(740, 580);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtImagePath);
            this.Controls.Add(this.lblPath);
            this.Controls.Add(this.cardControls);
            this.Controls.Add(this.cardMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPictureManagerExample";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestionnaire d'Images - EduKin";
            this.cardMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.cardControls.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Siticone.Desktop.UI.WinForms.SiticonePanel cardMain;
        private System.Windows.Forms.PictureBox pictureBox;
        private Siticone.Desktop.UI.WinForms.SiticonePanel cardControls;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnLoadFromFile;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnBrowse;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnCaptureWebcam;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnClear;
        private System.Windows.Forms.Label lblPath;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txtImagePath;
        private System.Windows.Forms.Label lblStatus;
    }
}
