using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using EduKin.Inits;
using EduKin.DataSets;
using EduKin.Csharp.Admins;
using EduKinContext = EduKin.Inits.EduKinContext; // Résoudre le conflit de noms

namespace EduKin.Layouts
{
    /// <summary>
    /// Formulaire de démarrage avec vérification de configuration et chargement des données
    /// </summary>
    public partial class FormStart : Form
    {
        private readonly SchoolConfigManager _configManager;
        private readonly Connexion _connexion;
        private readonly Administrations _administrations;

        public FormStart()
        {
            InitializeComponent();
            _configManager = new SchoolConfigManager();
            _connexion = Connexion.Instance;
            _administrations = new Administrations();

            // S'abonner aux changements de connexion
            _connexion.ConnectionChanged += OnConnectionChanged;
        }

        /// <summary>
        /// Gère les changements de connexion internet/base de données
        /// </summary>
        private void OnConnectionChanged(object? sender, ConnectionChangedEventArgs e)
        {
            // Mettre à jour l'interface utilisateur sur le thread principal
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnConnectionChanged(sender, e)));
                return;
            }

            var message = e.IsOnline
                ? $"Connexion rétablie - Basculement vers {e.DatabaseType}"
                : $"Connexion perdue - Basculement vers {e.DatabaseType}";

            UpdateProgress(50, message);

            // Optionnel : Afficher une notification à l'utilisateur
            // MessageBox.Show(message, "Changement de connexion", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Se désabonner de l'événement et arrêter la surveillance
            if (_connexion != null)
            {
                _connexion.ConnectionChanged -= OnConnectionChanged;
                _connexion.StopConnectionMonitoring();
            }
            base.OnFormClosed(e);
        }

        private async void FormStart_Load(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("=== FormStart_Load DEBUT ===");
                System.Diagnostics.Debug.WriteLine("=== FormStart_Load DEBUT ===");
                await InitializeApplication();
                Console.WriteLine("=== FormStart_Load FIN ===");
                System.Diagnostics.Debug.WriteLine("=== FormStart_Load FIN ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERREUR CRITIQUE dans FormStart_Load: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"ERREUR CRITIQUE dans FormStart_Load: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                MessageBox.Show(
                    $"Erreur critique au démarrage:\n{ex.Message}\n\nL'application va se fermer.",
                    "Erreur Critique",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                
                Application.Exit();
            }
        }

        /// <summary>
        /// Initialise l'application en vérifiant la configuration et chargeant les données
        /// </summary>
        private async Task InitializeApplication()
        {
            try
            {
                UpdateProgress(10, "Démarrage de l'application...");
                await Task.Delay(500);

                UpdateProgress(20, "Vérification de la configuration...");
                
                await CheckConfiguration();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du démarrage : {ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Application.Exit();
            }
        }

        /// <summary>
        /// Vérifie si une configuration existe et charge les données
        /// </summary>
        private async Task CheckConfiguration()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== DEBUT CheckConfiguration ===");
                
                if (!_configManager.ConfigExists())
                {
                    // Pas de configuration - premier lancement
                    System.Diagnostics.Debug.WriteLine("INFO: Aucune configuration trouvée - Premier lancement");
                    UpdateProgress(100, "Configuration requise...");
                    await Task.Delay(500);
                    NavigateToConfig();
                    return;
                }

                System.Diagnostics.Debug.WriteLine("Configuration trouvée, chargement...");
                UpdateProgress(40, "Chargement de la configuration...");
                
                var config = await Task.Run(() => _configManager.LoadConfig());

                if (config == null)
                {
                    System.Diagnostics.Debug.WriteLine("ERREUR: Configuration corrompue ou illisible");
                    UpdateProgress(100, "Configuration corrompue...");
                    await Task.Delay(500);
                    
                    // Supprimer la configuration corrompue
                    _configManager.DeleteConfig();
                    NavigateToConfig();
                    return;
                }

                if (string.IsNullOrWhiteSpace(config.IdEcole))
                {
                    System.Diagnostics.Debug.WriteLine($"ERREUR: IdEcole est vide dans la configuration");
                    UpdateProgress(100, "Configuration invalide...");
                    await Task.Delay(500);
                    
                    // Supprimer la configuration invalide
                    _configManager.DeleteConfig();
                    NavigateToConfig();
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Configuration valide: IdEcole='{config.IdEcole}', Denomination='{config.Denomination}'");

                UpdateProgress(60, "Vérification de l'école dans la base de données...");
                
                // Charger les données de l'école et initialiser le contexte
                var schoolData = await LoadSchoolData(config.IdEcole);

                UpdateProgress(80, "Préparation de la connexion...");
                await Task.Delay(300); // Petit délai pour l'UX

                // Vérification finale que ApplicationContext est bien initialisé
                if (!EduKinContext.IsConfigured)
                {
                    System.Diagnostics.Debug.WriteLine("ERREUR: ApplicationContext n'est pas configuré après LoadSchoolData");
                    throw new InvalidOperationException("Le contexte de l'école n'a pas été correctement initialisé.");
                }

                System.Diagnostics.Debug.WriteLine($"SUCCESS: ApplicationContext initialisé avec '{EduKinContext.CurrentDenomination}'");
                UpdateProgress(100, "Chargement terminé !");
                await Task.Delay(500);

                NavigateToLogin();
            }
            catch (InvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERREUR InvalidOperationException: {ex.Message}");
                
                // Afficher l'erreur à l'utilisateur
                MessageBox.Show(
                    $"Erreur de configuration : {ex.Message}\n\n" +
                    "La configuration va être réinitialisée.",
                    "Erreur de configuration",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                // Supprimer la configuration problématique
                try
                {
                    _configManager.DeleteConfig();
                    EduKinContext.Clear();
                }
                catch (Exception deleteEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Erreur lors de la suppression de la config: {deleteEx.Message}");
                }

                NavigateToConfig();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERREUR inattendue dans CheckConfiguration: {ex.Message}");
                
                MessageBox.Show(
                    $"Erreur inattendue lors de l'initialisation : {ex.Message}\n\n" +
                    "L'application va redémarrer la configuration.",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                // En cas d'erreur inattendue, rediriger vers la configuration
                NavigateToConfig();
            }
        }

        /// <summary>
        /// Charge les données de l'école depuis la base de données et initialise ApplicationContext
        /// </summary>
        private async Task<EcoleModel> LoadSchoolData(string idEcole)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== DEBUT LoadSchoolData pour idEcole: '{idEcole}' ===");

                // Utiliser la méthode spéciale qui bypasse la vérification de ApplicationContext
                var ecoleData = await Task.Run(() => _administrations.GetEcoleForInitialization(idEcole));

                if (ecoleData == null)
                {
                    System.Diagnostics.Debug.WriteLine($"ERREUR: Aucune école trouvée avec l'ID '{idEcole}'");
                    throw new InvalidOperationException($"L'école '{idEcole}' n'existe pas dans la base de données.");
                }

                System.Diagnostics.Debug.WriteLine($"École trouvée: {ecoleData}");

                // Extraction 100% sécurisée des données
                string denomination = ecoleData.denomination?.ToString() ?? "École inconnue";
                string anneeScol = DateTime.Now.Year.ToString(); // Simplifier pour éviter les erreurs

                System.Diagnostics.Debug.WriteLine($"Données extraites - Denomination: '{denomination}', Année: '{anneeScol}'");

                // Initialiser ApplicationContext sur le thread UI de manière synchrone
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        EduKinContext.InitializeSchool(idEcole, denomination);
                        System.Diagnostics.Debug.WriteLine($"ApplicationContext initialisé via Invoke");
                    }));
                }
                else
                {
                    EduKinContext.InitializeSchool(idEcole, denomination);
                    System.Diagnostics.Debug.WriteLine($"ApplicationContext initialisé directement");
                }

                // Valider que l'initialisation a réussi
                if (!EduKinContext.IsConfigured)
                {
                    System.Diagnostics.Debug.WriteLine("ERREUR: ApplicationContext n'est pas configuré après initialisation");
                    throw new InvalidOperationException("Échec de l'initialisation du contexte de l'école.");
                }

                System.Diagnostics.Debug.WriteLine($"SUCCESS: ApplicationContext initialisé avec '{EduKinContext.CurrentDenomination}'");

                // Retourner un modèle typé au lieu de dynamic
                return new EcoleModel
                {
                    IdEcole = idEcole,
                    Denomination = denomination,
                    AnneeScol = anneeScol,
                    Adresse = ecoleData.FkAvenue?.ToString() ?? "",
                    Telephone = "", // Ces champs n'existent pas dans t_ecoles
                    Email = ""      // Ces champs n'existent pas dans t_ecoles
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERREUR dans LoadSchoolData: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                throw new InvalidOperationException($"Erreur lors du chargement des données de l'école: {ex.Message}", ex);
            }
        }



        /// <summary>
        /// Met à jour la barre de progression et le message
        /// </summary>
        private void UpdateProgress(int percentage, string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgress(percentage, message)));
                return;
            }

            progressBar.Value = percentage;
            lblStatus.Text = message;
            Application.DoEvents();
        }

        /// <summary>
        /// Navigue vers le formulaire de configuration
        /// </summary>
        private void NavigateToConfig()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== DEBUT NavigateToConfig ===");
                
                this.Hide();
                System.Diagnostics.Debug.WriteLine("FormStart masqué");

                var formConfig = new FormConfig();
                System.Diagnostics.Debug.WriteLine("FormConfig créé");
                
                formConfig.Show();
                System.Diagnostics.Debug.WriteLine("FormConfig affiché");

                // Ne pas fermer le formulaire de démarrage pour éviter la fermeture de l'application
                System.Diagnostics.Debug.WriteLine("=== FIN NavigateToConfig ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERREUR dans NavigateToConfig: {ex.Message}");
                MessageBox.Show($"Erreur lors de la navigation vers la configuration: {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Navigue vers le formulaire de connexion
        /// </summary>
        private void NavigateToLogin()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== DEBUT NavigateToLogin ===");
                
                var formLogin = new FormLogin();
                System.Diagnostics.Debug.WriteLine("FormLogin créé");
                
                // Configurer l'événement de fermeture pour fermer l'application
                formLogin.FormClosed += (s, e) => {
                    System.Diagnostics.Debug.WriteLine("FormLogin fermé - Fermeture de l'application");
                    Application.Exit();
                };
                
                this.Hide();
                System.Diagnostics.Debug.WriteLine("FormStart masqué");
                
                formLogin.Show();
                System.Diagnostics.Debug.WriteLine("FormLogin affiché");

                // NE PAS fermer FormStart pour éviter la fermeture de l'application
                System.Diagnostics.Debug.WriteLine("=== FIN NavigateToLogin ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERREUR dans NavigateToLogin: {ex.Message}");
                MessageBox.Show($"Erreur lors de la navigation vers la connexion: {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panelMain_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
