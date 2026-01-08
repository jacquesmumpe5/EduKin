using System;
using System.Drawing;
using System.Windows.Forms;
using EduKin.Inits;


namespace EduKin.Layouts
{
    /// <summary>
    /// Exemple d'utilisation de PictureManager - Style Fluent
    /// </summary>
    public partial class FormPictureManagerExample : Form
    {
        private readonly PictureManager _pictureManager;
        private string? _currentImagePath;

        public FormPictureManagerExample()
        {
            InitializeComponent();
            _pictureManager = new PictureManager("Photos");
        }

        private void BtnLoadFromFile_Click(object? sender, EventArgs e)
        {
            var path = txtImagePath.Text.Trim();

            if (string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("Veuillez entrer un chemin d'image.",
                    "Chemin requis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_pictureManager.LoadPicture(pictureBox, path))
            {
                _currentImagePath = path;
                lblStatus.Text = $"✓ Image chargée : {path}";
                lblStatus.ForeColor = Color.FromArgb(16, 137, 62);
            }
            else
            {
                lblStatus.Text = "✗ Échec du chargement de l'image.";
                lblStatus.ForeColor = Color.FromArgb(232, 17, 35);
            }
        }

        private void BtnBrowse_Click(object? sender, EventArgs e)
        {
            if (_pictureManager.BrowseAndLoadPicture(pictureBox, out string selectedPath))
            {
                _currentImagePath = selectedPath;
                txtImagePath.Text = selectedPath;
                lblStatus.Text = $"✓ Image chargée : {selectedPath}";
                lblStatus.ForeColor = Color.FromArgb(16, 137, 62);
            }
            else
            {
                lblStatus.Text = "Aucune image sélectionnée.";
                lblStatus.ForeColor = Color.FromArgb(255, 140, 0);
            }
        }

        private async void BtnCaptureWebcam_Click(object? sender, EventArgs e)
        {
            lblStatus.Text = "📷 Ouverture de la webcam...";
            lblStatus.ForeColor = Color.FromArgb(0, 120, 212);

            var capturedPath = await _pictureManager.CapturePhotoAsync(pictureBox);

            if (capturedPath != null)
            {
                _currentImagePath = capturedPath;
                txtImagePath.Text = capturedPath;
                lblStatus.Text = $"✓ Photo capturée et enregistrée : {capturedPath}";
                lblStatus.ForeColor = Color.FromArgb(16, 137, 62);
            }
            else
            {
                lblStatus.Text = "Capture annulée ou échouée.";
                lblStatus.ForeColor = Color.FromArgb(255, 140, 0);
            }
        }

        private void BtnClear_Click(object? sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                var oldImage = pictureBox.Image;
                pictureBox.Image = null;
                oldImage.Dispose();
            }

            txtImagePath.Clear();
            _currentImagePath = null;
            lblStatus.Text = "Image effacée.";
            lblStatus.ForeColor = Color.Gray;
        }

        /// <summary>
        /// Méthode pour afficher le formulaire d'exemple
        /// </summary>
        public static void ShowExample()
        {
            using (var form = new FormPictureManagerExample())
            {
                form.ShowDialog();
            }
        }

        private void FormPictureManagerExample_Load(object sender, EventArgs e)
        {

        }
    }
}
