using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using EduKin.Inits;
using EduKin.Layouts;

namespace EduKin.Csharp.Admins
{
    /// <summary>
    /// Contrôleur centralisé pour la gestion des élèves
    /// Gère les opérations CRUD et l'état de l'interface utilisateur
    /// </summary>
    public class EleveController
    {
        #region Enumerations

        /// <summary>
        /// Modes d'opération pour la gestion des élèves
        /// </summary>
        public enum OperationMode
        {
            /// <summary>
            /// Mode création d'un nouvel élève
            /// </summary>
            Create,
            
            /// <summary>
            /// Mode modification d'un élève existant
            /// </summary>
            Edit,
            
            /// <summary>
            /// Mode consultation d'un élève (lecture seule)
            /// </summary>
            View
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Service métier pour les opérations sur les élèves
        /// </summary>
        private readonly Eleves _elevesService;

        /// <summary>
        /// Référence vers le formulaire principal
        /// </summary>
        private readonly FormMain _formMain;

        /// <summary>
        /// Modèle de données de l'élève actuellement en cours de traitement
        /// </summary>
        private EleveViewModel? _currentEleve;

        /// <summary>
        /// Mode d'opération actuel
        /// </summary>
        private OperationMode _currentMode;

        #endregion

        #region Properties

        /// <summary>
        /// Élève actuellement sélectionné ou en cours de création
        /// </summary>
        public EleveViewModel? CurrentEleve => _currentEleve;

