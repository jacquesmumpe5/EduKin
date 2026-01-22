using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EduKin.DataSets;

namespace EduKin.Layouts
{
    /// <summary>
    /// Formulaire de recherche d'adresse réutilisable pour élèves et agents
    /// Utilise la même logique que FormConfig
    /// </summary>
    public partial class FormAddressSearch : Form
    {
        private readonly Connexion _connexion;
        
        public FormAddressSearch()
        {
            InitializeComponent();
            _connexion = Connexion.Instance;
        }

        #region Propriétés Publiques

        /// <summary>
        /// Avenue sélectionnée
        /// </summary>
        public string SelectedAvenue { get; private set; } = string.Empty;

        /// <summary>
        /// Quartier sélectionné
        /// </summary>
        public string SelectedQuartier { get; private set; } = string.Empty;

        /// <summary>
        /// Commune sélectionnée
        /// </summary>
        public string SelectedCommune { get; private set; } = string.Empty;

        /// <summary>
        /// Ville sélectionnée
        /// </summary>
        public string SelectedVille { get; private set; } = string.Empty;

        /// <summary>
        /// Province sélectionnée
        /// </summary>
        public string SelectedProvince { get; private set; } = string.Empty;

        /// <summary>
        /// Adresse complète formatée
        /// </summary>
        public string FullAddress => GetFullAddress();

        #endregion

        #region Méthodes Publiques

        /// <summary>
        /// Définit l'adresse initiale dans le formulaire
        /// </summary>
        public void SetInitialAddress(string avenue, string quartier, string commune, string ville, string province = "Kinshasa")
        {
            txtAvenue.Text = avenue ?? string.Empty;
            txtQuartier.Text = quartier ?? string.Empty;
            txtCommune.Text = commune ?? string.Empty;
            txtVille.Text = ville ?? string.Empty;
            txtProvince.Text = province ?? string.Empty;
        }

        /// <summary>
        /// Retourne l'adresse complète formatée
        /// </summary>
        public string GetFullAddress()
        {
            var parts = new[] { SelectedAvenue, SelectedQuartier, SelectedCommune, SelectedVille, SelectedProvince }
                .Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(", ", parts);
        }

        #endregion

        #region Recherche d'Avenue (Logic from FormConfig)

        /// <summary>
        /// Recherche les avenues correspondant au texte saisi
        /// Réutilise exactement la logique de FormConfig.SearchAvenues
        /// </summary>
        private async Task SearchAvenues(string searchText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    lstAvenues.Items.Clear();
                    return;
                }

                // Minimum 2 caractères pour déclencher la recherche
                if (searchText.Length < 2)
                {
                    return;
                }

                // Forcer une vérification de connexion avant de charger (comme dans FormConfig)
                _connexion.ForceCheckConnection();

                // Même requête exacte que dans FormConfig
                var query = @"
                    SELECT DISTINCT 
                        id_avenue,
                        Avenue, 
                        Quartier, 
                        Commune, 
                        Ville, 
                        Province
                    FROM vue_avenue_hierarchie 
                    WHERE Avenue LIKE @searchText
                    ORDER BY Avenue, Quartier, Commune";
                
                var dataTable = await Task.Run(() => 
                    ExecuteQueryWithParameter(query, "@searchText", $"%{searchText}%"));
                
                lstAvenues.Items.Clear();
                
                foreach (DataRow row in dataTable.Rows)
                {
                    // Format: Avenue → Quartier → Commune → Ville → Province
                    var hierarchyText = $"{row["Avenue"]} → {row["Quartier"]} → {row["Commune"]} → {row["Ville"]} → {row["Province"]}";
                    var item = new ListViewItem(hierarchyText);
                    item.Tag = new AvenueInfo
                    {
                        IdAvenue = row["id_avenue"].ToString() ?? string.Empty,
                        Avenue = row["Avenue"].ToString() ?? string.Empty,
                        Quartier = row["Quartier"].ToString() ?? string.Empty,
                        Commune = row["Commune"].ToString() ?? string.Empty,
                        Ville = row["Ville"].ToString() ?? string.Empty,
                        Province = row["Province"].ToString() ?? string.Empty
                    };
                    lstAvenues.Items.Add(item);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la recherche d'avenues : {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Exécute une requête SQL avec un paramètre (copié exactement de FormConfig)
        /// </summary>
        private DataTable ExecuteQueryWithParameter(string query, string paramName, string paramValue)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = query;
                        
                        var parameter = cmd.CreateParameter();
                        parameter.ParameterName = paramName;
                        parameter.Value = (object?)paramValue ?? DBNull.Value;
                        cmd.Parameters.Add(parameter);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Erreur de base de données : {ex.Message}\n\nRequête : {query}\nParamètre {paramName} : {paramValue}";
                throw new Exception(errorMessage, ex);
            }
            return dt;
        }

        #endregion

        #region Gestionnaires d'Événements

        /// <summary>
        /// Gère le changement de texte dans le TextBox de recherche d'avenue
        /// </summary>
        private async void txtAvenue_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // Debounce de 300ms comme dans FormConfig
                await Task.Delay(300);
                
