using Dapper;
using EduKin.DataSets;
using EduKin.Inits;
using System;
using System.Collections.Generic;
using System.Linq;
using EduKinContext = EduKin.Inits.EduKinContext; // Résoudre le conflit de noms

namespace EduKin.Csharp.Admins
{
    /// <summary>
    /// Gestionnaire des années scolaires avec règles strictes d'isolation
    /// </summary>
    public class SchoolYearManager
    {
        private readonly Connexion _connexion;

        public SchoolYearManager()
        {
            _connexion = Connexion.Instance;
        }

        #region Création et gestion des années scolaires

        /// <summary>
        /// Crée une nouvelle année scolaire pour une école
        /// RÈGLE: Une seule année active par école
        /// </summary>
        public bool CreateSchoolYear(string idEcole, string codeAnnee, DateTime dateDebut, DateTime dateFin, bool setAsActive = true)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Vérifier qu'il n'existe pas déjà une année avec ce code pour cette école
                            var existingQuery = @"
                                SELECT COUNT(*) 
                                FROM t_annee_scolaire 
                                WHERE id_ecole = @IdEcole
                                  AND code_annee = @CodeAnnee";

                            var existingCount = conn.ExecuteScalar<int>(existingQuery, new { IdEcole = idEcole, CodeAnnee = codeAnnee }, transaction);
                            
                            if (existingCount > 0)
                            {
                                throw new InvalidOperationException($"L'année scolaire {codeAnnee} existe déjà pour cette école");
                            }

                            // 2. Si on veut définir cette année comme active, désactiver les autres
                            if (setAsActive)
                            {
                                var deactivateQuery = @"
                                    UPDATE t_annee_scolaire 
                                    SET est_active = 0 
                                    WHERE id_ecole = @IdEcole
                                      AND est_active = 1";

                                conn.Execute(deactivateQuery, new { IdEcole = idEcole }, transaction);
                            }

                            // 3. Créer la nouvelle année scolaire
                            var insertQuery = @"
                                INSERT INTO t_annee_scolaire (id_ecole, code_annee, date_debut, date_fin, est_active, est_cloturee, date_creation)
                                VALUES (
                                    @IdEcole,
                                    @CodeAnnee, 
                                    @DateDebut, 
                                    @DateFin, 
                                    @EstActive, 
                                    0, 
                                    NOW()
                                )";

