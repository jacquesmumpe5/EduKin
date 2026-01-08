using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using EduKin.Inits;
using EduKinContext = EduKin.Inits.EduKinContext;
using EduKin.Layouts;

namespace EduKin.Csharp.Admins
{
    /// <summary>
    /// Contrôleur centralisé pour la gestion des agents
    /// Gère les opérations CRUD et l'état de l'interface utilisateur
    /// </summary>
    public class AgentController
    {
        #region Enumerations

        /// <summary>
        /// Modes d'opération pour la gestion des agents
        /// </summary>
        public enum OperationMode
        {
            /// <summary>
            /// Mode création d'un nouvel agent
            /// </summary>
            Create,
            
            /// <summary>
            /// Mode modification d'un agent existant
            /// </summary>
            Edit,
            
            /// <summary>
            /// Mode consultation d'un agent (lecture seule)
            /// </summary>
            View
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Service métier pour les opérations sur les agents
        /// </summary>
        private readonly Agents _agentsService;

        /// <summary>
        /// Référence vers le formulaire principal
        /// </summary>
        private readonly FormMain _formMain;

        /// <summary>
        /// Modèle de données de l'agent actuellement en cours de traitement
        /// </summary>
        private AgentViewModel? _currentAgent;

        /// <summary>
        /// Mode d'opération actuel
        /// </summary>
        private OperationMode _currentMode;

        #endregion

        #region Properties

        /// <summary>
        /// Agent actuellement sélectionné ou en cours de création
        /// </summary>
        public AgentViewModel? CurrentAgent => _currentAgent;

