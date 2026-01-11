using EduKin.Csharp.Securites;
using EduKin.DataSets;
using EduKin.Csharp.Admins;
using EduKin.Inits;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dapper;
using EduKinContext = EduKin.Inits.EduKinContext; // Résoudre le conflit de noms

namespace EduKin.Layouts
{
    public partial class FormMain : Form
    {
        #region Private Fields
        private DashBoard_Accueil _dashboardService;
        private Eleves _elevesService;
        private Agents _agentsService;
        private Administrations _administrations;
        private EleveController _eleveController;
        private AgentController _agentController;
        private string _currentSchoolYear, numParcelle;
        private string _selectedIdAvenueAgent; // Store selected avenue ID for agent
        private string _selectedIdAvenueEleve; // Store selected avenue ID for eleve
        private string _selectedNumParcelleAgent; // Store selected parcel number for agent
        private string _selectedPhotoPathAgent; // Store selected photo path for agent
        private string _selectedPhotoPathEleve; // Store selected photo path for eleve

        // Assignment information storage
        private string _selectedAnneeScolaire = string.Empty;
        private string _selectedCodePromotion = string.Empty;
        private string _selectedNomPromotion = string.Empty;
        private string _selectedIndicePromotion = string.Empty;

        // Agent affectation information storage
        private string? _selectedServiceAgent;
        private string? _selectedGradeAgent;
        private string? _selectedRoleAgent;
        private string? _selectedFonctionAgent;
        private decimal _selectedSalBaseAgent;
        private decimal _selectedPrimeAgent;
        private decimal _selectedCnssAgent;
        private decimal _selectedIprAgent;
        private decimal _selectedSalNetAgent;

        #endregion

        #region Public Properties for Data Access

        /// <summary>
        /// Gets the selected IdAvenue for eleve database recording
        /// </summary>
        public string SelectedIdAvenueEleve => _selectedIdAvenueEleve;

        /// <summary>
        /// Gets the selected assignment information for database recording
        /// </summary>
        public string SelectedAnneeScolaire => _selectedAnneeScolaire;
        public string SelectedCodePromotion => _selectedCodePromotion;
        public string SelectedNomPromotion => _selectedNomPromotion;
        public string SelectedIndicePromotion => _selectedIndicePromotion;

        /// <summary>
        /// Gets the selected agent affectation information
        /// </summary>
        public string? ServiceAgent => _selectedServiceAgent;
        public string? GradeAgent => _selectedGradeAgent;
        public string? RoleAgent => _selectedRoleAgent;
        public string? FonctionAgent => _selectedFonctionAgent;
        public decimal SalBaseAgent => _selectedSalBaseAgent;
        public decimal IprAgent => _selectedIprAgent;
        public decimal SalNetAgent => _selectedSalNetAgent;

        #endregion

        public FormMain()
        {
            InitializeComponent();
            ValidateContexts();
            InitializeServices();
            InitializeBannerLabels();
        }


        /// <summary>
        /// Valide que les contextes utilisateur et école sont correctement initialisés
        /// </summary>
        private void ValidateContexts()
        {
            try
            {
                // Vérifier que le contexte utilisateur est initialisé
                if (!EduKinContext.IsAuthenticated)
                {
                    throw new InvalidOperationException("Le contexte utilisateur n'est pas initialisé. L'utilisateur doit être connecté.");
                }

                // Vérifier que le contexte école est initialisé
                if (!EduKinContext.IsConfigured)
                {
                    throw new InvalidOperationException("Le contexte école n'est pas initialisé. L'école doit être configurée.");
                }

                // Afficher les informations de contexte dans le titre de la fenêtre
                // this.Text = $"EduKin - {ApplicationContext.CurrentDenomination} - {ApplicationContext.CurrentUserName} ({ApplicationContext.CurrentUserRole})";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de contexte: {ex.Message}",
                    "Erreur d'initialisation", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Fermer l'application si les contextes ne sont pas valides
                Application.Exit();
            }
        }

        /// <summary>
        /// Handles form load event - initializes default panel and loads initial data
        /// </summary>
        private async void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                // Initialize default panel (Accueil)
                await InitializeDefaultPanel();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de la forme: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Wires up event handlers for UI components
        /// </summary>


        #region Initialization

