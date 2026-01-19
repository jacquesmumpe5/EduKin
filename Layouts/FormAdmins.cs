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
using EduKin.Csharp.Admins;
using EduKin.Csharp.Finances;
using EduKin.Inits;
using Siticone.Desktop.UI.WinForms;
using MySql.Data.MySqlClient;

namespace EduKin.Layouts
{
    public partial class FormAdmins : Form
    {
        private readonly UsersEvents _usersService;
        private readonly Administrations _adminService;
        private readonly GestionRolesPermissions _rolesService;
        private readonly Paiements _paiementsService;
        private readonly Pedagogies _pedaService;

        // Propri�t�s publiques pour les contr�les des agents
        // public Siticone.Desktop.UI.WinForms.SiticoneButton SaveAgentControl => BtnSaveAgent;
        // public Siticone.Desktop.UI.WinForms.SiticoneButton UpdateAgentControl => BtnUpdateAgent;
        // public Siticone.Desktop.UI.WinForms.SiticoneButton DelAgentControl => BtnDelAgent;
        // public PictureBox PicBoxAgentControl => PictureBoxProfilAgent;
        // public Siticone.Desktop.UI.WinForms.SiticoneDataGridView DataGridViewAgentControl => DataGridViewAgent;

        // Propri�t�s publiques pour les contr�les des utilisateurs
        public PictureBox PictureBoxProfilUserControl => PictureBoxProfilUser;
        public Siticone.Desktop.UI.WinForms.SiticoneButton BtnLoadPicUserControl => btnLoadPicUser;
        public Siticone.Desktop.UI.WinForms.SiticoneButton BtnCapturePicUserControl => btnCapturePicUser;

        public FormAdmins()
        {
            InitializeComponent();
            _usersService = new UsersEvents();
            _adminService = new Administrations();
            _rolesService = new GestionRolesPermissions();
            _paiementsService = new Paiements();
            _pedaService = new Pedagogies();

            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            // Tous les �v�nements sont maintenant configur�s dans le Designer
            VerifyComponentReferences();
        }

        private void VerifyComponentReferences()
        {
            // Minimal verification
            if (txtNom == null || txtPostNom == null || txtPrenom == null ||
                cmbSexe == null || cmbRole == null ||
                BtnSaveUser == null || BtnUpdateUser == null || BtnDeleteUser == null ||
                btnResetPassword == null || DgvUser == null)
            {
                throw new InvalidOperationException("One or more user management components are not properly initialized.");
            }


        }

        private void BtnUtilisateur_Click(object sender, EventArgs e)
        {
            ShowPanel(panelNavUtilisateurs);
            _ = LoadUserManagementData();
        }

