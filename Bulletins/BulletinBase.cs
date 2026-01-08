using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using GemBox.Spreadsheet;
using EduKin.Csharp.Admins;
using System.Diagnostics;

namespace EduKin.Bulletins
{
    /// <summary>
    /// Classe de base moderne pour la génération de bulletins Excel
    /// </summary>
    public abstract class BulletinBase : Pedagogies
    {
        protected ExcelFile _workbook;
        protected string _templatePath;
        protected string _outputPath;
        protected string _pdfPath;
        
        // Modèle de données pour un élève
        protected class EleveData
        {
            public string Matricule { get; set; }
            public string Nom { get; set; }
            public string Postnom { get; set; }
            public string Prenom { get; set; }
            public string Sexe { get; set; }
            public string LieuNaissance { get; set; }
            public string DateNaissance { get; set; }
            public string Promotion { get; set; }
        }

        // Modèle de données pour les notes
        protected class NoteData
        {
            public string Indice { get; set; }
            public decimal Cote { get; set; }
            public string Cours { get; set; }
            public int Coefficient { get; set; }
        }

        // Modèle de données pour les résultats
        protected class ResultatData
        {
            public decimal TotalPoints { get; set; }
            public decimal MaximumGeneral { get; set; }
            public decimal Pourcentage { get; set; }
            public string Statut { get; set; }
        }

        protected BulletinBase()
        {
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
        }

