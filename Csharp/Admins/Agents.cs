using Dapper;
using EduKin.DataSets;
using EduKin.Inits;
using System.Data;

namespace EduKin.Csharp.Admins
{
    /// <summary>
    /// Service de gestion des agents avec isolation automatique par école
    /// Hérite de BaseService pour bénéficier des fonctionnalités d'isolation et de gestion d'erreurs
    /// </summary>
    public class Agents : BaseService
    {
        private readonly Administrations _admin;

        public Agents() : base()
        {
            _admin = new Administrations();
        }

        #region CRUD Agents

        /// <summary>
        /// Crée un nouvel agent avec isolation automatique par école et gestion d'erreurs avancée
        /// </summary>
        public bool CreateAgent(string nom, string postNom, string prenom, string sexe, string lieuNaiss, 
                                DateTime dateNaiss, string userIndex,
                                string? email = null, string? tel = null, string? fkAvenue = null, 
                                string? numero = null, string? profil = null, decimal? salBase = null, decimal? prime = null, 
                                decimal? cnss = null, decimal? ipr = null, decimal? salNet = null)
        {
            return ExecuteWithErrorHandling(() =>
            {
                // Générer le matricule avec sp_generate_id
                string matricule = _admin.GenerateId("t_agents", "matricule", "AGT", userIndex);

                var agentData = new
                {
                    matricule = matricule,
                    nom = nom,
                    postnom = postNom,
                    prenom = prenom,
                    sexe = sexe,
                    lieu_naiss = lieuNaiss,
                    date_naiss = dateNaiss,
                    email = email,
                    tel = tel,
                    FkAvenue = fkAvenue,
                    Numero = numero,
                    profil = profil,
                    sal_base = salBase,
                    prime = prime,
                    cnss = cnss,
                    ipr = ipr,
                    sal_net = salNet
                };

                var result = InsertWithIsolation("t_agents", agentData);
                return result > 0;
            }, "CreateAgent");
        }

