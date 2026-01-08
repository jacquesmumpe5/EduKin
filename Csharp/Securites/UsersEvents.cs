using Dapper;
using EduKin.DataSets;
using System.Security.Cryptography;
using System.Text;

namespace EduKin.Csharp.Securites
{
    public class UsersEvents
    {
        private readonly Connexion _connexion;
        private static string? _currentUser;
        private static string? _currentUserRole;

        public UsersEvents()
        {
            _connexion = Connexion.Instance;
        }

        public static string? CurrentUser => _currentUser;
        public static string? CurrentUserRole => _currentUserRole;

        #region Authentification

        public bool Login(string username, string password)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"SELECT u.*, r.nom_role as role 
                                  FROM t_users_infos u 
                                  LEFT JOIN t_roles r ON u.fk_role = r.id_role 
                                  WHERE u.username = @Username AND u.pwd_hash = @Password 
                                  AND u.compte_verrouille = 0 
                                  AND (u.account_locked_until IS NULL OR u.account_locked_until < NOW())";
                    
                    var hashedPassword = HashPassword(password);
                    var user = conn.QueryFirstOrDefault(query, new { Username = username, Password = hashedPassword });

                    if (user != null)
                    {
                        _currentUser = user.username;
                        _currentUserRole = user.role;
                        
                        // Réinitialiser les tentatives de connexion et mettre à jour la dernière connexion
                        var updateQuery = @"UPDATE t_users_infos 
                                           SET tentatives_connexion = 0, derniere_connexion = NOW() 
                                           WHERE id_user = @UserId";
                        conn.Execute(updateQuery, new { UserId = user.id_user });
                        
                        // Enregistrer l'événement de connexion
                        LogUserEvent(user.id_user, "LOGIN", "Connexion réussie");
                        
                        return true;
                    }
                    else
                    {
                        // Incrémenter les tentatives de connexion échouées
                        var updateFailedQuery = @"UPDATE t_users_infos 
                                                 SET tentatives_connexion = tentatives_connexion + 1,
                                                     failed_login_attempts = failed_login_attempts + 1
                                                 WHERE username = @Username";
                        conn.Execute(updateFailedQuery, new { Username = username });
                        
                        // Vérifier si le compte doit être verrouillé (après 5 tentatives)
                        var checkLockQuery = @"UPDATE t_users_infos 
                                              SET compte_verrouille = 1, 
                                                  date_verrouillage = NOW(),
                                                  account_locked_until = DATE_ADD(NOW(), INTERVAL 30 MINUTE)
                                              WHERE username = @Username AND tentatives_connexion >= 5";
                        conn.Execute(checkLockQuery, new { Username = username });
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la connexion: {ex.Message}", ex);
            }
        }

        public void Logout()
        {
            if (!string.IsNullOrEmpty(_currentUser))
            {
                try
                {
                    using (var conn = _connexion.GetConnection())
                    {
                        var query = "SELECT id_user FROM t_users_infos WHERE username = @Username";
                        var userId = conn.QueryFirstOrDefault<string>(query, new { Username = _currentUser });
                        
                        if (!string.IsNullOrEmpty(userId))
                        {
                            LogUserEvent(userId, "LOGOUT", "Déconnexion");
                        }
                    }
                }
                catch
                {
                    // Ignorer les erreurs lors de la déconnexion
                }
                
                _currentUser = null;
                _currentUserRole = null;
            }
        }

        public bool IsAuthenticated()
        {
            return !string.IsNullOrEmpty(_currentUser);
        }

        public bool HasRole(string role)
        {
            return _currentUserRole?.Equals(role, StringComparison.OrdinalIgnoreCase) ?? false;
        }

        #endregion

        #region CRUD Utilisateurs

