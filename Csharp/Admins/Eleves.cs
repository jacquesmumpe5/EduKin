using System.Data;
using Dapper;
using EduKin.DataSets;
using EduKin.Csharp.Admins;
using EduKin.Inits;

namespace EduKin.Csharp.Admins
{
    /// <summary>
    /// Service de gestion des élèves avec isolation automatique par école
    /// </summary>
    public class Eleves : BaseService
    {
        private readonly Administrations _admin;

        public Eleves()
        {
            _admin = new Administrations();
        }

        #region CRUD Élèves avec Isolation par École

        /// <summary>
        /// Crée un nouvel élève avec validation complète et gestion d'erreurs avancée
        /// </summary>
        public bool CreateEleve(string nom, string postNom, string prenom, string sexe, string nomTuteur,
                                string lieuNaiss, string fkAvenue, string numero, string userIndex,
                                DateTime? dateNaiss = null, string? telTuteur = null, 
                                string? ecoleProv = null, string? profil = null)
        {
            return ExecuteWithRetry(() =>
            {
                // Validation des données d'entrée
                var eleveData = new Dictionary<string, object?>
                {
                    { "nom", nom },
                    { "postnom", postNom },
                    { "prenom", prenom },
                    { "sexe", sexe },
                    { "nom_tuteur", nomTuteur },
                    { "lieu_naiss", lieuNaiss },
                    { "FkAvenue", fkAvenue },
                    { "numero", numero },
                    { "date_naiss", dateNaiss },
                    { "tel_tuteur", telTuteur },
                    { "ecole_prov", ecoleProv },
                    { "profil", profil }
                };

                if (!ValidateData("t_eleves", eleveData))
                {
                    throw new ArgumentException("Données d'élève invalides");
                }

                using (var conn = GetSecureConnection())
                {
                    conn.Open();
                    
                    // Générer le matricule unique avec gestion des conflits
                    var matricule = GenerateUniqueMatricule(conn, userIndex);
                    eleveData["matricule"] = matricule;

                    // Ajouter les timestamps
                    AddTimestamps(eleveData, isUpdate: false);

                    // Adapter la requête selon le type de base de données
                    var insertQuery = @"INSERT INTO t_eleves 
                        (matricule, nom, postnom, prenom, sexe, date_naiss, lieu_naiss, nom_tuteur, tel_tuteur, FkAvenue, numero, ecole_prov, profil, created_at, updated_at)
                        VALUES (@matricule, @nom, @postnom, @prenom, @sexe, @date_naiss, @lieu_naiss, @nom_tuteur, @tel_tuteur, @FkAvenue, @numero, @ecole_prov, @profil, @created_at, @updated_at)";

                    var adaptedQuery = SqlCompatibilityAdapter.GetAdaptedQuery(insertQuery, _connexion.IsOnline);
                    var result = conn.Execute(adaptedQuery, eleveData);

                    if (result > 0)
                    {
                        // Log de l'opération réussie
                        LogOperation("CREATE_ELEVE", $"Élève créé: {matricule} - {nom} {postNom} {prenom}");
                        return true;
                    }

                    return false;
                }
            }, "CreateEleve");
        }

        /// <summary>
        /// Génère un matricule unique avec gestion des conflits
        /// Format: ELV{userIndex}{number}{year}
        /// </summary>
        private string GenerateUniqueMatricule(IDbConnection conn, string userIndex)
        {
            var maxAttempts = 10;
            var attempt = 0;

            while (attempt < maxAttempts)
            {
                try
                {
                    var year = DateTime.Now.Year.ToString();
                    
                    // Rechercher le dernier numéro pour ce userIndex et cette année
                    var pattern = $"ELV{userIndex}%{year}";
                    var query = SqlCompatibilityAdapter.GetAdaptedQuery(
                        "SELECT MAX(CAST(SUBSTRING(matricule, LENGTH(@Prefix) + 1, 10) AS UNSIGNED)) FROM t_eleves WHERE matricule LIKE @Pattern",
                        _connexion.IsOnline);

                    var maxNum = conn.QueryFirstOrDefault<int?>(query, new 
                    { 
                        Pattern = pattern,
                        Prefix = $"ELV{userIndex}"
                    }) ?? 0;
                    
                    var newNum = maxNum + attempt + 1;
                    var radical = newNum.ToString().PadLeft(10, '0');
                    var matricule = $"ELV{userIndex}{radical}{year}";

                    // Vérifier l'unicité
                    var existsQuery = "SELECT COUNT(*) FROM t_eleves WHERE matricule = @Matricule";
                    var exists = conn.ExecuteScalar<int>(existsQuery, new { Matricule = matricule });

                    if (exists == 0)
                    {
                        return matricule;
                    }

                    attempt++;
                }
                catch (Exception ex)
                {
                    if (attempt >= maxAttempts - 1)
                    {
                        throw new InvalidOperationException($"Impossible de générer un matricule unique après {maxAttempts} tentatives", ex);
                    }
                    attempt++;
                }
            }

            throw new InvalidOperationException("Impossible de générer un matricule unique");
        }

