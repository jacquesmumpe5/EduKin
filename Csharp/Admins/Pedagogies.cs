using Dapper;
using EduKin.DataSets;

namespace EduKin.Csharp.Admins
{
    public class Pedagogies
    {
        private readonly Connexion _connexion;
        private readonly Administrations _admin;

        public Pedagogies()
        {
            _connexion = Connexion.Instance;
            _admin = new Administrations();
        }

        #region CRUD Cours

        public bool CreateCours(string idCours, string intitule)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = "INSERT INTO t_cours (id_cours, intitule) VALUES (@IdCours, @Intitule)";
                    conn.Execute(query, new { IdCours = idCours, Intitule = intitule });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création du cours: {ex.Message}", ex);
            }
        }

        public dynamic? GetCours(string idCours)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = "SELECT * FROM t_cours WHERE id_cours = @IdCours";
                return conn.QueryFirstOrDefault(query, new { IdCours = idCours });
            }
        }

        public IEnumerable<dynamic> GetAllCours()
        {
            using (var conn = _connexion.GetConnection())
            {
                return conn.Query("SELECT * FROM t_cours ORDER BY intitule");
            }
        }

        public IEnumerable<dynamic> SearchCours(string searchTerm)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT * FROM t_cours 
                              WHERE intitule LIKE @Search OR id_cours LIKE @Search 
                              ORDER BY intitule";
                return conn.Query(query, new { Search = $"%{searchTerm}%" });
            }
        }

        public bool UpdateCours(string idCours, string intitule)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = "UPDATE t_cours SET intitule = @Intitule WHERE id_cours = @IdCours";
                    conn.Execute(query, new { IdCours = idCours, Intitule = intitule });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour: {ex.Message}", ex);
            }
        }

        public bool DeleteCours(string idCours)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = "DELETE FROM t_cours WHERE id_cours = @IdCours";
                    conn.Execute(query, new { IdCours = idCours });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression: {ex.Message}", ex);
            }
        }

        #endregion

        #region CRUD Grille

        public bool CreateGrille(string matricule, string periode, string anneeScol, string idCours, string intitule, decimal cotes, decimal maxima, string statut, string fkPromo, string indice)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"INSERT INTO t_grilles (fk_matricule_eleve, periode, annee_scol, fk_cours, intitule, cotes, maxima, statut, fk_promo, indice) 
                                  VALUES (@Matricule, @Periode, @AnneeScol, @IdCours, @Intitule, @Cotes, @Maxima, @Statut, @FkPromo, @Indice)";
                    
                    conn.Execute(query, new 
                    { 
                        Matricule = matricule,
                        Periode = periode,
                        AnneeScol = anneeScol,
                        IdCours = idCours,
                        Intitule = intitule,
                        Cotes = cotes,
                        Maxima = maxima,
                        Statut = statut,
                        FkPromo = fkPromo,
                        Indice = indice
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public IEnumerable<dynamic> GetGrilleByPromotion(string codPromo, string anneeScol)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT g.*, c.intitule as cours_intitule 
                              FROM t_grilles g 
                              INNER JOIN t_cours c ON g.fk_cours = c.id_cours 
                              WHERE g.fk_promo = @CodPromo AND g.annee_scol = @AnneeScol 
                              ORDER BY c.intitule";
                return conn.Query(query, new { CodPromo = codPromo, AnneeScol = anneeScol });
            }
        }

        public int GetTotalPonderationByPromotion(string codPromo, string anneeScol)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT COALESCE(SUM(maxima), 0) 
                              FROM t_grilles 
                              WHERE fk_promo = @CodPromo AND annee_scol = @AnneeScol";
                return conn.ExecuteScalar<int>(query, new { CodPromo = codPromo, AnneeScol = anneeScol });
            }
        }

        public bool UpdateGrille(int num, decimal cotes, decimal maxima)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = "UPDATE t_grilles SET cotes = @Cotes, maxima = @Maxima WHERE num = @Num";
                    conn.Execute(query, new { Num = num, Cotes = cotes, Maxima = maxima });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public bool DeleteGrille(int num)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = "DELETE FROM t_grilles WHERE num = @Num";
                    conn.Execute(query, new { Num = num });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public bool DeleteGrilleByPromotion(string codPromo, string anneeScol)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = "DELETE FROM t_grille WHERE cod_promo = @CodPromo AND annee_scol = @AnneeScol";
                    conn.Execute(query, new { CodPromo = codPromo, AnneeScol = anneeScol });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        #endregion

        #region Méthodes utilitaires

        public IEnumerable<dynamic> GetCoursNotInGrille(string codPromo, string anneeScol)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT c.* FROM t_cours c 
                              WHERE c.id_cours NOT IN (
                                  SELECT fk_cours FROM t_grilles 
                                  WHERE fk_promo = @CodPromo AND annee_scol = @AnneeScol
                              )
                              ORDER BY c.intitule";
                return conn.Query(query, new { CodPromo = codPromo, AnneeScol = anneeScol });
            }
        }

        public bool CopyGrilleToNewYear(string codPromo, string oldAnneeScol, string newAnneeScol)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"INSERT INTO t_grilles (fk_promo, fk_cours, ponderation, annee_scol, created_at, updated_at)
                                  SELECT fk_promo, fk_cours, ponderation, @NewAnneeScol, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
                                  FROM t_grilles
                                  WHERE fk_promo = @FkPromo AND annee_scol = @OldAnneeScol";
                    
                    conn.Execute(query, new 
                    { 
                        FkPromo = codPromo,
                        OldAnneeScol = oldAnneeScol,
                        NewAnneeScol = newAnneeScol
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la copie: {ex.Message}", ex);
            }
        }

        #endregion

        #region Méthodes asynchrones pour les bulletins

        /// <summary>
        /// Collecte des données de façon asynchrone
        /// </summary>
        public async Task<System.Data.DataTable> CollecteAsync(string query, params MySql.Data.MySqlClient.MySqlParameter[] parameters)
        {
            return await Task.Run(() =>
            {
                var dt = new System.Data.DataTable();
                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = query;
                        if (parameters != null && parameters.Length > 0)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.Add(param);
                            }
                        }
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
                return dt;
            });
        }

        /// <summary>
        /// Mise à jour de données de façon asynchrone
        /// </summary>
        public async Task<int> MiseAJourAsync(string query, params MySql.Data.MySqlClient.MySqlParameter[] parameters)
        {
            return await Task.Run(() =>
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = query;
                        if (parameters != null && parameters.Length > 0)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.Add(param);
                            }
                        }
                        return cmd.ExecuteNonQuery();
                    }
                }
            });
        }

        #endregion
    }
}
