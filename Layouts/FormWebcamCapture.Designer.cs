using System.Drawing;
using System.Windows.Forms;

namespace EduKin.Layouts
{
    partial class FormWebcamCapture
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
            pictureBoxCamera = new Siticone.Desktop.UI.WinForms.SiticonePictureBox();
            panelControls = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            btnCancel = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            btnValidate = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            btnRetake = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            btnCapture = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            comboBoxCameras = new Siticone.Desktop.UI.WinForms.SiticoneComboBox();
            lblCamera = new Label();
            panelEdit = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            lblEdit = new Label();
            btnCrop = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            lblBrightness = new Label();
            trackBarBrightness = new TrackBar();
            lblContrast = new Label();
            trackBarContrast = new TrackBar();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCamera).BeginInit();
            panelControls.SuspendLayout();
            panelEdit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarContrast).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxCamera
            // 
            pictureBoxCamera.BackColor = Color.Black;
            pictureBoxCamera.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxCamera.ImageRotate = 0F;
            pictureBoxCamera.Location = new Point(20, 20);
            pictureBoxCamera.Name = "pictureBoxCamera";
            pictureBoxCamera.Size = new Size(640, 480);
            pictureBoxCamera.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxCamera.TabIndex = 0;
            pictureBoxCamera.TabStop = false;
            pictureBoxCamera.Paint += PictureBoxCamera_Paint;
            pictureBoxCamera.MouseDown += PictureBoxCamera_MouseDown;
            pictureBoxCamera.MouseMove += PictureBoxCamera_MouseMove;
            pictureBoxCamera.MouseUp += PictureBoxCamera_MouseUp;
            // 
            // panelControls
            // 
            panelControls.BackColor = Color.Transparent;
            panelControls.BorderRadius = 15;
            panelControls.Controls.Add(btnCancel);
            panelControls.Controls.Add(btnValidate);
            panelControls.Controls.Add(btnRetake);
            panelControls.Controls.Add(btnCapture);
            panelControls.Controls.Add(comboBoxCameras);
            panelControls.Controls.Add(lblCamera);
            panelControls.FillColor = Color.White;
            panelControls.Location = new Point(680, 20);
            panelControls.Name = "panelControls";
            panelControls.Padding = new Padding(15);
            panelControls.ShadowDecoration.BorderRadius = 15;
            panelControls.ShadowDecoration.Depth = 10;
            panelControls.ShadowDecoration.Enabled = true;
            panelControls.Size = new Size(250, 480);
            panelControls.TabIndex = 1;
            // 
            // btnCancel
            // 
            btnCancel.BorderRadius = 14;
            btnCancel.FillColor = Color.FromArgb(200, 200, 200);
            btnCancel.Font = new Font("Segoe UI", 10F);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(15, 430);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(220, 40);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "✗ Annuler";
            btnCancel.Click += BtnCancel_Click;
            // 
            // btnValidate
            // 
            btnValidate.BorderRadius = 14;
            btnValidate.DisabledState.BorderColor = Color.DarkGray;
            btnValidate.DisabledState.CustomBorderColor = Color.DarkGray;
            btnValidate.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnValidate.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnValidate.Enabled = false;
            btnValidate.FillColor = Color.FromArgb(94, 255, 148);
            btnValidate.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnValidate.ForeColor = Color.White;
            btnValidate.Location = new Point(15, 385);
            btnValidate.Name = "btnValidate";
            btnValidate.Size = new Size(220, 40);
            btnValidate.TabIndex = 4;
            btnValidate.Text = "✓ Valider";
            btnValidate.Click += BtnValidate_Click;
            // 
            // btnRetake
            // 
            btnRetake.BorderRadius = 14;
            btnRetake.DisabledState.BorderColor = Color.DarkGray;
            btnRetake.DisabledState.CustomBorderColor = Color.DarkGray;
            btnRetake.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnRetake.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnRetake.Enabled = false;
            btnRetake.FillColor = Color.FromArgb(255, 193, 7);
            btnRetake.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnRetake.ForeColor = Color.White;
            btnRetake.Location = new Point(15, 140);
            btnRetake.Name = "btnRetake";
            btnRetake.Size = new Size(220, 40);
            btnRetake.TabIndex = 3;
            btnRetake.Text = "🔄 Reprendre";
            btnRetake.Click += BtnRetake_Click;
            // 
            // btnCapture
            // 
            btnCapture.BorderRadius = 14;
            btnCapture.DisabledState.BorderColor = Color.DarkGray;
            btnCapture.DisabledState.CustomBorderColor = Color.DarkGray;
            btnCapture.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnCapture.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnCapture.Enabled = true;
            btnCapture.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCapture.ForeColor = Color.White;
            btnCapture.Location = new Point(15, 90);
            btnCapture.Name = "btnCapture";
            btnCapture.Size = new Size(220, 40);
            btnCapture.TabIndex = 2;
            btnCapture.Text = "📷 Capturer";
            btnCapture.Click += BtnCapture_Click;
            // 
            // comboBoxCameras
            // 
            comboBoxCameras.BackColor = Color.Transparent;
            comboBoxCameras.BorderRadius = 14;
            comboBoxCameras.DrawMode = DrawMode.OwnerDrawFixed;
            comboBoxCameras.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxCameras.FocusedColor = Color.Empty;
            comboBoxCameras.Font = new Font("Segoe UI", 9F);
            comboBoxCameras.ForeColor = Color.FromArgb(68, 88, 112);
            comboBoxCameras.FormattingEnabled = true;
            comboBoxCameras.ItemHeight = 30;
            comboBoxCameras.Location = new Point(15, 45);
            comboBoxCameras.Name = "comboBoxCameras";
            comboBoxCameras.Size = new Size(220, 36);
            comboBoxCameras.TabIndex = 1;
            comboBoxCameras.SelectedIndexChanged += ComboBoxCameras_SelectedIndexChanged;
            // 
            // lblCamera
            // 
            lblCamera.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCamera.ForeColor = Color.FromArgb(33, 37, 41);
            lblCamera.Location = new Point(15, 15);
            lblCamera.Name = "lblCamera";
            lblCamera.Size = new Size(220, 25);
            lblCamera.TabIndex = 0;
            lblCamera.Text = "Sélectionner la caméra:";
            // 
            // panelEdit
            // 
            panelEdit.BackColor = Color.Transparent;
            panelEdit.BorderRadius = 15;
            panelEdit.Controls.Add(lblEdit);
            panelEdit.Controls.Add(btnCrop);
            panelEdit.Controls.Add(lblBrightness);
            panelEdit.Controls.Add(trackBarBrightness);
            panelEdit.Controls.Add(lblContrast);
            panelEdit.Controls.Add(trackBarContrast);
            panelEdit.FillColor = Color.White;
            panelEdit.Location = new Point(20, 520);
            panelEdit.Name = "panelEdit";
            panelEdit.Padding = new Padding(15);
            panelEdit.ShadowDecoration.BorderRadius = 15;
            panelEdit.ShadowDecoration.Depth = 10;
            panelEdit.ShadowDecoration.Enabled = true;
            panelEdit.Size = new Size(910, 120);
            panelEdit.TabIndex = 2;
            // 
            // lblEdit
            // 
            lblEdit.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblEdit.ForeColor = Color.FromArgb(33, 37, 41);
            lblEdit.Location = new Point(15, 15);
            lblEdit.Name = "lblEdit";
            lblEdit.Size = new Size(150, 25);
            lblEdit.TabIndex = 0;
            lblEdit.Text = "Édition d'image:";
            // 
            // btnCrop
            // 
            btnCrop.BorderRadius = 10;
            btnCrop.DisabledState.BorderColor = Color.DarkGray;
            btnCrop.DisabledState.CustomBorderColor = Color.DarkGray;
            btnCrop.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnCrop.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnCrop.Enabled = false;
            btnCrop.Font = new Font("Segoe UI", 9F);
            btnCrop.ForeColor = Color.White;
            btnCrop.Location = new Point(15, 50);
            btnCrop.Name = "btnCrop";
            btnCrop.Size = new Size(150, 35);
            btnCrop.TabIndex = 1;
            btnCrop.Text = "✂ Recadrer";
            btnCrop.Click += BtnCrop_Click;
            // 
            // lblBrightness
            // 
            lblBrightness.Font = new Font("Segoe UI", 9F);
            lblBrightness.ForeColor = Color.FromArgb(100, 100, 100);
            lblBrightness.Location = new Point(200, 15);
            lblBrightness.Name = "lblBrightness";
            lblBrightness.Size = new Size(100, 20);
            lblBrightness.TabIndex = 2;
            lblBrightness.Text = "Luminosité:";
            // 
            // trackBarBrightness
            // 
            trackBarBrightness.Enabled = false;
            trackBarBrightness.Location = new Point(200, 40);
            trackBarBrightness.Maximum = 100;
            trackBarBrightness.Minimum = -100;
            trackBarBrightness.Name = "trackBarBrightness";
            trackBarBrightness.Size = new Size(300, 56);
            trackBarBrightness.TabIndex = 3;
            trackBarBrightness.TickFrequency = 10;
            trackBarBrightness.Scroll += TrackBarBrightness_Scroll;
            // 
            // lblContrast
            // 
            lblContrast.Font = new Font("Segoe UI", 9F);
            lblContrast.ForeColor = Color.FromArgb(100, 100, 100);
            lblContrast.Location = new Point(540, 15);
            lblContrast.Name = "lblContrast";
            lblContrast.Size = new Size(100, 20);
            lblContrast.TabIndex = 4;
            lblContrast.Text = "Contraste:";
            // 
            // trackBarContrast
            // 
            trackBarContrast.Enabled = false;
            trackBarContrast.Location = new Point(540, 40);
            trackBarContrast.Maximum = 100;
            trackBarContrast.Minimum = -100;
            trackBarContrast.Name = "trackBarContrast";
            trackBarContrast.Size = new Size(300, 56);
            trackBarContrast.TabIndex = 5;
            trackBarContrast.TickFrequency = 10;
            trackBarContrast.Scroll += TrackBarContrast_Scroll;
            // 
            // FormWebcamCapture
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(243, 243, 243);
            ClientSize = new Size(950, 660);
            Controls.Add(panelEdit);
            Controls.Add(panelControls);
            Controls.Add(pictureBoxCamera);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormWebcamCapture";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Capture Webcam - EduKin";
            FormClosing += FormWebcamCapture_FormClosing;
            ((System.ComponentModel.ISupportInitialize)pictureBoxCamera).EndInit();
            panelControls.ResumeLayout(false);
            panelEdit.ResumeLayout(false);
            panelEdit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarContrast).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Siticone.Desktop.UI.WinForms.SiticonePictureBox pictureBoxCamera;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelControls;
        private System.Windows.Forms.Label lblCamera;
        private Siticone.Desktop.UI.WinForms.SiticoneComboBox comboBoxCameras;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnCapture;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnRetake;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnValidate;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnCancel;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelEdit;
        private System.Windows.Forms.Label lblEdit;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnCrop;
        private System.Windows.Forms.Label lblBrightness;
        private System.Windows.Forms.TrackBar trackBarBrightness;
        private System.Windows.Forms.Label lblContrast;
        private System.Windows.Forms.TrackBar trackBarContrast;
    }
}
