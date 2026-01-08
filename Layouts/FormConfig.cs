using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using EduKin.DataSets;
using EduKin.Inits;
using EduKin.Csharp.Admins;
using EduKinContext = EduKin.Inits.EduKinContext; // Résoudre le conflit de noms

namespace EduKin.Layouts
{
    /// <summary>
    /// Formulaire de configuration de l'école avec sélection ou création
    /// </summary>
    public partial class FormConfig : Form
    {
        private readonly SchoolConfigManager _configManager;
        private readonly Connexion _connexion;
        private readonly Administrations _administrations;
        private bool _isCreatingNewSchool = false;

        public FormConfig()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== DEBUT Constructeur FormConfig ===");
                
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("InitializeComponent() terminé");
                
                _configManager = new SchoolConfigManager();
                System.Diagnostics.Debug.WriteLine("SchoolConfigManager créé");
                
                _connexion = Connexion.Instance;
                System.Diagnostics.Debug.WriteLine("Connexion.Instance récupéré");
                
                _administrations = new Administrations();
                System.Diagnostics.Debug.WriteLine("Administrations créé");
                
                // S'abonner aux changements de connexion
                _connexion.ConnectionChanged += OnConnectionChanged;
                System.Diagnostics.Debug.WriteLine("Événement ConnectionChanged abonné");
                
                System.Diagnostics.Debug.WriteLine("=== FIN Constructeur FormConfig ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERREUR dans constructeur FormConfig: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                MessageBox.Show(
                    $"Erreur lors de l'initialisation de FormConfig:\n{ex.Message}",
                    "Erreur Critique",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                
                throw;
            }
        }

