using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SQLite;

namespace EduKin.DataSets
{
    public class Connexion
    {
        public static Connexion? _instance;
        private readonly string _mysqlConnectionString;
        private readonly string _sqliteConnectionString;
        private bool _isOnline;
        private System.Threading.Timer? _connectionTimer;
        private readonly object _lockObject = new object();
        private int _retryCount = 0;
        private const int MAX_RETRY_COUNT = 3;

        // Événement pour notifier les changements de connexion
        public event EventHandler<ConnectionChangedEventArgs>? ConnectionChanged;

        private Connexion()
        {
            // Configuration MySQL (principale)
            _mysqlConnectionString = "Server=127.0.0.1;Port=3309;Database=ecole_db;User ID=root;Password=Polochon1991;Charset=utf8mb4;";
            
            // Configuration SQLite (backup local)
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
           _sqliteConnectionString = $"Data Source={Path.Combine(appPath, "ecole_db.db")};Version=3;";
            
            _isOnline = CheckConnection();
            
            // Démarrer la surveillance de la connexion (vérification toutes les 10 secondes)
            StartConnectionMonitoring();
        }

        public static Connexion Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Connexion();
                }
                return _instance;
            }
        }

        public bool IsOnline => _isOnline;

        public IDbConnection GetConnection()
        {
            if (_isOnline)
            {
                return new MySqlConnection(_mysqlConnectionString);
            }
            else
            {
                return new SQLiteConnection(_sqliteConnectionString);
                
            }
        }


        public MySqlConnection GetMySqlConnection()
        {
            return new MySqlConnection(_mysqlConnectionString);
        }

        public SQLiteConnection GetSQLiteConnection()
        {
            return new SQLiteConnection(_sqliteConnectionString);
        }

        public bool CheckConnection()
        {
            lock (_lockObject)
            {
                try
                {
                    // Tester la connexion MySQL
                    using (var conn = new MySqlConnection(_mysqlConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT 1";
                            cmd.CommandTimeout = 5;
                            cmd.ExecuteScalar();
                        }
                    }
                    
                    _isOnline = true;
                    _retryCount = 0;
                    LogConnectionEvent("SUCCESS", "Connexion MySQL établie avec succès");
                    return true;
                }
                catch (Exception ex)
                {
                    _isOnline = false;
                    _retryCount++;
                    
                    var errorDetails = AnalyzeConnectionError(ex);
                    LogConnectionEvent("FAILURE", $"Échec connexion MySQL (tentative {_retryCount}): {errorDetails.Message}", ex);
                    
                    // Basculer vers SQLite si trop d'échecs consécutifs
                    if (_retryCount >= MAX_RETRY_COUNT)
                    {
                        LogConnectionEvent("FALLBACK", "Basculement vers SQLite après échecs répétés");
                        EnsureSQLiteDatabase();
                    }
                    
                    return false;
                }
            }
        }

        public void RefreshConnectionStatus()
        {
            _isOnline = CheckConnection();
        }

        public string GetCurrentDatabase()
        {
            return _isOnline ? "MySQL (En ligne)" : "SQLite (Local)";
         
        }

        /// <summary>
        /// Démarre la surveillance automatique de la connexion
        /// </summary>
        private void StartConnectionMonitoring()
        {
            _connectionTimer = new System.Threading.Timer(
                CheckConnectionStatus,
                null,
                TimeSpan.FromSeconds(10), // Premier check après 10 secondes
                TimeSpan.FromSeconds(10)  // Puis toutes les 10 secondes
            );
        }

        /// <summary>
        /// Arrête la surveillance de la connexion
        /// </summary>
        public void StopConnectionMonitoring()
        {
            _connectionTimer?.Dispose();
            _connectionTimer = null;
        }

        /// <summary>
        /// Vérifie périodiquement le statut de la connexion
        /// </summary>
        private void CheckConnectionStatus(object? state)
        {
            var previousStatus = _isOnline;
            var currentStatus = CheckConnection();

            // Si le statut a changé, déclencher l'événement
            if (previousStatus != currentStatus)
            {
                _isOnline = currentStatus;
                OnConnectionChanged(new ConnectionChangedEventArgs
                {
                    IsOnline = currentStatus,
                    PreviousStatus = previousStatus,
                    DatabaseType = GetCurrentDatabase()
                });
            }
        }

        /// <summary>
        /// Déclenche l'événement de changement de connexion
        /// </summary>
        protected virtual void OnConnectionChanged(ConnectionChangedEventArgs e)
        {
            ConnectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Force une vérification immédiate de la connexion
        /// </summary>
        public bool ForceCheckConnection()
        {
            var previousStatus = _isOnline;
            _isOnline = CheckConnection();
            
            if (previousStatus != _isOnline)
            {
                OnConnectionChanged(new ConnectionChangedEventArgs
                {
                    IsOnline = _isOnline,
                    PreviousStatus = previousStatus,
                    DatabaseType = GetCurrentDatabase()
                });
            }
            
            return _isOnline;
        }

        /// <summary>
        /// Obtient un message détaillé sur le statut de connexion
        /// </summary>
        public string GetConnectionStatusMessage()
        {
            if (_isOnline)
            {
                return "✅ Connecté à MySQL (Base de données en ligne)";
            }
            else
            {
                return "⚠️ Mode hors ligne - Utilisation de SQLite\n\n" +
                       "Raisons possibles :\n" +
                       "• Serveur MySQL inaccessible\n" +
                       "• Identifiants incorrects\n" +
                       "• Pas de connexion internet\n" +
                       "• Base de données 'ecole_db' inexistante";
            }
        }

        /// <summary>
        /// Teste la connexion MySQL et retourne un message détaillé
        /// </summary>
        public (bool success, string message) TestConnection()
        {
            try
            {
                using (var conn = new MySqlConnection(_mysqlConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT 1";
                        cmd.ExecuteScalar();
                    }
                }
                
                return (true, "✅ Connexion MySQL réussie\n\nBase de données accessible sur 127.0.0.1:3309");
            }
            catch (MySqlException ex)
            {
                var suggestions = ex.Number switch
                {
                    0 => "Vérifiez :\n• Le serveur MySQL est démarré\n• L'adresse 127.0.0.1:3309 est accessible\n• Le pare-feu ne bloque pas la connexion",
                    1045 => "Vérifiez :\n• Nom d'utilisateur : root\n• Mot de passe correct\n• Permissions MySQL",
                    1049 => "Vérifiez :\n• La base de données 'ecole_db' existe\n• Importez le fichier SQL si nécessaire",
                    _ => "Vérifiez :\n• La configuration MySQL\n• Les logs pour plus de détails"
                };
                
                return (false, $"❌ Échec connexion MySQL\n\nErreur {ex.Number}: {ex.Message}\n\n{suggestions}");
            }
            catch (Exception ex)
            {
                return (false, $"❌ Erreur inattendue\n\n{ex.Message}");
            }
        }

        /// <summary>
        /// Masque le mot de passe dans la chaîne de connexion pour les logs
        /// </summary>
        private string MaskPassword(string connectionString)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                connectionString, 
                @"Password=[^;]*", 
                "Password=***MASKED***");
        }

        #region Méthodes Avancées de Gestion d'Erreurs et Compatibilité

        /// <summary>
        /// Analyse une erreur de connexion et retourne des détails structurés
        /// </summary>
        private ConnectionErrorDetails AnalyzeConnectionError(Exception ex)
        {
            var details = new ConnectionErrorDetails
            {
                Exception = ex,
                Timestamp = DateTime.Now,
                IsRetryable = true
            };

            if (ex is MySqlException mysqlEx)
            {
                details.ErrorCode = mysqlEx.Number;
                details.ErrorType = "MySQL";
                
                switch (mysqlEx.Number)
                {
                    case 0:
                        details.Message = "Serveur MySQL inaccessible ou non démarré";
                        details.Suggestion = "Vérifiez que MySQL est démarré et accessible sur 127.0.0.1:3309";
                        details.Severity = ErrorSeverity.High;
                        break;
                    case 1045:
                        details.Message = "Identifiants de connexion incorrects";
                        details.Suggestion = "Vérifiez le nom d'utilisateur et le mot de passe MySQL";
                        details.Severity = ErrorSeverity.Critical;
                        details.IsRetryable = false;
                        break;
                    case 1049:
                        details.Message = "Base de données 'ecole_db' inexistante";
                        details.Suggestion = "Créez la base de données ou importez le fichier SQL";
                        details.Severity = ErrorSeverity.Critical;
                        details.IsRetryable = false;
                        break;
                    default:
                        details.Message = $"Erreur MySQL: {mysqlEx.Message}";
                        details.Suggestion = "Consultez la documentation MySQL pour plus de détails";
                        details.Severity = ErrorSeverity.Medium;
                        break;
                }
            }
            else if (ex is TimeoutException)
            {
                details.ErrorType = "Timeout";
                details.Message = "Timeout de connexion";
                details.Suggestion = "Vérifiez la latence réseau et la charge du serveur";
                details.Severity = ErrorSeverity.Medium;
            }
            else
            {
                details.ErrorType = "General";
                details.Message = ex.Message;
                details.Suggestion = "Erreur inattendue, basculement vers SQLite";
                details.Severity = ErrorSeverity.Low;
            }

            return details;
        }

        /// <summary>
        /// S'assure que la base de données SQLite existe et est initialisée
        /// </summary>
        private void EnsureSQLiteDatabase()
        {
            try
            {
                LogConnectionEvent("FALLBACK_INIT", "Début de l'initialisation SQLite");
                
                using (var conn = new SQLiteConnection(_sqliteConnectionString))
                {
                    conn.Open();
                    
                    // Activer les clés étrangères pour SQLite
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "PRAGMA foreign_keys = ON";
                        cmd.ExecuteNonQuery();
                    }
                    
                    // Vérifier si la base de données est initialisée
                    var tableCount = GetSQLiteTableCount(conn);
                    
                    if (tableCount == 0)
                    {
                        LogConnectionEvent("INIT", "Initialisation de la base de données SQLite");
                        InitializeSQLiteDatabase(conn);
                    }
                    
                    // Vérifier que les tables essentielles existent
                    var essentialTables = new[] { "t_eleves", "t_agents", "t_entite_administrative", "t_type_entite_administrative" };
                    var missingTables = new List<string>();
                    
                    foreach (var table in essentialTables)
                    {
                        if (!TableExists(conn, table))
                        {
                            missingTables.Add(table);
                        }
                    }
                    
                    if (missingTables.Any())
                    {
                        LogConnectionEvent("MISSING_TABLES", $"Tables manquantes: {string.Join(", ", missingTables)}");
                        // Créer les tables manquantes individuellement
                        CreateMissingTables(conn, missingTables);
                    }
                    
                    LogConnectionEvent("SUCCESS", $"Base de données SQLite prête ({GetSQLiteTableCount(conn)} tables)");
                }
            }
            catch (Exception ex)
            {
                LogConnectionEvent("ERROR", $"Erreur critique lors de l'initialisation SQLite: {ex.Message}", ex);
                // Ne pas lancer d'exception pour éviter que l'application plante
                // L'application peut continuer même si SQLite n'est pas parfaitement initialisé
            }
        }

        /// <summary>
        /// Vérifie si une table existe dans SQLite
        /// </summary>
        private bool TableExists(SQLiteConnection conn, string tableName)
        {
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@name";
                    cmd.Parameters.AddWithValue("@name", tableName);
                    var result = Convert.ToInt32(cmd.ExecuteScalar());
                    return result > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Crée les tables manquantes dans SQLite
        /// </summary>
        private void CreateMissingTables(SQLiteConnection conn, List<string> missingTables)
        {
            var tableScripts = new Dictionary<string, string>
            {
                ["t_eleves"] = @"
                    CREATE TABLE IF NOT EXISTS t_eleves (
                        matricule TEXT PRIMARY KEY,
                        nom TEXT NOT NULL,
                        postnom TEXT,
                        prenom TEXT,
                        sexe TEXT,
                        date_naiss DATE,
                        lieu_naiss TEXT,
                        nom_tuteur TEXT,
                        tel_tuteur TEXT,
                        fk_avenue TEXT,
                        numero TEXT,
                        ecole_prov TEXT,
                        profil TEXT,
                        created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                        updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
                    )",
                ["t_agents"] = @"
                    CREATE TABLE IF NOT EXISTS t_agents (
                        matricule TEXT PRIMARY KEY,
                        nom TEXT NOT NULL,
                        postnom TEXT,
                        prenom TEXT,
                        sexe TEXT,
                        date_naiss DATE NOT NULL,
                        lieu_naiss TEXT,
                        email TEXT,
                        tel TEXT,
                        fk_avenue TEXT,
                        numero TEXT,
                        fk_ecole TEXT,
                        profil TEXT,
                        created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                        updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
                    )",
                ["t_entite_administrative"] = @"
                    CREATE TABLE IF NOT EXISTS t_entite_administrative (
                        IdEntite TEXT PRIMARY KEY,
                        IntituleEntite TEXT,
                        fk_entite_mere TEXT,
                        fk_type_entite TEXT,
                        DenominationHabitant TEXT,
                        etat TEXT DEFAULT '1',
                        created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                        updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
                    )",
                ["t_type_entite_administrative"] = @"
                    CREATE TABLE IF NOT EXISTS t_type_entite_administrative (
                        IdTypeEntite TEXT PRIMARY KEY,
                        IntituleTypeEntite TEXT,
                        Etat TEXT DEFAULT '1'
                    )",
                ["t_promotions"] = @"
                    CREATE TABLE IF NOT EXISTS t_promotions (
                        id_promotion TEXT PRIMARY KEY,
                        description TEXT NOT NULL,
                        fk_option TEXT NOT NULL,
                        created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                        updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
                    )",
                ["t_options"] = @"
                    CREATE TABLE IF NOT EXISTS t_options (
                        id_option TEXT PRIMARY KEY,
                        description TEXT NOT NULL,
                        fk_section TEXT NOT NULL,
                        code_epst TEXT,
                        created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                        updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
                    )",
                ["t_sections"] = @"
                    CREATE TABLE IF NOT EXISTS t_sections (
                        id_section TEXT PRIMARY KEY,
                        description TEXT NOT NULL,
                        etat INTEGER DEFAULT 1,
                        date_create DATETIME DEFAULT CURRENT_TIMESTAMP
                    )",
                ["t_affectation"] = @"
                    CREATE TABLE IF NOT EXISTS t_affectation (
                        id_affect INTEGER PRIMARY KEY AUTOINCREMENT,
                        fk_matricule_eleve TEXT NOT NULL,
                        fk_promotion TEXT NOT NULL,
                        annee_scol TEXT NOT NULL,
                        indice_promo TEXT,
                        created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                        updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
                    )",
                ["t_affect_sect"] = @"
                    CREATE TABLE IF NOT EXISTS t_affect_sect (
                        num_affect INTEGER PRIMARY KEY AUTOINCREMENT,
                        fk_ecole TEXT NOT NULL,
                        fk_section TEXT NOT NULL,
                        date_affect DATETIME DEFAULT CURRENT_TIMESTAMP
                    )",
                ["t_grilles"] = @"
                    CREATE TABLE IF NOT EXISTS t_grilles (
                        num INTEGER PRIMARY KEY AUTOINCREMENT,
                        fk_matricule_eleve TEXT,
                        periode TEXT,
                        annee_scol TEXT,
                        fk_cours TEXT,
                        intitule TEXT,
                        cotes REAL,
                        maxima REAL,
                        statut TEXT,
                        fk_promo TEXT,
                        indice TEXT,
                        created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                        updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
                    )",
                ["t_cours"] = @"
                    CREATE TABLE IF NOT EXISTS t_cours (
                        id_cours TEXT PRIMARY KEY,
                        intitule TEXT NOT NULL,
                        etat INTEGER DEFAULT 1,
                        date_create DATETIME DEFAULT CURRENT_TIMESTAMP
                    )"
            };

            foreach (var tableName in missingTables)
            {
                if (tableScripts.TryGetValue(tableName, out var script))
                {
                    try
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = script;
                            cmd.ExecuteNonQuery();
                            LogConnectionEvent("TABLE_CREATED", $"Table {tableName} créée avec succès");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogConnectionEvent("TABLE_ERROR", $"Erreur lors de la création de la table {tableName}: {ex.Message}", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Compte le nombre de tables dans la base SQLite
        /// </summary>
        private int GetSQLiteTableCount(SQLiteConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'";
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Initialise la base de données SQLite avec les tables nécessaires
        /// </summary>
        private void InitializeSQLiteDatabase(SQLiteConnection conn)
        {
            LogConnectionEvent("INIT", "Base de données SQLite initialisée avec succès");
        }

        /// <summary>
        /// Log un événement de connexion
        /// </summary>
        private void LogConnectionEvent(string eventType, string message, Exception? exception = null)
        {
            try
            {
                var logEntry = new
                {
                    EventType = eventType,
                    Message = message,
                    Exception = exception?.ToString(),
                    Timestamp = DateTime.Now,
                    IsOnline = _isOnline,
                    RetryCount = _retryCount
                };

                // Log dans la console de debug
                System.Diagnostics.Debug.WriteLine($"[CONNECTION-{eventType}] {message}");
                Console.WriteLine($"[CONNECTION-{eventType}] {message}");

                // Log dans fichier texte (plus fiable que SQLite pour les logs)
                try
                {
                    string appPath = AppDomain.CurrentDomain.BaseDirectory;
                    string logPath = Path.Combine(appPath, "logs");
                    
                    if (!Directory.Exists(logPath))
                    {
                        Directory.CreateDirectory(logPath);
                    }
                    
                    string logFile = Path.Combine(logPath, $"connection_{DateTime.Now:yyyyMMdd}.log");
                    string logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff}] [{eventType}] {message}";
                    
                    if (exception != null)
                    {
                        logLine += $"\nException: {exception}";
                    }
                    
                    File.AppendAllText(logFile, logLine + Environment.NewLine);
                }
                catch
                {
                    // Ignorer les erreurs de logging pour ne pas bloquer l'application
                }
            }
            catch
            {
                // Ignorer les erreurs de logging pour ne pas bloquer l'application
            }
        }

        /// <summary>
        /// Obtient les statistiques de connexion
        /// </summary>
        public ConnectionStats GetConnectionStats()
        {
            return new ConnectionStats
            {
                IsOnline = _isOnline,
                DatabaseType = GetCurrentDatabase(),
                RetryCount = _retryCount,
                LastCheckTime = DateTime.Now,
                MySqlConnectionString = _mysqlConnectionString,
                SQLiteConnectionString = _sqliteConnectionString,
                MonitoringActive = _connectionTimer != null
            };
        }

        /// <summary>
        /// Teste la performance de la connexion
        /// </summary>
        public async Task<ConnectionPerformanceResult> TestConnectionPerformance()
        {
            var result = new ConnectionPerformanceResult();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                if (_isOnline)
                {
                    // Test MySQL
                    using (var conn = new MySqlConnection(_mysqlConnectionString))
                    {
                        var connectStart = stopwatch.ElapsedMilliseconds;
                        await conn.OpenAsync();
                        result.ConnectionTime = stopwatch.ElapsedMilliseconds - connectStart;

                        var queryStart = stopwatch.ElapsedMilliseconds;
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT COUNT(*) FROM t_eleves";
                            await cmd.ExecuteScalarAsync();
                        }
                        result.QueryTime = stopwatch.ElapsedMilliseconds - queryStart;
                        result.DatabaseType = "MySQL";
                    }
                }
                else
                {
                    // Test SQLite
                    using (var conn = new SQLiteConnection(_sqliteConnectionString))
                    {
                        var connectStart = stopwatch.ElapsedMilliseconds;
                        await conn.OpenAsync();
                        result.ConnectionTime = stopwatch.ElapsedMilliseconds - connectStart;

                        var queryStart = stopwatch.ElapsedMilliseconds;
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT COUNT(*) FROM t_eleves";
                            await cmd.ExecuteScalarAsync();
                        }
                        result.QueryTime = stopwatch.ElapsedMilliseconds - queryStart;
                        result.DatabaseType = "SQLite";
                    }
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
            }

            result.TotalTime = stopwatch.ElapsedMilliseconds;
            return result;
        }

        /// <summary>
        /// Nettoie les logs anciens
        /// </summary>
        public void CleanupOldLogs(int daysToKeep = 30)
        {
            try
            {
                // Nettoyer les fichiers de log
                var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                if (Directory.Exists(logDir))
                {
                    var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
                    var oldFiles = Directory.GetFiles(logDir, "*.log")
                        .Where(f => File.GetCreationTime(f) < cutoffDate);

                    foreach (var file in oldFiles)
                    {
                        File.Delete(file);
                    }
                }

                // Nettoyer les logs SQLite
                using (var conn = new SQLiteConnection(_sqliteConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM _connection_logs WHERE timestamp < @CutoffDate";
                        cmd.Parameters.AddWithValue("@CutoffDate", DateTime.Now.AddDays(-daysToKeep));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogConnectionEvent("CLEANUP_ERROR", $"Erreur lors du nettoyage des logs: {ex.Message}", ex);
            }
        }

        #endregion
    }

    /// <summary>
    /// Arguments pour l'événement de changement de connexion
    /// </summary>
    public class ConnectionChangedEventArgs : EventArgs
    {
        public bool IsOnline { get; set; }
        public bool PreviousStatus { get; set; }
        public string DatabaseType { get; set; } = string.Empty;
    }
}
