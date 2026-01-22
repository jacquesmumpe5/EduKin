using System;
using System.Collections.Generic;

namespace EduKin.Csharp.Admins
{
    /// <summary>
    /// Rapport de santé des données d'élèves
    /// </summary>
    public class EleveDataHealthReport
    {
        public bool Success { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string? Error { get; set; }
        
        public int TotalEleves { get; set; }
        public int ElevesWithPhotos { get; set; }
        public int ElevesWithCompleteInfo { get; set; }
        
        public List<string> MissingDataIssues { get; set; } = new();
        public List<string> InconsistentDataIssues { get; set; } = new();
        public List<string> DuplicateIssues { get; set; } = new();
        
        public double HealthScore { get; set; }
        
        public string GetHealthStatus()
        {
            return HealthScore switch
            {
                >= 90 => "Excellent",
                >= 75 => "Bon",
                >= 60 => "Moyen",
                >= 40 => "Faible",
                _ => "Critique"
            };
        }
        
        public List<string> GetRecommendations()
        {
            var recommendations = new List<string>();
            
            if (MissingDataIssues.Count > 0)
            {
                recommendations.Add("Compléter les données manquantes identifiées");
            }
            
            if (InconsistentDataIssues.Count > 0)
            {
                recommendations.Add("Corriger les incohérences dans les données");
            }
            
            if (DuplicateIssues.Count > 0)
            {
                recommendations.Add("Résoudre les doublons détectés");
            }
            
            var photoCompleteness = TotalEleves > 0 ? (double)ElevesWithPhotos / TotalEleves * 100 : 0;
            if (photoCompleteness < 50)
            {
                recommendations.Add("Améliorer la couverture des photos d'élèves");
            }
            
            var infoCompleteness = TotalEleves > 0 ? (double)ElevesWithCompleteInfo / TotalEleves * 100 : 0;
            if (infoCompleteness < 80)
            {
                recommendations.Add("Compléter les informations manquantes des élèves");
            }
            
            if (recommendations.Count == 0)
            {
                recommendations.Add("Les données sont en bon état - aucune action requise");
            }
            
            return recommendations;
        }
    }

    /// <summary>
    /// Résultat d'une opération de nettoyage de données
    /// </summary>
    public class DataCleanupResult
    {
        public bool Success { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string? Error { get; set; }
        
        public List<string> CleanupActions { get; set; } = new();
        
        public int GetTotalActionsCount()
        {
            return CleanupActions.Count;
        }
        
        public string GetSummary()
        {
            if (!Success)
            {
                return $"Échec du nettoyage: {Error}";
            }
            
            return $"Nettoyage réussi - {CleanupActions.Count} actions effectuées en {Duration.TotalSeconds:F1}s";
        }
    }

    /// <summary>
    /// Options d'export de données
    /// </summary>
    public class ExportOptions
    {
        public string Format { get; set; } = "JSON"; // JSON, CSV, XML
        public string OutputDirectory { get; set; } = string.Empty;
        public bool IncludePhotos { get; set; } = false;
        public bool IncludeAddresses { get; set; } = true;
        public bool IncludeAffectations { get; set; } = true;
        public string? FilterBySexe { get; set; }
        public string? FilterByPromotion { get; set; }
        public DateTime? FilterFromDate { get; set; }
        public DateTime? FilterToDate { get; set; }
        public List<string> ColumnsToInclude { get; set; } = new();
        public List<string> ColumnsToExclude { get; set; } = new();
        
        public ExportOptions()
        {
            OutputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "exports");
        }
        
        public void EnsureOutputDirectoryExists()
        {
            if (!Directory.Exists(OutputDirectory))
            {
                Directory.CreateDirectory(OutputDirectory);
            }
        }
        
        public bool ShouldIncludeColumn(string columnName)
        {
            if (ColumnsToExclude.Contains(columnName, StringComparer.OrdinalIgnoreCase))
                return false;
                
            if (ColumnsToInclude.Count > 0)
                return ColumnsToInclude.Contains(columnName, StringComparer.OrdinalIgnoreCase);
                
            return true;
        }
    }

    /// <summary>
    /// Résultat d'une opération d'export
    /// </summary>
    public class DataExportResult
    {
        public bool Success { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string? Error { get; set; }
        
        public string FilePath { get; set; } = string.Empty;
        public int RecordCount { get; set; }
        public long FileSizeBytes { get; set; }
        public ExportOptions Options { get; set; } = new();
        
        public string GetFileSizeFormatted()
        {
            if (FileSizeBytes < 1024)
                return $"{FileSizeBytes} B";
            if (FileSizeBytes < 1024 * 1024)
                return $"{FileSizeBytes / 1024.0:F1} KB";
            if (FileSizeBytes < 1024 * 1024 * 1024)
                return $"{FileSizeBytes / (1024.0 * 1024.0):F1} MB";
            return $"{FileSizeBytes / (1024.0 * 1024.0 * 1024.0):F1} GB";
        }
        
        public string GetSummary()
        {
            if (!Success)
            {
                return $"Échec de l'export: {Error}";
            }
            
            return $"Export réussi - {RecordCount} enregistrements exportés vers {Path.GetFileName(FilePath)} " +
                   $"({GetFileSizeFormatted()}) en {Duration.TotalSeconds:F1}s";
        }
        
        public void UpdateFileSize()
        {
            if (File.Exists(FilePath))
            {
                FileSizeBytes = new FileInfo(FilePath).Length;
            }
        }
    }

