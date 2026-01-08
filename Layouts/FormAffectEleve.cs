using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dapper;
using EduKin.DataSets;
using EduKin.Inits;
using EduKinContext = EduKin.Inits.EduKinContext;

namespace EduKin.Layouts
{
    /// <summary>
    /// Formulaire d'affectation des élèves (année scolaire, section, option, promotion)
    /// </summary>
    public partial class FormAffectEleve : Form
    {
        private readonly Connexion _connexion;
        private readonly string _matriculeEleve;

        // Propriétés publiques pour retourner les valeurs sélectionnées
        public string SelectedAnneeScolaire { get; private set; } = string.Empty;
        public string SelectedCodeSection { get; private set; } = string.Empty;
        public string SelectedNomSection { get; private set; } = string.Empty;
        public string SelectedCodeOption { get; private set; } = string.Empty;
        public string SelectedNomOption { get; private set; } = string.Empty;
        public string SelectedCodePromotion { get; private set; } = string.Empty;
        public string SelectedNomPromotion { get; private set; } = string.Empty;
        public string SelectedIndicePromotion { get; private set; } = string.Empty;
        public bool IsAffectationValid { get; private set; } = false;

        /// <summary>
        /// Constructeur avec valeurs actuelles
        /// </summary>
        public FormAffectEleve(
            string matriculeEleve,
            string? currentAnneeScolaire = null,
            string? currentCodePromotion = null,
            string? currentIndicePromotion = null)
        {
            InitializeComponent();
            _connexion = Connexion.Instance;
            _matriculeEleve = matriculeEleve;

            // Stocker les valeurs actuelles
            SelectedAnneeScolaire = currentAnneeScolaire ?? string.Empty;
            SelectedCodePromotion = currentCodePromotion ?? string.Empty;
            SelectedIndicePromotion = currentIndicePromotion ?? string.Empty;

            InitializeForm();
        }
        /// <summary>
        /// Initialise le formulaire
        /// </summary>
        private void InitializeForm()
        {
            this.Text = $"Affectation de l'élève - {_matriculeEleve}";

            // Connecter les événements
            TxtIndicePromotion.TextChanged += TxtIndicePromotion_TextChanged;

            // Charger les données
            LoadAnnesScolaires();

            // Charger les affectations actuelles si elles existent
            if (!string.IsNullOrEmpty(SelectedAnneeScolaire))
            {
                SetInitialAffectation(SelectedAnneeScolaire, SelectedCodePromotion, SelectedIndicePromotion);
            }

     
      
        }

        #region Méthodes Publiques

