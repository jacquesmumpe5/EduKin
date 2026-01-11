using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dapper;
using EduKin.DataSets;
using EduKin.Inits;
using EduKinContext = EduKin.Inits.EduKinContext; // Résoudre le conflit de noms

namespace EduKin.Layouts
{
    /// <summary>
    /// Formulaire de connexion utilisateur
    /// </summary>
    public partial class FormLogin : Form
    {
        private readonly Connexion _connexion;
        private readonly string _currentIdEcole;
        private int _attemptCount = 0;
        private const int MAX_ATTEMPTS = 3;
        private DateTime? _lockTime = null;
        private const int LOCK_DURATION_MINUTES = 30;

        public FormLogin()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== DEBUT Constructeur FormLogin ===");
                
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("InitializeComponent() terminé");
                
                _connexion = Connexion.Instance;
                System.Diagnostics.Debug.WriteLine("Connexion.Instance récupéré");
                
                // Validation défensive du contexte école avant utilisation
                ValidateSchoolContext();
                System.Diagnostics.Debug.WriteLine("ValidateSchoolContext() terminé");
                
                _currentIdEcole = EduKinContext.CurrentIdEcole;
                System.Diagnostics.Debug.WriteLine($"_currentIdEcole = '{_currentIdEcole}'");
                
                // Configuration initiale
                InitializeForm();
                System.Diagnostics.Debug.WriteLine("InitializeForm() terminé");
                
                System.Diagnostics.Debug.WriteLine("=== FIN Constructeur FormLogin ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERREUR dans constructeur FormLogin: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                MessageBox.Show(
                    $"Erreur lors de l'initialisation de FormLogin:\n{ex.Message}",
                    "Erreur Critique",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                
                throw;
            }
        }

