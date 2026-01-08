using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using EduKin.DataSets;

namespace EduKin.Inits
{
    /// <summary>
    /// Contexte global unifié : École + Année Scolaire + Utilisateur + Permissions + Isolation SQL
    /// Fusion complète de ApplicationContext, SchoolContext et UserContext
    /// Gère l'isolation complète des données, les opérations sur les années scolaires et la sécurité
    /// </summary>
    public static class EduKinContext
    {
        #region Variables privées

        // Contexte École
        private static string? _currentIdEcole;
        private static string? _currentDenomination;
        
        // Contexte Année Scolaire
        private static int? _currentIdAnnee;
        private static string? _currentCodeAnnee;
        private static DateTime? _dateDebutAnnee;
        private static DateTime? _dateFinAnnee;
        private static bool _estActive;
        private static bool _estCloturee;
        
        // Contexte Utilisateur
        private static string? _currentUserId;
        private static string? _currentUserName;
        private static string? _currentUserRole;
        private static string? _currentUserIndex;
        
        private static bool _isConfigured;
        private static bool _isAuthenticated;

        #endregion

        #region Propriétés publiques - École

        public static string CurrentIdEcole
        {
            get
            {
                if (!_isConfigured)
                    throw new InvalidOperationException("Le contexte de l'école n'est pas initialisé.");
                return _currentIdEcole!;
            }
        }

        public static string CurrentDenomination
        {
            get
            {
                if (!_isConfigured)
                    throw new InvalidOperationException("Le contexte de l'école n'est pas initialisé.");
                return _currentDenomination!;
            }
        }

        #endregion

        #region Propriétés publiques - Année Scolaire

        public static int CurrentIdAnnee
        {
            get
            {
                if (!_isConfigured || _currentIdAnnee == null)
                    throw new InvalidOperationException("Le contexte de l'année scolaire n'est pas initialisé.");
                return _currentIdAnnee.Value;
            }
        }

        public static string CurrentCodeAnnee
        {
            get
            {
                if (!_isConfigured || _currentCodeAnnee == null)
                    throw new InvalidOperationException("Le contexte de l'année scolaire n'est pas initialisé.");
                return _currentCodeAnnee;
            }
        }

        public static DateTime DateDebutAnnee
        {
            get
            {
                if (!_isConfigured || _dateDebutAnnee == null)
                    throw new InvalidOperationException("Le contexte de l'année scolaire n'est pas initialisé.");
                return _dateDebutAnnee.Value;
            }
        }

        public static DateTime DateFinAnnee
        {
            get
            {
                if (!_isConfigured || _dateFinAnnee == null)
                    throw new InvalidOperationException("Le contexte de l'année scolaire n'est pas initialisé.");
                return _dateFinAnnee.Value;
            }
        }

        public static bool EstActive => _estActive;
        public static bool EstCloturee => _estCloturee;

        #endregion

        #region Propriétés publiques - Utilisateur

        public static string CurrentUserId
        {
            get
            {
                if (!_isAuthenticated)
                    throw new InvalidOperationException("Aucun utilisateur n'est connecté.");
                return _currentUserId!;
            }
        }

        public static string CurrentUserName
        {
            get
            {
                if (!_isAuthenticated)
                    throw new InvalidOperationException("Aucun utilisateur n'est connecté.");
                return _currentUserName!;
            }
        }

        public static string CurrentUserRole
        {
            get
            {
                if (!_isAuthenticated)
                    throw new InvalidOperationException("Aucun utilisateur n'est connecté.");
                return _currentUserRole!;
            }
        }

        public static string CurrentUserIndex
        {
            get
            {
                if (!_isAuthenticated)
                    throw new InvalidOperationException("Aucun utilisateur n'est connecté.");
                return _currentUserIndex!;
            }
        }

        #endregion

        #region Propriétés d'état

        public static bool IsConfigured => _isConfigured;
        public static bool IsAuthenticated => _isAuthenticated;
        public static bool HasActiveYear => _isConfigured && _currentIdAnnee != null;

        #endregion

        #region Méthodes d'initialisation

