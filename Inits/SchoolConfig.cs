using System;

namespace EduKin.Inits
{
    /// <summary>
    /// Modèle de configuration de l'école pour la machine
    /// </summary>
    public class SchoolConfig
    {
        /// <summary>
        /// Identifiant unique de l'école (clé primaire de t_ecoles)
        /// </summary>
        public string IdEcole { get; set; } = string.Empty;

        /// <summary>
        /// Dénomination de l'école
        /// </summary>
        public string Denomination { get; set; } = string.Empty;

        /// <summary>
        /// Date de configuration sur cette machine
        /// </summary>
        public DateTime ConfiguredDate { get; set; }
    }
}
