using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SQLite;
using System.Text.Json;

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
            // Configuration MySQL (en ligne)
            _mysqlConnectionString = "Server=127.0.0.1;Port=3309;Database=ecole_db;User ID=root;Password=Polochon1991;Charset=utf8mb4;";
            
            // Configuration SQLite (local)
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
           _sqliteConnectionString = $"Data Source={Path.Combine(appPath, "ecole_local.db")};Version=3;";
            
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
                    using (var conn = new MySqlConnection(_mysqlConnectionString))
                    {
                        conn.Open();
                        
                        // Test de validation de la connexion
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT 1";
                            cmd.CommandTimeout = 5; // Timeout court pour éviter les blocages
                            cmd.ExecuteScalar();
                        }
                        
                        _isOnline = true;
                        _retryCount = 0; // Réinitialiser le compteur de retry
                        
                        LogConnectionEvent("SUCCESS", "Connexion MySQL établie avec succès");
                        return true;
                    }
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
                       "• Serveur MySQL non démarré\n" +
                       "• Identifiants incorrects\n" +
                       "• Base de données 'ecole_db' inexistante\n" +
                       "• Pas de connexion réseau au serveur";
            }
        }

        /// <summary>
        /// Teste la connexion et retourne un message détaillé
        /// </summary>
        public (bool success, string message) TestConnection()
        {
            try
            {
                using (var conn = new MySqlConnection(_mysqlConnectionString))
                {
                    conn.Open();
                    
                    // Vérifier que la base de données contient des tables
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SHOW TABLES";
                        using (var reader = cmd.ExecuteReader())
                        {
                            int tableCount = 0;
                            while (reader.Read())
                            {
                                tableCount++;
                            }
                            
                            if (tableCount == 0)
                            {
                                return (false, "⚠️ Base de données vide - Aucune table trouvée");
                            }
                            
                            return (true, $"✅ Connexion MySQL réussie - {tableCount} tables trouvées");
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                string message = ex.Number switch
                {
                    0 => "❌ Serveur MySQL inaccessible\n\nVérifiez que :\n• MySQL est démarré\n• Le serveur écoute sur 127.0.0.1:3306",
                    1045 => "❌ Authentification échouée\n\nVérifiez :\n• Nom d'utilisateur : root\n• Mot de passe",
                    1049 => "❌ Base de données 'ecole_db' introuvable\n\nCréez la base de données ou importez le fichier SQL",
                    _ => $"❌ Erreur MySQL ({ex.Number})\n\n{ex.Message}"
                };
                
                return (false, message);
            }
            catch (Exception ex)
            {
                return (false, $"❌ Erreur inattendue\n\n{ex.Message}");
            }
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
                        details.Suggestion = "Vérifiez que MySQL est démarré et accessible sur 127.0.0.1:3306";
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
                    case 2006:
                        details.Message = "Connexion MySQL perdue";
                        details.Suggestion = "Reconnexion automatique en cours...";
                        details.Severity = ErrorSeverity.Medium;
                        break;
                    case 2013:
                        details.Message = "Connexion au serveur MySQL perdue pendant la requête";
                        details.Suggestion = "Vérifiez la stabilité du réseau";
                        details.Severity = ErrorSeverity.Medium;
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
                using (var conn = new SQLiteConnection(_sqliteConnectionString))
                {
                    conn.Open();
                    
                    // Vérifier si la base de données est initialisée
                    var tableCount = GetSQLiteTableCount(conn);
                    
                    if (tableCount == 0)
                    {
                        LogConnectionEvent("INIT", "Initialisation de la base de données SQLite");
                        InitializeSQLiteDatabase(conn);
                    }
                    
                    // Activer les clés étrangères pour SQLite
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "PRAGMA foreign_keys = ON";
                        cmd.ExecuteNonQuery();
                    }
                    
                    LogConnectionEvent("SUCCESS", $"Base de données SQLite prête ({tableCount} tables)");
                }
            }
            catch (Exception ex)
            {
                LogConnectionEvent("ERROR", $"Erreur lors de l'initialisation SQLite: {ex.Message}", ex);
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
            var initializer = new SQLiteInitializer();
            
            // Créer les tables de base
            CreateBasicSQLiteTables(conn);
            
            // Créer les vues
            initializer.InitializeViews();
            
            LogConnectionEvent("INIT", "Base de données SQLite initialisée avec succès");
        }

        /// <summary>
        /// Crée les tables de base dans SQLite
        /// </summary>
        private void CreateBasicSQLiteTables(SQLiteConnection conn)
        {
            var tableScripts = new[]
            {
                // Table de métadonnées pour le suivi des synchronisations
                @"CREATE TABLE IF NOT EXISTS _sync_metadata (
                    table_name TEXT PRIMARY KEY,
                    last_sync DATETIME,
                    checksum TEXT,
                    record_count INTEGER,
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
                )",
                
                // Table de logs pour le debugging
                @"CREATE TABLE IF NOT EXISTS _connection_logs (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    event_type TEXT NOT NULL,
                    message TEXT NOT NULL,
                    details TEXT,
                    timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
                )"
            };

            foreach (var script in tableScripts)
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = script;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Log un événement de connexion
        /// </summary>
        private void LogConnectionEvent(string eventType, string message, Exception? exception = null)
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
            
            if (exception != null)
            {
                System.Diagnostics.Debug.WriteLine($"[CONNECTION-EXCEPTION] {exception}");
            }

            // Log dans un fichier
            LogToFile(logEntry);
            
            // Log dans SQLite si disponible
            LogToSQLite(logEntry);
        }

        /// <summary>
        /// Log dans un fichier
        /// </summary>
        private void LogToFile(object logEntry)
        {
            try
            {
                var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                Directory.CreateDirectory(logDir);
                
                var logFile = Path.Combine(logDir, $"connection_{DateTime.Now:yyyy-MM-dd}.log");
                var jsonEntry = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions { WriteIndented = true });
                var logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {jsonEntry}{Environment.NewLine}";
                
                File.AppendAllText(logFile, logLine);
            }
            catch
            {
                // Ignorer les erreurs de logging pour éviter les boucles infinies
            }
        }

        /// <summary>
        /// Log dans la base SQLite
        /// </summary>
        private void LogToSQLite(object logEntry)
        {
            try
            {
                using (var conn = new SQLiteConnection(_sqliteConnectionString))
                {
                    conn.Open();
                    
                    var json = JsonSerializer.Serialize(logEntry);
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO _connection_logs (event_type, message, details) 
                                          VALUES (@EventType, @Message, @Details)";
                        
                        var entry = (dynamic)logEntry;
                        cmd.Parameters.AddWithValue("@EventType", entry.EventType ?? "UNKNOWN");
                        cmd.Parameters.AddWithValue("@Message", entry.Message ?? "");
                        cmd.Parameters.AddWithValue("@Details", json);
                        
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Ignorer les erreurs de logging SQLite
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
