using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace EduKin.Inits
{
    /// <summary>
    /// Gestionnaire de photos pour charger et capturer des images
    /// </summary>
    public class PictureManager
    {
        private readonly string _defaultPhotoDirectory;

        public PictureManager(string photoDirectory = "Photos")
        {
            _defaultPhotoDirectory = photoDirectory;
            
            // Créer le répertoire s'il n'existe pas
            if (!Directory.Exists(_defaultPhotoDirectory))
            {
                Directory.CreateDirectory(_defaultPhotoDirectory);
            }
        }

        /// <summary>
        /// Charge une photo depuis un chemin et l'affiche dans un PictureBox
        /// </summary>
        /// <param name="pictureBox">PictureBox où afficher l'image</param>
        /// <param name="imagePath">Chemin de l'image</param>
        /// <returns>True si le chargement a réussi</returns>
        public bool LoadPicture(PictureBox pictureBox, string imagePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imagePath))
                {
                    SetDefaultImage(pictureBox);
                    return false;
                }

                // Vérifier si le fichier existe
                if (!File.Exists(imagePath))
                {
                    MessageBox.Show($"Le fichier image n'existe pas : {imagePath}", 
                        "Fichier introuvable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetDefaultImage(pictureBox);
                    return false;
                }

                // Charger l'image sans verrouiller le fichier
                using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    var image = Image.FromStream(stream);
                    
                    // Libérer l'ancienne image si elle existe
                    if (pictureBox.Image != null)
                    {
                        var oldImage = pictureBox.Image;
                        pictureBox.Image = null;
                        oldImage.Dispose();
                    }

                    pictureBox.Image = new Bitmap(image);
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de l'image : {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetDefaultImage(pictureBox);
                return false;
            }
        }

        /// <summary>
        /// Ouvre une boîte de dialogue pour sélectionner une image et la copie vers un emplacement sécurisé
        /// </summary>
        /// <param name="pictureBox">PictureBox où afficher l'image</param>
        /// <param name="securedPath">Chemin sécurisé de l'image copiée</param>
        /// <param name="uniqueIdentifier">Identifiant unique pour nommer le fichier (ex: matricule)</param>
        /// <returns>True si la sélection et la copie ont réussi</returns>
        public bool BrowseAndLoadPicture(PictureBox pictureBox, out string securedPath, string? uniqueIdentifier = null)
        {
            securedPath = string.Empty;

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Tous les fichiers|*.*";
                openFileDialog.Title = "Sélectionner une photo";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var sourcePath = openFileDialog.FileName;
                    
                    // Copier l'image vers un emplacement sécurisé
                    securedPath = CopyToSecureLocation(sourcePath, uniqueIdentifier);
                    
                    if (!string.IsNullOrEmpty(securedPath))
                    {
                        return LoadPicture(pictureBox, securedPath);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Copie une image depuis sa source vers un emplacement sécurisé dans l'application
        /// </summary>
        /// <param name="sourcePath">Chemin source de l'image</param>
        /// <param name="uniqueIdentifier">Identifiant unique pour nommer le fichier (ex: matricule)</param>
        /// <returns>Chemin sécurisé de l'image copiée ou null en cas d'erreur</returns>
        public string? CopyToSecureLocation(string sourcePath, string? uniqueIdentifier = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                {
                    MessageBox.Show("Le fichier source n'existe pas.", 
                        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                // Créer le répertoire sécurisé s'il n'existe pas
                var secureDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _defaultPhotoDirectory);
                if (!Directory.Exists(secureDirectory))
                {
                    Directory.CreateDirectory(secureDirectory);
                }

                // Générer un nom de fichier unique
                var extension = Path.GetExtension(sourcePath);
                string fileName;
                
                if (!string.IsNullOrWhiteSpace(uniqueIdentifier))
                {
                    // Utiliser l'identifiant unique + timestamp pour éviter les conflits
                    fileName = $"{uniqueIdentifier}_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
                }
                else
                {
                    // Utiliser uniquement le timestamp
                    fileName = $"Photo_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
                }

                var destinationPath = Path.Combine(secureDirectory, fileName);

                // Copier le fichier vers l'emplacement sécurisé
                File.Copy(sourcePath, destinationPath, overwrite: true);

                return destinationPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la copie de l'image : {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Sécurise une image existante en la copiant vers l'emplacement sécurisé si elle n'y est pas déjà
        /// </summary>
        /// <param name="imagePath">Chemin actuel de l'image</param>
        /// <param name="uniqueIdentifier">Identifiant unique pour nommer le fichier</param>
        /// <returns>Chemin sécurisé de l'image ou le chemin original si déjà sécurisé</returns>
        public string? SecureExistingImage(string imagePath, string? uniqueIdentifier = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imagePath))
                {
                    return null;
                }

                // Vérifier si l'image est déjà dans l'emplacement sécurisé
                var secureDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _defaultPhotoDirectory);
                var imageDirectory = Path.GetDirectoryName(Path.GetFullPath(imagePath));

                if (imageDirectory != null && imageDirectory.Equals(secureDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    // L'image est déjà dans l'emplacement sécurisé
                    return imagePath;
                }

                // L'image est ailleurs, la copier vers l'emplacement sécurisé
                if (File.Exists(imagePath))
                {
                    return CopyToSecureLocation(imagePath, uniqueIdentifier);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la sécurisation de l'image : {ex.Message}");
                return imagePath; // Retourner le chemin original en cas d'erreur
            }
        }

        /// <summary>
        /// Capture une photo depuis la webcam avec éditeur intégré
        /// </summary>
        /// <param name="pictureBox">PictureBox où afficher l'image</param>
        /// <param name="uniqueIdentifier">Identifiant unique (matricule) pour nommer le fichier</param>
        /// <returns>Chemin sécurisé de l'image capturée</returns>
        public async Task<string?> CapturePhotoAsync(PictureBox pictureBox, string? uniqueIdentifier = null)
        {
            try
            {
                // Créer et afficher le formulaire de capture
                using (var captureForm = new Layouts.FormWebcamCapture())
                {
                    if (captureForm.ShowDialog() == DialogResult.OK)
                    {
                        var capturedImage = captureForm.CapturedImage;
                        
                        if (capturedImage != null)
                        {
                            // Créer le répertoire sécurisé s'il n'existe pas
                            var secureDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _defaultPhotoDirectory);
                            if (!Directory.Exists(secureDirectory))
                            {
                                Directory.CreateDirectory(secureDirectory);
                            }

                            // Générer un nom de fichier unique
                            string fileName;
                            if (!string.IsNullOrWhiteSpace(uniqueIdentifier))
                            {
                                fileName = $"{uniqueIdentifier}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                            }
                            else
                            {
                                fileName = $"Photo_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                            }

                            var fullPath = Path.Combine(secureDirectory, fileName);

                            // Sauvegarder l'image dans l'emplacement sécurisé
                            capturedImage.Save(fullPath, ImageFormat.Jpeg);

                            // Charger dans le PictureBox
                            LoadPicture(pictureBox, fullPath);

                            return fullPath;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la capture : {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Définit une image par défaut (placeholder)
        /// </summary>
        private void SetDefaultImage(PictureBox pictureBox)
        {
            // Créer une image par défaut simple
            var defaultImage = new Bitmap(200, 200);
            using (var g = Graphics.FromImage(defaultImage))
            {
                g.Clear(Color.LightGray);
                g.DrawString("Aucune photo", 
                    new Font("Arial", 12), 
                    Brushes.DarkGray, 
                    new PointF(50, 90));
            }

            if (pictureBox.Image != null)
            {
                var oldImage = pictureBox.Image;
                pictureBox.Image = null;
                oldImage.Dispose();
            }

            pictureBox.Image = defaultImage;
            pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        /// <summary>
        /// Sauvegarde une image depuis un PictureBox vers un fichier
        /// </summary>
        /// <param name="pictureBox">PictureBox contenant l'image</param>
        /// <param name="fileName">Nom du fichier (optionnel)</param>
        /// <param name="targetDirectory">Répertoire cible (optionnel, utilise le répertoire par défaut si null)</param>
        /// <returns>Chemin complet du fichier sauvegardé ou null en cas d'erreur</returns>
        public string? SavePicture(PictureBox pictureBox, string? fileName = null, string? targetDirectory = null)
        {
            try
            {
                if (pictureBox.Image == null)
                {
                    MessageBox.Show("Aucune image à sauvegarder.", 
                        "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                // Utiliser le répertoire par défaut si non spécifié
                var directory = targetDirectory ?? _defaultPhotoDirectory;
                
                // Créer le répertoire s'il n'existe pas
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Générer un nom de fichier si non fourni
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = $"Photo_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                }

                // Assurer l'extension .jpg
                if (!fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
                    !fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    fileName += ".jpg";
                }

                var fullPath = Path.Combine(directory, fileName);

                // Sauvegarder l'image
                pictureBox.Image.Save(fullPath, ImageFormat.Jpeg);

                return fullPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde : {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Sauvegarde une image Bitmap vers un fichier
        /// </summary>
        /// <param name="image">Image à sauvegarder</param>
        /// <param name="fileName">Nom du fichier</param>
        /// <param name="targetDirectory">Répertoire cible (optionnel, utilise le répertoire par défaut si null)</param>
        /// <returns>Chemin complet du fichier sauvegardé ou null en cas d'erreur</returns>
        public string? SavePicture(Image image, string fileName, string? targetDirectory = null)
        {
            try
            {
                if (image == null)
                {
                    MessageBox.Show("Aucune image à sauvegarder.", 
                        "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                // Utiliser le répertoire par défaut si non spécifié
                var directory = targetDirectory ?? _defaultPhotoDirectory;
                
                // Créer le répertoire s'il n'existe pas
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Assurer l'extension .jpg
                if (!fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
                    !fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    fileName += ".jpg";
                }

                var fullPath = Path.Combine(directory, fileName);

                // Sauvegarder l'image
                image.Save(fullPath, ImageFormat.Jpeg);

                return fullPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde : {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Supprime une photo du disque
        /// </summary>
        public bool DeletePicture(string imagePath)
        {
            try
            {
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression : {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
