using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dapper;
using EduKin.DataSets;
using EduKin.Inits;
using Siticone.Desktop.UI.WinForms;

namespace EduKin.Layouts
{
    /// <summary>
    /// Formulaire d'affectation des options aux sections d'une école
    /// </summary>
    public partial class FormAffectSection : Form
    {
        private readonly Connexion _connexion;
        private string _selectedSectionCode = string.Empty;
        private int _selectedSectionAffectId = 0;
        private List<OptionItem> _currentOptions = new List<OptionItem>();
        private List<SiticonePanel> _optionPanels = new List<SiticonePanel>();

        /// <summary>
        /// Classe pour représenter une option
        /// </summary>
        private class OptionItem
        {
            public string CodeOption { get; set; } = string.Empty;
            public string NomOption { get; set; } = string.Empty;
            public string CodeEpst { get; set; } = string.Empty;
            public bool IsAffected { get; set; } = false;
            public bool IsSelected { get; set; } = false;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        public FormAffectSection()
        {
            InitializeComponent();
            _connexion = Connexion.Instance;
        }

        #region Chargement des Données

        /// <summary>
        /// Charge les sections affectées à l'école courante
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

                    // Récupérer TOUTES les sections disponibles pour l'école courante
                    var query = @"SELECT s.cod_sect, s.description, 
                                  CASE WHEN a.num_affect IS NOT NULL THEN a.num_affect ELSE 0 END as num_affect
                                  FROM t_sections s
                                  LEFT JOIN t_affect_sect a ON s.cod_sect = a.cod_sect AND a.id_ecole = @IdEcole
                                  ORDER BY s.description";

                    var sections = conn.Query(query, new { IdEcole = EduKinContext.CurrentIdEcole });

                    foreach (var section in sections)
                    {
                        var item = new
                        {
                            Text = section.description,
                            Value = section.cod_sect,
                            AffectId = section.num_affect
                        };
                        CmbSection.Items.Add(item);
                    }