    /// <summary>
    /// Statistiques avancées des élèves
    /// </summary>
    public class EleveStatistics
    {
        public int TotalEleves { get; set; }
        public int TotalGarcons { get; set; }
        public int TotalFilles { get; set; }
        public double PourcentageGarcons => TotalEleves > 0 ? (double)TotalGarcons / TotalEleves * 100 : 0;
        public double PourcentageFilles => TotalEleves > 0 ? (double)TotalFilles / TotalEleves * 100 : 0;
        
        public Dictionary<string, int> RepartitionParAge { get; set; } = new();
        public Dictionary<string, int> RepartitionParPromotion { get; set; } = new();
        public Dictionary<string, int> RepartitionParCommune { get; set; } = new();
        
        public int ElevesAvecPhoto { get; set; }
        public int ElevesAvecTelephoneTuteur { get; set; }
        public int ElevesAvecAdresseComplete { get; set; }
        
        public double TauxCompletionPhoto => TotalEleves > 0 ? (double)ElevesAvecPhoto / TotalEleves * 100 : 0;
        public double TauxCompletionTelephone => TotalEleves > 0 ? (double)ElevesAvecTelephoneTuteur / TotalEleves * 100 : 0;
        public double TauxCompletionAdresse => TotalEleves > 0 ? (double)ElevesAvecAdresseComplete / TotalEleves * 100 : 0;
        
        public DateTime DateGeneration { get; set; } = DateTime.Now;
        public string AnneeScol { get; set; } = string.Empty;
        
        public double GetScoreCompletionGlobal()
        {
            return (TauxCompletionPhoto + TauxCompletionTelephone + TauxCompletionAdresse) / 3;
        }
        
        public string GetStatutCompletionGlobal()
        {
            var score = GetScoreCompletionGlobal();
            return score switch
            {
                >= 90 => "Excellent",
                >= 75 => "Bon",
                >= 60 => "Moyen",
                >= 40 => "Faible",
                _ => "Critique"
            };
        }
    }

    /// <summary>
    /// Critères de recherche avancée d'élèves
    /// </summary>
    public class EleveSearchCriteria
    {
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string? Matricule { get; set; }
        public string? Sexe { get; set; }
        public string? NomTuteur { get; set; }
        public string? TelephoneTuteur { get; set; }
        
        public DateTime? DateNaissanceMin { get; set; }
        public DateTime? DateNaissanceMax { get; set; }
        public int? AgeMin { get; set; }
        public int? AgeMax { get; set; }
        
        public string? Promotion { get; set; }
        public string? Section { get; set; }
        public string? Option { get; set; }
        public string? AnneeScol { get; set; }
        
        public string? Commune { get; set; }
        public string? Quartier { get; set; }
        public string? Avenue { get; set; }
        
        public bool? AvecPhoto { get; set; }
        public bool? AvecTelephone { get; set; }
        public bool? AvecAdresse { get; set; }
        
        public string? EcoleProvenance { get; set; }
        
        public int? Limit { get; set; }
        public int? Offset { get; set; }
        public string SortBy { get; set; } = "nom";
        public string SortDirection { get; set; } = "ASC";
        
        public bool HasCriteria()
        {
            return !string.IsNullOrEmpty(Nom) ||
                   !string.IsNullOrEmpty(Prenom) ||
                   !string.IsNullOrEmpty(Matricule) ||
                   !string.IsNullOrEmpty(Sexe) ||
                   !string.IsNullOrEmpty(NomTuteur) ||
                   !string.IsNullOrEmpty(TelephoneTuteur) ||
                   DateNaissanceMin.HasValue ||
                   DateNaissanceMax.HasValue ||
                   AgeMin.HasValue ||
                   AgeMax.HasValue ||
                   !string.IsNullOrEmpty(Promotion) ||
                   !string.IsNullOrEmpty(Section) ||
                   !string.IsNullOrEmpty(Option) ||
                   !string.IsNullOrEmpty(AnneeScol) ||
                   !string.IsNullOrEmpty(Commune) ||
                   !string.IsNullOrEmpty(Quartier) ||
                   !string.IsNullOrEmpty(Avenue) ||
                   AvecPhoto.HasValue ||
                   AvecTelephone.HasValue ||
                   AvecAdresse.HasValue ||
                   !string.IsNullOrEmpty(EcoleProvenance);
        }
        
