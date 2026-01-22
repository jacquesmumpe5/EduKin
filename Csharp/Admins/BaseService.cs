using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using EduKin.DataSets;
using EduKin.Inits;
using System.Text.Json;
using System.Diagnostics;

namespace EduKin.Csharp.Admins
{
    /// <summary>
    /// Classe de base pour tous les services avec isolation automatique par école
    /// Gestion d'erreurs avancée et compatibilité SQLite/MySQL parfaite
    /// </summary>
    public abstract class BaseService
    {
        protected readonly Connexion _connexion;
        private readonly Dictionary<string, int> _retryCounters = new();
        private const int MAX_RETRY_ATTEMPTS = 3;
        private const int RETRY_DELAY_MS = 1000;

        protected BaseService()
        {
            _connexion = Connexion.Instance;
        }

        /// <summary>
        /// Exécute une requête SELECT avec isolation automatique par école et gestion d'erreurs avancée
        /// </summary>
        /// <param name="query">Requête SQL de base</param>
        /// <param name="parameters">Paramètres additionnels (optionnel)</param>
        /// <param name="tableAlias">Alias de la table pour la clause WHERE (optionnel)</param>
        /// <returns>Résultats de la requête</returns>
        public IEnumerable<dynamic> QueryWithIsolation(string query, object? parameters = null, string? tableAlias = null)
        {
            return ExecuteWithRetry(() =>
            {
                var adaptedQuery = SqlCompatibilityAdapter.GetAdaptedQuery(query, _connexion.IsOnline);
                var isolatedQuery = EduKinContext.AddIsolationClause(adaptedQuery, tableAlias);
                var combinedParameters = CombineParameters(parameters, EduKinContext.GetIsolationParameters());
                
                using (var conn = GetSecureConnection())
                {
                    conn.Open();
                    return conn.Query(isolatedQuery, combinedParameters).ToList();
                }
            }, $"QueryWithIsolation: {query.Substring(0, Math.Min(50, query.Length))}...");
        }

        /// <summary>
        /// Exécute une requête SELECT qui retourne un seul résultat avec isolation et gestion d'erreurs avancée
        /// </summary>
        /// <param name="query">Requête SQL de base</param>
        /// <param name="parameters">Paramètres additionnels (optionnel)</param>
        /// <param name="tableAlias">Alias de la table pour la clause WHERE (optionnel)</param>
        /// <returns>Premier résultat ou null</returns>
        protected dynamic? QueryFirstOrDefaultWithIsolation(string query, object? parameters = null, string? tableAlias = null)
        {
            return ExecuteWithRetry(() =>
            {
                var adaptedQuery = SqlCompatibilityAdapter.GetAdaptedQuery(query, _connexion.IsOnline);
                var isolatedQuery = EduKinContext.AddIsolationClause(adaptedQuery, tableAlias);
                var combinedParameters = CombineParameters(parameters, EduKinContext.GetIsolationParameters());
                
                using (var conn = GetSecureConnection())
                {
                    conn.Open();
                    return conn.QueryFirstOrDefault(isolatedQuery, combinedParameters);
                }
            }, $"QueryFirstOrDefaultWithIsolation: {query.Substring(0, Math.Min(50, query.Length))}...");
        }

        /// <summary>
        /// Exécute une requête COUNT avec isolation automatique et gestion d'erreurs avancée
        /// </summary>
        /// <param name="tableName">Nom de la table</param>
        /// <param name="whereClause">Clause WHERE additionnelle (optionnel)</param>
        /// <param name="parameters">Paramètres pour la clause WHERE (optionnel)</param>
        /// <returns>Nombre d'enregistrements</returns>
        protected int CountWithIsolation(string tableName, string? whereClause = null, object? parameters = null)
        {
            return ExecuteWithRetry(() =>
            {
                var query = $"SELECT COUNT(*) FROM {tableName}";
                
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query += $" WHERE {whereClause}";
                }
                
                var adaptedQuery = SqlCompatibilityAdapter.GetAdaptedQuery(query, _connexion.IsOnline);
                var isolatedQuery = EduKinContext.AddIsolationClause(adaptedQuery);
                var combinedParameters = CombineParameters(parameters, EduKinContext.GetIsolationParameters());
                
                using (var conn = GetSecureConnection())
                {
                    conn.Open();
                    return conn.ExecuteScalar<int>(isolatedQuery, combinedParameters);
                }
            }, $"CountWithIsolation: {tableName}");
        }