        /// <summary>
        /// Récupère un agent par son matricule avec isolation automatique par école
        /// </summary>
        public dynamic? GetAgent(string matricule)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = "SELECT * FROM t_agents WHERE matricule = @Matricule";
                return QueryFirstOrDefaultWithIsolation(query, new { Matricule = matricule });
            }, "GetAgent");
        }

        /// <summary>
        /// Récupère tous les agents de l'école courante avec isolation automatique
        /// </summary>
        public IEnumerable<dynamic> GetAllAgents()
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = "SELECT * FROM t_agents ORDER BY nom, postnom, prenom";
                return QueryWithIsolation(query);
            }, "GetAllAgents");
        }

        /// <summary>
        /// Récupère les agents par service avec isolation automatique par école
        /// </summary>
        public IEnumerable<dynamic> GetAgentsByService(string service)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = "SELECT * FROM t_agents WHERE service = @Service ORDER BY nom";
                return QueryWithIsolation(query, new { Service = service });
            }, "GetAgentsByService");
        }

        /// <summary>
        /// Récupère les agents par école (méthode legacy maintenue pour compatibilité)
        /// Note: Avec l'isolation automatique, cette méthode retourne les agents de l'école courante
        /// </summary>
        public IEnumerable<dynamic> GetAgentsByEcole(string? idEcole = null)
        {
            return ExecuteWithErrorHandling(() =>
            {
                // Si idEcole est fourni et différent de l'école courante, retourner une liste vide
                // pour respecter l'isolation
                if (!string.IsNullOrEmpty(idEcole) && idEcole != EduKinContext.CurrentIdEcole)
                {
                    return new List<dynamic>();
                }
                
                var query = "SELECT * FROM t_agents ORDER BY nom";
                return QueryWithIsolation(query);
            }, "GetAgentsByEcole");
        }

        /// <summary>
        /// Recherche des agents par terme avec isolation automatique par école
        /// </summary>
        public IEnumerable<dynamic> SearchAgents(string searchTerm)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT * FROM t_agents 
                              WHERE nom LIKE @Search OR postnom LIKE @Search OR prenom LIKE @Search 
                              OR matricule LIKE @Search
                              ORDER BY nom";
                return QueryWithIsolation(query, new { Search = $"%{searchTerm}%" });
            }, "SearchAgents");
        }

        /// <summary>
        /// Met à jour un agent avec isolation automatique par école et gestion d'erreurs avancée
        /// </summary>
        public bool UpdateAgent(string matricule, string nom, string postNom, string prenom, string sexe, 
                                string lieuNaiss, DateTime dateNaiss,
                                string? email = null, string? tel = null, string? fkAvenue = null, 
                                string? numero = null, string? profil = null, decimal? salBase = null, decimal? prime = null,
                                decimal? cnss = null, decimal? ipr = null, decimal? salNet = null)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var agentData = new
                {
                    nom = nom,
                    postnom = postNom,
                    prenom = prenom,
                    sexe = sexe,
                    lieu_naiss = lieuNaiss,
                    date_naiss = dateNaiss,
                    email = email,
                    tel = tel,
                    FkAvenue = fkAvenue,
                    Numero = numero,
                    profil = profil,
                    sal_base = salBase,
                    prime = prime,
                    cnss = cnss,
                    ipr = ipr,
                    sal_net = salNet
                };

                var result = UpdateWithIsolation("t_agents", agentData, "matricule = @Matricule", new { Matricule = matricule });
                return result > 0;
            }, "UpdateAgent");
        }

        /// <summary>
        /// Supprime un agent avec isolation automatique par école et gestion d'erreurs avancée
        /// </summary>
        public bool DeleteAgent(string matricule)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var result = DeleteWithIsolation("t_agents", "matricule = @Matricule", new { Matricule = matricule });
                return result > 0;
            }, "DeleteAgent");
        }

        #endregion

        #region CRUD Affectation Professeur

        /// <summary>
        /// Crée une affectation de professeur avec isolation automatique par école
        /// </summary>
        public bool CreateAffectProf(string idProf, string codPromo, string anneeScol)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var affectData = new
                {
                    id_prof = idProf,
                    cod_promo = codPromo,
                    annee_scol = anneeScol
                };

                var result = InsertWithIsolation("t_affect_prof", affectData);
                return result > 0;
            }, "CreateAffectProf");
        }

        /// <summary>
        /// Récupère les affectations de professeurs par promotion avec isolation automatique
        /// </summary>
        public IEnumerable<dynamic> GetAffectProfByPromotion(string codPromo, string anneeScol)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT ap.*, a.nom, a.postnom, a.prenom, a.service 
                              FROM t_affect_prof ap 
                              INNER JOIN t_agents a ON ap.id_prof = a.matricule 
                              WHERE ap.cod_promo = @CodPromo AND ap.annee_scol = @AnneeScol";
                return QueryWithIsolation(query, new { CodPromo = codPromo, AnneeScol = anneeScol }, "ap");
            }, "GetAffectProfByPromotion");
        }

        /// <summary>
        /// Récupère les affectations d'un professeur par agent avec isolation automatique
        /// </summary>
        public IEnumerable<dynamic> GetAffectProfByAgent(string idProf, string anneeScol)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT ap.*, p.description as promotion 
                              FROM t_affect_prof ap 
                              INNER JOIN t_promotions p ON ap.cod_promo = p.cod_promo 
                              WHERE ap.id_prof = @IdProf AND ap.annee_scol = @AnneeScol";
                return QueryWithIsolation(query, new { IdProf = idProf, AnneeScol = anneeScol }, "ap");
            }, "GetAffectProfByAgent");
        }

        /// <summary>
        /// Supprime une affectation de professeur avec isolation automatique par école
        /// </summary>
        public bool DeleteAffectProf(int num)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var result = DeleteWithIsolation("t_affect_prof", "num = @Num", new { Num = num });
                return result > 0;
            }, "DeleteAffectProf");
        }

        #endregion

        #region CRUD Dettes

        /// <summary>
        /// Crée une dette d'agent avec isolation automatique par école et gestion d'erreurs avancée
        /// </summary>
        public bool CreateDette(string matricule, string motif, decimal montant, DateTime dateDette, 
                                string mois, string anneeScol)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var detteData = new
                {
                    matricule = matricule,
                    motif = motif,
                    montant = montant,
                    date_dette = dateDette,
                    mois = mois,
                    annee_scol = anneeScol
                };

                var result = InsertWithIsolation("t_dettes", detteData);
                return result > 0;
            }, "CreateDette");
        }

        /// <summary>
        /// Récupère les dettes d'un agent avec isolation automatique par école
        /// </summary>
        public IEnumerable<dynamic> GetDettesByAgent(string matricule)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = "SELECT * FROM t_dettes WHERE matricule = @Matricule ORDER BY date_dette DESC";
                return QueryWithIsolation(query, new { Matricule = matricule });
            }, "GetDettesByAgent");
        }

        /// <summary>
        /// Récupère les dettes par période avec isolation automatique par école
        /// </summary>
        public IEnumerable<dynamic> GetDettesByPeriode(string mois, string anneeScol)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT d.*, a.nom, a.postnom, a.prenom 
                              FROM t_dettes d 
                              INNER JOIN t_agents a ON d.matricule = a.matricule 
                              WHERE d.mois = @Mois AND d.annee_scol = @AnneeScol";
                return QueryWithIsolation(query, new { Mois = mois, AnneeScol = anneeScol }, "d");
            }, "GetDettesByPeriode");
        }

        /// <summary>
        /// Calcule le total des dettes d'un agent avec isolation automatique par école
        /// </summary>
        public decimal GetTotalDettesByAgent(string matricule, string anneeScol)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT COALESCE(SUM(montant), 0) 
                              FROM t_dettes 
                              WHERE matricule = @Matricule AND annee_scol = @AnneeScol";
                return ExecuteScalarWithIsolation<decimal>(query, new { Matricule = matricule, AnneeScol = anneeScol });
            }, "GetTotalDettesByAgent");
        }

        /// <summary>
        /// Met à jour une dette avec isolation automatique par école et gestion d'erreurs avancée
        /// </summary>
        public bool UpdateDette(int idDette, string motif, decimal montant, DateTime dateDette, string mois)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var detteData = new
                {
                    motif = motif,
                    montant = montant,
                    date_dette = dateDette,
                    mois = mois
                };

                var result = UpdateWithIsolation("t_dettes", detteData, "id_dettes = @IdDette", new { IdDette = idDette });
                return result > 0;
            }, "UpdateDette");
        }

        /// <summary>
        /// Supprime une dette avec isolation automatique par école et gestion d'erreurs avancée
        /// </summary>
        public bool DeleteDette(int idDette)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var result = DeleteWithIsolation("t_dettes", "id_dettes = @IdDette", new { IdDette = idDette });
                return result > 0;
            }, "DeleteDette");
        }

        #endregion

        #region CRUD Affectation Cours

        /// <summary>
        /// Crée une affectation de cours avec isolation automatique par école et gestion d'erreurs avancée
        /// </summary>
        public bool CreateAffectCours(string idCours, string codPromo, string periodeMax, string anneeScol, 
                                      string titulaire, string indice, string? telTitulaire = null, string? statutExamen = null)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var affectCoursData = new
                {
                    id_cours = idCours,
                    cod_promo = codPromo,
                    periode_max = periodeMax,
                    annee_scol = anneeScol,
                    titulaire = titulaire,
                    tel_titulaire = telTitulaire,
                    indice = indice,
                    statut_examen = statutExamen
                };

                var result = InsertWithIsolation("t_affect_cours", affectCoursData);
                return result > 0;
            }, "CreateAffectCours");
        }

        /// <summary>
        /// Récupère les affectations de cours par promotion avec isolation automatique par école
        /// </summary>
        public IEnumerable<dynamic> GetAffectCoursByPromotion(string codPromo, string anneeScol)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT ac.*, c.intitule as cours_intitule 
                              FROM t_affect_cours ac 
                              INNER JOIN t_cours c ON ac.id_cours = c.id_cours 
                              WHERE ac.cod_promo = @CodPromo AND ac.annee_scol = @AnneeScol";
                return QueryWithIsolation(query, new { CodPromo = codPromo, AnneeScol = anneeScol }, "ac");
            }, "GetAffectCoursByPromotion");
        }

        /// <summary>
        /// Récupère les affectations de cours par titulaire avec isolation automatique par école
        /// </summary>
        public IEnumerable<dynamic> GetAffectCoursByTitulaire(string titulaire, string anneeScol)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var query = @"SELECT ac.*, c.intitule as cours_intitule, p.description as promotion 
                              FROM t_affect_cours ac 
                              INNER JOIN t_cours c ON ac.id_cours = c.id_cours 
                              INNER JOIN t_promotions p ON ac.cod_promo = p.cod_promo 
                              WHERE ac.titulaire = @Titulaire AND ac.annee_scol = @AnneeScol";
                return QueryWithIsolation(query, new { Titulaire = titulaire, AnneeScol = anneeScol }, "ac");
            }, "GetAffectCoursByTitulaire");
        }

        /// <summary>
        /// Met à jour une affectation de cours avec isolation automatique par école et gestion d'erreurs avancée
        /// </summary>
        public bool UpdateAffectCours(int num, string periodeMax, string titulaire, string? telTitulaire = null, string? statutExamen = null)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var affectCoursData = new
                {
                    periode_max = periodeMax,
                    titulaire = titulaire,
                    tel_titulaire = telTitulaire,
                    statut_examen = statutExamen
                };

                var result = UpdateWithIsolation("t_affect_cours", affectCoursData, "num = @Num", new { Num = num });
                return result > 0;
            }, "UpdateAffectCours");
        }

        /// <summary>
        /// Supprime une affectation de cours avec isolation automatique par école et gestion d'erreurs avancée
        /// </summary>
        public bool DeleteAffectCours(int num)
        {
            return ExecuteWithErrorHandling(() =>
            {
                var result = DeleteWithIsolation("t_affect_cours", "num = @Num", new { Num = num });
                return result > 0;
            }, "DeleteAffectCours");
        }

        #endregion
    }
}
