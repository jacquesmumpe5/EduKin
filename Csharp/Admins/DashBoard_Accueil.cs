using Dapper;
using EduKin.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EduKin.Csharp.Admins
{
    /// <summary>
    /// Business logic class for dashboard data operations
    /// Provides methods to retrieve statistics and information for the home dashboard
    /// </summary>
    public class DashBoard_Accueil
    {
        private readonly Connexion _connexion;

        public DashBoard_Accueil()
        {
            _connexion = Connexion.Instance;
        }

        #region Dashboard Statistics Methods

        /// <summary>
        /// Gets dashboard statistics for DataGridViewAccueil1
        /// Returns student count statistics grouped by Section, Option, and Promotion
        /// </summary>
        /// <param name="anneeScol">School year filter (optional)</param>
        /// <returns>Dynamic objects with Section, Option, Promotion, NbreEleveGarcons, NbreElevesFilles, TotalNbreEleves</returns>
        public IEnumerable<dynamic> GetDashboardStatistics(string? anneeScol = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"
                        SELECT 
                            COALESCE(s.description, 'Non défini') as Section,
                            COALESCE(o.description, 'Non défini') as Option,
                            COALESCE(p.description, 'Non défini') as Promotion,
                            SUM(CASE WHEN e.sexe = 'M' THEN 1 ELSE 0 END) as NbreEleveGarcons,
                            SUM(CASE WHEN e.sexe = 'F' THEN 1 ELSE 0 END) as NbreElevesFilles,
                            COUNT(e.matricule) as TotalNbreEleves
                        FROM t_sections s
                        LEFT JOIN t_options o ON s.cod_sect = o.cod_sect
                        LEFT JOIN t_promotions p ON o.cod_opt = p.cod_opt
                        LEFT JOIN t_affectation a ON p.cod_promo = a.cod_promo";

                    // Add school year filter if provided
                    if (!string.IsNullOrEmpty(anneeScol))
                    {
                        query += " AND a.annee_scol = @AnneeScol";
                    }

                    query += @"
                        LEFT JOIN t_eleves e ON a.matricule = e.matricule
                        GROUP BY s.cod_sect, s.description, o.cod_opt, o.description, p.cod_promo, p.description
                        HAVING COUNT(e.matricule) > 0
                        ORDER BY s.description, o.description, p.description";

                    var parameters = new { AnneeScol = anneeScol };
                    return conn.Query(query, parameters);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des statistiques du tableau de bord: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets titulaire information for DataGridViewAccueil2
        /// Returns information about class supervisors (titulaires) by promotion
        /// </summary>
        /// <param name="anneeScol">School year filter (optional)</param>
        /// <returns>Dynamic objects with Section, Option, Promotion, NomTitulaire, TotalNbreEleves</returns>
        public IEnumerable<dynamic> GetTitulaireInfo(string? anneeScol = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"
                        SELECT 
                            COALESCE(s.description, 'Non défini') as Section,
                            COALESCE(o.description, 'Non défini') as Option,
                            COALESCE(p.description, 'Non défini') as Promotion,
                            COALESCE(ac.titulaire, 'Non assigné') as NomTitulaire,
                            COUNT(DISTINCT a.matricule) as TotalNbreEleves
                        FROM t_sections s
                        LEFT JOIN t_options o ON s.cod_sect = o.cod_sect
                        LEFT JOIN t_promotions p ON o.cod_opt = p.cod_opt
                        LEFT JOIN t_affect_cours ac ON p.cod_promo = ac.cod_promo";

                    // Add school year filter if provided
                    if (!string.IsNullOrEmpty(anneeScol))
                    {
                        query += " AND ac.annee_scol = @AnneeScol";
                    }

                    query += @"
                        LEFT JOIN t_affectation a ON p.cod_promo = a.cod_promo";

                    // Add school year filter for affectation if provided
                    if (!string.IsNullOrEmpty(anneeScol))
                    {
                        query += " AND a.annee_scol = @AnneeScol";
                    }

                    query += @"
                        LEFT JOIN t_eleves e ON a.matricule = e.matricule
                        GROUP BY s.cod_sect, s.description, o.cod_opt, o.description, p.cod_promo, p.description, ac.titulaire
                        HAVING ac.titulaire IS NOT NULL
                        ORDER BY s.description, o.description, p.description, ac.titulaire";

                    var parameters = new { AnneeScol = anneeScol };
                    return conn.Query(query, parameters);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des informations des titulaires: {ex.Message}", ex);
            }
        }

        #endregion

        #region Summary Statistics Methods

        /// <summary>
        /// Gets total student count across all sections
        /// </summary>
        /// <param name="anneeScol">School year filter (optional)</param>
        /// <returns>Total number of students</returns>
        public int GetTotalStudents(string? anneeScol = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"
                        SELECT COUNT(DISTINCT e.matricule) 
                        FROM t_eleves e";

                    if (!string.IsNullOrEmpty(anneeScol))
                    {
                        query += @"
                            INNER JOIN t_affectation a ON e.matricule = a.matricule 
                            WHERE a.annee_scol = @AnneeScol";
                        return conn.ExecuteScalar<int>(query, new { AnneeScol = anneeScol });
                    }

                    return conn.ExecuteScalar<int>(query);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du calcul du total des élèves: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets student count by gender
        /// </summary>
        /// <param name="anneeScol">School year filter (optional)</param>
        /// <returns>Dynamic object with Garcons and Filles counts</returns>
        public dynamic GetStudentsByGender(string? anneeScol = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"
                        SELECT 
                            SUM(CASE WHEN e.sexe = 'M' THEN 1 ELSE 0 END) as Garcons,
                            SUM(CASE WHEN e.sexe = 'F' THEN 1 ELSE 0 END) as Filles
                        FROM t_eleves e";

                    if (!string.IsNullOrEmpty(anneeScol))
                    {
                        query += @"
                            INNER JOIN t_affectation a ON e.matricule = a.matricule 
                            WHERE a.annee_scol = @AnneeScol";
                        return conn.QueryFirstOrDefault(query, new { AnneeScol = anneeScol });
                    }

                    return conn.QueryFirstOrDefault(query);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du calcul des élèves par sexe: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets count of active sections
        /// </summary>
        /// <returns>Number of sections with students</returns>
        public int GetActiveSectionsCount()
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"
                        SELECT COUNT(DISTINCT s.cod_sect)
                        FROM t_sections s
                        INNER JOIN t_options o ON s.cod_sect = o.cod_sect
                        INNER JOIN t_promotions p ON o.cod_opt = p.cod_opt
                        INNER JOIN t_affectation a ON p.cod_promo = a.cod_promo
                        INNER JOIN t_eleves e ON a.matricule = e.matricule";

                    return conn.ExecuteScalar<int>(query);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du calcul des sections actives: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets count of active promotions
        /// </summary>
        /// <returns>Number of promotions with students</returns>
        public int GetActivePromotionsCount()
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"
                        SELECT COUNT(DISTINCT p.cod_promo)
                        FROM t_promotions p
                        INNER JOIN t_affectation a ON p.cod_promo = a.cod_promo
                        INNER JOIN t_eleves e ON a.matricule = e.matricule";

                    return conn.ExecuteScalar<int>(query);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du calcul des promotions actives: {ex.Message}", ex);
            }
        }

        #endregion

        #region Data Validation Methods

        /// <summary>
        /// Validates if the provided school year exists in the database
        /// </summary>
        /// <param name="anneeScol">School year to validate</param>
        /// <returns>True if the school year exists</returns>
        public bool IsValidSchoolYear(string anneeScol)
        {
            try
            {
                if (string.IsNullOrEmpty(anneeScol))
                    return false;

                using (var conn = _connexion.GetConnection())
                {
                    var query = @"
                        SELECT COUNT(DISTINCT annee_scol) 
                        FROM t_affectation 
                        WHERE annee_scol = @AnneeScol";

                    return conn.ExecuteScalar<int>(query, new { AnneeScol = anneeScol }) > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la validation de l'année scolaire: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all available school years from the database
        /// </summary>
        /// <returns>List of school years</returns>
        public IEnumerable<string> GetAvailableSchoolYears()
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"
                        SELECT DISTINCT annee_scol 
                        FROM t_affectation 
                        WHERE annee_scol IS NOT NULL 
                        ORDER BY annee_scol DESC";

                    return conn.Query<string>(query);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des années scolaires: {ex.Message}", ex);
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets the current school year (most recent year in database)
        /// </summary>
        /// <returns>Current school year string</returns>
        public string GetCurrentSchoolYear()
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"
                        SELECT annee_scol 
                        FROM t_affectation 
                        WHERE annee_scol IS NOT NULL 
                        ORDER BY annee_scol DESC 
                        LIMIT 1";

                    return conn.QueryFirstOrDefault<string>(query) ?? DateTime.Now.Year + "-" + (DateTime.Now.Year + 1);
                }
            }
            catch (Exception ex)
            {
                // Return current year format if database query fails
                var currentYear = DateTime.Now.Year;
                return $"{currentYear}-{currentYear + 1}";
            }
        }

        /// <summary>
        /// Calculates totals for dashboard statistics
        /// </summary>
        /// <param name="statistics">Statistics data from GetDashboardStatistics</param>
        /// <returns>Dynamic object with total calculations</returns>
        public dynamic CalculateStatisticsTotals(IEnumerable<dynamic> statistics)
        {
            try
            {
                if (statistics == null || !statistics.Any())
                {
                    return new
                    {
                        Section = "TOTAUX",
                        Option = "",
                        Promotion = "",
                        NbreEleveGarcons = 0,
                        NbreElevesFilles = 0,
                        TotalNbreEleves = 0
                    };
                }

                var totalGarcons = 0;
                var totalFilles = 0;
                var totalEleves = 0;

                foreach (var stat in statistics)
                {
                    totalGarcons += (int)(stat.NbreEleveGarcons ?? 0);
                    totalFilles += (int)(stat.NbreElevesFilles ?? 0);
                    totalEleves += (int)(stat.TotalNbreEleves ?? 0);
                }

                return new
                {
                    Section = "TOTAUX",
                    Option = "",
                    Promotion = "",
                    NbreEleveGarcons = totalGarcons,
                    NbreElevesFilles = totalFilles,
                    TotalNbreEleves = totalEleves
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du calcul des totaux: {ex.Message}", ex);
            }
        }

        #endregion
    }
}