        /// <summary>
        /// Exécute une commande INSERT avec l'ID école automatiquement ajouté et gestion d'erreurs avancée
        /// </summary>
        /// <param name="tableName">Nom de la table</param>
        /// <param name="data">Données à insérer</param>
        /// <returns>Nombre de lignes affectées</returns>
        protected int InsertWithIsolation(string tableName, object data)
        {
            return ExecuteWithRetry(() =>
            {
                var dataDict = ObjectToDictionary(data);
                
                // Ajouter l'ID école seulement si la table le supporte
                if (TableSupportsIsolation(tableName))
                {
                    dataDict["fk_ecole"] = EduKinContext.CurrentIdEcole;
                }
                
                // Ajouter les timestamps automatiquement
                AddTimestamps(dataDict, isUpdate: false);
                
                var columns = string.Join(", ", dataDict.Keys);
                var values = string.Join(", ", dataDict.Keys.Select(k => $"@{k}"));
                
                var query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
                var adaptedQuery = SqlCompatibilityAdapter.GetAdaptedQuery(query, _connexion.IsOnline);
                
                using (var conn = GetSecureConnection())
                {
                    conn.Open();
                    return conn.Execute(adaptedQuery, dataDict);
                }
            }, $"InsertWithIsolation: {tableName}");
        }

        /// <summary>
        /// Exécute une commande UPDATE avec isolation automatique et gestion d'erreurs avancée
        /// </summary>
        /// <param name="tableName">Nom de la table</param>
        /// <param name="data">Données à mettre à jour</param>
        /// <param name="whereClause">Clause WHERE (sans le WHERE)</param>
        /// <param name="whereParameters">Paramètres pour la clause WHERE</param>
        /// <returns>Nombre de lignes affectées</returns>
        protected int UpdateWithIsolation(string tableName, object data, string whereClause, object? whereParameters = null)
        {
            return ExecuteWithRetry(() =>
            {
                var dataDict = ObjectToDictionary(data);
                
                // Ajouter les timestamps automatiquement
                AddTimestamps(dataDict, isUpdate: true);
                
                var setClause = string.Join(", ", dataDict.Keys.Select(k => $"{k} = @{k}"));
                
                var query = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";
                var adaptedQuery = SqlCompatibilityAdapter.GetAdaptedQuery(query, _connexion.IsOnline);
                var isolatedQuery = EduKinContext.AddIsolationClause(adaptedQuery);
                
                var combinedParameters = CombineParameters(dataDict, whereParameters, EduKinContext.GetIsolationParameters());
                
                using (var conn = GetSecureConnection())
                {
                    conn.Open();
                    return conn.Execute(isolatedQuery, combinedParameters);
                }
            }, $"UpdateWithIsolation: {tableName}");
        }

        /// <summary>
        /// Exécute une commande DELETE avec isolation automatique et gestion d'erreurs avancée
        /// </summary>
        /// <param name="tableName">Nom de la table</param>
        /// <param name="whereClause">Clause WHERE (sans le WHERE)</param>
        /// <param name="whereParameters">Paramètres pour la clause WHERE</param>
        /// <returns>Nombre de lignes supprimées</returns>
        protected int DeleteWithIsolation(string tableName, string whereClause, object? whereParameters = null)
        {
            return ExecuteWithRetry(() =>
            {
                var query = $"DELETE FROM {tableName} WHERE {whereClause}";
                var adaptedQuery = SqlCompatibilityAdapter.GetAdaptedQuery(query, _connexion.IsOnline);
                var isolatedQuery = EduKinContext.AddIsolationClause(adaptedQuery);
                
                var combinedParameters = CombineParameters(whereParameters, EduKinContext.GetIsolationParameters());
                
                using (var conn = GetSecureConnection())
                {
                    conn.Open();
                    return conn.Execute(isolatedQuery, combinedParameters);
                }
            }, $"DeleteWithIsolation: {tableName}");
        }

        /// <summary>
        /// Combine plusieurs objets de paramètres en un seul dictionnaire
        /// </summary>
        /// <param name="parameterObjects">Objets de paramètres à combiner</param>
        /// <returns>Dictionnaire combiné des paramètres</returns>
        protected Dictionary<string, object?> CombineParameters(params object?[] parameterObjects)
        {
            var combined = new Dictionary<string, object?>();
            
            foreach (var paramObj in parameterObjects)
            {
                if (paramObj == null) continue;
                
                var paramDict = ObjectToDictionary(paramObj);
                foreach (var kvp in paramDict)
                {
                    combined[kvp.Key] = kvp.Value;
                }
            }
            
            return combined;
        }

