using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EduKin.Inits
{
    /// <summary>
    /// Modèle de vue pour la gestion des élèves avec validation des données
    /// </summary>
    public class EleveViewModel
    {
        #region Propriétés de base de l'élève

        /// <summary>
        /// Matricule unique de l'élève (généré automatiquement)
        /// </summary>
        public string Matricule { get; set; } = string.Empty;

        /// <summary>
        /// Nom de famille de l'élève (obligatoire, max 25 caractères)
        /// </summary>
        public string Nom { get; set; } = string.Empty;

        /// <summary>
        /// Post-nom de l'élève (obligatoire, max 25 caractères)
        /// </summary>
        public string PostNom { get; set; } = string.Empty;

        /// <summary>
        /// Prénom de l'élève (obligatoire, max 25 caractères)
        /// </summary>
        public string Prenom { get; set; } = string.Empty;

        /// <summary>
        /// Sexe de l'élève (M ou F, obligatoire)
        /// </summary>
        public string Sexe { get; set; } = string.Empty;

        /// <summary>
        /// Date de naissance de l'élève
        /// </summary>
        public DateTime? DateNaissance { get; set; }

        /// <summary>
        /// Lieu de naissance de l'élève (max 50 caractères)
        /// </summary>
        public string LieuNaissance { get; set; } = string.Empty;

        /// <summary>
        /// Nom du tuteur/parent (obligatoire, max 30 caractères)
        /// </summary>
        public string NomTuteur { get; set; } = string.Empty;

        /// <summary>
        /// Numéro de téléphone du tuteur (max 15 caractères)
        /// </summary>
        public string TelTuteur { get; set; } = string.Empty;

        /// <summary>
        /// Clé étrangère vers l'avenue (adresse)
        /// </summary>
        public string FkAvenue { get; set; } = string.Empty;

        /// <summary>
        /// Numéro de parcelle/adresse (max 50 caractères)
        /// </summary>
        public string NumeroAdresse { get; set; } = string.Empty;

        /// <summary>
        /// Adresse complète formatée (lecture seule)
        /// </summary>
        public string AdresseComplete { get; set; } = string.Empty;

        /// <summary>
        /// Chemin vers la photo de l'élève (max 1024 caractères)
        /// </summary>
        public string CheminPhoto { get; set; } = string.Empty;

        /// <summary>
        /// École de provenance (max 50 caractères)
        /// </summary>
        public string EcoleProvenance { get; set; } = string.Empty;

        #endregion

        #region Propriétés d'affectation

        /// <summary>
        /// Année scolaire d'affectation (max 10 caractères)
        /// </summary>
        public string AnneeScolaire { get; set; } = string.Empty;

        /// <summary>
        /// Code de la promotion (obligatoire pour l'affectation)
        /// </summary>
        public string CodePromotion { get; set; } = string.Empty;

        /// <summary>
        /// Nom/description de la promotion
        /// </summary>
        public string NomPromotion { get; set; } = string.Empty;

        /// <summary>
        /// Indice de la promotion (max 10 caractères)
        /// </summary>
        public string IndicePromotion { get; set; } = string.Empty;

        #endregion

        #region Méthodes de validation principales

        /// <summary>
        /// Valide toutes les données de l'élève
        /// </summary>
        /// <returns>True si toutes les validations passent</returns>
        public bool IsValid()
        {
            var errors = GetValidationErrors();
            return errors.Count == 0;
        }

        /// <summary>
        /// Obtient la liste de toutes les erreurs de validation
        /// </summary>
        /// <returns>Liste des messages d'erreur</returns>
        public List<string> GetValidationErrors()
        {
            var errors = new List<string>();

            // Validation des champs obligatoires
            var nomResult = ValidateNom(Nom);
            if (!nomResult.IsSuccess)
                errors.Add(nomResult.ErrorMessage);

            var postNomResult = ValidatePostNom(PostNom);
            if (!postNomResult.IsSuccess)
                errors.Add(postNomResult.ErrorMessage);

            var prenomResult = ValidatePrenom(Prenom);
            if (!prenomResult.IsSuccess)
                errors.Add(prenomResult.ErrorMessage);

            var sexeResult = ValidateSexe(Sexe);
            if (!sexeResult.IsSuccess)
                errors.Add(sexeResult.ErrorMessage);

            var nomTuteurResult = ValidateNomTuteur(NomTuteur);
            if (!nomTuteurResult.IsSuccess)
                errors.Add(nomTuteurResult.ErrorMessage);

            // Validation des champs optionnels mais avec format
            if (!string.IsNullOrWhiteSpace(TelTuteur))
            {
                var telResult = ValidateTelephone(TelTuteur);
                if (!telResult.IsSuccess)
                    errors.Add(telResult.ErrorMessage);
            }

            if (DateNaissance.HasValue)
            {
                var dateResult = ValidateDateNaissance(DateNaissance.Value);
                if (!dateResult.IsSuccess)
                    errors.Add(dateResult.ErrorMessage);
            }

            var lieuResult = ValidateLieuNaissance(LieuNaissance);
            if (!lieuResult.IsSuccess)
                errors.Add(lieuResult.ErrorMessage);

            var ecoleProvResult = ValidateEcoleProvenance(EcoleProvenance);
            if (!ecoleProvResult.IsSuccess)
                errors.Add(ecoleProvResult.ErrorMessage);

            var numeroResult = ValidateNumeroAdresse(NumeroAdresse);
            if (!numeroResult.IsSuccess)
                errors.Add(numeroResult.ErrorMessage);

            var cheminPhotoResult = ValidateCheminPhoto(CheminPhoto);
            if (!cheminPhotoResult.IsSuccess)
                errors.Add(cheminPhotoResult.ErrorMessage);

            return errors;
        }

        #endregion

        #region Méthodes de validation spécifiques

        /// <summary>
        /// Valide le nom de l'élève
        /// </summary>
        /// <param name="nom">Nom à valider</param>
        /// <returns>Résultat de la validation</returns>
        public static ValidationResult ValidateNom(string nom)
        {
            if (string.IsNullOrWhiteSpace(nom))
                return ValidationResult.Error("Le nom est obligatoire");

            if (nom.Length > 25)
                return ValidationResult.Error("Le nom ne peut pas dépasser 25 caractères");

            if (!Regex.IsMatch(nom, @"^[a-zA-ZÀ-ÿ\s\-']+$"))
                return ValidationResult.Error("Le nom contient des caractères invalides");

            return ValidationResult.Success();
        }

        /// <summary>
        /// Valide le post-nom de l'élève
        /// </summary>
        /// <param name="postNom">Post-nom à valider</param>
        /// <returns>Résultat de la validation</returns>
        public static ValidationResult ValidatePostNom(string postNom)
        {
            if (string.IsNullOrWhiteSpace(postNom))
                return ValidationResult.Error("Le post-nom est obligatoire");

            if (postNom.Length > 25)
                return ValidationResult.Error("Le post-nom ne peut pas dépasser 25 caractères");

            if (!Regex.IsMatch(postNom, @"^[a-zA-ZÀ-ÿ\s\-']+$"))
                return ValidationResult.Error("Le post-nom contient des caractères invalides");

            return ValidationResult.Success();
        }

        /// <summary>
        /// Valide le prénom de l'élève
        /// </summary>
        /// <param name="prenom">Prénom à valider</param>
        /// <returns>Résultat de la validation</returns>
        public static ValidationResult ValidatePrenom(string prenom)
        {
            if (string.IsNullOrWhiteSpace(prenom))
                return ValidationResult.Error("Le prénom est obligatoire");

            if (prenom.Length > 25)
                return ValidationResult.Error("Le prénom ne peut pas dépasser 25 caractères");

            if (!Regex.IsMatch(prenom, @"^[a-zA-ZÀ-ÿ\s\-']+$"))
                return ValidationResult.Error("Le prénom contient des caractères invalides");

            return ValidationResult.Success();
        }

        /// <summary>
        /// Valide le sexe de l'élève
        /// </summary>
        /// <param name="sexe">Sexe à valider</param>
        /// <returns>Résultat de la validation</returns>
        public static ValidationResult ValidateSexe(string sexe)
        {
            if (string.IsNullOrWhiteSpace(sexe))
                return ValidationResult.Error("Le sexe est obligatoire");

            if (sexe != "M" && sexe != "F")
                return ValidationResult.Error("Le sexe doit être 'M' (Masculin) ou 'F' (Féminin)");

            return ValidationResult.Success();
        }

        /// <summary>
        /// Valide le nom du tuteur
        /// </summary>
        /// <param name="nomTuteur">Nom du tuteur à valider</param>
        /// <returns>Résultat de la validation</returns>
        public static ValidationResult ValidateNomTuteur(string nomTuteur)
        {
            if (string.IsNullOrWhiteSpace(nomTuteur))
                return ValidationResult.Error("Le nom du tuteur est obligatoire");

            if (nomTuteur.Length > 30)
                return ValidationResult.Error("Le nom du tuteur ne peut pas dépasser 30 caractères");

            if (!Regex.IsMatch(nomTuteur, @"^[a-zA-ZÀ-ÿ\s\-']+$"))
                return ValidationResult.Error("Le nom du tuteur contient des caractères invalides");

            return ValidationResult.Success();
        }

        /// <summary>
        /// Valide le numéro de téléphone
        /// </summary>
        /// <param name="telephone">Numéro de téléphone à valider</param>
        /// <returns>Résultat de la validation</returns>
        public static ValidationResult ValidateTelephone(string telephone)
        {
            if (string.IsNullOrWhiteSpace(telephone))
                return ValidationResult.Success(); // Téléphone optionnel

            if (telephone.Length > 15)
                return ValidationResult.Error("Le numéro de téléphone ne peut pas dépasser 15 caractères");

            // Format accepté: +243XXXXXXXXX, 0XXXXXXXXX, ou XXXXXXXXX (chiffres, espaces, tirets, parenthèses)
            if (!Regex.IsMatch(telephone, @"^[\+]?[0-9\s\-\(\)]{7,15}$"))
                return ValidationResult.Error("Le format du numéro de téléphone est invalide");

            return ValidationResult.Success();
        }

        /// <summary>
        /// Valide la date de naissance
        /// </summary>
        /// <param name="dateNaissance">Date de naissance à valider</param>
        /// <returns>Résultat de la validation</returns>
        public static ValidationResult ValidateDateNaissance(DateTime dateNaissance)
        {
            var today = DateTime.Today;
            var age = today.Year - dateNaissance.Year;
            
            // Ajuster l'âge si l'anniversaire n'est pas encore passé cette année
            if (dateNaissance.Date > today.AddYears(-age))
                age--;

            if (dateNaissance > today)
                return ValidationResult.Error("La date de naissance ne peut pas être dans le futur");

            if (age > 25)
                return ValidationResult.Error("L'âge de l'élève ne peut pas dépasser 25 ans");

            if (age < 3)
                return ValidationResult.Error("L'âge de l'élève doit être d'au moins 3 ans");

            return ValidationResult.Success();
        }

        /// <summary>
        /// Valide le lieu de naissance
        /// </summary>
        /// <param name="lieuNaissance">Lieu de naissance à valider</param>
        /// <returns>Résultat de la validation</returns>
        public static ValidationResult ValidateLieuNaissance(string lieuNaissance)
        {
            if (string.IsNullOrWhiteSpace(lieuNaissance))
                return ValidationResult.Success(); // Lieu de naissance optionnel

            if (lieuNaissance.Length > 50)
                return ValidationResult.Error("Le lieu de naissance ne peut pas dépasser 50 caractères");

            if (!Regex.IsMatch(lieuNaissance, @"^[a-zA-ZÀ-ÿ\s\-',\.]+$"))
                return ValidationResult.Error("Le lieu de naissance contient des caractères invalides");

            return ValidationResult.Success();
        }

        /// <summary>
        /// Valide l'école de provenance
        /// </summary>
        /// <param name="ecoleProvenance">École de provenance à valider</param>
        /// <returns>Résultat de la validation</returns>
        public static ValidationResult ValidateEcoleProvenance(string ecoleProvenance)
        {
            if (string.IsNullOrWhiteSpace(ecoleProvenance))
                return ValidationResult.Success(); // École de provenance optionnelle

            if (ecoleProvenance.Length > 50)
                return ValidationResult.Error("L'école de provenance ne peut pas dépasser 50 caractères");

            return ValidationResult.Success();
        }

        /// <summary>
        /// Valide le numéro d'adresse
        /// </summary>
        /// <param name="numeroAdresse">Numéro d'adresse à valider</param>
        /// <returns>Résultat de la validation</returns>
        public static ValidationResult ValidateNumeroAdresse(string numeroAdresse)
        {
            if (string.IsNullOrWhiteSpace(numeroAdresse))
                return ValidationResult.Success(); // Numéro d'adresse optionnel

            if (numeroAdresse.Length > 50)
                return ValidationResult.Error("Le numéro d'adresse ne peut pas dépasser 50 caractères");

            return ValidationResult.Success();
        }

        /// <summary>
        /// Valide le chemin de la photo
        /// </summary>
        /// <param name="cheminPhoto">Chemin de la photo à valider</param>
        /// <returns>Résultat de la validation</returns>
        public static ValidationResult ValidateCheminPhoto(string cheminPhoto)
        {
            if (string.IsNullOrWhiteSpace(cheminPhoto))
                return ValidationResult.Success(); // Photo optionnelle

            if (cheminPhoto.Length > 1024)
                return ValidationResult.Error("Le chemin de la photo ne peut pas dépasser 1024 caractères");

            return ValidationResult.Success();
        }

        #endregion

        #region Méthodes utilitaires

        /// <summary>
        /// Réinitialise toutes les propriétés à leurs valeurs par défaut
        /// </summary>
        public void Clear()
        {
            Matricule = string.Empty;
            Nom = string.Empty;
            PostNom = string.Empty;
            Prenom = string.Empty;
            Sexe = string.Empty;
            DateNaissance = null;
            LieuNaissance = string.Empty;
            NomTuteur = string.Empty;
            TelTuteur = string.Empty;
            FkAvenue = string.Empty;
            NumeroAdresse = string.Empty;
            AdresseComplete = string.Empty;
            CheminPhoto = string.Empty;
            EcoleProvenance = string.Empty;
            AnneeScolaire = string.Empty;
            CodePromotion = string.Empty;
            NomPromotion = string.Empty;
            IndicePromotion = string.Empty;
        }

        /// <summary>
        /// Copie les données depuis un autre EleveViewModel
        /// </summary>
        /// <param name="source">Source à copier</param>
        public void CopyFrom(EleveViewModel source)
        {
            if (source == null) return;

            Matricule = source.Matricule;
            Nom = source.Nom;
            PostNom = source.PostNom;
            Prenom = source.Prenom;
            Sexe = source.Sexe;
            DateNaissance = source.DateNaissance;
            LieuNaissance = source.LieuNaissance;
            NomTuteur = source.NomTuteur;
            TelTuteur = source.TelTuteur;
            FkAvenue = source.FkAvenue;
            NumeroAdresse = source.NumeroAdresse;
            AdresseComplete = source.AdresseComplete;
            CheminPhoto = source.CheminPhoto;
            EcoleProvenance = source.EcoleProvenance;
            AnneeScolaire = source.AnneeScolaire;
            CodePromotion = source.CodePromotion;
            NomPromotion = source.NomPromotion;
            IndicePromotion = source.IndicePromotion;
        }

        /// <summary>
        /// Obtient le nom complet de l'élève
        /// </summary>
        /// <returns>Nom complet formaté</returns>
        public string GetNomComplet()
        {
            var parts = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(Nom))
                parts.Add(Nom);
            
            if (!string.IsNullOrWhiteSpace(PostNom))
                parts.Add(PostNom);
            
            if (!string.IsNullOrWhiteSpace(Prenom))
                parts.Add(Prenom);

            return string.Join(" ", parts);
        }

        /// <summary>
        /// Calcule l'âge de l'élève
        /// </summary>
        /// <returns>Âge en années ou null si pas de date de naissance</returns>
        public int? GetAge()
        {
            if (!DateNaissance.HasValue)
                return null;

            var today = DateTime.Today;
            var age = today.Year - DateNaissance.Value.Year;
            
            if (DateNaissance.Value.Date > today.AddYears(-age))
                age--;

            return age;
        }

        #endregion
    }

    /// <summary>
    /// Résultat d'une validation
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Indique si la validation a réussi
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// Message d'erreur si la validation a échoué
        /// </summary>
        public string ErrorMessage { get; private set; } = string.Empty;

        private ValidationResult(bool isSuccess, string errorMessage = "")
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Crée un résultat de validation réussi
        /// </summary>
        /// <returns>Résultat de succès</returns>
        public static ValidationResult Success()
        {
            return new ValidationResult(true);
        }

        /// <summary>
        /// Crée un résultat de validation échoué
        /// </summary>
        /// <param name="errorMessage">Message d'erreur</param>
        /// <returns>Résultat d'erreur</returns>
        public static ValidationResult Error(string errorMessage)
        {
            return new ValidationResult(false, errorMessage);
        }
    }

    /// <summary>
    /// Exception levée lors d'erreurs de validation
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Liste des erreurs de validation
        /// </summary>
        public List<string> ValidationErrors { get; }

        /// <summary>
        /// Constructeur avec liste d'erreurs
        /// </summary>
        /// <param name="errors">Liste des erreurs de validation</param>
        public ValidationException(List<string> errors)
            : base($"Erreurs de validation: {string.Join(", ", errors)}")
        {
            ValidationErrors = errors ?? new List<string>();
        }

        /// <summary>
        /// Constructeur avec une seule erreur
        /// </summary>
        /// <param name="error">Message d'erreur unique</param>
        public ValidationException(string error)
            : base($"Erreur de validation: {error}")
        {
            ValidationErrors = new List<string> { error };
        }
    }
}