        /// <summary>
        /// Récupère un élève par son matricule avec isolation
        /// </summary>
        public dynamic? GetEleve(string matricule)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT e.*, 
                              (SELECT IntituleEntite FROM t_entite_administrative WHERE IdEntite = e.FkAvenue) as avenue_nom
                              FROM t_eleves e 
                              WHERE e.matricule = @Matricule";
                
                return QueryFirstOrDefaultWithIsolation(query, new { Matricule = matricule }, "e");
            }, "GetEleve");
        }

        /// <summary>
        /// Récupère tous les élèves de l'école courante via la chaîne d'affectation
        /// Élève → Affectation → Promotion → Option → Section → École
        /// </summary>
        public IEnumerable<dynamic> GetAllEleves()
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT DISTINCT e.*, 
                              a.annee_scol,
                              a.cod_promo,
                              a.indice_promo,
                              p.description as promotion_nom,
                              o.description as option_nom,
                              s.description as section_nom,
                              (SELECT IntituleEntite FROM t_entite_administrative WHERE IdEntite = e.FkAvenue) as avenue_nom
                              FROM t_eleves e 
                              INNER JOIN t_affectation a ON e.matricule = a.matricule
                              INNER JOIN t_promotions p ON a.cod_promo = p.cod_promo
                              INNER JOIN t_options o ON p.cod_opt = o.cod_opt
                              INNER JOIN t_sections s ON o.cod_sect = s.cod_sect
                              INNER JOIN t_affect_sect afs ON s.cod_sect = afs.cod_sect
                              WHERE afs.id_ecole = @IdEcole
                              ORDER BY e.nom, e.prenom";
                
                using (var conn = GetSecureConnection())
                {
                    return conn.Query(query, new { IdEcole = EduKinContext.CurrentIdEcole });
                }
            }, "GetAllEleves");
        }

        /// <summary>
        /// Recherche des élèves par terme avec isolation
        /// </summary>
        public IEnumerable<dynamic> SearchEleves(string searchTerm)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT e.*, 
                              (SELECT IntituleEntite FROM t_entite_administrative WHERE IdEntite = e.FkAvenue) as avenue_nom
                              FROM t_eleves e 
                              WHERE e.nom LIKE @Search OR e.prenom LIKE @Search 
                              OR e.matricule LIKE @Search OR e.nom_tuteur LIKE @Search
                              ORDER BY e.nom";
                
                return QueryWithIsolation(query, new { Search = $"%{searchTerm}%" }, "e");
            }, "SearchEleves");
        }

        /// <summary>
        /// Récupère les élèves par sexe avec isolation
        /// </summary>
        public IEnumerable<dynamic> GetElevesBySexe(string sexe)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT e.*, 
                              (SELECT IntituleEntite FROM t_entite_administrative WHERE IdEntite = e.FkAvenue) as avenue_nom
                              FROM t_eleves e 
                              WHERE e.sexe = @Sexe 
                              ORDER BY e.nom";
                
                return QueryWithIsolation(query, new { Sexe = sexe }, "e");
            }, "GetElevesBySexe");
        }

        /// <summary>
        /// Récupère les élèves par tranche d'âge avec isolation
        /// </summary>
        public IEnumerable<dynamic> GetElevesByAge(int minAge, int maxAge)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT e.*, 
                              (YEAR(CURDATE()) - YEAR(e.date_naiss)) as age,
                              (SELECT IntituleEntite FROM t_entite_administrative WHERE IdEntite = e.FkAvenue) as avenue_nom
                              FROM t_eleves e 
                              WHERE (YEAR(CURDATE()) - YEAR(e.date_naiss)) BETWEEN @MinAge AND @MaxAge
                              ORDER BY e.date_naiss DESC";
                
                return QueryWithIsolation(query, new { MinAge = minAge, MaxAge = maxAge }, "e");
            }, "GetElevesByAge");
        }

        /// <summary>
        /// Met à jour un élève avec isolation
        /// </summary>
        public bool UpdateEleve(string matricule, string nom, string postNom, string prenom, string sexe,
                                string nomTuteur, string lieuNaiss, string fkAvenue, string numero,
                                DateTime? dateNaiss = null, string? telTuteur = null,
                                string? ecoleProv = null, string? profil = null)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var updateData = new
                {
                    nom = nom,
                    postnom = postNom,
                    prenom = prenom,
                    sexe = sexe,
                    date_naiss = dateNaiss,
                    lieu_naiss = lieuNaiss,
                    FkAvenue = fkAvenue,
                    nom_tuteur = nomTuteur,
                    tel_tuteur = telTuteur,
                    ecole_prov = ecoleProv,
                    numero = numero,
                    profil = profil
                };

                var result = UpdateWithIsolation("t_eleves", updateData, "matricule = @Matricule", new { Matricule = matricule });
                return result > 0;
            }, "UpdateEleve");
        }

        /// <summary>
        /// Supprime un élève avec isolation
        /// </summary>
        public bool DeleteEleve(string matricule)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var result = DeleteWithIsolation("t_eleves", "matricule = @Matricule", new { Matricule = matricule });
                return result > 0;
            }, "DeleteEleve");
        }

        #endregion

        #region Gestion des Adresses (Vue Hiérarchique) - Tables Globales

        /// <summary>
        /// Recherche dans la hiérarchie des adresses (table globale, pas d'isolation)
        /// </summary>
        public IEnumerable<dynamic> SearchAvenueHierarchie(string searchTerm)
        {
            using (var conn = GetSecureConnection())
            {
                var query = @"SELECT * FROM vue_avenue_hierarchie 
                              WHERE avenue LIKE @Search 
                              OR quartier LIKE @Search 
                              OR commune LIKE @Search 
                              OR ville LIKE @Search 
                              OR province LIKE @Search
                              ORDER BY province, ville, commune, quartier, avenue
                              LIMIT 50";
                
                return conn.Query(query, new { Search = $"%{searchTerm}%" });
            }
        }

        /// <summary>
        /// Récupère une avenue par ID (table globale, pas d'isolation)
        /// </summary>
        public dynamic? GetAvenueHierarchie(string idAvenue)
        {
            using (var conn = GetSecureConnection())
            {
                var query = "SELECT * FROM vue_avenue_hierarchie WHERE id_avenue = @IdAvenue";
                return conn.QueryFirstOrDefault(query, new { IdAvenue = idAvenue });
            }
        }

        /// <summary>
        /// Récupère les avenues par quartier (table globale, pas d'isolation)
        /// </summary>
        public IEnumerable<dynamic> GetAvenuesByQuartier(string quartier)
        {
            using (var conn = GetSecureConnection())
            {
                var query = @"SELECT * FROM vue_avenue_hierarchie 
                              WHERE quartier = @Quartier 
                              ORDER BY avenue";
                return conn.Query(query, new { Quartier = quartier });
            }
        }

        /// <summary>
        /// Récupère les avenues par commune (table globale, pas d'isolation)
        /// </summary>
        public IEnumerable<dynamic> GetAvenuesByCommune(string commune)
        {
            using (var conn = GetSecureConnection())
            {
                var query = @"SELECT * FROM vue_avenue_hierarchie 
                              WHERE commune = @Commune 
                              ORDER BY quartier, avenue";
                return conn.Query(query, new { Commune = commune });
            }
        }

        /// <summary>
        /// Construit l'adresse complète à partir des références
        /// </summary>
        public string GetAdresseComplete(string? fkAvenue, string? numero)
        {
            if (string.IsNullOrEmpty(fkAvenue))
                return "Adresse non renseignée";

            using (var conn = GetSecureConnection())
            {
                var query = "SELECT * FROM vue_avenue_hierarchie WHERE id_avenue = @IdAvenue";
                var adresse = conn.QueryFirstOrDefault(query, new { IdAvenue = fkAvenue });

                if (adresse == null)
                    return "Adresse non trouvée";

                var numeroStr = !string.IsNullOrEmpty(numero) ? $"N° {numero}, " : "";
                return $"{numeroStr}{adresse.avenue}, {adresse.quartier}, {adresse.commune}, {adresse.ville}, {adresse.province}";
            }
        }

        #endregion

        #region CRUD Palmarès avec Isolation par École

        /// <summary>
        /// Crée un nouveau palmarès avec isolation automatique par école
        /// </summary>
        public bool CreatePalmares(string matricule, string codPromo, string periode, int place, 
                                   decimal moyenne, string anneeScol, string? mention = null)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var palmaresData = new
                {
                    Matricule = matricule,
                    CodPromo = codPromo,
                    Periode = periode,
                    Place = place,
                    Moyenne = moyenne,
                    AnneeScol = anneeScol,
                    Mention = mention
                };

                var result = InsertWithIsolation("t_palmares", palmaresData);
                return result > 0;
            }, "CreatePalmares");
        }

        /// <summary>
        /// Récupère les palmarès d'un élève avec isolation
        /// </summary>
        public IEnumerable<dynamic> GetPalmaresByEleve(string matricule, string anneeScol)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT p.*, e.Nom, e.Prenom, pr.intitule as promotion 
                              FROM t_palmares p 
                              INNER JOIN eleves e ON p.Matricule = e.Matricule 
                              INNER JOIN promotions pr ON p.CodPromo = pr.Code 
                              WHERE p.Matricule = @Matricule AND p.AnneeScol = @AnneeScol 
                              ORDER BY p.Periode";
                
                return QueryWithIsolation(query, new { Matricule = matricule, AnneeScol = anneeScol }, "p");
            }, "GetPalmaresByEleve");
        }

        /// <summary>
        /// Récupère les palmarès par promotion avec isolation
        /// </summary>
        public IEnumerable<dynamic> GetPalmaresByPromotion(string codPromo, string periode, string anneeScol, int? topN = null)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT p.*, e.Nom, e.Prenom 
                              FROM t_palmares p 
                              INNER JOIN eleves e ON p.Matricule = e.Matricule 
                              WHERE p.CodPromo = @CodPromo AND p.Periode = @Periode AND p.AnneeScol = @AnneeScol 
                              ORDER BY p.Place";
                
                if (topN.HasValue)
                {
                    query += $" LIMIT {topN.Value}";
                }
                
                return QueryWithIsolation(query, new { CodPromo = codPromo, Periode = periode, AnneeScol = anneeScol }, "p");
            }, "GetPalmaresByPromotion");
        }

        /// <summary>
        /// Récupère le top 10 d'une promotion avec isolation
        /// </summary>
        public IEnumerable<dynamic> GetTop10ByPromotion(string codPromo, string periode, string anneeScol)
        {
            return GetPalmaresByPromotion(codPromo, periode, anneeScol, 10);
        }

        /// <summary>
        /// Récupère un palmarès spécifique avec isolation
        /// </summary>
        public dynamic? GetPalmaresEleve(string matricule, string codPromo, string periode, string anneeScol)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT p.*, e.Nom, e.Prenom 
                              FROM t_palmares p 
                              INNER JOIN eleves e ON p.Matricule = e.Matricule 
                              WHERE p.Matricule = @Matricule AND p.CodPromo = @CodPromo 
                              AND p.Periode = @Periode AND p.AnneeScol = @AnneeScol";
                
                return QueryFirstOrDefaultWithIsolation(query, new 
                { 
                    Matricule = matricule,
                    CodPromo = codPromo,
                    Periode = periode,
                    AnneeScol = anneeScol
                }, "p");
            }, "GetPalmaresEleve");
        }

        /// <summary>
        /// Met à jour un palmarès avec isolation
        /// </summary>
        public bool UpdatePalmares(int idPalmares, int place, decimal moyenne, string? mention = null)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var updateData = new
                {
                    Place = place,
                    Moyenne = moyenne,
                    Mention = mention
                };

                var result = UpdateWithIsolation("t_palmares", updateData, "IdPalmares = @IdPalmares", new { IdPalmares = idPalmares });
                return result > 0;
            }, "UpdatePalmares");
        }

        /// <summary>
        /// Supprime un palmarès avec isolation
        /// </summary>
        public bool DeletePalmares(int idPalmares)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var result = DeleteWithIsolation("t_palmares", "IdPalmares = @IdPalmares", new { IdPalmares = idPalmares });
                return result > 0;
            }, "DeletePalmares");
        }

        /// <summary>
        /// Supprime les palmarès d'une période avec isolation
        /// </summary>
        public bool DeletePalmaresByPeriode(string codPromo, string periode, string anneeScol)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var result = DeleteWithIsolation("t_palmares", 
                    "CodPromo = @CodPromo AND Periode = @Periode AND AnneeScol = @AnneeScol", 
                    new { CodPromo = codPromo, Periode = periode, AnneeScol = anneeScol });
                return result > 0;
            }, "DeletePalmaresByPeriode");
        }

        #endregion

        #region Statistiques et Rapports avec Isolation par École

        /// <summary>
        /// Récupère le nombre total d'élèves de l'école courante
        /// </summary>
        public int GetTotalEleves()
        {
            return ExecuteWithErrorHandling(() =>
            {
                return CountWithIsolation("t_eleves");
            }, "GetTotalEleves");
        }

        /// <summary>
        /// Récupère le nombre d'élèves par sexe de l'école courante
        /// </summary>
        public int GetTotalElevesBySexe(string sexe)
        {
            return ExecuteWithErrorHandling(() =>
            {
                return CountWithIsolation("t_eleves", "sexe = @Sexe", new { Sexe = sexe });
            }, "GetTotalElevesBySexe");
        }

        /// <summary>
        /// Récupère les statistiques par sexe de l'école courante
        /// </summary>
        public IEnumerable<dynamic> GetStatistiquesBySexe()
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT sexe, COUNT(*) as total 
                              FROM t_eleves 
                              GROUP BY sexe";
                
                return QueryWithIsolation(query);
            }, "GetStatistiquesBySexe");
        }

        /// <summary>
        /// Récupère les élèves d'une promotion avec isolation
        /// </summary>
        public IEnumerable<dynamic> GetElevesByPromotion(string codPromo, string anneeScol)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT e.*, a.indice_promo,
                              (SELECT IntituleEntite FROM t_entite_administrative WHERE IdEntite = e.FkAvenue) as avenue_nom
                              FROM t_eleves e 
                              INNER JOIN t_affectation a ON e.matricule = a.matricule 
                              WHERE a.cod_promo = @CodPromo AND a.annee_scol = @AnneeScol 
                              ORDER BY e.nom, e.prenom";
                
                return QueryWithIsolation(query, new { CodPromo = codPromo, AnneeScol = anneeScol }, "e");
            }, "GetElevesByPromotion");
        }

        /// <summary>
        /// Récupère l'effectif d'une promotion avec isolation
        /// </summary>
        public int GetEffectifPromotion(string codPromo, string anneeScol)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT COUNT(*) FROM t_affectation a
                              INNER JOIN t_eleves e ON a.matricule = e.matricule
                              WHERE a.cod_promo = @CodPromo AND a.annee_scol = @AnneeScol";
                
                var isolatedQuery = EduKinContext.AddIsolationClause(query, "e");
                var combinedParameters = CombineParameters(
                    new { CodPromo = codPromo, AnneeScol = anneeScol }, 
                    EduKinContext.GetIsolationParameters()
                );
                
                using (var conn = GetSecureConnection())
                {
                    return conn.ExecuteScalar<int>(isolatedQuery, combinedParameters);
                }
            }, "GetEffectifPromotion");
        }

        /// <summary>
        /// Récupère les statistiques par promotion avec isolation
        /// </summary>
        public IEnumerable<dynamic> GetStatistiquesParPromotion(string anneeScol)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT p.cod_promo, p.description as intitule, 
                              COUNT(a.matricule) as effectif,
                              SUM(CASE WHEN e.sexe = 'M' THEN 1 ELSE 0 END) as garcons,
                              SUM(CASE WHEN e.sexe = 'F' THEN 1 ELSE 0 END) as filles
                              FROM t_promotions p 
                              LEFT JOIN t_affectation a ON p.cod_promo = a.cod_promo AND a.annee_scol = @AnneeScol
                              LEFT JOIN t_eleves e ON a.matricule = e.matricule
                              GROUP BY p.cod_promo, p.description
                              ORDER BY p.description";
                
                var isolatedQuery = EduKinContext.AddIsolationClause(query, "e");
                var combinedParameters = CombineParameters(
                    new { AnneeScol = anneeScol }, 
                    EduKinContext.GetIsolationParameters()
                );
                
                using (var conn = GetSecureConnection())
                {
                    return conn.Query(isolatedQuery, combinedParameters);
                }
            }, "GetStatistiquesParPromotion");
        }

        /// <summary>
        /// Récupère les meilleurs élèves de l'école courante
        /// </summary>
        public IEnumerable<dynamic> GetMeilleursEleves(string anneeScol, int topN = 10)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT e.matricule, e.nom, e.prenom, 
                              AVG(p.Moyenne) as moyenne_generale, 
                              COUNT(p.IdPalmares) as nb_periodes,
                              pr.description as promotion
                              FROM t_eleves e
                              INNER JOIN t_palmares p ON e.matricule = p.Matricule
                              INNER JOIN t_promotions pr ON p.CodPromo = pr.cod_promo
                              WHERE p.AnneeScol = @AnneeScol
                              GROUP BY e.matricule, e.nom, e.prenom, pr.description
                              ORDER BY moyenne_generale DESC
                              LIMIT @TopN";
                
                return QueryWithIsolation(query, new { AnneeScol = anneeScol, TopN = topN }, "e");
            }, "GetMeilleursEleves");
        }

        /// <summary>
        /// Récupère les informations de l'école courante
        /// </summary>
        public dynamic? GetEcoleInfo()
        {
            return ExecuteWithErrorHandling(() =>
            {
                using (var conn = GetSecureConnection())
                {
                    var query = "SELECT * FROM t_ecoles WHERE id_ecole = @IdEcole";
                    return conn.QueryFirstOrDefault(query, new { IdEcole = EduKinContext.CurrentIdEcole });
                }
            }, "GetEcoleInfo");
        }

        /// <summary>
        /// Valide qu'un élève appartient à l'école courante
        /// </summary>
        public bool ValidateEleveOwnership(string matricule)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var count = CountWithIsolation("t_eleves", "matricule = @Matricule", new { Matricule = matricule });
                return count > 0;
            }, "ValidateEleveOwnership");
        }

        #endregion

        #region Méthodes Avancées de Gestion et Monitoring

        /// <summary>
        /// Log une opération avec détails complets
        /// </summary>
        private void LogOperation(string operationType, string details)
        {
            try
            {
                var logEntry = new
                {
                    OperationType = operationType,
                    Details = details,
                    Timestamp = DateTime.Now,
                    School = EduKinContext.TryGetCurrentDenomination(),
                    DatabaseMode = _connexion.IsOnline ? "MySQL" : "SQLite",
                    User = EduKinContext.IsAuthenticated ? EduKinContext.CurrentUserName : "System"
                };

                System.Diagnostics.Debug.WriteLine($"[ELEVES-{operationType}] {details}");
                
                // Log dans un fichier si nécessaire
                LogToFile("eleves_operations", logEntry);
            }
            catch
            {
                // Ignorer les erreurs de logging
            }
        }

        /// <summary>
        /// Log dans un fichier
        /// </summary>
        private void LogToFile(string logType, object logEntry)
        {
            try
            {
                var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                Directory.CreateDirectory(logDir);
                
                var logFile = Path.Combine(logDir, $"{logType}_{DateTime.Now:yyyy-MM-dd}.log");
                var jsonEntry = System.Text.Json.JsonSerializer.Serialize(logEntry, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                var logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {jsonEntry}{Environment.NewLine}";
                
                File.AppendAllText(logFile, logLine);
            }
            catch
            {
                // Ignorer les erreurs de logging
            }
        }

        /// <summary>
        /// Exécute un diagnostic de santé des données d'élèves
        /// </summary>
        public async Task<EleveDataHealthReport> RunDataHealthCheck()
        {
            var report = new EleveDataHealthReport
            {
                StartTime = DateTime.Now
            };

            try
            {
                using var conn = GetSecureConnection();
                conn.Open();

                // Vérifier les données manquantes
                report.MissingDataIssues = await CheckMissingData(conn);

                // Vérifier les données incohérentes
                report.InconsistentDataIssues = await CheckInconsistentData(conn);

                // Vérifier les doublons
                report.DuplicateIssues = await CheckDuplicates(conn);

                // Statistiques générales
                report.TotalEleves = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM t_eleves");
                report.ElevesWithPhotos = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM t_eleves WHERE profil IS NOT NULL AND profil != ''");
                report.ElevesWithCompleteInfo = await CheckCompleteInfo(conn);

                report.HealthScore = CalculateDataHealthScore(report);
                report.Success = true;
            }
            catch (Exception ex)
            {
                report.Success = false;
                report.Error = ex.Message;
            }

            report.EndTime = DateTime.Now;
            report.Duration = report.EndTime - report.StartTime;

            return report;
        }

        /// <summary>
        /// Vérifie les données manquantes
        /// </summary>
        private async Task<List<string>> CheckMissingData(IDbConnection conn)
        {
            var issues = new List<string>();

            try
            {
                // Élèves sans date de naissance
                var missingBirthDate = await conn.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM t_eleves WHERE date_naiss IS NULL");
                if (missingBirthDate > 0)
                    issues.Add($"{missingBirthDate} élèves sans date de naissance");

                // Élèves sans adresse
                var missingAddress = await conn.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM t_eleves WHERE FkAvenue IS NULL OR FkAvenue = ''");
                if (missingAddress > 0)
                    issues.Add($"{missingAddress} élèves sans adresse");

                // Élèves sans téléphone tuteur
                var missingPhone = await conn.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM t_eleves WHERE tel_tuteur IS NULL OR tel_tuteur = ''");
                if (missingPhone > 0)
                    issues.Add($"{missingPhone} élèves sans téléphone du tuteur");
            }
            catch (Exception ex)
            {
                issues.Add($"Erreur lors de la vérification des données manquantes: {ex.Message}");
            }

            return issues;
        }

        /// <summary>
        /// Vérifie les données incohérentes
        /// </summary>
        private async Task<List<string>> CheckInconsistentData(IDbConnection conn)
        {
            var issues = new List<string>();

            try
            {
                // Dates de naissance futures
                var futureBirthDates = await conn.ExecuteScalarAsync<int>(
                    SqlCompatibilityAdapter.GetAdaptedQuery(
                        "SELECT COUNT(*) FROM t_eleves WHERE date_naiss > CURDATE()",
                        _connexion.IsOnline));
                if (futureBirthDates > 0)
                    issues.Add($"{futureBirthDates} élèves avec des dates de naissance futures");

                // Âges incohérents (trop jeunes ou trop vieux)
                var tooYoung = await conn.ExecuteScalarAsync<int>(
                    SqlCompatibilityAdapter.GetAdaptedQuery(
                        "SELECT COUNT(*) FROM t_eleves WHERE YEAR(CURDATE()) - YEAR(date_naiss) < 3",
                        _connexion.IsOnline));
                if (tooYoung > 0)
                    issues.Add($"{tooYoung} élèves de moins de 3 ans");

                var tooOld = await conn.ExecuteScalarAsync<int>(
                    SqlCompatibilityAdapter.GetAdaptedQuery(
                        "SELECT COUNT(*) FROM t_eleves WHERE YEAR(CURDATE()) - YEAR(date_naiss) > 25",
                        _connexion.IsOnline));
                if (tooOld > 0)
                    issues.Add($"{tooOld} élèves de plus de 25 ans");

                // Noms suspects (trop courts)
                var shortNames = await conn.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM t_eleves WHERE LENGTH(nom) < 2 OR LENGTH(prenom) < 2");
                if (shortNames > 0)
                    issues.Add($"{shortNames} élèves avec des noms très courts");
            }
            catch (Exception ex)
            {
                issues.Add($"Erreur lors de la vérification de cohérence: {ex.Message}");
            }

            return issues;
        }

        /// <summary>
        /// Vérifie les doublons
        /// </summary>
        private async Task<List<string>> CheckDuplicates(IDbConnection conn)
        {
            var issues = new List<string>();

            try
            {
                // Doublons de matricule
                var duplicateMatricules = await conn.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) - COUNT(DISTINCT matricule) FROM t_eleves");
                if (duplicateMatricules > 0)
                    issues.Add($"{duplicateMatricules} matricules en doublon");

                // Élèves avec même nom, prénom et date de naissance
                var possibleDuplicates = await conn.ExecuteScalarAsync<int>(@"
                    SELECT COUNT(*) FROM (
                        SELECT nom, prenom, date_naiss, COUNT(*) as cnt
                        FROM t_eleves 
                        WHERE date_naiss IS NOT NULL
                        GROUP BY nom, prenom, date_naiss
                        HAVING COUNT(*) > 1
                    ) duplicates");
                if (possibleDuplicates > 0)
                    issues.Add($"{possibleDuplicates} groupes d'élèves potentiellement en doublon");
            }
            catch (Exception ex)
            {
                issues.Add($"Erreur lors de la vérification des doublons: {ex.Message}");
            }

            return issues;
        }

        /// <summary>
        /// Vérifie les informations complètes
        /// </summary>
        private async Task<int> CheckCompleteInfo(IDbConnection conn)
        {
            try
            {
                return await conn.ExecuteScalarAsync<int>(@"
                    SELECT COUNT(*) FROM t_eleves 
                    WHERE nom IS NOT NULL AND nom != ''
                    AND prenom IS NOT NULL AND prenom != ''
                    AND date_naiss IS NOT NULL
                    AND nom_tuteur IS NOT NULL AND nom_tuteur != ''
                    AND FkAvenue IS NOT NULL AND FkAvenue != ''");
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Calcule le score de santé des données
        /// </summary>
        private double CalculateDataHealthScore(EleveDataHealthReport report)
        {
            if (report.TotalEleves == 0) return 100;

            double score = 100;

            // Pénalités pour les problèmes
            score -= report.MissingDataIssues.Count * 5;
            score -= report.InconsistentDataIssues.Count * 10;
            score -= report.DuplicateIssues.Count * 15;

            // Bonus pour les données complètes
            var completenessRatio = (double)report.ElevesWithCompleteInfo / report.TotalEleves;
            score += completenessRatio * 20;

            return Math.Max(0, Math.Min(100, score));
        }

        /// <summary>
        /// Nettoie automatiquement les données d'élèves
        /// </summary>
        public async Task<DataCleanupResult> CleanupEleveData()
        {
            var result = new DataCleanupResult
            {
                StartTime = DateTime.Now
            };

            try
            {
                using var conn = GetSecureConnection();
                conn.Open();

                using var transaction = conn.BeginTransaction();

                try
                {
                    // Nettoyer les espaces en trop
                    var trimmedNames = await conn.ExecuteAsync(@"
                        UPDATE t_eleves 
                        SET nom = TRIM(nom), 
                            prenom = TRIM(prenom), 
                            postnom = TRIM(postnom),
                            nom_tuteur = TRIM(nom_tuteur)
                        WHERE nom != TRIM(nom) 
                           OR prenom != TRIM(prenom) 
                           OR postnom != TRIM(postnom)
                           OR nom_tuteur != TRIM(nom_tuteur)", transaction: transaction);

                    result.CleanupActions.Add($"Nettoyé {trimmedNames} noms avec espaces superflus");

                    // Normaliser les sexes
                    var normalizedSex = await conn.ExecuteAsync(@"
                        UPDATE t_eleves 
                        SET sexe = CASE 
                            WHEN UPPER(sexe) IN ('M', 'MASCULIN', 'MALE', 'H', 'HOMME') THEN 'M'
                            WHEN UPPER(sexe) IN ('F', 'FEMININ', 'FEMALE', 'FEMME') THEN 'F'
                            ELSE sexe
                        END
                        WHERE sexe NOT IN ('M', 'F')", transaction: transaction);

                    if (normalizedSex > 0)
                        result.CleanupActions.Add($"Normalisé {normalizedSex} valeurs de sexe");

                    // Nettoyer les numéros de téléphone
                    var cleanedPhones = await conn.ExecuteAsync(@"
                        UPDATE t_eleves 
                        SET tel_tuteur = REGEXP_REPLACE(tel_tuteur, '[^0-9+]', '')
                        WHERE tel_tuteur IS NOT NULL 
                        AND tel_tuteur REGEXP '[^0-9+]'", transaction: transaction);

                    if (cleanedPhones > 0)
                        result.CleanupActions.Add($"Nettoyé {cleanedPhones} numéros de téléphone");

                    transaction.Commit();
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception($"Erreur lors du nettoyage: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
            }

            result.EndTime = DateTime.Now;
            result.Duration = result.EndTime - result.StartTime;

            return result;
        }

        /// <summary>
        /// Exporte les données d'élèves avec options avancées
        /// </summary>
        public async Task<DataExportResult> ExportEleveData(ExportOptions options)
        {
            var result = new DataExportResult
            {
                StartTime = DateTime.Now,
                Options = options
            };

            try
            {
                var eleves = GetAllEleves().ToList();
                
                switch (options.Format.ToUpper())
                {
                    case "JSON":
                        result.FilePath = await ExportToJson(eleves, options);
                        break;
                    case "CSV":
                        result.FilePath = await ExportToCsv(eleves, options);
                        break;
                    case "XML":
                        result.FilePath = await ExportToXml(eleves, options);
                        break;
                    default:
                        throw new ArgumentException($"Format d'export non supporté: {options.Format}");
                }

                result.RecordCount = eleves.Count;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
            }

            result.EndTime = DateTime.Now;
            result.Duration = result.EndTime - result.StartTime;

            return result;
        }

        /// <summary>
        /// Exporte vers JSON
        /// </summary>
        private async Task<string> ExportToJson(List<dynamic> eleves, ExportOptions options)
        {
            var fileName = $"eleves_export_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var filePath = Path.Combine(options.OutputDirectory, fileName);
            
            var jsonOptions = new System.Text.Json.JsonSerializerOptions 
            { 
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            
            var json = System.Text.Json.JsonSerializer.Serialize(eleves, jsonOptions);
            await File.WriteAllTextAsync(filePath, json);
            
            return filePath;
        }

        /// <summary>
        /// Exporte vers CSV
        /// </summary>
        private async Task<string> ExportToCsv(List<dynamic> eleves, ExportOptions options)
        {
            var fileName = $"eleves_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            var filePath = Path.Combine(options.OutputDirectory, fileName);
            
            using var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8);
            
            // En-têtes
            if (eleves.Count > 0)
            {
                var firstRecord = (IDictionary<string, object>)eleves[0];
                await writer.WriteLineAsync(string.Join(",", firstRecord.Keys.Select(k => $"\"{k}\"")));
                
                // Données
                foreach (var eleve in eleves)
                {
                    var record = (IDictionary<string, object>)eleve;
                    var values = record.Values.Select(v => $"\"{v?.ToString()?.Replace("\"", "\"\"")}\"");
                    await writer.WriteLineAsync(string.Join(",", values));
                }
            }
            
            return filePath;
        }

        /// <summary>
        /// Exporte vers XML
        /// </summary>
        private async Task<string> ExportToXml(List<dynamic> eleves, ExportOptions options)
        {
            var fileName = $"eleves_export_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
            var filePath = Path.Combine(options.OutputDirectory, fileName);
            
            using var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8);
            
            await writer.WriteLineAsync("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            await writer.WriteLineAsync("<eleves>");
            
            foreach (var eleve in eleves)
            {
                await writer.WriteLineAsync("  <eleve>");
                var record = (IDictionary<string, object>)eleve;
                
                foreach (var kvp in record)
                {
                    var value = System.Security.SecurityElement.Escape(kvp.Value?.ToString() ?? "");
                    await writer.WriteLineAsync($"    <{kvp.Key}>{value}</{kvp.Key}>");
                }
                
                await writer.WriteLineAsync("  </eleve>");
            }
            
            await writer.WriteLineAsync("</eleves>");
            
            return filePath;
        }

        #endregion

        #region Public Utility Methods

        /// <summary>
        /// Génère un nouveau matricule unique pour un élève
        /// </summary>
        /// <returns>Nouveau matricule unique</returns>
        public string GenerateNewMatricule()
        {
            return ExecuteWithErrorHandling(() =>
            {
                // Pour t_eleves, on ne peut pas utiliser l'isolation par id_ecole car cette colonne n'existe pas
                // On génère directement sans isolation
                var query = "SELECT MAX(matricule) FROM t_eleves";
                
                using (var conn = _connexion.GetConnection())
                {
                    var maxId = conn.QueryFirstOrDefault<string>(query);
                    
                    if (string.IsNullOrEmpty(maxId))
                    {
                        return "ELV001";
                    }
                    
                    // Extraire le numéro de la fin de l'ID
                    var numericPart = maxId.Substring(3); // "ELV" = 3 caractères
                    if (int.TryParse(numericPart, out var currentNum))
                    {
                        var nextNum = currentNum + 1;
                        return $"ELV{nextNum:D3}";
                    }
                    
                    return "ELV001";
                }
            }, "GenerateNewMatricule");
        }

        #endregion
    }
}
