using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EduKin.Csharp.Securites;
using Siticone.Desktop.UI.WinForms;

namespace EduKin.Layouts
{
    public partial class FormRolesPermissions : Form
    {
        private readonly GestionRolesPermissions _rolesService;

        public FormRolesPermissions()
        {
            InitializeComponent();
            _rolesService = new GestionRolesPermissions();
            ConfigureDataGridViews();
        }

        private void ConfigureDataGridViews()
        {
            // Configuration dgvRoles
            dgvRoles.AutoGenerateColumns = false;
            dgvRoles.AllowUserToAddRows = false;
            dgvRoles.AllowUserToDeleteRows = false;
            dgvRoles.ReadOnly = true;
            dgvRoles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRoles.MultiSelect = false;

            // Ajouter les colonnes manuelles avec des en-têtes propres
            if (dgvRoles.Columns.Count == 0)
            {
                dgvRoles.Columns.Add("colIdRole", "ID Rôle");
                dgvRoles.Columns.Add("colNomRole", "Nom Rôle");
                dgvRoles.Columns.Add("colDescription", "Description");
                dgvRoles.Columns.Add("colDateCreation", "Date Création");

                // Configurer les DataPropertyName
                dgvRoles.Columns["colIdRole"].DataPropertyName = "id_role";
                dgvRoles.Columns["colNomRole"].DataPropertyName = "nom_role";
                dgvRoles.Columns["colDescription"].DataPropertyName = "description";
                dgvRoles.Columns["colDateCreation"].DataPropertyName = "date_creation";

                // Ajuster la largeur des colonnes
                dgvRoles.Columns["colIdRole"].Width = 80;
                dgvRoles.Columns["colNomRole"].Width = 150;
                dgvRoles.Columns["colDescription"].Width = 200;
                dgvRoles.Columns["colDateCreation"].Width = 100;
            }

            // Configuration dgvPermissions
            dgvPermissions.AutoGenerateColumns = false;
            dgvPermissions.AllowUserToAddRows = false;
            dgvPermissions.AllowUserToDeleteRows = false;
            dgvPermissions.ReadOnly = true;
            dgvPermissions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPermissions.MultiSelect = false;

            // Ajouter les colonnes manuelles avec des en-têtes propres
            if (dgvPermissions.Columns.Count == 0)
            {
                dgvPermissions.Columns.Add("colIdPermission", "ID Permission");
                dgvPermissions.Columns.Add("colNomPermission", "Nom Permission");
                dgvPermissions.Columns.Add("colDescription", "Description");
                dgvPermissions.Columns.Add("colModule", "Module");
                dgvPermissions.Columns.Add("colDateCreation", "Date Création");

                // Configurer les DataPropertyName
                dgvPermissions.Columns["colIdPermission"].DataPropertyName = "id_permission";
                dgvPermissions.Columns["colNomPermission"].DataPropertyName = "nom_permission";
                dgvPermissions.Columns["colDescription"].DataPropertyName = "description";
                dgvPermissions.Columns["colModule"].DataPropertyName = "module";
                dgvPermissions.Columns["colDateCreation"].DataPropertyName = "date_creation";

                // Ajuster la largeur des colonnes
                dgvPermissions.Columns["colIdPermission"].Width = 80;
                dgvPermissions.Columns["colNomPermission"].Width = 150;
                dgvPermissions.Columns["colDescription"].Width = 200;
                dgvPermissions.Columns["colModule"].Width = 100;
                dgvPermissions.Columns["colDateCreation"].Width = 100;
            }

            // Configuration dgvRolePermissions
            dgvRolePermissions.AutoGenerateColumns = false;
            dgvRolePermissions.AllowUserToAddRows = false;
            dgvRolePermissions.AllowUserToDeleteRows = false;
            dgvRolePermissions.ReadOnly = true;
            dgvRolePermissions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRolePermissions.MultiSelect = false;

            // Ajouter les colonnes manuelles avec des en-têtes propres
            if (dgvRolePermissions.Columns.Count == 0)
            {
                dgvRolePermissions.Columns.Add("colIdRole", "ID Rôle");
                dgvRolePermissions.Columns.Add("colNomRole", "Nom Rôle");
                dgvRolePermissions.Columns.Add("colIdPermission", "ID Permission");
                dgvRolePermissions.Columns.Add("colNomPermission", "Nom Permission");
                dgvRolePermissions.Columns.Add("colDateAttribution", "Date Attribution");

                // Configurer les DataPropertyName
                dgvRolePermissions.Columns["colIdRole"].DataPropertyName = "id_role";
                dgvRolePermissions.Columns["colNomRole"].DataPropertyName = "nom_role";
                dgvRolePermissions.Columns["colIdPermission"].DataPropertyName = "id_permission";
                dgvRolePermissions.Columns["colNomPermission"].DataPropertyName = "nom_permission";
                dgvRolePermissions.Columns["colDateAttribution"].DataPropertyName = "date_attribution";

                // Ajuster la largeur des colonnes
                dgvRolePermissions.Columns["colIdRole"].Width = 80;
                dgvRolePermissions.Columns["colNomRole"].Width = 150;
                dgvRolePermissions.Columns["colIdPermission"].Width = 80;
                dgvRolePermissions.Columns["colNomPermission"].Width = 150;
                dgvRolePermissions.Columns["colDateAttribution"].Width = 100;
            }
        }

