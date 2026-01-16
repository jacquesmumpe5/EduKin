using System;
using System.Data;
using System.Windows.Forms;
using EduKin.DataSets;

namespace EduKin.Layouts
{
    public partial class FormNewAddress : Form
    {
        private readonly Connexion _connexion;

        private const string TYPE_PROVINCE = "TEA00000000022019";
        private const string TYPE_VILLE = "TEA00000000032019";
        private const string TYPE_COMMUNE = "TEA00000000072019";
        private const string TYPE_QUARTIER = "TEA00000000092019";
        private const string TYPE_AVENUE = "TEA00000000132019";

        public FormNewAddress()
        {
            InitializeComponent();
            _connexion = Connexion.Instance;
            LoadProvinces();
        }

        #region Proprietes Publiques

        public string Avenue { get; private set; } = string.Empty;
        public string Quartier { get; private set; } = string.Empty;
        public string Commune { get; private set; } = string.Empty;
        public string Ville { get; private set; } = string.Empty;
        public string Province { get; private set; } = string.Empty;
        public string IdAvenue { get; private set; } = string.Empty;
        public string IdQuartier { get; private set; } = string.Empty;

        #endregion

        #region Methodes Publiques

        public void SetInitialAvenue(string avenue)
        {
            txtAvenue.Text = avenue ?? string.Empty;
        }

        #endregion

        #region Chargement des ComboBox