                    if (CmbSection.Items.Count > 0)
                    {
                        CmbSection.Enabled = true;
                        lblInfo.Text = "Sélectionnez une section pour afficher ses options.";
                    }
                    else
                    {
                        lblInfo.Text = "Aucune section disponible dans la base de données.";
                        ClearOptionsPanel();
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
        private void LoadOptionsForSection(string codeSection, int affectId)
        {
            try
            {
                _currentOptions.Clear();
                _selectedSectionCode = codeSection;
                _selectedSectionAffectId = affectId;

                if (string.IsNullOrWhiteSpace(codeSection))
                {
                    ClearOptionsPanel();
                    return;
                }

                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();

                    // Récupérer toutes les options de la section avec leur statut d'affectation
                    var query = @"SELECT o.cod_opt, o.description, o.code_epst,
                                         CASE WHEN ao.num_affect_opt IS NOT NULL THEN 1 ELSE 0 END as is_affected
                                  FROM t_options o
                                  LEFT JOIN t_affect_options ao ON o.cod_opt = ao.cod_opt AND ao.num_affect_sect = @AffectId
                                  WHERE o.cod_sect = @CodeSection
                                  ORDER BY o.description";

                    var options = conn.Query(query, new { CodeSection = codeSection, AffectId = affectId });

                    foreach (var option in options)
                    {
                        _currentOptions.Add(new OptionItem
                        {
                            CodeOption = option.cod_opt,
                            NomOption = option.description,
                            CodeEpst = option.code_epst ?? "N/A",
                            IsAffected = option.is_affected == 1,
                            IsSelected = false
                        });
                    }

                    DisplayOptions();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des options : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblInfo.Text = "Erreur lors du chargement des options.";
                ClearOptionsPanel();
            }
        }

        #endregion

        #region Affichage des Options

        /// <summary>
        /// Affiche les options dans le panel avec un style moderne
        /// </summary>
        private void DisplayOptions()
        {
            try
            {
                // Nettoyer les anciens contrôles sans redraw
                ClearOptionsPanel();

                if (_currentOptions.Count == 0)
                {
                    lblNoOptions.Text = "Aucune option disponible pour cette section.";
                    lblNoOptions.Visible = true;
                    BtnAffecterOptions.Enabled = false;
                    return;
                }

                lblNoOptions.Visible = false;
                panelOptions.SuspendLayout();

                // Calculer les dimensions
                int panelWidth = panelOptions.Width - 40; // Marge de 20px de chaque côté
                int columnWidth = (panelWidth - 20) / 2; // 2 colonnes avec espacement de 20px
                int itemHeight = 50;
                int spacing = 10;
                int currentRow = 0;

                for (int i = 0; i < _currentOptions.Count; i++)
                {
                    var option = _currentOptions[i];
                    int column = i % 2;
                    int row = i / 2;

                    // Créer le panel pour l'option
                    var optionPanel = CreateOptionPanel(option, column, row, columnWidth, itemHeight, spacing);
                    _optionPanels.Add(optionPanel);
                    panelOptions.Controls.Add(optionPanel);

                    currentRow = row;
                }

                // Ajuster la hauteur du panel de défilement si nécessaire
                int totalHeight = (currentRow + 1) * (itemHeight + spacing) + 20;
                if (totalHeight > panelOptions.Height)
                {
                    panelOptions.AutoScrollMinSize = new Size(0, totalHeight);
                }

                panelOptions.ResumeLayout();
                UpdateAffectButtonState();

                lblInfo.Text = $"{_currentOptions.Count} option(s) trouvée(s). Sélectionnez celles à affecter.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'affichage des options : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Crée un panel stylé pour une option
        /// </summary>
        private SiticonePanel CreateOptionPanel(OptionItem option, int column, int row, int columnWidth, int itemHeight, int spacing)
        {
            var optionPanel = new SiticonePanel
            {
                Size = new Size(columnWidth, itemHeight),
                Location = new Point(20 + column * (columnWidth + 20), 10 + row * (itemHeight + spacing)),
                BorderRadius = 14,
                FillColor = option.IsAffected ? Color.FromArgb(220, 248, 198) : Color.White,
                BorderColor = option.IsAffected ? Color.FromArgb(40, 167, 69) : Color.FromArgb(213, 218, 223),
                BorderThickness = 2,
                Cursor = Cursors.Hand
            };

            // CheckBox
            var checkBox = new SiticoneCheckBox
            {
                Location = new Point(10, 15),
                Size = new Size(20, 20),
                Checked = option.IsSelected,
                Enabled = !option.IsAffected,
                CheckedState = { FillColor = Color.FromArgb(94, 148, 255) }
            };

            // Label pour le nom de l'option
            var lblNom = new Label
            {
                Text = option.NomOption,
                Location = new Point(40, 8),
                Size = new Size(columnWidth - 120, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = option.IsAffected ? Color.FromArgb(40, 167, 69) : Color.FromArgb(64, 64, 64),
                BackColor = Color.Transparent
            };

            // Label pour le code EPST
            var lblCode = new Label
            {
                Text = $"Code: {option.CodeEpst}",
                Location = new Point(40, 25),
                Size = new Size(columnWidth - 120, 20),
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(108, 117, 125),
                BackColor = Color.Transparent
            };

            // Label de statut
            var lblStatut = new Label
            {
                Text = option.IsAffected ? "✓ Affectée" : "Non affectée",
                Location = new Point(columnWidth - 80, 15),
                Size = new Size(70, 20),
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = option.IsAffected ? Color.FromArgb(40, 167, 69) : Color.FromArgb(108, 117, 125),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleRight
            };

            // Événements
            checkBox.CheckedChanged += (s, e) =>
            {
                option.IsSelected = checkBox.Checked;
                UpdateAffectButtonState();
            };

            // Clic sur le panel pour toggle le checkbox
            optionPanel.Click += (s, e) =>
            {
                if (!option.IsAffected)
                {
                    checkBox.Checked = !checkBox.Checked;
                }
            };

            // Ajouter les contrôles au panel
            optionPanel.Controls.AddRange(new Control[] { checkBox, lblNom, lblCode, lblStatut });

            return optionPanel;
        }

        /// <summary>
        /// Vide le panel des options
        /// </summary>
        private void ClearOptionsPanel()
        {
            panelOptions.SuspendLayout();

            // Supprimer tous les panels d'options
            foreach (var panel in _optionPanels)
            {
                panelOptions.Controls.Remove(panel);
                panel.Dispose();
            }
            _optionPanels.Clear();

            lblNoOptions.Text = "Aucune option à afficher";
            lblNoOptions.Visible = true;

            panelOptions.ResumeLayout();
            BtnAffecterOptions.Enabled = false;
        }

        /// <summary>
        /// Met à jour l'état du bouton d'affectation
        /// </summary>
        private void UpdateAffectButtonState()
        {
            bool hasSelectedOptions = _currentOptions.Any(o => o.IsSelected && !o.IsAffected);
            BtnAffecterOptions.Enabled = hasSelectedOptions;

            if (hasSelectedOptions)
            {
                int selectedCount = _currentOptions.Count(o => o.IsSelected && !o.IsAffected);
                lblInfo.Text = $"{selectedCount} option(s) sélectionnée(s) pour affectation.";
            }
            else
            {
                lblInfo.Text = "Sélectionnez des options non affectées pour les affecter.";
            }
        }

        #endregion

        #region Affectation des Options

        /// <summary>
        /// Affecte les options sélectionnées à la section
        /// </summary>
        private async void AffectSelectedOptions()
        {
            try
            {
                var selectedOptions = _currentOptions.Where(o => o.IsSelected && !o.IsAffected).ToList();

                if (selectedOptions.Count == 0)
                {
                    MessageBox.Show("Aucune option sélectionnée pour l'affectation.",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var result = MessageBox.Show(
                    $"Voulez-vous affecter {selectedOptions.Count} option(s) à cette section ?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;

                BtnAffecterOptions.Enabled = false;
                BtnAffecterOptions.Text = "Affectation...";

                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Vérifier si la section est déjà affectée à l'école
                            // Si _selectedSectionAffectId est 0, c'est qu'elle n'est pas encore affectée dans l'interface
                            // Mais on vérifie quand même en base pour être sûr (cas multi-utilisateurs)
                            
                            int affectSectId = _selectedSectionAffectId;
                            
                            if (affectSectId == 0)
                            {
                                // Vérification en base
                                var checkQuery = "SELECT num_affect FROM t_affect_sect WHERE id_ecole = @IdEcole AND cod_sect = @CodSect";
                                var existingId = await conn.QueryFirstOrDefaultAsync<int?>(checkQuery, new 
                                { 
                                    IdEcole = EduKinContext.CurrentIdEcole, 
                                    CodSect = _selectedSectionCode 
                                }, transaction);
                                
                                if (existingId.HasValue && existingId.Value > 0)
                                {
                                    affectSectId = existingId.Value;
                                }
                                else
                                {
                                    // La section n'est pas affectée, on l'affecte
                                    var insertNoteQuery = @"INSERT INTO t_affect_sect (id_ecole, cod_sect, date_affect)
                                                          VALUES (@IdEcole, @CodSect, @DateAffect);
                                                          SELECT LAST_INSERT_ID();";

                                    affectSectId = await conn.ExecuteScalarAsync<int>(insertNoteQuery, new
                                    {
                                        IdEcole = EduKinContext.CurrentIdEcole,
                                        CodSect = _selectedSectionCode,
                                        DateAffect = DateTime.Now
                                    }, transaction);
                                }
                                
                                // Mettre à jour la variable locale pour la suite
                                _selectedSectionAffectId = affectSectId;
                            }

                            // 2. Affecter les options
                            foreach (var option in selectedOptions)
                            {
                                var insertQuery = @"INSERT INTO t_affect_options (num_affect_sect, cod_opt, date_affect)
                                                   VALUES (@NumAffectSect, @CodOpt, @DateAffect)";

                                await conn.ExecuteAsync(insertQuery, new
                                {
                                    NumAffectSect = affectSectId,
                                    CodOpt = option.CodeOption,
                                    DateAffect = DateTime.Now
                                }, transaction);
                            }

                            transaction.Commit();

                            MessageBox.Show($"{selectedOptions.Count} option(s) affectée(s) avec succès !",
                                "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Initialement je rechargerais tout, mais c'est mieux de rafraîchir proprement
                            // Recharger la liste des sections pour mettre à jour les IDs d'affectation
                            LoadSections();
                            
                            // Re-sélectionner la section courante dans la combo box
                            foreach (var item in CmbSection.Items)
                            {
                                var valueProp = item.GetType().GetProperty("Value");
                                if (valueProp?.GetValue(item)?.ToString() == _selectedSectionCode)
                                {
                                    CmbSection.SelectedItem = item;
                                    break;
                                }
                            }
                            // Pas besoin d'appeler LoadOptionsForSection ici car le changement de sélection le fera
                            // ou si on a trouvé l'item et qu'on l'a set.
                            // Si on veut forcer le refresh visuel des options:
                            // LoadOptionsForSection(_selectedSectionCode, _selectedSectionAffectId); 
                            // (mais attention _selectedSectionAffectId a potentiellement changé, d'où le re-load sections avant)
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'affectation des options : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                BtnAffecterOptions.Text = "Affecter Options";
                UpdateAffectButtonState();
            }
        }

        #endregion

        #region Événements

        private void FormAffectSection_Load(object sender, EventArgs e)
        {
            try
            {
                // Configuration du formulaire
                this.Text = "Affectation des Options aux Sections";

                // Charger les sections
                LoadSections();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du formulaire : {ex.Message}",
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
                    var affectIdProp = selectedItem.GetType().GetProperty("AffectId");

                    var codeSection = valueProp?.GetValue(selectedItem)?.ToString() ?? string.Empty;
                    var affectId = Convert.ToInt32(affectIdProp?.GetValue(selectedItem) ?? 0);

                    LoadOptionsForSection(codeSection, affectId);
                }
                else
                {
                    ClearOptionsPanel();
                    lblInfo.Text = "Sélectionnez une section pour afficher ses options.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du changement de section : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAffecterOptions_Click(object sender, EventArgs e)
        {
            AffectSelectedOptions();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadSections();
                ClearOptionsPanel();
                lblInfo.Text = "Sections actualisées. Sélectionnez une section.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'actualisation : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnFermer_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        private void lblNoOptions_Click(object sender, EventArgs e)
        {

        }
    }
}