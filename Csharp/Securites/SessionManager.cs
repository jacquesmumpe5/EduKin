using Dapper;
using EduKin.DataSets;

namespace EduKin.Csharp.Securites
{
    public class SessionManager
    {
        private readonly Connexion _connexion;
        private static SessionManager? _instance;
        private static readonly object _lock = new object();

        // Informations de session
        private string? _currentUsername;
        private string? _currentMatricule;
        private string? _currentNomComplet;
        private string? _currentRole;
        private string? _currentIdEcole;
        private DateTime? _sessionStartTime;
        private string? _sessionId;
        private List<string> _userPermissions;

        private SessionManager()
        {
            _connexion = Connexion.Instance;
            _userPermissions = new List<string>();
        }

        public static SessionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SessionManager();
                        }
                    }
                }
                return _instance;
            }
        }

        #region Propriétés de Session

        public string? CurrentUsername => _currentUsername;
        public string? CurrentMatricule => _currentMatricule;
        public string? CurrentNomComplet => _currentNomComplet;
        public string? CurrentRole => _currentRole;
        public string? CurrentIdEcole => _currentIdEcole;
        public DateTime? SessionStartTime => _sessionStartTime;
        public string? SessionId => _sessionId;
        public bool IsAuthenticated => !string.IsNullOrEmpty(_currentUsername);

        public TimeSpan? SessionDuration
        {
            get
            {
                if (_sessionStartTime.HasValue)
                {
                    return DateTime.Now - _sessionStartTime.Value;
                }
                return null;
            }
        }

        #endregion

        #region Gestion de Session

        public bool StartSession(string username, string password)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"SELECT id_user, nom, postnom, prenom, role, fk_role, id_ecole, compte_verrouille
                                  FROM t_users_infos 
                                  WHERE id_user = @Username 
                                  AND pwd_hash = @Password 
                                  AND compte_verrouille = 0";
                    
                    var hashedPassword = HashPassword(password);
                    var user = conn.QueryFirstOrDefault(query, new { Username = username, Password = hashedPassword });

                    if (user != null)
                    {
                        _currentUsername = user.id_user;
                        _currentMatricule = user.id_user; // Utiliser id_user comme matricule
                        _currentNomComplet = $"{user.nom} {user.postnom} {user.prenom}";
                        _currentRole = user.role;
                        _currentIdEcole = user.id_ecole;
                        _sessionStartTime = DateTime.Now;
                        _sessionId = Guid.NewGuid().ToString();

                        // Charger les permissions
                        LoadUserPermissions(user.fk_role);

                        // Enregistrer la session
                        CreateSessionRecord();

                        // Logger l'événement
                        LogSessionEvent("LOGIN", "Connexion réussie");

                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du démarrage de session: {ex.Message}", ex);
            }
        }

        public void EndSession()
        {
            if (IsAuthenticated)
            {
                try
                {
                    // Logger la déconnexion
                    LogSessionEvent("LOGOUT", "Déconnexion");

                    // Mettre à jour l'enregistrement de session
                    UpdateSessionRecord();
                }
                catch
                {
                    // Ignorer les erreurs lors de la déconnexion
                }

                // Réinitialiser les variables de session
                _currentUsername = null;
                _currentMatricule = null;
                _currentNomComplet = null;
                _currentRole = null;
                _currentIdEcole = null;
                _sessionStartTime = null;
                _sessionId = null;
                _userPermissions.Clear();
            }
        }

        public void RefreshSession()
        {
            if (IsAuthenticated && !string.IsNullOrEmpty(_currentUsername))
            {
                try
                {
                    using (var conn = _connexion.GetConnection())
                    {
                        var query = @"SELECT id_user, nom, postnom, prenom, role, fk_role, id_ecole, compte_verrouille
                                      FROM t_users_infos 
                                      WHERE id_user = @Username 
                                      AND compte_verrouille = 0";
                        
                        var user = conn.QueryFirstOrDefault(query, new { Username = _currentUsername });

                        if (user != null)
                        {
                            _currentNomComplet = $"{user.nom} {user.postnom} {user.prenom}";
                            _currentRole = user.role;
                            _currentIdEcole = user.id_ecole;

                            // Recharger les permissions
                            LoadUserPermissions(user.fk_role);
                        }
                        else
                        {
                            // Utilisateur désactivé ou supprimé
                            EndSession();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erreur lors du rafraîchissement: {ex.Message}", ex);
                }
            }
        }

        #endregion

        #region Gestion des Permissions

        private void LoadUserPermissions(string? fkRole = null)
        {
            _userPermissions.Clear();

            if (!IsAuthenticated || string.IsNullOrEmpty(fkRole))
                return;

            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"SELECT DISTINCT p.nom_permission 
                                  FROM t_roles_permissions rp 
                                  INNER JOIN t_permissions p ON rp.fk_permission = p.id_permission 
                                  WHERE rp.fk_role = @FkRole 
                                  AND rp.accordee = 1 
                                  AND p.etat = 1";
                    
                    var permissions = conn.Query<string>(query, new { FkRole = fkRole });
                    _userPermissions.AddRange(permissions);
                }
            }
            catch
            {
                // Ignorer les erreurs de chargement des permissions
            }
        }

        public bool HasPermission(string permissionName)
        {
            if (!IsAuthenticated)
                return false;

            // Le Super Administrateur et l'Administrateur ont toutes les permissions
            if (_currentRole?.Equals("Super Administrateur", StringComparison.OrdinalIgnoreCase) == true ||
                _currentRole?.Equals("Administrateur", StringComparison.OrdinalIgnoreCase) == true)
                return true;

            return _userPermissions.Contains(permissionName);
        }

        public bool HasAnyPermission(params string[] permissionNames)
        {
            if (!IsAuthenticated)
                return false;

            if (_currentRole?.Equals("Super Administrateur", StringComparison.OrdinalIgnoreCase) == true ||
                _currentRole?.Equals("Administrateur", StringComparison.OrdinalIgnoreCase) == true)
                return true;

            return permissionNames.Any(p => _userPermissions.Contains(p));
        }

        public bool HasAllPermissions(params string[] permissionNames)
        {
            if (!IsAuthenticated)
                return false;

            if (_currentRole?.Equals("Super Administrateur", StringComparison.OrdinalIgnoreCase) == true ||
                _currentRole?.Equals("Administrateur", StringComparison.OrdinalIgnoreCase) == true)
                return true;

            return permissionNames.All(p => _userPermissions.Contains(p));
        }

        public IEnumerable<string> GetUserPermissions()
        {
            return _userPermissions.AsReadOnly();
        }

        #endregion

        #region Enregistrement des Sessions

        private void CreateSessionRecord()
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"INSERT INTO t_sessions 
                                  (session_id, username, matricule, date_debut, id_ecole, ip_address, machine_name) 
                                  VALUES (@SessionId, @Username, @Matricule, @DateDebut, @IdEcole, @IpAddress, @MachineName)";
                    
                    conn.Execute(query, new 
                    { 
                        SessionId = _sessionId,
                        Username = _currentUsername,
                        Matricule = _currentMatricule,
                        DateDebut = _sessionStartTime,
                        IdEcole = _currentIdEcole,
                        IpAddress = GetLocalIPAddress(),
                        MachineName = Environment.MachineName
                    });
                }
            }
            catch
            {
                // Ignorer les erreurs d'enregistrement
            }
        }

        private void UpdateSessionRecord()
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"UPDATE t_sessions 
                                  SET date_fin = @DateFin, duree_minutes = @DureeMinutes 
                                  WHERE session_id = @SessionId";
                    
                    var duree = SessionDuration?.TotalMinutes ?? 0;
                    
                    conn.Execute(query, new 
                    { 
                        SessionId = _sessionId,
                        DateFin = DateTime.Now,
                        DureeMinutes = (int)duree
                    });
                }
            }
            catch
            {
                // Ignorer les erreurs
            }
        }

        public IEnumerable<dynamic> GetActiveSessions()
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT s.*, a.nom, a.postnom, a.prenom 
                              FROM t_sessions s 
                              INNER JOIN t_agents a ON s.matricule = a.matricule 
                              WHERE s.date_fin IS NULL 
                              ORDER BY s.date_debut DESC";
                return conn.Query(query);
            }
        }

        public IEnumerable<dynamic> GetSessionHistory(string? username = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT s.*, a.nom, a.postnom, a.prenom 
                              FROM t_sessions s 
                              INNER JOIN t_agents a ON s.matricule = a.matricule 
                              WHERE 1=1";

                if (!string.IsNullOrEmpty(username))
                    query += " AND s.username = @Username";

                if (startDate.HasValue)
                    query += " AND s.date_debut >= @StartDate";

                if (endDate.HasValue)
                    query += " AND s.date_debut <= @EndDate";

                query += " ORDER BY s.date_debut DESC";

                return conn.Query(query, new { Username = username, StartDate = startDate, EndDate = endDate });
            }
        }

        #endregion

        #region Logging des Événements

        private void LogSessionEvent(string eventType, string description)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"INSERT INTO t_session_events 
                                  (session_id, username, event_type, description, event_date) 
                                  VALUES (@SessionId, @Username, @EventType, @Description, @EventDate)";
                    
                    conn.Execute(query, new 
                    { 
                        SessionId = _sessionId,
                        Username = _currentUsername,
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

        public void LogActivity(string activity, string? details = null)
        {
            if (IsAuthenticated)
            {
                LogSessionEvent("ACTIVITY", $"{activity}: {details}");
            }
        }

        public IEnumerable<dynamic> GetSessionEvents(string sessionId)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT * FROM t_session_events 
                              WHERE session_id = @SessionId 
                              ORDER BY event_date";
                return conn.Query(query, new { SessionId = sessionId });
            }
        }

        #endregion

        #region Validation et Sécurité

        public bool ValidateSession()
        {
            if (!IsAuthenticated)
                return false;

            // Vérifier si la session n'a pas expiré (ex: 8 heures)
            if (SessionDuration?.TotalHours > 8)
            {
                EndSession();
                return false;
            }

            // Vérifier si l'utilisateur est toujours actif et non verrouillé
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = "SELECT compte_verrouille FROM t_users_infos WHERE id_user = @Username";
                    var isLocked = conn.QueryFirstOrDefault<bool>(query, new { Username = _currentUsername });

                    if (isLocked)
                    {
                        EndSession();
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void RequirePermission(string permissionName)
        {
            if (!HasPermission(permissionName))
            {
                throw new UnauthorizedAccessException($"Permission requise: {permissionName}");
            }
        }

        public void RequireAuthentication()
        {
            if (!IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Authentification requise");
            }
        }

        #endregion

        #region Utilitaires

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private string GetLocalIPAddress()
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "127.0.0.1";
            }
            catch
            {
                return "Unknown";
            }
        }

        public dynamic GetSessionInfo()
        {
            return new
            {
                Username = _currentUsername,
                Matricule = _currentMatricule,
                NomComplet = _currentNomComplet,
                Role = _currentRole,
                IdEcole = _currentIdEcole,
                SessionId = _sessionId,
                SessionStartTime = _sessionStartTime,
                SessionDuration = SessionDuration,
                IsAuthenticated = IsAuthenticated,
                PermissionsCount = _userPermissions.Count
            };
        }

        #endregion
    }
}