        /// <summary>
        /// Mode d'opération actuel
        /// </summary>
        public OperationMode CurrentMode => _currentMode;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialise une nouvelle instance du contrôleur d'agents
        /// </summary>
        /// <param name="agentsService">Service métier pour les agents</param>
        /// <param name="formMain">Référence vers le formulaire principal</param>
        /// <exception cref="ArgumentNullException">Si un des paramètres est null</exception>
        public AgentController(Agents agentsService, FormMain formMain)
        {
            _agentsService = agentsService ?? throw new ArgumentNullException(nameof(agentsService));
            _formMain = formMain ?? throw new ArgumentNullException(nameof(formMain));
            _currentMode = OperationMode.Create;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialise un nouvel agent pour la création
        /// Génère automatiquement le matricule et configure l'interface en mode création
        /// </summary>
        /// <exception cref="InvalidOperationException">Si la génération du matricule échoue</exception>
        public void InitializeNewAgent()
        {
            try
            {
                // Créer un nouveau modèle d'agent
                _currentAgent = new AgentViewModel();

                // Générer le matricule unique avec le user_index de l'utilisateur connecté
                var adminService = new Administrations();
                var userIndex = EduKinContext.CurrentUserIndex; // ✅ Récupération dynamique depuis la base de données
                var matricule = adminService.GenerateId("t_agents", "matricule", "AGT", userIndex);
                _currentAgent.Matricule = matricule;

                // Définir l'école courante
                _currentAgent.IdEcole = EduKinContext.CurrentIdEcole;

                // Définir le mode création
                SetOperationMode(OperationMode.Create);

                // Vider tous les champs de l'interface
                ClearAllFields();

                // Afficher le matricule généré
                if (_formMain.MatriculeAgentControl != null)
                {
                    _formMain.MatriculeAgentControl.Text = matricule;
                    _formMain.MatriculeAgentControl.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erreur lors de l'initialisation d'un nouvel agent: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Charge les données d'un agent depuis la grille de sélection
        /// </summary>
        /// <param name="matricule">Matricule de l'agent à charger</param>
        /// <exception cref="ArgumentException">Si le matricule est vide ou null</exception>
        /// <exception cref="InvalidOperationException">Si l'agent n'est pas trouvé</exception>
        public void LoadAgentFromGrid(string matricule)
        {
            if (string.IsNullOrWhiteSpace(matricule))
            {
                throw new ArgumentException("Le matricule ne peut pas être vide.", nameof(matricule));
            }

            try
            {
                // Récupérer les données de l'agent depuis le service
                var agentData = _agentsService.GetAgent(matricule);
                
                if (agentData == null)
                {
                    throw new InvalidOperationException($"Agent avec le matricule '{matricule}' introuvable.");
                }

                // Créer le modèle de vue et le remplir avec les données
                _currentAgent = new AgentViewModel();
                MapDataToViewModel(agentData, _currentAgent);

                // Passer en mode modification
                SetOperationMode(OperationMode.Edit);

                // Charger les données dans l'interface
                LoadDataToInterface(_currentAgent);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erreur lors du chargement de l'agent '{matricule}': {ex.Message}", ex);
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
        /// Valide les données de l'agent courant
        /// </summary>
        /// <returns>True si les données sont valides</returns>
        public bool ValidateCurrentAgent()
        {
            if (_currentAgent == null)
            {
                return false;
            }

            return _currentAgent.IsValid() && _currentAgent.ValidateSalaryInfo() && _currentAgent.ValidateAge();
        }

        /// <summary>
        /// Obtient les erreurs de validation de l'agent courant
        /// </summary>
        /// <returns>Liste des erreurs de validation</returns>
        public List<string> GetValidationErrors()
        {
            if (_currentAgent == null)
            {
                return new List<string> { "Aucun agent sélectionné" };
            }

            var errors = _currentAgent.GetValidationErrors();

            if (!_currentAgent.ValidateSalaryInfo())
            {
                errors.Add("Les informations salariales ne sont pas cohérentes");
            }

            if (!_currentAgent.ValidateAge())
            {
                errors.Add("L'âge de l'agent doit être entre 18 et 70 ans");
            }

            return errors;
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
            if (_formMain.SaveAgentControl != null)
            {
                _formMain.SaveAgentControl.Enabled = true;
                _formMain.SaveAgentControl.Text = "Enregistrer";
            }

            // Le matricule reste en lecture seule
            if (_formMain.MatriculeAgentControl != null)
            {
                _formMain.MatriculeAgentControl.ReadOnly = true;
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
            if (_formMain.UpdateAgentControl != null)
            {
                _formMain.UpdateAgentControl.Enabled = true;
                _formMain.UpdateAgentControl.Text = "Modifier";
            }

            // Le matricule reste toujours en lecture seule
            if (_formMain.MatriculeAgentControl != null)
            {
                _formMain.MatriculeAgentControl.ReadOnly = true;
            }
        }

        /// <summary>
        /// Configure l'interface en mode consultation
        /// </summary>
        private void SetViewMode()
        {
            // Désactiver tous les champs de saisie
            EnableAllFields(false);

            // Désactiver les boutons de modification
            if (_formMain.SaveAgentControl != null)
            {
                _formMain.SaveAgentControl.Enabled = false;
            }

            if (_formMain.UpdateAgentControl != null)
            {
                _formMain.UpdateAgentControl.Enabled = false;
            }
        }

        /// <summary>
        /// Active ou désactive tous les champs de saisie
        /// </summary>
        /// <param name="enabled">True pour activer, false pour désactiver</param>
        private void EnableAllFields(bool enabled)
        {
            // Champs de base de l'agent
            if (_formMain.NomAgentControl != null)
                _formMain.NomAgentControl.Enabled = enabled;
            
            if (_formMain.PostnomAgentControl != null)
                _formMain.PostnomAgentControl.Enabled = enabled;
            
            if (_formMain.PrenomAgentControl != null)
                _formMain.PrenomAgentControl.Enabled = enabled;
            
            if (_formMain.SexeAgentControl != null)
                _formMain.SexeAgentControl.Enabled = enabled;
            
            if (_formMain.DateNaissanceAgentControl != null)
                _formMain.DateNaissanceAgentControl.Enabled = enabled;
            
            if (_formMain.LieuNaissanceAgentControl != null)
                _formMain.LieuNaissanceAgentControl.Enabled = enabled;
            
            
            // Informations de contact
            if (_formMain.EmailAgentControl != null)
                _formMain.EmailAgentControl.Enabled = enabled;
            
            if (_formMain.TelAgentControl != null)
                _formMain.TelAgentControl.Enabled = enabled;
            
            if (_formMain.AdresseAgentControl != null)
                _formMain.AdresseAgentControl.Enabled = enabled;
          
            // Boutons de photo
            if (_formMain.CapturePicAgentControl != null)
                _formMain.CapturePicAgentControl.Enabled = enabled;
            
            if (_formMain.LoadPicAgentControl != null)
                _formMain.LoadPicAgentControl.Enabled = enabled;
        }

        /// <summary>
        /// Vide tous les champs de l'interface
        /// </summary>
        private void ClearAllFields()
        {
            // Champs de base de l'agent
            if (_formMain.MatriculeAgentControl != null)
                _formMain.MatriculeAgentControl.Text = string.Empty;
            
            if (_formMain.NomAgentControl != null)
                _formMain.NomAgentControl.Text = string.Empty;
            
            if (_formMain.PostnomAgentControl != null)
                _formMain.PostnomAgentControl.Text = string.Empty;
            
            if (_formMain.PrenomAgentControl != null)
                _formMain.PrenomAgentControl.Text = string.Empty;
            
            if (_formMain.SexeAgentControl != null)
                _formMain.SexeAgentControl.SelectedIndex = -1;
            
            if (_formMain.DateNaissanceAgentControl != null)
                _formMain.DateNaissanceAgentControl.Value = DateTime.Now.AddYears(-25);
            
            if (_formMain.LieuNaissanceAgentControl != null)
                _formMain.LieuNaissanceAgentControl.Text = string.Empty;
            
           
            
            // Informations de contact
            if (_formMain.EmailAgentControl != null)
                _formMain.EmailAgentControl.Text = string.Empty;
            
            if (_formMain.TelAgentControl != null)
                _formMain.TelAgentControl.Text = string.Empty;
            
            if (_formMain.AdresseAgentControl != null)
                _formMain.AdresseAgentControl.Text = string.Empty;

            // Vider la photo
            if (_formMain.PicBoxAgentControl != null)
                _formMain.PicBoxAgentControl.Image = null;
        }

        /// <summary>
        /// Mappe les données de la base vers le modèle de vue
        /// </summary>
        /// <param name="data">Données de la base</param>
        /// <param name="viewModel">Modèle de vue à remplir</param>
        private void MapDataToViewModel(dynamic data, AgentViewModel viewModel)
        {
            viewModel.Matricule = data.matricule ?? string.Empty;
            viewModel.Nom = data.nom ?? string.Empty;
            viewModel.PostNom = data.postnom ?? string.Empty;
            viewModel.Prenom = data.prenom ?? string.Empty;
            viewModel.Sexe = data.sexe ?? string.Empty;
            viewModel.DateNaissance = data.date_naiss ?? DateTime.Now.AddYears(-25);
            viewModel.LieuNaissance = data.lieu_naiss ?? string.Empty;
            viewModel.IdEcole = data.id_ecole ?? string.Empty;
            viewModel.Email = data.email;
            viewModel.Telephone = data.tel;
            viewModel.FkAvenue = data.FkAvenue;
            viewModel.Numero = data.Numero;
            viewModel.SalaireBase = data.sal_base ?? 0;
            viewModel.Prime = data.prime ?? 0;
            viewModel.Cnss = data.cnss ?? 0;
            viewModel.Ipr = data.ipr ?? 0;
            viewModel.SalaireNet = data.sal_net ?? 0;
            viewModel.CheminPhoto = data.profil;
            viewModel.DateCreation = data.created_at ?? DateTime.Now;
            viewModel.DateModification = data.updated_at ?? DateTime.Now;
        }

        /// <summary>
        /// Charge les données du modèle vers l'interface
        /// </summary>
        /// <param name="viewModel">Modèle de vue contenant les données</param>
        private void LoadDataToInterface(AgentViewModel viewModel)
        {
            // Champs de base de l'agent
            if (_formMain.MatriculeAgentControl != null)
                _formMain.MatriculeAgentControl.Text = viewModel.Matricule;
            
            if (_formMain.NomAgentControl != null)
                _formMain.NomAgentControl.Text = viewModel.Nom;
            
            if (_formMain.PostnomAgentControl != null)
                _formMain.PostnomAgentControl.Text = viewModel.PostNom;
            
            if (_formMain.PrenomAgentControl != null)
                _formMain.PrenomAgentControl.Text = viewModel.Prenom;
            
            if (_formMain.SexeAgentControl != null)
                _formMain.SexeAgentControl.Text = viewModel.Sexe;
            
            if (_formMain.DateNaissanceAgentControl != null)
                _formMain.DateNaissanceAgentControl.Value = viewModel.DateNaissance;
            
            if (_formMain.LieuNaissanceAgentControl != null)
                _formMain.LieuNaissanceAgentControl.Text = viewModel.LieuNaissance;
            
            
            // Informations de contact
            if (_formMain.EmailAgentControl != null)
                _formMain.EmailAgentControl.Text = viewModel.Email ?? string.Empty;
            
            if (_formMain.TelAgentControl != null)
                _formMain.TelAgentControl.Text = viewModel.Telephone ?? string.Empty;
            
            if (_formMain.AdresseAgentControl != null)
                _formMain.AdresseAgentControl.Text = $"{viewModel.FkAvenue} - {viewModel.Numero}";
            
           
            
            // Charger la photo si elle existe
            if (_formMain.PicBoxAgentControl != null && !string.IsNullOrEmpty(viewModel.CheminPhoto))
            {
                try
                {
                    if (System.IO.File.Exists(viewModel.CheminPhoto))
                    {
                        _formMain.PicBoxAgentControl.Image = System.Drawing.Image.FromFile(viewModel.CheminPhoto);
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