        /// <summary>
        /// Définit l'affectation initiale dans le formulaire
        /// </summary>
        public void SetInitialAffectation(string anneeScolaire, string codePromotion, string indicePromotion)
        {
            try
            {
                // Charger d'abord les années scolaires
                LoadAnnesScolaires();

                if (!string.IsNullOrWhiteSpace(anneeScolaire))
                {
                    // Sélectionner l'année scolaire
                    for (int i = 0; i < CmbAnneeScolaire.Items.Count; i++)
                    {
                        if (CmbAnneeScolaire.Items[i]?.ToString() == anneeScolaire)
                        {
                            CmbAnneeScolaire.SelectedIndex = i;
                            break;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(codePromotion))
                    {
                        // Charger la hiérarchie pour retrouver la promotion
                        LoadPromotionHierarchy(codePromotion);
                    }
                }

                if (!string.IsNullOrWhiteSpace(indicePromotion))
                {
                    TxtIndicePromotion.Text = indicePromotion;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'initialisation de l'affectation : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Chargement des Données

        /// <summary>
        /// Charge les années scolaires disponibles
        /// </summary>
        private void LoadAnnesScolaires()
        {
            try
            {
                CmbAnneeScolaire.Items.Clear();

                // Générer les années scolaires (année courante et suivantes)
                var currentYear = DateTime.Now.Year;
                var currentMonth = DateTime.Now.Month;

                // Si on est après juin, l'année scolaire courante commence cette année
                // Sinon, elle a commencé l'année précédente
                var startYear = currentMonth >= 7 ? currentYear : currentYear - 1;

                for (int i = 0; i < 5; i++) // 5 années scolaires
                {
                    var year = startYear + i;
                    var anneeScolaire = $"{year}-{year + 1}";
                    CmbAnneeScolaire.Items.Add(anneeScolaire);
                }

                // Sélectionner l'année courante par défaut
                if (CmbAnneeScolaire.Items.Count > 0)
                {
                    CmbAnneeScolaire.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des années scolaires : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Charge les sections disponibles pour l'école courante
        /// </summary>
        private void LoadSections()
        {
            try
            {
                CmbSection.Items.Clear();
                CmbSection.DisplayMember = "Text";
                CmbSection.ValueMember = "Value";
                CmbSection.Enabled = false;

                if (!EduKinContext.IsConfigured)
                {
                    lblInfo.Text = "Aucune école configurée.";
                    return;
                }

                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();

                    // Récupérer les sections affectées à l'école courante
                    var query = @"SELECT DISTINCT s.cod_sect, s.description 
                                  FROM t_sections s
                                  INNER JOIN t_affect_sect a ON s.cod_sect = a.cod_sect
                                  WHERE a.id_ecole = @IdEcole
                                  ORDER BY s.description";

                    var sections = conn.Query(query, new { IdEcole = EduKinContext.CurrentIdEcole });

                    foreach (var section in sections)
                    {
                        var item = new { Text = section.description, Value = section.cod_sect };
                        CmbSection.Items.Add(item);
                    }

                    if (CmbSection.Items.Count > 0)
                    {
                        CmbSection.Enabled = true;
                        lblInfo.Text = "Sélectionnez une section.";
                    }
                    else
                    {
                        lblInfo.Text = "Aucune section disponible pour cette école.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des sections : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblInfo.Text = "Erreur lors du chargement des sections.";
            }
        }

        /// <summary>
        /// Charge les options disponibles pour la section sélectionnée
        /// </summary>
        private void LoadOptions(string codeSection)
        {
            try
            {
                CmbOption.Items.Clear();
                CmbOption.DisplayMember = "Text";
                CmbOption.ValueMember = "Value";
                CmbOption.Enabled = false;
                ClearPromotions();

                if (string.IsNullOrWhiteSpace(codeSection))
                {
                    return;
                }

                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();

                    var query = @"SELECT cod_opt, description 
                                  FROM t_options 
                                  WHERE cod_sect = @CodeSection
                                  ORDER BY description";

                    var options = conn.Query(query, new { CodeSection = codeSection });

                    foreach (var option in options)
                    {
                        var item = new { Text = option.description, Value = option.cod_opt };
                        CmbOption.Items.Add(item);
                    }

                    if (CmbOption.Items.Count > 0)
                    {
                        CmbOption.Enabled = true;
                        lblInfo.Text = "Sélectionnez une option.";
                    }
                    else
                    {
                        lblInfo.Text = "Aucune option disponible pour cette section.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des options : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblInfo.Text = "Erreur lors du chargement des options.";
            }
        }

        /// <summary>
        /// Charge les promotions disponibles pour l'option sélectionnée
        /// </summary>
        private void LoadPromotions(string codeOption)
        {
            try
            {
                CmbPromotion.Items.Clear();
                CmbPromotion.DisplayMember = "Text";
                CmbPromotion.ValueMember = "Value";
                CmbPromotion.Enabled = false;

                if (string.IsNullOrWhiteSpace(codeOption))
                {
                    return;
                }

                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();

                    var query = @"SELECT cod_promo, description 
                                  FROM t_promotions 
                                  WHERE cod_opt = @CodeOption
                                  ORDER BY description";

                    var promotions = conn.Query(query, new { CodeOption = codeOption });

                    foreach (var promotion in promotions)
                    {
                        var item = new { Text = promotion.description, Value = promotion.cod_promo };
                        CmbPromotion.Items.Add(item);
                    }

                    if (CmbPromotion.Items.Count > 0)
                    {
                        CmbPromotion.Enabled = true;
                        lblInfo.Text = "Sélectionnez une promotion.";
                    }
                    else
                    {
                        lblInfo.Text = "Aucune promotion disponible pour cette option.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des promotions : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblInfo.Text = "Erreur lors du chargement des promotions.";
            }
        }

        /// <summary>
        /// Charge la hiérarchie complète pour une promotion donnée
        /// </summary>
        private void LoadPromotionHierarchy(string codePromotion)
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();

                    var query = @"SELECT p.cod_promo, p.description as promo_desc,
                                         o.cod_opt, o.description as option_desc,
                                         s.cod_sect, s.description as section_desc
                                  FROM t_promotions p
                                  INNER JOIN t_options o ON p.cod_opt = o.cod_opt
                                  INNER JOIN t_sections s ON o.cod_sect = s.cod_sect
                                  WHERE p.cod_promo = @CodePromotion";

                    var result = conn.QueryFirstOrDefault(query, new { CodePromotion = codePromotion });

                    if (result != null)
                    {
                        // Charger et sélectionner la section
                        LoadSections();
                        SelectComboBoxItem(CmbSection, result.cod_sect);

                        // Charger et sélectionner l'option
                        LoadOptions(result.cod_sect);
                        SelectComboBoxItem(CmbOption, result.cod_opt);

                        // Charger et sélectionner la promotion
                        LoadPromotions(result.cod_opt);
                        SelectComboBoxItem(CmbPromotion, result.cod_promo);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de la hiérarchie : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Méthodes Utilitaires

        /// <summary>
        /// Sélectionne un élément dans une ComboBox par sa valeur
        /// </summary>
        private void SelectComboBoxItem(ComboBox comboBox, string value)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                var item = comboBox.Items[i];
                // On suppose que chaque item est un objet anonyme avec les propriétés 'Text' et 'Value'
                var valueProp = item.GetType().GetProperty("Value");
                if (valueProp != null)
                {
                    var itemValue = valueProp.GetValue(item)?.ToString();
                    if (itemValue == value)
                    {
                        comboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Vide les ComboBox des promotions
        /// </summary>
        private void ClearPromotions()
        {
            CmbPromotion.Items.Clear();
            CmbPromotion.Enabled = false;
        }

        /// <summary>
        /// Valide que l'affectation est complète
        /// </summary>
        private bool ValidateAffectation()
        {
            try
            {
                // Vérifier que tous les champs sont remplis
                if (string.IsNullOrWhiteSpace(SelectedAnneeScolaire))
                {
                    lblInfo.Text = "Veuillez sélectionner une année scolaire.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(SelectedCodePromotion))
                {
                    lblInfo.Text = "Veuillez sélectionner une promotion.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(TxtIndicePromotion.Text.Trim()))
                {
                    lblInfo.Text = "Veuillez saisir un indice de promotion.";
                    return false;
                }

                // Valider le format de l'indice
                var indice = TxtIndicePromotion.Text.Trim();
                if (indice.Length > 10)
                {
                    lblInfo.Text = "L'indice de promotion ne peut pas dépasser 10 caractères.";
                    return false;
                }

                SelectedIndicePromotion = indice;
                lblInfo.Text = "Affectation valide. Cliquez sur OK pour confirmer.";
                return true;
            }
            catch (Exception ex)
            {
                lblInfo.Text = $"Erreur de validation : {ex.Message}";
                return false;
            }
        }

        #endregion
        /// <summary>
        /// Gère le clic sur le bouton Annuler
        /// </summary>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void FormAffectEleve_Load(object sender, EventArgs e)
        {
            try
            {
                LoadAnnesScolaires();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du formulaire : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbAnneeScolaire_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (CmbAnneeScolaire.SelectedItem != null)
                {
                    SelectedAnneeScolaire = CmbAnneeScolaire.SelectedItem.ToString() ?? string.Empty;
                    LoadSections();
                }
                else
                {
                    SelectedAnneeScolaire = string.Empty;
                }

                // Réinitialiser les sélections suivantes
                SelectedCodeSection = string.Empty;
                SelectedNomSection = string.Empty;
                SelectedCodeOption = string.Empty;
                SelectedNomOption = string.Empty;
                SelectedCodePromotion = string.Empty;
                SelectedNomPromotion = string.Empty;

                CmbOption.Items.Clear();
                CmbOption.Enabled = false;
                ClearPromotions();

                IsAffectationValid = ValidateAffectation();
                BtnAffectEleve.Enabled = IsAffectationValid;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du changement d'année scolaire : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var selectedItem = CmbSection.SelectedItem;
                if (selectedItem != null)
                {
                    var valueProp = selectedItem.GetType().GetProperty("Value");
                    var textProp = selectedItem.GetType().GetProperty("Text");
                    SelectedCodeSection = valueProp?.GetValue(selectedItem)?.ToString() ?? string.Empty;
                    SelectedNomSection = textProp?.GetValue(selectedItem)?.ToString() ?? string.Empty;
                    LoadOptions(SelectedCodeSection);
                }
                else
                {
                    SelectedCodeSection = string.Empty;
                    SelectedNomSection = string.Empty;
                }

                // Réinitialiser les sélections suivantes
                SelectedCodeOption = string.Empty;
                SelectedNomOption = string.Empty;
                SelectedCodePromotion = string.Empty;
                SelectedNomPromotion = string.Empty;

                ClearPromotions();

                IsAffectationValid = ValidateAffectation();
                BtnAffectEleve.Enabled = IsAffectationValid;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du changement de section : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var selectedItem = CmbOption.SelectedItem;
                if (selectedItem != null)
                {
                    var valueProp = selectedItem.GetType().GetProperty("Value");
                    var textProp = selectedItem.GetType().GetProperty("Text");
                    SelectedCodeOption = valueProp?.GetValue(selectedItem)?.ToString() ?? string.Empty;
                    SelectedNomOption = textProp?.GetValue(selectedItem)?.ToString() ?? string.Empty;
                    LoadPromotions(SelectedCodeOption);
                }
                else
                {
                    SelectedCodeOption = string.Empty;
                    SelectedNomOption = string.Empty;
                }

                // Réinitialiser la sélection de promotion
                SelectedCodePromotion = string.Empty;
                SelectedNomPromotion = string.Empty;

                IsAffectationValid = ValidateAffectation();
                BtnAffectEleve.Enabled = IsAffectationValid;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du changement d'option : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbPromotion_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var selectedItem = CmbPromotion.SelectedItem;
                if (selectedItem != null)
                {
                    var valueProp = selectedItem.GetType().GetProperty("Value");
                    var textProp = selectedItem.GetType().GetProperty("Text");
                    SelectedCodePromotion = valueProp?.GetValue(selectedItem)?.ToString() ?? string.Empty;
                    SelectedNomPromotion = textProp?.GetValue(selectedItem)?.ToString() ?? string.Empty;
                }
                else
                {
                    SelectedCodePromotion = string.Empty;
                    SelectedNomPromotion = string.Empty;
                }

                IsAffectationValid = ValidateAffectation();
                BtnAffectEleve.Enabled = IsAffectationValid;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du changement de promotion : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gère le changement de texte dans l'indice de promotion
        /// </summary>
        private void TxtIndicePromotion_TextChanged(object sender, EventArgs e)
        {
            try
            {
                IsAffectationValid = ValidateAffectation();
                BtnAffectEleve.Enabled = IsAffectationValid;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la validation de l'indice : {ex.Message}");
            }
        }

        private void BtnAffectEleve_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateAffectation())
                {
                    // Marquer l'affectation comme valide
                    IsAffectationValid = true;
                    
                    // Retourner au formulaire principal avec DialogResult.OK
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la validation : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnQuitter_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
