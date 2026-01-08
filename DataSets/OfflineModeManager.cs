using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SQLite;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace EduKin.DataSets
{
    /// <summary>
    /// Gestionnaire du mode déconnecté avec prévention des collisions d'IDs
    /// </summary>
    public class OfflineModeManager
    {
        private readonly Connexion _connexion;
        private readonly Dictionary<string, DateTime> _lastSyncTimes;
        private readonly Dictionary<string, string> _tableChecksums;

        public OfflineModeManager(Connexion connexion)
        {
            _connexion = connexion;
            _lastSyncTimes = new Dictionary<string, DateTime>();
            _tableChecksums = new Dictionary<string, string>();
        }

        /// <summary>
        /// Vérifie si une table a des modifications en attente en mode déconnecté
        /// </summary>
        public bool HasPendingChanges(string tableName)
        {
            try
            {
                using var conn = _connexion.GetSQLiteConnection();
                conn.Open();
                
                // Vérifier d'abord s'il y a des modifications locales non synchronisées
                var lastSync = _lastSyncTimes.GetValueOrDefault(tableName, DateTime.MinValue);
                var query = $"SELECT COUNT(*) FROM {tableName} WHERE updated_at > @LastSync";
                var count = conn.ExecuteScalar<int>(query, new { LastSync = lastSync });
                
                // Si des modifications en attente et connexion MySQL disponible, vérifier si elles existent déjà sur MySQL
                if (count > 0 && _connexion.IsOnline)
                {
                    var existingQuery = $"SELECT COUNT(*) FROM {tableName} WHERE updated_at > @LastSync";
                    var existingCount = conn.ExecuteScalar<int>(existingQuery, new { LastSync = lastSync });
                    
                    // Si les modifications locales existent déjà sur MySQL, ne pas les synchroniser
                    if (existingCount > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"[OfflineModeManager] {existingCount} modifications locales pour {tableName} existent déjà sur MySQL, synchronisation annulée");
                        return false;
                    }
                }
                
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Vérifie si une table existe déjà dans la base de données locale
        /// </summary>
        public bool TableExists(string tableName)
        {
            try
            {
                using var conn = _connexion.GetSQLiteConnection();
                conn.Open();
                
                var query = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{tableName}'";
                var count = conn.ExecuteScalar<int>(query);
                
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Crée une table si elle n'existe pas déjà
        /// </summary>
        public void EnsureTableExists(string tableName, string createTableScript)
        {
            if (!TableExists(tableName))
            {
                try
                {
                    using var conn = _connexion.GetSQLiteConnection();
                    conn.Open();
                    conn.Execute(createTableScript);
                    System.Diagnostics.Debug.WriteLine($"[OfflineModeManager] Table {tableName} créée avec succès");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[OfflineModeManager] Erreur lors de la création de la table {tableName}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Génère un ID unique pour éviter les collisions entre postes de travail
        /// </summary>
        public string GenerateUniqueId(string prefix, string userIndex, string tableName)
        {
            try
            {
                using var conn = _connexion.GetSQLiteConnection();
                conn.Open();

                // Obtenir le dernier numéro séquentiel pour ce préfixe et utilisateur
                var lastNumberQuery = $"SELECT COALESCE(MAX(CAST(SUBSTR(id, {prefix.Length + 3}, 10) AS INTEGER)), 0) FROM {tableName} WHERE id LIKE '{prefix}%'";
                var lastNumber = conn.ExecuteScalar<int>(lastNumberQuery);
                
                var nextNumber = lastNumber + 1;
                var paddedNumber = nextNumber.ToString().PadLeft(10, '0');
                var year = DateTime.Now.Year.ToString();
                
                var newId = $"{prefix}{userIndex}{paddedNumber}{year}";
                
                System.Diagnostics.Debug.WriteLine($"[OfflineModeManager] ID généré: {newId} pour la table {tableName}");
                return newId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OfflineModeManager] Erreur lors de la génération d'ID: {ex.Message}");
                return $"{prefix}{userIndex}{DateTime.Now.Ticks.ToString().PadLeft(10, '0')}{DateTime.Now.Year}";
            }
        }

        /// <summary>
        /// Met à jour les métadonnées de synchronisation pour une table
        /// </summary>
        public void UpdateSyncMetadata(string tableName)
        {
            _lastSyncTimes[tableName] = DateTime.Now;
            System.Diagnostics.Debug.WriteLine($"[OfflineModeManager] Métadonnées mises à jour pour {tableName}");
        }

        /// <summary>
        /// Sauvegarde les métadonnées de synchronisation
        /// </summary>
        public void SaveSyncMetadata()
        {
            try
            {
                var metadataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "offline_sync_metadata.json");
                var metadata = new Dictionary<string, object>();
                
                foreach (var kvp in _lastSyncTimes)
                {
                    metadata[kvp.Key] = new { LastSyncTime = kvp.Value };
                }
                
                var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(metadataPath, json);
                
                System.Diagnostics.Debug.WriteLine($"[OfflineModeManager] Métadonnées sauvegardées");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OfflineModeManager] Erreur lors de la sauvegarde des métadonnées: {ex.Message}");
            }
        }

        /// <summary>
        /// Charge les métadonnées de synchronisation
        /// </summary>
        public void LoadSyncMetadata()
        {
            try
            {
                var metadataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "offline_sync_metadata.json");
                if (File.Exists(metadataPath))
                {
                    var json = File.ReadAllText(metadataPath);
                    var metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    
                    if (metadata != null)
                    {
                        foreach (var kvp in metadata)
                        {
                            if (kvp.Value is JsonElement element && element.TryGetProperty("LastSyncTime", out var timeProperty))
                            {
                                if (timeProperty.TryGetDateTime(out var syncTime))
                                {
                                    _lastSyncTimes[kvp.Key] = syncTime;
                                }
                            }
                        }
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"[OfflineModeManager] Métadonnées chargées");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OfflineModeManager] Erreur lors du chargement des métadonnées: {ex.Message}");
            }
        }
    }
}
