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

        public bool CreateGrille(string codPromo, string idCours, int ponderation, string anneeScol)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"INSERT INTO t_grille (cod_promo, id_cours, ponderation, annee_scol) 
                                  VALUES (@CodPromo, @IdCours, @Ponderation, @AnneeScol)";
                    
                    conn.Execute(query, new 
                    { 
                        CodPromo = codPromo,
                        IdCours = idCours,
                        Ponderation = ponderation,
                        AnneeScol = anneeScol
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
                              FROM t_grille g 
                              INNER JOIN t_cours c ON g.id_cours = c.id_cours 
                              WHERE g.cod_promo = @CodPromo AND g.annee_scol = @AnneeScol 
                              ORDER BY c.intitule";
                return conn.Query(query, new { CodPromo = codPromo, AnneeScol = anneeScol });
            }
        }

        public int GetTotalPonderationByPromotion(string codPromo, string anneeScol)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT COALESCE(SUM(ponderation), 0) 
                              FROM t_grille 
                              WHERE cod_promo = @CodPromo AND annee_scol = @AnneeScol";
                return conn.ExecuteScalar<int>(query, new { CodPromo = codPromo, AnneeScol = anneeScol });
            }
        }

        public bool UpdateGrille(int num, int ponderation)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = "UPDATE t_grille SET ponderation = @Ponderation WHERE num = @Num";
                    conn.Execute(query, new { Num = num, Ponderation = ponderation });
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
                    var query = "DELETE FROM t_grille WHERE num = @Num";
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
                                  SELECT id_cours FROM t_grille 
                                  WHERE cod_promo = @CodPromo AND annee_scol = @AnneeScol
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
                    var query = @"INSERT INTO t_grille (cod_promo, id_cours, ponderation, annee_scol)
                                  SELECT cod_promo, id_cours, ponderation, @NewAnneeScol
                                  FROM t_grille
                                  WHERE cod_promo = @CodPromo AND annee_scol = @OldAnneeScol";
                    
                    conn.Execute(query, new 
                    { 
                        CodPromo = codPromo,
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