                // Vérifier que le texte n'a pas changé pendant le délai
                if (sender is Siticone.Desktop.UI.WinForms.SiticoneTextBox stb && txtAvenue.Text == stb.Text)
                {
                    await SearchAvenues(txtAvenue.Text);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans txtAvenue_TextChanged: {ex.Message}");
            }
        }

        /// <summary>
        /// Gère la sélection d'une avenue dans la ListView
        /// </summary>
        private void lstAvenues_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstAvenues.SelectedItems.Count > 0)
            {
                var selectedItem = lstAvenues.SelectedItems[0];
                if (selectedItem.Tag is AvenueInfo avenueInfo)
                {
                    // Remplir automatiquement les champs
                    txtQuartier.Text = avenueInfo.Quartier;
                    txtCommune.Text = avenueInfo.Commune;
                    txtVille.Text = avenueInfo.Ville;
                    txtProvince.Text = avenueInfo.Province;
                    
                    // Mettre à jour les propriétés
                    SelectedAvenue = avenueInfo.Avenue;
                    SelectedQuartier = avenueInfo.Quartier;
                    SelectedCommune = avenueInfo.Commune;
                    SelectedVille = avenueInfo.Ville;
                    SelectedProvince = avenueInfo.Province;
                    
                    btnOK.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Valide et ferme le formulaire
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            // Permettre la saisie manuelle si aucune avenue n'est sélectionnée
            if (string.IsNullOrWhiteSpace(SelectedAvenue))
            {
                SelectedAvenue = txtAvenue.Text.Trim();
                SelectedQuartier = txtQuartier.Text.Trim();
                SelectedCommune = txtCommune.Text.Trim();
                SelectedVille = txtVille.Text.Trim();
                SelectedProvince = txtProvince.Text.Trim();
            }

            if (string.IsNullOrWhiteSpace(SelectedAvenue))
            {
                MessageBox.Show("Veuillez saisir au moins une avenue.", "Validation", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Annule et ferme le formulaire
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Ajoute une nouvelle adresse
        /// </summary>
        private async void btnAjouterAdresse_Click(object sender, EventArgs e)
        {
            try
            {
                
                var formNewAddress = new FormNewAddress();
                
                // Pré-remplir l'avenue si elle a été saisie
                formNewAddress.SetInitialAvenue(txtAvenue.Text.Trim());
                
                if (formNewAddress.ShowDialog() == DialogResult.OK)
                {
                    // L'avenue a été ajoutée avec succès
                    // Rafraîchir la recherche de manière asynchrone
                    await SearchAvenues(txtAvenue.Text);
                    
                    // Sélectionner automatiquement la nouvelle avenue si elle correspond
                    SelectNewlyAddedAvenue(formNewAddress.Avenue, formNewAddress.Quartier, formNewAddress.Commune);
                    
                    MessageBox.Show($"L'avenue '{formNewAddress.Avenue}' a été ajoutée avec succès !", 
                        "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout de l'adresse : {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sélectionne automatiquement la nouvelle avenue ajoutée
        /// </summary>
        private void SelectNewlyAddedAvenue(string avenue, string quartier, string commune)
        {
            try
            {
                if (string.IsNullOrEmpty(avenue) || lstAvenues.Items.Count == 0)
                    return;

                for (int i = 0; i < lstAvenues.Items.Count; i++)
                {
                    var item = lstAvenues.Items[i];
                    if (item.Tag is AvenueInfo info &&
                        !string.IsNullOrEmpty(info.Avenue) &&
                        !string.IsNullOrEmpty(info.Quartier) &&
                        !string.IsNullOrEmpty(info.Commune) &&
                        info.Avenue.Equals(avenue, StringComparison.OrdinalIgnoreCase) &&
                        info.Quartier.Equals(quartier, StringComparison.OrdinalIgnoreCase) &&
                        info.Commune.Equals(commune, StringComparison.OrdinalIgnoreCase))
                    {
                        // Désélectionner tous les éléments d'abord
                        lstAvenues.SelectedItems.Clear();
                        
                        // Sélectionner le nouvel élément
                        item.Selected = true;
                        lstAvenues.EnsureVisible(i);
                        
                        // Mettre à jour les champs avec gestion des nulls
                        txtQuartier.Text = info.Quartier ?? string.Empty;
                        txtCommune.Text = info.Commune ?? string.Empty;
                        txtVille.Text = info.Ville ?? string.Empty;
                        txtProvince.Text = info.Province ?? string.Empty;
                        
                        // Mettre à jour les propriétés avec gestion des nulls
                        SelectedAvenue = info.Avenue ?? string.Empty;
                        SelectedQuartier = info.Quartier ?? string.Empty;
                        SelectedCommune = info.Commune ?? string.Empty;
                        SelectedVille = info.Ville ?? string.Empty;
                        SelectedProvince = info.Province ?? string.Empty;
                        
                        btnOK.Enabled = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la sélection de la nouvelle avenue: {ex.Message}");
            }
        }

        #endregion
    }


}