        /// <summary>
        /// Initialise le contexte avec les informations de l'école seulement
        /// </summary>
        public static void InitializeSchool(string idEcole, string denomination)
        {
            if (string.IsNullOrWhiteSpace(idEcole))
                throw new ArgumentException("L'ID de l'école ne peut pas être vide.", nameof(idEcole));
            if (string.IsNullOrWhiteSpace(denomination))
                throw new ArgumentException("La dénomination ne peut pas être vide.", nameof(denomination));

            _currentIdEcole = idEcole;
            _currentDenomination = denomination;
            _isConfigured = true;

            System.Diagnostics.Debug.WriteLine($"[ApplicationContext] École initialisée: {denomination} (ID: {idEcole})");
        }

        /// <summary>
        /// Initialise le contexte utilisateur après une connexion réussie
        /// </summary>
        public static void InitializeUser(string userId, string userName, string userRole, string? userIndex = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("L'ID utilisateur ne peut pas être vide.", nameof(userId));
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Le nom d'utilisateur ne peut pas être vide.", nameof(userName));
            if (string.IsNullOrWhiteSpace(userRole))
                throw new ArgumentException("Le rôle utilisateur ne peut pas être vide.", nameof(userRole));

            _currentUserId = userId;
            _currentUserName = userName;
            _currentUserRole = userRole;
            _currentUserIndex = userIndex ?? "001";
            _isAuthenticated = true;

            System.Diagnostics.Debug.WriteLine($"[ApplicationContext] Utilisateur connecté: {userName} (Rôle: {userRole})");
        }

        /// <summary>
        /// Initialise le contexte complet (École + Année + Utilisateur)
        /// </summary>
        public static void InitializeComplete(string idEcole, string denomination, int idAnnee, 
            string codeAnnee, string userId, string userName, string userRole, DateTime dateDebut, 
            DateTime dateFin, bool estActive, bool estCloturee, string? userIndex = null)
        {
            // Validation stricte
            if (string.IsNullOrWhiteSpace(idEcole))
                throw new ArgumentException("ID école requis", nameof(idEcole));
            if (string.IsNullOrWhiteSpace(denomination))
                throw new ArgumentException("Dénomination école requise", nameof(denomination));
            if (idAnnee <= 0)
                throw new ArgumentException("ID année scolaire invalide", nameof(idAnnee));
            if (string.IsNullOrWhiteSpace(codeAnnee))
                throw new ArgumentException("Code année scolaire requis", nameof(codeAnnee));
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("ID utilisateur requis", nameof(userId));
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Nom d'utilisateur requis", nameof(userName));
            if (string.IsNullOrWhiteSpace(userRole))
                throw new ArgumentException("Rôle utilisateur requis", nameof(userRole));
            if (dateDebut >= dateFin)
                throw new ArgumentException("Date de début doit être antérieure à la date de fin");

            // Initialisation complète
            _currentIdEcole = idEcole;
            _currentDenomination = denomination;
            _currentIdAnnee = idAnnee;
            _currentCodeAnnee = codeAnnee;
            _currentUserId = userId;
            _currentUserName = userName;
            _currentUserRole = userRole;
            _currentUserIndex = userIndex ?? "001";
            _dateDebutAnnee = dateDebut;
            _dateFinAnnee = dateFin;
            _estActive = estActive;
            _estCloturee = estCloturee;
            _isConfigured = true;
            _isAuthenticated = true;

            System.Diagnostics.Debug.WriteLine($"[ApplicationContext] Contexte complet initialisé: {denomination} - {codeAnnee} - {userName} ({userRole})");
        }