        public bool CreateUser(string nom, string postNom, string prenom, string sexe, string username, string password, 
                              string telephone = null, string idEcole = null, string roleId = null, string profil = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    // Vérifier si l'utilisateur existe déjà
                    var checkQuery = "SELECT COUNT(*) FROM t_users_infos WHERE username = @Username";
                    object checkParameters = new { Username = username };
                    
                    // Si un téléphone est fourni, vérifier aussi l'unicité du téléphone
                    if (!string.IsNullOrWhiteSpace(telephone))
                    {
                        checkQuery += " OR telephone = @Telephone";
                        checkParameters = new { Username = username, Telephone = telephone };
                    }
                    
                    var exists = conn.ExecuteScalar<int>(checkQuery, checkParameters) > 0;

                    if (exists)
                    {
                        throw new Exception("Un utilisateur avec ce nom d'utilisateur ou téléphone existe déjà");
                    }

                    // Générer un ID utilisateur unique
                    var generateIdQuery = "CALL sp_generate_id('t_users_infos', 'id_user', 'USR', '001', @new_id)";
                    var parameters = new DynamicParameters();
                    parameters.Add("@new_id", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 128);
                    conn.Execute(generateIdQuery, parameters);
                    var newUserId = parameters.Get<string>("@new_id");

                    var hashedPassword = HashPassword(password);
                    var query = @"INSERT INTO t_users_infos (id_user, nom, postnom, prenom, sexe, username, pwd_hash, 
                                                           telephone, id_ecole, fk_role, profil, created_at, last_password_change) 
                                  VALUES (@IdUser, @Nom, @PostNom, @Prenom, @Sexe, @Username, @Password, 
                                         @Telephone, @IdEcole, @RoleId, @Profil, @DateCreation, @DateCreation)";
                    
                    conn.Execute(query, new 
                    { 
                        IdUser = newUserId,
                        Nom = nom,
                        PostNom = postNom,
                        Prenom = prenom,
                        Sexe = sexe,
                        Username = username,
                        Password = hashedPassword,
                        Telephone = telephone,
                        IdEcole = idEcole,
                        RoleId = roleId,
                        Profil = profil,
                        DateCreation = DateTime.Now
                    });