        public string GetSummary()
        {
            var criteria = new List<string>();
            
            if (!string.IsNullOrEmpty(Nom)) criteria.Add($"Nom: {Nom}");
            if (!string.IsNullOrEmpty(Prenom)) criteria.Add($"Prénom: {Prenom}");
            if (!string.IsNullOrEmpty(Matricule)) criteria.Add($"Matricule: {Matricule}");
            if (!string.IsNullOrEmpty(Sexe)) criteria.Add($"Sexe: {Sexe}");
            if (!string.IsNullOrEmpty(Promotion)) criteria.Add($"Promotion: {Promotion}");
            if (AgeMin.HasValue || AgeMax.HasValue)
            {
                var ageRange = $"Âge: {AgeMin ?? 0}-{AgeMax ?? 100}";
                criteria.Add(ageRange);
            }
            
            return criteria.Count > 0 ? string.Join(", ", criteria) : "Aucun critère spécifié";
        }
    }

    /// <summary>
    /// Résultat d'une recherche d'élèves
    /// </summary>
    public class EleveSearchResult
    {
        public List<dynamic> Eleves { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 1;
        
        public EleveSearchCriteria Criteria { get; set; } = new();
        public TimeSpan SearchDuration { get; set; }
        public DateTime SearchTime { get; set; } = DateTime.Now;
        
        public bool HasNextPage => CurrentPage < TotalPages;
        public bool HasPreviousPage => CurrentPage > 1;
        
        public string GetSummary()
        {
            return $"{Eleves.Count} élèves trouvés sur {TotalCount} total " +
                   $"(page {CurrentPage}/{TotalPages}) en {SearchDuration.TotalMilliseconds:F0}ms";
        }
    }

    /// <summary>
    /// Modèle pour l'import d'élèves
    /// </summary>
    public class EleveImportModel
    {
        public string Matricule { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string PostNom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Sexe { get; set; } = string.Empty;
        public DateTime? DateNaissance { get; set; }
        public string LieuNaissance { get; set; } = string.Empty;
        public string NomTuteur { get; set; } = string.Empty;
        public string? TelTuteur { get; set; }
        public string? FkAvenue { get; set; }
        public string? Numero { get; set; }
        public string? EcoleProvenance { get; set; }
        public string? FkPromotion { get; set; }
        public string? AnneeScol { get; set; }
        
        public List<string> ValidationErrors { get; set; } = new();
        
        public bool IsValid()
        {
            ValidationErrors.Clear();
            
            if (string.IsNullOrWhiteSpace(Nom))
                ValidationErrors.Add("Le nom est obligatoire");
                
            if (string.IsNullOrWhiteSpace(Prenom))
                ValidationErrors.Add("Le prénom est obligatoire");
                
            if (string.IsNullOrWhiteSpace(Sexe) || (Sexe != "M" && Sexe != "F"))
                ValidationErrors.Add("Le sexe doit être 'M' ou 'F'");
                
            if (string.IsNullOrWhiteSpace(NomTuteur))
                ValidationErrors.Add("Le nom du tuteur est obligatoire");
                
            if (DateNaissance.HasValue && DateNaissance > DateTime.Now)
                ValidationErrors.Add("La date de naissance ne peut pas être future");
                
            if (DateNaissance.HasValue && DateNaissance < DateTime.Now.AddYears(-100))
                ValidationErrors.Add("La date de naissance ne peut pas être antérieure à 100 ans");
                
            return ValidationErrors.Count == 0;
        }
        
        public string GetValidationSummary()
        {
            return ValidationErrors.Count > 0 
                ? string.Join("; ", ValidationErrors)
                : "Données valides";
        }
    }

    /// <summary>
    /// Résultat d'une opération d'import
    /// </summary>
    public class EleveImportResult
    {
        public bool Success { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string? Error { get; set; }
        
        public int TotalRecords { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }
        public int SkippedRecords { get; set; }
        
        public List<string> ImportErrors { get; set; } = new();
        public List<string> ImportWarnings { get; set; } = new();
        public List<string> CreatedMatricules { get; set; } = new();
        
        public double SuccessRate => TotalRecords > 0 ? (double)SuccessfulImports / TotalRecords * 100 : 0;
        
        public string GetSummary()
        {
            if (!Success)
            {
                return $"Échec de l'import: {Error}";
            }
            
            return $"Import terminé - {SuccessfulImports}/{TotalRecords} réussis " +
                   $"({SuccessRate:F1}%) en {Duration.TotalSeconds:F1}s";
        }
        
        public string GetDetailedReport()
        {
            var report = new List<string>
            {
                GetSummary(),
                $"Enregistrements traités: {TotalRecords}",
                $"Imports réussis: {SuccessfulImports}",
                $"Échecs: {FailedImports}",
                $"Ignorés: {SkippedRecords}"
            };
            
            if (ImportErrors.Count > 0)
            {
                report.Add($"Erreurs ({ImportErrors.Count}):");
                report.AddRange(ImportErrors.Take(10).Select(e => $"  - {e}"));
                if (ImportErrors.Count > 10)
                    report.Add($"  ... et {ImportErrors.Count - 10} autres erreurs");
            }
            
            if (ImportWarnings.Count > 0)
            {
                report.Add($"Avertissements ({ImportWarnings.Count}):");
                report.AddRange(ImportWarnings.Take(5).Select(w => $"  - {w}"));
                if (ImportWarnings.Count > 5)
                    report.Add($"  ... et {ImportWarnings.Count - 5} autres avertissements");
            }
            
            return string.Join(Environment.NewLine, report);
        }
    }
}