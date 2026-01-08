using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dapper;
using EduKin.DataSets;
using BCrypt.Net;

namespace EduKin.Layouts
{
    public partial class FormAuthDialog : Form
    {
        private readonly Connexion _connexion;
        private string _superAdminRoleId = string.Empty;

        public string AuthenticatedUserId { get; private set; } = string.Empty;
        public string UserIndex { get; private set; } = string.Empty;
        public string UserRole { get; private set; } = string.Empty;

        public FormAuthDialog()
        {
            InitializeComponent();
            _connexion = Connexion.Instance;
            InitializeForm();
        }

        private void InitializeForm()
        {
            Text = "Authentification requise";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            lblError.Visible = false;
            this.Load += (s, e) => txtUsername.Focus();
        }

        private async void btnAuthenticate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowError("Veuillez saisir le nom d'utilisateur et le mot de passe.");
                return;
            }

            btnAuthenticate.Enabled = false;

            try
            {
                var result = await AuthenticateAsync(
                    txtUsername.Text.Trim(),
                    txtPassword.Text
                );

                if (!result.Success)
                {
                    ShowError(result.ErrorMessage ?? "Identifiants incorrects ou compte verrouillé.");
                    return;
                }

                AuthenticatedUserId = result.UserId;
                UserIndex = result.UserIndex;
                UserRole = result.UserRole;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                ShowError($"Erreur d'authentification : {ex.Message}");
            }
            finally
            {
                btnAuthenticate.Enabled = true;
                txtPassword.Clear();
            }
        }

        /// <summary>
        /// Récupère l'ID du rôle Super Administrateur depuis la base de données
        /// </summary>
        private async Task<string> GetSuperAdminRoleIdAsync()
        {
            if (!string.IsNullOrEmpty(_superAdminRoleId))
                return _superAdminRoleId;

            try
            {
                using var conn = _connexion.GetConnection();
                conn.Open();

                const string sql = @"
                    SELECT id_role 
                    FROM t_roles 
                    WHERE nom_role = 'Super Administrateur' 
                    LIMIT 1";

                _superAdminRoleId = await conn.QueryFirstOrDefaultAsync<string>(sql);
                return _superAdminRoleId ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private async Task<AuthResult> AuthenticateAsync(string username, string password)
        {
            using var conn = _connexion.GetConnection();
            conn.Open();

            const string sql = @"
                SELECT 
                    u.id_user,
                    u.username,
                    u.pwd_hash,
                    u.user_index,
                    u.fk_role as role_id,
                    r.nom_role as fk_role,
                    u.type_user,
                    u.compte_verrouille,
                    u.account_locked_until
                FROM t_users_infos u
                LEFT JOIN t_roles r ON u.fk_role = r.id_role
                WHERE LOWER(u.username) = LOWER(@username)
                LIMIT 1";

            var user = await conn.QueryFirstOrDefaultAsync<UserAuthDto>(
                sql, new { username });

            if (user == null)
                return AuthResult.FailedWith("Utilisateur introuvable.");

            if (user.compte_verrouille ||
                (user.account_locked_until.HasValue &&
                 user.account_locked_until > DateTime.Now))
            {
                return AuthResult.FailedWith("Compte verrouillé.");
            }

            // Récupérer dynamiquement l'ID du Super Admin
            string superAdminRoleId = await GetSuperAdminRoleIdAsync();
            
            // Vérifier que l'utilisateur a les droits d'accès (Super Admin ou SYSTEM)
            if (!IsUserAuthorized(user, superAdminRoleId))
            {
                return AuthResult.FailedWith("Accès refusé : Permissions insuffisantes.");
            }

            if (!VerifyPassword(password, user.pwd_hash))
                return AuthResult.FailedWith("Mot de passe incorrect.");

            return new AuthResult
            {
                Success = true,
                UserId = user.id_user,
                UserIndex = user.user_index.ToString().PadLeft(3, '0'),
                UserRole = user.fk_role ?? string.Empty
            };
        }

        /// <summary>
        /// Vérifie si l'utilisateur est autorisé à accéder au système
        /// </summary>
        private bool IsUserAuthorized(UserAuthDto user, string superAdminRoleId)
        {
            // Utilisateur système : toujours autorisé
            if (user.type_user == "SYSTEM")
                return true;

            // Utilisateur d'école : doit être Super Admin
            if (!string.IsNullOrEmpty(superAdminRoleId) && user.role_id == superAdminRoleId)
                return true;

            return false;
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hashedPassword))
                {
                    System.Diagnostics.Debug.WriteLine("[AUTH] Hash is null or empty");
                    return false;
                }

                // Vérification BCrypt
                bool result = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                System.Diagnostics.Debug.WriteLine($"[AUTH] BCrypt.Verify result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AUTH] Exception in VerifyPassword: {ex.Message}");
                return false;
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.ForeColor = System.Drawing.Color.Red;
            lblError.Visible = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                btnAuthenticate_Click(sender, e);
        }

        private void txtUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                txtPassword.Focus();
            }
        }
    }

    internal sealed class UserAuthDto
    {
        public string id_user { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string pwd_hash { get; set; } = string.Empty;
        public int user_index { get; set; }
        public string? fk_role { get; set; }
        public string? role_id { get; set; }
        public string? type_user { get; set; }
        public bool compte_verrouille { get; set; }
        public DateTime? account_locked_until { get; set; }
    }

    internal sealed class AuthResult
    {
        public static AuthResult Failed => new() { Success = false };
        public static AuthResult FailedWith(string message) => new() { Success = false, ErrorMessage = message };

        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserIndex { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
    }
}