        /// <summary>
        /// Convertit un objet en dictionnaire de propriétés
        /// </summary>
        /// <param name="obj">Objet à convertir</param>
        /// <returns>Dictionnaire des propriétés</returns>
        protected Dictionary<string, object?> ObjectToDictionary(object obj)
        {
            if (obj == null)
                return new Dictionary<string, object?>();

            if (obj is Dictionary<string, object?> dict)
                return dict;

            var result = new Dictionary<string, object?>();
            var properties = obj.GetType().GetProperties();
            
            foreach (var prop in properties)
            {
                if (prop.CanRead)
                {
                    result[prop.Name] = prop.GetValue(obj);
                }
            }
            
            return result;
        }

        /// <summary>
        /// Vérifie si le contexte école est initialisé et lance une exception si ce n'est pas le cas
        /// </summary>
        protected void EnsureEduKinContextInitialized()
        {
            if (!EduKinContext.IsConfigured)
            {
                throw new InvalidOperationException("Le contexte de l'école n'a pas été initialisé. Veuillez sélectionner une école.");
            }
        }

        /// <summary>
        /// Exécute une requête avec gestion d'erreur et logging
        /// </summary>
        /// <typeparam name="T">Type de retour</typeparam>
        /// <param name="operation">Opération à exécuter</param>
        /// <param name="operationName">Nom de l'opération pour le logging</param>
        /// <returns>Résultat de l'opération</returns>
        protected T ExecuteWithErrorHandling<T>(Func<T> operation, string operationName)
        {
            try
            {
                EnsureEduKinContextInitialized();
                return operation();
            }
            catch (Exception ex)
            {
                var errorMessage = $"Erreur lors de l'opération '{operationName}' pour l'école {EduKinContext.CurrentDenomination}: {ex.Message}";
                System.Diagnostics.Debug.WriteLine(errorMessage);
                throw new InvalidOperationException(errorMessage, ex);
            }
        }

        /// <summary>
        /// Obtient une connexion à la base de données avec vérification du contexte
        /// </summary>
        /// <returns>Connexion à la base de données</returns>
        public IDbConnection GetSecureConnection()
        {
            EnsureEduKinContextInitialized();
            return _connexion.GetConnection();
        }

        /// <summary>
        /// Génère un nouvel ID unique pour une table avec isolation par école et gestion d'erreurs avancée
        /// </summary>
        /// <param name="tableName">Nom de la table</param>
        /// <param name="idColumn">Nom de la colonne ID</param>
        /// <param name="prefix">Préfixe de l'ID</param>
        /// <param name="defaultSuffix">Suffixe par défaut</param>
        /// <returns>Nouvel ID unique</returns>
        protected string GenerateUniqueId(string tableName, string idColumn, string prefix, string defaultSuffix)
        {
            return ExecuteWithRetry(() =>
            {
                var query = $"SELECT MAX({idColumn}) FROM {tableName}";
                var adaptedQuery = SqlCompatibilityAdapter.GetAdaptedQuery(query, _connexion.IsOnline);
                var isolatedQuery = EduKinContext.AddIsolationClause(adaptedQuery);
                
                using (var conn = GetSecureConnection())
                {
                    conn.Open();
                    var maxId = conn.QueryFirstOrDefault<string>(isolatedQuery, EduKinContext.GetIsolationParameters());
                    
                    if (string.IsNullOrEmpty(maxId))
                    {
                        return $"{prefix}{defaultSuffix}";
                    }
                    
                    // Extraire le numéro de la fin de l'ID
                    var numericPart = maxId.Substring(prefix.Length);
                    if (int.TryParse(numericPart, out var currentNum))
                    {
                        var nextNum = currentNum + 1;
                        return $"{prefix}{nextNum:D3}";
                    }
                    
                    return $"{prefix}{defaultSuffix}";
                }
            }, $"GenerateUniqueId: {tableName}.{idColumn}");
        }

        #region Méthodes de Gestion d'Erreurs Avancées

