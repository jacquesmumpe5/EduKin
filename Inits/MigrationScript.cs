using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace EduKin.Inits
{
    /// <summary>
    /// Script de migration automatique pour remplacer SchoolContext et UserContext par ApplicationContext
    /// </summary>
    public static class MigrationScript
    {
        private static readonly Dictionary<string, string> _replacements = new()
        {
            // SchoolContext vers ApplicationContext
            { @"SchoolContext\.Initialize\(", "ApplicationContext.Initialize(" },
            { @"SchoolContext\.InitializeComplete\(", "ApplicationContext.InitializeComplete(" },
            { @"SchoolContext\.InitializeWithActiveYear\(", "ApplicationContext.InitializeWithActiveYear(" },
            { @"SchoolContext\.Clear\(\)", "ApplicationContext.Clear()" },
            { @"SchoolContext\.CurrentIdEcole", "ApplicationContext.CurrentIdEcole" },
            { @"SchoolContext\.CurrentDenomination", "ApplicationContext.CurrentDenomination" },
            { @"SchoolContext\.CurrentIdAnnee", "ApplicationContext.CurrentIdAnnee" },
            { @"SchoolContext\.CurrentCodeAnnee", "ApplicationContext.CurrentCodeAnnee" },
            { @"SchoolContext\.DateDebutAnnee", "ApplicationContext.DateDebutAnnee" },
            { @"SchoolContext\.DateFinAnnee", "ApplicationContext.DateFinAnnee" },
            { @"SchoolContext\.EstActive", "ApplicationContext.EstActive" },
            { @"SchoolContext\.EstCloturee", "ApplicationContext.EstCloturee" },
            { @"SchoolContext\.CurrentUserId", "ApplicationContext.CurrentUserId" },
            { @"SchoolContext\.CurrentUsername", "ApplicationContext.CurrentUserName" },
            { @"SchoolContext\.IsConfigured", "ApplicationContext.IsConfigured" },
            { @"SchoolContext\.CreateSchoolYear\(", "ApplicationContext.CreateSchoolYear(" },
            { @"SchoolContext\.GetActiveSchoolYear\(\)", "ApplicationContext.GetActiveSchoolYear()" },
            { @"SchoolContext\.GetAllSchoolYears\(\)", "ApplicationContext.GetAllSchoolYears()" },
            { @"SchoolContext\.ActivateSchoolYear\(", "ApplicationContext.ActivateSchoolYear(" },
            { @"SchoolContext\.GenerateSchoolYearCode\(", "ApplicationContext.GenerateSchoolYearCode(" },
            { @"SchoolContext\.GenerateCurrentSchoolYearCode\(\)", "ApplicationContext.GenerateCurrentSchoolYearCode()" },
            { @"SchoolContext\.CalculateSchoolYearDates\(", "ApplicationContext.CalculateSchoolYearDates(" },
            { @"SchoolContext\.HasSchoolYears\(\)", "ApplicationContext.HasSchoolYears()" },
            { @"SchoolContext\.ValidateCanModify\(\)", "ApplicationContext.ValidateCanModify()" },
            { @"SchoolContext\.GetContextSummary\(\)", "ApplicationContext.GetContextSummary()" },
            { @"SchoolContext\.AddIsolationClause\(", "ApplicationContext.AddIsolationClause(" },
            { @"SchoolContext\.GetIsolationParameters\(\)", "ApplicationContext.GetIsolationParameters()" },
            { @"SchoolContext\.GetIsolationErrorMessage\(\)", "ApplicationContext.GetIsolationErrorMessage()" },
            { @"SchoolContext\.TryGetCurrentIdEcole\(\)", "ApplicationContext.TryGetCurrentIdEcole()" },
            { @"SchoolContext\.TryGetCurrentDenomination\(\)", "ApplicationContext.TryGetCurrentDenomination()" },
            { @"SchoolContext\.TryGetCurrentCodeAnnee\(\)", "ApplicationContext.TryGetCurrentCodeAnnee()" },
            { @"SchoolContext\.TryGetCurrentUserId\(\)", "ApplicationContext.TryGetCurrentUserId()" },

            // UserContext vers ApplicationContext
            { @"UserContext\.Initialize\(", "ApplicationContext.InitializeUser(" },
            { @"UserContext\.Logout\(\)", "ApplicationContext.Logout()" },
            { @"UserContext\.CurrentUserId", "ApplicationContext.CurrentUserId" },
            { @"UserContext\.CurrentUserName", "ApplicationContext.CurrentUserName" },
            { @"UserContext\.CurrentUserRole", "ApplicationContext.CurrentUserRole" },
            { @"UserContext\.CurrentUserIndex", "ApplicationContext.CurrentUserIndex" },
            { @"UserContext\.IsAuthenticated", "ApplicationContext.IsAuthenticated" },
            { @"UserContext\.HasRole\(", "ApplicationContext.HasRole(" },
            { @"UserContext\.HasAnyRole\(", "ApplicationContext.HasAnyRole(" },
            { @"UserContext\.IsAdmin\(\)", "ApplicationContext.IsAdmin()" },
            { @"UserContext\.TryGetCurrentUser\(\)", "ApplicationContext.TryGetCurrentUser()" },
            { @"UserContext\.GetAuditParameters\(\)", "ApplicationContext.GetAuditParameters()" },
            { @"UserContext\.GetAuthenticationErrorMessage\(\)", "ApplicationContext.GetAuthenticationErrorMessage()" },
            { @"UserContext\.ValidatePermissions\(", "ApplicationContext.ValidatePermissions(" }
        };

        private static readonly List<string> _fileExtensions = new() { ".cs" };
        private static readonly List<string> _excludeDirectories = new() { "bin", "obj", ".vs", ".git" };

        /// <summary>
        /// Exécute la migration complète du projet
        /// </summary>
        public static void ExecuteMigration(string projectPath = ".")
        {
            Console.WriteLine("=== DÉBUT DE LA MIGRATION AUTOMATIQUE ===");
            Console.WriteLine($"Répertoire du projet: {Path.GetFullPath(projectPath)}");
            
            var files = GetCSharpFiles(projectPath);
            int totalFiles = files.Count;
            int modifiedFiles = 0;
            int totalReplacements = 0;

            Console.WriteLine($"Fichiers C# trouvés: {totalFiles}");
            Console.WriteLine();

            foreach (var file in files)
            {
                var (modified, replacements) = ProcessFile(file);
                if (modified)
                {
                    modifiedFiles++;
                    totalReplacements += replacements;
                    Console.WriteLine($"✅ {file} - {replacements} remplacements");
                }
            }

            Console.WriteLine();
            Console.WriteLine("=== RÉSUMÉ DE LA MIGRATION ===");
            Console.WriteLine($"Fichiers traités: {totalFiles}");
            Console.WriteLine($"Fichiers modifiés: {modifiedFiles}");
            Console.WriteLine($"Total des remplacements: {totalReplacements}");
            
            if (modifiedFiles > 0)
            {
                Console.WriteLine();
                Console.WriteLine("⚠️  ÉTAPES SUIVANTES RECOMMANDÉES:");
                Console.WriteLine("1. Compiler le projet pour vérifier les erreurs");
                Console.WriteLine("2. Tester les fonctionnalités critiques");
                Console.WriteLine("3. Supprimer SchoolContext.cs et UserContext.cs");
                Console.WriteLine("4. Nettoyer les using inutiles");
            }
            
            Console.WriteLine("=== FIN DE LA MIGRATION ===");
        }

        /// <summary>
        /// Traite un fichier individuel
        /// </summary>
        private static (bool modified, int replacements) ProcessFile(string filePath)
        {
            try
            {
                string content = File.ReadAllText(filePath);
                string originalContent = content;
                int replacementCount = 0;

                // Appliquer tous les remplacements
                foreach (var replacement in _replacements)
                {
                    var regex = new Regex(replacement.Key);
                    var matches = regex.Matches(content);
                    if (matches.Count > 0)
                    {
                        content = regex.Replace(content, replacement.Value);
                        replacementCount += matches.Count;
                    }
                }

                // Ajouter le using ApplicationContext si nécessaire
                if (replacementCount > 0 && !content.Contains("using EduKin.Inits;"))
                {
                    content = AddApplicationContextUsing(content);
                }

                // Sauvegarder si modifié
                if (content != originalContent)
                {
                    File.WriteAllText(filePath, content);
                    return (true, replacementCount);
                }

                return (false, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors du traitement de {filePath}: {ex.Message}");
                return (false, 0);
            }
        }

        /// <summary>
        /// Ajoute le using EduKin.Inits si nécessaire
        /// </summary>
        private static string AddApplicationContextUsing(string content)
        {
            // Chercher la position après les autres usings
            var lines = content.Split('\n');
            int insertPosition = -1;
            
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (line.StartsWith("using ") && !line.Contains("EduKin.Inits"))
                {
                    insertPosition = i + 1;
                }
                else if (line.StartsWith("namespace ") || line.StartsWith("public ") || line.StartsWith("internal "))
                {
                    break;
                }
            }

            if (insertPosition > -1)
            {
                var newLines = new List<string>(lines);
                newLines.Insert(insertPosition, "using EduKin.Inits;");
                return string.Join("\n", newLines);
            }

            return content;
        }

        /// <summary>
        /// Récupère tous les fichiers C# du projet
        /// </summary>
        private static List<string> GetCSharpFiles(string directory)
        {
            var files = new List<string>();
            
            try
            {
                // Fichiers dans le répertoire courant
                foreach (var extension in _fileExtensions)
                {
                    files.AddRange(Directory.GetFiles(directory, $"*{extension}"));
                }

                // Récursion dans les sous-répertoires
                foreach (var subDir in Directory.GetDirectories(directory))
                {
                    var dirName = Path.GetFileName(subDir);
                    if (!_excludeDirectories.Contains(dirName))
                    {
                        files.AddRange(GetCSharpFiles(subDir));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de la lecture du répertoire {directory}: {ex.Message}");
            }

            return files;
        }

        /// <summary>
        /// Crée une sauvegarde du projet avant migration
        /// </summary>
        public static void CreateBackup(string projectPath = ".")
        {
            try
            {
                string backupPath = $"{projectPath}_backup_{DateTime.Now:yyyyMMdd_HHmmss}";
                Console.WriteLine($"Création de la sauvegarde: {backupPath}");
                
                CopyDirectory(projectPath, backupPath);
                Console.WriteLine("✅ Sauvegarde créée avec succès");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de la création de la sauvegarde: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Copie récursive d'un répertoire
        /// </summary>
        private static void CopyDirectory(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(destDir, fileName);
                File.Copy(file, destFile);
            }

            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                var dirName = Path.GetFileName(subDir);
                if (!_excludeDirectories.Contains(dirName))
                {
                    var destSubDir = Path.Combine(destDir, dirName);
                    CopyDirectory(subDir, destSubDir);
                }
            }
        }

        /// <summary>
        /// Valide que ApplicationContext contient toutes les méthodes nécessaires
        /// </summary>
        public static bool ValidateApplicationContext()
        {
            try
            {
                var appContextPath = Path.Combine("Inits", "ApplicationContext.cs");
                if (!File.Exists(appContextPath))
                {
                    Console.WriteLine("❌ ApplicationContext.cs non trouvé");
                    return false;
                }

                var content = File.ReadAllText(appContextPath);
                var requiredMethods = new[]
                {
                    "InitializeSchool", "InitializeUser", "InitializeComplete",
                    "CurrentIdEcole", "CurrentUserName", "CurrentUserIndex",
                    "AddIsolationClause", "GetIsolationParameters",
                    "HasRole", "IsAdmin", "IsAuthenticated"
                };

                foreach (var method in requiredMethods)
                {
                    if (!content.Contains(method))
                    {
                        Console.WriteLine($"❌ Méthode manquante dans ApplicationContext: {method}");
                        return false;
                    }
                }

                Console.WriteLine("✅ ApplicationContext validé - toutes les méthodes sont présentes");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de la validation: {ex.Message}");
                return false;
            }
        }
    }
}