        /// <summary>
        /// Gère les changements de connexion en temps réel
        /// </summary>
        private async void OnConnectionChanged(object? sender, ConnectionChangedEventArgs e)
        {
            // Mettre à jour l'interface utilisateur sur le thread principal
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnConnectionChanged(sender, e)));
                return;
            }

            var message = e.IsOnline 
                ? $"🟢 Connexion rétablie - {e.DatabaseType}"
                : $"🔴 Mode hors ligne - {e.DatabaseType}";

            // Afficher le statut dans l'interface (vous pouvez ajouter un label pour cela)
            this.Text = $"Configuration École - {message}";
            
            // Afficher une notification discrète
            MessageBox.Show(message, "Changement de connexion", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Se désabonner de l'événement
            if (_connexion != null)
            {
                _connexion.ConnectionChanged -= OnConnectionChanged;
            }
            base.OnFormClosed(e);
        }

        private async void FormConfig_Load(object sender, EventArgs e)
        {
            // Afficher le statut de connexion au démarrage
            var (success, message) = _connexion.TestConnection();
            
            if (!success)
            {
                // Afficher pourquoi MySQL n'est pas accessible
                var result = MessageBox.Show(
                    $"{message}\n\nVoulez-vous continuer en mode hors ligne (SQLite) ?",
                    "Connexion MySQL échouée",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    this.Close();
                    return;
                }
            }
            else
            {
                // Connexion MySQL réussie
                System.Diagnostics.Debug.WriteLine(message);
                
                // Vérifier et corriger le schéma de la base de données si nécessaire
                EnsureSchemaCorrection();
            }

            // Initialiser les vues SQLite si nécessaire (mode hors ligne)
            if (!_connexion.IsOnline)
            {
                var initializer = new SQLiteInitializer();
                if (!initializer.ViewsExist())
                {
                    var result = MessageBox.Show(
                        "Les vues de la base de données locale doivent être initialisées.\nVoulez-vous continuer ?",
                        "Initialisation requise",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        if (!initializer.InitializeViews())
                        {
                            MessageBox.Show(
                                "Erreur lors de l'initialisation des vues.\nVeuillez vérifier que la base de données contient les tables nécessaires.",
                                "Erreur",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }
                        
                        MessageBox.Show(
                            "Vues initialisées avec succès !",
                            "Succès",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        return;
                    }
                }
            }

            // Mettre à jour le statut de connexion dans le titre
            var dbInfo = _connexion.GetCurrentDatabase();
            var statusIcon = _connexion.IsOnline ? "🟢" : "🔴";
            this.Text = $"Configuration École - {statusIcon} {dbInfo}";
            
            btnSelectSchool.Enabled = false;
        }

        #region Recherche d'avenue optimisée

        /// <summary>
        /// Recherche les avenues correspondant au texte saisi
        /// </summary>
        private async Task SearchAvenues(string searchText)
        {
            try
            {
                // Forcer une vérification de connexion avant de charger
                _connexion.ForceCheckConnection();
                
                var dbInfo = _connexion.GetCurrentDatabase();
                var statusIcon = _connexion.IsOnline ? "🟢" : "🔴";
                
                // Mettre à jour le titre de la fenêtre avec le statut
                this.Text = $"Configuration École - {statusIcon} {dbInfo}";

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    lstAvenues.Items.Clear();
                    lstEcoles.Items.Clear();
                    lblNoSchool.Visible = false;
                    btnSelectSchool.Enabled = false;
                    return;
                }

                // Rechercher dans la vue vue_avenue_hierarchie
                var query = @"
                    SELECT DISTINCT 
                        id_avenue,
                        Avenue, 
                        Quartier, 
                        Commune, 
                        Ville, 
                        Province
                    FROM vue_avenue_hierarchie 
                    WHERE Avenue LIKE @searchText
                    ORDER BY Avenue, Quartier, Commune";
                
                var dataTable = await Task.Run(() => 
                    ExecuteQueryWithParameter(query, "@searchText", $"%{searchText}%"));
                
                lstAvenues.Items.Clear();
                
                foreach (DataRow row in dataTable.Rows)
                {
                    // Format: Avenue -> Quartier -> Commune -> Ville -> Province
                    var hierarchyText = $"{row["Avenue"]} → {row["Quartier"]} → {row["Commune"]} → {row["Ville"]} → {row["Province"]}";
                    var item = new ListViewItem(hierarchyText);
                    item.Tag = new AvenueInfo
                    {
                        IdAvenue = row["id_avenue"].ToString(),
                        Avenue = row["Avenue"].ToString(),
                        Quartier = row["Quartier"].ToString(),
                        Commune = row["Commune"].ToString(),
                        Ville = row["Ville"].ToString(),
                        Province = row["Province"].ToString()
                    };
                    lstAvenues.Items.Add(item);
                }
                
                // Réinitialiser la liste des écoles
                lstEcoles.Items.Clear();
                lblNoSchool.Visible = false;
                btnSelectSchool.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la recherche d'avenues : {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Charge les écoles de l'avenue sélectionnée
        /// </summary>
        private async Task LoadEcolesByAvenue(AvenueInfo avenueInfo)
        {
            try
            {
                // Utiliser la vue vue_ecole qui contient les informations complètes des écoles
                var query = @"
                    SELECT id_ecole, Ecole as denomination, Avenue, NumParcelle as numero
                    FROM vue_ecole 
                    WHERE Avenue = @avenue 
                      AND Quartier = @quartier
                    ORDER BY Ecole";
                
                var dataTable = await Task.Run(() => 
                {
                    var dt = new DataTable();
                    using (var conn = _connexion.GetConnection())
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = query;
                            
                            var paramAvenue = cmd.CreateParameter();
                            paramAvenue.ParameterName = "@avenue";
                            paramAvenue.Value = avenueInfo.Avenue;
                            cmd.Parameters.Add(paramAvenue);
                            
                            var paramQuartier = cmd.CreateParameter();
                            paramQuartier.ParameterName = "@quartier";
                            paramQuartier.Value = avenueInfo.Quartier;
                            cmd.Parameters.Add(paramQuartier);
                            
                            using (var reader = cmd.ExecuteReader())
                            {
                                dt.Load(reader);
                            }
                        }
                    }
                    return dt;
                });
                
                lstEcoles.Items.Clear();
                
                foreach (DataRow row in dataTable.Rows)
                {
                    var item = new ListViewItem(row["denomination"].ToString());
                    item.SubItems.Add(row["Avenue"].ToString());
                    item.SubItems.Add(row["numero"].ToString());
                    item.Tag = row["id_ecole"].ToString();
                    lstEcoles.Items.Add(item);
                }
                
                if (lstEcoles.Items.Count == 0)
                {
                    lblNoSchool.Visible = true;
                }
                else
                {
                    lblNoSchool.Visible = false;
                }
                
                btnSelectSchool.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des écoles : {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Classe pour stocker les informations d'une avenue
        /// </summary>
        private class AvenueInfo
        {
            public string IdAvenue { get; set; } = string.Empty;
            public string Avenue { get; set; } = string.Empty;
            public string Quartier { get; set; } = string.Empty;
            public string Commune { get; set; } = string.Empty;
            public string Ville { get; set; } = string.Empty;
            public string Province { get; set; } = string.Empty;
        }

        #endregion

        #region Événements de recherche d'avenue

        /// <summary>
        /// Gère le changement de texte dans le TextBox de recherche d'avenue
        /// </summary>
        private async void txtAvenue_TextChanged(object sender, EventArgs e)
        {
            // Attendre un peu avant de lancer la recherche (debounce)
            await Task.Delay(300);
            
            if (txtAvenue.Text == ((TextBox)sender).Text)
            {
                await SearchAvenues(txtAvenue.Text);
            }
        }

        /// <summary>
        /// Gère la sélection d'une avenue dans la ListBox
        /// </summary>
        private async void lstAvenues_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstAvenues.SelectedItems.Count > 0)
            {
                var selectedItem = lstAvenues.SelectedItems[0];
                if (selectedItem.Tag is AvenueInfo avenueInfo)
                {
                    await LoadEcolesByAvenue(avenueInfo);
                }
            }
        }

        #endregion

        #region Sélection d'école existante

        private void lstEcoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelectSchool.Enabled = lstEcoles.SelectedItems.Count > 0;
        }

        private void btnSelectSchool_Click(object sender, EventArgs e)
        {
            if (lstEcoles.SelectedItems.Count == 0) return;
            
            try
            {
                var selectedItem = lstEcoles.SelectedItems[0];
                var idEcole = selectedItem.Tag.ToString();
                var denomination = selectedItem.Text;
                
                // Demander l'authentification avant de configurer l'école
                string userIndex;
                string authenticatedUserId;
                if (!AuthenticateAdmin())
                {
                    MessageBox.Show("Authentification échouée. Seuls les Super Administrateurs, Administrateurs ou Directeurs peuvent configurer une école.", 
                        "Accès refusé", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Récupérer les informations d'authentification
                using (var authDialog = new FormAuthDialog())
                {
                    if (authDialog.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    userIndex = authDialog.UserIndex;
                    authenticatedUserId = authDialog.AuthenticatedUserId;
                }
                
                // Créer et sauvegarder la configuration
                var config = new SchoolConfig
                {
                    IdEcole = idEcole,
                    Denomination = denomination,
                    ConfiguredDate = DateTime.Now
                };
                
                _configManager.SaveConfig(config);
                
                // Initialiser le contexte de l'école
                EduKinContext.Initialize(idEcole, denomination);
                
                // Initialiser le contexte complet avec l'année scolaire active
                var schoolYearManager = new SchoolYearManager();
                var contextInitialized = schoolYearManager.InitializeContextWithActiveYear(
                    idEcole: idEcole,
                    userId: authenticatedUserId,
                    username: authenticatedUserId
                );

                if (!contextInitialized)
                {
                    MessageBox.Show("École sélectionnée mais erreur lors de l'initialisation de l'année scolaire.", 
                        "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                
                // Debug: Vérifier que l'initialisation a fonctionné
                var debugMessage = $"École '{denomination}' configurée avec succès!\n\n" +
                                 $"Debug Info:\n" +
                                 $"ID École: {idEcole}\n" +
                                 $"Dénomination: {denomination}\n" +
                                 $"Contexte configuré: {EduKinContext.IsConfigured}\n" +
                                 $"ID Contexte: {(EduKinContext.IsConfigured ? EduKinContext.TryGetCurrentIdEcole() : "Non disponible")}";
                
                MessageBox.Show(debugMessage, "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                NavigateToLogin();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la configuration : {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Basculement entre sélection et création

        private void btnToggleMode_Click(object sender, EventArgs e)
        {
            _isCreatingNewSchool = !_isCreatingNewSchool;
            
            if (_isCreatingNewSchool)
            {
                // Passer en mode création
                panelSelection.Visible = false;
                panelCreation.Visible = true;
                btnToggleMode.Text = "Sélectionner une école existante";
            }
            else
            {
                // Passer en mode sélection
                panelSelection.Visible = true;
                panelCreation.Visible = false;
                btnToggleMode.Text = "Créer une nouvelle école";
            }
        }

        #endregion

        #region Création de nouvelle école

        /// <summary>
        /// Gère le changement de texte dans le TextBox de recherche d'avenue pour la création
        /// </summary>
        private async void txtNewAvenue_TextChanged(object sender, EventArgs e)
        {
            // Attendre un peu avant de lancer la recherche (debounce)
            await Task.Delay(300);
            
            if (txtNewAvenue.Text == ((TextBox)sender).Text)
            {
                await SearchAvenuesForCreation(txtNewAvenue.Text);
            }
        }

        /// <summary>
        /// Recherche les avenues pour la création d'école
        /// </summary>
        private async Task SearchAvenuesForCreation(string searchText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    lstNewAvenues.Items.Clear();
                    return;
                }

                // Rechercher dans la vue vue_avenue_hierarchie
                var query = @"
                    SELECT DISTINCT 
                        id_avenue,
                        Avenue, 
                        Quartier, 
                        Commune, 
                        Ville, 
                        Province
                    FROM vue_avenue_hierarchie 
                    WHERE Avenue LIKE @searchText
                    ORDER BY Avenue, Quartier, Commune";
                
                var dataTable = await Task.Run(() => 
                    ExecuteQueryWithParameter(query, "@searchText", $"%{searchText}%"));
                
                lstNewAvenues.Items.Clear();
                
                foreach (DataRow row in dataTable.Rows)
                {
                    // Format: Avenue -> Quartier -> Commune -> Ville -> Province
                    var hierarchyText = $"{row["Avenue"]} → {row["Quartier"]} → {row["Commune"]} → {row["Ville"]} → {row["Province"]}";
                    var item = new ListViewItem(hierarchyText);
                    item.Tag = new AvenueInfo
                    {
                        IdAvenue = row["id_avenue"].ToString(),
                        Avenue = row["Avenue"].ToString(),
                        Quartier = row["Quartier"].ToString(),
                        Commune = row["Commune"].ToString(),
                        Ville = row["Ville"].ToString(),
                        Province = row["Province"].ToString()
                    };
                    lstNewAvenues.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la recherche d'avenues : {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gère la sélection d'une avenue dans la ListBox pour la création
        /// </summary>
        private void lstNewAvenues_SelectedIndexChanged(object sender, EventArgs e)
        {
            // L'avenue sélectionnée sera utilisée lors de la création
            btnCreateSchool.Enabled = lstNewAvenues.SelectedItems.Count > 0 && 
                                      !string.IsNullOrWhiteSpace(txtDenomination?.Text);
        }

        #endregion

        #region Méthodes utilitaires

        /// <summary>
        /// Vérifie et corrige le schéma de la base de données si nécessaire
        /// </summary>
        private void EnsureSchemaCorrection()
        {
            try
            {
                if (_connexion.IsOnline) // MySQL correction
                {
                    // Vérifier le type de colonne pour id_ecole dans t_annee_scolaire
                    var checkQuery = @"
                        SELECT DATA_TYPE 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_SCHEMA = DATABASE() 
                          AND TABLE_NAME = 't_annee_scolaire' 
                          AND COLUMN_NAME = 'id_ecole'";

                    var dt = ExecuteQuery(checkQuery);
                    
                    if (dt.Rows.Count > 0)
                    {
                        var dataType = dt.Rows[0]["DATA_TYPE"].ToString().ToLower();
                        if (dataType == "int" || dataType == "integer" || dataType == "smallint" || dataType == "tinyint")
                        {
                            System.Diagnostics.Debug.WriteLine($"[SchemaCorrection] Correction requise: id_ecole est {dataType}, doit être VARCHAR");
                            
                            using (var conn = _connexion.GetConnection())
                            {
                                conn.Open();
                                using (var cmd = conn.CreateCommand())
                                {
                                    // Modification critique : changer INT en VARCHAR pour supporter les IDs générés
                                    cmd.CommandText = "ALTER TABLE t_annee_scolaire MODIFY COLUMN id_ecole VARCHAR(50) NOT NULL";
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            System.Diagnostics.Debug.WriteLine("[SchemaCorrection] Schéma corrigé avec succès");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Ne pas bloquer l'application mais logger l'erreur
                System.Diagnostics.Debug.WriteLine($"[SchemaCorrection] Erreur: {ex.Message}");
            }
        }

        /// <summary>
        /// Exécute une requête SQL et retourne un DataTable
        /// </summary>
        private DataTable ExecuteQuery(string query)
        {
            var dt = new DataTable();
            try
            {
                // Debug : afficher la requête exacte
                System.Diagnostics.Debug.WriteLine($"Requête SQL : {query}");
                
                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = query;
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Afficher la requête qui a causé l'erreur
                var errorMessage = $"Erreur de base de données : {ex.Message}\n\nRequête : {query}";
                throw new Exception(errorMessage, ex);
            }
            return dt;
        }

        /// <summary>
        /// Exécute une requête SQL avec un paramètre et retourne un DataTable
        /// </summary>
        private DataTable ExecuteQueryWithParameter(string query, string paramName, string paramValue)
        {
            var dt = new DataTable();
            try
            {
                // Debug : afficher la requête exacte
                System.Diagnostics.Debug.WriteLine($"Requête SQL : {query}");
                System.Diagnostics.Debug.WriteLine($"Paramètre {paramName} : {paramValue}");
                
                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = query;
                        
                        // Ajouter le paramètre de manière sécurisée
                        var parameter = cmd.CreateParameter();
                        parameter.ParameterName = paramName;
                        parameter.Value = (object?)paramValue ?? DBNull.Value;
                        cmd.Parameters.Add(parameter);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Afficher la requête qui a causé l'erreur
                var errorMessage = $"Erreur de base de données : {ex.Message}\n\nRequête : {query}\nParamètre {paramName} : {paramValue}";
                throw new Exception(errorMessage, ex);
            }
            return dt;
        }

        /// <summary>
        /// Navigue vers le formulaire de connexion
        /// </summary>
        private void NavigateToLogin()
        {
            this.Hide();
            var formLogin = new FormLogin();
            formLogin.Show();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Êtes-vous sûr de vouloir annuler la configuration ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Authentifie un administrateur pour la configuration d'école
        /// </summary>
        private bool AuthenticateAdmin()
        {
            using (var authDialog = new FormAuthDialog())
            {
                return authDialog.ShowDialog() == DialogResult.OK;
            }
        }

        /// <summary>
        /// Gère le changement de texte dans le champ dénomination
        /// </summary>
        private void txtDenomination_TextChanged(object sender, EventArgs e)
        {
            btnCreateSchool.Enabled = lstNewAvenues.SelectedItems.Count > 0 && 
                                      !string.IsNullOrWhiteSpace(txtDenomination?.Text);
        }

        /// <summary>
        /// Gère le clic sur le bouton de création d'école
        /// </summary>
        private async void btnCreateSchool_Click(object sender, EventArgs e)
        {
            await CreateNewSchool();
        }

        /// <summary>
        /// Crée une nouvelle école avec année scolaire
        /// </summary>
        private async Task CreateNewSchool()
        {
            try
            {
                // Validation des champs
                if (string.IsNullOrWhiteSpace(txtDenomination?.Text))
                {
                    MessageBox.Show("Veuillez saisir la dénomination de l'école.", "Validation", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (lstNewAvenues.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner une avenue.", "Validation", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedItem = lstNewAvenues.SelectedItems[0];
                if (selectedItem.Tag is not AvenueInfo avenueInfo)
                {
                    MessageBox.Show("Erreur lors de la récupération des informations de l'avenue.", "Erreur", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ÉTAPE 1: Authentification AVANT la création
                string userIndex;
                string authenticatedUserId;
                using (var authDialog = new FormAuthDialog())
                {
                    if (authDialog.ShowDialog() != DialogResult.OK)
                    {
                        MessageBox.Show("Authentification annulée. La création de l'école a été interrompue.", 
                            "Authentification requise", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    
                    userIndex = authDialog.UserIndex;
                    authenticatedUserId = authDialog.AuthenticatedUserId;
                    MessageBox.Show($"Authentification réussie !\nUtilisateur: {authenticatedUserId}\nRôle: {authDialog.UserRole}\nIndex: {userIndex}", 
                        "Authentification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // ÉTAPE 2: Récupérer les données du formulaire
                var denomination = txtDenomination.Text.Trim();
                var numeroParcelle = string.IsNullOrWhiteSpace(txtNumero?.Text) ? "N/A" : txtNumero.Text.Trim();
                
                // Gestion de l'année scolaire
                string anneeScol;
                DateTime dateDebut, dateFin;
                
                if (string.IsNullOrWhiteSpace(txtAnneeScol?.Text))
                {
                    // Générer automatiquement l'année scolaire courante
                    anneeScol = SchoolYearManager.GenerateCurrentSchoolYearCode();
                    var currentYear = DateTime.Now.Month >= 7 ? DateTime.Now.Year : DateTime.Now.Year - 1;
                    (dateDebut, dateFin) = SchoolYearManager.CalculateSchoolYearDates(currentYear);
                }
                else
                {
                    anneeScol = txtAnneeScol.Text.Trim();
                    // Essayer de parser l'année (format "2025-2026")
                    if (anneeScol.Contains("-") && anneeScol.Length == 9)
                    {
                        var parts = anneeScol.Split('-');
                        if (int.TryParse(parts[0], out int startYear))
                        {
                            (dateDebut, dateFin) = SchoolYearManager.CalculateSchoolYearDates(startYear);
                        }
                        else
                        {
                            MessageBox.Show("Format d'année scolaire invalide. Utilisez le format YYYY-YYYY (ex: 2025-2026)", 
                                "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Format d'année scolaire invalide. Utilisez le format YYYY-YYYY (ex: 2025-2026)", 
                            "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
             
                // ÉTAPE 3: Générer un ID unique pour l'école avec le user_index authentifié
                // ✅ Ne pas retirer les zéros, GenerateId() va les formater correctement
                var idEcole = _administrations.GenerateId("t_ecoles", "id_ecole", "ECO", userIndex);
                System.Diagnostics.Debug.WriteLine($"[FormConfig.CreateNewSchool] userIndex reçu: {userIndex}");
                System.Diagnostics.Debug.WriteLine($"[FormConfig.CreateNewSchool] ID École généré: {idEcole}");
                MessageBox.Show("création de l'id école : " + idEcole, "info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // ÉTAPE 4: Gérer le logo de l'école
                string? logoPath = null;
                using (var openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Tous les fichiers|*.*";
                    openFileDialog.Title = "Sélectionner le logo de l'école";
                    
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        var pictureManager = new PictureManager("Photos/Ecole");
                        logoPath = pictureManager.CopyToSecureLocation(openFileDialog.FileName, idEcole);
                        
                        if (!string.IsNullOrEmpty(logoPath))
                        {
                            MessageBox.Show($"Logo sauvegardé avec succès: {logoPath}", "Succès", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }

                // ÉTAPE 5: Créer l'école via la couche métier
                var success = _administrations.CreateEcole(
                    idEcole: idEcole,
                    denomination: denomination,
                    anneeScol: anneeScol,
                    fkAvenue: avenueInfo.IdAvenue,
                    numero: numeroParcelle,
                    logo: logoPath // Utiliser le chemin du logo sauvegardé
                );

                if (!success)
                {
                    MessageBox.Show("Erreur lors de la création de l'école dans la base de données.", 
                        "Erreur de création", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ÉTAPE 5: Créer l'année scolaire associée
                var schoolYearManager = new SchoolYearManager();
                var yearCreated = schoolYearManager.CreateSchoolYear(
                    idEcole: idEcole,
                    codeAnnee: anneeScol,
                    dateDebut: dateDebut,
                    dateFin: dateFin,
                    setAsActive: true // Première année = active par défaut
                );

                if (!yearCreated)
                {
                    MessageBox.Show("École créée mais erreur lors de la création de l'année scolaire.", 
                        "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // ÉTAPE 6: Créer et sauvegarder la configuration
                var config = new SchoolConfig
                {
                    IdEcole = idEcole,
                    Denomination = denomination,
                    ConfiguredDate = DateTime.Now
                };
                
                _configManager.SaveConfig(config);
                
                // 🔴 CRITIQUE: Initialiser EduKinContext avec l'idEcole généré
                // Note: InitializeComplete sera appelé plus tard via InitializeContextWithActiveYear
                EduKinContext.Initialize(idEcole, denomination);
                System.Diagnostics.Debug.WriteLine($"[FormConfig.CreateNewSchool] ✅ EduKinContext.Initialize() appelé");
                System.Diagnostics.Debug.WriteLine($"[FormConfig.CreateNewSchool] ID École: {idEcole}");

                // ÉTAPE 7: Initialiser le contexte d'isolation
                try {
                    var contextInitialized = schoolYearManager.InitializeContextWithActiveYear(
                        idEcole: idEcole,
                        userId: authenticatedUserId,
                        username: authenticatedUserId // Utiliser l'ID comme nom d'utilisateur temporaire
                    );

                    if (!contextInitialized)
                    {
                        MessageBox.Show("École créée mais erreur lors de l'initialisation du contexte.", 
                            "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"École créée mais erreur lors de l'initialisation du contexte: {ex.Message}", 
                        "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                MessageBox.Show(
                    $"École '{denomination}' créée et configurée avec succès !\n\n" +
                    $"ID École: {idEcole}\n" +
                    $"Adresse: {avenueInfo.Avenue}, {avenueInfo.Quartier}\n" +
                    $"N° Parcelle: {numeroParcelle}\n" +
                    $"Année Scolaire: {anneeScol}\n" +
                    $"Période: {dateDebut:dd/MM/yyyy} - {dateFin:dd/MM/yyyy}\n" +
                    $"User Index: {userIndex}\n" +
                    $"Contexte d'isolation: {(EduKinContext.IsConfigured ? "✓ Configuré" : "✗ Non configuré")}", 
                    "École créée", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);

                NavigateToLogin();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création de l'école : {ex.Message}", "Erreur", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
