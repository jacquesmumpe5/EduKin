using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace EduKin.Layouts
{
    /// <summary>
    /// Formulaire de capture webcam avec éditeur d'image intégré
    /// </summary>
    public partial class FormWebcamCapture : Form
    {
        private VideoCapture? _capture;
        private System.Threading.Timer? _timer;
        private Bitmap? _capturedImage;
        private Bitmap? _originalImage;
        private Rectangle _cropRectangle;
        private bool _isCropping;
        private System.Drawing.Point _cropStartPoint;

        public Bitmap? CapturedImage => _capturedImage;

        public FormWebcamCapture()
        {
            InitializeComponent();
            InitializeFluentTheme();
            InitializeWebcam();
        }

        private void InitializeFluentTheme()
        {
            this.BackColor = Color.FromArgb(243, 243, 243);
        }

        private void InitializeWebcam()
        {
            try
            {
                // Lister les caméras disponibles
                for (int i = 0; i < 5; i++)
                {
                    using (var testCapture = new VideoCapture(i))
                    {
                        if (testCapture.IsOpened())
                        {
                            comboBoxCameras.Items.Add($"Caméra {i}");
                        }
                    }
                }

                if (comboBoxCameras.Items.Count == 0)
                {
                    MessageBox.Show("Aucune webcam détectée sur cet ordinateur.", "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnCapture.Enabled = false;
                    return;
                }

                comboBoxCameras.SelectedIndex = 0;
                btnCapture.Enabled = true; // Activer le bouton capture quand une caméra est disponible
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur d'initialisation de la webcam : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ComboBoxCameras_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (comboBoxCameras.SelectedIndex >= 0)
            {
                StartCamera();
            }
        }

        private void StartCamera()
        {
            try
            {
                StopCamera();

                _capture = new VideoCapture(comboBoxCameras.SelectedIndex);
                _capture.Set(VideoCaptureProperties.FrameWidth, 640);
                _capture.Set(VideoCaptureProperties.FrameHeight, 480);

                // Timer pour capturer les frames
                _timer = new System.Threading.Timer(GrabFrame, null, 0, 33); // ~30 FPS
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de démarrage de la caméra : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GrabFrame(object? state)
        {
            if (_capture == null || !_capture.IsOpened())
                return;

            try
            {
                using (var frame = new Mat())
                {
                    _capture.Read(frame);
                    if (!frame.Empty())
                    {
                        var bitmap = BitmapConverter.ToBitmap(frame);
                        
                        if (pictureBoxCamera.InvokeRequired)
                        {
                            pictureBoxCamera.Invoke(new Action(() =>
                            {
                                var oldImage = pictureBoxCamera.Image;
                                pictureBoxCamera.Image = bitmap;
                                oldImage?.Dispose();
                            }));
                        }
                        else
                        {
                            var oldImage = pictureBoxCamera.Image;
                            pictureBoxCamera.Image = bitmap;
                            oldImage?.Dispose();
                        }
                    }
                }
            }
            catch
            {
                // Ignorer les erreurs de frame
            }
        }

        private void BtnCapture_Click(object? sender, EventArgs e)
        {
            try
            {
                if (pictureBoxCamera.Image != null)
                {
                    _originalImage = new Bitmap(pictureBoxCamera.Image);
                    _capturedImage = new Bitmap(_originalImage);
                    
                    StopCamera();
                    
                    pictureBoxCamera.Image = _capturedImage;
                    
                    // Désactiver les contrôles de capture
                    btnCapture.Enabled = false;
                    comboBoxCameras.Enabled = false;
                    
                    // Activer les contrôles d'édition
                    btnRetake.Enabled = true;
                    btnValidate.Enabled = true;
                    panelEdit.Enabled = true;
                    btnCrop.Enabled = true;
                    trackBarBrightness.Enabled = true;
                    trackBarContrast.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la capture : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRetake_Click(object? sender, EventArgs e)
        {
            _capturedImage?.Dispose();
            _capturedImage = null;
            _originalImage?.Dispose();
            _originalImage = null;
            
            // Réinitialiser les contrôles d'édition
            trackBarBrightness.Value = 0;
            trackBarContrast.Value = 0;
            lblBrightness.Text = "Luminosité: 0";
            lblContrast.Text = "Contraste: 0";
            _cropRectangle = Rectangle.Empty;
            _isCropping = false;
            btnCrop.Text = "✂ Rogner";
            
            // Réactiver les contrôles de capture
            btnCapture.Enabled = true;
            comboBoxCameras.Enabled = true;
            
            // Désactiver les contrôles d'édition
            btnRetake.Enabled = false;
            btnValidate.Enabled = false;
            panelEdit.Enabled = false;
            btnCrop.Enabled = false;
            trackBarBrightness.Enabled = false;
            trackBarContrast.Enabled = false;
            
            StartCamera();
        }

        private void BtnValidate_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            _capturedImage?.Dispose();
            _capturedImage = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnCrop_Click(object? sender, EventArgs e)
        {
            if (_capturedImage == null) return;

            _isCropping = !_isCropping;
            
            if (_isCropping)
            {
                btnCrop.Text = "✓ Appliquer le rognage";
                btnCrop.BackColor = Color.FromArgb(0, 120, 212);
                btnCrop.ForeColor = Color.White;
                MessageBox.Show("Sélectionnez la zone à rogner en maintenant le bouton gauche de la souris enfoncé.", 
                    "Mode rognage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Désactiver les autres contrôles pendant le rognage
                trackBarBrightness.Enabled = false;
                trackBarContrast.Enabled = false;
                btnValidate.Enabled = false;
                btnRetake.Enabled = false;
            }
            else
            {
                if (_cropRectangle.Width > 0 && _cropRectangle.Height > 0)
                {
                    ApplyCrop();
                }
                
                btnCrop.Text = "✂ Rogner";
                btnCrop.BackColor = SystemColors.Control;
                btnCrop.ForeColor = SystemColors.ControlText;
                
                // Réactiver les autres contrôles
                trackBarBrightness.Enabled = true;
                trackBarContrast.Enabled = true;
                btnValidate.Enabled = true;
                btnRetake.Enabled = true;
            }
        }

        private void PictureBoxCamera_MouseDown(object? sender, MouseEventArgs e)
        {
            if (_isCropping && _capturedImage != null)
            {
                _cropStartPoint = e.Location;
            }
        }

        private void PictureBoxCamera_MouseMove(object? sender, MouseEventArgs e)
        {
            if (_isCropping && e.Button == MouseButtons.Left && _capturedImage != null)
            {
                var width = e.X - _cropStartPoint.X;
                var height = e.Y - _cropStartPoint.Y;
                _cropRectangle = new Rectangle(_cropStartPoint.X, _cropStartPoint.Y, width, height);
                pictureBoxCamera.Invalidate();
            }
        }

        private void PictureBoxCamera_MouseUp(object? sender, MouseEventArgs e)
        {
            if (_isCropping)
            {
                pictureBoxCamera.Invalidate();
            }
        }

        private void PictureBoxCamera_Paint(object? sender, PaintEventArgs e)
        {
            if (_isCropping && _cropRectangle.Width > 0 && _cropRectangle.Height > 0)
            {
                using (var pen = new Pen(Color.FromArgb(0, 120, 212), 3))
                {
                    pen.DashStyle = DashStyle.Dash;
                    e.Graphics.DrawRectangle(pen, _cropRectangle);
                }
            }
        }

        private void ApplyCrop()
        {
            if (_capturedImage == null || _cropRectangle.Width <= 0 || _cropRectangle.Height <= 0)
            {
                MessageBox.Show("Veuillez sélectionner une zone à rogner.", 
                    "Rognage", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var imageRect = GetImageRectangle();
                
                // Calculer les ratios d'échelle
                var scaleX = (float)_capturedImage.Width / imageRect.Width;
                var scaleY = (float)_capturedImage.Height / imageRect.Height;

                // Convertir les coordonnées du rectangle de sélection en coordonnées de l'image
                var cropX = (int)((_cropRectangle.X - imageRect.X) * scaleX);
                var cropY = (int)((_cropRectangle.Y - imageRect.Y) * scaleY);
                var cropWidth = (int)(_cropRectangle.Width * scaleX);
                var cropHeight = (int)(_cropRectangle.Height * scaleY);

                // S'assurer que les coordonnées sont dans les limites de l'image
                cropX = Math.Max(0, Math.Min(cropX, _capturedImage.Width - 1));
                cropY = Math.Max(0, Math.Min(cropY, _capturedImage.Height - 1));
                cropWidth = Math.Min(cropWidth, _capturedImage.Width - cropX);
                cropHeight = Math.Min(cropHeight, _capturedImage.Height - cropY);

                if (cropWidth > 0 && cropHeight > 0)
                {
                    var croppedImage = new Bitmap(cropWidth, cropHeight);
                    using (var g = Graphics.FromImage(croppedImage))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(_capturedImage, 
                            new Rectangle(0, 0, cropWidth, cropHeight),
                            new Rectangle(cropX, cropY, cropWidth, cropHeight),
                            GraphicsUnit.Pixel);
                    }

                    _capturedImage.Dispose();
                    _capturedImage = croppedImage;
                    _originalImage?.Dispose();
                    _originalImage = new Bitmap(_capturedImage);
                    
                    // Réinitialiser les ajustements
                    trackBarBrightness.Value = 0;
                    trackBarContrast.Value = 0;
                    lblBrightness.Text = "Luminosité: 0";
                    lblContrast.Text = "Contraste: 0";
                    
                    pictureBoxCamera.Image = _capturedImage;
                    MessageBox.Show("Rognage appliqué avec succès!", 
                        "Rognage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                _cropRectangle = Rectangle.Empty;
                pictureBoxCamera.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du rognage : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Rectangle GetImageRectangle()
        {
            if (pictureBoxCamera.Image == null)
                return Rectangle.Empty;

            var imageWidth = pictureBoxCamera.Image.Width;
            var imageHeight = pictureBoxCamera.Image.Height;
            var controlWidth = pictureBoxCamera.Width;
            var controlHeight = pictureBoxCamera.Height;

            var imageRatio = (float)imageWidth / imageHeight;
            var controlRatio = (float)controlWidth / controlHeight;

            int width, height, x, y;

            if (imageRatio > controlRatio)
            {
                width = controlWidth;
                height = (int)(controlWidth / imageRatio);
                x = 0;
                y = (controlHeight - height) / 2;
            }
            else
            {
                height = controlHeight;
                width = (int)(controlHeight * imageRatio);
                x = (controlWidth - width) / 2;
                y = 0;
            }

            return new Rectangle(x, y, width, height);
        }

        private void TrackBarBrightness_ValueChanged(object? sender, EventArgs e)
        {
            lblBrightness.Text = $"Luminosité: {trackBarBrightness.Value}";
            ApplyImageAdjustments();
        }

        private void TrackBarContrast_ValueChanged(object? sender, EventArgs e)
        {
            lblContrast.Text = $"Contraste: {trackBarContrast.Value}";
            ApplyImageAdjustments();
        }

        private void ApplyImageAdjustments()
        {
            if (_originalImage == null) return;

            try
            {
                var brightness = trackBarBrightness.Value / 100f;
                var contrast = trackBarContrast.Value / 100f;

                _capturedImage?.Dispose();
                _capturedImage = AdjustBrightnessContrast(_originalImage, brightness, contrast);
                pictureBoxCamera.Image = _capturedImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajustement : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Bitmap AdjustBrightnessContrast(Bitmap image, float brightness, float contrast)
        {
            var adjustedImage = new Bitmap(image.Width, image.Height);
            
            // Utiliser ColorMatrix pour de meilleures performances
            var contrastFactor = (1.0f + contrast) * (1.0f + contrast);
            
            // Créer une matrice de couleur pour les ajustements
            float[][] colorMatrixElements = {
                new float[] {contrastFactor, 0, 0, 0, 0},
                new float[] {0, contrastFactor, 0, 0, 0},
                new float[] {0, 0, contrastFactor, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {brightness, brightness, brightness, 0, 1}
            };
            
            var colorMatrix = new ColorMatrix(colorMatrixElements);
            var imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            
            using (var g = Graphics.FromImage(adjustedImage))
            {
                g.DrawImage(image, 
                    new Rectangle(0, 0, image.Width, image.Height),
                    0, 0, image.Width, image.Height,
                    GraphicsUnit.Pixel, imageAttributes);
            }
            
            return adjustedImage;
        }

        private void StopCamera()
        {
            _timer?.Dispose();
            _timer = null;
            
            _capture?.Release();
            _capture?.Dispose();
            _capture = null;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopCamera();
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Gère le changement de luminosité
        /// </summary>
        private void TrackBarBrightness_Scroll(object sender, EventArgs e)
        {
            if (trackBarBrightness != null && _originalImage != null)
            {
                ApplyImageAdjustments();
            }
        }

        /// <summary>
        /// Gère le changement de contraste
        /// </summary>
        private void TrackBarContrast_Scroll(object sender, EventArgs e)
        {
            if (trackBarContrast != null && _originalImage != null)
            {
                ApplyImageAdjustments();
            }
        }

        /// <summary>
        /// Gère la fermeture du formulaire
        /// </summary>
        private void FormWebcamCapture_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopCamera();
        }
    }
}