        private async void FormRolesPermissions_Load(object sender, EventArgs e)
        {
            try
            {
                await LoadInitialData();
                ShowPanel(panelRoles);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadInitialData()
        {
            await LoadRolesAsync();
            await LoadPermissionsAsync();
            LoadModulesComboBox();
        }

        #region Gestion des Rôles

        private async Task LoadRolesAsync()
        {
            try
            {
                var roles = await Task.Run(() => _rolesService.GetAllRoles());
                var roleList = roles.Select(r => new
                {
                    id_role = r.id_role,
                    nom_role = r.nom_role,
                    description = r.description ?? "",
                    date_creation = r.date_creation?.ToString("dd/MM/yyyy") ?? ""
                }).ToList();

                dgvRoles.DataSource = roleList;

                // Charger aussi dans le ComboBox pour les attributions
                cmbRoleAssignment.DataSource = null;
                cmbRoleAssignment.DataSource = roleList;
                cmbRoleAssignment.DisplayMember = "nom_role";
                cmbRoleAssignment.ValueMember = "id_role";
                cmbRoleAssignment.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des rôles: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnCreateRole_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNomRole.Text))
                {
                    MessageBox.Show("Le nom du rôle est requis.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNomRole.Focus();
                    return;
                }

                var success = _rolesService.CreateRole(
                    txtNomRole.Text.Trim(),
                    txtDescriptionRole.Text.Trim());

                if (success)
                {
                    MessageBox.Show("Rôle créé avec succès!", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearRoleForm();
                    await LoadRolesAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnUpdateRole_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvRoles.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner un rôle à modifier.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var idRole = Convert.ToInt32(dgvRoles.SelectedRows[0].Cells["id_role"].Value);

                var success = _rolesService.UpdateRole(
                    idRole,
                    txtNomRole.Text.Trim(),
                    txtDescriptionRole.Text.Trim());

                if (success)
                {
                    MessageBox.Show("Rôle modifié avec succès!", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearRoleForm();
                    await LoadRolesAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDeleteRole_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvRoles.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner un rôle à supprimer.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var idRole = Convert.ToInt32(dgvRoles.SelectedRows[0].Cells["id_role"].Value);
                var nomRole = dgvRoles.SelectedRows[0].Cells["nom_role"].Value?.ToString();

                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer le rôle '{nomRole}' ?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = _rolesService.DeleteRole(idRole);

                    if (success)
                    {
                        MessageBox.Show("Rôle supprimé avec succès!", "Succès",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearRoleForm();
                        await LoadRolesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvRoles_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRoles.SelectedRows.Count > 0)
            {
                var row = dgvRoles.SelectedRows[0];
                txtNomRole.Text = row.Cells["nom_role"].Value?.ToString() ?? "";
                txtDescriptionRole.Text = row.Cells["description"].Value?.ToString() ?? "";
            }
        }

        private void ClearRoleForm()
        {
            txtNomRole.Clear();
            txtDescriptionRole.Clear();
        }

        #endregion

        #region Gestion des Permissions

        private async Task LoadPermissionsAsync()
        {
            try
            {
                var permissions = await Task.Run(() => _rolesService.GetAllPermissions());
                var permissionList = permissions.Select(p => new
                {
                    id_permission = p.id_permission,
                    nom_permission = p.nom_permission,
                    description = p.description ?? "",
                    module = p.module ?? "",
                    date_creation = p.date_creation?.ToString("dd/MM/yyyy") ?? ""
                }).ToList();

                dgvPermissions.DataSource = permissionList;

                // Charger aussi dans le ComboBox pour les attributions
                cmbPermissionAssignment.DataSource = null;
                cmbPermissionAssignment.DataSource = permissionList;
                cmbPermissionAssignment.DisplayMember = "nom_permission";
                cmbPermissionAssignment.ValueMember = "id_permission";
                cmbPermissionAssignment.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des permissions: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadModulesComboBox()
        {
            var modules = new[]
            {
                "Administration",
                "Élèves",
                "Agents",
                "Finances",
                "Pédagogie",
                "Sécurité",
                "Rapports"
            };

            cmbModulePermission.Items.Clear();
            cmbModulePermission.Items.AddRange(modules);
        }

        private async void BtnCreatePermission_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNomPermission.Text))
                {
                    MessageBox.Show("Le nom de la permission est requis.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNomPermission.Focus();
                    return;
                }

                var success = _rolesService.CreatePermission(
                    txtNomPermission.Text.Trim(),
                    txtDescriptionPermission.Text.Trim(),
                    cmbModulePermission.SelectedItem?.ToString());

                if (success)
                {
                    MessageBox.Show("Permission créée avec succès!", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearPermissionForm();
                    await LoadPermissionsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnUpdatePermission_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvPermissions.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner une permission à modifier.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var idPermission = Convert.ToInt32(dgvPermissions.SelectedRows[0].Cells["id_permission"].Value);

                var success = _rolesService.UpdatePermission(
                    idPermission,
                    txtNomPermission.Text.Trim(),
                    txtDescriptionPermission.Text.Trim(),
                    cmbModulePermission.SelectedItem?.ToString());

                if (success)
                {
                    MessageBox.Show("Permission modifiée avec succès!", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearPermissionForm();
                    await LoadPermissionsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDeletePermission_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvPermissions.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner une permission à supprimer.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var idPermission = Convert.ToInt32(dgvPermissions.SelectedRows[0].Cells["id_permission"].Value);
                var nomPermission = dgvPermissions.SelectedRows[0].Cells["nom_permission"].Value?.ToString();

                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer la permission '{nomPermission}' ?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = _rolesService.DeletePermission(idPermission);

                    if (success)
                    {
                        MessageBox.Show("Permission supprimée avec succès!", "Succès",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearPermissionForm();
                        await LoadPermissionsAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvPermissions_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPermissions.SelectedRows.Count > 0)
            {
                var row = dgvPermissions.SelectedRows[0];
                txtNomPermission.Text = row.Cells["nom_permission"].Value?.ToString() ?? "";
                txtDescriptionPermission.Text = row.Cells["description"].Value?.ToString() ?? "";
                cmbModulePermission.SelectedItem = row.Cells["module"].Value?.ToString();
            }
        }

        private void ClearPermissionForm()
        {
            txtNomPermission.Clear();
            txtDescriptionPermission.Clear();
            cmbModulePermission.SelectedIndex = -1;
        }

        #endregion

        #region Attribution des Permissions aux Rôles

        private void LoadRolePermissions()
        {
            try
            {
                if (dgvRoles.SelectedRows.Count == 0) return;

                var idRole = Convert.ToInt32(dgvRoles.SelectedRows[0].Cells["id_role"].Value);
                var permissions = _rolesService.GetRolePermissions(idRole);

                var permissionList = permissions.Select(p => new
                {
                    id_permission = p.id_permission,
                    nom_permission = p.nom_permission,
                    description = p.description ?? "",
                    module = p.module ?? "",
                    date_attribution = p.date_attribution?.ToString("dd/MM/yyyy") ?? ""
                }).ToList();

                dgvRolePermissions.DataSource = permissionList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des permissions du rôle: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnAssignPermission_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbRoleAssignment.SelectedValue == null)
                {
                    MessageBox.Show("Veuillez sélectionner un rôle.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbPermissionAssignment.SelectedValue == null)
                {
                    MessageBox.Show("Veuillez sélectionner une permission.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var idRole = Convert.ToInt32(cmbRoleAssignment.SelectedValue);
                var idPermission = Convert.ToInt32(cmbPermissionAssignment.SelectedValue);

                var success = _rolesService.AssignPermissionToRole(idRole, idPermission);

                if (success)
                {
                    MessageBox.Show("Permission attribuée avec succès!", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadRolePermissions();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'attribution: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnRemovePermission_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvRolePermissions.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner une permission à retirer.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbRoleAssignment.SelectedValue == null)
                {
                    MessageBox.Show("Veuillez sélectionner un rôle.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var idRole = Convert.ToInt32(cmbRoleAssignment.SelectedValue);
                var idPermission = Convert.ToInt32(dgvRolePermissions.SelectedRows[0].Cells["id_permission"].Value);
                var nomPermission = dgvRolePermissions.SelectedRows[0].Cells["nom_permission"].Value?.ToString();

                var result = MessageBox.Show($"Êtes-vous sûr de vouloir retirer la permission '{nomPermission}' ?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = _rolesService.RemovePermissionFromRole(idRole, idPermission);

                    if (success)
                    {
                        MessageBox.Show("Permission retirée avec succès!", "Succès",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadRolePermissions();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du retrait: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Navigation

        private void BtnRoles_Click(object sender, EventArgs e)
        {
            ShowPanel(panelRoles);
        }

        private void BtnPermissions_Click(object sender, EventArgs e)
        {
            ShowPanel(panelPermissions);
        }

        private void BtnAssignments_Click(object sender, EventArgs e)
        {
            ShowPanel(panelAssignments);
        }

        private void ShowPanel(Panel panelToShow)
        {
            panelRoles.Visible = false;
            panelPermissions.Visible = false;
            panelAssignments.Visible = false;

            panelToShow.Visible = true;
            panelToShow.BringToFront();
        }

        #endregion

        #region Event Handlers for Clear Buttons

        private void BtnClearRole_Click(object sender, EventArgs e)
        {
            ClearRoleForm();
        }

        private void BtnClearPermission_Click(object sender, EventArgs e)
        {
            ClearPermissionForm();
        }

        #endregion
    }
}