        /// <summary>
        /// Mode d'opération actuel
        /// </summary>
        public OperationMode CurrentMode => _currentMode;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialise une nouvelle instance du contrôleur d'élèves
        /// </summary>
        /// <param name="elevesService">Service métier pour les élèves</param>
        /// <param name="formMain">Référence vers le formulaire principal</param>
        /// <exception cref="ArgumentNullException">Si un des paramètres est null</exception>
        public EleveController(Eleves elevesService, FormMain formMain)
        {
            _elevesService = elevesService ?? throw new ArgumentNullException(nameof(elevesService));
            _formMain = formMain ?? throw new ArgumentNullException(nameof(formMain));
            _currentMode = OperationMode.Create;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialise un nouvel élève pour la création
        /// Génère automatiquement le matricule et configure l'interface en mode création
        /// </summary>
        /// <exception cref="InvalidOperationException">Si la génération du matricule échoue</exception>
        public void InitializeNewEleve()
        {
            try
            {
                // Créer un nouveau modèle d'élève
                _currentEleve = new EleveViewModel();

                // Générer le matricule unique
                var matricule = _elevesService.GenerateNewMatricule();
                _currentEleve.Matricule = matricule;

                // Définir le mode création
                SetOperationMode(OperationMode.Create);

                // Vider tous les champs de l'interface
                ClearAllFields();

                // Afficher le matricule généré
                if (_formMain.MatriculeEleveControl != null)
                {
                    _formMain.MatriculeEleveControl.Text = matricule;
                    _formMain.MatriculeEleveControl.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erreur lors de l'initialisation d'un nouvel élève: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Initialise un nouveau modèle d'élève sans générer le matricule
        /// Le matricule sera généré plus tard par l'événement Enter du champ Nom
        /// </summary>
        public void InitializeNewEleveWithoutMatricule()
        {
            try
            {
                // Créer un nouveau modèle d'élève
                _currentEleve = new EleveViewModel();

                // Définir le mode création
                SetOperationMode(OperationMode.Create);

                // Vider tous les champs de l'interface
                ClearAllFields();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erreur lors de l'initialisation d'un nouvel élève: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Charge les données d'un élève depuis la grille de sélection
        /// </summary>
        /// <param name="matricule">Matricule de l'élève à charger</param>
        /// <exception cref="ArgumentException">Si le matricule est vide ou null</exception>
        /// <exception cref="InvalidOperationException">Si l'élève n'est pas trouvé</exception>
        public void LoadEleveFromGrid(string matricule)
        {
            if (string.IsNullOrWhiteSpace(matricule))
            {
                throw new ArgumentException("Le matricule ne peut pas être vide.", nameof(matricule));
            }

            try
            {
                // Récupérer les données de l'élève depuis le service
                var eleveData = _elevesService.GetEleve(matricule);
                
                if (eleveData == null)
                {
                    throw new InvalidOperationException($"Élève avec le matricule '{matricule}' introuvable.");
                }

                // Créer le modèle de vue et le remplir avec les données
                _currentEleve = new EleveViewModel();
                MapDataToViewModel(eleveData, _currentEleve);

                // Passer en mode modification
                SetOperationMode(OperationMode.Edit);

                // Charger les données dans l'interface
                LoadDataToInterface(_currentEleve);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erreur lors du chargement de l'élève '{matricule}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Définit le mode d'opération et met à jour l'interface en conséquence
        /// </summary>
        /// <param name="mode">Mode d'opération à définir</param>
        public void SetOperationMode(OperationMode mode)
        {
            _currentMode = mode;
            UpdateInterfaceForMode(mode);
        }

        /// <summary>
        /// Met à jour les informations d'affectation de l'élève courant
        /// </summary>
        /// <param name="anneeScolaire">Année scolaire</param>
        /// <param name="codePromotion">Code de la promotion</param>
        /// <param name="nomPromotion">Nom de la promotion</param>
        /// <param name="indicePromotion">Indice de la promotion</param>
        public void UpdateAffectationInfo(string anneeScolaire, string codePromotion, string nomPromotion, string indicePromotion)
        {
            if (_currentEleve == null)
            {
                throw new InvalidOperationException("Aucun élève n'est actuellement sélectionné pour l'affectation.");
            }

            try
            {
                _currentEleve.AnneeScolaire = anneeScolaire ?? string.Empty;
                _currentEleve.CodePromotion = codePromotion ?? string.Empty;
                _currentEleve.NomPromotion = nomPromotion ?? string.Empty;
                _currentEleve.IndicePromotion = indicePromotion ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erreur lors de la mise à jour des informations d'affectation: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Vérifie si l'affectation de l'élève courant est complète
        /// </summary>
        /// <returns>True si l'affectation est complète</returns>
        public bool IsAffectationComplete()
        {
            if (_currentEleve == null)
                return false;

            return !string.IsNullOrWhiteSpace(_currentEleve.AnneeScolaire) &&
                   !string.IsNullOrWhiteSpace(_currentEleve.CodePromotion) &&
                   !string.IsNullOrWhiteSpace(_currentEleve.IndicePromotion);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Met à jour l'interface utilisateur selon le mode d'opération
        /// </summary>
        /// <param name="mode">Mode d'opération actuel</param>
        private void UpdateInterfaceForMode(OperationMode mode)
        {
            try
            {
                switch (mode)
                {
                    case OperationMode.Create:
                        SetCreateMode();
                        break;
                    case OperationMode.Edit:
                        SetEditMode();
                        break;
                    case OperationMode.View:
                        SetViewMode();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la mise à jour de l'interface: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Configure l'interface en mode création
        /// </summary>
        private void SetCreateMode()
        {
            // Activer tous les champs de saisie
            EnableAllFields(true);

            // Configurer les boutons
            if (_formMain.SaveEleveControl != null)
            {
                _formMain.SaveEleveControl.Enabled = true;
                _formMain.SaveEleveControl.Text = "Enregistrer";
            }

            // Le matricule reste en lecture seule
            if (_formMain.MatriculeEleveControl != null)
            {
                _formMain.MatriculeEleveControl.ReadOnly = true;
            }
        }

        /// <summary>
        /// Configure l'interface en mode modification
        /// </summary>
        private void SetEditMode()
        {
            // Activer tous les champs sauf le matricule
            EnableAllFields(true);

            // Configurer les boutons
            if (_formMain.SaveEleveControl != null)
            {
                _formMain.SaveEleveControl.Enabled = true;
                _formMain.SaveEleveControl.Text = "Modifier";
            }

            // Le matricule reste toujours en lecture seule
            if (_formMain.MatriculeEleveControl != null)
            {
                _formMain.MatriculeEleveControl.ReadOnly = true;
            }
        }

        /// <summary>
        /// Configure l'interface en mode consultation
        /// </summary>
        private void SetViewMode()
        {
            // Désactiver tous les champs de saisie
            EnableAllFields(false);

            // Désactiver le bouton de sauvegarde
            if (_formMain.SaveEleveControl != null)
            {
                _formMain.SaveEleveControl.Enabled = false;
            }
        }

        /// <summary>
        /// Active ou désactive tous les champs de saisie
        /// </summary>
        /// <param name="enabled">True pour activer, false pour désactiver</param>
        private void EnableAllFields(bool enabled)
        {
            // Champs de base de l'élève
            if (_formMain.NomEleveControl != null)
                _formMain.NomEleveControl.Enabled = enabled;
            
            if (_formMain.PostnomEleveControl != null)
                _formMain.PostnomEleveControl.Enabled = enabled;
            
            if (_formMain.PrenomEleveControl != null)
                _formMain.PrenomEleveControl.Enabled = enabled;
            
            if (_formMain.SexeEleveControl != null)
                _formMain.SexeEleveControl.Enabled = enabled;
            
            if (_formMain.DateNaissanceEleveControl != null)
                _formMain.DateNaissanceEleveControl.Enabled = enabled;
            
            if (_formMain.LieuNaissanceEleveControl != null)
                _formMain.LieuNaissanceEleveControl.Enabled = enabled;
            
            // Informations tuteur
            if (_formMain.NomTuteurEleveControl != null)
                _formMain.NomTuteurEleveControl.Enabled = enabled;
            
            if (_formMain.TelTuteurEleveControl != null)
                _formMain.TelTuteurEleveControl.Enabled = enabled;
            
            // Adresse
            if (_formMain.AdresseEleveControl != null)
                _formMain.AdresseEleveControl.Enabled = enabled;
            
            if (_formMain.NumParcelleEleveControl != null)
                _formMain.NumParcelleEleveControl.Enabled = enabled;
            
            if (_formMain.SetAdresseEleveControl != null)
                _formMain.SetAdresseEleveControl.Enabled = enabled;
            
            // École de provenance
            if (_formMain.EcoleProvenanceEleveControl != null)
                _formMain.EcoleProvenanceEleveControl.Enabled = enabled;
            
            // Boutons de photo
            if (_formMain.CapturePicEleveControl != null)
                _formMain.CapturePicEleveControl.Enabled = enabled;
            
            if (_formMain.LoadPicEleveControl != null)
                _formMain.LoadPicEleveControl.Enabled = enabled;
            
            // Bouton d'affectation
            if (_formMain.AffectEleveControl != null)
                _formMain.AffectEleveControl.Enabled = enabled;
        }

        /// <summary>
        /// Vide tous les champs de l'interface
        /// </summary>
        private void ClearAllFields()
        {
            // Champs de base de l'élève
            if (_formMain.MatriculeEleveControl != null)
                _formMain.MatriculeEleveControl.Text = string.Empty;
            
            if (_formMain.NomEleveControl != null)
                _formMain.NomEleveControl.Text = string.Empty;
            
            if (_formMain.PostnomEleveControl != null)
                _formMain.PostnomEleveControl.Text = string.Empty;
            
            if (_formMain.PrenomEleveControl != null)
                _formMain.PrenomEleveControl.Text = string.Empty;
            
            if (_formMain.SexeEleveControl != null)
                _formMain.SexeEleveControl.SelectedIndex = -1;
            
            if (_formMain.DateNaissanceEleveControl != null)
                _formMain.DateNaissanceEleveControl.Value = DateTime.Now;
            
            if (_formMain.LieuNaissanceEleveControl != null)
                _formMain.LieuNaissanceEleveControl.Text = string.Empty;
            
            // Informations tuteur
            if (_formMain.NomTuteurEleveControl != null)
                _formMain.NomTuteurEleveControl.Text = string.Empty;
            
            if (_formMain.TelTuteurEleveControl != null)
                _formMain.TelTuteurEleveControl.Text = string.Empty;
            
            // Adresse
            if (_formMain.AdresseEleveControl != null)
                _formMain.AdresseEleveControl.Text = string.Empty;
            
            if (_formMain.NumParcelleEleveControl != null)
                _formMain.NumParcelleEleveControl.Text = string.Empty;
            
            // École de provenance
            if (_formMain.EcoleProvenanceEleveControl != null)
                _formMain.EcoleProvenanceEleveControl.Text = string.Empty;
            
            // Vider la photo
            if (_formMain.PicBoxEleveControl != null)
                _formMain.PicBoxEleveControl.Image = null;
        }

        /// <summary>
        /// Mappe les données de la base vers le modèle de vue
        /// </summary>
        /// <param name="data">Données de la base</param>
        /// <param name="viewModel">Modèle de vue à remplir</param>
        private void MapDataToViewModel(dynamic data, EleveViewModel viewModel)
        {
            viewModel.Matricule = data.matricule ?? string.Empty;
            viewModel.Nom = data.nom ?? string.Empty;
            viewModel.PostNom = data.postnom ?? string.Empty;
            viewModel.Prenom = data.prenom ?? string.Empty;
            viewModel.Sexe = data.sexe ?? string.Empty;
            viewModel.DateNaissance = data.date_naiss;
            viewModel.LieuNaissance = data.lieu_naiss ?? string.Empty;
            viewModel.NomTuteur = data.nom_tuteur ?? string.Empty;
            viewModel.TelTuteur = data.tel_tuteur ?? string.Empty;
            viewModel.FkAvenue = data.FkAvenue ?? string.Empty;
            viewModel.NumeroAdresse = data.numero ?? string.Empty;
            viewModel.EcoleProvenance = data.ecole_prov ?? string.Empty;
            viewModel.CheminPhoto = data.profil ?? string.Empty;
        }

        /// <summary>
        /// Charge les données du modèle vers l'interface
        /// </summary>
        /// <param name="viewModel">Modèle de vue contenant les données</param>
        private void LoadDataToInterface(EleveViewModel viewModel)
        {
            // Champs de base de l'élève
            if (_formMain.MatriculeEleveControl != null)
                _formMain.MatriculeEleveControl.Text = viewModel.Matricule;
            
            if (_formMain.NomEleveControl != null)
                _formMain.NomEleveControl.Text = viewModel.Nom;
            
            if (_formMain.PostnomEleveControl != null)
                _formMain.PostnomEleveControl.Text = viewModel.PostNom;
            
            if (_formMain.PrenomEleveControl != null)
                _formMain.PrenomEleveControl.Text = viewModel.Prenom;
            
            if (_formMain.SexeEleveControl != null)
                _formMain.SexeEleveControl.Text = viewModel.Sexe;
            
            if (_formMain.DateNaissanceEleveControl != null && viewModel.DateNaissance.HasValue)
                _formMain.DateNaissanceEleveControl.Value = viewModel.DateNaissance.Value;
            
            if (_formMain.LieuNaissanceEleveControl != null)
                _formMain.LieuNaissanceEleveControl.Text = viewModel.LieuNaissance;
            
            // Informations tuteur
            if (_formMain.NomTuteurEleveControl != null)
                _formMain.NomTuteurEleveControl.Text = viewModel.NomTuteur;
            
            if (_formMain.TelTuteurEleveControl != null)
                _formMain.TelTuteurEleveControl.Text = viewModel.TelTuteur;
            
            // Adresse
            if (_formMain.AdresseEleveControl != null)
                _formMain.AdresseEleveControl.Text = viewModel.AdresseComplete;
            
            if (_formMain.NumParcelleEleveControl != null)
                _formMain.NumParcelleEleveControl.Text = viewModel.NumeroAdresse;
            
            // École de provenance
            if (_formMain.EcoleProvenanceEleveControl != null)
                _formMain.EcoleProvenanceEleveControl.Text = viewModel.EcoleProvenance;
            
            // Charger la photo si elle existe
            if (_formMain.PicBoxEleveControl != null && !string.IsNullOrEmpty(viewModel.CheminPhoto))
            {
                try
                {
                    if (System.IO.File.Exists(viewModel.CheminPhoto))
                    {
                        _formMain.PicBoxEleveControl.Image = System.Drawing.Image.FromFile(viewModel.CheminPhoto);
                    }
                }
                catch (Exception ex)
                {
                    // Log l'erreur mais ne pas interrompre le chargement
                    System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement de la photo: {ex.Message}");
                }
            }
        }

        #endregion
    }


}