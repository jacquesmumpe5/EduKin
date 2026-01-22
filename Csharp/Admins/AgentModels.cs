using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EduKin.Csharp.Admins
{
    /// <summary>
    /// Modèle de vue pour la gestion des agents
    /// Contient toutes les propriétés nécessaires pour l'affichage et la validation
    /// </summary>
    public class AgentViewModel
    {
        #region Propriétés de Base

        /// <summary>
        /// Matricule unique de l'agent (généré automatiquement)
        /// </summary>
        public string Matricule { get; set; } = string.Empty;

        /// <summary>
        /// Nom de famille de l'agent
        /// </summary>
        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(25, ErrorMessage = "Le nom ne peut pas dépasser 25 caractères")]
        public string Nom { get; set; } = string.Empty;

        /// <summary>
        /// Post-nom de l'agent
        /// </summary>
        [Required(ErrorMessage = "Le post-nom est obligatoire")]
        [StringLength(25, ErrorMessage = "Le post-nom ne peut pas dépasser 25 caractères")]
        public string PostNom { get; set; } = string.Empty;

        /// <summary>
        /// Prénom de l'agent
        /// </summary>
        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [StringLength(25, ErrorMessage = "Le prénom ne peut pas dépasser 25 caractères")]
        public string Prenom { get; set; } = string.Empty;

        /// <summary>
        /// Sexe de l'agent (M ou F)
        /// </summary>
        [Required(ErrorMessage = "Le sexe est obligatoire")]
        [RegularExpression("^[MF]$", ErrorMessage = "Le sexe doit être M ou F")]
        public string Sexe { get; set; } = string.Empty;

        /// <summary>
        /// Date de naissance de l'agent
        /// </summary>
        [Required(ErrorMessage = "La date de naissance est obligatoire")]
        public DateTime DateNaissance { get; set; } = DateTime.Now.AddYears(-25);

        /// <summary>
        /// Lieu de naissance de l'agent
        /// </summary>
        [Required(ErrorMessage = "Le lieu de naissance est obligatoire")]
        [StringLength(50, ErrorMessage = "Le lieu de naissance ne peut pas dépasser 50 caractères")]
        public string LieuNaissance { get; set; } = string.Empty;

        #endregion

        #region Informations Professionnelles

        /// <summary>
        /// Service d'affectation de l'agent
        /// </summary>
        [Required(ErrorMessage = "Le service est obligatoire")]
        [StringLength(50, ErrorMessage = "Le service ne peut pas dépasser 50 caractères")]
        public string Service { get; set; } = string.Empty;

        /// <summary>
        /// Fonction occupée par l'agent
        /// </summary>
        [StringLength(45, ErrorMessage = "La fonction ne peut pas dépasser 45 caractères")]
        public string? Fonction { get; set; }

        /// <summary>
        /// Grade de l'agent
        /// </summary>
        [StringLength(10, ErrorMessage = "Le grade ne peut pas dépasser 10 caractères")]
        public string? Grade { get; set; }

        /// <summary>
        /// Rôle de l'agent dans le système
        /// </summary>
        [StringLength(35, ErrorMessage = "Le rôle ne peut pas dépasser 35 caractères")]
        public string? Role { get; set; }

        /// <summary>
        /// ID de l'école d'affectation
        /// </summary>
        [Required(ErrorMessage = "L'école d'affectation est obligatoire")]
        public string FkEcole { get; set; } = string.Empty;

        #endregion

        #region Informations de Contact

        /// <summary>
        /// Adresse email de l'agent
        /// </summary>
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [StringLength(50, ErrorMessage = "L'email ne peut pas dépasser 50 caractères")]
        public string? Email { get; set; }

        /// <summary>
        /// Numéro de téléphone de l'agent
        /// </summary>
        [StringLength(15, ErrorMessage = "Le téléphone ne peut pas dépasser 15 caractères")]
        public string? Tel { get; set; }

        /// <summary>
        /// Ecole de provenance
        /// </summary>
        [StringLength(50, ErrorMessage = "L'école de provenance ne peut pas dépasser 50 caractères")]
        public string? EcoleProvenance { get; set; }

        /// <summary>
        /// Référence à l'avenue (adresse)
        /// </summary>
        [StringLength(50, ErrorMessage = "La référence avenue ne peut pas dépasser 50 caractères")]
        public string? FkAvenue { get; set; }

        /// <summary>
        /// Numéro de parcelle
        /// </summary>
        [StringLength(10, ErrorMessage = "Le numéro ne peut pas dépasser 10 caractères")]
        public string? Numero { get; set; }

        #endregion

        #region Informations Salariales

        /// <summary>
        /// Salaire de base de l'agent
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Le salaire de base doit être positif")]
        public decimal? SalBase { get; set; }

        /// <summary>
        /// Prime de l'agent
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "La prime doit être positive ou zéro")]
        public decimal? Prime { get; set; }

        /// <summary>
        /// Cotisation CNSS de l'agent
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "La CNSS doit être positive ou zéro")]
        public decimal? Cnss { get; set; }

        /// <summary>
        /// Impôt Professionnel sur les Rémunérations (IPR)
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "L'IPR doit être positif ou zéro")]
        public decimal? Ipr { get; set; }

        /// <summary>
        /// Salaire net de l'agent
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Le salaire net doit être positif")]
        public decimal? SalNet { get; set; }

        #endregion

        #region Informations Système

        /// <summary>
        /// Chemin vers la photo de profil de l'agent
        /// </summary>
        public string? CheminPhoto { get; set; }

        /// <summary>
        /// Date de création de l'enregistrement
        /// </summary>
        public DateTime DateCreation { get; set; } = DateTime.Now;

        /// <summary>
        /// Date de dernière modification
        /// </summary>
        public DateTime DateModification { get; set; } = DateTime.Now;

        #endregion

        #region Propriétés Calculées

        /// <summary>
        /// Nom complet de l'agent (Nom + Post-nom + Prénom)
        /// </summary>
        public string NomComplet => $"{Nom} {PostNom} {Prenom}".Trim();

        /// <summary>
        /// Nom d'affichage court (Nom + Prénom)
        /// </summary>
        public string NomAffichage => $"{Nom} {Prenom}".Trim();

        /// <summary>
        /// Âge calculé de l'agent
        /// </summary>
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateNaissance.Year;
                if (DateNaissance.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        /// <summary>
        /// Ancienneté en années (basée sur la date de création)
        /// </summary>
        public int Anciennete
        {
            get
            {
                var today = DateTime.Today;
                var anciennete = today.Year - DateCreation.Year;
                if (DateCreation.Date > today.AddYears(-anciennete)) anciennete--;
                return Math.Max(0, anciennete);
            }
        }

        /// <summary>
        /// Indique si l'agent a une photo de profil
        /// </summary>
        public bool APhoto => !string.IsNullOrWhiteSpace(CheminPhoto);

        /// <summary>
        /// Statut salarial (Payé/Non payé)
        /// </summary>
        public string StatutSalarial => SalNet.HasValue && SalNet > 0 ? "Payé" : "Non payé";

        #endregion

        #region Méthodes de Validation

        /// <summary>
        /// Valide toutes les propriétés du modèle
        /// </summary>
        /// <returns>True si le modèle est valide</returns>
        public bool IsValid()
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(this);
            
            return Validator.TryValidateObject(this, validationContext, validationResults, true);
        }

        /// <summary>
        /// Obtient la liste des erreurs de validation
        /// </summary>
        /// <returns>Liste des messages d'erreur</returns>
        public List<string> GetValidationErrors()
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(this);
            
            Validator.TryValidateObject(this, validationContext, validationResults, true);
            
            return validationResults.Select(vr => vr.ErrorMessage ?? "Erreur de validation").ToList();
        }

        /// <summary>
        /// Valide les informations salariales
        /// </summary>
        /// <returns>True si les informations salariales sont cohérentes</returns>
        public bool ValidateSalaryInfo()
        {
            // Si un salaire de base est défini, le salaire net doit être calculé
            if (SalBase.HasValue && SalBase > 0)
            {
                if (!SalNet.HasValue)
                {
                    return false; // Salaire net manquant
                }

                // Calculer le salaire net attendu
                var prime = Prime ?? 0;
                var cnss = Cnss ?? 0;
                var ipr = Ipr ?? 0;
                var expectedNet = SalBase + prime - cnss - ipr;

                // Tolérance de 1 unité pour les arrondis
                if (Math.Abs((SalNet ?? 0) - expectedNet.Value) > 1)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Valide l'âge de l'agent
        /// </summary>
        /// <returns>True si l'âge est dans une plage acceptable</returns>
        public bool ValidateAge()
        {
            var age = Age;
            return age >= 18 && age <= 70; // Âge de travail légal
        }

        #endregion

        #region Méthodes Utilitaires

        /// <summary>
        /// Calcule automatiquement le salaire net basé sur le salaire de base, prime, CNSS et l'IPR
        /// </summary>
        public void CalculateSalaireNet()
        {
            if (SalBase.HasValue && SalBase > 0)
            {
                var prime = Prime ?? 0;
                var cnss = Cnss ?? 0;
                var ipr = Ipr ?? 0;
                
                // Salaire net = Salaire de base + Prime - CNSS - IPR
                SalNet = SalBase + prime - cnss - ipr;
                
                // S'assurer que le salaire net n'est pas négatif
                if (SalNet < 0)
                {
                    SalNet = 0;
                }
            }
        }

        /// <summary>
        /// Formate le nom complet selon les conventions
        /// </summary>
        /// <returns>Nom formaté en majuscules</returns>
        public string GetFormattedName()
        {
            return NomComplet.ToUpperInvariant();
        }

        /// <summary>
        /// Obtient une description du service avec la fonction
        /// </summary>
        /// <returns>Description complète du poste</returns>
        public string GetJobDescription()
        {
            if (!string.IsNullOrWhiteSpace(Fonction))
            {
                return $"{Fonction} - {Service}";
            }
            return Service;
        }

        /// <summary>
        /// Clone le modèle actuel
        /// </summary>
        /// <returns>Nouvelle instance avec les mêmes valeurs</returns>
        public AgentViewModel Clone()
        {
            return new AgentViewModel
            {
                Matricule = Matricule,
                Nom = Nom,
                PostNom = PostNom,
                Prenom = Prenom,
                Sexe = Sexe,
                DateNaissance = DateNaissance,
                LieuNaissance = LieuNaissance,
                Service = Service,
                Fonction = Fonction,
                Grade = Grade,
                Role = Role,
                FkEcole = FkEcole,
                Email = Email,
                Tel = Tel,
                FkAvenue = FkAvenue,
                Numero = Numero,
                SalBase = SalBase,
                Prime = Prime,
                Cnss = Cnss,
                Ipr = Ipr,
                SalNet = SalNet,
                CheminPhoto = CheminPhoto,
                DateCreation = DateCreation,
                DateModification = DateTime.Now
            };
        }

        /// <summary>
        /// Convertit le modèle en dictionnaire pour la base de données
        /// </summary>
        /// <returns>Dictionnaire des propriétés</returns>
        public Dictionary<string, object?> ToDictionary()
        {
            return new Dictionary<string, object?>
            {
                { "is_valid", !string.IsNullOrEmpty(Matricule) ||
                   !string.IsNullOrEmpty(Nom) ||
                   !string.IsNullOrEmpty(PostNom) ||
                   !string.IsNullOrEmpty(Prenom) ||
                   !string.IsNullOrEmpty(Sexe) ||
                   DateNaissance != default(DateTime) ||
                   !string.IsNullOrEmpty(LieuNaissance) ||
                   !string.IsNullOrEmpty(Service) ||
                   !string.IsNullOrEmpty(Fonction) ||
                   !string.IsNullOrEmpty(Grade) ||
                   !string.IsNullOrEmpty(Role) ||
                   !string.IsNullOrEmpty(FkEcole) ||
                   !string.IsNullOrEmpty(Email) ||
                   !string.IsNullOrEmpty(Tel) ||
                   !string.IsNullOrEmpty(FkAvenue) ||
                   !string.IsNullOrEmpty(Numero) ||
                   !string.IsNullOrEmpty(EcoleProvenance) },
                { "matricule", Matricule },
                { "nom", Nom },
                { "postnom", PostNom },
                { "prenom", Prenom },
                { "sexe", Sexe },
                { "date_naiss", DateNaissance },
                { "lieu_naiss", LieuNaissance },
                { "service", Service },
                { "fonction", Fonction },
                { "grade", Grade },
                { "role", Role },
                { "fk_ecole", FkEcole },
                { "email", Email },
                { "tel", Tel },
                { "fk_avenue", FkAvenue },
                { "numero", Numero },
                { "sal_base", SalBase },
                { "prime", Prime },
                { "cnss", Cnss },
                { "ipr", Ipr },
                { "sal_net", SalNet },
                { "profil", CheminPhoto },
                { "created_at", DateCreation },
                { "updated_at", DateModification }
            };
        }

        #endregion
    }

    /// <summary>
    /// Exception personnalisée pour les erreurs de validation des agents
    /// </summary>
    public class AgentValidationException : Exception
    {
        public List<string> ValidationErrors { get; }

        public AgentValidationException(List<string> validationErrors) 
            : base($"Erreurs de validation: {string.Join(", ", validationErrors)}")
        {
            ValidationErrors = validationErrors;
        }

        public AgentValidationException(string message) : base(message)
        {
            ValidationErrors = new List<string> { message };
        }

        public AgentValidationException(string message, Exception innerException) : base(message, innerException)
        {
            ValidationErrors = new List<string> { message };
        }
    }

    /// <summary>
    /// Modèle pour les statistiques des agents
    /// </summary>
    public class AgentStatistics
    {
        public int TotalAgents { get; set; }
        public int TotalHommes { get; set; }
        public int TotalFemmes { get; set; }
        public int TotalEnseignants { get; set; }
        public int TotalAdministratifs { get; set; }
        public int TotalAvecSalaire { get; set; }
        public int TotalSansSalaire { get; set; }
        public decimal MoyenneSalaire { get; set; }
        public decimal TotalMasseSalariale { get; set; }
        
        /// <summary>
        /// Pourcentage d'hommes
        /// </summary>
        public double PourcentageHommes => TotalAgents > 0 ? (double)TotalHommes / TotalAgents * 100 : 0;
        
        /// <summary>
        /// Pourcentage de femmes
        /// </summary>
        public double PourcentageFemmes => TotalAgents > 0 ? (double)TotalFemmes / TotalAgents * 100 : 0;
        
        /// <summary>
        /// Pourcentage d'enseignants
        /// </summary>
        public double PourcentageEnseignants => TotalAgents > 0 ? (double)TotalEnseignants / TotalAgents * 100 : 0;
    }

    /// <summary>
    /// Critères de recherche pour les agents
    /// </summary>
    public class AgentSearchCriteria
    {
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string? Matricule { get; set; }
        public string? Service { get; set; }
        public string? Fonction { get; set; }
        public string? Sexe { get; set; }
        public DateTime? DateNaissanceMin { get; set; }
        public DateTime? DateNaissanceMax { get; set; }
        public decimal? SalaireMin { get; set; }
        public decimal? SalaireMax { get; set; }
        public bool? AvecPhoto { get; set; }
        public bool? AvecSalaire { get; set; }
    }
}