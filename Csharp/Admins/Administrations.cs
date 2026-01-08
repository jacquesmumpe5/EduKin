using Dapper;
using EduKin.DataSets;
using EduKin.Inits;
using EduKinContext = EduKin.Inits.EduKinContext;
using EduKin.Layouts;
using MySql.Data.MySqlClient;
using Siticone.Desktop.UI.WinForms;
using System.Data;
using Windows.ApplicationModel;

namespace EduKin.Csharp.Admins
{
    /// <summary>
    /// Service d'administration avec isolation automatique par école
    /// Hérite de BaseService pour bénéficier des fonctionnalités d'isolation
    /// </summary>
    public class Administrations : BaseService
    {
        public Administrations() : base()
        {
            // Le constructeur de BaseService initialise déjà _connexion
        }

        #region Gestion du Contexte École

        /// <summary>
        /// Initialise le contexte pour une école spécifique
        /// </summary>
        /// <param name="schoolId">ID de l'école à activer</param>
        /// <returns>True si l'initialisation réussit</returns>
        public bool InitializeSchoolContext(string schoolId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(schoolId))
                {
                    throw new ArgumentException("L'ID de l'école ne peut pas être vide.", nameof(schoolId));
                }

                var school = GetEcole(schoolId);
                if (school == null)
                {
                    throw new InvalidOperationException($"École avec l'ID '{schoolId}' non trouvée.");
                }

                EduKinContext.Initialize(school.id_ecole, school.denomination);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'initialisation du contexte école: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Récupère les informations de l'école courante
        /// </summary>
        /// <returns>Informations de l'école active ou null si aucune école n'est configurée</returns>
        public dynamic? GetCurrentSchoolInfo()
        {
            if (!EduKinContext.IsConfigured)
            {
                return null;
            }

            return GetEcole(EduKinContext.CurrentIdEcole);
        }

        /// <summary>
        /// Réinitialise le contexte école
        /// </summary>
        public void ClearSchoolContext()
        {
            EduKinContext.Clear();
        }

        /// <summary>
        /// Vérifie si une école existe et est accessible
        /// </summary>
        /// <param name="schoolId">ID de l'école à vérifier</param>
        /// <returns>True si l'école existe</returns>
        public bool SchoolExists(string schoolId)
        {
            if (string.IsNullOrWhiteSpace(schoolId))
            {
                return false;
            }

            try
            {
                var school = GetEcole(schoolId);
                return school != null;
            }
            catch
            {
                return false;
            }
        }



        #endregion

        #region Génération d'ID avec sp_generate_id
        
        public string GenerateId(string table, string column, string prefix, string userIndex)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    conn.Open();
                    
