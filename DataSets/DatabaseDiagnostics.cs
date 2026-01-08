using System.Data;
using System.Data.SQLite;
using MySql.Data.MySqlClient;
using Dapper;
using System.Diagnostics;
using System.Text.Json;

namespace EduKin.DataSets
{
    /// <summary>
    /// Système de diagnostic avancé pour les bases de données MySQL et SQLite
    /// </summary>
    public class DatabaseDiagnostics
    {
        private readonly Connexion _connexion;
        private readonly Dictionary<string, PerformanceCounter> _performanceCounters;
        private readonly System.Threading.Timer _diagnosticTimer;

        public event EventHandler<DiagnosticEventArgs>? DiagnosticCompleted;
        public event EventHandler<PerformanceAlertEventArgs>? PerformanceAlert;

        public DatabaseDiagnostics()
        {
            _connexion = Connexion.Instance;
            _performanceCounters = new Dictionary<string, PerformanceCounter>();
            
            // Démarrer le monitoring automatique toutes les 5 minutes
            _diagnosticTimer = new System.Threading.Timer(RunAutomaticDiagnostics, null, 
                TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5));
        }

        /// <summary>
        /// Exécute un diagnostic complet des bases de données
        /// </summary>
        public async Task<CompleteDiagnosticResult> RunCompleteDiagnostic()
        {
            var result = new CompleteDiagnosticResult
            {
                StartTime = DateTime.Now
            };

            try
            {
                // Diagnostic de connectivité
                result.ConnectivityTest = await TestConnectivity();

                // Diagnostic de performance
                result.PerformanceTest = await TestPerformance();

                // Diagnostic de l'intégrité des données
                result.DataIntegrityTest = await TestDataIntegrity();

                // Diagnostic de l'espace disque
                result.DiskSpaceTest = await TestDiskSpace();

                // Diagnostic de la synchronisation
                result.SynchronizationTest = await TestSynchronization();

                // Diagnostic du schéma
                var schemaValidator = new SchemaValidator();
                result.SchemaValidation = await schemaValidator.ValidateSchemaCompatibility();

                // Calcul du score global de santé
                result.OverallHealthScore = CalculateHealthScore(result);
                result.HealthStatus = DetermineHealthStatus(result.OverallHealthScore);
                
                result.EndTime = DateTime.Now;
                result.Duration = result.EndTime - result.StartTime;
                result.Success = true;

                // Générer des recommandations
                result.Recommendations = GenerateRecommendations(result);

                OnDiagnosticCompleted(new DiagnosticEventArgs
                {
                    Result = result,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
                result.EndTime = DateTime.Now;
            }

            return result;
        }

        /// <summary>
        /// Teste la connectivité des bases de données
        /// </summary>
        private async Task<ConnectivityTestResult> TestConnectivity()
        {
            var result = new ConnectivityTestResult();
            var stopwatch = Stopwatch.StartNew();

            // Test MySQL
            try
            {
                using var mysqlConn = _connexion.GetMySqlConnection();
                var connectStart = stopwatch.ElapsedMilliseconds;
                await mysqlConn.OpenAsync();
                result.MySQLConnectionTime = stopwatch.ElapsedMilliseconds - connectStart;
                
                using var cmd = mysqlConn.CreateCommand();
                cmd.CommandText = "SELECT 1";
                var queryStart = stopwatch.ElapsedMilliseconds;
                await cmd.ExecuteScalarAsync();
                result.MySQLQueryTime = stopwatch.ElapsedMilliseconds - queryStart;
                
                result.MySQLAvailable = true;
            }
            catch (Exception ex)
            {
                result.MySQLAvailable = false;
                result.MySQLError = ex.Message;
            }

            // Test SQLite
            try
            {
                using var sqliteConn = _connexion.GetSQLiteConnection();
                var connectStart = stopwatch.ElapsedMilliseconds;
                await sqliteConn.OpenAsync();
                result.SQLiteConnectionTime = stopwatch.ElapsedMilliseconds - connectStart;
                
                using var cmd = sqliteConn.CreateCommand();
                cmd.CommandText = "SELECT 1";
                var queryStart = stopwatch.ElapsedMilliseconds;
                await cmd.ExecuteScalarAsync();
                result.SQLiteQueryTime = stopwatch.ElapsedMilliseconds - queryStart;
                
                result.SQLiteAvailable = true;
            }
            catch (Exception ex)
            {
                result.SQLiteAvailable = false;
                result.SQLiteError = ex.Message;
            }

            result.Success = result.MySQLAvailable || result.SQLiteAvailable;
            return result;
        }

        /// <summary>
        /// Teste les performances des bases de données
        /// </summary>
        private async Task<PerformanceTestResult> TestPerformance()
        {
            var result = new PerformanceTestResult();

            if (_connexion.IsOnline)
            {
                result.MySQLPerformance = await TestDatabasePerformance("MySQL");
            }

            result.SQLitePerformance = await TestDatabasePerformance("SQLite");
            result.Success = true;

            return result;
        }

        /// <summary>
        /// Teste les performances d'une base de données spécifique
        /// </summary>
        private async Task<DatabasePerformanceMetrics> TestDatabasePerformance(string databaseType)
        {
            var metrics = new DatabasePerformanceMetrics { DatabaseType = databaseType };
            var stopwatch = Stopwatch.StartNew();

            try
            {
                IDbConnection conn = databaseType == "MySQL" 
                    ? _connexion.GetMySqlConnection() 
                    : _connexion.GetSQLiteConnection();

                using (conn)
                {
                    conn.Open();

                    // Test de requête simple
                    var simpleStart = stopwatch.ElapsedMilliseconds;
                    await conn.ExecuteScalarAsync("SELECT 1");
                    metrics.SimpleQueryTime = stopwatch.ElapsedMilliseconds - simpleStart;

                    // Test de comptage d'enregistrements
                    var countStart = stopwatch.ElapsedMilliseconds;
                    try
                    {
                        metrics.RecordCount = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM t_eleves");
                        metrics.CountQueryTime = stopwatch.ElapsedMilliseconds - countStart;
                    }
                    catch
                    {
                        metrics.CountQueryTime = -1; // Table n'existe pas
                    }

                    // Test de requête complexe
                    var complexStart = stopwatch.ElapsedMilliseconds;
                    try
                    {
                        var complexQuery = databaseType == "MySQL" 
                            ? @"SELECT COUNT(*) FROM t_eleves e 
                                LEFT JOIN t_affectation a ON e.matricule = a.matricule 
                                WHERE e.created_at > DATE_SUB(NOW(), INTERVAL 30 DAY)"
                            : @"SELECT COUNT(*) FROM t_eleves e 
                                LEFT JOIN t_affectation a ON e.matricule = a.matricule 
                                WHERE e.created_at > datetime('now', '-30 days')";
                        
                        await conn.ExecuteScalarAsync(complexQuery);
                        metrics.ComplexQueryTime = stopwatch.ElapsedMilliseconds - complexStart;
                    }
                    catch
                    {
                        metrics.ComplexQueryTime = -1; // Erreur dans la requête
                    }

                    // Test d'insertion
                    var insertStart = stopwatch.ElapsedMilliseconds;
                    try
                    {
                        var testTable = "_performance_test";
                        await conn.ExecuteAsync($"CREATE TEMPORARY TABLE {testTable} (id INTEGER, data TEXT)");
                        await conn.ExecuteAsync($"INSERT INTO {testTable} (id, data) VALUES (1, 'test')");
                        await conn.ExecuteAsync($"DROP TABLE {testTable}");
                        metrics.InsertTime = stopwatch.ElapsedMilliseconds - insertStart;
                    }
                    catch
                    {
                        metrics.InsertTime = -1;
                    }
                }

                metrics.Success = true;
            }
            catch (Exception ex)
            {
                metrics.Success = false;
                metrics.Error = ex.Message;
            }

            return metrics;
        }

        /// <summary>
        /// Teste l'intégrité des données
        /// </summary>
        private async Task<DataIntegrityTestResult> TestDataIntegrity()
        {
            var result = new DataIntegrityTestResult();

            try
            {
                using var conn = _connexion.GetConnection();
                conn.Open();

                // Vérifier les contraintes de clés étrangères
                result.ForeignKeyViolations = await CheckForeignKeyConstraints(conn);

                // Vérifier les doublons sur les clés primaires
                result.PrimaryKeyDuplicates = await CheckPrimaryKeyDuplicates(conn);

                // Vérifier les données orphelines
                result.OrphanedRecords = await CheckOrphanedRecords(conn);

                // Vérifier la cohérence des données
                result.DataConsistencyIssues = await CheckDataConsistency(conn);

                result.Success = result.ForeignKeyViolations.Count == 0 && 
                               result.PrimaryKeyDuplicates.Count == 0 && 
                               result.OrphanedRecords.Count == 0 &&
                               result.DataConsistencyIssues.Count == 0;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Teste l'espace disque disponible
        /// </summary>
        private async Task<DiskSpaceTestResult> TestDiskSpace()
        {
            var result = new DiskSpaceTestResult();

            try
            {
                // Vérifier l'espace disque pour SQLite
                var sqliteDbPath = Path.GetDirectoryName(_connexion.GetSQLiteConnection().ConnectionString.Split('=')[1]);
                if (!string.IsNullOrEmpty(sqliteDbPath))
                {
                    var drive = new DriveInfo(Path.GetPathRoot(sqliteDbPath) ?? "C:");
                    result.AvailableSpace = drive.AvailableFreeSpace;
                    result.TotalSpace = drive.TotalSize;
                    result.UsedSpacePercentage = (double)(drive.TotalSize - drive.AvailableFreeSpace) / drive.TotalSize * 100;
                }

                // Obtenir la taille de la base SQLite
                var sqliteDbFile = _connexion.GetSQLiteConnection().ConnectionString.Split('=')[1].Split(';')[0];
                if (File.Exists(sqliteDbFile))
                {
                    result.SQLiteDatabaseSize = new FileInfo(sqliteDbFile).Length;
                }

                result.Success = result.AvailableSpace > 100 * 1024 * 1024; // Au moins 100MB disponible
                
                if (!result.Success)
                {
                    result.Warning = "Espace disque faible détecté";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Teste la synchronisation entre MySQL et SQLite
        /// </summary>
        private async Task<SynchronizationTestResult> TestSynchronization()
        {
            var result = new SynchronizationTestResult();

            try
            {
                if (!_connexion.IsOnline)
                {
                    result.Success = true;
                    result.Message = "Mode hors ligne - Synchronisation non applicable";
                    return result;
                }

                var syncManager = new SyncManager();
                
                // Vérifier s'il y a des données en attente de synchronisation
                result.HasPendingChanges = await CheckPendingChanges();
                
                // Tester une synchronisation de test
                var testResult = await syncManager.SyncBidirectional();
                result.LastSyncSuccessful = testResult;
                
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Vérifie les contraintes de clés étrangères
        /// </summary>
        private async Task<List<string>> CheckForeignKeyConstraints(IDbConnection conn)
        {
            var violations = new List<string>();

            try
            {
                // Vérifications spécifiques selon le type de base de données
                if (conn is MySqlConnection)
                {
                    // Vérifications MySQL
                    var mysqlChecks = new[]
                    {
                        "SELECT COUNT(*) FROM t_agents WHERE id_ecole NOT IN (SELECT id_ecole FROM t_ecoles)",
                        "SELECT COUNT(*) FROM t_affectation WHERE matricule NOT IN (SELECT matricule FROM t_eleves)"
                    };

                    foreach (var check in mysqlChecks)
                    {
                        var count = await conn.ExecuteScalarAsync<int>(check);
                        if (count > 0)
                        {
                            violations.Add($"Violation de clé étrangère détectée: {count} enregistrements");
                        }
                    }
                }
                else if (conn is SQLiteConnection)
                {
                    // Vérifications SQLite
                    var result = await conn.QueryAsync("PRAGMA foreign_key_check");
                    foreach (var violation in result)
                    {
                        violations.Add($"Violation FK: {violation}");
                    }
                }
            }
            catch (Exception ex)
            {
                violations.Add($"Erreur lors de la vérification FK: {ex.Message}");
            }

            return violations;
        }

        /// <summary>
        /// Vérifie les doublons sur les clés primaires
        /// </summary>
        private async Task<List<string>> CheckPrimaryKeyDuplicates(IDbConnection conn)
        {
            var duplicates = new List<string>();

            var tables = new[] { "t_eleves", "t_agents", "t_ecoles" };
            var primaryKeys = new Dictionary<string, string>
            {
                { "t_eleves", "matricule" },
                { "t_agents", "matricule" },
                { "t_ecoles", "id_ecole" }
            };

            foreach (var table in tables)
            {
                try
                {
                    var pk = primaryKeys[table];
                    var query = $"SELECT {pk}, COUNT(*) as cnt FROM {table} GROUP BY {pk} HAVING COUNT(*) > 1";
                    var result = await conn.QueryAsync(query);
                    
                    foreach (var duplicate in result)
                    {
                        duplicates.Add($"Doublon dans {table}.{pk}: {duplicate}");
                    }
                }
                catch
                {
                    // Table n'existe pas ou erreur de requête
                }
            }

            return duplicates;
        }

        /// <summary>
        /// Vérifie les enregistrements orphelins
        /// </summary>
        private async Task<List<string>> CheckOrphanedRecords(IDbConnection conn)
        {
            var orphans = new List<string>();

            try
            {
                // Vérifier les affectations sans élèves
                var orphanedAffectations = await conn.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM t_affectation WHERE matricule NOT IN (SELECT matricule FROM t_eleves)");
                
                if (orphanedAffectations > 0)
                {
                    orphans.Add($"{orphanedAffectations} affectations orphelines détectées");
                }
            }
            catch
            {
                // Tables n'existent pas
            }

            return orphans;
        }

        /// <summary>
        /// Vérifie la cohérence des données
        /// </summary>
        private async Task<List<string>> CheckDataConsistency(IDbConnection conn)
        {
            var issues = new List<string>();

            try
            {
                // Vérifier les dates incohérentes
                var futureDates = await conn.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM t_eleves WHERE date_naiss > date('now')");
                
                if (futureDates > 0)
                {
                    issues.Add($"{futureDates} élèves avec des dates de naissance futures");
                }

                // Vérifier les âges incohérents
                var tooOld = await conn.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM t_eleves WHERE date_naiss < date('now', '-100 years')");
                
                if (tooOld > 0)
                {
                    issues.Add($"{tooOld} élèves avec des âges supérieurs à 100 ans");
                }
            }
            catch
            {
                // Erreurs de requête ou tables manquantes
            }

            return issues;
        }

        /// <summary>
        /// Vérifie s'il y a des modifications en attente de synchronisation
        /// </summary>
        private async Task<bool> CheckPendingChanges()
        {
            try
            {
                using var conn = _connexion.GetSQLiteConnection();
                conn.Open();

                var recentChanges = await conn.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM t_eleves WHERE updated_at > datetime('now', '-1 hour')");

                return recentChanges > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Calcule le score de santé global
        /// </summary>
        private double CalculateHealthScore(CompleteDiagnosticResult result)
        {
            double score = 0;
            int totalTests = 0;

            // Connectivité (30%)
            if (result.ConnectivityTest.Success)
            {
                score += 30;
                if (result.ConnectivityTest.MySQLAvailable) score += 5;
                if (result.ConnectivityTest.SQLiteAvailable) score += 5;
            }
            totalTests += 40;

            // Performance (25%)
            if (result.PerformanceTest.Success)
            {
                score += 20;
                if (result.PerformanceTest.MySQLPerformance?.Success == true) score += 2.5;
                if (result.PerformanceTest.SQLitePerformance?.Success == true) score += 2.5;
            }
            totalTests += 25;

            // Intégrité des données (25%)
            if (result.DataIntegrityTest.Success) score += 25;
            totalTests += 25;

            // Espace disque (10%)
            if (result.DiskSpaceTest.Success) score += 10;
            totalTests += 10;

            return Math.Min(100, (score / totalTests) * 100);
        }

        /// <summary>
        /// Détermine le statut de santé basé sur le score
        /// </summary>
        private HealthStatus DetermineHealthStatus(double score)
        {
            return score switch
            {
                >= 90 => HealthStatus.Excellent,
                >= 75 => HealthStatus.Good,
                >= 60 => HealthStatus.Fair,
                >= 40 => HealthStatus.Poor,
                _ => HealthStatus.Critical
            };
        }

        /// <summary>
        /// Génère des recommandations basées sur les résultats du diagnostic
        /// </summary>
        private List<string> GenerateRecommendations(CompleteDiagnosticResult result)
        {
            var recommendations = new List<string>();

            if (!result.ConnectivityTest.MySQLAvailable)
            {
                recommendations.Add("Vérifiez que le serveur MySQL est démarré et accessible");
            }

            if (result.DiskSpaceTest.UsedSpacePercentage > 90)
            {
                recommendations.Add("Libérez de l'espace disque - utilisation > 90%");
            }

            if (result.DataIntegrityTest.ForeignKeyViolations.Count > 0)
            {
                recommendations.Add("Corrigez les violations de clés étrangères détectées");
            }

            if (result.PerformanceTest.MySQLPerformance?.SimpleQueryTime > 1000)
            {
                recommendations.Add("Performance MySQL dégradée - vérifiez la charge du serveur");
            }

            if (!result.SchemaValidation.IsValid)
            {
                recommendations.Add("Exécutez une migration de schéma pour corriger les incompatibilités");
            }

            if (recommendations.Count == 0)
            {
                recommendations.Add("Système en bon état - aucune action requise");
            }

            return recommendations;
        }

        /// <summary>
        /// Exécute des diagnostics automatiques périodiques
        /// </summary>
        private async void RunAutomaticDiagnostics(object? state)
        {
            try
            {
                var quickDiagnostic = await RunQuickDiagnostic();
                
                // Déclencher des alertes si nécessaire
                if (quickDiagnostic.OverallHealthScore < 60)
                {
                    OnPerformanceAlert(new PerformanceAlertEventArgs
                    {
                        AlertLevel = AlertLevel.Warning,
                        Message = $"Score de santé faible détecté: {quickDiagnostic.OverallHealthScore:F1}%",
                        Timestamp = DateTime.Now,
                        DiagnosticResult = quickDiagnostic
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors du diagnostic automatique: {ex.Message}");
            }
        }

        /// <summary>
        /// Exécute un diagnostic rapide (version allégée)
        /// </summary>
        public async Task<CompleteDiagnosticResult> RunQuickDiagnostic()
        {
            var result = new CompleteDiagnosticResult
            {
                StartTime = DateTime.Now
            };

            try
            {
                // Tests essentiels seulement
                result.ConnectivityTest = await TestConnectivity();
                result.DiskSpaceTest = await TestDiskSpace();
                
                result.OverallHealthScore = CalculateHealthScore(result);
                result.HealthStatus = DetermineHealthStatus(result.OverallHealthScore);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
            }

            result.EndTime = DateTime.Now;
            result.Duration = result.EndTime - result.StartTime;

            return result;
        }

        /// <summary>
        /// Déclenche l'événement de diagnostic terminé
        /// </summary>
        protected virtual void OnDiagnosticCompleted(DiagnosticEventArgs e)
        {
            DiagnosticCompleted?.Invoke(this, e);
        }

        /// <summary>
        /// Déclenche l'événement d'alerte de performance
        /// </summary>
        protected virtual void OnPerformanceAlert(PerformanceAlertEventArgs e)
        {
            PerformanceAlert?.Invoke(this, e);
        }

        /// <summary>
        /// Nettoie les ressources
        /// </summary>
        public void Dispose()
        {
            _diagnosticTimer?.Dispose();
            foreach (var counter in _performanceCounters.Values)
            {
                counter?.Dispose();
            }
        }
    }

    #region Classes de support pour les diagnostics

    public class CompleteDiagnosticResult
    {
        public bool Success { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string? Error { get; set; }
        
        public ConnectivityTestResult ConnectivityTest { get; set; } = new();
        public PerformanceTestResult PerformanceTest { get; set; } = new();
        public DataIntegrityTestResult DataIntegrityTest { get; set; } = new();
        public DiskSpaceTestResult DiskSpaceTest { get; set; } = new();
        public SynchronizationTestResult SynchronizationTest { get; set; } = new();
        public SchemaValidationResult SchemaValidation { get; set; } = new();
        
        public double OverallHealthScore { get; set; }
        public HealthStatus HealthStatus { get; set; }
        public List<string> Recommendations { get; set; } = new();
    }

    public class ConnectivityTestResult
    {
        public bool Success { get; set; }
        public bool MySQLAvailable { get; set; }
        public bool SQLiteAvailable { get; set; }
        public long MySQLConnectionTime { get; set; }
        public long SQLiteConnectionTime { get; set; }
        public long MySQLQueryTime { get; set; }
        public long SQLiteQueryTime { get; set; }
        public string? MySQLError { get; set; }
        public string? SQLiteError { get; set; }
    }

    public class PerformanceTestResult
    {
        public bool Success { get; set; }
        public DatabasePerformanceMetrics? MySQLPerformance { get; set; }
        public DatabasePerformanceMetrics? SQLitePerformance { get; set; }
    }

    public class DatabasePerformanceMetrics
    {
        public bool Success { get; set; }
        public string DatabaseType { get; set; } = string.Empty;
        public long SimpleQueryTime { get; set; }
        public long CountQueryTime { get; set; }
        public long ComplexQueryTime { get; set; }
        public long InsertTime { get; set; }
        public int RecordCount { get; set; }
        public string? Error { get; set; }
    }

    public class DataIntegrityTestResult
    {
        public bool Success { get; set; }
        public List<string> ForeignKeyViolations { get; set; } = new();
        public List<string> PrimaryKeyDuplicates { get; set; } = new();
        public List<string> OrphanedRecords { get; set; } = new();
        public List<string> DataConsistencyIssues { get; set; } = new();
        public string? Error { get; set; }
    }

    public class DiskSpaceTestResult
    {
        public bool Success { get; set; }
        public long AvailableSpace { get; set; }
        public long TotalSpace { get; set; }
        public double UsedSpacePercentage { get; set; }
        public long SQLiteDatabaseSize { get; set; }
        public string? Warning { get; set; }
        public string? Error { get; set; }
    }

    public class SynchronizationTestResult
    {
        public bool Success { get; set; }
        public bool HasPendingChanges { get; set; }
        public bool LastSyncSuccessful { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
    }

    public enum HealthStatus
    {
        Critical,
        Poor,
        Fair,
        Good,
        Excellent
    }

    public class DiagnosticEventArgs : EventArgs
    {
        public CompleteDiagnosticResult Result { get; set; } = new();
        public DateTime Timestamp { get; set; }
    }

    public class PerformanceAlertEventArgs : EventArgs
    {
        public AlertLevel AlertLevel { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public CompleteDiagnosticResult? DiagnosticResult { get; set; }
    }

    public enum AlertLevel
    {
        Info,
        Warning,
        Error,
        Critical
    }

    #endregion
}