        /// <summary>
        /// Initialise le contexte avec l'année active de l'école courante
        /// </summary>
        public static bool InitializeWithActiveYear()
        {
            if (!_isConfigured)
                throw new InvalidOperationException("Le contexte de l'école n'est pas initialisé.");
            if (!_isAuthenticated)
                throw new InvalidOperationException("Aucun utilisateur n'est connecté.");

            try
            {
                var activeYear = GetActiveSchoolYear();
                if (activeYear == null) return false;

                _currentIdAnnee = activeYear.id_annee;
                _currentCodeAnnee = activeYear.code_annee;
                _dateDebutAnnee = activeYear.date_debut;
                _dateFinAnnee = activeYear.date_fin;
                _estActive = activeYear.est_active;
                _estCloturee = activeYear.est_cloturee;

                System.Diagnostics.Debug.WriteLine($"[ApplicationContext] Année active chargée: {_currentCodeAnnee}");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'initialisation du contexte: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Réinitialise complètement le contexte
        /// </summary>
        public static void Clear()
        {
            _currentIdEcole = null;
            _currentDenomination = null;
            _currentIdAnnee = null;
            _currentCodeAnnee = null;
            _currentUserId = null;
            _currentUserName = null;
            _currentUserRole = null;
            _currentUserIndex = null;
            _dateDebutAnnee = null;
            _dateFinAnnee = null;
            _estActive = false;
            _estCloturee = false;
            _isConfigured = false;
            _isAuthenticated = false;

            System.Diagnostics.Debug.WriteLine("[ApplicationContext] Contexte réinitialisé");
        }

        /// <summary>
        /// Déconnecte l'utilisateur mais conserve le contexte école
        /// </summary>
        public static void Logout()
        {
            _currentUserId = null;
            _currentUserName = null;
            _currentUserRole = null;
            _currentUserIndex = null;
            _currentIdAnnee = null;
            _currentCodeAnnee = null;
            _dateDebutAnnee = null;
            _dateFinAnnee = null;
            _estActive = false;
            _estCloturee = false;
            _isAuthenticated = false;

            System.Diagnostics.Debug.WriteLine("[ApplicationContext] Utilisateur déconnecté");
        }

        /// <summary>
        /// Méthode alternative pour initialiser seulement l'école (compatibilité SchoolContext)
        /// </summary>
        public static void Initialize(string idEcole, string denomination)
        {
            InitializeSchool(idEcole, denomination);
        }

        /// <summary>
        /// Initialise le contexte avec l'année active de l'école courante (surcharge avec utilisateur)
        /// </summary>
        public static bool InitializeWithActiveYear(string userId, string username)
        {
            if (!_isConfigured)
                throw new InvalidOperationException("Le contexte de l'école n'est pas initialisé.");

            try
            {
                var activeYear = GetActiveSchoolYear();
                if (activeYear == null) return false;

                _currentIdAnnee = activeYear.id_annee;
                _currentCodeAnnee = activeYear.code_annee;
                _currentUserId = userId;
                _currentUserName = username;
                _dateDebutAnnee = activeYear.date_debut;
                _dateFinAnnee = activeYear.date_fin;
                _estActive = activeYear.est_active;
                _estCloturee = activeYear.est_cloturee;
                _isAuthenticated = true;

                System.Diagnostics.Debug.WriteLine($"[ApplicationContext] Année active chargée avec utilisateur: {_currentCodeAnnee} - {username}");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'initialisation du contexte: {ex.Message}", ex);
            }
        }

        #endregion

        #region Gestion des années scolaires (fusionné de SchoolContext)

        /// <summary>
        /// Crée une nouvelle année scolaire pour l'école courante
        /// </summary>
        public static bool CreateSchoolYear(string codeAnnee, DateTime dateDebut, DateTime dateFin, bool setAsActive = true)
        {
            if (!_isConfigured)
                throw new InvalidOperationException("Le contexte de l'école n'est pas initialisé.");

            try
            {
                var connexion = Connexion.Instance;
                using (var conn = connexion.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Vérifier qu'il n'existe pas déjà une année avec ce code
                            var existingQuery = @"
                                SELECT COUNT(*) 
                                FROM t_annee_scolaire 
                                WHERE id_ecole = (SELECT CAST(SUBSTRING(id_ecole, 4) AS UNSIGNED) FROM t_ecoles WHERE id_ecole = @IdEcole)
                                  AND code_annee = @CodeAnnee";

                            var existingCount = conn.ExecuteScalar<int>(existingQuery, new { IdEcole = _currentIdEcole, CodeAnnee = codeAnnee }, transaction);
                            
                            if (existingCount > 0)
                                throw new InvalidOperationException($"L'année scolaire {codeAnnee} existe déjà pour cette école");

                            // Si on veut définir cette année comme active, désactiver les autres
                            if (setAsActive)
                            {
                                var deactivateQuery = @"
                                    UPDATE t_annee_scolaire 
                                    SET est_active = 0 
                                    WHERE id_ecole = (SELECT CAST(SUBSTRING(id_ecole, 4) AS UNSIGNED) FROM t_ecoles WHERE id_ecole = @IdEcole)
                                      AND est_active = 1";

                                conn.Execute(deactivateQuery, new { IdEcole = _currentIdEcole }, transaction);
                            }

                            // Créer la nouvelle année scolaire
                            var insertQuery = @"
                                INSERT INTO t_annee_scolaire (id_ecole, code_annee, date_debut, date_fin, est_active, est_cloturee, date_creation)
                                VALUES (
                                    (SELECT CAST(SUBSTRING(id_ecole, 4) AS UNSIGNED) FROM t_ecoles WHERE id_ecole = @IdEcole),
                                    @CodeAnnee, @DateDebut, @DateFin, @EstActive, 0, NOW()
                                )";

                            conn.Execute(insertQuery, new 
                            { 
                                IdEcole = _currentIdEcole,
                                CodeAnnee = codeAnnee,
                                DateDebut = dateDebut,
                                DateFin = dateFin,
                                EstActive = setAsActive
                            }, transaction);

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création de l'année scolaire: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Récupère l'année scolaire active pour l'école courante
        /// </summary>
        public static dynamic? GetActiveSchoolYear()
        {
            if (!_isConfigured)
                throw new InvalidOperationException("Le contexte de l'école n'est pas initialisé.");

            try
            {
                var connexion = Connexion.Instance;
                using (var conn = connexion.GetConnection())
                {
                    var query = @"
                        SELECT a.*, e.id_ecole as ecole_id_string, e.denomination
                        FROM t_annee_scolaire a
                        INNER JOIN t_ecoles e ON a.id_ecole = CAST(SUBSTRING(e.id_ecole, 4) AS UNSIGNED)
                        WHERE e.id_ecole = @IdEcole AND a.est_active = 1
                        LIMIT 1";

                    return conn.QueryFirstOrDefault(query, new { IdEcole = _currentIdEcole });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de l'année active: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Récupère toutes les années scolaires de l'école courante
        /// </summary>
        public static IEnumerable<dynamic> GetAllSchoolYears()
        {
            if (!_isConfigured)
                throw new InvalidOperationException("Le contexte de l'école n'est pas initialisé.");

            try
            {
                var connexion = Connexion.Instance;
                using (var conn = connexion.GetConnection())
                {
                    var query = @"
                        SELECT a.*, e.id_ecole as ecole_id_string, e.denomination
                        FROM t_annee_scolaire a
                        INNER JOIN t_ecoles e ON a.id_ecole = CAST(SUBSTRING(e.id_ecole, 4) AS UNSIGNED)
                        WHERE e.id_ecole = @IdEcole
                        ORDER BY a.date_debut DESC";

                    return conn.Query(query, new { IdEcole = _currentIdEcole });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des années scolaires: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Active une année scolaire (désactive automatiquement les autres)
        /// </summary>
        public static bool ActivateSchoolYear(int idAnnee)
        {
            if (!_isConfigured)
                throw new InvalidOperationException("Le contexte de l'école n'est pas initialisé.");

            try
            {
                var connexion = Connexion.Instance;
                using (var conn = connexion.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Vérifier que l'année n'est pas clôturée
                            var checkQuery = "SELECT est_cloturee FROM t_annee_scolaire WHERE id_annee = @IdAnnee";
                            var estCloturee = conn.ExecuteScalar<bool>(checkQuery, new { IdAnnee = idAnnee }, transaction);
                            
                            if (estCloturee)
                                throw new InvalidOperationException("Impossible d'activer une année scolaire clôturée");

                            // Désactiver toutes les autres années de cette école
                            var deactivateQuery = @"
                                UPDATE t_annee_scolaire 
                                SET est_active = 0 
                                WHERE id_ecole = (SELECT CAST(SUBSTRING(id_ecole, 4) AS UNSIGNED) FROM t_ecoles WHERE id_ecole = @IdEcole)
                                  AND est_active = 1";

                            conn.Execute(deactivateQuery, new { IdEcole = _currentIdEcole }, transaction);

                            // Activer l'année demandée
                            var activateQuery = "UPDATE t_annee_scolaire SET est_active = 1 WHERE id_annee = @IdAnnee";
                            conn.Execute(activateQuery, new { IdAnnee = idAnnee }, transaction);

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'activation de l'année scolaire: {ex.Message}", ex);
            }
        }

        #endregion

        #region Méthodes utilitaires (fusionné de SchoolContext)

        /// <summary>
        /// Génère automatiquement le code d'année scolaire
        /// </summary>
        public static string GenerateSchoolYearCode(int startYear)
        {
            return $"{startYear}-{startYear + 1}";
        }

        /// <summary>
        /// Génère le code d'année scolaire pour l'année courante
        /// </summary>
        public static string GenerateCurrentSchoolYearCode()
        {
            var currentYear = DateTime.Now.Year;
            // Si on est après juillet, on considère qu'on est dans la nouvelle année scolaire
            if (DateTime.Now.Month >= 7)
                return GenerateSchoolYearCode(currentYear);
            else
                return GenerateSchoolYearCode(currentYear - 1);
        }

        /// <summary>
        /// Calcule les dates de début et fin d'année scolaire
        /// </summary>
        public static (DateTime debut, DateTime fin) CalculateSchoolYearDates(int startYear)
        {
            var debut = new DateTime(startYear, 9, 1); // 1er septembre
            var fin = new DateTime(startYear + 1, 7, 31); // 31 juillet
            return (debut, fin);
        }

        /// <summary>
        /// Vérifie si l'école courante a au moins une année scolaire
        /// </summary>
        public static bool HasSchoolYears()
        {
            if (!_isConfigured)
                throw new InvalidOperationException("Le contexte de l'école n'est pas initialisé.");

            try
            {
                var connexion = Connexion.Instance;
                using (var conn = connexion.GetConnection())
                {
                    var query = @"
                        SELECT COUNT(*) 
                        FROM t_annee_scolaire a
                        INNER JOIN t_ecoles e ON a.id_ecole = CAST(SUBSTRING(e.id_ecole, 4) AS UNSIGNED)
                        WHERE e.id_ecole = @IdEcole";

                    var count = conn.ExecuteScalar<int>(query, new { IdEcole = _currentIdEcole });
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la vérification des années scolaires: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Vérifie si l'utilisateur peut modifier des données (année non clôturée)
        /// </summary>
        public static void ValidateCanModify()
        {
            if (!_isConfigured)
                throw new InvalidOperationException("Contexte non configuré");
            if (_currentIdAnnee == null)
                throw new InvalidOperationException("Contexte année scolaire non configuré");
            if (_estCloturee)
                throw new InvalidOperationException($"L'année scolaire {_currentCodeAnnee} est clôturée et ne peut pas être modifiée");
        }

        /// <summary>
        /// Retourne un résumé du contexte actuel
        /// </summary>
        public static string GetContextSummary()
        {
            if (!_isConfigured)
                return "Contexte non configuré";

            var summary = $"École: {_currentDenomination} (ID: {_currentIdEcole})";
            
            if (_currentIdAnnee != null)
            {
                summary += $" | Année: {_currentCodeAnnee} | Utilisateur: {_currentUserName} | " +
                          $"Active: {(_estActive ? "Oui" : "Non")} | Clôturée: {(_estCloturee ? "Oui" : "Non")}";
            }
            
            return summary;
        }

        #endregion

        #region Méthodes d'isolation SQL (fusionné de SchoolContext)

        /// <summary>
        /// Ajoute automatiquement la clause d'isolation par école à une requête SQL
        /// </summary>
        public static string AddIsolationClause(string query, string? tableAlias = null)
        {
            if (!_isConfigured)
                throw new InvalidOperationException("Le contexte de l'école n'est pas initialisé.");

            var normalizedQuery = System.Text.RegularExpressions.Regex.Replace(query.Trim(), @"\s+", " ");
            var lowerQuery = normalizedQuery.ToLower();
            string tablePrefix = string.IsNullOrEmpty(tableAlias) ? "" : $"{tableAlias}.";

            if (lowerQuery.StartsWith("select"))
                return AddIsolationToSelect(normalizedQuery, tablePrefix);
            else if (lowerQuery.StartsWith("update"))
                return AddIsolationToUpdate(normalizedQuery, tablePrefix);
            else if (lowerQuery.StartsWith("delete"))
                return AddIsolationToDelete(normalizedQuery, tablePrefix);

            return query;
        }

        private static string AddIsolationToSelect(string query, string tablePrefix)
        {
            var lowerQuery = query.ToLower();
            
            if (lowerQuery.Contains(" where "))
            {
                var whereIndex = lowerQuery.IndexOf(" where ");
                var beforeWhere = query.Substring(0, whereIndex + 7);
                var afterWhere = query.Substring(whereIndex + 7);
                return $"{beforeWhere}({afterWhere}) AND {tablePrefix}id_ecole = @IdEcole";
            }
            else
            {
                var insertPosition = FindInsertPositionForWhere(query);
                var beforeInsert = query.Substring(0, insertPosition);
                var afterInsert = query.Substring(insertPosition);
                return $"{beforeInsert} WHERE {tablePrefix}id_ecole = @IdEcole{afterInsert}";
            }
        }

        private static string AddIsolationToUpdate(string query, string tablePrefix)
        {
            var lowerQuery = query.ToLower();
            
            if (lowerQuery.Contains(" where "))
            {
                var whereIndex = lowerQuery.IndexOf(" where ");
                var beforeWhere = query.Substring(0, whereIndex + 7);
                var afterWhere = query.Substring(whereIndex + 7);
                return $"{beforeWhere}({afterWhere}) AND {tablePrefix}id_ecole = @IdEcole";
            }
            else
            {
                return $"{query} WHERE {tablePrefix}id_ecole = @IdEcole";
            }
        }

        private static string AddIsolationToDelete(string query, string tablePrefix)
        {
            var lowerQuery = query.ToLower();
            
            if (lowerQuery.Contains(" where "))
            {
                var whereIndex = lowerQuery.IndexOf(" where ");
                var beforeWhere = query.Substring(0, whereIndex + 7);
                var afterWhere = query.Substring(whereIndex + 7);
                return $"{beforeWhere}({afterWhere}) AND {tablePrefix}id_ecole = @IdEcole";
            }
            else
            {
                return $"{query} WHERE {tablePrefix}id_ecole = @IdEcole";
            }
        }

        private static int FindInsertPositionForWhere(string query)
        {
            var lowerQuery = query.ToLower();
            var keywords = new[] { " order by ", " group by ", " having ", " limit ", " offset " };
            
            foreach (var keyword in keywords)
            {
                var index = lowerQuery.IndexOf(keyword);
                if (index != -1) return index;
            }
            
            return query.Length;
        }

        /// <summary>
        /// Obtient les paramètres d'isolation pour les requêtes
        /// </summary>
        public static object GetIsolationParameters()
        {
            if (!_isConfigured)
                throw new InvalidOperationException("Le contexte de l'école n'est pas initialisé.");

            return new { IdEcole = _currentIdEcole };
        }

        /// <summary>
        /// Obtient un message d'erreur détaillé pour l'isolation
        /// </summary>
        public static string GetIsolationErrorMessage()
        {
            if (!_isConfigured)
                return "ERREUR D'ISOLATION: Le contexte de l'école n'est pas initialisé.";

            return $"Contexte école actuel: {_currentDenomination} (ID: {_currentIdEcole})";
        }

        #endregion

        #region Méthodes de sécurité et permissions (fusionné de UserContext)

        /// <summary>
        /// Vérifie si l'utilisateur a un rôle spécifique
        /// </summary>
        /// <param name="role">Rôle à vérifier</param>
        /// <returns>True si l'utilisateur a ce rôle</returns>
        public static bool HasRole(string role)
        {
            if (!_isAuthenticated)
                return false;

            return string.Equals(_currentUserRole, role, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Vérifie si l'utilisateur a l'un des rôles spécifiés
        /// </summary>
        /// <param name="roles">Rôles à vérifier</param>
        /// <returns>True si l'utilisateur a au moins un de ces rôles</returns>
        public static bool HasAnyRole(params string[] roles)
        {
            if (!_isAuthenticated || roles == null)
                return false;

            foreach (var role in roles)
            {
                if (HasRole(role))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Vérifie si l'utilisateur est administrateur
        /// </summary>
        /// <returns>True si l'utilisateur est administrateur</returns>
        public static bool IsAdmin()
        {
            return HasAnyRole("Super Administrateur", "Administrateur", "Directeur");
        }

        /// <summary>
        /// Obtient les informations de l'utilisateur de manière sécurisée
        /// </summary>
        /// <returns>Informations utilisateur ou null si non connecté</returns>
        public static (string? UserId, string? UserName, string? UserRole, string? UserIndex)? TryGetCurrentUser()
        {
            if (!_isAuthenticated)
                return null;

            return (_currentUserId, _currentUserName, _currentUserRole, _currentUserIndex);
        }

        /// <summary>
        /// Obtient les paramètres d'audit pour les requêtes
        /// </summary>
        /// <returns>Objet contenant les paramètres d'audit</returns>
        public static object GetAuditParameters()
        {
            if (!_isAuthenticated)
            {
                throw new InvalidOperationException("Aucun utilisateur n'est connecté pour l'audit.");
            }

            return new 
            { 
                UserId = _currentUserId,
                UserName = _currentUserName,
                UserRole = _currentUserRole,
                Timestamp = DateTime.Now
            };
        }

        /// <summary>
        /// Obtient un message d'erreur détaillé pour l'authentification
        /// </summary>
        /// <returns>Message d'erreur avec contexte</returns>
        public static string GetAuthenticationErrorMessage()
        {
            if (!_isAuthenticated)
            {
                return "ERREUR D'AUTHENTIFICATION: Aucun utilisateur n'est connecté. " +
                       "Veuillez vous connecter avant d'accéder aux données.";
            }

            return $"Utilisateur connecté: {_currentUserName} (ID: {_currentUserId}, Rôle: {_currentUserRole})";
        }

        /// <summary>
        /// Valide les permissions pour une opération
        /// </summary>
        /// <param name="requiredRoles">Rôles requis pour l'opération</param>
        /// <param name="operationName">Nom de l'opération (pour les logs)</param>
        /// <returns>True si l'utilisateur a les permissions</returns>
        public static bool ValidatePermissions(string[] requiredRoles, string operationName = "")
        {
            if (!_isAuthenticated)
            {
                throw new UnauthorizedAccessException($"Accès refusé à l'opération '{operationName}': Utilisateur non connecté.");
            }

            if (!HasAnyRole(requiredRoles))
            {
                throw new UnauthorizedAccessException($"Accès refusé à l'opération '{operationName}': Rôle insuffisant. Rôles requis: {string.Join(", ", requiredRoles)}. Rôle actuel: {_currentUserRole}");
            }

            return true;
        }

        /// <summary>
        /// Méthodes de sécurité pour accès sécurisé aux propriétés
        /// </summary>
        public static string? TryGetCurrentIdEcole() => _isConfigured ? _currentIdEcole : null;
        public static string? TryGetCurrentDenomination() => _isConfigured ? _currentDenomination : null;
        public static string? TryGetCurrentCodeAnnee() => _isConfigured ? _currentCodeAnnee : null;
        public static string? TryGetCurrentUserId() => _isAuthenticated ? _currentUserId : null;
        public static string? TryGetCurrentUserName() => _isAuthenticated ? _currentUserName : null;
        public static string? TryGetCurrentUserRole() => _isAuthenticated ? _currentUserRole : null;
        public static string? TryGetCurrentUserIndex() => _isAuthenticated ? _currentUserIndex : null;

        #endregion

        #region Propriétés de compatibilité (alias pour les anciennes classes)

        /// <summary>
        /// Alias pour CurrentUserName (compatibilité UserContext)
        /// </summary>
        public static string CurrentUsername => CurrentUserName;

        #endregion
    }
}