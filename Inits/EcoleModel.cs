namespace EduKin.Inits
{
    /// <summary>
    /// Modèle typé pour les données d'une école
    /// </summary>
    public class EcoleModel
    {
        public string IdEcole { get; set; } = string.Empty;
        public string Denomination { get; set; } = string.Empty;
        public string AnneeScol { get; set; } = string.Empty;
        public string? Adresse { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
    }
}