        /// <summary>
        /// Génère un bulletin de façon asynchrone
        /// </summary>
        public async Task<bool> GenererBulletinAsync(string matricule, string codePromo, string anneeScol, string periode, string idEcole)
        {
            try
            {
                // 1. Récupérer les données de l'élève
                var eleveData = await GetEleveDataAsync(matricule, codePromo, anneeScol);
                if (eleveData == null) return false;

                // 2. Récupérer les notes
                var notes = await GetNotesAsync(matricule, codePromo, anneeScol, periode);
                if (notes.Count == 0) return false;

                // 3. Calculer les résultats
                var resultats = CalculerResultats(notes);

                // 4. Récupérer les informations de l'école
                var ecoleInfo = await GetEcoleInfoAsync(idEcole);

                // 5. Générer le bulletin Excel
                await GenererExcelAsync(eleveData, notes, resultats, ecoleInfo, periode, anneeScol);

                // 6. Sauvegarder le coupon
                await SauvegarderCouponAsync(eleveData, resultats, periode, anneeScol, codePromo);

                // 7. Exporter en PDF et ouvrir
                await ExporterEtOuvrirPdfAsync(eleveData, anneeScol);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la génération du bulletin : {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Récupère les données de l'élève de façon asynchrone
        /// </summary>
        protected async Task<EleveData> GetEleveDataAsync(string matricule, string codePromo, string anneeScol)
        {
            var dt = await CollecteAsync(
                "SELECT Nom, Postnom, Prenom, Sexe, LieuNaiss, DateNaiss, DescripPromo " +
                "FROM View_Grilles_Globale " +
                "WHERE Matricule=@matricule AND CodPromo=@codPromo AND AnneeScol=@anneeScol " +
                "LIMIT 1",
                new MySql.Data.MySqlClient.MySqlParameter("@matricule", matricule),
                new MySql.Data.MySqlClient.MySqlParameter("@codPromo", codePromo),
                new MySql.Data.MySqlClient.MySqlParameter("@anneeScol", anneeScol));

            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new EleveData
            {
                Matricule = matricule,
                Nom = row["Nom"].ToString().Trim(),
                Postnom = row["Postnom"].ToString().Trim(),
                Prenom = row["Prenom"].ToString().Trim(),
                Sexe = row["Sexe"].ToString().Trim(),
                LieuNaissance = row["LieuNaiss"].ToString().Trim(),
                DateNaissance = row["DateNaiss"].ToString().Trim(),
                Promotion = row["DescripPromo"].ToString().Trim()
            };
        }

        /// <summary>
        /// Récupère les notes de l'élève pour une période donnée
        /// </summary>
        protected async Task<List<NoteData>> GetNotesAsync(string matricule, string codePromo, string anneeScol, string periode)
        {
            var dt = await CollecteAsync(
                "SELECT Indice, Cotes, Intitulé " +
                "FROM View_Grilles_Globale " +
                "WHERE Matricule=@matricule AND CodPromo=@codPromo AND AnneeScol=@anneeScol AND Periode=@periode " +
                "ORDER BY CAST(Indice AS UNSIGNED)",
                new MySql.Data.MySqlClient.MySqlParameter("@matricule", matricule),
                new MySql.Data.MySqlClient.MySqlParameter("@codPromo", codePromo),
                new MySql.Data.MySqlClient.MySqlParameter("@anneeScol", anneeScol),
                new MySql.Data.MySqlClient.MySqlParameter("@periode", periode));

            var notes = new List<NoteData>();
            foreach (DataRow row in dt.Rows)
            {
                if (decimal.TryParse(row["Cotes"].ToString(), out decimal cote))
                {
                    notes.Add(new NoteData
                    {
                        Indice = row["Indice"].ToString().Trim(),
                        Cote = cote,
                        Cours = row["Intitulé"].ToString().Trim(),
                        Coefficient = GetCoefficient(row["Indice"].ToString().Trim())
                    });
                }
            }
            return notes;
        }

        /// <summary>
        /// Calcule les résultats basés sur les notes
        /// </summary>
        protected ResultatData CalculerResultats(List<NoteData> notes)
        {
            decimal totalPoints = 0;
            decimal maximumGeneral = 0;

            foreach (var note in notes)
            {
                totalPoints += note.Cote;
                maximumGeneral += note.Coefficient;
            }

            decimal pourcentage = maximumGeneral > 0 ? (totalPoints * 100) / maximumGeneral : 0;
            string statut = pourcentage >= 50 ? "ADMIS" : "AJOURNÉ";

            return new ResultatData
            {
                TotalPoints = totalPoints,
                MaximumGeneral = maximumGeneral,
                Pourcentage = Math.Round(pourcentage, 2),
                Statut = statut
            };
        }

        /// <summary>
        /// Récupère les informations de l'école
        /// </summary>
        protected async Task<Dictionary<string, string>> GetEcoleInfoAsync(string idEcole)
        {
            var dt = await CollecteAsync(
                "SELECT DenominProv, DenominVille, DenominComm, DenominEcole " +
                "FROM View_Ecoles WHERE Idecole=@idecole",
                new MySql.Data.MySqlClient.MySqlParameter("@idecole", idEcole));

            var info = new Dictionary<string, string>();
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                info["Province"] = row["DenominProv"].ToString().Trim();
                info["Ville"] = row["DenominVille"].ToString().Trim();
                info["Commune"] = row["DenominComm"].ToString().Trim();
                info["Ecole"] = row["DenominEcole"].ToString().Trim();
            }
            return info;
        }

        /// <summary>
        /// Sauvegarde les données du coupon de façon asynchrone
        /// </summary>
        protected async Task SauvegarderCouponAsync(EleveData eleve, ResultatData resultats, string periode, string anneeScol, string codePromo)
        {
            var dtExistant = await CollecteAsync(
                "SELECT COUNT(*) as count FROM COUPONS " +
                "WHERE Matricule=@matricule AND CodPromo=@codPromo AND Periode=@periode AND AnneeScol=@anneeScol",
                new MySql.Data.MySqlClient.MySqlParameter("@matricule", eleve.Matricule),
                new MySql.Data.MySqlClient.MySqlParameter("@codPromo", codePromo),
                new MySql.Data.MySqlClient.MySqlParameter("@periode", periode),
                new MySql.Data.MySqlClient.MySqlParameter("@anneeScol", anneeScol));

            bool existe = Convert.ToInt32(dtExistant.Rows[0]["count"]) > 0;

            if (existe)
            {
                await MiseAJourAsync(
                    "UPDATE COUPONS SET Nom=@nom, Postnom=@postnom, Prenom=@prenom, Sexe=@sexe, " +
                    "MaxGen=@maxGen, Totaux=@totaux, Pourc=@pourc " +
                    "WHERE Matricule=@matricule AND CodPromo=@codPromo AND AnneeScol=@anneeScol AND Periode=@periode",
                    new MySql.Data.MySqlClient.MySqlParameter("@nom", eleve.Nom),
                    new MySql.Data.MySqlClient.MySqlParameter("@postnom", eleve.Postnom),
                    new MySql.Data.MySqlClient.MySqlParameter("@prenom", eleve.Prenom),
                    new MySql.Data.MySqlClient.MySqlParameter("@sexe", eleve.Sexe),
                    new MySql.Data.MySqlClient.MySqlParameter("@maxGen", resultats.MaximumGeneral),
                    new MySql.Data.MySqlClient.MySqlParameter("@totaux", resultats.TotalPoints),
                    new MySql.Data.MySqlClient.MySqlParameter("@pourc", resultats.Pourcentage),
                    new MySql.Data.MySqlClient.MySqlParameter("@matricule", eleve.Matricule),
                    new MySql.Data.MySqlClient.MySqlParameter("@codPromo", codePromo),
                    new MySql.Data.MySqlClient.MySqlParameter("@anneeScol", anneeScol),
                    new MySql.Data.MySqlClient.MySqlParameter("@periode", periode));
            }
            else
            {
                await MiseAJourAsync(
                    "INSERT INTO COUPONS (Matricule, Nom, Postnom, Prenom, Sexe, Periode, MaxGen, Totaux, Pourc, CodPromo, AnneeScol) " +
                    "VALUES (@matricule, @nom, @postnom, @prenom, @sexe, @periode, @maxGen, @totaux, @pourc, @codPromo, @anneeScol)",
                    new MySql.Data.MySqlClient.MySqlParameter("@matricule", eleve.Matricule),
                    new MySql.Data.MySqlClient.MySqlParameter("@nom", eleve.Nom),
                    new MySql.Data.MySqlClient.MySqlParameter("@postnom", eleve.Postnom),
                    new MySql.Data.MySqlClient.MySqlParameter("@prenom", eleve.Prenom),
                    new MySql.Data.MySqlClient.MySqlParameter("@sexe", eleve.Sexe),
                    new MySql.Data.MySqlClient.MySqlParameter("@periode", periode),
                    new MySql.Data.MySqlClient.MySqlParameter("@maxGen", resultats.MaximumGeneral),
                    new MySql.Data.MySqlClient.MySqlParameter("@totaux", resultats.TotalPoints),
                    new MySql.Data.MySqlClient.MySqlParameter("@pourc", resultats.Pourcentage),
                    new MySql.Data.MySqlClient.MySqlParameter("@codPromo", codePromo),
                    new MySql.Data.MySqlClient.MySqlParameter("@anneeScol", anneeScol));
            }
        }

        /// <summary>
        /// Exporte en PDF et ouvre le fichier
        /// </summary>
        protected async Task ExporterEtOuvrirPdfAsync(EleveData eleve, string anneeScol)
        {
            await Task.Run(() =>
            {
                try
                {
                    string nomFichier = $"{eleve.Nom}_{eleve.Postnom}_{eleve.Prenom}_{anneeScol}";
                    string pdfPath = Path.Combine(_pdfPath, $"{nomFichier}.pdf");
                    
                    // Créer le répertoire s'il n'existe pas
                    Directory.CreateDirectory(Path.GetDirectoryName(pdfPath));
                    
                    // Exporter en PDF
                    _workbook.Save(pdfPath, SaveOptions.PdfDefault);
                    
                    // Ouvrir le PDF
                    Process.Start(pdfPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de l'export PDF : {ex.Message}", 
                        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            });
        }

        /// <summary>
        /// Obtient le coefficient basé sur l'indice du cours
        /// </summary>
        protected virtual int GetCoefficient(string indice)
        {
            if (int.TryParse(indice, out int idx))
            {
                return idx switch
                {
                    >= 1 and <= 6 => 10,   // Cours de base
                    >= 7 and <= 12 => 20,  // Cours intermédiaires
                    >= 13 and <= 14 => 30, // Cours avancés
                    >= 15 and <= 16 => 40, // Cours spécialisés
                    17 => 60,              // Cours principal
                    18 => 100,             // Cours majeur
                    _ => 10                // Par défaut
                };
            }
            return 10;
        }

        /// <summary>
        /// Crée les répertoires nécessaires
        /// </summary>
        protected void CreerRepertoires()
        {
            Directory.CreateDirectory(_outputPath);
            Directory.CreateDirectory(_pdfPath);
        }

        // Méthodes abstraites à implémenter par les classes dérivées
        protected abstract Task GenererExcelAsync(EleveData eleve, List<NoteData> notes, ResultatData resultats, Dictionary<string, string> ecoleInfo, string periode, string anneeScol);
        protected abstract void ConfigurerChemins(string codeOption);
        protected abstract void ChargerTemplate();
    }
}