        private void LoadProvinces()
        {
            try
            {
                cmbProvince.Items.Clear();
                var query = @"SELECT IdEntite, IntituleEntite 
                              FROM t_entite_administrative 
                              WHERE Fk_TypeEntite = @TypeEntite AND Etat = 1
                              ORDER BY IntituleEntite";

                var dt = ExecuteQueryWithParameter(query, "@TypeEntite", TYPE_PROVINCE);

                foreach (DataRow row in dt.Rows)
                {
                    cmbProvince.Items.Add(new ComboItem(
                        row["IdEntite"].ToString() ?? "",
                        row["IntituleEntite"].ToString() ?? ""
                    ));
                }

                if (cmbProvince.Items.Count > 0)
                    cmbProvince.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des provinces : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadVilles(string idProvince)
        {
            try
            {
                cmbVille.Items.Clear();
                cmbCommune.Items.Clear();
                cmbQuartier.Items.Clear();

                var query = @"SELECT IdEntite, IntituleEntite 
                              FROM t_entite_administrative 
                              WHERE Fk_EntiteMere = @IdParent AND Fk_TypeEntite = @TypeEntite AND Etat = 1
                              ORDER BY IntituleEntite";

                var dt = ExecuteQueryWithTwoParameters(query, "@IdParent", idProvince, "@TypeEntite", TYPE_VILLE);

                foreach (DataRow row in dt.Rows)
                {
                    cmbVille.Items.Add(new ComboItem(
                        row["IdEntite"].ToString() ?? "",
                        row["IntituleEntite"].ToString() ?? ""
                    ));
                }

                if (cmbVille.Items.Count > 0)
                    cmbVille.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des villes : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCommunes(string idVille)
        {
            try
            {
                cmbCommune.Items.Clear();
                cmbQuartier.Items.Clear();

                var query = @"SELECT IdEntite, IntituleEntite 
                              FROM t_entite_administrative 
                              WHERE Fk_EntiteMere = @IdParent AND Fk_TypeEntite = @TypeEntite AND Etat = 1
                              ORDER BY IntituleEntite";

                var dt = ExecuteQueryWithTwoParameters(query, "@IdParent", idVille, "@TypeEntite", TYPE_COMMUNE);

                foreach (DataRow row in dt.Rows)
                {
                    cmbCommune.Items.Add(new ComboItem(
                        row["IdEntite"].ToString() ?? "",
                        row["IntituleEntite"].ToString() ?? ""
                    ));
                }

                if (cmbCommune.Items.Count > 0)
                    cmbCommune.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des communes : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadQuartiers(string idCommune)
        {
            try
            {
                cmbQuartier.Items.Clear();

                var query = @"SELECT IdEntite, IntituleEntite 
                              FROM t_entite_administrative 
                              WHERE Fk_EntiteMere = @IdParent AND Fk_TypeEntite = @TypeEntite AND Etat = 1
                              ORDER BY IntituleEntite";

                var dt = ExecuteQueryWithTwoParameters(query, "@IdParent", idCommune, "@TypeEntite", TYPE_QUARTIER);

                foreach (DataRow row in dt.Rows)
                {
                    cmbQuartier.Items.Add(new ComboItem(
                        row["IdEntite"].ToString() ?? "",
                        row["IntituleEntite"].ToString() ?? ""
                    ));
                }

                if (cmbQuartier.Items.Count > 0)
                    cmbQuartier.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des quartiers : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Methodes d'execution SQL

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
                throw new Exception($"Erreur de base de donnees : {ex.Message}", ex);
            }
            return dt;
        }

        private DataTable ExecuteQueryWithTwoParameters(string query, string param1Name, string param1Value, string param2Name, string param2Value)
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

                        var p1 = cmd.CreateParameter();
                        p1.ParameterName = param1Name;
                        p1.Value = (object?)param1Value ?? DBNull.Value;
                        cmd.Parameters.Add(p1);

                        var p2 = cmd.CreateParameter();
                        p2.ParameterName = param2Name;
                        p2.Value = (object?)param2Value ?? DBNull.Value;
                        cmd.Parameters.Add(p2);

                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur de base de donnees : {ex.Message}", ex);
            }
            return dt;
        }

        private string GenerateAvenueId()
        {
            try
            {
                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();
                    if (_connexion.IsOnline)
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT sp_generate_id('AV')";
                            var result = cmd.ExecuteScalar();
                            return result?.ToString() ?? $"AV{DateTime.Now:yyyyMMddHHmmss}";
                        }
                    }
                    else
                    {
                        return $"AV{DateTime.Now:yyyyMMddHHmmss}";
                    }
                }
            }
            catch
            {
                return $"AV{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        #endregion

        #region Validation et Sauvegarde

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtAvenue.Text))
            {
                MessageBox.Show("Veuillez saisir le nom de l'avenue.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAvenue.Focus();
                return false;
            }

            if (cmbProvince.SelectedItem == null)
            {
                MessageBox.Show("Veuillez selectionner une province.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbVille.SelectedItem == null)
            {
                MessageBox.Show("Veuillez selectionner une ville.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbCommune.SelectedItem == null)
            {
                MessageBox.Show("Veuillez selectionner une commune.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbQuartier.SelectedItem == null)
            {
                MessageBox.Show("Veuillez selectionner un quartier.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private bool SaveAddress()
        {
            try
            {
                var selectedQuartier = (ComboItem)cmbQuartier.SelectedItem;
                var avenueNom = txtAvenue.Text.Trim();
                var idQuartier = selectedQuartier.Id;

                using (var conn = _connexion.GetConnection())
                {
                    conn.Open();

                    var checkQuery = @"SELECT COUNT(*) FROM t_entite_administrative 
                                       WHERE IntituleEntite = @Intitule 
                                       AND Fk_EntiteMere = @IdQuartier 
                                       AND Fk_TypeEntite = @TypeEntite";

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = checkQuery;

                        var p1 = cmd.CreateParameter();
                        p1.ParameterName = "@Intitule";
                        p1.Value = avenueNom;
                        cmd.Parameters.Add(p1);

                        var p2 = cmd.CreateParameter();
                        p2.ParameterName = "@IdQuartier";
                        p2.Value = idQuartier;
                        cmd.Parameters.Add(p2);

                        var p3 = cmd.CreateParameter();
                        p3.ParameterName = "@TypeEntite";
                        p3.Value = TYPE_AVENUE;
                        cmd.Parameters.Add(p3);

                        var count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show("Cette avenue existe deja dans ce quartier.",
                                "Doublon", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }

                    var newId = GenerateAvenueId();
                    var insertQuery = @"INSERT INTO t_entite_administrative 
                                        (IdEntite, IntituleEntite, Fk_EntiteMere, Fk_TypeEntite, Etat) 
                                        VALUES (@IdEntite, @Intitule, @IdQuartier, @TypeEntite, 1)";

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = insertQuery;

                        var p1 = cmd.CreateParameter();
                        p1.ParameterName = "@IdEntite";
                        p1.Value = newId;
                        cmd.Parameters.Add(p1);

                        var p2 = cmd.CreateParameter();
                        p2.ParameterName = "@Intitule";
                        p2.Value = avenueNom;
                        cmd.Parameters.Add(p2);

                        var p3 = cmd.CreateParameter();
                        p3.ParameterName = "@IdQuartier";
                        p3.Value = idQuartier;
                        cmd.Parameters.Add(p3);

                        var p4 = cmd.CreateParameter();
                        p4.ParameterName = "@TypeEntite";
                        p4.Value = TYPE_AVENUE;
                        cmd.Parameters.Add(p4);

                        var rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            IdAvenue = newId;
                            IdQuartier = idQuartier;
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion

        #region Gestionnaires d'Evenements

        private void cmbProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProvince.SelectedItem is ComboItem selected)
            {
                LoadVilles(selected.Id);
            }
        }

        private void cmbVille_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbVille.SelectedItem is ComboItem selected)
            {
                LoadCommunes(selected.Id);
            }
        }

        private void cmbCommune_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCommune.SelectedItem is ComboItem selected)
            {
                LoadQuartiers(selected.Id);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            btnSave.Enabled = false;

            try
            {
                var success = SaveAddress();
                if (success)
                {
                    Avenue = txtAvenue.Text.Trim();
                    Quartier = ((ComboItem)cmbQuartier.SelectedItem).Text;
                    Commune = ((ComboItem)cmbCommune.SelectedItem).Text;
                    Ville = ((ComboItem)cmbVille.SelectedItem).Text;
                    Province = ((ComboItem)cmbProvince.SelectedItem).Text;

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            finally
            {
                btnSave.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion
    }

    public class ComboItem
    {
        public string Id { get; }
        public string Text { get; }

        public ComboItem(string id, string text)
        {
            Id = id;
            Text = text;
        }

        public override string ToString() => Text;
    }
}
