using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using GemBox.Spreadsheet;


namespace EduKin.Bulletins 
{
    /// <summary>
    /// Générateur de bulletins pour l'option 000 (Septième année)
    /// Version moderne et optimisée
    /// </summary>
    public class B_Opt_000 : BulletinBase
    {
        private readonly Dictionary<string, string> _cellMapping;

        public B_Opt_000() : base()
        {
            ConfigurerChemins("000");
            _cellMapping = InitialiserMappingCellules();
        }

        /// <summary>
        /// Configure les chemins spécifiques à l'option 000
        /// </summary>
        protected override void ConfigurerChemins(string codeOption)
        {
            string userPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _outputPath = Path.Combine(userPath, "MonEcole", "Bulletin", codeOption, "Septième");
            _pdfPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MonEcole", "PdfExport", codeOption, "Septième");
            _templatePath = "EB_78.xlsx";
        }

        /// <summary>
        /// Charge le template Excel
        /// </summary>
        protected override void ChargerTemplate()
        {
            _workbook = ExcelFile.Load(Path.GetFullPath(_templatePath));
        }

        /// <summary>
        /// Initialise le mapping des cellules pour chaque indice de cours
        /// </summary>
        private Dictionary<string, string> InitialiserMappingCellules()
        {
            return new Dictionary<string, string>
            {
                {"1", "D14"}, {"2", "D15"}, {"3", "D16"}, {"4", "D17"},
                {"5", "D20"}, {"6", "D21"}, {"7", "D22"}, {"8", "D25"},
                {"9", "D26"}, {"10", "B23"}, {"11", "B24"}, {"12", "B26"},
                {"13", "B28"}, {"14", "B29"}, {"15", "B31"}, {"16", "B32"},
                {"17", "B34"}, {"18", "B36"}
            };
        }

        /// <summary>
        /// Génère le fichier Excel avec les données de l'élève
        /// </summary>
        protected override async Task GenererExcelAsync(EleveData eleve, List<NoteData> notes, ResultatData resultats, Dictionary<string, string> ecoleInfo, string periode, string anneeScol)
        {
            ChargerTemplate();
            var worksheet = _workbook.Worksheets.ActiveWorksheet;

            // Remplir les informations de l'école
            RemplirInfosEcole(worksheet, ecoleInfo, anneeScol);

            // Remplir les informations de l'élève
            RemplirInfosEleve(worksheet, eleve);

            // Remplir les notes selon la période
            await RemplirNotesSelonPeriode(worksheet, notes, periode);

            // Remplir les totaux
            RemplirTotaux(worksheet, resultats);

            // Sauvegarder le fichier Excel
            CreerRepertoires();
            string nomFichier = $"{eleve.Nom}_{eleve.Postnom}_{eleve.Prenom}_{anneeScol}";
            string excelPath = Path.Combine(_outputPath, $"{nomFichier}.xlsx");
            _workbook.Save(excelPath, SaveOptions.XlsxDefault);
        }

        /// <summary>
        /// Remplit les informations de l'école dans le worksheet
        /// </summary>
        private void RemplirInfosEcole(ExcelWorksheet worksheet, Dictionary<string, string> ecoleInfo, string anneeScol)
        {
            if (ecoleInfo.ContainsKey("Province"))
                worksheet.Cells["A3"].Value = $"PROVINCE  :  {ecoleInfo["Province"]}";
            if (ecoleInfo.ContainsKey("Ville"))
                worksheet.Cells["A4"].Value = $"VILLE  :  {ecoleInfo["Ville"]}";
            if (ecoleInfo.ContainsKey("Commune"))
                worksheet.Cells["A5"].Value = $"COMMUNE / TERR (1) : {ecoleInfo["Commune"]}";
            if (ecoleInfo.ContainsKey("Ecole"))
                worksheet.Cells["A6"].Value = $"ECOLE  :  {ecoleInfo["Ecole"]}";

            worksheet.Cells["A8"].Value = $"BULLETIN  DE LA 1ère & 2ème  ANNEE HUMANITE COMMERCIALE & GESTION      ANNEE SCOLAIRE {anneeScol}";
        }

        /// <summary>
        /// Remplit les informations de l'élève dans le worksheet
        /// </summary>
        private void RemplirInfosEleve(ExcelWorksheet worksheet, EleveData eleve)
        {
            worksheet.Cells["J4"].Value = $"ELEVE : {eleve.Nom} {eleve.Postnom} {eleve.Prenom}";
            worksheet.Cells["Q4"].Value = $"SEXE : {eleve.Sexe}";
            worksheet.Cells["J5"].Value = $"NE(E) A : {eleve.LieuNaissance}";
            worksheet.Cells["Q5"].Value = $"LE : {eleve.DateNaissance}";
        }

        /// <summary>
        /// Remplit les notes selon la période
        /// </summary>
        private async Task RemplirNotesSelonPeriode(ExcelWorksheet worksheet, List<NoteData> notes, string periode)
        {
            string colonneBase = GetColonnePeriode(periode);
            
            foreach (var note in notes)
            {
                if (_cellMapping.ContainsKey(note.Indice))
                {
                    string cellule = _cellMapping[note.Indice];
                    if (!string.IsNullOrEmpty(colonneBase))
                    {
                        // Adapter la cellule selon la période (B, C, D, E, F, G)
                        cellule = colonneBase + cellule.Substring(1);
                    }
                    worksheet.Cells[cellule].Value = note.Cote;
                }
            }
        }

        /// <summary>
        /// Obtient la colonne correspondant à la période
        /// </summary>
        private string GetColonnePeriode(string periode)
        {
            return periode switch
            {
                "Première Période" => "B",
                "Deuxième Période" => "C",
                "Examen Semestre 1" => "D",
                "Troisième Période" => "E",
                "Quatrième Période" => "F",
                "Examen Semestre 2" => "G",
                _ => "B"
            };
        }

        /// <summary>
        /// Remplit les totaux dans le worksheet
        /// </summary>
        private void RemplirTotaux(ExcelWorksheet worksheet, ResultatData resultats)
        {
            worksheet.Cells["B37"].Value = resultats.MaximumGeneral;
            worksheet.Cells["B38"].Value = resultats.TotalPoints;
            worksheet.Cells["B39"].Value = resultats.Pourcentage;
        }

        /// <summary>
        /// Point d'entrée principal pour générer un bulletin (toutes périodes)
        /// </summary>
        public async Task<bool> GenererBulletinAsync(string matricule, string codePromo, string anneeScol, string periode, string idEcole)
        {
            return await base.GenererBulletinAsync(matricule, codePromo, anneeScol, periode, idEcole);
        }
    }
}