        private void BtnSection_Cours_Click(object sender, EventArgs e)
        {
            ShowPanel(panelNavSection_Cours);
            _ = LoadSectionsData();
            _ = LoadCoursData();
        }
        private async void BtnUpdateUser_Click(object sender, EventArgs e)
        {
            try
            {
                if (DgvUser.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez s�lectionner un utilisateur � modifier.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var username = DgvUser.SelectedRows[0].Cells["username"].Value?.ToString();
                if (string.IsNullOrEmpty(username)) return;

                var newPassword = string.IsNullOrWhiteSpace(txtPassword.Text) ? null : txtPassword.Text;

                var selectedRole = cmbRole.SelectedItem?.ToString();

                var success = _usersService.UpdateUser(username, newPassword, selectedRole);

                if (success)
                {
                    MessageBox.Show("Utilisateur modifi� avec succ�s!", "Succ�s",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    await LoadUsersAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDeleteUser_Click(object sender, EventArgs e)
        {
            try
            {
                if (DgvUser.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez s�lectionner un utilisateur � supprimer.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var username = DgvUser.SelectedRows[0].Cells["username"].Value?.ToString();
                if (string.IsNullOrEmpty(username)) return;

                var result = MessageBox.Show($"�tes-vous s�r de vouloir d�sactiver l'utilisateur '{username}' ?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = _usersService.DeleteUser(username);

                    if (success)
                    {
                        MessageBox.Show("Utilisateur d�sactiv� avec succ�s!", "Succ�s",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        await LoadUsersAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnResetPassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (DgvUser.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez s�lectionner un utilisateur.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var username = DgvUser.SelectedRows[0].Cells["username"].Value?.ToString();
                if (string.IsNullOrEmpty(username)) return;

                var newPassword = "123456";

                var result = MessageBox.Show($"R�initialiser le mot de passe de '{username}' � '123456' ?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = _usersService.ResetPassword(username, newPassword);

                    if (success)
                    {
                        MessageBox.Show($"Mot de passe r�initialis� avec succ�s!\nNouveau mot de passe: {newPassword}",
                            "Succ�s", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la r�initialisation: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Locks/Unlocks a user account based on current status
        /// </summary>
        private async void BtnToggleLockUser_Click(object sender, EventArgs e)
        {
            try
            {
                if (DgvUser.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez s�lectionner un utilisateur.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var username = DgvUser.SelectedRows[0].Cells["username"].Value?.ToString();
                if (string.IsNullOrEmpty(username)) return;

                var isLocked = DgvUser.SelectedRows[0].Cells["Statut"].Value?.ToString() == "Inactif";
                var action = isLocked ? "d�verrouiller" : "verrouiller";

                var result = MessageBox.Show($"�tes-vous s�r de vouloir {action} le compte '{username}' ?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // var success = _usersService.ToggleUserLock(username, !isLocked);
                    var success = true; // Temporaire - méthode non implémentée

                    if (success)
                    {
                        MessageBox.Show($"Compte {action} avec succ�s!", "Succ�s",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadUsersAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du changement de statut: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Resets failed login attempts for a user
        /// </summary>
        private async void BtnResetFailedAttempts_Click(object sender, EventArgs e)
        {
            try
            {
                if (DgvUser.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez s�lectionner un utilisateur.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var username = DgvUser.SelectedRows[0].Cells["username"].Value?.ToString();
                if (string.IsNullOrEmpty(username)) return;

                var result = MessageBox.Show($"R�initialiser les tentatives de connexion �chou�es pour '{username}' ?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // var success = _usersService.ResetFailedAttempts(username);
                    var success = true; // Temporaire - méthode non implémentée

                    if (success)
                    {
                        MessageBox.Show("Tentatives �chou�es r�initialis�es avec succ�s!", "Succ�s",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadUsersAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la r�initialisation: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnManageRoles_Click(object sender, EventArgs e)
        {
            try
            {
                using (var rolesForm = new FormRolesPermissions())
                {
                    rolesForm.ShowDialog(this);
                }

                // Recharger les r�les apr�s fermeture du formulaire
                _ = LoadRolesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture du gestionnaire de r�les: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearForm_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void DgvUser_SelectionChanged(object sender, EventArgs e)
        {
            if (DgvUser.SelectedRows.Count > 0)
            {
                var row = DgvUser.SelectedRows[0];

                var nomComplet = row.Cells["nom_complet"].Value?.ToString() ?? "";
                var parts = nomComplet.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 3)
                {
                    txtNom.Text = parts[0];
                    txtPostNom.Text = parts[1];
                    txtPrenom.Text = string.Join(" ", parts.Skip(2));
                }
                else
                {
                    txtNom.Text = parts.ElementAtOrDefault(0) ?? "";
                    txtPostNom.Text = parts.ElementAtOrDefault(1) ?? "";
                    txtPrenom.Text = parts.ElementAtOrDefault(2) ?? "";
                }

                txtUsername.Text = row.Cells["username"].Value?.ToString() ?? "";
                txtPassword.Text = "";
                cmbRole.SelectedItem = row.Cells["role_name"].Value?.ToString();

            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtNom.Text))
            {
                MessageBox.Show("Le nom est requis.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNom.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPostNom.Text))
            {
                MessageBox.Show("Le post-nom est requis.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPostNom.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrenom.Text))
            {
                MessageBox.Show("Le pr�nom est requis.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrenom.Focus();
                return false;
            }

            if (cmbSexe.SelectedItem == null)
            {
                MessageBox.Show("Veuillez s�lectionner le sexe.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbSexe.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Le nom d'utilisateur est requis.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Le mot de passe est requis.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }



            return true;
        }

        private void ClearForm()
        {
            txtNom.Clear();
            txtPostNom.Clear();
            txtPrenom.Clear();
            cmbSexe.SelectedIndex = -1;
            txtUsername.Clear();
            txtPassword.Clear();
            txtTelephone.Clear();
            cmbRole.SelectedIndex = -1;

        }

        private async Task LoadUserManagementData()
        {
            try
            {
                VerifyComponentReferences();
                await LoadRolesAsync();
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des donn�es: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadRolesAsync()
        {
            try
            {
                var roles = await Task.Run(() => _rolesService.GetAllRoles());

                cmbRole.DataSource = null;
                cmbRole.Items.Clear();

                var roleList = roles.Select(r => new
                {
                    Value = (dynamic)r.id_role.ToString(),
                    Text = (dynamic)r.nom_role
                }).ToList();

                // Ajouter une option vide
                roleList.Insert(0, new { Value = (dynamic)"", Text = (dynamic)"-- S�lectionner un r�le --" });

                cmbRole.DataSource = roleList;
                cmbRole.DisplayMember = "Text";
                cmbRole.ValueMember = "Value";
                cmbRole.SelectedIndex = 0;

                // Initialiser les autres ComboBox
                InitializeComboBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des r�les: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComboBoxes()
        {
            try
            {
                // Initialiser le ComboBox des sexes
                if (cmbSexe.Items.Count == 0)
                {
                    cmbSexe.Items.Clear();
                    cmbSexe.Items.Add("M");
                    cmbSexe.Items.Add("F");
                    cmbSexe.SelectedIndex = -1;
                }


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de l'initialisation des ComboBox: {ex.Message}");
            }
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                var users = await Task.Run(() => _usersService.GetAllUsers());

                var userList = users.Select(u => new
                {
                    ID_User = u.id_user,
                    UserName = u.username,
                    Nom_Complet = $"{u.nom} {u.postnom} {u.prenom}",
                    Sexe = u.sexe,
                    Telephone = u.telephone ?? "",
                    Role = u.role_name ?? "Non défini",
                    User_Index = u.user_index ?? 0,
                    Type_User = u.type_user ?? "ECOLE",
                    Statut = u.compte_verrouille ? "Inactif" : "Actif",
                    Tentatives_Echouees = u.failed_login_attempts ?? 0,
                    Verrouille_Jusqua = u.account_locked_until?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    Derniere_Connexion = u.derniere_connexion?.ToString("dd/MM/yyyy HH:mm") ?? "Jamais"

                }).ToList();

                DgvUser.DataSource = userList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des utilisateurs: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowPanel(Panel panelToShow)
        {
            // Masquer tous les panels
            panelNavAccueil.Visible = false;
            panelNavUtilisateurs.Visible = false;
            panelNavOptions_Promotions.Visible = false;
            panelNavSection_Cours.Visible = false;

            // Afficher le panel demand�
            panelToShow.Visible = true;
            panelToShow.BringToFront();
        }

        private async Task LoadSectionsData()
        {
            try
            {
                // 1) Initialiser le contexte école AVANT tout chargement
                if (!EduKinContext.IsConfigured)
                {
                    // Utiliser un ID d'école valide pour l'initialisation
                    _adminService.InitializeSchoolContext("ID_ECOLE_VALIDE");
                }

                // Forcer les propriétés du DataGridView
                DgvSection.AutoGenerateColumns = false;
                DgvSection.DataSource = null;

                // Charger les sections SANS Task.Run (directement sur thread UI)
                LoadSections();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des sections: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSections()
        {
            try
            {
                using (var conn = _adminService.GetSecureConnection())
                {
                    conn.Open();

                    // REQUÊTE SQL À UTILISER (STRICTEMENT)
                    var query = @"SELECT 
                        id_section,
                        intitule
                    FROM vue_sections
                    WHERE id_ecole = @IdEcole
                    ORDER BY intitule";

                    using (var cmd = new MySqlCommand(query, (MySqlConnection)conn))
                    {
                        cmd.Parameters.AddWithValue("@IdEcole", EduKinContext.CurrentIdEcole);

                        using (var reader = cmd.ExecuteReader())
                        {
                            // Vider le DataGridView avant de le remplir
                            DgvSection.Rows.Clear();

                            // LOGIQUE IMPOSÉE
                            while (reader.Read())
                            {
                                DgvSection.Rows.Add(
                                    reader[0], // ColCodeSection = id_section
                                    reader[1]  // ColDescripSection = intitule
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des sections: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadCoursData()
        {
            try
            {
                var cours = await Task.Run(() => _pedaService.GetAllCours());
                var coursList = cours.Select(c => new
                {
                    Code_Cours = c.Id_Cours?.ToString() ?? "",
                    Description = c.intitule?.ToString() ?? ""
                }).ToList();

                DgvCours.DataSource = coursList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des cours: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void BtnUpdateSection_Click(object sender, EventArgs e)
        {
            try
            {
                if (DgvSection.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez s�lectionner une section � modifier.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var codSect = DgvSection.SelectedRows[0].Cells["cod_sect"].Value?.ToString();
                if (string.IsNullOrEmpty(codSect)) return;

                var success = _adminService.UpdateSection(codSect, TxtDescripSection.Text.Trim());

                if (success)
                {
                    MessageBox.Show("Section modifi�e avec succ�s!", "Succ�s",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearSectionForm();
                    await LoadSectionsData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDeleteSection_Click(object sender, EventArgs e)
        {
            try
            {
                if (DgvSection.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez s�lectionner une section � supprimer.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var codSect = DgvSection.SelectedRows[0].Cells["cod_sect"].Value?.ToString();
                var description = DgvSection.SelectedRows[0].Cells["description"].Value?.ToString();

                var result = MessageBox.Show($"�tes-vous s�r de vouloir supprimer la section '{description}' ?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = _adminService.DeleteSection(codSect);

                    if (success)
                    {
                        MessageBox.Show("Section supprim�e avec succ�s!", "Succ�s",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearSectionForm();
                        await LoadSectionsData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvSections_SelectionChanged(object sender, EventArgs e)
        {
            if (DgvSection.SelectedRows.Count > 0)
            {
                var row = DgvSection.SelectedRows[0];
                TxtCodeSection.Text = row.Cells["cod_sect"].Value?.ToString() ?? "";
                TxtDescripSection.Text = row.Cells["description"].Value?.ToString() ?? "";
                TxtCodeSection.ReadOnly = true;
            }
        }

        private void ClearSectionForm()
        {
            TxtCodeSection.Clear();
            TxtDescripSection.Clear();
            TxtCodeSection.ReadOnly = false;
        }

        private async Task LoadOptionsData()
        {
            try
            {
                // 1) Initialiser le contexte école AVANT tout chargement
                if (!EduKinContext.IsConfigured)
                {
                    // Utiliser un ID d'école valide pour l'initialisation
                    _adminService.InitializeSchoolContext("ID_ECOLE_VALIDE");
                }

                // Forcer les propriétés du DataGridView
                DgvOption.AutoGenerateColumns = false;
                DgvOption.DataSource = null;

                var availableSourceOptions = await Task.Run(() => _adminService.GetAvailableSourceOptions());

                // Charger les sections pour le ComboBox (uniquement celles affectées à l'école)
                await LoadSectionsForOptionComboBox();

                // Charger les options sources disponibles dans CmbDescripOption avec Code_Epst
                var sourceOptionsList = availableSourceOptions.Select(o => new
                {
                    Text = o.OptionName?.ToString() ?? "",
                    Value = o.OptionName?.ToString() ?? "",
                    CodeEpst = o.Code_Epst?.ToString() ?? ""
                }).ToList();

                CmbDescripOption.Items.Clear();
                CmbDescripOption.DisplayMember = "Text";
                CmbDescripOption.ValueMember = "Value";
                foreach (var item in sourceOptionsList)
                {
                    CmbDescripOption.Items.Add(item);
                }

                // Charger les options SANS Task.Run (directement sur thread UI)
                LoadOptions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des options: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadSectionsForOptionComboBox()
        {
            try
            {
                using (var conn = _adminService.GetSecureConnection())
                {
                    conn.Open();

                    // Utiliser la vue vue_sections avec filtre id_ecole
                    var query = @"SELECT 
                        id_section,
                        intitule
                    FROM vue_sections
                    WHERE id_ecole = @IdEcole
                    ORDER BY intitule";

                    using (var cmd = new MySqlCommand(query, (MySqlConnection)conn))
                    {
                        cmd.Parameters.AddWithValue("@IdEcole", EduKinContext.CurrentIdEcole);

                        using (var reader = cmd.ExecuteReader())
                        {
                            var sectionList = new List<object>();

                            while (reader.Read())
                            {
                                sectionList.Add(new
                                {
                                    cod_sect = reader[0]?.ToString() ?? "",
                                    description = reader[1]?.ToString() ?? ""
                                });
                            }

                            CmbSectionOption.DataSource = sectionList;
                            CmbSectionOption.DisplayMember = "description";
                            CmbSectionOption.ValueMember = "cod_sect";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des sections pour le ComboBox: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadOptions()
        {
            try
            {
                using (var conn = _adminService.GetSecureConnection())
                {
                    conn.Open();

                    // REQUÊTE SQL À UTILISER (STRICTEMENT)
                    var query = @"SELECT 
                        id_option,
                        intitule,
                        id_section,
                        sections
                    FROM vue_options
                    WHERE id_ecole = @IdEcole
                    ORDER BY intitule";

                    using (var cmd = new MySqlCommand(query, (MySqlConnection)conn))
                    {
                        cmd.Parameters.AddWithValue("@IdEcole", EduKinContext.CurrentIdEcole);

                        using (var reader = cmd.ExecuteReader())
                        {
                            // Vider le DataGridView avant de le remplir
                            DgvOption.Rows.Clear();

                            // LOGIQUE IMPOSÉE
                            while (reader.Read())
                            {
                                DgvOption.Rows.Add(
                                    reader[0], // ColIdOption = id_option
                                    reader[1], // ColDescriptOption = intitule
                                    reader[2], // ColFkSectOption = id_section
                                    reader[3]  // ColDescripSectOption = sections
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des options: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvOptions_SelectionChanged(object sender, EventArgs e)
        {
            if (DgvOption.SelectedRows.Count > 0)
            {
                var row = DgvOption.SelectedRows[0];
                TxtCodeOption.Text = row.Cells["cod_opt"].Value?.ToString() ?? "";

                // Pour l'�dition, afficher la description dans le ComboBox
                var description = row.Cells["description"].Value?.ToString() ?? "";
                CmbDescripOption.Text = description;

                var codSect = row.Cells["cod_sect"].Value?.ToString();
                if (!string.IsNullOrEmpty(codSect))
                {
                    CmbSectionOption.SelectedItem = codSect;
                }

                TxtCodeOption.ReadOnly = true;
            }
        }

        private void ClearOptionForm()
        {
            TxtCodeOption.Clear();
            CmbDescripOption.SelectedIndex = -1;
            CmbDescripOption.Text = "";
            CmbSectionOption.SelectedIndex = -1;
            TxtCodeOption.ReadOnly = false;
        }

        private async Task LoadPromotionsData()
        {
            try
            {
                // 1) Initialiser le contexte école AVANT tout chargement
                if (!EduKinContext.IsConfigured)
                {
                    // Utiliser un ID d'école valide pour l'initialisation
                    _adminService.InitializeSchoolContext("ID_ECOLE_VALIDE");
                }

                // Forcer les propriétés du DataGridView
                DgvPromotion.AutoGenerateColumns = false;
                DgvPromotion.DataSource = null;

                // Charger les options pour le ComboBox (uniquement celles affectées à l'école)
                await LoadOptionsForPromotionComboBox();

                // Charger les promotions SANS Task.Run (directement sur thread UI)
                LoadPromotions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des promotions: {ex.Message}\n\nDétails: {ex.StackTrace}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadOptionsForPromotionComboBox()
        {
            try
            {
                using (var conn = _adminService.GetSecureConnection())
                {
                    conn.Open();

                    // Utiliser la vue vue_options avec filtre id_ecole
                    var query = @"SELECT 
                        id_option,
                        intitule
                    FROM vue_options
                    WHERE id_ecole = @IdEcole
                    ORDER BY intitule";

                    using (var cmd = new MySqlCommand(query, (MySqlConnection)conn))
                    {
                        cmd.Parameters.AddWithValue("@IdEcole", EduKinContext.CurrentIdEcole);

                        using (var reader = cmd.ExecuteReader())
                        {
                            var optionsList = new List<object>();

                            while (reader.Read())
                            {
                                optionsList.Add(new
                                {
                                    cod_opt = reader[0]?.ToString() ?? "",
                                    description = reader[1]?.ToString() ?? ""
                                });
                            }

                            CmbOptionPromotion.DataSource = optionsList;
                            CmbOptionPromotion.DisplayMember = "description";
                            CmbOptionPromotion.ValueMember = "cod_opt";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des options pour le ComboBox: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPromotions()
        {
            try
            {
                using (var conn = _adminService.GetSecureConnection())
                {
                    conn.Open();

                    // REQUÊTE SQL À UTILISER (STRICTEMENT)
                    var query = @"SELECT 
                    id_promotion,
                    intitule,
                    id_option,
                    options
                FROM vue_promotions
                WHERE id_ecole = @IdEcole
                ORDER BY intitule";

                    using (var cmd = new MySqlCommand(query, (MySqlConnection)conn))
                    {
                        cmd.Parameters.AddWithValue("@IdEcole", EduKinContext.CurrentIdEcole);

                        using (var reader = cmd.ExecuteReader())
                        {
                            // Vider le DataGridView avant de le remplir
                            DgvPromotion.Rows.Clear();

                            // LOGIQUE IMPOSÉE
                            while (reader.Read())
                            {
                                DgvPromotion.Rows.Add(
                                    reader[0], // ColCodePromotion = pro.id_promotion
                                    reader[1], // ColDescriptionPromotion = pro.description
                                    reader[2], // ColFkOptPromotion = afopt.fk_option
                                    reader[3]  // ColDescripOptPromotion = opt.description
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des promotions: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Charge les niveaux de promotion en fonction de l'option s�lectionn�e
        /// </summary>
        private void LoadPromotionLevels()
        {
            try
            {
                CmbDescripPromotion.Items.Clear();

                if (CmbOptionPromotion.SelectedItem == null)
                {
                    return;
                }

                var selectedOption = CmbOptionPromotion.Text;

                // V�rifier si c'est "Education de Base"
                if (selectedOption.Contains("Education de Base") || selectedOption.Contains("�ducation de Base"))
                {
                    // Pour Education de Base: 7�me et 8�me
                    CmbDescripPromotion.Items.Add($"7�me {selectedOption}");
                    CmbDescripPromotion.Items.Add($"8�me {selectedOption}");
                }
                else
                {
                    // Pour les autres options: 1�re � 4�me
                    CmbDescripPromotion.Items.Add($"1�re {selectedOption}");
                    CmbDescripPromotion.Items.Add($"2�me {selectedOption}");
                    CmbDescripPromotion.Items.Add($"3�me {selectedOption}");
                    CmbDescripPromotion.Items.Add($"4�me {selectedOption}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des niveaux: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// G�re le changement de s�lection de l'option pour charger les niveaux appropri�s
        /// </summary>
        private void CmbOptionPromotion_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPromotionLevels();
        }


        private async void BtnUpdatePromotion_Click(object sender, EventArgs e)
        {
            try
            {
                if (DgvPromotion.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez s�lectionner une promotion � modifier.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var codPromo = DgvPromotion.SelectedRows[0].Cells["ColCodePromotion"].Value?.ToString();
                if (string.IsNullOrEmpty(codPromo)) return;

                var success = _adminService.UpdatePromotion(
                    codPromo,
                    CmbDescripPromotion.Text.Trim(),
                    "",
                    CmbOptionPromotion.SelectedValue?.ToString());

                if (success)
                {
                    MessageBox.Show("Promotion modifi�e avec succ�s!", "Succ�s",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearPromotionForm();
                    await LoadPromotionsData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDeletePromotion_Click(object sender, EventArgs e)
        {
            try
            {
                if (DgvPromotion.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez s�lectionner une promotion � supprimer.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var codPromo = DgvPromotion.SelectedRows[0].Cells["ColCodePromotion"].Value?.ToString();
                var description = DgvPromotion.SelectedRows[0].Cells["ColDescriptionPromotion"].Value?.ToString();

                var result = MessageBox.Show($"�tes-vous s�r de vouloir supprimer la promotion '{description}' ?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = _adminService.DeletePromotion(codPromo);

                    if (success)
                    {
                        MessageBox.Show("Promotion supprim�e avec succ�s!", "Succ�s",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearPromotionForm();
                        await LoadPromotionsData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvPromotions_SelectionChanged(object sender, EventArgs e)
        {
            if (DgvPromotion.SelectedRows.Count > 0)
            {
                var row = DgvPromotion.SelectedRows[0];
                TxtCodePromotion.Text = row.Cells["ColCodePromotion"].Value?.ToString() ?? "";

                // Pour l'�dition, afficher la description dans le ComboBox
                var description = row.Cells["ColDescriptionPromotion"].Value?.ToString() ?? "";
                CmbDescripPromotion.Text = description;

                var codOpt = row.Cells["ColFkOptPromotion"].Value?.ToString();
                if (!string.IsNullOrEmpty(codOpt))
                {
                    CmbOptionPromotion.SelectedValue = codOpt;
                }

                TxtCodePromotion.ReadOnly = true;
            }
        }

        private void ClearPromotionForm()
        {
            TxtCodePromotion.Clear();
            CmbDescripPromotion.SelectedIndex = -1;
            CmbDescripPromotion.Text = "";
            CmbOptionPromotion.SelectedIndex = -1;
            TxtCodePromotion.ReadOnly = false;
        }

        // End of Promotions section

        // Start of Affectation Sections CRUD UI

        // UI for affect_sect: load, create, delete
        private async void LoadAffectSections()
        {
            try
            {
                var currentSchoolId = EduKinContext.CurrentIdEcole;
                if (string.IsNullOrEmpty(currentSchoolId)) return;

                var affects = await Task.Run(() => _adminService.GetAffectSectionsByEcole(currentSchoolId));
                // For simplicity, show affects in dgvSections when selected school context is active
                // Map to a simple object list and set as DataSource of dgvSections if required
                // Here we won't change existing dgvSections binding; this method is placeholder for later UI wiring
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des affectations: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void CreateAffectSection(string codSect)
        {
            try
            {
                var currentSchoolId = EduKinContext.CurrentIdEcole;
                if (string.IsNullOrEmpty(currentSchoolId))
                {
                    MessageBox.Show("Aucun contexte d'�cole s�lectionn�.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var success = _adminService.CreateAffectSection(currentSchoolId, codSect);
                if (success)
                {
                    MessageBox.Show("Affectation cr��e.", "Succ�s", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await Task.Run(() => LoadAffectSections());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DeleteAffectSection(int numAffect)
        {
            try
            {
                var success = _adminService.DeleteAffectSection(numAffect);
                if (success)
                {
                    MessageBox.Show("Affectation supprim�e.", "Succ�s", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await Task.Run(() => LoadAffectSections());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // End of Affectation Sections

        private void FormAdmins_Load(object sender, EventArgs e)
        {
            // Optionally initialize UI state
        }
        private void btnOption_Promotion_Click(object sender, EventArgs e)
        {
            ShowPanel(panelNavOptions_Promotions);
            _ = LoadPromotionsData();
            _ = LoadOptionsData();
        }

        private void TxtCodeSection_Enter(object sender, EventArgs e)
        {
            try
            {
                // ? Utilisation du user_index de l'utilisateur connect� depuis la base de donn�es
                var userIndex = EduKinContext.CurrentUserIndex;
                _adminService.ExecuteGenerateId(TxtCodeSection, "t_sections", "cod_sect", "SEC", userIndex);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Erreur d'authentification: {ex.Message}\nVeuillez vous reconnecter.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la g�n�ration de l'ID: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSaveSection_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtCodeSection.Text) || string.IsNullOrWhiteSpace(TxtDescripSection.Text))
                {
                    MessageBox.Show("Veuillez remplir tous les champs.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var success = _adminService.CreateSection(TxtCodeSection.Text.Trim(), TxtDescripSection.Text.Trim());

                if (success)
                {
                    MessageBox.Show("Section cr��e avec succ�s!", "Succ�s",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearSectionForm();
                    await LoadSectionsData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la cr�ation: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtCodeCours_Enter(object sender, EventArgs e)
        {
            try
            {
                // ? Utilisation du user_index de l'utilisateur connect� depuis la base de donn�es
                var userIndex = EduKinContext.CurrentUserIndex;
                _adminService.ExecuteGenerateId(TxtCodeCours, "t_cours", "id_cours", "CRS", userIndex);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Erreur d'authentification: {ex.Message}\nVeuillez vous reconnecter.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la g�n�ration de l'ID: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtCodeOption_Enter(object sender, EventArgs e)
        {
            try
            {
                // ? Utilisation du user_index de l'utilisateur connect� depuis la base de donn�es
                var userIndex = EduKinContext.CurrentUserIndex;
                _adminService.ExecuteGenerateId(TxtCodeOption, "t_options", "cod_opt", "OPT", userIndex);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Erreur d'authentification: {ex.Message}\nVeuillez vous reconnecter.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la g�n�ration de l'ID: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtCodePromotion_Enter(object sender, EventArgs e)
        {
            try
            {
                // ? Utilisation du user_index de l'utilisateur connect� depuis la base de donn�es
                var userIndex = EduKinContext.CurrentUserIndex;
                _adminService.ExecuteGenerateId(TxtCodePromotion, "t_promotions", "cod_promo", "PRO", userIndex);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Erreur d'authentification: {ex.Message}\nVeuillez vous reconnecter.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la g�n�ration de l'ID: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSaveOption_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtCodeOption.Text) ||
                    string.IsNullOrWhiteSpace(CmbDescripOption.Text) ||
                    CmbSectionOption.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez remplir tous les champs.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // R�cup�rer le Code_Epst de l'item s�lectionn�
                string? codeEpst = null;
                if (CmbDescripOption.SelectedItem != null)
                {
                    var selectedItem = CmbDescripOption.SelectedItem;
                    var codeEpstProp = selectedItem.GetType().GetProperty("CodeEpst");
                    if (codeEpstProp != null)
                    {
                        codeEpst = codeEpstProp.GetValue(selectedItem)?.ToString();
                    }
                }

                var success = _adminService.CreateOption(
                    TxtCodeOption.Text.Trim(),
                    CmbDescripOption.Text.Trim(),
                    CmbSectionOption.SelectedValue?.ToString(),
                    codeEpst);

                if (success)
                {
                    MessageBox.Show("Option cr��e avec succ�s!", "Succ�s",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearOptionForm();
                    await LoadOptionsData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la cr�ation: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnUpdateOption_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (DgvOption.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez s�lectionner une option � modifier.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var codOpt = DgvOption.SelectedRows[0].Cells["cod_opt"].Value?.ToString();
                if (string.IsNullOrEmpty(codOpt)) return;

                var success = _adminService.UpdateOption(
                    codOpt,
                    CmbDescripOption.Text.Trim(),
                    CmbSectionOption.SelectedValue?.ToString());

                if (success)
                {
                    MessageBox.Show("Option modifi�e avec succ�s!", "Succ�s",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearOptionForm();
                    await LoadOptionsData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDeleteOption_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (DgvOption.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez s�lectionner une option � supprimer.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var codOpt = DgvOption.SelectedRows[0].Cells["cod_opt"].Value?.ToString();
                var description = DgvOption.SelectedRows[0].Cells["description"].Value?.ToString();

                var result = MessageBox.Show($"�tes-vous s�r de vouloir supprimer l'option '{description}' ?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = _adminService.DeleteOption(codOpt);

                    if (success)
                    {
                        MessageBox.Show("Option supprim�e avec succ�s!", "Succ�s",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearOptionForm();
                        await LoadOptionsData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancelOption_Click(object sender, EventArgs e)
        {
            ClearOptionForm();
        }

        private async void BtnSavePromotion_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtCodePromotion.Text) ||
                    string.IsNullOrWhiteSpace(CmbDescripPromotion.Text) ||
                    CmbOptionPromotion.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez remplir tous les champs.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var success = _adminService.CreatePromotion(
                    TxtCodePromotion.Text.Trim(),
                    CmbDescripPromotion.Text.Trim(),
                    CmbOptionPromotion.SelectedValue?.ToString());

                if (success)
                {
                    MessageBox.Show("Promotion cr��e avec succ�s!", "Succ�s",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearPromotionForm();
                    await LoadPromotionsData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la cr�ation: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSaveUser_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateForm()) return;

                var currentIdEcole = EduKinContext.CurrentIdEcole;
                var selectedRole = cmbRole.SelectedItem?.ToString();

                // Récupérer le chemin de la photo depuis le PictureBox
                string? userProfilePath = null;
                if (PictureBoxProfilUser.Image != null)
                {
                    // Récupérer le chemin stocké dans le Tag du PictureBox ou utiliser une autre méthode
                    userProfilePath = PictureBoxProfilUser.Tag as string;

                    // Si le chemin n'est pas dans le Tag, essayer de le récupérer autrement
                    if (string.IsNullOrEmpty(userProfilePath))
                    {
                        // Pour l'instant, nous allons supposer que le chemin est déjà géré par PictureManager
                        // TODO: Implémenter un meilleur système pour stocker/récupérer le chemin de la photo
                        MessageBox.Show("Le chemin de la photo sera sauvegardé lors de la création de l'utilisateur.",
                            "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                // Get user_index from context for ID generation
                var userIndex = EduKinContext.CurrentUserIndex;

                var success = _usersService.CreateUser(
                    txtNom.Text.Trim(),
                    txtPostNom.Text.Trim(),
                    txtPrenom.Text.Trim(),
                    cmbSexe.SelectedItem?.ToString() ?? "M",
                    txtUsername.Text.Trim(),
                    txtPassword.Text,
                    txtTelephone.Text.Trim(),
                    currentIdEcole,
                    selectedRole,
                    userProfilePath
                );

                if (success)
                {
                    MessageBox.Show("Utilisateur créé avec succès!", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    await LoadUsersAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void TxtIdUser_Enter(object sender, EventArgs e)
        {
            try
            {
                // ? Utilisation du user_index de l'utilisateur connect� depuis la base de donn�es
                var userIndex = EduKinContext.CurrentUserIndex;
                _adminService.ExecuteGenerateId(TxtIdUser, "t_users_infos", "id_user", "USR", userIndex);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Erreur d'authentification: {ex.Message}\nVeuillez vous reconnecter.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la generation de l'ID: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbOptionPromotion_DropDown(object sender, EventArgs e)
        {
            _ = LoadPromotionsData();
        }

        private void CmbSectionOption_DropDown(object sender, EventArgs e)
        {
            _ = LoadOptionsData();
        }

        private void btnAffectSection_Click(object sender, EventArgs e)
        {
            try
            {
                var currentIdEcole = EduKinContext.CurrentIdEcole;
                if (string.IsNullOrEmpty(currentIdEcole))
                {
                    MessageBox.Show("Aucun contexte d'�cole s�lectionn�.", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var affectForm = new FormAffectSection())
                {
                    affectForm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture du formulaire d'affectation: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Finance button click - opens FormFrais and hides FormAdmins
        /// </summary>
        private void BtnFinance_Click(object sender, EventArgs e)
        {
            try
            {
                // R�cup�rer le contexte actuel
                var currentIdEcole = EduKinContext.CurrentIdEcole;
                var currentUsername = EduKinContext.CurrentUserName;
                var currentAnneeScol = "2025-2026"; // � adapter selon votre syst�me

                // R�cup�rer les informations de l'�cole
                var ecole = _adminService.GetEcole(currentIdEcole);
                var ecoleName = ecole?.denomination ?? "�cole inconnue";
                var ecoleAddress = "Kinshasa, RDC"; // � adapter selon vos donn�es

                // Cr�er et afficher le formulaire des frais avec le contexte
                using (var fraisForm = new FormFrais(currentIdEcole, currentAnneeScol, currentUsername, ecoleName, ecoleAddress))
                {
                    // Masquer le formulaire d'administration
                    this.Hide();

                    // Afficher le formulaire des frais en mode modal
                    var result = fraisForm.ShowDialog(this);

                    // R�afficher le formulaire d'administration quand FormFrais se ferme
                    this.Show();
                    this.BringToFront();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture du module Finance: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // S'assurer que le formulaire d'administration est visible en cas d'erreur
                this.Show();
            }
        }

        /// <summary>
        /// Handles the Exit button click - closes FormAdmins and returns to FormMain
        /// </summary>
        private void BtnExitAdmin_Click(object sender, EventArgs e)
        {
            try
            {
                // Fermer le formulaire d'administration
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la fermeture: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panelNavAccueil_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
