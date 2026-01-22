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
    /// Gestionnaire de synchronisation avec mode déconnecté amélioré
    /// </summary>
    public class SyncManager
    {
        private readonly Connexion _connexion;
        private System.Threading.Timer? _syncTimer;
        private readonly int _syncIntervalMinutes = 5;
        private readonly Dictionary<string, DateTime> _lastSyncTimes;
        private readonly Dictionary<string, string> _tableChecksums;
        private readonly OfflineModeManager _offlineModeManager;
        private bool _isSyncing;
        private readonly object _syncLock;

        // Événements pour notifier le progrès et la completion
        public event EventHandler<SyncProgressEventArgs>? SyncProgress;
        public event EventHandler<SyncEventArgs>? SyncCompleted;
        public event EventHandler<SyncEventArgs>? SyncFailed;

        public SyncManager()
        {
            _connexion = Connexion.Instance;
            _lastSyncTimes = new Dictionary<string, DateTime>();
            _tableChecksums = new Dictionary<string, string>();
            _offlineModeManager = new OfflineModeManager(_connexion);
            _syncLock = new object();
            
            LoadSyncMetadata();
            _offlineModeManager.LoadSyncMetadata();
        }

        public void StartAutoSync()
        {
            _syncTimer = new System.Threading.Timer(
                AutoSyncCallback,
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(_syncIntervalMinutes)
            );
        }

        public void StopAutoSync()
        {
            _syncTimer?.Dispose();
            _syncTimer = null;
        }

        private void AutoSyncCallback(object? state)
        {
            if (_isSyncing) return;

            _connexion.RefreshConnectionStatus();
            
            if (_connexion.IsOnline)
            {
                _ = Task.Run(() => SyncBidirectional());
            }
        }

        /// <summary>
        /// Charge les métadonnées de synchronisation depuis le fichier local
        /// </summary>
        private void LoadSyncMetadata()
        {
            try
            {
                var metadataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sync_metadata.json");
                if (File.Exists(metadataPath))
                {
                    var json = File.ReadAllText(metadataPath);
                    var metadata = JsonSerializer.Deserialize<Dictionary<string, SyncMetadata>>(json);
                    if (metadata != null)
                    {
                        foreach (var item in metadata)
                        {
                            _lastSyncTimes[item.Key] = item.Value.LastSyncTime;
                            _tableChecksums[item.Key] = item.Value.LastChecksum;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement des métadonnées de sync: {ex.Message}");
            }
        }

        /// <summary>
        /// Sauvegarde les métadonnées de synchronisation
        /// </summary>
        private void SaveSyncMetadata()
        {
            try
            {
                var metadata = new Dictionary<string, SyncMetadata>();
                foreach (var table in _lastSyncTimes.Keys)
                {
                    metadata[table] = new SyncMetadata
                    {
                        TableName = table,
                        LastSyncTime = _lastSyncTimes.GetValueOrDefault(table, DateTime.MinValue),
                        LastChecksum = _tableChecksums.GetValueOrDefault(table, ""),
                        HasPendingChanges = HasPendingChanges(table)
                    };
                }

                var metadataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sync_metadata.json");
                var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(metadataPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la sauvegarde des métadonnées: {ex.Message}");
            }
        }

        public bool SyncToMySQL()
        {
            try
            {
                if (!_connexion.IsOnline)
                {
                    OnSyncFailed(new SyncEventArgs { Message = "Connexion MySQL non disponible" });
                    return false;
                }

                using (var sqliteConn = _connexion.GetSQLiteConnection())
                using (var mysqlConn = _connexion.GetMySqlConnection())
                {
                    sqliteConn.Open();
                    mysqlConn.Open();

                    using (var transaction = mysqlConn.BeginTransaction())
                    {
                        try
                        {
                            // Synchronisation des tables principales
                            SyncTable(sqliteConn, mysqlConn, transaction, "t_eleves");
                            SyncTable(sqliteConn, mysqlConn, transaction, "t_agents");
                            SyncTable(sqliteConn, mysqlConn, transaction, "t_affectation");
                            SyncTable(sqliteConn, mysqlConn, transaction, "t_affect_prof");
                            SyncTable(sqliteConn, mysqlConn, transaction, "t_affect_cours");
                            SyncTable(sqliteConn, mysqlConn, transaction, "t_dettes");
                            SyncTable(sqliteConn, mysqlConn, transaction, "t_cours");
                            SyncTable(sqliteConn, mysqlConn, transaction, "t_grille");
                            SyncTable(sqliteConn, mysqlConn, transaction, "t_coupons");
                            SyncTable(sqliteConn, mysqlConn, transaction, "t_caisse");

                            transaction.Commit();
                            OnSyncCompleted(new SyncEventArgs { Message = "Synchronisation réussie", Success = true });
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            OnSyncFailed(new SyncEventArgs { Message = $"Erreur lors de la synchronisation: {ex.Message}" });
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnSyncFailed(new SyncEventArgs { Message = $"Erreur de connexion: {ex.Message}" });
                return false;
            }
        }

        private void SyncTable(SQLiteConnection sqliteConn, MySqlConnection mysqlConn, MySqlTransaction transaction, string tableName)
        {
            // Récupérer les données modifiées depuis SQLite
            var query = $"SELECT * FROM {tableName} WHERE updated_at > (SELECT COALESCE(MAX(updated_at), '1900-01-01') FROM {tableName})";
            
            try
            {
                var data = sqliteConn.Query(query);
                
                foreach (var row in data)
                {
                    // Insérer ou mettre à jour dans MySQL
                    var properties = ((IDictionary<string, object>)row);
                    var columns = string.Join(", ", properties.Keys);
                    var parameters = string.Join(", ", properties.Keys.Select(k => $"@{k}"));
                    
                    var upsertQuery = $@"
                        INSERT INTO {tableName} ({columns}) 
                        VALUES ({parameters})
                        ON DUPLICATE KEY UPDATE {string.Join(", ", properties.Keys.Select(k => $"{k}=VALUES({k})"))}";
                    
                    // Cast explicite pour éviter l'erreur CS1973 avec les méthodes d'extension Dapper
                    mysqlConn.Execute(upsertQuery, (object)row, transaction);
                }
            }
            catch
            {
                // Table n'existe pas ou pas de colonne updated_at, ignorer
            }
        }

        public bool SyncFromMySQL()
        {
            try
            {
                if (!_connexion.IsOnline)
                {
                    return false;
                }

                using (var sqliteConn = _connexion.GetSQLiteConnection())
                using (var mysqlConn = _connexion.GetMySqlConnection())
                {
                    sqliteConn.Open();
                    mysqlConn.Open();

                    // Création des tables SQLite désactivée - les objets DB doivent être créés manuellement
                    System.Diagnostics.Debug.WriteLine("Création des tables SQLite désactivée - les objets DB doivent être créés manuellement");

                    // Synchroniser depuis MySQL vers SQLite
                    SyncTableFromMySQL(mysqlConn, sqliteConn, "t_eleves");
                    SyncTableFromMySQL(mysqlConn, sqliteConn, "t_agents");
                    SyncTableFromMySQL(mysqlConn, sqliteConn, "t_affectation");
                    SyncTableFromMySQL(mysqlConn, sqliteConn, "t_affect_prof");
                    SyncTableFromMySQL(mysqlConn, sqliteConn, "t_affect_cours");
                    SyncTableFromMySQL(mysqlConn, sqliteConn, "t_dettes");
                    SyncTableFromMySQL(mysqlConn, sqliteConn, "t_cours");
                    SyncTableFromMySQL(mysqlConn, sqliteConn, "t_grille");

                    OnSyncCompleted(new SyncEventArgs { Message = "Synchronisation depuis MySQL réussie", Success = true });
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnSyncFailed(new SyncEventArgs { Message = $"Erreur: {ex.Message}" });
                return false;
            }
        }

        private void SyncTableFromMySQL(MySqlConnection mysqlConn, SQLiteConnection sqliteConn, string tableName)
        {
            var data = mysqlConn.Query($"SELECT * FROM {tableName}");
            
            foreach (var row in data)
            {
                var properties = ((IDictionary<string, object>)row);
                var columns = string.Join(", ", properties.Keys);
                var parameters = string.Join(", ", properties.Keys.Select(k => $"@{k}"));
                
                var insertQuery = $"INSERT OR REPLACE INTO {tableName} ({columns}) VALUES ({parameters})";
                // Cast explicite pour éviter l'erreur CS1973 avec les méthodes d'extension Dapper
                sqliteConn.Execute(insertQuery, (object)row);
            }
        }

        /// <summary>
        /// Synchronisation bidirectionnelle intelligente avec gestion des conflits
        /// </summary>
        public async Task<bool> SyncBidirectional()
        {
            if (_isSyncing) return false;

            lock (_syncLock)
            {
                if (_isSyncing) return false;
                _isSyncing = true;
            }

            try
            {
                var startTime = DateTime.Now;
                var tables = GetSyncTables();
                var totalTables = tables.Count;
                var processedTables = 0;

                foreach (var table in tables)
                {
                    try
                    {
                        OnSyncProgress(new SyncProgressEventArgs
                        {
                            TableName = table,
                            CurrentRecord = processedTables,
                            TotalRecords = totalTables,
                            Status = $"Synchronisation de {table}..."
                        });

                        await SyncTableBidirectional(table);
                        processedTables++;
                    }
                    catch (Exception ex)
                    {
                        OnSyncFailed(new SyncEventArgs
                        {
                            Message = $"Erreur lors de la synchronisation de {table}: {ex.Message}",
                            TableName = table,
                            Error = ex,
                            Success = false
                        });
                    }
                }

                SaveSyncMetadata();
                var duration = DateTime.Now - startTime;

                OnSyncCompleted(new SyncEventArgs
                {
                    Message = $"Synchronisation bidirectionnelle terminée ({processedTables}/{totalTables} tables)",
                    Success = true,
                    RecordsProcessed = processedTables,
                    Duration = duration
                });

                return true;
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
                    System.Diagnostics.Debug.WriteLine($"[SyncManager] Table {tableName} créée avec succès");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SyncManager] Erreur lors de la création de la table {tableName}: {ex.Message}");
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
                
                System.Diagnostics.Debug.WriteLine($"[SyncManager] ID généré: {newId} pour la table {tableName}");
                return newId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SyncManager] Erreur lors de la génération d'ID: {ex.Message}");
                return $"{prefix}{userIndex}{DateTime.Now.Ticks.ToString().PadLeft(10, '0')}{DateTime.Now.Year}";
            }
        }

        /// <summary>
        /// Synchronise une table spécifique de manière bidirectionnelle
        /// </summary>
        private async Task SyncTableBidirectional(string tableName)
        {
            using var sqliteConn = _connexion.GetSQLiteConnection();
            using var mysqlConn = _connexion.GetMySqlConnection();

            sqliteConn.Open();
            mysqlConn.Open();

            // 1. Vérifier les modifications locales (SQLite)
            var localChanges = await GetLocalChanges(sqliteConn, tableName);
            
            // 2. Vérifier les modifications distantes (MySQL)
            var remoteChanges = await GetRemoteChanges(mysqlConn, tableName);

            // 3. Détecter et résoudre les conflits
            var conflicts = DetectConflicts(localChanges, remoteChanges, tableName);
            await ResolveConflicts(conflicts, sqliteConn, mysqlConn, tableName);

            // 4. Appliquer les changements non conflictuels
            await ApplyLocalChangesToRemote(localChanges, mysqlConn, tableName);
            await ApplyRemoteChangesToLocal(remoteChanges, sqliteConn, tableName);

            // 5. Mettre à jour les métadonnées
            UpdateSyncMetadata(tableName);
        }

        /// <summary>
        /// Récupère les modifications locales depuis la dernière synchronisation
        /// </summary>
        private async Task<List<Dictionary<string, object>>> GetLocalChanges(SQLiteConnection conn, string tableName)
        {
            var lastSync = _lastSyncTimes.GetValueOrDefault(tableName, DateTime.MinValue);
            var query = SqlCompatibilityAdapter.AdaptQueryForSQLite(
                $"SELECT * FROM {tableName} WHERE updated_at > @LastSync OR created_at > @LastSync"
            );

            try
            {
                var result = await conn.QueryAsync(query, new { LastSync = lastSync });
                return result.Select(r => (Dictionary<string, object>)r).ToList();
            }
            catch
            {
                // Table sans colonnes de timestamp, récupérer toutes les données
                var allDataQuery = $"SELECT * FROM {tableName}";
                var result = await conn.QueryAsync(allDataQuery);
                return result.Select(r => (Dictionary<string, object>)r).ToList();
            }
        }

        /// <summary>
        /// Récupère les modifications distantes depuis la dernière synchronisation
        /// </summary>
        private async Task<List<Dictionary<string, object>>> GetRemoteChanges(MySqlConnection conn, string tableName)
        {
            var lastSync = _lastSyncTimes.GetValueOrDefault(tableName, DateTime.MinValue);
            var query = $"SELECT * FROM {tableName} WHERE updated_at > @LastSync OR created_at > @LastSync";

            try
            {
                var result = await conn.QueryAsync(query, new { LastSync = lastSync });
                return result.Select(r => (Dictionary<string, object>)r).ToList();
            }
            catch
            {
                // Table sans colonnes de timestamp, utiliser checksum pour détecter les changements
                return await GetChangesUsingChecksum(conn, tableName);
            }
        }

        /// <summary>
        /// Détecte les changements en utilisant un checksum de table
        /// </summary>
        private async Task<List<Dictionary<string, object>>> GetChangesUsingChecksum(IDbConnection conn, string tableName)
        {
            var currentChecksum = await CalculateTableChecksum(conn, tableName);
            var lastChecksum = _tableChecksums.GetValueOrDefault(tableName, "");

            if (currentChecksum != lastChecksum)
            {
                _tableChecksums[tableName] = currentChecksum;
                var result = await conn.QueryAsync($"SELECT * FROM {tableName}");
                return result.Select(r => (Dictionary<string, object>)r).ToList();
            }

            return new List<Dictionary<string, object>>();
        }

        /// <summary>
        /// Calcule un checksum pour une table
        /// </summary>
        private async Task<string> CalculateTableChecksum(IDbConnection conn, string tableName)
        {
            try
            {
                var query = conn is MySqlConnection 
                    ? $"SELECT MD5(GROUP_CONCAT(CONCAT_WS('|', *) ORDER BY 1)) FROM {tableName}"
                    : $"SELECT COUNT(*) || '|' || COALESCE(MAX(rowid), 0) FROM {tableName}";

                var result = await conn.QueryFirstOrDefaultAsync<string>(query);
                return result ?? "";
            }
            catch
            {
                return DateTime.Now.Ticks.ToString();
            }
        }

        /// <summary>
        /// Détecte les conflits entre les modifications locales et distantes
        /// </summary>
        private List<SyncConflictEventArgs> DetectConflicts(
            List<Dictionary<string, object>> localChanges,
            List<Dictionary<string, object>> remoteChanges,
            string tableName)
        {
            var conflicts = new List<SyncConflictEventArgs>();
            var primaryKey = GetPrimaryKeyColumn(tableName);

            foreach (var localRecord in localChanges)
            {
                var localId = localRecord.GetValueOrDefault(primaryKey)?.ToString();
                if (string.IsNullOrEmpty(localId)) continue;

                var conflictingRemote = remoteChanges.FirstOrDefault(r => 
                    r.GetValueOrDefault(primaryKey)?.ToString() == localId);

                if (conflictingRemote != null)
                {
                    conflicts.Add(new SyncConflictEventArgs
                    {
                        TableName = tableName,
                        RecordId = localId,
                        LocalData = localRecord,
                        RemoteData = conflictingRemote,
                        Resolution = DetermineConflictResolution(localRecord, conflictingRemote)
                    });
                }
            }

            return conflicts;
        }

        /// <summary>
        /// Détermine automatiquement la résolution d'un conflit
        /// </summary>
        private ConflictResolution DetermineConflictResolution(
            Dictionary<string, object> localData,
            Dictionary<string, object> remoteData)
        {
            // Stratégie: utiliser la version la plus récente
            var localUpdated = GetTimestamp(localData, "updated_at") ?? GetTimestamp(localData, "created_at");
            var remoteUpdated = GetTimestamp(remoteData, "updated_at") ?? GetTimestamp(remoteData, "created_at");

            if (localUpdated.HasValue && remoteUpdated.HasValue)
            {
                return localUpdated > remoteUpdated ? ConflictResolution.UseLocal : ConflictResolution.UseRemote;
            }

            // Par défaut, privilégier la version distante
            return ConflictResolution.UseRemote;
        }

        /// <summary>
        /// Résout les conflits détectés
        /// </summary>
        private async Task ResolveConflicts(
            List<SyncConflictEventArgs> conflicts,
            SQLiteConnection sqliteConn,
            MySqlConnection mysqlConn,
            string tableName)
        {
            foreach (var conflict in conflicts)
            {
                try
                {
                    switch (conflict.Resolution)
                    {
                        case ConflictResolution.UseLocal:
                            await ApplyRecordToRemote(conflict.LocalData, mysqlConn, tableName);
                            break;

                        case ConflictResolution.UseRemote:
                            await ApplyRecordToLocal(conflict.RemoteData, sqliteConn, tableName);
                            break;

                        case ConflictResolution.Merge:
                            var mergedData = MergeRecords(conflict.LocalData, conflict.RemoteData);
                            await ApplyRecordToLocal(mergedData, sqliteConn, tableName);
                            await ApplyRecordToRemote(mergedData, mysqlConn, tableName);
                            break;

                        case ConflictResolution.Skip:
                            // Ne rien faire
                            break;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erreur lors de la résolution du conflit pour {conflict.RecordId}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Fusionne deux enregistrements en cas de conflit
        /// </summary>
        private Dictionary<string, object> MergeRecords(
            Dictionary<string, object> localData,
            Dictionary<string, object> remoteData)
        {
            var merged = new Dictionary<string, object>(remoteData);

            // Stratégie de fusion: privilégier les valeurs non nulles les plus récentes
            foreach (var kvp in localData)
            {
                if (kvp.Value != null && kvp.Value != DBNull.Value)
                {
                    // Garder la valeur locale si elle n'est pas nulle
                    if (!merged.ContainsKey(kvp.Key) || 
                        merged[kvp.Key] == null || 
                        merged[kvp.Key] == DBNull.Value)
                    {
                        merged[kvp.Key] = kvp.Value;
                    }
                }
            }

            return merged;
        }

        /// <summary>
        /// Applique les modifications locales vers la base distante
        /// </summary>
        private async Task ApplyLocalChangesToRemote(
            List<Dictionary<string, object>> localChanges,
            MySqlConnection mysqlConn,
            string tableName)
        {
            foreach (var record in localChanges)
            {
                await ApplyRecordToRemote(record, mysqlConn, tableName);
            }
        }

        /// <summary>
        /// Applique les modifications distantes vers la base locale
        /// </summary>
        private async Task ApplyRemoteChangesToLocal(
            List<Dictionary<string, object>> remoteChanges,
            SQLiteConnection sqliteConn,
            string tableName)
        {
            foreach (var record in remoteChanges)
            {
                await ApplyRecordToLocal(record, sqliteConn, tableName);
            }
        }

        /// <summary>
        /// Applique un enregistrement vers MySQL
        /// </summary>
        private async Task ApplyRecordToRemote(Dictionary<string, object> record, MySqlConnection conn, string tableName)
        {
            var columns = string.Join(", ", record.Keys);
            var parameters = string.Join(", ", record.Keys.Select(k => $"@{k}"));
            var updateClause = string.Join(", ", record.Keys.Select(k => $"{k}=VALUES({k})"));

            var query = $@"INSERT INTO {tableName} ({columns}) 
                          VALUES ({parameters})
                          ON DUPLICATE KEY UPDATE {updateClause}";

            await conn.ExecuteAsync(query, record);
        }

        /// <summary>
        /// Applique un enregistrement vers SQLite
        /// </summary>
        private async Task ApplyRecordToLocal(Dictionary<string, object> record, SQLiteConnection conn, string tableName)
        {
            var columns = string.Join(", ", record.Keys);
            var parameters = string.Join(", ", record.Keys.Select(k => $"@{k}"));

            var query = SqlCompatibilityAdapter.AdaptQueryForSQLite(
                $"INSERT OR REPLACE INTO {tableName} ({columns}) VALUES ({parameters})"
            );

            await conn.ExecuteAsync(query, record);
        }

        /// <summary>
        /// Met à jour les métadonnées de synchronisation pour une table
        /// </summary>
        private void UpdateSyncMetadata(string tableName)
        {
            _lastSyncTimes[tableName] = DateTime.Now;
        }

        /// <summary>
        /// Vérifie si une table a des modifications en attente
        /// </summary>
        private bool HasPendingChanges(string tableName)
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
                        System.Diagnostics.Debug.WriteLine($"[SyncManager] {existingCount} modifications locales pour {tableName} existent déjà sur MySQL, synchronisation annulée");
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

        private List<string> GetSyncTables()
        {
            return new List<string>
            {
                "t_eleves", "t_agents", "t_affectation", "t_affect_prof",
                "t_affect_cours", "t_dettes", "t_cours", "t_grilles",
                "t_coupons", "t_caisse", "t_paiement", "t_entree"
            };
        }

        /// <summary>
        /// Obtient le nom de la colonne clé primaire pour une table
        /// </summary>
        private string GetPrimaryKeyColumn(string tableName)
        {
            var primaryKeys = new Dictionary<string, string>
            {
                { "t_eleves", "matricule" },
                { "t_agents", "matricule" },
                { "t_affectation", "id_affect" },
                { "t_affect_prof", "num" },
                { "t_affect_cours", "num" },
                { "t_dettes", "id_dettes" },
                { "t_cours", "id_cours" },
                { "t_grilles", "num" },
                { "t_coupons", "num_coupon" },
                { "t_caisse", "id_caisse" },
                { "t_paiement", "num_recu" },
                { "t_entree", "id_entree" }
            };

            return primaryKeys.GetValueOrDefault(tableName, "id");
        }

        /// <summary>
        /// Extrait un timestamp d'un enregistrement
        /// </summary>
        private DateTime? GetTimestamp(Dictionary<string, object> record, string columnName)
        {
            if (record.TryGetValue(columnName, out var value) && value != null)
            {
                if (DateTime.TryParse(value.ToString(), out var dateTime))
                {
                    return dateTime;
                }
            }
            return null;
        }

        /// <summary>
        /// Déclenche l'événement de progression
        /// </summary>
        protected virtual void OnSyncProgress(SyncProgressEventArgs e)
        {
            SyncProgress?.Invoke(this, e);
        }

        protected virtual void OnSyncCompleted(SyncEventArgs e)
        {
            SyncCompleted?.Invoke(this, e);
        }

        protected virtual void OnSyncFailed(SyncEventArgs e)
        {
            SyncFailed?.Invoke(this, e);
        }
    }

    public class SyncEventArgs : EventArgs
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? TableName { get; set; }
        public int RecordsProcessed { get; set; }
        public TimeSpan Duration { get; set; }
        public Exception? Error { get; set; }
    }

    public class SyncProgressEventArgs : EventArgs
    {
        public string TableName { get; set; } = string.Empty;
        public int CurrentRecord { get; set; }
        public int TotalRecords { get; set; }
        public double ProgressPercentage => TotalRecords > 0 ? (double)CurrentRecord / TotalRecords * 100 : 0;
        public string Status { get; set; } = string.Empty;
    }

    public class SyncConflictEventArgs : EventArgs
    {
        public string TableName { get; set; } = string.Empty;
        public string RecordId { get; set; } = string.Empty;
        public Dictionary<string, object> LocalData { get; set; } = new();
        public Dictionary<string, object> RemoteData { get; set; } = new();
        public ConflictResolution Resolution { get; set; } = ConflictResolution.UseRemote;
    }

    public enum ConflictResolution
    {
        UseLocal,
        UseRemote,
        Merge,
        Skip
    }

    public class SyncMetadata
    {
        public string TableName { get; set; } = string.Empty;
        public DateTime LastSyncTime { get; set; }
        public string LastChecksum { get; set; } = string.Empty;
        public int RecordCount { get; set; }
        public bool HasPendingChanges { get; set; }
    }
}
