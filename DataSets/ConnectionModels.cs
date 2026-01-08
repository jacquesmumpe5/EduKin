using System;

namespace EduKin.DataSets
{
    /// <summary>
    /// Détails d'une erreur de connexion
    /// </summary>
    public class ConnectionErrorDetails
    {
        public Exception Exception { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public bool IsRetryable { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Suggestion { get; set; } = string.Empty;
        public string ErrorType { get; set; } = string.Empty;
        public int ErrorCode { get; set; }
        public ErrorSeverity Severity { get; set; }
    }

    /// <summary>
    /// Niveaux de sévérité des erreurs
    /// </summary>
    public enum ErrorSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    /// <summary>
    /// Statistiques de connexion
    /// </summary>
    public class ConnectionStats
    {
        public bool IsOnline { get; set; }
        public string DatabaseType { get; set; } = string.Empty;
        public int RetryCount { get; set; }
        public DateTime LastCheckTime { get; set; }
        public string MySqlConnectionString { get; set; } = string.Empty;
        public string SQLiteConnectionString { get; set; } = string.Empty;
        public bool MonitoringActive { get; set; }
        public TimeSpan Uptime { get; set; }
        public int TotalConnections { get; set; }
        public int FailedConnections { get; set; }
        public double SuccessRate => TotalConnections > 0 ? (double)(TotalConnections - FailedConnections) / TotalConnections * 100 : 0;
    }

    /// <summary>
    /// Résultat d'un test de performance de connexion
    /// </summary>
    public class ConnectionPerformanceResult
    {
        public bool Success { get; set; }
        public string DatabaseType { get; set; } = string.Empty;
        public long ConnectionTime { get; set; }
        public long QueryTime { get; set; }
        public long TotalTime { get; set; }
        public string Error { get; set; } = string.Empty;
        public DateTime TestTime { get; set; } = DateTime.Now;
        
        public string GetPerformanceRating()
        {
            if (!Success) return "Échec";
            
            return TotalTime switch
            {
                < 100 => "Excellent",
                < 500 => "Bon",
                < 1000 => "Moyen",
                < 2000 => "Lent",
                _ => "Très lent"
            };
        }
    }

    /// <summary>
    /// Événement de monitoring de connexion
    /// </summary>
    public class ConnectionMonitoringEventArgs : EventArgs
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool IsOnline { get; set; }
        public string DatabaseType { get; set; } = string.Empty;
        public long ResponseTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public Exception? Error { get; set; }
    }

    /// <summary>
    /// Configuration de la surveillance de connexion
    /// </summary>
    public class ConnectionMonitoringConfig
    {
        public TimeSpan CheckInterval { get; set; } = TimeSpan.FromSeconds(10);
        public int MaxRetryAttempts { get; set; } = 3;
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(5);
        public bool EnableLogging { get; set; } = true;
        public bool EnableFileLogging { get; set; } = true;
        public bool EnableSQLiteLogging { get; set; } = true;
        public int LogRetentionDays { get; set; } = 30;
        public bool AutoFallbackToSQLite { get; set; } = true;
        public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }

    /// <summary>
    /// Métriques de base de données
    /// </summary>
    public class DatabaseMetrics
    {
        public string DatabaseType { get; set; } = string.Empty;
        public long TotalQueries { get; set; }
        public long SuccessfulQueries { get; set; }
        public long FailedQueries { get; set; }
        public double AverageQueryTime { get; set; }
        public DateTime LastQueryTime { get; set; }
        public long TotalDataTransferred { get; set; }
        public int ActiveConnections { get; set; }
        public double SuccessRate => TotalQueries > 0 ? (double)SuccessfulQueries / TotalQueries * 100 : 0;
    }

    /// <summary>
    /// État de santé de la base de données
    /// </summary>
    public class DatabaseHealthStatus
    {
        public bool IsHealthy { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<string> Issues { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public DateTime LastCheckTime { get; set; } = DateTime.Now;
        public DatabaseMetrics Metrics { get; set; } = new();
        
        public HealthLevel GetHealthLevel()
        {
            if (!IsHealthy) return HealthLevel.Critical;
            if (Issues.Count > 0) return HealthLevel.Warning;
            return HealthLevel.Good;
        }
    }

    /// <summary>
    /// Niveaux de santé de la base de données
    /// </summary>
    public enum HealthLevel
    {
        Good,
        Warning,
        Critical
    }
}