                            conn.Execute(insertQuery, new 
                            { 
                                IdEcole = idEcole,
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
        /// Récupère l'année scolaire active pour une école
        /// </summary>
        public dynamic? GetActiveSchoolYear(string idEcole)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"
                        SELECT a.*, e.id_ecole as ecole_id_string, e.denomination
                        FROM t_annee_scolaire a
                        INNER JOIN t_ecoles e ON a.id_ecole = e.id_ecole
                        WHERE e.id_ecole = @IdEcole 
                          AND a.est_active = 1
                        LIMIT 1";

                    return conn.QueryFirstOrDefault(query, new { IdEcole = idEcole });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de l'année active: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Récupère toutes les années scolaires d'une école
        /// </summary>
        public IEnumerable<dynamic> GetAllSchoolYears(string idEcole)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"
                        SELECT a.*, e.id_ecole as ecole_id_string, e.denomination
                        FROM t_annee_scolaire a
                        INNER JOIN t_ecoles e ON a.id_ecole = e.id_ecole
                        WHERE e.id_ecole = @IdEcole
                        ORDER BY a.date_debut DESC";

                    return conn.Query(query, new { IdEcole = idEcole });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des années scolaires: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Active une année scolaire (désactive automatiquement les autres)
        /// RÈGLE: Une seule année active par école
        /// </summary>
        public bool ActivateSchoolYear(int idAnnee, string idEcole)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Vérifier que l'année n'est pas clôturée
                            var checkQuery = @"
                                SELECT est_cloturee 
                                FROM t_annee_scolaire 
                                WHERE id_annee = @IdAnnee";

                            var estCloturee = conn.ExecuteScalar<bool>(checkQuery, new { IdAnnee = idAnnee }, transaction);
                            
                            if (estCloturee)
                            {
                                throw new InvalidOperationException("Impossible d'activer une année scolaire clôturée");
                            }

                            // 2. Désactiver toutes les autres années de cette école
                            var deactivateQuery = @"
                                UPDATE t_annee_scolaire 
                                SET est_active = 0 
                                WHERE id_ecole = @IdEcole
                                  AND est_active = 1";

                            conn.Execute(deactivateQuery, new { IdEcole = idEcole }, transaction);

                            // 3. Activer l'année demandée
                            var activateQuery = @"
                                UPDATE t_annee_scolaire 
                                SET est_active = 1 
                                WHERE id_annee = @IdAnnee";

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

        /// <summary>
        /// Clôture une année scolaire (IRRÉVERSIBLE)
        /// RÈGLE: Une année clôturée est intouchable
        /// </summary>
        public bool CloseSchoolYear(int idAnnee, string userId)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Vérifier que l'année n'est pas déjà clôturée
                            var checkQuery = @"
                                SELECT est_cloturee, code_annee
                                FROM t_annee_scolaire 
                                WHERE id_annee = @IdAnnee";

                            var anneeInfo = conn.QueryFirstOrDefault(checkQuery, new { IdAnnee = idAnnee }, transaction);
                            
                            if (anneeInfo == null)
                            {
                                throw new InvalidOperationException("Année scolaire introuvable");
                            }

                            if (anneeInfo.est_cloturee)
                            {
                                throw new InvalidOperationException($"L'année scolaire {anneeInfo.code_annee} est déjà clôturée");
                            }

                            // 2. Clôturer l'année (IRRÉVERSIBLE)
                            var closeQuery = @"
                                UPDATE t_annee_scolaire 
                                SET est_cloturee = 1, 
                                    est_active = 0
                                WHERE id_annee = @IdAnnee";

                            conn.Execute(closeQuery, new { IdAnnee = idAnnee }, transaction);

                            // 3. Log d'audit (optionnel - à implémenter selon vos besoins)
                            System.Diagnostics.Debug.WriteLine($"[AUDIT] Année scolaire {anneeInfo.code_annee} clôturée par utilisateur {userId} le {DateTime.Now}");

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
                throw new Exception($"Erreur lors de la clôture de l'année scolaire: {ex.Message}", ex);
            }
        }

        #endregion

        #region Méthodes utilitaires

        /// <summary>
        /// Génère automatiquement le code d'année scolaire basé sur l'année courante
        /// </summary>
        public static string GenerateSchoolYearCode(int startYear)
        {
            return $"{startYear}-{startYear + 1}";
        }

        /// <summary>
        /// Génère automatiquement le code d'année scolaire pour l'année courante
        /// </summary>
        public static string GenerateCurrentSchoolYearCode()
        {
            var currentYear = DateTime.Now.Year;
            // Si on est après juillet, on considère qu'on est dans la nouvelle année scolaire
            if (DateTime.Now.Month >= 7)
            {
                return GenerateSchoolYearCode(currentYear);
            }
            else
            {
                return GenerateSchoolYearCode(currentYear - 1);
            }
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
        /// Vérifie si une école a au moins une année scolaire
        /// </summary>
        public bool HasSchoolYears(string idEcole)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"
                        SELECT COUNT(*) 
                        FROM t_annee_scolaire 
                        WHERE id_ecole = @IdEcole";

                    var count = conn.ExecuteScalar<int>(query, new { IdEcole = idEcole });
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la vérification des années scolaires: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Initialise le contexte d'isolation avec l'année active
        /// </summary>
        public bool InitializeContextWithActiveYear(string idEcole, string userId, string username)
        {
            try
            {
                var activeYear = GetActiveSchoolYear(idEcole);
                
                if (activeYear == null)
                {
                    return false;
                }

                EduKinContext.InitializeComplete(
                    idEcole: idEcole,
                    denomination: activeYear.denomination ?? "École",
                    idAnnee: activeYear.id_annee,
                    codeAnnee: activeYear.code_annee,
                    userId: userId,
                    userName: username,
                    userRole: "Utilisateur", // Rôle par défaut
                    dateDebut: activeYear.date_debut,
                    dateFin: activeYear.date_fin,
                    estActive: activeYear.est_active,
                    estCloturee: activeYear.est_cloturee
                );

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'initialisation du contexte: {ex.Message}", ex);
            }
        }

        #endregion
    }
}