                    LogUserEvent(newUserId, "CREATE_USER", $"Création de l'utilisateur {username}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création de l'utilisateur: {ex.Message}", ex);
            }
        }

        public dynamic? GetUser(string username)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT u.*, r.nom_role as role_name, r.description_role, r.niveau_acces 
                              FROM t_users_infos u 
                              LEFT JOIN t_roles r ON u.fk_role = r.id_role 
                              WHERE u.username = @Username";
                return conn.QueryFirstOrDefault(query, new { Username = username });
            }
        }

        public IEnumerable<dynamic> GetAllUsers()
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT u.*, r.nom_role as role_name, r.description_role, r.niveau_acces,
                                     e.denomination as ecole_name
                              FROM t_users_infos u 
                              LEFT JOIN t_roles r ON u.fk_role = r.id_role 
                              LEFT JOIN t_ecoles e ON u.id_ecole = e.id_ecole
                              ORDER BY u.nom, u.prenom";
                return conn.Query(query);
            }
        }

        public IEnumerable<dynamic> GetActiveUsers()
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT u.*, r.nom_role as role_name, r.description_role, r.niveau_acces,
                                     e.denomination as ecole_name
                              FROM t_users_infos u 
                              LEFT JOIN t_roles r ON u.fk_role = r.id_role 
                              LEFT JOIN t_ecoles e ON u.id_ecole = e.id_ecole
                              WHERE u.compte_verrouille = 0 
                              AND (u.account_locked_until IS NULL OR u.account_locked_until < NOW())
                              ORDER BY u.nom, u.prenom";
                return conn.Query(query);
            }
        }

        public bool UpdateUser(string username, string? newPassword = null, string? roleId = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    // Récupérer l'ID utilisateur
                    var getUserQuery = "SELECT id_user FROM t_users_infos WHERE username = @Username";
                    var userId = conn.QueryFirstOrDefault<string>(getUserQuery, new { Username = username });
                    
                    if (string.IsNullOrEmpty(userId))
                    {
                        throw new Exception("Utilisateur non trouvé");
                    }

                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        var hashedPassword = HashPassword(newPassword);
                        var query = @"UPDATE t_users_infos 
                                     SET pwd_hash = @Password, last_password_change = NOW(), 
                                         tentatives_connexion = 0, failed_login_attempts = 0
                                     WHERE username = @Username";
                        conn.Execute(query, new { Username = username, Password = hashedPassword });
                        
                        LogUserEvent(userId, "UPDATE_PASSWORD", "Modification du mot de passe");
                    }

                  

                    if (!string.IsNullOrEmpty(roleId))
                    {
                        var query = "UPDATE t_users_infos SET fk_role = @RoleId WHERE username = @Username";
                        conn.Execute(query, new { Username = username, RoleId = roleId });
                        
                        LogUserEvent(userId, "UPDATE_ROLE", $"Modification du rôle: {roleId}");
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour: {ex.Message}", ex);
            }
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var hashedOldPassword = HashPassword(oldPassword);
                    var checkQuery = "SELECT id_user FROM t_users_infos WHERE username = @Username AND pwd_hash = @Password";
                    var userId = conn.QueryFirstOrDefault<string>(checkQuery, new { Username = username, Password = hashedOldPassword });

                    if (string.IsNullOrEmpty(userId))
                    {
                        throw new Exception("Ancien mot de passe incorrect");
                    }

                    var hashedNewPassword = HashPassword(newPassword);
                    var updateQuery = @"UPDATE t_users_infos 
                                       SET pwd_hash = @Password, last_password_change = NOW(),
                                           tentatives_connexion = 0, failed_login_attempts = 0
                                       WHERE username = @Username";
                    conn.Execute(updateQuery, new { Username = username, Password = hashedNewPassword });

                    LogUserEvent(userId, "CHANGE_PASSWORD", "Changement de mot de passe");
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public bool DeleteUser(string username)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    // Récupérer l'ID utilisateur avant de le verrouiller
                    var getUserQuery = "SELECT id_user FROM t_users_infos WHERE username = @Username";
                    var userId = conn.QueryFirstOrDefault<string>(getUserQuery, new { Username = username });
                    
                    if (string.IsNullOrEmpty(userId))
                    {
                        throw new Exception("Utilisateur non trouvé");
                    }

                    // Verrouiller le compte plutôt que supprimer
                    var query = @"UPDATE t_users_infos 
                                 SET compte_verrouille = 1, 
                                     date_verrouillage = NOW(),
                                     account_locked_until = DATE_ADD(NOW(), INTERVAL 10 YEAR)
                                 WHERE username = @Username";
                    conn.Execute(query, new { Username = username });

                    LogUserEvent(userId, "DELETE_USER", $"Désactivation permanente de l'utilisateur {username}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        #endregion

        #region Gestion des événements utilisateur

        private void LogUserEvent(string userId, string eventType, string description)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"INSERT INTO t_user_events (id_user, event_type, description, event_date) 
                                  VALUES (@UserId, @EventType, @Description, @EventDate)";
                    
                    conn.Execute(query, new 
                    { 
                        UserId = userId,
                        EventType = eventType,
                        Description = description,
                        EventDate = DateTime.Now
                    });
                }
            }
            catch
            {
                // Ignorer les erreurs de log
            }
        }

        public IEnumerable<dynamic> GetUserEvents(string userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT ue.*, u.nom, u.postnom, u.prenom, u.username 
                              FROM t_user_events ue 
                              INNER JOIN t_users_infos u ON ue.id_user = u.id_user 
                              WHERE ue.id_user = @UserId";

                if (startDate.HasValue)
                {
                    query += " AND ue.event_date >= @StartDate";
                }

                if (endDate.HasValue)
                {
                    query += " AND ue.event_date <= @EndDate";
                }

                query += " ORDER BY ue.event_date DESC";

                return conn.Query(query, new { UserId = userId, StartDate = startDate, EndDate = endDate });
            }
        }

        public IEnumerable<dynamic> GetAllUserEvents(DateTime? startDate = null, DateTime? endDate = null)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT ue.*, u.nom, u.postnom, u.prenom, u.username 
                              FROM t_user_events ue 
                              INNER JOIN t_users_infos u ON ue.id_user = u.id_user 
                              WHERE 1=1";

                if (startDate.HasValue)
                {
                    query += " AND ue.event_date >= @StartDate";
                }

                if (endDate.HasValue)
                {
                    query += " AND ue.event_date <= @EndDate";
                }

                query += " ORDER BY ue.event_date DESC";

                return conn.Query(query, new { StartDate = startDate, EndDate = endDate });
            }
        }

        #endregion

        #region Utilitaires

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public bool ResetPassword(string username, string newPassword)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    // Récupérer l'ID utilisateur
                    var getUserQuery = "SELECT id_user FROM t_users_infos WHERE username = @Username";
                    var userId = conn.QueryFirstOrDefault<string>(getUserQuery, new { Username = username });
                    
                    if (string.IsNullOrEmpty(userId))
                    {
                        throw new Exception("Utilisateur non trouvé");
                    }

                    var hashedPassword = HashPassword(newPassword);
                    var query = @"UPDATE t_users_infos 
                                 SET pwd_hash = @Password, last_password_change = NOW(),
                                     tentatives_connexion = 0, failed_login_attempts = 0,
                                     compte_verrouille = 0, date_verrouillage = NULL,
                                     account_locked_until = NULL
                                 WHERE username = @Username";
                    conn.Execute(query, new { Username = username, Password = hashedPassword });

                    LogUserEvent(userId, "RESET_PASSWORD", "Réinitialisation du mot de passe");
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