                    if (Connexion.Instance.IsOnline)
                    {
                        // ✅ Formater userIndex avec les zéros à gauche (ex: "3" → "003")
                        string formattedUserIndex;
                        if (int.TryParse(userIndex, out int userIndexNumeric))
                        {
                            // Si c'est un numérique, formater avec 3 zéros: "3" → "003"
                            formattedUserIndex = userIndexNumeric.ToString("D3");
                        }
                        else if (userIndex.Length < 3)
                        {
                            // Si c'est une string courte, la compléter: "1" → "001"
                            formattedUserIndex = userIndex.PadLeft(3, '0');
                        }
                        else
                        {
                            // Sinon, utiliser tel quel
                            formattedUserIndex = userIndex;
                        }

                        System.Diagnostics.Debug.WriteLine($"[GenerateId] userIndex original: {userIndex}");
                        System.Diagnostics.Debug.WriteLine($"[GenerateId] userIndex formaté: {formattedUserIndex}");

                        using (var cmd = new MySqlCommand("sp_generate_id", (MySqlConnection)conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@p_table", table);
                            cmd.Parameters.AddWithValue("@p_column", column);
                            cmd.Parameters.AddWithValue("@p_prefix", prefix);
                            cmd.Parameters.AddWithValue("@p_user_index", formattedUserIndex); // ✅ Passer "003" avec les zéros
                            
                            var outputParam = new MySqlParameter("@p_new_id", MySqlDbType.VarChar, 128)
                            {
                                Direction = ParameterDirection.Output
                            };
                            cmd.Parameters.Add(outputParam);
                            
                            cmd.ExecuteNonQuery();
                            var generatedId = outputParam.Value?.ToString() ?? string.Empty;
                            
                            System.Diagnostics.Debug.WriteLine($"[GenerateId] ID généré: {generatedId}");
                            return generatedId;
                        }
                    }
                    else
                    {
                        // Génération locale pour SQLite
                        return GenerateLocalId(conn, table, column, prefix, userIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GenerateId] ERREUR: {ex.Message}");
                throw new Exception($"Erreur lors de la génération de l'ID: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Génère un ID pour la création d'une nouvelle école (bypass la vérification de contexte)
        /// </summary>
        /// <param name="table">Nom de la table</param>
        /// <param name="column">Nom de la colonne ID</param>
        /// <param name="prefix">Préfixe de l'ID</param>
        /// <param name="userIndex">Index utilisateur</param>
        /// <returns>ID généré</returns>
        public string GenerateIdForNewSchool(string table, string column, string prefix, string userIndex)
        {
            try
            {
                // ✅ Utiliser directement _connexion sans passer par GetSecureConnection()
                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();
                    
                    if (Connexion.Instance.IsOnline)
                    {
                        // ✅ Formater userIndex avec les zéros à gauche (ex: "3" → "003")
                        string formattedUserIndex;
                        if (int.TryParse(userIndex, out int userIndexNumeric))
                        {
                            // Si c'est un numérique, formater avec 3 zéros: "3" → "003"
                            formattedUserIndex = userIndexNumeric.ToString("D3");
                        }
                        else if (userIndex.Length < 3)
                        {
                            // Si c'est une string courte, la compléter: "1" → "001"
                            formattedUserIndex = userIndex.PadLeft(3, '0');
                        }
                        else
                        {
                            // Sinon, utiliser tel quel
                            formattedUserIndex = userIndex;
                        }

                        System.Diagnostics.Debug.WriteLine($"[GenerateIdForNewSchool] userIndex original: {userIndex}");
                        System.Diagnostics.Debug.WriteLine($"[GenerateIdForNewSchool] userIndex formaté: {formattedUserIndex}");

                        using (var cmd = new MySqlCommand("sp_generate_id", (MySqlConnection)conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@p_table", table);
                            cmd.Parameters.AddWithValue("@p_column", column);
                            cmd.Parameters.AddWithValue("@p_prefix", prefix);
                            cmd.Parameters.AddWithValue("@p_user_index", formattedUserIndex);
                            
                            var outputParam = new MySqlParameter("@p_new_id", MySqlDbType.VarChar, 128)
                            {
                                Direction = ParameterDirection.Output
                            };
                            cmd.Parameters.Add(outputParam);
                            
                            cmd.ExecuteNonQuery();
                            var generatedId = outputParam.Value?.ToString() ?? string.Empty;
                            
                            System.Diagnostics.Debug.WriteLine($"[GenerateIdForNewSchool] ID généré: {generatedId}");
                            return generatedId;
                        }
                    }
                    else
                    {
                        // Génération locale pour SQLite
                        return GenerateLocalIdForNewSchool(conn, table, column, prefix, userIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GenerateIdForNewSchool] ERREUR: {ex.Message}");
                throw new Exception($"Erreur lors de la génération de l'ID pour nouvelle école: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Génération locale d'ID pour nouvelle école (SQLite)
        /// </summary>
        private string GenerateLocalIdForNewSchool(IDbConnection conn, string table, string column, string prefix, string userIndex)
        {
            var year = DateTime.Now.Year.ToString();
            var query = $"SELECT MAX(CAST(SUBSTR({column}, LENGTH('{prefix}{userIndex}') + 1, 10) AS INTEGER)) FROM {table} WHERE {column} LIKE '{prefix}{userIndex}%'";
            
            var lastNum = conn.ExecuteScalar<int?>(query) ?? 0;
            var nextNum = lastNum + 1;
            var radical = nextNum.ToString().PadLeft(10, '0');
            
            return $"{prefix}{userIndex}{radical}{year}";
        }

        public void ExecuteGenerateId(SiticoneTextBox txtId, string table, string column, string prefix, string indexUser)
        {
            try
            {
                // Only generate Txtid if it's empty (new agent)
                if (string.IsNullOrWhiteSpace(txtId.Text))
                {
                    // Generate unique Id using the administration service
                    var adminService = new Administrations();
                    string newId = adminService.GenerateId(table, column, prefix,indexUser);

                    if (!string.IsNullOrEmpty(newId))
                    {
                        // Display the generated Id and lock the field
                        txtId.Text = newId;
                        txtId.ReadOnly = true;
                       // txtId.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la génération de l'Id. Veuillez réessayer.",
                            "Erreur de génération", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la génération automatique de l'Id: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string GenerateLocalId(IDbConnection conn, string table, string column, string prefix, string userIndex)
        {
            var year = DateTime.Now.Year.ToString();
            var query = $"SELECT MAX(CAST(SUBSTR({column}, LENGTH('{prefix}{userIndex}') + 1, 10) AS INTEGER)) FROM {table} WHERE {column} LIKE '{prefix}{userIndex}%'";
            
            var lastNum = conn.ExecuteScalar<int?>(query) ?? 0;
            var nextNum = lastNum + 1;
            var radical = nextNum.ToString().PadLeft(10, '0');
            
            return $"{prefix}{userIndex}{radical}{year}";
        }

        #endregion

        #region CRUD Promotions

        public bool CreatePromotion(string codPromo, string intitule, string codSect, string codOption)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    var query = @"INSERT INTO t_promotions (cod_promo, description, cod_opt) 
                                  VALUES (@CodPromo, @Description, @CodOpt)";
                    
                    conn.Execute(query, new { CodPromo = codPromo, Description = intitule, CodOpt = codOption });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création de la promotion: {ex.Message}", ex);
            }
        }

        public dynamic? GetPromotion(string codPromo)
        {
            using (var conn = GetSecureConnection())
            {
                var query = "SELECT * FROM t_promotions WHERE cod_promo = @CodPromo";
                return conn.QueryFirstOrDefault(query, new { CodPromo = codPromo });
            }
        }

        public IEnumerable<dynamic> GetAllPromotions()
        {
            using (var conn = GetSecureConnection())
            {
                return conn.Query("SELECT * FROM t_promotions");
            }
        }

        public bool UpdatePromotion(string codPromo, string intitule, string codSect, string codOption)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    var query = @"UPDATE t_promotions 
                                  SET description = @Description, cod_opt = @CodOpt 
                                  WHERE cod_promo = @CodPromo";
                    
                    conn.Execute(query, new { CodPromo = codPromo, Description = intitule, CodOpt = codOption });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour: {ex.Message}", ex);
            }
        }

        public bool DeletePromotion(string codPromo)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    var query = "DELETE FROM t_promotions WHERE cod_promo = @CodPromo";
                    conn.Execute(query, new { CodPromo = codPromo });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression: {ex.Message}", ex);
            }
        }

        #endregion

        #region CRUD Options

        public bool CreateOption(string codOption, string intitule, string codSect, string? codeEpst = null)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    var query = @"INSERT INTO t_options (cod_opt, description, cod_sect, code_epst) 
                                  VALUES (@CodOpt, @Description, @CodSect, @CodeEpst)";
                    
                    conn.Execute(query, new { CodOpt = codOption, Description = intitule, CodSect = codSect, CodeEpst = codeEpst });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public IEnumerable<dynamic> GetAllOptions()
        {
            using (var conn = GetSecureConnection())
            {
                return conn.Query("SELECT * FROM t_options");
            }
        }

        /// <summary>
        /// Récupère les options disponibles depuis t_source_options qui ne sont pas encore dans t_options
        /// </summary>
        /// <returns>Liste des options disponibles</returns>
        public IEnumerable<dynamic> GetAvailableSourceOptions()
        {
            using (var conn = GetSecureConnection())
            {
                var query = @"SELECT Code_Epst, OptionName 
                              FROM t_source_options 
                              WHERE OptionName NOT IN (SELECT description FROM t_options)
                              ORDER BY OptionName";
                return conn.Query(query);
            }
        }

        public bool UpdateOption(string codOption, string intitule, string codSect)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    var query = @"UPDATE t_options 
                                  SET description = @Description, cod_sect = @CodSect 
                                  WHERE cod_opt = @CodOpt";
                    
                    conn.Execute(query, new { CodOpt = codOption, Description = intitule, CodSect = codSect });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public bool DeleteOption(string codOption)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    conn.Execute("DELETE FROM t_options WHERE cod_opt = @CodOpt", new { CodOpt = codOption });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        #endregion

        #region CRUD Sections

        public bool CreateSection(string codSect, string intitule)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    var query = "INSERT INTO t_sections (cod_sect, description) VALUES (@CodSect, @Description)";
                    conn.Execute(query, new { CodSect = codSect, Description = intitule });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public IEnumerable<dynamic> GetAllSections()
        {
            using (var conn = GetSecureConnection())
            {
                return conn.Query("SELECT * FROM t_sections");
            }
        }

        public bool UpdateSection(string codSect, string intitule)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    var query = "UPDATE t_sections SET description = @Description WHERE cod_sect = @CodSect";
                    conn.Execute(query, new { CodSect = codSect, Description = intitule });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public bool DeleteSection(string codSect)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    conn.Execute("DELETE FROM t_sections WHERE cod_sect = @CodSect", new { CodSect = codSect });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        #endregion

        #region CRUD Affectation Section

        public bool CreateAffectSection(string idEcole, string codSect)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    // Vérifier si cette section est déjà affectée à cette école
                    var checkQuery = "SELECT COUNT(*) FROM t_affect_sect WHERE id_ecole = @IdEcole AND cod_sect = @CodSect";
                    var exists = conn.ExecuteScalar<int>(checkQuery, new { IdEcole = idEcole, CodSect = codSect });
                    
                    if (exists > 0)
                    {
                        throw new InvalidOperationException("Cette section est déjà affectée à cette école.");
                    }
                    
                    var query = "INSERT INTO t_affect_sect (id_ecole, cod_sect) VALUES (@IdEcole, @CodSect)";
                    conn.Execute(query, new { IdEcole = idEcole, CodSect = codSect });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public IEnumerable<dynamic> GetAffectSectionsByEcole(string idEcole)
        {
            using (var conn = GetSecureConnection())
            {
                var query = @"SELECT a.*, s.description as section_description 
                              FROM t_affect_sect a 
                              INNER JOIN t_sections s ON a.cod_sect = s.cod_sect 
                              WHERE a.id_ecole = @IdEcole";
                return conn.Query(query, new { IdEcole = idEcole });
            }
        }

        public bool DeleteAffectSection(int numAffect)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    conn.Execute("DELETE FROM t_affect_sect WHERE num_affect = @NumAffect", new { NumAffect = numAffect });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        #endregion

        #region CRUD Affectation Élève avec Isolation

        /// <summary>
        /// Crée une nouvelle affectation avec isolation automatique par école
        /// </summary>
        public bool CreateAffectation(string matricule, string codPromo, string anneeScol, string indicePromo)
        {
            return ExecuteWithErrorHandling(() =>
            {
                using (var conn = GetSecureConnection())
                {
                    // Note: t_affectation n'a pas de colonnes created_at/updated_at
                    // donc on fait une insertion directe sans utiliser InsertWithIsolation
                    var query = @"INSERT INTO t_affectation (matricule, cod_promo, annee_scol, indice_promo) 
                                  VALUES (@matricule, @cod_promo, @annee_scol, @indice_promo)";
                    
                    var parameters = new
                    {
                        matricule = matricule,
                        cod_promo = codPromo,
                        annee_scol = anneeScol,
                        indice_promo = indicePromo
                    };
                    
                    var result = conn.Execute(query, parameters);
                    return result > 0;
                }
            }, "CreateAffectation");
        }

        /// <summary>
        /// Récupère les affectations par promotion avec isolation automatique
        /// </summary>
        public IEnumerable<dynamic> GetAffectationsByPromotion(string codPromo, string anneeScol)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT a.*, e.nom, e.postnom, e.prenom, e.sexe 
                              FROM t_affectation a 
                              INNER JOIN t_eleves e ON a.matricule = e.matricule 
                              WHERE a.cod_promo = @CodPromo AND a.annee_scol = @AnneeScol";
                
                return QueryWithIsolation(query, new { CodPromo = codPromo, AnneeScol = anneeScol }, "a");
            }, "GetAffectationsByPromotion");
        }

        /// <summary>
        /// Supprime une affectation avec isolation automatique
        /// </summary>
        public bool DeleteAffectation(int idAffect)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var result = DeleteWithIsolation("t_affectation", "id_affect = @IdAffect", new { IdAffect = idAffect });
                return result > 0;
            }, "DeleteAffectation");
        }

