using Dapper;
using EduKin.Csharp.Admins;
using EduKin.DataSets;
using System.Data;

namespace EduKin.Csharp.Finances
{

    public class Paiements
    {
        private readonly Connexion _connexion;
        private readonly Administrations _admin;

        public Paiements()
        {
            _connexion = Connexion.Instance;
            _admin = new Administrations();
        }
        
        #region CRUD Caisse

        public bool CreateCaisse(DateTime dateStock, decimal montant, string idEcole)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"INSERT INTO t_caisse (date_stock, montant, id_ecole) 
                                  VALUES (@DateStock, @Montant, @IdEcole)";
                    
                    conn.Execute(query, new { DateStock = dateStock, Montant = montant, IdEcole = idEcole });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création: {ex.Message}", ex);
            }
        }

        public dynamic? GetCaisseByEcole(string idEcole, DateTime? date = null)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = "SELECT * FROM t_caisse WHERE id_ecole = @IdEcole";
                
                if (date.HasValue)
                {
                    query += " AND date_stock = @Date";
                    return conn.QueryFirstOrDefault(query, new { IdEcole = idEcole, Date = date.Value });
                }
                
                query += " ORDER BY date_stock DESC LIMIT 1";
                return conn.QueryFirstOrDefault(query, new { IdEcole = idEcole });
            }
        }

        public IEnumerable<dynamic> GetHistoriqueCaisse(string idEcole, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = "SELECT * FROM t_caisse WHERE id_ecole = @IdEcole";
                
                if (startDate.HasValue)
                    query += " AND date_stock >= @StartDate";
                
                if (endDate.HasValue)
                    query += " AND date_stock <= @EndDate";
                
                query += " ORDER BY date_stock DESC";
                
                return conn.Query(query, new { IdEcole = idEcole, StartDate = startDate, EndDate = endDate });
            }
        }

        public bool UpdateCaisse(int idCaisse, decimal montant)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = "UPDATE t_caisse SET montant = @Montant WHERE id_caisse = @IdCaisse";
                    conn.Execute(query, new { IdCaisse = idCaisse, Montant = montant });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public decimal GetSoldeCaisse(string idEcole)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT COALESCE(montant, 0) FROM t_caisse 
                              WHERE id_ecole = @IdEcole 
                              ORDER BY date_stock DESC LIMIT 1";
                return conn.ExecuteScalar<decimal>(query, new { IdEcole = idEcole });
            }
        }

        #endregion

        #region CRUD Coupons

        public bool CreateCoupon(string matricule, string periode, int maxGen, decimal totaux, 
                                 decimal pourc, string codPromo, string anneeScol)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"INSERT INTO t_coupons 
                                  (matricule, periode, max_gen, totaux, pourc, cod_promo, annee_scol) 
                                  VALUES (@Matricule, @Periode, @MaxGen, @Totaux, @Pourc, @CodPromo, @AnneeScol)";
                    
                    conn.Execute(query, new 
                    { 
                        Matricule = matricule,
                        Periode = periode,
                        MaxGen = maxGen,
                        Totaux = totaux,
                        Pourc = pourc,
                        CodPromo = codPromo,
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

        public IEnumerable<dynamic> GetCouponsByEleve(string matricule, string anneeScol)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT c.*, e.nom, e.postnom, e.prenom 
                              FROM t_coupons c 
                              INNER JOIN t_eleves e ON c.matricule = e.matricule 
                              WHERE c.matricule = @Matricule AND c.annee_scol = @AnneeScol 
                              ORDER BY c.periode DESC";
                return conn.Query(query, new { Matricule = matricule, AnneeScol = anneeScol });
            }
        }

        public IEnumerable<dynamic> GetCouponsByPromotion(string codPromo, string periode, string anneeScol)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT c.*, e.nom, e.postnom, e.prenom 
                              FROM t_coupons c 
                              INNER JOIN t_eleves e ON c.matricule = e.matricule 
                              WHERE c.cod_promo = @CodPromo AND c.periode = @Periode AND c.annee_scol = @AnneeScol 
                              ORDER BY e.nom DESC";
                return conn.Query(query, new { CodPromo = codPromo, Periode = periode, AnneeScol = anneeScol });
            }
        }

        public bool UpdateCoupon(int numCoupon, int maxGen, decimal totaux, decimal pourc)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"UPDATE t_coupons 
                                  SET max_gen = @MaxGen, totaux = @Totaux, pourc = @Pourc 
                                  WHERE num_coupon = @NumCoupon";
                    
                    conn.Execute(query, new { NumCoupon = numCoupon, MaxGen = maxGen, Totaux = totaux, Pourc = pourc });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public bool DeleteCoupon(int numCoupon)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Execute("DELETE FROM t_coupons WHERE num_coupon = @NumCoupon", new { NumCoupon = numCoupon });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        #endregion

        #region CRUD Frais

        public bool CreateFrais(string codFrais, string description, decimal montant, string codType_Frais, 
                                string modalite, string idEcole, string? periode = null, string? anneeScol = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"INSERT INTO t_frais (cod_frais, description, montant, cod_type_frais, modalite, periode, annee_scol, id_ecole) 
                                  VALUES (@CodFrais, @Description, @Montant, @CodType_Frais, @Modalite, @Periode, @AnneeScol, @IdEcole)";
                    
                    conn.Execute(query, new 
                    { 
                        CodFrais = codFrais,
                        Description = description,
                        Montant = montant,
                        CodType_Frais = codType_Frais,
                        Modalite = modalite,
                        Periode = periode,
                        AnneeScol = anneeScol,
                        IdEcole = idEcole
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public IEnumerable<dynamic> GetFraisByType_Frais(string codType_Frais, string? anneeScol = null)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT f.*, o.description as Type_Frais_desc 
                              FROM t_frais f 
                              INNER JOIN t_type_frais o ON f.cod_type_frais = o.cod_type_frais 
                              WHERE f.cod_type_frais = @CodType_Frais";
                
                if (!string.IsNullOrEmpty(anneeScol))
                    query += " AND f.annee_scol = @AnneeScol";
                
                query += " ORDER BY f.description";
                
                return conn.Query(query, new { CodType_Frais = codType_Frais, AnneeScol = anneeScol });
            }
        }

        public IEnumerable<dynamic> GetFraisByEcole(string idEcole, string? anneeScol = null)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT f.*, o.description as Type_Frais_desc 
                              FROM t_frais f 
                              INNER JOIN t_type_frais o ON f.cod_type_frais = o.cod_type_frais 
                              WHERE f.id_ecole = @IdEcole";
                
                if (!string.IsNullOrEmpty(anneeScol))
                    query += " AND f.annee_scol = @AnneeScol";
                
                query += " ORDER BY o.description, f.description";
                
                return conn.Query(query, new { IdEcole = idEcole, AnneeScol = anneeScol });
            }
        }

        public IEnumerable<dynamic> GetAllFrais(string? anneeScol = null)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT f.*, o.description as Type_Frais_desc, e.denomination as ecole_nom 
                              FROM t_frais f 
                              INNER JOIN t_type_frais o ON f.cod_type_frais = o.cod_type_frais 
                              INNER JOIN t_ecoles e ON f.id_ecole = e.id_ecole";
                
                if (!string.IsNullOrEmpty(anneeScol))
                    query += " WHERE f.annee_scol = @AnneeScol";
                
                query += " ORDER BY e.denomination, o.description, f.description";
                
                return conn.Query(query, new { AnneeScol = anneeScol });
            }
        }

        public decimal GetTotalFraisByType_Frais(string codType_Frais, string? anneeScol = null)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT COALESCE(SUM(montant), 0) FROM t_frais 
                              WHERE cod_type_frais = @CodType_Frais";
                
                if (!string.IsNullOrEmpty(anneeScol))
                    query += " AND annee_scol = @AnneeScol";
                
                return conn.ExecuteScalar<decimal>(query, new { CodType_Frais = codType_Frais, AnneeScol = anneeScol });
            }
        }

        public decimal GetTotalFraisByEcole(string idEcole, string? anneeScol = null)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT COALESCE(SUM(montant), 0) FROM t_frais 
                              WHERE id_ecole = @IdEcole";
                
                if (!string.IsNullOrEmpty(anneeScol))
                    query += " AND annee_scol = @AnneeScol";
                
                return conn.ExecuteScalar<decimal>(query, new { IdEcole = idEcole, AnneeScol = anneeScol });
            }
        }

        public bool UpdateFrais(string codFrais, string description, decimal montant, string modalite, string? periode = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"UPDATE t_frais 
                                  SET description = @Description, montant = @Montant, modalite = @Modalite, periode = @Periode 
                                  WHERE cod_frais = @CodFrais";
                    
                    conn.Execute(query, new 
                    { 
                        CodFrais = codFrais, 
                        Description = description, 
                        Montant = montant, 
                        Modalite = modalite, 
                        Periode = periode 
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public bool DeleteFrais(string codFrais)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Execute("DELETE FROM t_frais WHERE cod_frais = @CodFrais", new { CodFrais = codFrais });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public dynamic? GetFraisById(string codFrais)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT f.*, o.description as Type_Frais_desc 
                              FROM t_frais f 
                              INNER JOIN t_type_frais o ON f.cod_type_frais = o.cod_type_frais 
                              WHERE f.cod_frais = @CodFrais";
                return conn.QueryFirstOrDefault(query, new { CodFrais = codFrais });
            }
        }

        #endregion

        #region CRUD Type_Frais 

        public bool CreateType_Frais(string codtype_frais, string intitule)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = "INSERT INTO t_type_frais (cod_type_frais, description) VALUES (@codtype_frais, @Description)";
                    conn.Execute(query, new { codtype_frais = codtype_frais, Description = intitule });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public IEnumerable<dynamic> GetAllType_Fraiss()
        {
            using (var conn = _connexion.GetConnection())
            {
                return conn.Query("SELECT * FROM t_type_frais");
            }
        }

        public IEnumerable<dynamic> GetAllType_Frais()
        {
            using (var conn = _connexion.GetConnection())
            {
                return conn.Query("SELECT * FROM t_type_frais");
            }
        }

        public bool UpdateType_Frais(string codtype_frais, string intitule)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = "UPDATE t_type_frais SET description = @Description WHERE cod_type_frais = @codtype_frais";
                    conn.Execute(query, new { codtype_frais = codtype_frais, Description = intitule });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public bool DeleteType_Frais(string codtype_frais)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Execute("DELETE FROM t_type_frais WHERE cod_type_frais = @codtype_frais", new { codtype_frais = codtype_frais });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        #endregion

        #region CRUD Paie (Salaires)

        public bool CreatePaie(string matricule, decimal montant, string mois, string anneeScol, 
                               DateTime datePaie, string? observation = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"INSERT INTO t_paie 
                                  (matricule, montant, mois, annee_scol, date_paie, observation) 
                                  VALUES (@Matricule, @Montant, @Mois, @AnneeScol, @DatePaie, @Observation)";
                    
                    conn.Execute(query, new 
                    { 
                        Matricule = matricule,
                        Montant = montant,
                        Mois = mois,
                        AnneeScol = anneeScol,
                        DatePaie = datePaie,
                        Observation = observation
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public IEnumerable<dynamic> GetPaiesByAgent(string matricule, string anneeScol)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT p.*, a.nom, a.postnom, a.prenom 
                              FROM t_paie p 
                              INNER JOIN t_agents a ON p.matricule = a.matricule 
                              WHERE p.matricule = @Matricule AND p.annee_scol = @AnneeScol 
                              ORDER BY p.date_paie DESC";
                return conn.Query(query, new { Matricule = matricule, AnneeScol = anneeScol });
            }
        }

        public IEnumerable<dynamic> GetPaiesByMois(string mois, string anneeScol)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT p.*, a.nom, a.postnom, a.prenom, a.service 
                              FROM t_paie p 
                              INNER JOIN t_agents a ON p.matricule = a.matricule 
                              WHERE p.mois = @Mois AND p.annee_scol = @AnneeScol 
                              ORDER BY a.nom";
                return conn.Query(query, new { Mois = mois, AnneeScol = anneeScol });
            }
        }

        public decimal GetTotalPaiesByMois(string mois, string anneeScol)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT COALESCE(SUM(montant), 0) FROM t_paie 
                              WHERE mois = @Mois AND annee_scol = @AnneeScol";
                return conn.ExecuteScalar<decimal>(query, new { Mois = mois, AnneeScol = anneeScol });
            }
        }

        public bool UpdatePaie(int idPaie, decimal montant, string? observation = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"UPDATE t_paie 
                                  SET montant = @Montant, observation = @Observation 
                                  WHERE id_paie = @IdPaie";
                    
                    conn.Execute(query, new { IdPaie = idPaie, Montant = montant, Observation = observation });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public bool DeletePaie(int idPaie)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Execute("DELETE FROM t_paie WHERE id_paie = @IdPaie", new { IdPaie = idPaie });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        #endregion

        #region CRUD Paiements (Frais Scolaires)

        public bool CreatePaiement(string matricule, string codFrais, decimal montant, DateTime datePaiement, 
                                   string modePaiement, string anneeScol, string? referencePaiement = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"INSERT INTO t_paiement 
                                  (matricule, cod_frais, montant, date_paiement, mode_paiement, annee_scol, reference_paiement) 
                                  VALUES (@Matricule, @CodFrais, @Montant, @DatePaiement, @ModePaiement, @AnneeScol, @ReferencePaiement)";
                    
                    conn.Execute(query, new 
                    { 
                        Matricule = matricule,
                        CodFrais = codFrais,
                        Montant = montant,
                        DatePaiement = datePaiement,
                        ModePaiement = modePaiement,
                        AnneeScol = anneeScol,
                        ReferencePaiement = referencePaiement
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public IEnumerable<dynamic> GetPaiementsByEleve(string matricule, string anneeScol)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT p.*, f.description as frais_description, e.nom, e.postnom, e.prenom 
                              FROM t_paiement p 
                              INNER JOIN t_frais f ON p.cod_frais = f.cod_frais 
                              INNER JOIN t_eleves e ON p.matricule = e.matricule 
                              WHERE p.matricule = @Matricule AND p.annee_scol = @AnneeScol 
                              ORDER BY p.date_paiement DESC";
                return conn.Query(query, new { Matricule = matricule, AnneeScol = anneeScol });
            }
        }

        public IEnumerable<dynamic> GetPaiementsByFrais(string codFrais, string anneeScol)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT p.*, f.description as frais_description, e.nom, e.postnom, e.prenom 
                              FROM t_paiement p 
                              INNER JOIN t_frais f ON p.cod_frais = f.cod_frais 
                              INNER JOIN t_eleves e ON p.matricule = e.matricule 
                              WHERE p.cod_frais = @CodFrais AND p.annee_scol = @AnneeScol 
                              ORDER BY p.date_paiement DESC";
                return conn.Query(query, new { CodFrais = codFrais, AnneeScol = anneeScol });
            }
        }

        public IEnumerable<dynamic> GetPaiementsByDate(DateTime startDate, DateTime endDate, string? idEcole = null)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT p.*, f.description as frais_description, e.nom, e.postnom, e.prenom 
                              FROM t_paiement p 
                              INNER JOIN t_frais f ON p.cod_frais = f.cod_frais 
                              INNER JOIN t_eleves e ON p.matricule = e.matricule 
                              WHERE p.date_paiement BETWEEN @StartDate AND @EndDate";
                
                if (!string.IsNullOrEmpty(idEcole))
                {
                    query += " AND f.id_ecole = @IdEcole";
                }
                
                query += " ORDER BY p.date_paiement DESC";
                
                return conn.Query(query, new { StartDate = startDate, EndDate = endDate, IdEcole = idEcole });
            }
        }

        public decimal GetTotalPaiementsByEleve(string matricule, string anneeScol)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT COALESCE(SUM(montant), 0) FROM t_paiement 
                              WHERE matricule = @Matricule AND annee_scol = @AnneeScol";
                return conn.ExecuteScalar<decimal>(query, new { Matricule = matricule, AnneeScol = anneeScol });
            }
        }

        public decimal GetTotalPaiementsByFrais(string codFrais, string anneeScol)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT COALESCE(SUM(montant), 0) FROM t_paiement 
                              WHERE cod_frais = @CodFrais AND annee_scol = @AnneeScol";
                return conn.ExecuteScalar<decimal>(query, new { CodFrais = codFrais, AnneeScol = anneeScol });
            }
        }

        public bool UpdatePaiement(int idPaiement, decimal montant, string modePaiement, string? referencePaiement = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"UPDATE t_paiement 
                                  SET montant = @Montant, mode_paiement = @ModePaiement, reference_paiement = @ReferencePaiement 
                                  WHERE id_paiement = @IdPaiement";
                    
                    conn.Execute(query, new 
                    { 
                        IdPaiement = idPaiement,
                        Montant = montant,
                        ModePaiement = modePaiement,
                        ReferencePaiement = referencePaiement
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public bool DeletePaiement(int idPaiement)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Execute("DELETE FROM t_paiement WHERE id_paiement = @IdPaiement", new { IdPaiement = idPaiement });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        #endregion

        #region CRUD Entrées (Recettes)

        public bool CreateEntree(string motif, decimal montant, DateTime dateEntree, string idEcole, 
                                 string? source = null, string? referencePiece = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"INSERT INTO t_entree 
                                  (motif, montant, date_entree, id_ecole, source, reference_piece) 
                                  VALUES (@Motif, @Montant, @DateEntree, @IdEcole, @Source, @ReferencePiece)";
                    
                    conn.Execute(query, new 
                    { 
                        Motif = motif,
                        Montant = montant,
                        DateEntree = dateEntree,
                        IdEcole = idEcole,
                        Source = source,
                        ReferencePiece = referencePiece
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public IEnumerable<dynamic> GetEntreesByEcole(string idEcole, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = "SELECT * FROM t_entree WHERE id_ecole = @IdEcole";
                
                if (startDate.HasValue)
                    query += " AND date_entree >= @StartDate";
                
                if (endDate.HasValue)
                    query += " AND date_entree <= @EndDate";
                
                query += " ORDER BY date_entree DESC";
                
                return conn.Query(query, new { IdEcole = idEcole, StartDate = startDate, EndDate = endDate });
            }
        }

        public decimal GetTotalEntrees(string idEcole, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = "SELECT COALESCE(SUM(montant), 0) FROM t_entree WHERE id_ecole = @IdEcole";
                
                if (startDate.HasValue)
                    query += " AND date_entree >= @StartDate";
                
                if (endDate.HasValue)
                    query += " AND date_entree <= @EndDate";
                
                return conn.ExecuteScalar<decimal>(query, new { IdEcole = idEcole, StartDate = startDate, EndDate = endDate });
            }
        }

        public bool UpdateEntree(int idEntree, string motif, decimal montant, string? source = null, string? referencePiece = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"UPDATE t_entree 
                                  SET motif = @Motif, montant = @Montant, source = @Source, reference_piece = @ReferencePiece 
                                  WHERE id_entree = @IdEntree";
                    
                    conn.Execute(query, new 
                    { 
                        IdEntree = idEntree,
                        Motif = motif,
                        Montant = montant,
                        Source = source,
                        ReferencePiece = referencePiece
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public bool DeleteEntree(int idEntree)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Execute("DELETE FROM t_entree WHERE id_entree = @IdEntree", new { IdEntree = idEntree });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        #endregion

        #region CRUD Sorties (Dépenses)

        public bool CreateSortie(string motif, decimal montant, DateTime dateSortie, string idEcole, 
                                 string? beneficiaire = null, string? referencePiece = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"INSERT INTO t_sortie 
                                  (motif, montant, date_sortie, id_ecole, beneficiaire, reference_piece) 
                                  VALUES (@Motif, @Montant, @DateSortie, @IdEcole, @Beneficiaire, @ReferencePiece)";
                    
                    conn.Execute(query, new 
                    { 
                        Motif = motif,
                        Montant = montant,
                        DateSortie = dateSortie,
                        IdEcole = idEcole,
                        Beneficiaire = beneficiaire,
                        ReferencePiece = referencePiece
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public IEnumerable<dynamic> GetSortiesByEcole(string idEcole, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = "SELECT * FROM t_sortie WHERE id_ecole = @IdEcole";
                
                if (startDate.HasValue)
                    query += " AND date_sortie >= @StartDate";
                
                if (endDate.HasValue)
                    query += " AND date_sortie <= @EndDate";
                
                query += " ORDER BY date_sortie DESC";
                
                return conn.Query(query, new { IdEcole = idEcole, StartDate = startDate, EndDate = endDate });
            }
        }

        public decimal GetTotalSorties(string idEcole, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = "SELECT COALESCE(SUM(montant), 0) FROM t_sortie WHERE id_ecole = @IdEcole";
                
                if (startDate.HasValue)
                    query += " AND date_sortie >= @StartDate";
                
                if (endDate.HasValue)
                    query += " AND date_sortie <= @EndDate";
                
                return conn.ExecuteScalar<decimal>(query, new { IdEcole = idEcole, StartDate = startDate, EndDate = endDate });
            }
        }

        public bool UpdateSortie(int idSortie, string motif, decimal montant, string? beneficiaire = null, string? referencePiece = null)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    var query = @"UPDATE t_sortie 
                                  SET motif = @Motif, montant = @Montant, beneficiaire = @Beneficiaire, reference_piece = @ReferencePiece 
                                  WHERE id_sortie = @IdSortie";
                    
                    conn.Execute(query, new 
                    { 
                        IdSortie = idSortie,
                        Motif = motif,
                        Montant = montant,
                        Beneficiaire = beneficiaire,
                        ReferencePiece = referencePiece
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public bool DeleteSortie(int idSortie)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Execute("DELETE FROM t_sortie WHERE id_sortie = @IdSortie", new { IdSortie = idSortie });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        #endregion

        #region Rapports et Statistiques

        public dynamic GetBilanFinancier(string idEcole, DateTime startDate, DateTime endDate)
        {
            using (var conn = _connexion.GetConnection())
            {
                var totalEntrees = GetTotalEntrees(idEcole, startDate, endDate);
                var totalSorties = GetTotalSorties(idEcole, startDate, endDate);
                var solde = totalEntrees - totalSorties;

                return new
                {
                    TotalEntrees = totalEntrees,
                    TotalSorties = totalSorties,
                    Solde = solde,
                    Periode = $"{startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}"
                };
            }
        }

        public IEnumerable<dynamic> GetElevesSolvables(string codPromo, string anneeScol)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT e.matricule, e.nom, e.postnom, e.prenom,
                              COALESCE(SUM(p.montant), 0) as total_paye,
                              (SELECT SUM(montant) FROM t_frais WHERE cod_promo = @CodPromo AND annee_scol = @AnneeScol) as total_frais
                              FROM t_eleves e
                              INNER JOIN t_affectation a ON e.matricule = a.matricule
                              LEFT JOIN t_paiement p ON e.matricule = p.matricule AND p.annee_scol = @AnneeScol
                              WHERE a.cod_promo = @CodPromo AND a.annee_scol = @AnneeScol
                              GROUP BY e.matricule, e.nom, e.postnom, e.prenom
                              HAVING total_paye >= total_frais";
                
                return conn.Query(query, new { CodPromo = codPromo, AnneeScol = anneeScol });
            }
        }

        public IEnumerable<dynamic> GetElevesNonSolvables(string codPromo, string anneeScol)
        {
            using (var conn = _connexion.GetConnection())
            {
                var query = @"SELECT e.matricule, e.nom, e.postnom, e.prenom,
                              COALESCE(SUM(p.montant), 0) as total_paye,
                              (SELECT SUM(montant) FROM t_frais WHERE cod_promo = @CodPromo AND annee_scol = @AnneeScol) as total_frais,
                              ((SELECT SUM(montant) FROM t_frais WHERE cod_promo = @CodPromo AND annee_scol = @AnneeScol) - COALESCE(SUM(p.montant), 0)) as reste_a_payer
                              FROM t_eleves e
                              INNER JOIN t_affectation a ON e.matricule = a.matricule
                              LEFT JOIN t_paiement p ON e.matricule = p.matricule AND p.annee_scol = @AnneeScol
                              WHERE a.cod_promo = @CodPromo AND a.annee_scol = @AnneeScol
                              GROUP BY e.matricule, e.nom, e.postnom, e.prenom
                              HAVING total_paye < total_frais
                              ORDER BY reste_a_payer DESC";
                
                return conn.Query(query, new { CodPromo = codPromo, AnneeScol = anneeScol });
            }
        }

        #endregion
    }
}