        /// <summary>
        /// Valide que ApplicationContext est correctement initialisé
        /// </summary>
        private void ValidateSchoolContext()
        {
            if (!EduKinContext.IsConfigured)
            {
                MessageBox.Show(
                    "Le contexte de l'école n'est pas initialisé.\n\n" +
                    "L'application va vous rediriger vers la configuration.",
                    "Erreur de contexte",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                // Rediriger vers la configuration
                this.Hide();
                var formConfig = new FormConfig();
                formConfig.Show();
                this.Close();
                return;
            }

            System.Diagnostics.Debug.WriteLine($"ApplicationContext validé: {EduKinContext.CurrentDenomination}");
        }

        /// <summary>
        /// Initialise les paramètres du formulaire
        /// </summary>
        private void InitializeForm()
        {
            // Afficher le nom de l'école
            lblEcoleName.Text = EduKinContext.CurrentDenomination;
            lblEcoleInfo.Text = $"École: {EduKinContext.CurrentDenomination}";
            
            // Vérifier que le contrôle picLogoEcole est bien initialisé
            System.Diagnostics.Debug.WriteLine($"[InitializeForm] picLogoEcole est null: {picLogoEcole == null}");
            
            // Ne pas charger le logo ici - le faire dans FormLogin_Load pour s'assurer que tout est initialisé
            // LoadSchoolLogo();
            
            // Centrer le formulaire
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Configurer les contrôles
            txtUsername.Focus();
            txtPassword.UseSystemPasswordChar = true;
            lblError.Visible = false;
            
            // Vérifier si le compte est verrouillé
            CheckAccountLockStatus();
        }

        /// <summary>
        /// Charge et affiche le logo de l'école
        /// </summary>
        private void LoadSchoolLogo()
        {
            try
            {
                // Récupérer d'abord l'ID de l'école depuis la configuration
                var configManager = new SchoolConfigManager();
                var config = configManager.LoadConfig();
                
                if (config == null)
                {
                    System.Diagnostics.Debug.WriteLine("[LoadSchoolLogo] ❌ Aucune configuration d'école trouvée");
                    SetDefaultLogo();
                    return;
                }
                
                var idEcole = config.IdEcole;
                System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] ID École depuis config: {idEcole}");
                System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] picLogoEcole est null: {picLogoEcole == null}");
                
                if (picLogoEcole == null)
                {
                    System.Diagnostics.Debug.WriteLine("[LoadSchoolLogo] ❌ picLogoEcole est null - le contrôle n'est pas initialisé");
                    return;
                }
                
                using (var conn = _connexion.GetConnection())
                {
                    var query = "SELECT logo, denomination FROM t_ecoles WHERE id_ecole = @IdEcole";
                    var result = conn.QueryFirstOrDefault(query, new { IdEcole = idEcole });
                    
                    if (result != null)
                    {
                        var logoPath = result.logo?.ToString();
                        var denomination = result.denomination?.ToString() ?? "Inconnue";
                        
                        System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] École: {denomination}");
                        System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] Chemin du logo: '{logoPath}'");
                        
                        if (!string.IsNullOrEmpty(logoPath))
                        {
                            // Normaliser le chemin pour Windows
                            logoPath = logoPath.Replace('/', Path.DirectorySeparatorChar);
                            
                            System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] Chemin normalisé: '{logoPath}'");
                            System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] Fichier existe: {System.IO.File.Exists(logoPath)}");
                            System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] Chemin absolu: {Path.GetFullPath(logoPath)}");
                            System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] Répertoire courant: {Directory.GetCurrentDirectory()}");
                            System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] Base directory: {AppDomain.CurrentDomain.BaseDirectory}");
                            
                            if (System.IO.File.Exists(logoPath))
                            {
                                try
                                {
                                    System.Diagnostics.Debug.WriteLine("[LoadSchoolLogo] 📁 Tentative de chargement de l'image...");
                                    
                                    // Charger le logo de l'école directement
                                    var logoImage = Image.FromFile(logoPath);
                                    
                                    System.Diagnostics.Debug.WriteLine("[LoadSchoolLogo] ✅ Image chargée depuis fichier");
                                    
                                    // Libérer l'ancienne image si elle existe
                                    if (picLogoEcole.Image != null)
                                    {
                                        var oldImage = picLogoEcole.Image;
                                        picLogoEcole.Image = null;
                                        oldImage.Dispose();
                                        System.Diagnostics.Debug.WriteLine("[LoadSchoolLogo] 🗑️ Ancienne image libérée");
                                    }
                                    
                                    picLogoEcole.Image = logoImage;
                                    picLogoEcole.SizeMode = PictureBoxSizeMode.Zoom;
                                    picLogoEcole.Visible = true;
                                    
                                    // Forcer le rafraîchissement
                                    picLogoEcole.Refresh();
                                    picLogoEcole.Invalidate();
                                    
                                    System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] ✅ Logo AFFICHÉ: {logoPath}");
                                    return;
                                }
                                catch (Exception imgEx)
                                {
                                    System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] ❌ Erreur chargement image: {imgEx.Message}");
                                    System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] ❌ Type d'erreur: {imgEx.GetType().Name}");
                                    SetDefaultLogo();
                                    return;
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] ❌ Le fichier logo n'existe pas: {logoPath}");
                                System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] 🔍 Recherche dans le répertoire: {Path.GetDirectoryName(logoPath)}");
                                
                                // Lister les fichiers dans le répertoire pour débogage
                                try
                                {
                                    var dir = Path.GetDirectoryName(logoPath);
                                    if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                                    {
                                        var files = Directory.GetFiles(dir, "*.png");
                                        System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] 📁 Fichiers PNG trouvés: {string.Join(", ", files)}");
                                    }
                                }
                                catch (Exception dirEx)
                                {
                                    System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] ❌ Erreur lecture répertoire: {dirEx.Message}");
                                }
                                
                                SetDefaultLogo();
                                return;
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("[LoadSchoolLogo] ℹ️ Aucun chemin de logo défini pour cette école");
                            SetDefaultLogo();
                            return;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] ❌ École non trouvée: {idEcole}");
                        SetDefaultLogo();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] ❌ Erreur lors du chargement du logo: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] StackTrace: {ex.StackTrace}");
                SetDefaultLogo();
            }
        }

        /// <summary>
        /// Définit un logo par défaut vide
        /// </summary>
        private void SetDefaultLogo()
        {
            try
            {
                if (picLogoEcole != null)
                {
                    // Libérer l'ancienne image
                    if (picLogoEcole.Image != null)
                    {
                        var oldImage = picLogoEcole.Image;
                        picLogoEcole.Image = null;
                        oldImage.Dispose();
                    }
                    
                    // Logo par défaut : vide ou une image simple
                    var defaultBitmap = new Bitmap(200, 200);
                    using (var g = Graphics.FromImage(defaultBitmap))
                    {
                        g.Clear(Color.Transparent);
                        g.DrawString("LOGO", new Font("Arial", 10, FontStyle.Bold), Brushes.LightGray, 70, 90);
                    }
                    
                    picLogoEcole.Image = defaultBitmap;
                    picLogoEcole.SizeMode = PictureBoxSizeMode.Zoom;
                    picLogoEcole.Visible = true;
                    picLogoEcole.Refresh();
                    
                    System.Diagnostics.Debug.WriteLine("[LoadSchoolLogo] ℹ️ Logo par défaut défini");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadSchoolLogo] ❌ Erreur logo par défaut: {ex.Message}");
            }
        }

        /// <summary>
        /// Vérifie le statut de verrouillage du compte
        /// </summary>
        private async void CheckAccountLockStatus()
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();
                    
                    var query = @"
                        SELECT compte_verrouille, date_verrouillage 
                        FROM t_users_infos 
                        WHERE id_ecole = @idEcole 
                        LIMIT 1";
                    
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = query;
                        cmd.Parameters.Add(CreateParameter(cmd, "@idEcole", _currentIdEcole));
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var isLocked = !reader.IsDBNull(reader.GetOrdinal("compte_verrouille")) && 
                                             Convert.ToBoolean(reader["compte_verrouille"]);
                                
                                if (isLocked)
                                {
                                    var lockDate = Convert.ToDateTime(reader["date_verrouillage"]);
                                    var unlockTime = lockDate.AddMinutes(LOCK_DURATION_MINUTES);
                                    
                                    // Si le délai de verrouillage est passé, déverrouiller
                                    if (DateTime.Now > unlockTime)
                                    {
                                        await UnlockAccount();
                                    }
                                    else
                                    {
                                        _lockTime = unlockTime;
                                        ShowLockMessage();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification du verrouillage : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Affiche le message de verrouillage
        /// </summary>
        private void ShowLockMessage()
        {
            if (_lockTime.HasValue)
            {
                var remainingTime = _lockTime.Value - DateTime.Now;
                lblError.Text = $"Compte verrouillé. Réessayez dans {remainingTime.Minutes} minutes.";
                lblError.ForeColor = System.Drawing.Color.Red;
                lblError.Visible = true;
                btnLogin.Enabled = false;
            }
        }

        /// <summary>
        /// Déverrouille le compte
        /// </summary>
        private async Task UnlockAccount()
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();
                    
                    var query = @"
                        UPDATE t_users_infos 
                        SET compte_verrouille = 0, 
                            tentatives_connexion = 0,
                            date_verrouillage = NULL
                        WHERE id_ecole = @idEcole";
                    
                    await conn.ExecuteAsync(query, new { idEcole = _currentIdEcole });
                    
                    _lockTime = null;
                    _attemptCount = 0;
                    btnLogin.Enabled = true;
                    lblError.Text = "Compte déverrouillé.";
                    lblError.ForeColor = System.Drawing.Color.Green;
                    lblError.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du déverrouillage : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gère la connexion utilisateur
        /// </summary>
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || 
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblError.Text = "Veuillez saisir le nom d'utilisateur et le mot de passe.";
                lblError.ForeColor = System.Drawing.Color.Red;
                lblError.Visible = true;
                return;
            }

            if (_lockTime.HasValue && DateTime.Now < _lockTime.Value)
            {
                ShowLockMessage();
                return;
            }

            btnLogin.Enabled = false;
            lblError.Text = "Connexion en cours...";
            lblError.ForeColor = System.Drawing.Color.Blue;
            lblError.Visible = true;

            try
            {
                var loginResult = await AuthenticateUser(txtUsername.Text.Trim(), txtPassword.Text);
                
                if (loginResult.Success)
                {
                    // Connexion réussie
                    await ResetLoginAttempts();
                    
                    // Initialiser le contexte utilisateur pour l'isolation et la sécurité
                    // ✅ Passer le user_index récupéré depuis la base de données
                    EduKinContext.InitializeUser(
                        loginResult.UserId, 
                        loginResult.UserName, 
                        loginResult.UserType,
                        loginResult.UserIndex  // ✅ Index utilisateur dynamique
                    );
                    
                    lblError.Text = "Connexion réussie !";
                    lblError.ForeColor = System.Drawing.Color.Green;
                    lblError.Visible = true;
                    
                    MessageBox.Show($"Bienvenue {loginResult.UserName} !\nRole : {loginResult.UserType}",
                        "Connexion réussie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Créer et configurer le formulaire principal
                    FormMain mainForm = new FormMain();
                    
                    // Configurer l'événement de fermeture pour fermer l'application
                    mainForm.FormClosed += (s, e) => {
                        System.Diagnostics.Debug.WriteLine("FormMain fermé - Fermeture de l'application");
                        Application.Exit();
                    };
                    
                    // Masquer le formulaire de login
                    this.Hide();
                    
                    // Afficher le formulaire principal
                    mainForm.Show();
                    
                    // NE PAS fermer FormLogin pour éviter la fermeture de l'application
                    System.Diagnostics.Debug.WriteLine("Navigation vers FormMain réussie");
                }
                else
                {
                    // Connexion échouée
                    _attemptCount++;
                    await UpdateLoginAttempts();
                    
                    if (_attemptCount >= MAX_ATTEMPTS)
                    {
                        await LockAccount();
                        lblError.Text = "Compte verrouillé après 3 tentatives échouées.";
                        lblError.ForeColor = System.Drawing.Color.Red;
                        lblError.Visible = true;
                    }
                    else
                    {
                        lblError.Text = $"Identifiants incorrects. Tentative {_attemptCount}/{MAX_ATTEMPTS}";
                        lblError.ForeColor = System.Drawing.Color.Red;
                        lblError.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Erreur de connexion.";
                lblError.ForeColor = System.Drawing.Color.Red;
                lblError.Visible = true;
                
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                txtPassword.Clear();
            }
        }

        /// <summary>
        /// Authentifie l'utilisateur
        /// </summary>
        private async Task<LoginResult> AuthenticateUser(string username, string password)
        {
            using (var conn = _connexion.GetConnection())
            {
                conn.Open();
                
                // Requête modifiée pour inclure le type_user et permettre aux utilisateurs SYSTEM de se connecter
                var query = @"
                    SELECT u.id_user, u.username, u.pwd_hash, u.compte_verrouille, 
                           u.account_locked_until, u.user_index, u.type_user, r.nom_role
                    FROM t_users_infos u
                    LEFT JOIN t_roles r ON u.fk_role = r.id_role
                    WHERE u.username = @username 
                    AND u.compte_verrouille = 0
                    AND (u.account_locked_until IS NULL OR u.account_locked_until < NOW())
                    AND (
                        -- Utilisateurs SYSTEM : peuvent se connecter à n'importe quelle école
                        u.type_user = 'SYSTEM' OR
                        -- Utilisateurs d'école : doivent appartenir à l'école du contexte
                        (u.type_user != 'SYSTEM' AND u.id_ecole = @idEcole)
                    )";
                
                var user = await conn.QueryFirstOrDefaultAsync(query, new 
                { 
                    username = username, 
                    idEcole = _currentIdEcole 
                });
                
                if (user != null)
                {
                    // Vérifier le mot de passe avec bcrypt
                    if (VerifyPassword(password, user.pwd_hash))
                    {
                        // Utiliser directement user_index de la base de données
                        string userIndex = user.user_index.ToString().PadLeft(3, '0');
                        
                        // Journaliser le type d'utilisateur pour le débogage
                        System.Diagnostics.Debug.WriteLine($"[LOGIN] Utilisateur {username} connecté - Type: {user.type_user} - École: {user.id_ecole ?? "SYSTEM"}");
                        
                        return new LoginResult
                        {
                            Success = true,
                            UserId = user.id_user,
                            UserName = user.username,
                            UserType = user.nom_role ?? "Utilisateur",
                            UserIndex = userIndex
                        };
                    }
                }
                
                return new LoginResult { Success = false };
            }
        }

        /// <summary>
        /// Met à jour le nombre de tentatives de connexion
        /// </summary>
        private async Task UpdateLoginAttempts()
        {
            using (var conn = _connexion.GetConnection())
            {
                conn.Open();
                
                var query = @"
                    UPDATE t_users_infos 
                    SET tentatives_connexion = @attempts
                    WHERE id_ecole = @idEcole";
                
                await conn.ExecuteAsync(query, new 
                { 
                    attempts = _attemptCount, 
                    idEcole = _currentIdEcole 
                });
            }
        }

        /// <summary>
        /// Remet à zéro les tentatives de connexion
        /// </summary>
        private async Task ResetLoginAttempts()
        {
            using (var conn = _connexion.GetConnection())
            {
                conn.Open();
                
                var query = @"
                    UPDATE t_users_infos 
                    SET tentatives_connexion = 0,
                        derniere_connexion = @now
                    WHERE id_ecole = @idEcole";
                
                await conn.ExecuteAsync(query, new 
                { 
                    now = DateTime.Now, 
                    idEcole = _currentIdEcole 
                });
            }
        }

        /// <summary>
        /// Verrouille le compte
        /// </summary>
        private async Task LockAccount()
        {
            _lockTime = DateTime.Now.AddMinutes(LOCK_DURATION_MINUTES);
            
            using (var conn = _connexion.GetConnection())
            {
                conn.Open();
                
                var query = @"
                    UPDATE t_users_infos 
                    SET compte_verrouille = 1,
                        date_verrouillage = @lockTime
                    WHERE id_ecole = @idEcole";
                
                await conn.ExecuteAsync(query, new 
                { 
                    lockTime = DateTime.Now, 
                    idEcole = _currentIdEcole 
                });
            }
            
            btnLogin.Enabled = false;
        }

        /// <summary>
        /// Crée un paramètre pour la commande SQL
        /// </summary>
        private IDbDataParameter CreateParameter(IDbCommand cmd, string name, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }

        /// <summary>
        /// Vérifie le mot de passe avec bcrypt
        /// </summary>
        private bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                // Vérifier avec bcrypt
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                // Si bcrypt échoue, essayer avec SHA256 (fallback)
                try
                {
                    using (var sha256 = System.Security.Cryptography.SHA256.Create())
                    {
                        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                        var hash = sha256.ComputeHash(bytes);
                        var sha256Hash = Convert.ToBase64String(hash);
                        return sha256Hash == hashedPassword;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gère l'appui sur Entrée dans les champs de texte
        /// </summary>
        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private void txtUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                txtPassword.Focus();
            }
        }

        /// <summary>
        /// Gère le chargement du formulaire
        /// </summary>
        private void FormLogin_Load(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[FormLogin_Load] Début du chargement du formulaire");
                System.Diagnostics.Debug.WriteLine($"[FormLogin_Load] picLogoEcole est null: {picLogoEcole == null}");
                
                // Charger le logo de l'école maintenant que tous les contrôles sont initialisés
                LoadSchoolLogo();
                
                System.Diagnostics.Debug.WriteLine("[FormLogin_Load] Formulaire chargé avec succès");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FormLogin_Load] Erreur: {ex.Message}");
                MessageBox.Show($"Erreur lors du chargement du formulaire: {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gère le clic sur le bouton Annuler
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Gère le clic sur le lien de reconfiguration
        /// </summary>
        private void lnkReconfigure_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            var result = MessageBox.Show(
                "Êtes-vous sûr de vouloir reconfigurer l'école ? Cela supprimera la configuration actuelle.",
                "Reconfiguration",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var configManager = new SchoolConfigManager();
                    configManager.DeleteConfig();
                    EduKinContext.Clear();

                    MessageBox.Show(
                        "Configuration supprimée. L'application va redémarrer.",
                        "Reconfiguration",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    Application.Restart();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Erreur lors de la reconfiguration : {ex.Message}",
                        "Erreur",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }

    /// <summary>
    /// Résultat de l'authentification
    /// </summary>
    public class LoginResult
    {
        public bool Success { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public string UserIndex { get; set; } = "001"; // ✅ Index utilisateur pour génération d'IDs
    }
}