        /// <summary>
        /// Exécute une opération avec retry automatique en cas d'échec (version non-générique pour compatibilité)
        /// </summary>
        /// <param name="operation">Opération à exécuter</param>
        /// <param name="operationName">Nom de l'opération pour le logging</param>
        /// <returns>Résultat de l'opération</returns>
        public object ExecuteWithRetry(Func<object> operation, string operationName)
        {
            return ExecuteWithRetry<object>(operation, operationName);
        }

        /// <summary>
        /// Exécute une opération avec retry automatique en cas d'échec
        /// </summary>
        /// <typeparam name="T">Type de retour</typeparam>
        /// <param name="operation">Opération à exécuter</param>
        /// <param name="operationName">Nom de l'opération pour le logging</param>
        /// <returns>Résultat de l'opération</returns>
        public T ExecuteWithRetry<T>(Func<T> operation, string operationName)
        {
            var attempts = 0;
            var retryKey = $"{operationName}_{Thread.CurrentThread.ManagedThreadId}";
            
            while (attempts < MAX_RETRY_ATTEMPTS)
            {
                try
                {
                    EnsureEduKinContextInitialized();
                    var result = operation();
                    
                    // Réinitialiser le compteur de retry en cas de succès
                    _retryCounters.Remove(retryKey);
                    
                    return result;
                }
                catch (Exception ex) when (IsRetryableException(ex))
                {
                    attempts++;
                    _retryCounters[retryKey] = attempts;
                    
                    LogRetryAttempt(operationName, attempts, ex);
                    
                    if (attempts >= MAX_RETRY_ATTEMPTS)
                    {
                        var finalError = new InvalidOperationException(
                            $"Échec de l'opération '{operationName}' après {MAX_RETRY_ATTEMPTS} tentatives. " +
                            $"École: {EduKinContext.TryGetCurrentDenomination() ?? "Non configurée"}. " +
                            $"Mode: {(_connexion.IsOnline ? "En ligne (MySQL)" : "Hors ligne (SQLite)")}. " +
                            $"Dernière erreur: {ex.Message}", ex);
                        
                        LogFinalError(operationName, finalError);
                        throw finalError;
                    }
                    
                    // Attendre avant de réessayer
                    Thread.Sleep(RETRY_DELAY_MS * attempts);
                    
                    // Rafraîchir la connexion si nécessaire
                    _connexion.RefreshConnectionStatus();
                }
                catch (Exception ex)
                {
                    // Erreur non récupérable
                    var nonRetryableError = new InvalidOperationException(
                        $"Erreur non récupérable lors de l'opération '{operationName}' pour l'école " +
                        $"{EduKinContext.TryGetCurrentDenomination() ?? "Non configurée"}: {ex.Message}", ex);
                    
                    LogFinalError(operationName, nonRetryableError);
                    throw nonRetryableError;
                }
            }
            
            throw new InvalidOperationException($"Nombre maximum de tentatives atteint pour l'opération '{operationName}'");
        }