        /// <summary>
        /// Initializes business logic services
        /// </summary>
        private void InitializeServices()
        {
            try
            {
                _dashboardService = new DashBoard_Accueil();
                _elevesService = new Eleves();
                _agentsService = new Agents();
                _administrations = new Administrations();
                _eleveController = new EleveController(_elevesService, this);
                _agentController = new AgentController(_agentsService, this);
                _currentSchoolYear = _dashboardService.GetCurrentSchoolYear();

                // Initialiser les ComboBox
                InitializeComboBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'initialisation des services: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Initialise les ComboBox avec leurs valeurs
        /// </summary>
        private void InitializeComboBoxes()
        {
            try
            {
                // Initialiser le ComboBox des sexes pour les élèves
                CmbSexeEleve.Items.Clear();
                CmbSexeEleve.Items.Add("M");
                CmbSexeEleve.Items.Add("F");
                CmbSexeEleve.SelectedIndex = -1; // Aucune sélection par défaut

                // Initialiser le ComboBox des sexes pour les agents
                CmbSexeAgent.Items.Clear();
                CmbSexeAgent.Items.Add("M");
                CmbSexeAgent.Items.Add("F");
                CmbSexeAgent.SelectedIndex = -1;



            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de l'initialisation des ComboBox: {ex.Message}");
            }
        }

        /// <summary>
        /// Initialise les labels de la bannière avec les informations du contexte
        /// </summary>
        private void InitializeBannerLabels()
        {
            try
            {
                // Afficher le nom d'utilisateur et son rôle
                if (EduKinContext.IsAuthenticated)
                {
                    lblUsername.Text = $"Utilisateur: {EduKinContext.CurrentUserName} ({EduKinContext.CurrentUserRole})";
                }
                else
                {
                    lblUsername.Text = "Utilisateur: Non connecté";
                }

                // Afficher le nom de l'école
                if (EduKinContext.IsConfigured)
                {
                    lblEcole.Text = $"École: {EduKinContext.CurrentDenomination}";

                    // Récupérer et afficher l'adresse de l'école
                    try
                    {
                        var ecoleInfo = _administrations.GetEcole(EduKinContext.CurrentIdEcole);
                        if (ecoleInfo != null)
                        {
                            // Construire l'adresse complète en utilisant le service Eleves
                            var adresse = _elevesService.GetAdresseComplete(
                                ecoleInfo.FkAvenue?.ToString(),
                                ecoleInfo.numero?.ToString()
                            );
                            lblAdresseEcole.Text = $"Adresse: {adresse}";
                        }
                        else
                        {
                            lblAdresseEcole.Text = "Adresse: Non disponible";
                        }
                    }
                    catch
                    {
                        lblAdresseEcole.Text = "Adresse: Non disponible";
                    }
                }
                else
                {
                    lblEcole.Text = "École: Non configurée";
                    lblAdresseEcole.Text = "Adresse: Non disponible";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de l'initialisation des labels de bannière: {ex.Message}");
                lblUsername.Text = "Utilisateur: Erreur";
                lblEcole.Text = "École: Erreur";
                lblAdresseEcole.Text = "Adresse: Erreur";
            }
        }

        #endregion

        #region Navigation and Panel Switching

        /// <summary>
        /// Handles btnAccueil click event - switches to dashboard panel
        /// </summary>
        public async void BtnAccueil_Click(object sender, EventArgs e)
        {
            try
            {
                // Switch to dashboard panel
                SwitchToPanel("Accueil");

                // Update navigation button states
                UpdateNavigationButtonStates(sender as Control);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du tableau de bord: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles btnEleves click event - switches to student management panel
        /// </summary>
        public async void BtnEleves_Click(object sender, EventArgs e)
        {
            try
            {
                // Switch to student management panel
                SwitchToPanel("Eleves");

                // Update navigation button states
                UpdateNavigationButtonStates(sender as Control);

                // Load students data when switching to the panel
                await LoadElevesData();

                // Initialize a new student model if not already initialized
                // Note: Matricule will be generated when user enters TxtNomEleve field
                if (_eleveController.CurrentEleve == null)
                {
                    _eleveController.InitializeNewEleveWithoutMatricule();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de la gestion des élèves: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles btnAgents click event - switches to agents management panel
        /// </summary>
        public async void BtnAgents_Click(object sender, EventArgs e)
        {
            try
            {
                // Switch to agents management panel
                SwitchToPanel("Agents");

                // Update navigation button states
                UpdateNavigationButtonStates(sender as Control);

                // Load agents data when switching to the panel
                await LoadAgentsData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de la gestion des agents: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles btnAdministration click event - opens FormAdmins for authorized users only
        /// </summary>
        public void BtnAdministration_Click(object sender, EventArgs e)
        {
            try
            {
                // Debugging détaillé
                System.Diagnostics.Debug.WriteLine("=== DEBUT VERIFICATION ACCES ADMINISTRATION ===");
                System.Diagnostics.Debug.WriteLine($"ApplicationContext.IsAuthenticated: {EduKinContext.IsAuthenticated}");

                if (EduKinContext.IsAuthenticated)
                {
                    System.Diagnostics.Debug.WriteLine($"ApplicationContext.CurrentUserName: {EduKinContext.CurrentUserName}");
                    System.Diagnostics.Debug.WriteLine($"ApplicationContext.CurrentUserRole: '{EduKinContext.CurrentUserRole}'");
                }

                // Vérifier les permissions d'accès
                if (!HasAdministrationAccess())
                {
                    var currentRole = EduKinContext.IsAuthenticated ? EduKinContext.CurrentUserRole : "Non connecté";
                    MessageBox.Show(
                        $"Accès refusé. Vous n'avez pas les permissions nécessaires pour accéder à l'administration.\n\n" +
                        $"Votre rôle actuel : '{currentRole}'\n\n" +
                        "Seuls les utilisateurs avec les rôles suivants peuvent accéder à cette section :\n" +
                        "• Super Administrateur\n" +
                        "• Administrateur\n" +
                        "• Directeur",
                        "Accès refusé",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // Cacher FormMain avant d'ouvrir FormAdmins
                this.Hide();

                // Ouvrir le formulaire d'administration
                using (var adminForm = new FormAdmins())
                {
                    adminForm.ShowDialog(this);
                }

                // Réafficher FormMain après la fermeture de FormAdmins
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture du module d'administration: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // S'assurer que FormMain est visible en cas d'erreur
                this.Show();
            }
        }

        /// <summary>
        /// Vérifie si l'utilisateur actuel a accès au module d'administration
        /// </summary>
        /// <returns>True si l'utilisateur a les permissions nécessaires</returns>
        private bool HasAdministrationAccess()
        {
            try
            {
                // Vérifier que l'utilisateur est authentifié
                if (!EduKinContext.IsAuthenticated)
                {
                    return false;
                }

                // Récupérer le rôle de l'utilisateur actuel pour debugging
                var currentUserRole = EduKinContext.CurrentUserRole;
                System.Diagnostics.Debug.WriteLine($"ADMIN ACCESS: Rôle utilisateur actuel = '{currentUserRole}'");

                // Utiliser la méthode IsAdmin() d'ApplicationContext qui est plus fiable
                var hasAccess = EduKinContext.IsAdmin();
                System.Diagnostics.Debug.WriteLine($"ADMIN ACCESS: Accès autorisé = {hasAccess}");

                return hasAccess;
            }
            catch (Exception ex)
            {
                // En cas d'erreur, refuser l'accès par sécurité
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la vérification des permissions: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Handles btnFinance click event - opens FormFinance
        /// </summary>
        public void BtnFinance_Click(object sender, EventArgs e)
        {
            try
            {
                // Vérifier si le contexte de l'année scolaire est initialisé
                try
                {
                    // Cette propriété lance une exception si le contexte n'est pas initialisé
                    var checkAnnee = EduKinContext.CurrentIdAnnee;
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("Le module Finance nécessite une année scolaire active.\n" +
                                    "Le contexte de l'année scolaire n'est pas initialisé.\n\n" +
                                    "Veuillez contacter un administrateur pour configurer et activer une année scolaire.",
                        "Configuration requise", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                UpdateNavigationButtonStates(sender as Control);

                // Cacher FormMain avant d'ouvrir FormFinances
                this.Hide();

                // Ouvrir le formulaire de finances
                using (var financeForm = new FormFinances())
                {
                    financeForm.ShowDialog(this);
                }

                // Réafficher FormMain après la fermeture
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture du module Finance: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
            }
        }

        /// <summary>
        /// Switches between different panels and updates navigation label
        /// </summary>
        /// <param name="panelName">Name of the panel to switch to</param>
        private void SwitchToPanel(string panelName)
        {
            // Hide all panels first
            panelNavAccueil.Visible = false;
            panelNavEleves.Visible = false;
            panelNavAgents.Visible = false;
            panelNavDelib.Visible = false;
            panelNavPedag.Visible = false;

            // Show the selected panel and update navigation label
            switch (panelName)
            {
                case "Accueil":
                    panelNavAccueil.Visible = true;
                    lblNavPanel.Text = "Accueil";
                    break;
                case "Eleves":
                    panelNavEleves.Visible = true;
                    lblNavPanel.Text = "Élèves";
                    break;
                case "Agents":
                    panelNavAgents.Visible = true;
                    lblNavPanel.Text = "Agents";
                    break;
                case "Administration":
                    FormAdmins fAdm;
                    break;
                case "Deliberation":
                    panelNavDelib.Visible = true;
                    lblNavPanel.Text = "Délibération";
                    break;
                case "Pedagogie":
                    panelNavPedag.Visible = true;
                    lblNavPanel.Text = "Pédagogie";
                    break;
                default:
                    panelNavAccueil.Visible = true;
                    lblNavPanel.Text = "Accueil";
                    break;
            }
        }

        /// <summary>
        /// Updates navigation button states to show active button
        /// </summary>
        /// <param name="activeButton">The button that was clicked</param>
        private void UpdateNavigationButtonStates(Control activeButton)
        {
            // Reset all navigation buttons to default state
            BtnAccueil.Checked = false;
            BtnEleves.Checked = false;
            BtnAgents.Checked = false;
            btnAdministration.Checked = false;
            btnDeliberation.Checked = false;
            btnPedagogie.Checked = false;

            // Set the active button to checked state
            if (activeButton != null)
            {
                if (activeButton == BtnAccueil)
                    BtnAccueil.Checked = true;
                else if (activeButton == BtnEleves)
                    BtnEleves.Checked = true;
                else if (activeButton == BtnAgents)
                    BtnAgents.Checked = true;
                else if (activeButton == btnAdministration)
                    btnAdministration.Checked = true;
                else if (activeButton == btnDeliberation)
                    btnDeliberation.Checked = true;
                else if (activeButton == btnPedagogie)
                    btnPedagogie.Checked = true;
            }
        }

        /// <summary>
        /// Initializes the form by showing the default panel (Accueil)
        /// </summary>
        public async Task InitializeDefaultPanel()
        {
            try
            {
                // Show dashboard panel by default
                SwitchToPanel("Accueil");
                BtnAccueil.Checked = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'initialisation du panneau par défaut: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion



        #region Address Search Integration

        /// <summary>
        /// Opens FormAddressSearch dialog for student address selection
        /// </summary>
        /// <returns>Tuple containing IdAvenue (FkAvenue) and numero, or null if cancelled</returns>
        public (string IdAvenue, string Numero)? OpenAddressSearchDialog()
        {
            try
            {
                using (var addressForm = new FormAddressSearch())
                {
                    // Set initial values if needed
                    // addressForm.SetInitialAddress("", "", "", "", "Kinshasa");

                    if (addressForm.ShowDialog(this) == DialogResult.OK)
                    {
                        // Get the selected avenue information
                        var selectedAvenue = addressForm.SelectedAvenue;
                        var selectedQuartier = addressForm.SelectedQuartier;
                        var selectedCommune = addressForm.SelectedCommune;
                        var selectedVille = addressForm.SelectedVille;
                        var selectedProvince = addressForm.SelectedProvince;

                        // For now, we'll use the full address as IdAvenue
                        // In a real implementation, you would need to get the actual IdEntite from the database
                        var idAvenue = GetAvenueIdFromAddress(selectedAvenue, selectedQuartier, selectedCommune, selectedVille, selectedProvince);

                        // Get numero from user input (you might want to add a numero field to FormAddressSearch)
                        var numero = PromptForNumero();

                        return (idAvenue, numero);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la recherche d'adresse: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        /// <summary>
        /// Gets the IdAvenue from the database based on address components
        /// This is a placeholder - in real implementation, you would query t_entite_administrative
        /// </summary>
        private string GetAvenueIdFromAddress(string avenue, string quartier, string commune, string ville, string province)
        {
            try
            {
                using (var conn = Connexion.Instance.GetConnection())
                {
                    var query = @"
                        SELECT IdEntite 
                        FROM t_entite_administrative 
                        WHERE IntituleEntite = @Avenue 
                        AND Fk_EntiteMere IN (
                            SELECT IdEntite FROM t_entite_administrative WHERE IntituleEntite = @Quartier
                        )
                        LIMIT 1";

                    var result = conn.QueryFirstOrDefault<string>(query, new { Avenue = avenue, Quartier = quartier });
                    return result ?? "";
                }
            }
            catch (Exception ex)
            {
                // Log error and return empty string
                System.Diagnostics.Debug.WriteLine($"Error getting avenue ID: {ex.Message}");
                return "";
            }
        }

        /// <summary>
        /// Prompts user for street number
        /// </summary>
        private string? PromptForNumero()
        {


            // Simple input dialog for numero
            if (TxtNumParcelleEleve.Text.Trim() != "")
            {
                numParcelle = TxtNumParcelleEleve.Text.Trim();
                return numParcelle;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Handles the address search button click event
        /// </summary>
        public void HandleAddressSearchButtonClick()
        {
            var addressResult = OpenAddressSearchDialog();

            if (addressResult.HasValue)
            {
                var (idAvenue, numero) = addressResult.Value;

                // Here you would typically update the student creation form with the selected address
                // For now, we'll just show a message with the selected information
                var message = $"Adresse sélectionnée:\nIdAvenue: {idAvenue}\nNuméro: {numero}";
                MessageBox.Show(message, "Adresse sélectionnée", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // TODO: Update student creation form fields with selected address
                // This would be implemented when you create the student creation dialog
            }
        }

        /// <summary>
        /// Creates a new student with address information
        /// </summary>
        /// <param name="nom">Student's last name</param>
        /// <param name="postNom">Student's middle name</param>
        /// <param name="prenom">Student's first name</param>
        /// <param name="sexe">Student's gender (M/F)</param>
        /// <param name="nomTuteur">Guardian's name</param>
        /// <param name="userIndex">User index for ID generation</param>
        /// <param name="fkAvenue">Avenue ID from address search</param>
        /// <param name="numero">Street number from address search</param>
        /// <param name="lieuNaiss">Place of birth (optional)</param>
        /// <param name="dateNaiss">Date of birth (optional)</param>
        /// <param name="telTuteur">Guardian's phone (optional)</param>
        /// <param name="ecoleProv">Previous school (optional)</param>
        /// <param name="profil">Profile picture path (optional)</param>
        /// <returns>True if student was created successfully</returns>
        public async Task<bool> CreateStudentWithAddress(
            string nom, string postNom, string prenom, string sexe, string nomTuteur, string userIndex,
            string? fkAvenue = null, string? numero = null, string? lieuNaiss = null, DateTime? dateNaiss = null,
            string? telTuteur = null, string? ecoleProv = null, string? profil = null)
        {
            try
            {
                var success = _elevesService.CreateEleve(
                    nom: nom,
                    postNom: postNom,
                    prenom: prenom,
                    sexe: sexe,
                    nomTuteur: nomTuteur,
                    lieuNaiss: lieuNaiss ?? "",
                    fkAvenue: fkAvenue ?? "",
                    numero: numero ?? "",
                    userIndex: userIndex,
                    dateNaiss: dateNaiss,
                    telTuteur: telTuteur,
                    ecoleProv: ecoleProv,
                    profil: profil);

                if (success)
                {
                    MessageBox.Show("Élève créé avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

                return success;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création de l'élève: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion

        #region Photo Management Integration

        /// <summary>
        /// Loads an existing photo into the PictureBox from a file path
        /// </summary>
        /// <param name="photoPath">Path to the photo file</param>
        /// <returns>True if photo was loaded successfully</returns>
        public bool LoadExistingPhoto(string photoPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(photoPath))
                {
                    ClearPhoto();
                    return false;
                }

                // Create PictureManager instance
                var pictureManager = new PictureManager("Photos/Eleves");

                // Load the photo into the PictureBox
                if (pictureManager.LoadPicture(PicBoxEleve, photoPath))
                {
                    // Store the photo path for database recording
                    _selectedPhotoPathEleve = photoPath;
                    return true;
                }
                else
                {
                    ClearPhoto();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de la photo existante: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearPhoto();
                return false;
            }
        }

        /// <summary>
        /// Clears the current photo from the PictureBox and resets the selected photo path
        /// </summary>
        public void ClearPhoto()
        {
            try
            {
                // Clear the PictureBox
                if (PicBoxEleve.Image != null)
                {
                    var oldImage = PicBoxEleve.Image;
                    PicBoxEleve.Image = null;
                    oldImage.Dispose();
                }

                // Reset the selected photo path
                _selectedPhotoPathEleve = string.Empty;

                // Set PictureBox to show a placeholder or default state
                PicBoxEleve.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de l'effacement de la photo: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the current photo as a Bitmap (useful for saving to database as BLOB)
        /// </summary>
        /// <returns>Current photo as Bitmap or null if no photo</returns>
        public Bitmap? GetCurrentPhotoBitmap()
        {
            try
            {
                if (PicBoxEleve.Image != null)
                {
                    return new Bitmap(PicBoxEleve.Image);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération de la photo: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Student Assignment Management

        /// <summary>
        /// Stores the assignment information from FormAffectEleves for later use during save
        /// </summary>
        /// <param name="affectForm">The assignment form containing the selected values</param>
        private void StoreAffectationInfo(FormAffectEleve affectForm)
        {
            try
            {
                _selectedAnneeScolaire = affectForm.SelectedAnneeScolaire ?? string.Empty;
                _selectedCodePromotion = affectForm.SelectedCodePromotion ?? string.Empty;
                _selectedNomPromotion = affectForm.SelectedNomPromotion ?? string.Empty;
                _selectedIndicePromotion = affectForm.SelectedIndicePromotion ?? string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du stockage des informations d'affectation: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Clears all stored assignment information
        /// </summary>
        public void ClearAffectationInfo()
        {
            _selectedAnneeScolaire = string.Empty;
            _selectedCodePromotion = string.Empty;
            _selectedNomPromotion = string.Empty;
            _selectedIndicePromotion = string.Empty;
        }

        /// <summary>
        /// Checks if assignment information is complete and valid
        /// </summary>
        /// <returns>True if assignment is complete</returns>
        public bool IsAffectationComplete()
        {
            return !string.IsNullOrWhiteSpace(_selectedAnneeScolaire) &&
                   !string.IsNullOrWhiteSpace(_selectedCodePromotion) &&
                   !string.IsNullOrWhiteSpace(_selectedIndicePromotion);
        }

        #endregion

        #region Student Data Loading and Display

        /// <summary>
        /// Loads all students data from the database and displays them in DataGridViewEleve
        /// </summary>
        public async Task LoadElevesData()
        {
            try
            {
                // Show loading indicator
                if (DataGridViewEleve != null)
                {
                    DataGridViewEleve.DataSource = null;
                    DataGridViewEleve.Rows.Clear();
                }

                // Get all students from the service
                var elevesData = await Task.Run(() => _elevesService.GetAllEleves());

                if (elevesData != null)
                {
                    // Convert to list for better performance
                    var elevesList = elevesData.ToList();

                    // Create a DataTable with the correct column structure
                    var dataTable = CreateElevesDataTable();

                    // Populate the DataTable with student data
                    foreach (dynamic eleve in elevesList)
                    {
                        var row = dataTable.NewRow();

                        // Map the data from service to DataTable columns
                        // Using safe access to dynamic properties
                        row["matricule"] = GetDynamicProperty(eleve, "Matricule") ?? string.Empty;
                        row["nom"] = GetDynamicProperty(eleve, "Nom") ?? string.Empty;
                        row["postnom"] = GetDynamicProperty(eleve, "PostNom") ?? string.Empty; // Note: mapping from service
                        row["prenom"] = GetDynamicProperty(eleve, "Prenom") ?? string.Empty; // Note: mapping from service
                        row["sexe"] = GetDynamicProperty(eleve, "Sexe") ?? string.Empty;

                        // Handle date safely
                        var dateNaiss = GetDynamicProperty(eleve, "DateNaissance");
                        row["date_naiss"] = dateNaiss != null ? dateNaiss : (object)DBNull.Value;

                        row["nom_tuteur"] = GetDynamicProperty(eleve, "NomPere") ?? string.Empty;
                        row["tel_tuteur"] = GetDynamicProperty(eleve, "TelephoneTuteur") ?? string.Empty;
                        row["avenue_nom"] = GetDynamicProperty(eleve, "avenue_nom") ?? string.Empty;
                        row["ecole_prov"] = GetDynamicProperty(eleve, "EcoleProvenance") ?? string.Empty;

                        dataTable.Rows.Add(row);
                    }

                    // Bind the DataTable to the DataGridView
                    if (DataGridViewEleve != null)
                    {
                        DataGridViewEleve.DataSource = dataTable;

                        // Configure the DataGridView appearance
                        ConfigureDataGridViewAppearance();

                        // Update statistics
                        UpdateElevesStatistics(elevesList);
                    }
                }
                else
                {
                    // Handle case where no data is returned
                    if (DataGridViewEleve != null)
                    {
                        DataGridViewEleve.DataSource = null;
                    }

                    // Clear statistics
                    UpdateElevesStatistics(new List<dynamic>());
                }
            }
            catch (Exception ex)
            {
                // Handle loading errors gracefully
                HandleDataLoadingError(ex);
            }
        }

        /// <summary>
        /// Creates a DataTable with the correct structure for students data
        /// </summary>
        /// <returns>Configured DataTable for students</returns>
        private System.Data.DataTable CreateElevesDataTable()
        {
            var dataTable = new System.Data.DataTable();

            // Add columns matching the DataGridView column structure
            dataTable.Columns.Add("matricule", typeof(string));
            dataTable.Columns.Add("nom", typeof(string));
            dataTable.Columns.Add("postnom", typeof(string));
            dataTable.Columns.Add("prenom", typeof(string));
            dataTable.Columns.Add("sexe", typeof(string));
            dataTable.Columns.Add("date_naiss", typeof(DateTime));
            dataTable.Columns.Add("nom_tuteur", typeof(string));
            dataTable.Columns.Add("tel_tuteur", typeof(string));
            dataTable.Columns.Add("avenue_nom", typeof(string));
            dataTable.Columns.Add("ecole_prov", typeof(string));

            return dataTable;
        }

        /// <summary>
        /// Configures the DataGridView appearance and behavior
        /// </summary>
        private void ConfigureDataGridViewAppearance()
        {
            if (DataGridViewEleve == null) return;

            try
            {
                // Configure selection behavior
                DataGridViewEleve.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                DataGridViewEleve.MultiSelect = false;

                // Configure appearance
                DataGridViewEleve.AllowUserToAddRows = false;
                DataGridViewEleve.AllowUserToDeleteRows = false;
                DataGridViewEleve.ReadOnly = true;

                // Auto-size columns
                DataGridViewEleve.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Configure row headers
                DataGridViewEleve.RowHeadersVisible = false;

                // Set alternating row colors for better readability
                DataGridViewEleve.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

                // Configure column widths for better display
                if (DataGridViewEleve.Columns.Count > 0)
                {
                    // Set specific column widths
                    if (DataGridViewEleve.Columns["colMatricule"] != null)
                        DataGridViewEleve.Columns["colMatricule"].FillWeight = 80;

                    if (DataGridViewEleve.Columns["colNom"] != null)
                        DataGridViewEleve.Columns["colNom"].FillWeight = 100;

                    if (DataGridViewEleve.Columns["colPostNom"] != null)
                        DataGridViewEleve.Columns["colPostNom"].FillWeight = 100;

                    if (DataGridViewEleve.Columns["colPrenom"] != null)
                        DataGridViewEleve.Columns["colPrenom"].FillWeight = 100;

                    if (DataGridViewEleve.Columns["colSexe"] != null)
                        DataGridViewEleve.Columns["colSexe"].FillWeight = 50;

                    if (DataGridViewEleve.Columns["colDateNaiss"] != null)
                        DataGridViewEleve.Columns["colDateNaiss"].FillWeight = 90;

                    if (DataGridViewEleve.Columns["colTuteur"] != null)
                        DataGridViewEleve.Columns["colTuteur"].FillWeight = 120;

                    if (DataGridViewEleve.Columns["colTelTuteur"] != null)
                        DataGridViewEleve.Columns["colTelTuteur"].FillWeight = 90;

                    if (DataGridViewEleve.Columns["colAdresse"] != null)
                        DataGridViewEleve.Columns["colAdresse"].FillWeight = 150;

                    if (DataGridViewEleve.Columns["colEcoleProv"] != null)
                        DataGridViewEleve.Columns["colEcoleProv"].FillWeight = 120;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la configuration du DataGridView: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the student statistics display
        /// </summary>
        /// <param name="elevesList">List of students for statistics calculation</param>
        private void UpdateElevesStatistics(IList<dynamic> elevesList)
        {
            try
            {
                if (elevesList == null)
                {
                    elevesList = new List<dynamic>();
                }

                // Calculate statistics
                var totalEleves = elevesList.Count;
                var totalGarcons = elevesList.Count(e => e.Sexe?.ToString()?.ToUpper() == "M");
                var totalFilles = elevesList.Count(e => e.Sexe?.ToString()?.ToUpper() == "F");

                // Update statistics controls if they exist
                if (TxtNbreTotEleve != null)
                    TxtNbreTotEleve.Text = totalEleves.ToString();

                if (TxtNbreGarconsEleve != null)
                    TxtNbreGarconsEleve.Text = totalGarcons.ToString();

                if (TxtNbreFillesEleve != null)
                    TxtNbreFillesEleve.Text = totalFilles.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la mise à jour des statistiques: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles data loading errors gracefully
        /// </summary>
        /// <param name="ex">The exception that occurred</param>
        private void HandleDataLoadingError(Exception ex)
        {
            try
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement des données élèves: {ex.Message}");

                // Clear the DataGridView
                if (DataGridViewEleve != null)
                {
                    DataGridViewEleve.DataSource = null;
                    DataGridViewEleve.Rows.Clear();
                }

                // Clear statistics
                UpdateElevesStatistics(new List<dynamic>());

                // Show user-friendly error message
                MessageBox.Show(
                    "Erreur lors du chargement des données des élèves.\n" +
                    "Veuillez vérifier votre connexion à la base de données et réessayer.",
                    "Erreur de chargement",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            catch (Exception innerEx)
            {
                // If even error handling fails, log it
                System.Diagnostics.Debug.WriteLine($"Erreur critique lors de la gestion d'erreur: {innerEx.Message}");
            }
        }

        /// <summary>
        /// Refreshes the students data after CRUD operations
        /// </summary>
        public async Task RefreshElevesData()
        {
            try
            {
                await LoadElevesData();
            }
            catch (Exception ex)
            {
                HandleDataLoadingError(ex);
            }
        }

        /// <summary>
        /// Safely gets a property value from a dynamic object
        /// </summary>
        /// <param name="dynamicObject">The dynamic object</param>
        /// <param name="propertyName">The property name to get</param>
        /// <returns>The property value or null if not found</returns>
        private object? GetDynamicProperty(dynamic dynamicObject, string propertyName)
        {
            try
            {
                if (dynamicObject == null) return null;

                // Use reflection to get the property value safely
                var type = dynamicObject.GetType();
                var property = type.GetProperty(propertyName);

                if (property != null)
                {
                    return property.GetValue(dynamicObject);
                }

                // Try as dictionary if it's an ExpandoObject or similar
                if (dynamicObject is System.Collections.Generic.IDictionary<string, object> dict)
                {
                    return dict.TryGetValue(propertyName, out var value) ? value : null;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region Agent Data Loading and Display

        /// <summary>
        /// Loads all agents data from the database and displays them in DataGridViewAgent
        /// </summary>
        public async Task LoadAgentsData()
        {
            try
            {
                // Show loading indicator
                if (DataGridViewAgent != null)
                {
                    DataGridViewAgent.DataSource = null;
                    DataGridViewAgent.Rows.Clear();
                }

                // Get all agents from the service
                var agentsData = await Task.Run(() => _agentsService.GetAllAgents());

                if (agentsData != null)
                {
                    // Convert to list for better performance
                    var agentsList = agentsData.ToList();

                    // Create a DataTable with the correct column structure
                    var dataTable = CreateAgentsDataTable();

                    // Populate the DataTable with agent data
                    foreach (dynamic agent in agentsList)
                    {
                        var row = dataTable.NewRow();

                        // Map the data from service to DataTable columns
                        row["matricule"] = GetDynamicProperty(agent, "matricule") ?? string.Empty;
                        row["nom"] = GetDynamicProperty(agent, "nom") ?? string.Empty;
                        row["postnom"] = GetDynamicProperty(agent, "postnom") ?? string.Empty;
                        row["prenom"] = GetDynamicProperty(agent, "prenom") ?? string.Empty;
                        row["sexe"] = GetDynamicProperty(agent, "sexe") ?? string.Empty;

                        // Handle date safely
                        var dateNaiss = GetDynamicProperty(agent, "date_naiss");
                        row["date_naiss"] = dateNaiss != null ? dateNaiss : (object)DBNull.Value;

                        row["lieu_naiss"] = GetDynamicProperty(agent, "lieu_naiss") ?? string.Empty;
                        row["service"] = GetDynamicProperty(agent, "service") ?? string.Empty;
                        row["fonction"] = GetDynamicProperty(agent, "fonction") ?? string.Empty;
                        row["grade"] = GetDynamicProperty(agent, "grade") ?? string.Empty;
                        row["email"] = GetDynamicProperty(agent, "email") ?? string.Empty;
                        row["tel"] = GetDynamicProperty(agent, "tel") ?? string.Empty;

                        // Handle salary safely
                        var salNet = GetDynamicProperty(agent, "sal_net");
                        row["sal_net"] = salNet != null ? salNet : (object)DBNull.Value;

                        dataTable.Rows.Add(row);
                    }

                    // Bind the DataTable to the DataGridView
                    if (DataGridViewAgent != null)
                    {
                        DataGridViewAgent.DataSource = dataTable;

                        // Configure the DataGridView appearance
                        ConfigureAgentsDataGridViewAppearance();

                        // Update statistics
                        UpdateAgentsStatistics(agentsList);
                    }
                }
                else
                {
                    // Handle case where no data is returned
                    if (DataGridViewAgent != null)
                    {
                        DataGridViewAgent.DataSource = null;
                    }

                    // Clear statistics
                    UpdateAgentsStatistics(new List<dynamic>());
                }
            }
            catch (Exception ex)
            {
                // Handle loading errors gracefully
                HandleAgentsDataLoadingError(ex);
            }
        }

        /// <summary>
        /// Creates a DataTable with the correct structure for agents data
        /// </summary>
        /// <returns>Configured DataTable for agents</returns>
        private System.Data.DataTable CreateAgentsDataTable()
        {
            var dataTable = new System.Data.DataTable();

            // Add columns matching the DataGridView column structure
            dataTable.Columns.Add("matricule", typeof(string));
            dataTable.Columns.Add("nom", typeof(string));
            dataTable.Columns.Add("postnom", typeof(string));
            dataTable.Columns.Add("prenom", typeof(string));
            dataTable.Columns.Add("sexe", typeof(string));
            dataTable.Columns.Add("date_naiss", typeof(DateTime));
            dataTable.Columns.Add("lieu_naiss", typeof(string));
            dataTable.Columns.Add("service", typeof(string));
            dataTable.Columns.Add("fonction", typeof(string));
            dataTable.Columns.Add("grade", typeof(string));
            dataTable.Columns.Add("email", typeof(string));
            dataTable.Columns.Add("tel", typeof(string));
            dataTable.Columns.Add("sal_net", typeof(decimal));

            return dataTable;
        }

        /// <summary>
        /// Configures the agents DataGridView appearance and behavior
        /// </summary>
        private void ConfigureAgentsDataGridViewAppearance()
        {
            if (DataGridViewAgent == null) return;

            try
            {
                // Configure selection behavior
                DataGridViewAgent.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                DataGridViewAgent.MultiSelect = false;

                // Configure appearance
                DataGridViewAgent.AllowUserToAddRows = false;
                DataGridViewAgent.AllowUserToDeleteRows = false;
                DataGridViewAgent.ReadOnly = true;

                // Auto-size columns
                DataGridViewAgent.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Configure row headers
                DataGridViewAgent.RowHeadersVisible = false;

                // Set alternating row colors for better readability
                DataGridViewAgent.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la configuration du DataGridView agents: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the agents statistics display
        /// </summary>
        /// <param name="agentsList">List of agents for statistics calculation</param>
        private void UpdateAgentsStatistics(IList<dynamic> agentsList)
        {
            try
            {
                if (agentsList == null)
                {
                    agentsList = new List<dynamic>();
                }

                // Calculate statistics
                var totalAgents = agentsList.Count;
                var totalHommes = agentsList.Count(a => a.sexe?.ToString()?.ToUpper() == "M");
                var totalFemmes = agentsList.Count(a => a.sexe?.ToString()?.ToUpper() == "F");

                // Count by service
                var enseignants = agentsList.Count(a => a.service?.ToString()?.ToLower().Contains("enseignement") == true);
                var administratifs = agentsList.Count(a => a.service?.ToString()?.ToLower().Contains("administration") == true);

                // Update statistics controls if they exist
                if (TxtNbreTotAgent != null)
                    TxtNbreTotAgent.Text = totalAgents.ToString();

                if (TxtNbreHommesAgent != null)
                    TxtNbreHommesAgent.Text = totalHommes.ToString();

                if (TxtNbreFemmesAgent != null)
                    TxtNbreFemmesAgent.Text = totalFemmes.ToString();

                if (TxtNbreEnseignants != null)
                    TxtNbreEnseignants.Text = enseignants.ToString();

                if (TxtNbreAdministratifs != null)
                    TxtNbreAdministratifs.Text = administratifs.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la mise à jour des statistiques agents: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles agents data loading errors gracefully
        /// </summary>
        /// <param name="ex">The exception that occurred</param>
        private void HandleAgentsDataLoadingError(Exception ex)
        {
            try
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement des données agents: {ex.Message}");

                // Clear the DataGridView
                if (DataGridViewAgent != null)
                {
                    DataGridViewAgent.DataSource = null;
                    DataGridViewAgent.Rows.Clear();
                }

                // Clear statistics
                UpdateAgentsStatistics(new List<dynamic>());

                // Show user-friendly error message
                MessageBox.Show(
                    "Erreur lors du chargement des données des agents.\n" +
                    "Veuillez vérifier votre connexion à la base de données et réessayer.",
                    "Erreur de chargement",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            catch (Exception innerEx)
            {
                // If even error handling fails, log it
                System.Diagnostics.Debug.WriteLine($"Erreur critique lors de la gestion d'erreur agents: {innerEx.Message}");
            }
        }

        /// <summary>
        /// Refreshes the agents data after CRUD operations
        /// </summary>
        public async Task RefreshAgentsData()
        {
            try
            {
                await LoadAgentsData();
            }
            catch (Exception ex)
            {
                HandleAgentsDataLoadingError(ex);
            }
        }


        /// <summary>
        /// Handles TxtNomAgent Enter event - generates automatic matricule
        /// </summary>
        private void TxtNomAgent_Enter(object sender, EventArgs e)
        {
            try
            {
                // ✅ Utilisation du user_index de l'utilisateur connecté depuis la base de données
                var userIndex = EduKinContext.CurrentUserIndex;
                _administrations.ExecuteGenerateId(TxtMatriculeAgent, "t_agents", "matricule", "AGT", userIndex);
                if (TxtAdresseAgent.Text.Trim() != null)
                {
                    btnAffectAgent.Enabled = true;
                }
                else
                {
                    btnAffectAgent.Enabled = false;
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Erreur d'authentification: {ex.Message}\nVeuillez vous reconnecter.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la génération du matricule: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles BtnCapturePicAgent Click event - opens FormWebcamCapture for photo capture
        /// </summary>
        private async void BtnCapturePicAgent_Click(object sender, EventArgs e)
        {
            try
            {
                // Create PictureManager instance for agent photos
                var pictureManager = new PictureManager("Photos/Agents");

                // Get the matricule for unique identification
                var matricule = TxtMatriculeAgent.Text.Trim();
                if (string.IsNullOrEmpty(matricule))
                {
                    MessageBox.Show("Veuillez d'abord générer ou saisir un matricule avant de capturer une photo.",
                        "Matricule requis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Capture photo using PictureManager (will be saved to secure location with matricule)
                var capturedPhotoPath = await pictureManager.CapturePhotoAsync(PictureBoxProfilAgent, matricule);

                if (!string.IsNullOrEmpty(capturedPhotoPath))
                {
                    // Store the secured photo path for database recording
                    _selectedPhotoPathAgent = capturedPhotoPath;

                    // Provide user feedback
                    MessageBox.Show("Photo capturée et sécurisée avec succès!",
                        "Photo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // User cancelled or capture failed
                    MessageBox.Show("Capture de photo annulée ou échouée.",
                        "Photo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la capture de photo: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles BtnLoadPicAgent Click event - opens file dialog for photo selection
        /// </summary>
        private void BtnLoadPicAgent_Click(object sender, EventArgs e)
        {
            try
            {
                // Create PictureManager instance for agent photos
                var pictureManager = new PictureManager("Photos/Agents");

                // Get the matricule for unique identification
                var matricule = TxtMatriculeAgent.Text.Trim();
                if (string.IsNullOrEmpty(matricule))
                {
                    MessageBox.Show("Veuillez d'abord générer ou saisir un matricule avant de charger une photo.",
                        "Matricule requis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Open file dialog and load selected picture (will be copied to secure location)
                if (pictureManager.BrowseAndLoadPicture(PictureBoxProfilAgent, out string securedPath, matricule))
                {
                    // Store the secured photo path for database recording
                    _selectedPhotoPathAgent = securedPath;

                    // Provide user feedback
                    MessageBox.Show("Photo chargée et sécurisée avec succès!",
                        "Photo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // User cancelled or load failed
                    MessageBox.Show("Aucune photo sélectionnée.",
                        "Photo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de photo: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// <summary>
        /// Handles BtnSaveAgent Click event - saves new agent
        /// </summary>
        private async void BtnSaveAgent_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate required fields
                if (!ValidateAgentRequiredFields())
                    return;

                // Vérifier que l'affectation a été faite
                if (string.IsNullOrWhiteSpace(_selectedServiceAgent))
                {
                    MessageBox.Show("Veuillez affecter l'agent à un service en cliquant sur 'Affecter'.",
                        "Affectation requise", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Disable the save button to prevent double-clicking
                BtnSaveAgent.Enabled = false;
                BtnSaveAgent.Text = "Enregistrement...";

                // Vérifier que l'affectation a été faite
                if (string.IsNullOrWhiteSpace(_selectedServiceAgent))
                {
                    MessageBox.Show("Veuillez affecter l'agent à un service en cliquant sur 'Affecter'.",
                        "Affectation requise", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    BtnSaveAgent.Enabled = true;
                    BtnSaveAgent.Text = "Enregistrer";
                    return;
                }

                // Parse salary fields
                decimal? salBase = _selectedSalBaseAgent > 0 ? _selectedSalBaseAgent : null;
                decimal? prime = _selectedPrimeAgent > 0 ? _selectedPrimeAgent : null;
                decimal? cnss = _selectedCnssAgent > 0 ? _selectedCnssAgent : null;
                decimal? ipr = _selectedIprAgent >= 0 ? _selectedIprAgent : null;
                decimal? salNet = _selectedSalNetAgent > 0 ? _selectedSalNetAgent : null;

                // Create agent using Agents service
                var success = _agentsService.CreateAgent(
                    nom: TxtNomAgent.Text.Trim(),
                    postNom: TxtPostnomAgent.Text.Trim(),
                    prenom: TxtPrenomAgent.Text.Trim(),
                    sexe: CmbSexeAgent.Text.Trim(),
                    lieuNaiss: TxtLieuNaissAgent.Text.Trim(),
                    dateNaiss: DtpDateNaissAgent.Value.Date,
                    userIndex: EduKinContext.CurrentUserIndex.ToString(),
                    email: string.IsNullOrWhiteSpace(TxtEmailAgent.Text) ? null : TxtEmailAgent.Text.Trim(),
                    tel: string.IsNullOrWhiteSpace(TxtTelAgent.Text) ? null : TxtTelAgent.Text.Trim(),
                    fkAvenue: _selectedIdAvenueAgent,
                    numero: _selectedNumParcelleAgent,
                    profil: string.IsNullOrWhiteSpace(_selectedPhotoPathAgent) ? null : _selectedPhotoPathAgent,
                    salBase: salBase,
                    prime: prime,
                    cnss: cnss,
                    ipr: ipr,
                    salNet: salNet
                );

                if (success)
                {
                    MessageBox.Show("Agent créé avec succès!",
                        "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh the DataGridView to show the new agent
                    await RefreshAgentsData();

                    // Clear all fields for next entry
                    ClearAllAgentFieldsForNewEntry();
                }
                else
                {
                    MessageBox.Show("Erreur lors de l'enregistrement de l'agent. Veuillez réessayer.",
                        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement de l'agent: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable the save button
                BtnSaveAgent.Enabled = true;
                BtnSaveAgent.Text = "Enregistrer";
            }
        }

        /// <summary>
        /// Handles BtnUpdateAgent Click event - updates existing agent
        /// </summary>
        private async void BtnUpdateAgent_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate matricule exists
                if (string.IsNullOrWhiteSpace(TxtMatriculeAgent.Text))
                {
                    MessageBox.Show("Veuillez sélectionner un agent à modifier.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate required fields
                if (!ValidateAgentRequiredFields())
                    return;

                // Vérifier que l'affectation a été faite
                if (string.IsNullOrWhiteSpace(_selectedServiceAgent))
                {
                    MessageBox.Show("Veuillez affecter l'agent à un service en cliquant sur 'Affecter'.",
                        "Affectation requise", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Parse salary fields
                decimal? salBase = _selectedSalBaseAgent > 0 ? _selectedSalBaseAgent : null;
                decimal? prime = _selectedPrimeAgent > 0 ? _selectedPrimeAgent : null;
                decimal? cnss = _selectedCnssAgent > 0 ? _selectedCnssAgent : null;
                decimal? ipr = _selectedIprAgent >= 0 ? _selectedIprAgent : null;
                decimal? salNet = _selectedSalNetAgent > 0 ? _selectedSalNetAgent : null;

                // Update agent using Agents service
                var success = _agentsService.UpdateAgent(
                    matricule: TxtMatriculeAgent.Text.Trim(),
                    nom: TxtNomAgent.Text.Trim(),
                    postNom: TxtPostnomAgent.Text.Trim(),
                    prenom: TxtPrenomAgent.Text.Trim(),
                    sexe: CmbSexeAgent.Text.Trim(),
                    lieuNaiss: TxtLieuNaissAgent.Text.Trim(),
                    dateNaiss: DtpDateNaissAgent.Value.Date,
                    email: string.IsNullOrWhiteSpace(TxtEmailAgent.Text) ? null : TxtEmailAgent.Text.Trim(),
                    tel: string.IsNullOrWhiteSpace(TxtTelAgent.Text) ? null : TxtTelAgent.Text.Trim(),
                    fkAvenue: _selectedIdAvenueAgent,
                    numero: _selectedNumParcelleAgent,
                    profil: string.IsNullOrWhiteSpace(_selectedPhotoPathAgent) ? null : _selectedPhotoPathAgent,
                    salBase: salBase,
                    prime: prime,
                    cnss: cnss,
                    ipr: ipr,
                    salNet: salNet
                );

                if (success)
                {
                    MessageBox.Show("Agent modifié avec succès!",
                        "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reload data and clear form
                    await LoadAgentsData();
                    ClearAllAgentFieldsForNewEntry();
                }
                else
                {
                    MessageBox.Show("Erreur lors de la modification de l'agent.",
                        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles BtnDelAgent Click event - deletes agent
        /// </summary>
        private async void BtnDelAgent_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate matricule exists
                if (string.IsNullOrWhiteSpace(TxtMatriculeAgent.Text))
                {
                    MessageBox.Show("Veuillez sélectionner un agent à supprimer.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Confirm deletion
                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer l'agent {TxtNomAgent.Text} {TxtPrenomAgent.Text}?",
                    "Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = _agentsService.DeleteAgent(TxtMatriculeAgent.Text.Trim());

                    if (success)
                    {
                        MessageBox.Show("Agent supprimé avec succès!",
                            "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Reload data and clear form
                        await LoadAgentsData();
                        ClearAllAgentFieldsForNewEntry();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la suppression de l'agent.",
                            "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Validates all required fields for agents before saving
        /// </summary>
        private bool ValidateAgentRequiredFields()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(TxtNomAgent.Text))
                errors.Add("- Le nom est requis");

            if (string.IsNullOrWhiteSpace(TxtPostnomAgent.Text))
                errors.Add("- Le post-nom est requis");

            if (string.IsNullOrWhiteSpace(TxtPrenomAgent.Text))
                errors.Add("- Le prénom est requis");

            if (string.IsNullOrWhiteSpace(CmbSexeAgent.Text))
                errors.Add("- Le sexe est requis");

            if (string.IsNullOrWhiteSpace(TxtLieuNaissAgent.Text))
                errors.Add("- Le lieu de naissance est requis");

            // Validate email format if provided
            if (!string.IsNullOrWhiteSpace(TxtEmailAgent.Text))
            {
                var email = TxtEmailAgent.Text.Trim();
                if (!email.Contains("@") || !email.Contains("."))
                {
                    errors.Add("- Format d'email invalide");
                }
            }

            if (errors.Any())
            {
                MessageBox.Show(
                    "Veuillez corriger les erreurs suivantes:\n\n" + string.Join("\n", errors),
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clears all agent fields and resets the form for new entry
        /// </summary>
        private void ClearAllAgentFieldsForNewEntry()
        {
            // Clear basic agent information
            TxtMatriculeAgent.Text = string.Empty;
            TxtNomAgent.Text = string.Empty;
            TxtPostnomAgent.Text = string.Empty;
            TxtPrenomAgent.Text = string.Empty;
            CmbSexeAgent.SelectedIndex = -1;
            DtpDateNaissAgent.Value = DateTime.Now;
            TxtLieuNaissAgent.Text = string.Empty;

            // Clear contact information
            TxtEmailAgent.Text = string.Empty;
            TxtTelAgent.Text = string.Empty;
            TxtAdresseAgent.Text = string.Empty;

            // Clear photo
            if (PictureBoxProfilAgent.Image != null)
            {
                var oldImage = PictureBoxProfilAgent.Image;
                PictureBoxProfilAgent.Image = null;
                oldImage.Dispose();
            }

            // Reset stored values
            _selectedPhotoPathAgent = string.Empty;
            _selectedServiceAgent = null;
            _selectedGradeAgent = null;
            _selectedRoleAgent = null;
            _selectedFonctionAgent = null;
            _selectedSalBaseAgent = 0;
            _selectedIprAgent = 0;
            _selectedSalNetAgent = 0;

            // Reset matricule field properties
            TxtMatriculeAgent.ReadOnly = false;
            TxtMatriculeAgent.Enabled = true;
            TxtMatriculeAgent.FillColor = System.Drawing.Color.White;
        }

        /// <summary>
        /// Handles DataGridViewAgent CellClick event - loads selected agent data
        /// </summary>
        private void DataGridViewAgent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && DataGridViewAgent.Rows[e.RowIndex].Cells["colMatriculeAgent"].Value != null)
                {
                    var matricule = DataGridViewAgent.Rows[e.RowIndex].Cells["colMatriculeAgent"].Value.ToString();
                    if (!string.IsNullOrEmpty(matricule))
                    {
                        LoadAgentFromGrid(matricule);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de l'agent: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Loads agent data from the database and populates the form
        /// </summary>
        /// <param name="matricule">Agent matricule to load</param>
        private void LoadAgentFromGrid(string matricule)
        {
            try
            {
                var agentData = _agentsService.GetAgent(matricule);

                if (agentData != null)
                {
                    // Load basic information
                    TxtMatriculeAgent.Text = agentData.matricule ?? string.Empty;
                    TxtNomAgent.Text = agentData.nom ?? string.Empty;
                    TxtPostnomAgent.Text = agentData.postnom ?? string.Empty;
                    TxtPrenomAgent.Text = agentData.prenom ?? string.Empty;
                    CmbSexeAgent.Text = agentData.sexe ?? string.Empty;

                    if (agentData.date_naiss != null)
                        DtpDateNaissAgent.Value = agentData.date_naiss;

                    TxtLieuNaissAgent.Text = agentData.lieu_naiss ?? string.Empty;

                    // Store professional information (will be displayed via FormAffectAgent)
                    _selectedServiceAgent = agentData.service;
                    _selectedFonctionAgent = agentData.fonction;
                    _selectedGradeAgent = agentData.grade;
                    _selectedRoleAgent = agentData.role;

                    // Load contact information
                    TxtEmailAgent.Text = agentData.email ?? string.Empty;
                    TxtTelAgent.Text = agentData.tel ?? string.Empty;
                    TxtAdresseAgent.Text = agentData.adresse ?? string.Empty;

                    // Store salary information (will be displayed via FormAffectAgent)
                    _selectedSalBaseAgent = agentData.sal_base ?? 0;
                    _selectedIprAgent = agentData.ipr ?? 0;
                    _selectedSalNetAgent = agentData.sal_net ?? 0;

                    // Load photo if exists
                    if (!string.IsNullOrEmpty(agentData.profil))
                    {
                        LoadExistingAgentPhoto(agentData.profil);
                    }
                    else
                    {
                        ClearAgentPhoto();
                    }

                    // Lock matricule field for editing
                    TxtMatriculeAgent.ReadOnly = true;
                    TxtMatriculeAgent.Enabled = false;
                    TxtMatriculeAgent.FillColor = Color.LightGray;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des données de l'agent: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Loads an existing agent photo into the PictureBox
        /// </summary>
        /// <param name="photoPath">Path to the photo file</param>
        private bool LoadExistingAgentPhoto(string photoPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(photoPath))
                {
                    ClearAgentPhoto();
                    return false;
                }

                var pictureManager = new PictureManager("Photos/Agents");

                if (pictureManager.LoadPicture(PictureBoxProfilAgent, photoPath))
                {
                    _selectedPhotoPathAgent = photoPath;
                    return true;
                }
                else
                {
                    ClearAgentPhoto();
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement de la photo agent: {ex.Message}");
                ClearAgentPhoto();
                return false;
            }
        }

        /// <summary>
        /// Clears the agent photo from the PictureBox
        /// </summary>
        private void ClearAgentPhoto()
        {
            try
            {
                if (PictureBoxProfilAgent.Image != null)
                {
                    var oldImage = PictureBoxProfilAgent.Image;
                    PictureBoxProfilAgent.Image = null;
                    oldImage.Dispose();
                }

                _selectedPhotoPathAgent = string.Empty;
                PictureBoxProfilAgent.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de l'effacement de la photo agent: {ex.Message}");
            }
        }

        /// <summary>
        /// Gère le clic sur le bouton Affecter Agent - Ouvre FormAffectAgent
        /// </summary>
        private void btnAffectAgent_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate required fields before affectation
                if (string.IsNullOrWhiteSpace(TxtNomAgent.Text) ||
                    string.IsNullOrWhiteSpace(TxtPostnomAgent.Text) ||
                    string.IsNullOrWhiteSpace(TxtPrenomAgent.Text))
                {
                    MessageBox.Show("Veuillez remplir les informations de base de l'agent avant l'affectation.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get matricule (already generated in TxtNomAgent_Enter)
                string matricule = TxtMatriculeAgent.Text.Trim();
                if (string.IsNullOrWhiteSpace(matricule))
                {
                    MessageBox.Show("Le matricule n'a pas été généré. Veuillez cliquer dans le champ Nom pour générer le matricule.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    TxtNomAgent.Focus();
                    return;
                }

                // Open FormAffectAgent dialog
                using (var formAffect = new FormAffectAgent(
                    matricule,
                    _selectedServiceAgent,
                    _selectedGradeAgent,
                    _selectedRoleAgent,
                    _selectedFonctionAgent,
                    _selectedSalBaseAgent,
                    _selectedIprAgent,
                    _selectedSalNetAgent))
                {
                    if (formAffect.ShowDialog() == DialogResult.OK)
                    {
                        // Store the selected values
                        _selectedServiceAgent = formAffect.SelectedService;
                        _selectedGradeAgent = formAffect.SelectedGrade;
                        _selectedRoleAgent = formAffect.SelectedRole;
                        _selectedFonctionAgent = formAffect.SelectedFonction;
                        _selectedSalBaseAgent = formAffect.SelectedSalBase;
                        _selectedIprAgent = formAffect.SelectedIpr;
                        _selectedSalNetAgent = formAffect.SelectedSalNet;

                        MessageBox.Show("Affectation enregistrée avec succès!",
                            "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'affectation: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        private void btnModifierEleve_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles BtnCapturePicEleve Click event - opens FormWebcamCapture for photo capture
        /// </summary>
        private async void BtnCapturePicEleve_Click(object sender, EventArgs e)
        {
            try
            {
                // Create PictureManager instance for student photos
                var pictureManager = new PictureManager("Photos/Eleves");

                // Get the matricule for unique identification
                var matricule = TxtMatriculeEleve.Text.Trim();
                if (string.IsNullOrEmpty(matricule))
                {
                    MessageBox.Show("Veuillez d'abord générer ou saisir un matricule avant de capturer une photo.",
                        "Matricule requis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Capture photo using PictureManager (will be saved to secure location with matricule)
                var capturedPhotoPath = await pictureManager.CapturePhotoAsync(PicBoxEleve, matricule);

                if (!string.IsNullOrEmpty(capturedPhotoPath))
                {
                    // Store the secured photo path for database recording
                    _selectedPhotoPathEleve = capturedPhotoPath;

                    // Provide user feedback
                    MessageBox.Show("Photo capturée et sécurisée avec succès!",
                        "Photo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // User cancelled or capture failed
                    MessageBox.Show("Capture de photo annulée ou échouée.",
                        "Photo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la capture de photo: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles BtnLoadPicEleve Click event - opens file dialog for photo selection
        /// </summary>
        private void BtnLoadPicEleve_Click(object sender, EventArgs e)
        {
            try
            {
                // Create PictureManager instance for student photos
                var pictureManager = new PictureManager("Photos/Eleves");

                // Get the matricule for unique identification
                var matricule = TxtMatriculeEleve.Text.Trim();
                if (string.IsNullOrEmpty(matricule))
                {
                    MessageBox.Show("Veuillez d'abord générer ou saisir un matricule avant de charger une photo.",
                        "Matricule requis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Open file dialog and load selected picture (will be copied to secure location)
                if (pictureManager.BrowseAndLoadPicture(PicBoxEleve, out string securedPath, matricule))
                {
                    // Store the secured photo path for database recording
                    _selectedPhotoPathEleve = securedPath;

                    // Provide user feedback
                    MessageBox.Show("Photo chargée et sécurisée avec succès!",
                        "Photo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // User cancelled or load failed
                    MessageBox.Show("Aucune photo sélectionnée.",
                        "Photo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de photo: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles BtnAffectEleve Click event - opens FormAffectEleves for student assignment to promotion
        /// </summary>
        private void BtnAffectEleve_Click(object sender, EventArgs e)
        {
            try
            {
                // Vérifier qu'on a un matricule d'élève
                var matricule = TxtMatriculeEleve?.Text?.Trim();
                if (string.IsNullOrWhiteSpace(matricule))
                {
                    MessageBox.Show("Veuillez d'abord saisir les informations de l'élève ou générer un matricule.",
                        "Matricule requis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get current affectation values
                var currentAnneeScolaire = _selectedAnneeScolaire ?? _currentSchoolYear;
                var currentCodePromotion = _selectedCodePromotion;
                var currentIndicePromotion = _selectedIndicePromotion;

                // Create and show the assignment dialog with current values
                using (var affectForm = new FormAffectEleve(
                    matricule,
                    currentAnneeScolaire,
                    currentCodePromotion,
                    currentIndicePromotion))
                {
                    // Show the assignment dialog
                    if (affectForm.ShowDialog(this) == DialogResult.OK)
                    {
                        // Validate that the assignment is complete
                        if (!affectForm.IsAffectationValid)
                        {
                            MessageBox.Show("L'affectation n'est pas complète. Veuillez sélectionner tous les champs requis.",
                                "Affectation incomplète", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Store the assignment information from the dialog
                        _selectedAnneeScolaire = affectForm.SelectedAnneeScolaire;
                        _selectedCodePromotion = affectForm.SelectedCodePromotion;
                        _selectedNomPromotion = affectForm.SelectedNomPromotion;
                        _selectedIndicePromotion = affectForm.SelectedIndicePromotion;

                        // Provide user feedback
                        var message = $"Affectation sélectionnée:\n" +
                                    $"Année scolaire: {affectForm.SelectedAnneeScolaire}\n" +
                                    $"Section: {affectForm.SelectedNomSection}\n" +
                                    $"Option: {affectForm.SelectedNomOption}\n" +
                                    $"Promotion: {affectForm.SelectedNomPromotion}\n" +
                                    $"Indice: {affectForm.SelectedIndicePromotion}\n\n" +
                                    $"L'affectation sera enregistrée lors de la sauvegarde de l'élève.";

                        MessageBox.Show(message, "Affectation sélectionnée", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Enable the save button since we now have assignment information
                        if (BtnSaveEleve != null)
                        {
                            BtnSaveEleve.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'affectation de l'élève: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles BtnSaveEleve Click event - saves new student with validation and affectation
        /// </summary>
        private async void BtnSaveEleve_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure we have a current student model
                if (_eleveController.CurrentEleve == null)
                {
                    MessageBox.Show("Aucun élève n'est actuellement en cours de traitement.",
                        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Collect all data from UI controls into the current student model
                CollectDataFromUI();

                // Validate the student data using EleveViewModel.IsValid()
                if (!_eleveController.CurrentEleve.IsValid())
                {
                    var validationErrors = _eleveController.CurrentEleve.GetValidationErrors();
                    var errorMessage = "Erreurs de validation:\n" + string.Join("\n", validationErrors);
                    MessageBox.Show(errorMessage, "Erreurs de validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if affectation is complete (required for new students)
                if (_eleveController.CurrentMode == EleveController.OperationMode.Create &&
                    !_eleveController.IsAffectationComplete())
                {
                    MessageBox.Show("L'affectation de l'élève à une promotion est obligatoire. " +
                        "Veuillez cliquer sur 'Affecter' pour sélectionner une promotion.",
                        "Affectation requise", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Disable the save button to prevent double-clicking
                BtnSaveEleve.Enabled = false;
                BtnSaveEleve.Text = "Enregistrement...";

                // Call the service to create the student
                var success = await SaveEleveToDatabase();

                if (success)
                {
                    // Show success message
                    MessageBox.Show("Élève créé avec succès!",
                        "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh the DataGridView to show the new student
                    await RefreshElevesData();

                    // Clear all fields for next entry
                    ClearAllFieldsForNewEntry();

                    // Initialize a new student for next entry
                    _eleveController.InitializeNewEleve();
                }
                else
                {
                    MessageBox.Show("Erreur lors de l'enregistrement de l'élève. Veuillez réessayer.",
                        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (ValidationException vex)
            {
                var errorMessage = "Erreurs de validation:\n" + string.Join("\n", vex.ValidationErrors);
                MessageBox.Show(errorMessage, "Erreurs de validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement de l'élève: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable the save button
                BtnSaveEleve.Enabled = true;
                BtnSaveEleve.Text = "Enregistrer";
            }
        }

        /// <summary>
        /// Collects all data from UI controls and updates the current student model
        /// </summary>
        private void CollectDataFromUI()
        {
            if (_eleveController.CurrentEleve == null) return;

            var eleve = _eleveController.CurrentEleve;

            // Basic student information
            eleve.Matricule = TxtMatriculeEleve.Text.Trim();
            eleve.Nom = TxtNomEleve.Text.Trim();
            eleve.PostNom = TxtPostnomEleve.Text.Trim();
            eleve.Prenom = TxtPrenomEleve.Text.Trim();
            eleve.Sexe = CmbSexeEleve.Text.Trim();

            // Date of birth
            if (DtpDateNaissanceEleve.Value != DateTime.MinValue)
            {
                eleve.DateNaissance = DtpDateNaissanceEleve.Value.Date;
            }

            eleve.LieuNaissance = TxtLieuNaissanceEleve.Text.Trim();

            // Guardian information
            eleve.NomTuteur = TxtNomTuteurEleve.Text.Trim();
            eleve.TelTuteur = TxtTelTuteurEleve.Text.Trim();

            // Address information
            eleve.FkAvenue = _selectedIdAvenueEleve ?? string.Empty;
            eleve.NumeroAdresse = TxtNumParcelleEleve.Text.Trim();
            eleve.AdresseComplete = TxtAdresseEleve.Text.Trim();

            // School information
            eleve.EcoleProvenance = TxtEcoleProvenanceEleve.Text.Trim();

            // Photo path
            eleve.CheminPhoto = _selectedPhotoPathEleve ?? string.Empty;

            // Assignment information (already stored in controller)
            eleve.AnneeScolaire = _selectedAnneeScolaire;
            eleve.CodePromotion = _selectedCodePromotion;
            eleve.NomPromotion = _selectedNomPromotion;
            eleve.IndicePromotion = _selectedIndicePromotion;
        }

        /// <summary>
        /// Saves the student to database and creates affectation record
        /// </summary>
        /// <returns>True if successful</returns>
        private async Task<bool> SaveEleveToDatabase()
        {
            if (_eleveController.CurrentEleve == null) return false;

            var eleve = _eleveController.CurrentEleve;

            try
            {
                // Get the current user index for matricule generation
                var userIndex = EduKinContext.CurrentUserIndex;

                // Create the student record using the Eleves service
                var studentCreated = _elevesService.CreateEleve(
                    nom: eleve.Nom,
                    postNom: eleve.PostNom,
                    prenom: eleve.Prenom,
                    sexe: eleve.Sexe,
                    nomTuteur: eleve.NomTuteur,
                    userIndex: userIndex,
                    lieuNaiss: eleve.LieuNaissance,
                    dateNaiss: eleve.DateNaissance,
                    telTuteur: eleve.TelTuteur,
                    fkAvenue: eleve.FkAvenue,
                    numero: eleve.NumeroAdresse,
                    ecoleProv: eleve.EcoleProvenance,
                    profil: eleve.CheminPhoto
                );

                if (!studentCreated)
                {
                    return false;
                }

                // Create the affectation record if assignment information is available
                if (!string.IsNullOrWhiteSpace(eleve.CodePromotion) &&
                    !string.IsNullOrWhiteSpace(eleve.AnneeScolaire) &&
                    !string.IsNullOrWhiteSpace(eleve.IndicePromotion))
                {
                    var affectationCreated = await CreateAffectationRecord(
                        eleve.Matricule,
                        eleve.CodePromotion,
                        eleve.AnneeScolaire,
                        eleve.IndicePromotion
                    );

                    if (!affectationCreated)
                    {
                        // Log warning but don't fail the entire operation
                        MessageBox.Show("L'élève a été créé mais l'affectation n'a pas pu être enregistrée. " +
                            "Vous pouvez l'affecter manuellement plus tard.",
                            "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erreur lors de l'enregistrement en base de données: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates an affectation record in t_affectation table
        /// </summary>
        /// <param name="matricule">Student matricule</param>
        /// <param name="codePromotion">Promotion code</param>
        /// <param name="anneeScolaire">School year</param>
        /// <param name="indicePromotion">Promotion index</param>
        /// <returns>True if successful</returns>
        private async Task<bool> CreateAffectationRecord(string matricule, string codePromotion,
            string anneeScolaire, string indicePromotion)
        {
            try
            {
                // Use the Administrations service which handles school isolation
                var success = await Task.Run(() => _administrations.CreateAffectation(
                    matricule: matricule,
                    codPromo: codePromotion,
                    anneeScol: anneeScolaire,
                    indicePromo: indicePromotion
                ));

                return success;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la création de l'affectation: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Clears all fields and resets the form for new entry
        /// </summary>
        private void ClearAllFieldsForNewEntry()
        {
            // Clear basic student information
            TxtMatriculeEleve.Text = string.Empty;
            TxtNomEleve.Text = string.Empty;
            TxtPostnomEleve.Text = string.Empty;
            TxtPrenomEleve.Text = string.Empty;
            CmbSexeEleve.SelectedIndex = -1;
            DtpDateNaissanceEleve.Value = DateTime.Now;
            TxtLieuNaissanceEleve.Text = string.Empty;

            // Clear guardian information
            TxtNomTuteurEleve.Text = string.Empty;
            TxtTelTuteurEleve.Text = string.Empty;

            // Clear address information
            TxtAdresseEleve.Text = string.Empty;
            TxtNumParcelleEleve.Text = string.Empty;

            // Clear school information
            TxtEcoleProvenanceEleve.Text = string.Empty;

            // Clear photo
            if (PicBoxEleve.Image != null)
            {
                var oldImage = PicBoxEleve.Image;
                PicBoxEleve.Image = null;
                oldImage.Dispose();
            }

            // Reset stored values
            _selectedIdAvenueEleve = string.Empty;
            _selectedPhotoPathEleve = string.Empty;
            ClearAffectationInfo();

            // Reset matricule field properties
            TxtMatriculeEleve.ReadOnly = false;
            TxtMatriculeEleve.Enabled = true;
            TxtMatriculeEleve.FillColor = System.Drawing.Color.White;
        }

        /// <summary>
        /// Handles BtnSetAdresseEleve Click event - opens FormAddressSearch for address selection
        /// </summary>
        private void BtnSetAdresseEleve_Click(object sender, EventArgs e)
        {
            try
            {
                using (var addressForm = new FormAddressSearch())
                {
                    if (addressForm.ShowDialog(this) == DialogResult.OK)
                    {
                        // Get the selected address information directly from the form
                        var selectedAvenue = addressForm.SelectedAvenue;
                        var selectedQuartier = addressForm.SelectedQuartier;
                        var selectedCommune = addressForm.SelectedCommune;
                        var selectedVille = addressForm.SelectedVille;
                        var selectedProvince = addressForm.SelectedProvince;

                        // Get the IdAvenue from the database based on the selected address components
                        var idAvenue = GetAvenueIdFromAddress(selectedAvenue, selectedQuartier, selectedCommune, selectedVille, selectedProvince);

                        // Store the IdAvenue for database recording
                        _selectedIdAvenueEleve = idAvenue;

                        // Get the numero from TxtNumParcelleEleve if it exists
                        var numero = TxtNumParcelleEleve.Text.Trim();

                        // Build the complete address string
                        var fullAddress = addressForm.GetFullAddress();
                        if (!string.IsNullOrEmpty(numero))
                        {
                            fullAddress = $"{numero}, {fullAddress}";
                        }

                        // Display the complete address in TxtAdresseEleve
                        TxtAdresseEleve.Text = fullAddress;

                        // Provide user feedback
                        MessageBox.Show("Adresse sélectionnée avec succès!",
                            "Adresse", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sélection d'adresse: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles TxtNomEleve Enter event - generates automatic matricule
        /// </summary>
        private void TxtNomEleve_Enter(object sender, EventArgs e)
        {
            try
            {
                // ✅ Utilisation du user_index de l'utilisateur connecté depuis la base de données
                var userIndex = EduKinContext.CurrentUserIndex;
                _administrations.ExecuteGenerateId(TxtMatriculeEleve, "t_eleves", "matricule", "ELV", userIndex);

                // Store the generated matricule in the current student model
                if (_eleveController.CurrentEleve != null && !string.IsNullOrEmpty(TxtMatriculeEleve.Text))
                {
                    _eleveController.CurrentEleve.Matricule = TxtMatriculeEleve.Text;
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Erreur d'authentification: {ex.Message}\nVeuillez vous reconnecter.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la génération du matricule: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Student CRUD Methods

        /// <summary>
        /// Gets the current user index for matricule generation
        /// </summary>
        private string GetCurrentUserIndex()
        {
            try
            {
                // TODO: Implement proper user index retrieval from logged-in user
                // For now, return default value
                // This should be retrieved from the current user's information
                return "001";
            }
            catch
            {
                return "001";
            }
        }

        /// <summary>
        /// Validates all required fields before saving
        /// </summary>
        private bool ValidateRequiredFields()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(TxtNomEleve.Text))
                errors.Add("- Le nom est requis");

            if (string.IsNullOrWhiteSpace(TxtPostnomEleve.Text))
                errors.Add("- Le post-nom est requis");

            if (string.IsNullOrWhiteSpace(TxtPrenomEleve.Text))
                errors.Add("- Le prénom est requis");

            if (string.IsNullOrWhiteSpace(CmbSexeEleve.Text))
                errors.Add("- Le sexe est requis");

            if (string.IsNullOrWhiteSpace(TxtNomTuteurEleve.Text))
                errors.Add("- Le nom du tuteur est requis");

            if (string.IsNullOrWhiteSpace(TxtLieuNaissanceEleve.Text))
                errors.Add("- Le lieu de naissance est requis");

            if (string.IsNullOrWhiteSpace(_selectedIdAvenueEleve))
                errors.Add("- L'adresse (avenue) est requise. Cliquez sur 'Définir Adresse'");

            if (string.IsNullOrWhiteSpace(TxtNumParcelleEleve.Text))
                errors.Add("- Le numéro de parcelle est requis");

            if (errors.Any())
            {
                MessageBox.Show(
                    "Veuillez remplir les champs obligatoires:\n\n" + string.Join("\n", errors),
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handles BtnSaveEleve Click event - saves new student
        /// Event handler to be wired in Designer
        /// </summary>


        /// <summary>
        /// Handles BtnUpdateEleve Click event - updates existing student
        /// Event handler to be wired in Designer
        /// </summary>
        private async void BtnUpdateEleve_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate matricule exists
                if (string.IsNullOrWhiteSpace(TxtMatriculeEleve.Text))
                {
                    MessageBox.Show("Veuillez sélectionner un élève à modifier.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate required fields
                if (!ValidateRequiredFields())
                    return;

                // Update student using Eleves service
                var success = _elevesService.UpdateEleve(
                    matricule: TxtMatriculeEleve.Text.Trim(),
                    nom: TxtNomEleve.Text.Trim(),
                    postNom: TxtPostnomEleve.Text.Trim(),
                    prenom: TxtPrenomEleve.Text.Trim(),
                    sexe: CmbSexeEleve.Text.Trim(),
                    nomTuteur: TxtNomTuteurEleve.Text.Trim(),
                    lieuNaiss: TxtLieuNaissanceEleve.Text.Trim(),
                    fkAvenue: _selectedIdAvenueEleve,
                    numero: TxtNumParcelleEleve.Text.Trim(),
                    dateNaiss: DtpDateNaissanceEleve.Value != DateTime.MinValue ? DtpDateNaissanceEleve.Value : (DateTime?)null,
                    telTuteur: string.IsNullOrWhiteSpace(TxtTelTuteurEleve.Text) ? null : TxtTelTuteurEleve.Text.Trim(),
                    ecoleProv: string.IsNullOrWhiteSpace(TxtEcoleProvenanceEleve.Text) ? null : TxtEcoleProvenanceEleve.Text.Trim(),
                    profil: string.IsNullOrWhiteSpace(_selectedPhotoPathEleve) ? null : _selectedPhotoPathEleve
                );

                if (success)
                {
                    MessageBox.Show("Élève modifié avec succès!",
                        "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reload data and clear form
                    await LoadElevesData();
                    ClearAllFieldsForNewEntry();
                }
                else
                {
                    MessageBox.Show("Erreur lors de la modification de l'élève.",
                        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles BtnDelEleve Click event - deletes student
        /// Event handler to be wired in Designer
        /// </summary>
        private async void BtnDelEleve_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate matricule exists
                if (string.IsNullOrWhiteSpace(TxtMatriculeEleve.Text))
                {
                    MessageBox.Show("Veuillez sélectionner un élève à supprimer.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Confirm deletion
                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer l'élève {TxtNomEleve.Text} {TxtPrenomEleve.Text}?",
                    "Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = _elevesService.DeleteEleve(TxtMatriculeEleve.Text.Trim());

                    if (success)
                    {
                        MessageBox.Show("Élève supprimé avec succès!",
                            "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Reload data and clear form
                        await LoadElevesData();
                        ClearAllFieldsForNewEntry();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la suppression de l'élève.",
                            "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion




        /// <summary>
        /// Configures the DataGridView appearance for agents
        /// </summary>
        private void ConfigureAgentDataGridViewAppearance()
        {
            if (DataGridViewAgent == null) return;

            DataGridViewAgent.AutoGenerateColumns = false;
            DataGridViewAgent.AllowUserToAddRows = false;
            DataGridViewAgent.AllowUserToDeleteRows = false;
            DataGridViewAgent.ReadOnly = true;
            DataGridViewAgent.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DataGridViewAgent.MultiSelect = false;
        }

        /// <summary>
        /// Updates agent statistics
        /// </summary>
        private void UpdateAgentsStatistics(List<dynamic> agentsList)
        {
            var totalAgents = agentsList.Count;
            var hommes = agentsList.Count(a => GetDynamicProperty(a, "Sexe") == "M");
            var femmes = agentsList.Count(a => GetDynamicProperty(a, "Sexe") == "F");

            if (TxtNbreTotAgent != null) TxtNbreTotAgent.Text = $"Total: {totalAgents}";
            if (TxtNbreHommesAgent != null) TxtNbreHommesAgent.Text = $"Hommes: {hommes}";
            if (TxtNbreFemmesAgent != null) TxtNbreFemmesAgent.Text = $"Femmes: {femmes}";
        }

        /// <summary>
        /// Handles DataGridViewAgent row selection - loads agent data into form fields
        /// </summary>
        private void DataGridViewAgent_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (DataGridViewAgent.SelectedRows.Count > 0)
                {
                    var selectedRow = DataGridViewAgent.SelectedRows[0];

                    TxtMatriculeAgent.Text = selectedRow.Cells["colMatriculeAgent"].Value?.ToString() ?? "";
                    TxtNomAgent.Text = selectedRow.Cells["colNomAgent"].Value?.ToString() ?? "";
                    TxtPostnomAgent.Text = selectedRow.Cells["colPostNomAgent"].Value?.ToString() ?? "";
                    TxtPrenomAgent.Text = selectedRow.Cells["colPrenomAgent"].Value?.ToString() ?? "";
                    CmbSexeAgent.Text = selectedRow.Cells["colSexeAgent"].Value?.ToString() ?? "";
                    TxtTelAgent.Text = selectedRow.Cells["colTelAgent"].Value?.ToString() ?? "";
                    TxtEmailAgent.Text = selectedRow.Cells["colEmailAgent"].Value?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la sélection d'un agent: {ex.Message}");
            }
        }




        /// <summary>
        /// Handles btnSaveAgents Click event - saves new agent
        /// </summary>
        private async void btnSaveAgents_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate required fields
                if (!ValidateAgentRequiredFields())
                    return;

                // Get user index for ID generation
                var userIndex = GetCurrentUserIndex();

                // Create agent using Agents service (matricule is generated internally)
                var success = _agentsService.CreateAgent(
                    nom: TxtNomAgent.Text.Trim(),
                    postNom: TxtPostnomAgent.Text.Trim(),
                    prenom: TxtPrenomAgent.Text.Trim(),
                    sexe: CmbSexeAgent.Text.Trim(),
                    lieuNaiss: TxtLieuNaissAgent.Text.Trim(),
                    dateNaiss: DtpDateNaissAgent.Value,
                    userIndex: userIndex,
                    email: string.IsNullOrWhiteSpace(TxtEmailAgent.Text) ? null : TxtEmailAgent.Text.Trim(),
                    tel: string.IsNullOrWhiteSpace(TxtTelAgent.Text) ? null : TxtTelAgent.Text.Trim(),
                    fkAvenue: _selectedIdAvenueAgent,
                    numero: _selectedNumParcelleAgent,
                    profil: string.IsNullOrWhiteSpace(_selectedPhotoPathAgent) ? null : _selectedPhotoPathAgent
                );

                if (success)
                {
                    MessageBox.Show("Agent créé avec succès!",
                        "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reload data and clear form
                    await LoadAgentsData();
                    ClearAgentFields();
                }
                else
                {
                    MessageBox.Show("Erreur lors de la création de l'agent.",
                        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles btnUpdateAgents Click event - updates existing agent
        /// </summary>
        private async void btnUpdateAgents_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate matricule exists
                if (string.IsNullOrWhiteSpace(TxtMatriculeAgent.Text))
                {
                    MessageBox.Show("Veuillez sélectionner un agent à modifier.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate required fields
                if (!ValidateAgentRequiredFields())
                    return;

                // Update agent using Agents service
                var success = _agentsService.UpdateAgent(
                    matricule: TxtMatriculeAgent.Text.Trim(),
                    nom: TxtNomAgent.Text.Trim(),
                    postNom: TxtPostnomAgent.Text.Trim(),
                    prenom: TxtPrenomAgent.Text.Trim(),
                    sexe: CmbSexeAgent.Text.Trim(),
                    lieuNaiss: TxtLieuNaissAgent.Text.Trim(),
                    dateNaiss: DtpDateNaissAgent.Value,
                    email: string.IsNullOrWhiteSpace(TxtEmailAgent.Text) ? null : TxtEmailAgent.Text.Trim(),
                    tel: string.IsNullOrWhiteSpace(TxtTelAgent.Text) ? null : TxtTelAgent.Text.Trim(),
                    fkAvenue: _selectedIdAvenueAgent,
                    numero: _selectedNumParcelleAgent,
                    profil: string.IsNullOrWhiteSpace(_selectedPhotoPathAgent) ? null : _selectedPhotoPathAgent
                );

                if (success)
                {
                    MessageBox.Show("Agent modifié avec succès!",
                        "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reload data and clear form
                    await LoadAgentsData();
                    ClearAgentFields();
                }
                else
                {
                    MessageBox.Show("Erreur lors de la modification de l'agent.",
                        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles btnDelAgents Click event - deletes agent
        /// </summary>
        private async void btnDelAgents_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate matricule exists
                if (string.IsNullOrWhiteSpace(TxtMatriculeAgent.Text))
                {
                    MessageBox.Show("Veuillez sélectionner un agent à supprimer.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Confirm deletion
                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer l'agent {TxtNomAgent.Text} {TxtPrenomAgent.Text}?",
                    "Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = _agentsService.DeleteAgent(TxtMatriculeAgent.Text.Trim());

                    if (success)
                    {
                        MessageBox.Show("Agent supprimé avec succès!",
                            "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Reload data and clear form
                        await LoadAgentsData();
                        ClearAgentFields();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la suppression de l'agent.",
                            "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Clears all agent input fields
        /// </summary>
        private void ClearAgentFields()
        {
            TxtMatriculeAgent.Clear();
            TxtNomAgent.Clear();
            TxtPostnomAgent.Clear();
            TxtPrenomAgent.Clear();
            TxtLieuNaissAgent.Clear();
            TxtEmailAgent.Clear();
            TxtTelAgent.Clear();
            TxtAdresseAgent.Clear();
            TxtNumeroParcelleAgent.Clear();
            CmbSexeAgent.SelectedIndex = -1;
            DtpDateNaissAgent.Value = DateTime.Now;

            if (PictureBoxProfilAgent.Image != null)
            {
                var oldImage = PictureBoxProfilAgent.Image;
                PictureBoxProfilAgent.Image = null;
                oldImage.Dispose();
            }

            _selectedPhotoPathAgent = string.Empty;
        }

        /// <summary>
        /// <summary>
        /// Handles btnCaptureAgent Click event - captures photo from webcam for agent
        /// </summary>
        private void btnCaptureAgent_Click(object sender, EventArgs e)
        {
            try
            {
                using (var webcamForm = new FormWebcamCapture())
                {
                    if (webcamForm.ShowDialog(this) == DialogResult.OK)
                    {
                        var capturedImage = webcamForm.CapturedImage;
                        if (capturedImage != null)
                        {
                            // Create PictureManager instance
                            var pictureManager = new PictureManager("Photos/Agents");

                            // Save the captured image
                            var savedPath = pictureManager.SavePicture(capturedImage,
                                $"AGT_{DateTime.Now:yyyyMMddHHmmss}.jpg");

                            if (!string.IsNullOrEmpty(savedPath))
                            {
                                // Load the saved image into the PictureBox
                                pictureManager.LoadPicture(PictureBoxProfilAgent, savedPath);
                                _selectedPhotoPathAgent = savedPath;

                                MessageBox.Show("Photo capturée avec succès!",
                                    "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la capture photo: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles btnLoadPicAgent Click event - loads photo from file for agent
        /// </summary>
        private void btnLoadPicAgent_Click(object sender, EventArgs e)
        {
            try
            {
                using (var openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp";
                    openFileDialog.Title = "Sélectionner une photo";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Create PictureManager instance
                        var pictureManager = new PictureManager("Photos/Agents");

                        // Load and save the selected image
                        var image = Image.FromFile(openFileDialog.FileName);
                        var savedPath = pictureManager.SavePicture(image as Bitmap,
                            $"AGT_{DateTime.Now:yyyyMMddHHmmss}.jpg");

                        if (!string.IsNullOrEmpty(savedPath))
                        {
                            // Load  the saved image into the PictureBox
                            pictureManager.LoadPicture(PictureBoxProfilAgent, savedPath);
                            _selectedPhotoPathAgent = savedPath;

                            MessageBox.Show("Photo chargée avec succès!",
                                "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de la photo: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles BtnSetAdresseAgent Click event - opens FormAddressSearch for address selection
        /// </summary>
        private void BtnSetAdresseAgent_Click(object sender, EventArgs e)
        {
            try
            {
                using (var addressForm = new FormAddressSearch())
                {
                    if (addressForm.ShowDialog(this) == DialogResult.OK)
                    {
                        // Get the selected address information directly from the form
                        var selectedAvenue = addressForm.SelectedAvenue;
                        var selectedQuartier = addressForm.SelectedQuartier;
                        var selectedCommune = addressForm.SelectedCommune;
                        var selectedVille = addressForm.SelectedVille;
                        var selectedProvince = addressForm.SelectedProvince;

                        // Get the IdAvenue from the database based on the selected address components
                        var idAvenue = GetAvenueIdFromAddress(selectedAvenue, selectedQuartier, selectedCommune, selectedVille, selectedProvince);

                        // Store the IdAvenue for database recording
                        _selectedIdAvenueAgent = idAvenue;

                        // Get the numero from TxtNumeroParcelleAgent if it exists
                        var numero = TxtNumeroParcelleAgent?.Text.Trim() ?? string.Empty;

                        // Build the complete address string
                        var fullAddress = addressForm.GetFullAddress();
                        if (!string.IsNullOrEmpty(numero))
                        {
                            fullAddress = $"{numero}, {fullAddress}";
                        }

                        // Display the complete address in TxtAdresseAgent
                        TxtAdresseAgent.Text = fullAddress;

                        // Provide user feedback
                        MessageBox.Show("Adresse sélectionnée avec succès!",
                            "Adresse", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sélection d'adresse: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSetAdresseAgents_Click(object sender, EventArgs e)
        {
            try
            {
                using (var addressForm = new FormAddressSearch())
                {
                    if (addressForm.ShowDialog(this) == DialogResult.OK)
                    {
                        // Get the selected address information directly from the form
                        var selectedAvenue = addressForm.SelectedAvenue;
                        var selectedQuartier = addressForm.SelectedQuartier;
                        var selectedCommune = addressForm.SelectedCommune;
                        var selectedVille = addressForm.SelectedVille;
                        var selectedProvince = addressForm.SelectedProvince;

                        // Get the IdAvenue from the database based on the selected address components
                        var idAvenue = GetAvenueIdFromAddress(selectedAvenue, selectedQuartier, selectedCommune, selectedVille, selectedProvince);

                        // Store the IdAvenue for database recording
                        _selectedIdAvenueAgent = idAvenue;

                        // Get the numero from TxtNumParcelleEleve if it exists
                        var numero = TxtNumeroParcelleAgent.Text.Trim();

                        // Build the complete address string
                        var fullAddress = addressForm.GetFullAddress();
                        if (!string.IsNullOrEmpty(numero))
                        {
                            fullAddress = $"{numero}, {fullAddress}";
                        }

                        // Display the complete address in TxtAdresseEleve
                        TxtAdresseAgent.Text = fullAddress;

                        // Provide user feedback
                        MessageBox.Show("Adresse sélectionnée avec succès!",
                            "Adresse", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sélection d'adresse: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
