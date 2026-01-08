using System;
using System.IO;
using System.Text.Json;

namespace EduKin.Inits
{
    /// <summary>
    /// Gestionnaire de configuration de l'école pour la machine
    /// </summary>
    public class SchoolConfigManager
    {
        private const string CONFIG_FILE = "school_config.json";
        private readonly string _configPath;

        public SchoolConfigManager()
        {
            // Le fichier config est dans le répertoire de l'application
            _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIG_FILE);
        }

        /// <summary>
        /// Vérifie si le fichier de configuration existe
        /// </summary>
        public bool ConfigExists()
        {
            return File.Exists(_configPath);
        }

        /// <summary>
        /// Charge la configuration depuis le fichier JSON
        /// </summary>
        /// <returns>Configuration de l'école ou null si erreur</returns>
        public SchoolConfig? LoadConfig()
        {
            try
            {
                if (!ConfigExists())
                {
                    return null;
                }

                string jsonContent = File.ReadAllText(_configPath);
                
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    return null;
                }

                var config = JsonSerializer.Deserialize<SchoolConfig>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Validation basique
                if (config == null || string.IsNullOrWhiteSpace(config.IdEcole))
                {
                    return null;
                }

                return config;
            }
            catch (JsonException ex)
            {
                // Fichier JSON corrompu
                throw new InvalidOperationException("Le fichier de configuration est corrompu.", ex);
            }
            catch (Exception ex)
            {
                // Autre erreur (permissions, etc.)
                throw new InvalidOperationException($"Erreur lors du chargement de la configuration : {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Sauvegarde la configuration dans le fichier JSON
        /// </summary>
        public void SaveConfig(SchoolConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (string.IsNullOrWhiteSpace(config.IdEcole))
            {
                throw new ArgumentException("L'ID de l'école ne peut pas être vide.", nameof(config));
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string jsonContent = JsonSerializer.Serialize(config, options);
                File.WriteAllText(_configPath, jsonContent);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erreur lors de la sauvegarde de la configuration : {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Supprime le fichier de configuration
        /// </summary>
        public void DeleteConfig()
        {
            try
            {
                if (ConfigExists())
                {
                    File.Delete(_configPath);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erreur lors de la suppression de la configuration : {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtient le chemin complet du fichier de configuration
        /// </summary>
        public string GetConfigPath()
        {
            return _configPath;
        }
    }
}