        #endregion

        #region CRUD Écoles Étendues

        public bool CreateEcole(string idEcole, string denomination, string anneeScol, string fkAvenue, string numero, string? logo = null)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    var query = @"INSERT INTO t_ecoles (id_ecole, denomination, FkAvenue, numero, logo) 
                                  VALUES (@IdEcole, @Denomination, @FkAvenue, @Numero, @Logo)";
                    
                    conn.Execute(query, new 
                    { 
                        IdEcole = idEcole, 
                        Denomination = denomination, 
                        FkAvenue = fkAvenue,
                        Numero = numero,
                        Logo = logo
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public dynamic? GetEcole(string idEcole)
        {
            using (var conn = GetSecureConnection())
            {
                var query = "SELECT * FROM t_ecoles WHERE id_ecole = @IdEcole";
                return conn.QueryFirstOrDefault(query, new { IdEcole = idEcole });
            }
        }

        /// <summary>
        /// Récupère les données d'une école pour l'initialisation du contexte
        /// Cette méthode bypasse la vérification de SchoolContext car elle est utilisée pour l'initialiser
        /// </summary>
        public dynamic? GetEcoleForInitialization(string idEcole)
        {
            using (var conn = _connexion.GetConnection())
            {
                conn.Open();
                var query = "SELECT * FROM t_ecoles WHERE id_ecole = @IdEcole";
                return conn.QueryFirstOrDefault(query, new { IdEcole = idEcole });
            }
        }

        public IEnumerable<dynamic> GetAllEcoles()
        {
            using (var conn = GetSecureConnection())
            {
                return conn.Query("SELECT * FROM t_ecoles ORDER BY denomination");
            }
        }

        /// <summary>
        /// Récupère toutes les écoles actives (extension de GetAllEcoles)
        /// </summary>
        public IEnumerable<dynamic> GetActiveSchools()
        {
            using (var conn = GetSecureConnection())
            {
                // Assumant qu'il pourrait y avoir un champ 'actif' ou similaire dans le futur
                return conn.Query("SELECT * FROM t_ecoles ORDER BY denomination");
            }
        }

        public bool UpdateEcole(string idEcole, string denomination, string anneeScol, string fkAvenue, string numero, string? logo = null)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    var query = @"UPDATE t_ecoles 
                                  SET denomination = @Denomination, 
                                      FkAvenue = @FkAvenue, numero = @Numero, logo = @Logo 
                                  WHERE id_ecole = @IdEcole";
                    
                    conn.Execute(query, new 
                    { 
                        IdEcole = idEcole, 
                        Denomination = denomination, 
                        FkAvenue = fkAvenue,
                        Numero = numero,
                        Logo = logo
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Désactive une école (suppression logique)
        /// Note: Nécessite l'ajout d'un champ 'actif' à la table t_ecoles
        /// </summary>
        public bool DeactivateSchool(string schoolId)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    // Pour l'instant, on ne peut pas vraiment désactiver sans champ 'actif'
                    // Cette méthode est préparée pour une future évolution de la structure
                    var query = "SELECT COUNT(*) FROM t_ecoles WHERE id_ecole = @IdEcole";
                    var exists = conn.ExecuteScalar<int>(query, new { IdEcole = schoolId });
                    
                    if (exists == 0)
                    {
                        throw new InvalidOperationException($"École '{schoolId}' non trouvée.");
                    }
                    
                    // TODO: Implémenter la désactivation quand le champ sera ajouté
                    // UPDATE t_ecoles SET actif = 0 WHERE id_ecole = @IdEcole
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la désactivation de l'école: {ex.Message}", ex);
            }
        }

        #endregion

        #region CRUD Entité Administrative

        public bool CreateEntiteAdministrative(string idEntite, string intituleEntite, string? denominationHabitant, 
                                               string? fkEntiteMere, string? fkTypeEntite)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    var query = @"INSERT INTO t_entite_administrative 
                                  (IdEntite, IntituleEntite, DenominationHabitant, Fk_EntiteMere, Fk_TypeEntite) 
                                  VALUES (@IdEntite, @IntituleEntite, @DenominationHabitant, @FkEntiteMere, @FkTypeEntite)";
                    
                    conn.Execute(query, new 
                    { 
                        IdEntite = idEntite,
                        IntituleEntite = intituleEntite,
                        DenominationHabitant = denominationHabitant,
                        FkEntiteMere = fkEntiteMere,
                        FkTypeEntite = fkTypeEntite
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public IEnumerable<dynamic> GetEntitesAdministratives()
        {
            using (var conn = GetSecureConnection())
            {
                return conn.Query("SELECT * FROM t_entite_administrative WHERE Etat = 1");
            }
        }

        public IEnumerable<dynamic> GetEntitesByParent(string fkEntiteMere)
        {
            using (var conn = GetSecureConnection())
            {
                var query = "SELECT * FROM t_entite_administrative WHERE Fk_EntiteMere = @FkEntiteMere AND Etat = 1";
                return conn.Query(query, new { FkEntiteMere = fkEntiteMere });
            }
        }


        /// <summary>
        /// Prompts user for street number
        /// </summary>
        public string? PromptForNumero(string txtNumParcelle, string _numParcelle)
        {

            // Simple input dialog for numero
            if (txtNumParcelle.Trim() != "")
            {
                _numParcelle = txtNumParcelle.Trim();
                return _numParcelle;
            }
            else
            {
                return null;
            }
        }

        public string GetAvenueIdFromAddress(string avenue, string quartier, string commune, string ville, string province)
        {
            try
            {
                using (var conn = Connexion.Instance.GetConnection())
                {
                    var query = @"
                        SELECT IdEntite 
                        FROM t_entite_administrative 
                        WHERE IntituleEntite = @Avenue 
                        AND Fk_EntiteMere IN (
                            SELECT IdEntite FROM t_entite_administrative WHERE IntituleEntite = @Quartier
                        )
                        LIMIT 1";

                    var result = conn.QueryFirstOrDefault<string>(query, new { Avenue = avenue, Quartier = quartier });
                    return result ?? "";
                }
            }
            catch (Exception ex)
            {
                // Log error and return empty string
                System.Diagnostics.Debug.WriteLine($"Error getting avenue ID: {ex.Message}");
                return "";
            }
        }

        public void GetAdressePersonne(string NumParcelle, string _selectedIdAvenue,string Adresse, Form me)
        {
            try
            {
                using (var addressForm = new FormAddressSearch())
                {
                    if (addressForm.ShowDialog(me) == DialogResult.OK)
                    {
                        // Get the selected address information directly from the form
                        var selectedAvenue = addressForm.SelectedAvenue;
                        var selectedQuartier = addressForm.SelectedQuartier;
                        var selectedCommune = addressForm.SelectedCommune;
                        var selectedVille = addressForm.SelectedVille;
                        var selectedProvince = addressForm.SelectedProvince;

                        // Get the IdAvenue from the database based on the selected address components
                        var idAvenue = GetAvenueIdFromAddress(selectedAvenue, selectedQuartier, selectedCommune, selectedVille, selectedProvince);

                        // Store the IdAvenue for database recording
                        _selectedIdAvenue = idAvenue;

                        // Get the numero from TxtNumParcelleEleve if it exists
                        var numero = NumParcelle.Trim();

                        // Build the complete address string
                        var fullAddress = addressForm.GetFullAddress();
                        if (!string.IsNullOrEmpty(numero))
                        {
                            fullAddress = $"{numero}, {fullAddress}";
                        }

                        // Display the complete address in TxtAdresseEleve
                        Adresse = fullAddress;

                        // Provide user feedback
                        MessageBox.Show("Adresse sélectionnée avec succès!",
                            "Adresse", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sélection d'adresse: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Statistiques et Rapports

        /// <summary>
        /// Récupère les statistiques d'une école
        /// </summary>
        public dynamic? GetSchoolStatistics(string schoolId)
        {
            using (var conn = GetSecureConnection())
            {
                var query = @"SELECT 
                                ec.id_ecole as Id,
                                ec.denomination as Denomination,
                                ec.annee_scol as AnneeScolaire,
                                COUNT(DISTINCT a.id_affect) as TotalAffectations,
                                COUNT(DISTINCT o.id_orientation) as TotalOrientations
                              FROM t_ecoles ec
                              LEFT JOIN t_affectation a ON ec.id_ecole = a.id_ecole
                              LEFT JOIN t_orientation o ON ec.id_ecole = o.id_ecole
                              WHERE ec.id_ecole = @SchoolId
                              GROUP BY ec.id_ecole, ec.denomination, ec.annee_scol";
                
                return conn.QueryFirstOrDefault(query, new { SchoolId = schoolId });
            }
        }

        /// <summary>
        /// Récupère les statistiques de toutes les écoles
        /// </summary>
        public IEnumerable<dynamic> GetAllSchoolsStatistics()
        {
            using (var conn = GetSecureConnection())
            {
                var query = @"SELECT 
                                ec.id_ecole as Id,
                                ec.denomination as Denomination,
                                ec.annee_scol as AnneeScolaire,
                                COUNT(DISTINCT a.id_affect) as TotalAffectations,
                                COUNT(DISTINCT o.id_orientation) as TotalOrientations
                              FROM t_ecoles ec
                              LEFT JOIN t_affectation a ON ec.id_ecole = a.id_ecole
                              LEFT JOIN t_orientation o ON ec.id_ecole = o.id_ecole
                              GROUP BY ec.id_ecole, ec.denomination, ec.annee_scol
                              ORDER BY ec.denomination";
                
                return conn.Query(query);
            }
        }

        /// <summary>
        /// Vérifie l'intégrité de l'isolation des données
        /// </summary>
        public IEnumerable<dynamic> ValidateDataIsolation()
        {
            using (var conn = GetSecureConnection())
            {
                var query = @"SELECT 
                                't_affectation' as TableName,
                                a.id_ecole,
                                ec.denomination as EcoleName,
                                COUNT(*) as RecordCount,
                                CASE WHEN ec.id_ecole IS NULL THEN 1 ELSE 0 END as HasOrphanedData
                              FROM t_affectation a
                              LEFT JOIN t_ecoles ec ON a.id_ecole = ec.id_ecole
                              GROUP BY a.id_ecole, ec.denomination
                              
                              UNION ALL
                              
                              SELECT 
                                't_orientation' as TableName,
                                o.id_ecole,
                                ec.denomination as EcoleName,
                                COUNT(*) as RecordCount,
                                CASE WHEN ec.id_ecole IS NULL THEN 1 ELSE 0 END as HasOrphanedData
                              FROM t_orientation o
                              LEFT JOIN t_ecoles ec ON o.id_ecole = ec.id_ecole
                              GROUP BY o.id_ecole, ec.denomination
                              
                              ORDER BY TableName, id_ecole";
                
                return conn.Query(query);
            }
        }

        #endregion

        #region Utilitaires de Migration

        /// <summary>
        /// Migre les données existantes vers une école spécifique
        /// </summary>
        public bool MigrateDataToSchool(string schoolId, string? sourceSchoolId = null)
        {
            try
            {
                if (!SchoolExists(schoolId))
                {
                    throw new InvalidOperationException($"École de destination '{schoolId}' non trouvée.");
                }

                using (var conn = GetSecureConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Migrer les affectations
                            var updateAffectationsQuery = sourceSchoolId == null 
                                ? "UPDATE t_affectation SET id_ecole = @SchoolId WHERE id_ecole IS NULL OR id_ecole = ''"
                                : "UPDATE t_affectation SET id_ecole = @SchoolId WHERE id_ecole = @SourceSchoolId";
                            
                            conn.Execute(updateAffectationsQuery, new { SchoolId = schoolId, SourceSchoolId = sourceSchoolId }, transaction);

                            // Migrer les orientations
                            var updateOrientationsQuery = sourceSchoolId == null 
                                ? "UPDATE t_orientation SET id_ecole = @SchoolId WHERE id_ecole IS NULL OR id_ecole = ''"
                                : "UPDATE t_orientation SET id_ecole = @SchoolId WHERE id_ecole = @SourceSchoolId";
                            
                            conn.Execute(updateOrientationsQuery, new { SchoolId = schoolId, SourceSchoolId = sourceSchoolId }, transaction);

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
                throw new Exception($"Erreur lors de la migration des données: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Teste l'isolation des données pour une école
        /// </summary>
        public bool TestSchoolIsolation(string schoolId)
        {
            try
            {
                // Initialiser le contexte
                InitializeSchoolContext(schoolId);

                // Vérifier l'isolation des affectations
                using (var conn = GetSecureConnection())
                {
                    var query = @"SELECT COUNT(*) FROM t_affectation a
                                  LEFT JOIN t_ecoles e ON a.id_ecole = e.id_ecole 
                                  WHERE a.id_ecole != @SchoolId OR e.id_ecole IS NULL";
                    var wrongSchoolCount = conn.ExecuteScalar<int>(query, new { SchoolId = schoolId });
                    
                    // Si des données d'autres écoles sont présentes, l'isolation ne fonctionne pas
                    return wrongSchoolCount == 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du test d'isolation: {ex.Message}", ex);
            }
        }

        #endregion

        #region CRUD Orientations

        public bool CreateOrientation(string codOrientation, string description)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    var query = "INSERT INTO t_orientations (cod_orientation, description) VALUES (@CodOrientation, @Description)";
                    conn.Execute(query, new { CodOrientation = codOrientation, Description = description });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public IEnumerable<dynamic> GetAllOrientations()
        {
            using (var conn = GetSecureConnection())
            {
                return conn.Query("SELECT * FROM t_orientations ORDER BY description");
            }
        }

        public dynamic? GetOrientation(string codOrientation)
        {
            using (var conn = GetSecureConnection())
            {
                var query = "SELECT * FROM t_orientations WHERE cod_orientation = @CodOrientation";
                return conn.QueryFirstOrDefault(query, new { CodOrientation = codOrientation });
            }
        }

        public bool UpdateOrientation(string codOrientation, string description)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    var query = "UPDATE t_orientations SET description = @Description WHERE cod_orientation = @CodOrientation";
                    conn.Execute(query, new { CodOrientation = codOrientation, Description = description });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        public bool DeleteOrientation(string codOrientation)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    conn.Execute("DELETE FROM t_orientations WHERE cod_orientation = @CodOrientation", new { CodOrientation = codOrientation });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur: {ex.Message}", ex);
            }
        }

        #endregion

        #region CRUD Services

        /// <summary>
        /// Crée un nouveau service
        /// </summary>
        public bool CreateService(string idService, string description)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    var query = "INSERT INTO t_services (id_service, description) VALUES (@IdService, @Description)";
                    conn.Execute(query, new { IdService = idService, Description = description });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création du service: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Récupère tous les services
        /// </summary>
        public IEnumerable<dynamic> GetAllServices()
        {
            using (var conn = GetSecureConnection())
            {
                return conn.Query("SELECT * FROM t_services ORDER BY description");
            }
        }

        /// <summary>
        /// Récupère un service par ID
        /// </summary>
        public dynamic? GetService(string idService)
        {
            using (var conn = GetSecureConnection())
            {
                return conn.QueryFirstOrDefault("SELECT * FROM t_services WHERE id_service = @IdService", 
                    new { IdService = idService });
            }
        }

        /// <summary>
        /// Met à jour un service
        /// </summary>
        public bool UpdateService(string idService, string description)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    var query = "UPDATE t_services SET description = @Description WHERE id_service = @IdService";
                    conn.Execute(query, new { IdService = idService, Description = description });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour du service: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Supprime un service
        /// </summary>
        public bool DeleteService(string idService)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    conn.Execute("DELETE FROM t_services WHERE id_service = @IdService", new { IdService = idService });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression du service: {ex.Message}", ex);
            }
        }

        #endregion

        #region CRUD Grades

        /// <summary>
        /// Crée un nouveau grade
        /// </summary>
        public bool CreateGrade(string idGrade, string codeGrade, string sigle, string libelleGrade)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    var query = @"INSERT INTO t_grade (id_grade, code_grade, sigle, libelle_grade) 
                                  VALUES (@IdGrade, @CodeGrade, @Sigle, @LibelleGrade)";
                    conn.Execute(query, new 
                    { 
                        IdGrade = idGrade, 
                        CodeGrade = codeGrade, 
                        Sigle = sigle, 
                        LibelleGrade = libelleGrade 
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création du grade: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Récupère tous les grades
        /// </summary>
        public IEnumerable<dynamic> GetAllGrades()
        {
            using (var conn = GetSecureConnection())
            {
                return conn.Query("SELECT * FROM t_grade ORDER BY libelle_grade");
            }
        }

        /// <summary>
        /// Récupère un grade par ID
        /// </summary>
        public dynamic? GetGrade(string idGrade)
        {
            using (var conn = GetSecureConnection())
            {
                return conn.QueryFirstOrDefault("SELECT * FROM t_grade WHERE id_grade = @IdGrade", 
                    new { IdGrade = idGrade });
            }
        }

        #endregion

        #region CRUD Rôles (pour affectation)

        /// <summary>
        /// Récupère tous les rôles disponibles
        /// </summary>
        public IEnumerable<dynamic> GetAllRoles()
        {
            using (var conn = GetSecureConnection())
            {
                return conn.Query("SELECT * FROM t_roles WHERE etat = 1 ORDER BY nom_role");
            }
        }

        /// <summary>
        /// Récupère un rôle par ID
        /// </summary>
        public dynamic? GetRole(string idRole)
        {
            using (var conn = GetSecureConnection())
            {
                return conn.QueryFirstOrDefault("SELECT * FROM t_roles WHERE id_role = @IdRole", 
                    new { IdRole = idRole });
            }
        }

        #endregion

        #region Affectations Agent - Service

        /// <summary>
        /// Affecte un agent à un service
        /// </summary>
        public bool AffectAgentToService(string matriculeAgent, string idService, DateTime dateAffect)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    // Vérifier si l'affectation existe déjà
                    var checkQuery = @"SELECT COUNT(*) FROM t_service_agent 
                                      WHERE fk_agent = @Agent AND fk_service = @Service AND date_affect = @Date";
                    var exists = conn.ExecuteScalar<int>(checkQuery, 
                        new { Agent = matriculeAgent, Service = idService, Date = dateAffect });

                    if (exists > 0)
                    {
                        throw new InvalidOperationException("Cette affectation existe déjà.");
                    }

                    var query = @"INSERT INTO t_service_agent (fk_service, fk_agent, date_affect) 
                                  VALUES (@Service, @Agent, @Date)";
                    conn.Execute(query, new 
                    { 
                        Service = idService, 
                        Agent = matriculeAgent, 
                        Date = dateAffect 
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'affectation de l'agent au service: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Récupère les affectations de services d'un agent
        /// </summary>
        public IEnumerable<dynamic> GetAgentServiceAffectations(string matriculeAgent)
        {
            using (var conn = GetSecureConnection())
            {
                var query = @"SELECT sa.num_affect, sa.fk_service, sa.fk_agent, sa.date_affect,
                                     s.description as service_description
                              FROM t_service_agent sa
                              INNER JOIN t_services s ON sa.fk_service = s.id_service
                              WHERE sa.fk_agent = @Agent
                              ORDER BY sa.date_affect DESC";
                return conn.Query(query, new { Agent = matriculeAgent });
            }
        }

        /// <summary>
        /// Récupère la dernière affectation de service d'un agent
        /// </summary>
        public dynamic? GetAgentCurrentService(string matriculeAgent)
        {
            using (var conn = GetSecureConnection())
            {
                var query = @"SELECT sa.num_affect, sa.fk_service, sa.fk_agent, sa.date_affect,
                                     s.description as service_description
                              FROM t_service_agent sa
                              INNER JOIN t_services s ON sa.fk_service = s.id_service
                              WHERE sa.fk_agent = @Agent
                              ORDER BY sa.date_affect DESC
                              LIMIT 1";
                return conn.QueryFirstOrDefault(query, new { Agent = matriculeAgent });
            }
        }

        /// <summary>
        /// Supprime une affectation de service
        /// </summary>
        public bool DeleteServiceAffectation(int numAffect)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    conn.Execute("DELETE FROM t_service_agent WHERE num_affect = @NumAffect", 
                        new { NumAffect = numAffect });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression de l'affectation: {ex.Message}", ex);
            }
        }

        #endregion

        #region Affectations Agent - Grade

        /// <summary>
        /// Affecte un grade à un agent
        /// </summary>
        public bool AffectAgentToGrade(string matriculeAgent, string idGrade, DateTime dateAffect)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    // Vérifier si l'affectation existe déjà
                    var checkQuery = @"SELECT COUNT(*) FROM t_grade_agent 
                                      WHERE fk_agent = @Agent AND fk_grade = @Grade AND date_affect = @Date";
                    var exists = conn.ExecuteScalar<int>(checkQuery, 
                        new { Agent = matriculeAgent, Grade = idGrade, Date = dateAffect });

                    if (exists > 0)
                    {
                        throw new InvalidOperationException("Cette affectation existe déjà.");
                    }

                    var query = @"INSERT INTO t_grade_agent (fk_grade, fk_agent, date_affect) 
                                  VALUES (@Grade, @Agent, @Date)";
                    conn.Execute(query, new 
                    { 
                        Grade = idGrade, 
                        Agent = matriculeAgent, 
                        Date = dateAffect 
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'affectation du grade: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Récupère les affectations de grades d'un agent
        /// </summary>
        public IEnumerable<dynamic> GetAgentGradeAffectations(string matriculeAgent)
        {
            using (var conn = GetSecureConnection())
            {
                var query = @"SELECT ga.num_affect, ga.fk_grade, ga.fk_agent, ga.date_affect,
                                     g.code_grade, g.sigle, g.libelle_grade
                              FROM t_grade_agent ga
                              INNER JOIN t_grade g ON ga.fk_grade = g.id_grade
                              WHERE ga.fk_agent = @Agent
                              ORDER BY ga.date_affect DESC";
                return conn.Query(query, new { Agent = matriculeAgent });
            }
        }

        /// <summary>
        /// Récupère le grade actuel d'un agent
        /// </summary>
        public dynamic? GetAgentCurrentGrade(string matriculeAgent)
        {
            using (var conn = GetSecureConnection())
            {
                var query = @"SELECT ga.num_affect, ga.fk_grade, ga.fk_agent, ga.date_affect,
                                     g.code_grade, g.sigle, g.libelle_grade
                              FROM t_grade_agent ga
                              INNER JOIN t_grade g ON ga.fk_grade = g.id_grade
                              WHERE ga.fk_agent = @Agent
                              ORDER BY ga.date_affect DESC
                              LIMIT 1";
                return conn.QueryFirstOrDefault(query, new { Agent = matriculeAgent });
            }
        }

        /// <summary>
        /// Supprime une affectation de grade
        /// </summary>
        public bool DeleteGradeAffectation(int numAffect)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    conn.Execute("DELETE FROM t_grade_agent WHERE num_affect = @NumAffect", 
                        new { NumAffect = numAffect });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression de l'affectation: {ex.Message}", ex);
            }
        }

        #endregion

        #region Affectations Agent - Rôle

        /// <summary>
        /// Affecte un rôle à un agent
        /// </summary>
        public bool AffectAgentToRole(string matriculeAgent, string idRole, DateTime dateAffect)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    // Vérifier si l'affectation existe déjà
                    var checkQuery = @"SELECT COUNT(*) FROM t_roles_agents 
                                      WHERE fk_agent = @Agent AND fk_role = @Role AND date_affect = @Date";
                    var exists = conn.ExecuteScalar<int>(checkQuery, 
                        new { Agent = matriculeAgent, Role = idRole, Date = dateAffect });

                    if (exists > 0)
                    {
                        throw new InvalidOperationException("Cette affectation existe déjà.");
                    }

                    var query = @"INSERT INTO t_roles_agents (fk_role, fk_agent, date_affect) 
                                  VALUES (@Role, @Agent, @Date)";
                    conn.Execute(query, new 
                    { 
                        Role = idRole, 
                        Agent = matriculeAgent, 
                        Date = dateAffect 
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'affectation du rôle: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Récupère les affectations de rôles d'un agent
        /// </summary>
        public IEnumerable<dynamic> GetAgentRoleAffectations(string matriculeAgent)
        {
            using (var conn = GetSecureConnection())
            {
                var query = @"SELECT ra.num_affect, ra.fk_role, ra.fk_agent, ra.date_affect,
                                     r.nom_role, r.description_role, r.niveau_acces
                              FROM t_roles_agents ra
                              INNER JOIN t_roles r ON ra.fk_role = r.id_role
                              WHERE ra.fk_agent = @Agent
                              ORDER BY ra.date_affect DESC";
                return conn.Query(query, new { Agent = matriculeAgent });
            }
        }

        /// <summary>
        /// Récupère le rôle actuel d'un agent
        /// </summary>
        public dynamic? GetAgentCurrentRole(string matriculeAgent)
        {
            using (var conn = GetSecureConnection())
            {
                var query = @"SELECT ra.num_affect, ra.fk_role, ra.fk_agent, ra.date_affect,
                                     r.nom_role, r.description_role, r.niveau_acces
                              FROM t_roles_agents ra
                              INNER JOIN t_roles r ON ra.fk_role = r.id_role
                              WHERE ra.fk_agent = @Agent
                              ORDER BY ra.date_affect DESC
                              LIMIT 1";
                return conn.QueryFirstOrDefault(query, new { Agent = matriculeAgent });
            }
        }

        /// <summary>
        /// Supprime une affectation de rôle
        /// </summary>
        public bool DeleteRoleAffectation(int numAffect)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    conn.Execute("DELETE FROM t_roles_agents WHERE num_affect = @NumAffect", 
                        new { NumAffect = numAffect });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression de l'affectation: {ex.Message}", ex);
            }
        }

        #endregion

        #region Méthodes Combinées d'Affectation

        /// <summary>
        /// Affecte un agent (service, grade et rôle) en une seule transaction
        /// </summary>
        public bool AffectAgentComplete(string matriculeAgent, string? idService, string? idGrade, string? idRole, DateTime dateAffect)
        {
            try
            {
                using (var conn = GetSecureConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Affectation au service si spécifié
                            if (!string.IsNullOrWhiteSpace(idService))
                            {
                                var serviceQuery = @"INSERT INTO t_service_agent (fk_service, fk_agent, date_affect) 
                                                   VALUES (@Service, @Agent, @Date)";
                                conn.Execute(serviceQuery, new { Service = idService, Agent = matriculeAgent, Date = dateAffect }, transaction);
                            }

                            // Affectation au grade si spécifié
                            if (!string.IsNullOrWhiteSpace(idGrade))
                            {
                                var gradeQuery = @"INSERT INTO t_grade_agent (fk_grade, fk_agent, date_affect) 
                                                 VALUES (@Grade, @Agent, @Date)";
                                conn.Execute(gradeQuery, new { Grade = idGrade, Agent = matriculeAgent, Date = dateAffect }, transaction);
                            }

                            // Affectation au rôle si spécifié
                            if (!string.IsNullOrWhiteSpace(idRole))
                            {
                                var roleQuery = @"INSERT INTO t_roles_agents (fk_role, fk_agent, date_affect) 
                                                VALUES (@Role, @Agent, @Date)";
                                conn.Execute(roleQuery, new { Role = idRole, Agent = matriculeAgent, Date = dateAffect }, transaction);
                            }

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
                throw new Exception($"Erreur lors de l'affectation complète de l'agent: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Récupère toutes les affectations d'un agent (service, grade, rôle)
        /// </summary>
        public dynamic? GetAgentCompleteAffectations(string matriculeAgent)
        {
            using (var conn = GetSecureConnection())
            {
                var currentService = GetAgentCurrentService(matriculeAgent);
                var currentGrade = GetAgentCurrentGrade(matriculeAgent);
                var currentRole = GetAgentCurrentRole(matriculeAgent);

                return new
                {
                    Service = currentService,
                    Grade = currentGrade,
                    Role = currentRole
                };
            }
        }

        #endregion
    }
}