        /// <summary>
        /// Détermine si une exception justifie un retry
        /// </summary>
        /// <param name="ex">Exception à analyser</param>
        /// <returns>True si l'exception est récupérable</returns>
        private bool IsRetryableException(Exception ex)
        {
            // Exceptions liées à la connectivité réseau
            if (ex.Message.Contains("timeout") || 
                ex.Message.Contains("connection") ||
                ex.Message.Contains("network") ||
                ex.Message.Contains("server") ||
                ex.Message.Contains("database is locked"))
            {
                return true;
            }
            
            // Exceptions spécifiques aux bases de données
            if (ex is System.Data.Common.DbException dbEx)
            {
                // Codes d'erreur MySQL récupérables
                if (ex.Message.Contains("2006") || // MySQL server has gone away
                    ex.Message.Contains("2013") || // Lost connection to MySQL server
                    ex.Message.Contains("1205") || // Lock wait timeout
                    ex.Message.Contains("1213"))   // Deadlock found
                {
                    return true;
                }
            }
            
            // Exceptions SQLite récupérables
            if (ex.Message.Contains("SQLITE_BUSY") ||
                ex.Message.Contains("SQLITE_LOCKED") ||
                ex.Message.Contains("database is locked"))
            {
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Log une tentative de retry
        /// </summary>
        private void LogRetryAttempt(string operationName, int attempt, Exception ex)
        {
            var logMessage = $"[RETRY {attempt}/{MAX_RETRY_ATTEMPTS}] Opération '{operationName}' - " +
                           $"École: {EduKinContext.TryGetCurrentDenomination() ?? "Non configurée"} - " +
                           $"Mode: {(_connexion.IsOnline ? "MySQL" : "SQLite")} - " +
                           $"Erreur: {ex.Message}";
            
            Debug.WriteLine(logMessage);
            
            // Log dans un fichier si nécessaire
            LogToFile("retry", logMessage);
        }

        /// <summary>
        /// Log une erreur finale
        /// </summary>
        private void LogFinalError(string operationName, Exception ex)
        {
            var logMessage = $"[ERREUR FINALE] Opération '{operationName}' - " +
                           $"École: {EduKinContext.TryGetCurrentDenomination() ?? "Non configurée"} - " +
                           $"Mode: {(_connexion.IsOnline ? "MySQL" : "SQLite")} - " +
                           $"Erreur: {ex.Message} - " +
                           $"Stack: {ex.StackTrace}";
            
            Debug.WriteLine(logMessage);
            
            // Log dans un fichier
            LogToFile("error", logMessage);
        }

        /// <summary>
        /// Log dans un fichier
        /// </summary>
        private void LogToFile(string logType, string message)
        {
            try
            {
                var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                Directory.CreateDirectory(logDir);
                
                var logFile = Path.Combine(logDir, $"{logType}_{DateTime.Now:yyyy-MM-dd}.log");
                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
                
                File.AppendAllText(logFile, logEntry);
            }
            catch
            {
                // Ignorer les erreurs de logging pour éviter les boucles infinies
            }
        }

        #endregion

        #region Méthodes de Validation et Utilitaires

        /// <summary>
        /// Vérifie si une table supporte l'isolation par fk_ecole
        /// </summary>
        /// <param name="tableName">Nom de la table</param>
        /// <returns>True si la table supporte l'isolation</returns>
        protected bool TableSupportsIsolation(string tableName)
        {
            var tablesWithIsolation = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "t_agents", "t_ecoles", "t_users_infos", "t_caisse", 
                "t_entree", "t_sortie", "t_frais", "t_paiement"
            };

            return tablesWithIsolation.Contains(tableName);
        }

        /// <summary>
        /// Ajoute automatiquement les timestamps created_at et updated_at
        /// </summary>
        /// <param name="dataDict">Dictionnaire de données</param>
        /// <param name="isUpdate">True si c'est une mise à jour</param>
        protected void AddTimestamps(Dictionary<string, object?> dataDict, bool isUpdate)
        {
            var now = DateTime.Now;
            
            if (!isUpdate && !dataDict.ContainsKey("created_at"))
            {
                dataDict["created_at"] = now;
            }
            
            if (!dataDict.ContainsKey("updated_at"))
            {
                dataDict["updated_at"] = now;
            }
        }

        /// <summary>
        /// Valide les données avant insertion/mise à jour
        /// </summary>
        /// <param name="tableName">Nom de la table</param>
        /// <param name="data">Données à valider</param>
        /// <returns>True si les données sont valides</returns>
        public bool ValidateData(string tableName, Dictionary<string, object?> data)
        {
            try
            {
                // Validation générale
                if (data == null || data.Count == 0)
                {
                    throw new ArgumentException("Les données ne peuvent pas être vides");
                }

                // Validations spécifiques par table
                switch (tableName.ToLower())
                {
                    case "t_eleves":
                        return ValidateEleveData(data);
                    case "t_agents":
                        return ValidateAgentData(data);
                    case "t_affectation":
                        return ValidateAffectationData(data);
                    default:
                        return true; // Validation basique réussie
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur de validation pour {tableName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Valide les données d'un élève
        /// </summary>
        private bool ValidateEleveData(Dictionary<string, object?> data)
        {
            var requiredFields = new[] { "nom", "postnom", "prenom", "sexe", "nom_tuteur" };
            
            foreach (var field in requiredFields)
            {
                if (!data.ContainsKey(field) || string.IsNullOrWhiteSpace(data[field]?.ToString()))
                {
                    throw new ArgumentException($"Le champ '{field}' est obligatoire pour un élève");
                }
            }

            // Validation du sexe
            var sexe = data["sexe"]?.ToString();
            if (sexe != "M" && sexe != "F")
            {
                throw new ArgumentException("Le sexe doit être 'M' ou 'F'");
            }

            return true;
        }

        /// <summary>
        /// Valide les données d'un agent
        /// </summary>
        private bool ValidateAgentData(Dictionary<string, object?> data)
        {
            var requiredFields = new[] { "matricule", "nom", "postnom", "prenom", "sexe", "service", "fk_ecole" };
            
            foreach (var field in requiredFields)
            {
                if (!data.ContainsKey(field) || string.IsNullOrWhiteSpace(data[field]?.ToString()))
                {
                    throw new ArgumentException($"Le champ '{field}' est obligatoire pour un agent");
                }
            }

            // Validation de l'email si présent
            if (data.ContainsKey("email") && !string.IsNullOrEmpty(data["email"]?.ToString()))
            {
                var email = data["email"].ToString();
                if (!email.Contains("@") || !email.Contains("."))
                {
                    throw new ArgumentException("Format d'email invalide");
                }
            }

            return true;
        }

        /// <summary>
        /// Valide les données d'une affectation
        /// </summary>
        private bool ValidateAffectationData(Dictionary<string, object?> data)
        {
            var requiredFields = new[] { "matricule", "cod_promo", "annee_scol", "indice_promo" };
            
            foreach (var field in requiredFields)
            {
                if (!data.ContainsKey(field) || string.IsNullOrWhiteSpace(data[field]?.ToString()))
                {
                    throw new ArgumentException($"Le champ '{field}' est obligatoire pour une affectation");
                }
            }

            return true;
        }

        /// <summary>
        /// Obtient des statistiques de performance pour le monitoring
        /// </summary>
        /// <returns>Statistiques de performance</returns>
        protected Dictionary<string, object> GetPerformanceStats()
        {
            return new Dictionary<string, object>
            {
                { "ConnectionMode", _connexion.IsOnline ? "MySQL" : "SQLite" },
                { "EduKinContext", EduKinContext.TryGetCurrentDenomination() ?? "Non configuré" },
                { "RetryCounters", new Dictionary<string, int>(_retryCounters) },
                { "Timestamp", DateTime.Now }
            };
        }

        /// <summary>
        /// Exécute une requête non-SELECT (INSERT, UPDATE, DELETE) avec isolation automatique et gestion d'erreurs avancée
        /// </summary>
        /// <param name="query">Requête SQL</param>
        /// <param name="parameters">Paramètres de la requête</param>
        /// <returns>Nombre de lignes affectées</returns>
        protected int ExecuteNonQueryWithIsolation(string query, object? parameters = null)
        {
            return ExecuteWithRetry(() =>
            {
                var adaptedQuery = SqlCompatibilityAdapter.GetAdaptedQuery(query, _connexion.IsOnline);
                var isolatedQuery = EduKinContext.AddIsolationClause(adaptedQuery);
                var combinedParameters = CombineParameters(parameters, EduKinContext.GetIsolationParameters());
                
                using (var conn = GetSecureConnection())
                {
                    conn.Open();
                    return conn.Execute(isolatedQuery, combinedParameters);
                }
            }, $"ExecuteNonQueryWithIsolation: {query.Substring(0, Math.Min(50, query.Length))}...");
        }

        /// <summary>
        /// Exécute une requête qui retourne une valeur scalaire avec isolation automatique et gestion d'erreurs avancée
        /// </summary>
        /// <typeparam name="T">Type de la valeur de retour</typeparam>
        /// <param name="query">Requête SQL</param>
        /// <param name="parameters">Paramètres de la requête</param>
        /// <returns>Valeur scalaire</returns>
        protected T ExecuteScalarWithIsolation<T>(string query, object? parameters = null)
        {
            return ExecuteWithRetry(() =>
            {
                var adaptedQuery = SqlCompatibilityAdapter.GetAdaptedQuery(query, _connexion.IsOnline);
                var isolatedQuery = EduKinContext.AddIsolationClause(adaptedQuery);
                var combinedParameters = CombineParameters(parameters, EduKinContext.GetIsolationParameters());
                
                using (var conn = GetSecureConnection())
                {
                    conn.Open();
                    return conn.ExecuteScalar<T>(isolatedQuery, combinedParameters);
                }
            }, $"ExecuteScalarWithIsolation: {query.Substring(0, Math.Min(50, query.Length))}...");
        }

        /// <summary>
        /// Nettoie les ressources et compteurs
        /// </summary>
        protected void Cleanup()
        {
            _retryCounters.Clear();
        }

        #endregion
    }
}