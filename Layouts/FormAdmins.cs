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

namespace EduKin.Layouts
{
    public partial class FormAdmins : Form
    {
        private readonly UsersEvents _usersService;
        private readonly Administrations _adminService;
        private readonly GestionRolesPermissions _rolesService;
        private readonly Paiements _paiementsService;
        private readonly Pedagogies _pedaService;

        // Propriétés publiques pour les contrôles des utilisateurs
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
            // Tous les événements sont maintenant configurés dans le Designer
            VerifyComponentReferences();
        }

        private void VerifyComponentReferences()
        {
            // Minimal verification
            if (txtNom == null || txtPostNom == null || txtPrenom == null ||
                cmbSexe == null || cmbRole == null ||
                BtnSaveUser == null || BtnUpdateUser == null || BtnDeleteUser == null ||
                DgvUser == null)
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
                    MessageBox.Show("Veuillez sélectionner un utilisateur à modifier.", "Attention",
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
                    MessageBox.Show("Utilisateur modifié avec succès!", "Succès",
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
                    MessageBox.Show("Veuillez sélectionner un utilisateur à supprimer.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var username = DgvUser.SelectedRows[0].Cells["username"].Value?.ToString();
                if (string.IsNullOrEmpty(username)) return;

                var result = MessageBox.Show($"Êtes-vous sûr de vouloir désactiver l'utilisateur '{username}' ?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = _usersService.DeleteUser(username);

                    if (success)
                    {
                        MessageBox.Show("Utilisateur désactivé avec succès!", "Succès",
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

        private async void BtnResetPassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (DgvUser.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner un utilisateur.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var username = DgvUser.SelectedRows[0].Cells["username"].Value?.ToString();
                if (string.IsNullOrEmpty(username)) return;

                var newPassword = "123456";

                var result = MessageBox.Show($"Réinitialiser le mot de passe de '{username}' à '123456' ?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = _usersService.ResetPassword(username, newPassword);

                    if (success)
                    {
                        MessageBox.Show($"Mot de passe réinitialisé avec succès!\nNouveau mot de passe: {newPassword}",
                            "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la réinitialisation: {ex.Message}", "Erreur",
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

                // Recharger les rôles après fermeture du formulaire
                _ = LoadRolesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture du gestionnaire de rôles: {ex.Message}", "Erreur",
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
                MessageBox.Show("Le prénom est requis.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrenom.Focus();
                return false;
            }

            if (cmbSexe.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner le sexe.", "Validation",
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
                MessageBox.Show($"Erreur lors du chargement des données: {ex.Message}", "Erreur",
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
                roleList.Insert(0, new { Value = (dynamic)"", Text = (dynamic)"-- Sélectionner un rôle --" });

                cmbRole.DataSource = roleList;
                cmbRole.DisplayMember = "Text";
                cmbRole.ValueMember = "Value";
                cmbRole.SelectedIndex = 0;

                // Initialiser les autres ComboBox
                InitializeComboBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des rôles: {ex.Message}", "Erreur",
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
                    Statut = u.compte_verrouille ? "Inactif" : "Actif"

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

            // Afficher le panel demandé
            panelToShow.Visible = true;
            panelToShow.BringToFront();
        }

        private async Task LoadSectionsData()
        {
            try
            {
                var sections = await Task.Run(() => _adminService.GetAllSections());
                var sectionList = sections.Select(s => new
                {
                    Code_Section = s.cod_sect?.ToString() ?? "",
                    Description = s.description?.ToString() ?? ""
                }).ToList();

                DgvSection.DataSource = sectionList;
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
                    MessageBox.Show("Veuillez sélectionner une section à modifier.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var codSect = DgvSection.SelectedRows[0].Cells["cod_sect"].Value?.ToString();
                if (string.IsNullOrEmpty(codSect)) return;

                var success = _adminService.UpdateSection(codSect, TxtDescripSection.Text.Trim());

                if (success)
                {
                    MessageBox.Show("Section modifiée avec succès!", "Succès",
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
                    MessageBox.Show("Veuillez sélectionner une section à supprimer.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var codSect = DgvSection.SelectedRows[0].Cells["cod_sect"].Value?.ToString();
                var description = DgvSection.SelectedRows[0].Cells["description"].Value?.ToString();

                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer la section '{description}' ?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = _adminService.DeleteSection(codSect);

                    if (success)
                    {
                        MessageBox.Show("Section supprimée avec succès!", "Succès",
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
                var options = await Task.Run(() => _adminService.GetAllOptions());
                var sections = await Task.Run(() => _adminService.GetAllSections());
                var availableSourceOptions = await Task.Run(() => _adminService.GetAvailableSourceOptions());

                var sectionList = sections.Select(s => new
                {
                    cod_sect = s.cod_sect?.ToString() ?? "",
                    description = s.description?.ToString() ?? ""
                }).ToList();

                CmbSectionOption.DataSource = sectionList;
                CmbSectionOption.DisplayMember = "description";
                CmbSectionOption.ValueMember = "cod_sect";

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

                var optionList = options.Select(o => new
                {
                    Code_Option = o.cod_opt?.ToString() ?? "",
                    Description = o.description?.ToString() ?? "",
                    Code_Section = o.cod_sect?.ToString() ?? "",
                    Code_Epst = o.code_epst?.ToString() ?? ""
                }).ToList();

                DgvOption.DataSource = optionList;
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
                
                // Pour l'édition, afficher la description dans le ComboBox
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
                var promotions = await Task.Run(() => _adminService.GetAllPromotions());
                var options = await Task.Run(() => _adminService.GetAllOptions());

                // Convertir en liste typée pour éviter les problèmes avec DisplayMember
                var optionsList = options.Select(o => new
                {
                    cod_opt = (string)o.cod_opt,
                    description = (string)o.description
                }).ToList();

                CmbOptionPromotion.DataSource = optionsList;
                CmbOptionPromotion.DisplayMember = "description";
                CmbOptionPromotion.ValueMember = "cod_opt";

                var promotionList = promotions.Select(p => new
                {
                    Code_Promotion = (string)p.cod_promo,
                    Description = (string)p.description,
                    Code_Option = (string)p.cod_opt

                }).ToList();

                DgvPromotion.DataSource = promotionList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des promotions: {ex.Message}\n\nDétails: {ex.StackTrace}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Charge les niveaux de promotion en fonction de l'option sélectionnée
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
                
                // Vérifier si c'est "Education de Base"
                if (selectedOption.Contains("Education de Base") || selectedOption.Contains("Éducation de Base"))
                {
                    // Pour Education de Base: 7ème et 8ème
                    CmbDescripPromotion.Items.Add($"7ème {selectedOption}");
                    CmbDescripPromotion.Items.Add($"8ème {selectedOption}");
                }
                else
                {
                    // Pour les autres options: 1ère à 4ème
                    CmbDescripPromotion.Items.Add($"1ère {selectedOption}");
                    CmbDescripPromotion.Items.Add($"2ème {selectedOption}");
                    CmbDescripPromotion.Items.Add($"3ème {selectedOption}");
                    CmbDescripPromotion.Items.Add($"4ème {selectedOption}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des niveaux: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gère le changement de sélection de l'option pour charger les niveaux appropriés
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
                    MessageBox.Show("Veuillez sélectionner une promotion à modifier.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var codPromo = DgvPromotion.SelectedRows[0].Cells["cod_promo"].Value?.ToString();
                if (string.IsNullOrEmpty(codPromo)) return;

                var success = _adminService.UpdatePromotion(
                    codPromo,
                    CmbDescripPromotion.Text.Trim(),
                    "",
                    CmbOptionPromotion.SelectedValue?.ToString());

                if (success)
                {
                    MessageBox.Show("Promotion modifiée avec succès!", "Succès",
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
                    MessageBox.Show("Veuillez sélectionner une promotion à supprimer.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var codPromo = DgvPromotion.SelectedRows[0].Cells["cod_promo"].Value?.ToString();
                var description = DgvPromotion.SelectedRows[0].Cells["description"].Value?.ToString();

                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer la promotion '{description}' ?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = _adminService.DeletePromotion(codPromo);

                    if (success)
                    {
                        MessageBox.Show("Promotion supprimée avec succès!", "Succès",
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
                TxtCodePromotion.Text = row.Cells["cod_promo"].Value?.ToString() ?? "";
                
                // Pour l'édition, afficher la description dans le ComboBox
                var description = row.Cells["description"].Value?.ToString() ?? "";
                CmbDescripPromotion.Text = description;

                var codOpt = row.Cells["cod_opt"].Value?.ToString();
                if (!string.IsNullOrEmpty(codOpt))
                {
                    CmbOptionPromotion.SelectedItem = codOpt;
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
                    MessageBox.Show("Aucun contexte d'école sélectionné.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var success = _adminService.CreateAffectSection(currentSchoolId, codSect);
                if (success)
                {
                    MessageBox.Show("Affectation créée.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    MessageBox.Show("Affectation supprimée.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                // ✅ Utilisation du user_index de l'utilisateur connecté depuis la base de données
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
                MessageBox.Show($"Erreur lors de la génération de l'ID: {ex.Message}", "Erreur",
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
                    MessageBox.Show("Section créée avec succès!", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearSectionForm();
                    await LoadSectionsData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtCodeCours_Enter(object sender, EventArgs e)
        {
            try
            {
                // ✅ Utilisation du user_index de l'utilisateur connecté depuis la base de données
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
                MessageBox.Show($"Erreur lors de la génération de l'ID: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtCodeOption_Enter(object sender, EventArgs e)
        {
            try
            {
                // ✅ Utilisation du user_index de l'utilisateur connecté depuis la base de données
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
                MessageBox.Show($"Erreur lors de la génération de l'ID: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtCodePromotion_Enter(object sender, EventArgs e)
        {
            try
            {
                // ✅ Utilisation du user_index de l'utilisateur connecté depuis la base de données
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
                MessageBox.Show($"Erreur lors de la génération de l'ID: {ex.Message}", "Erreur",
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

                // Récupérer le Code_Epst de l'item sélectionné
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
                    MessageBox.Show("Option créée avec succès!", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearOptionForm();
                    await LoadOptionsData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnUpdateOption_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (DgvOption.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner une option à modifier.", "Attention",
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
                    MessageBox.Show("Option modifiée avec succès!", "Succès",
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
                    MessageBox.Show("Veuillez sélectionner une option à supprimer.", "Attention",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var codOpt = DgvOption.SelectedRows[0].Cells["cod_opt"].Value?.ToString();
                var description = DgvOption.SelectedRows[0].Cells["description"].Value?.ToString();

                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer l'option '{description}' ?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = _adminService.DeleteOption(codOpt);

                    if (success)
                    {
                        MessageBox.Show("Option supprimée avec succès!", "Succès",
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
                    "", // codSect not used
                    CmbOptionPromotion.SelectedValue?.ToString());

                if (success)
                {
                    MessageBox.Show("Promotion créée avec succès!", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearPromotionForm();
                    await LoadPromotionsData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création: {ex.Message}", "Erreur",
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
                    userProfilePath // Ajouter le chemin de la photo
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

        private void panelMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtUsername_Enter(object sender, EventArgs e)
        {

        }

        private void TxtIdUser_Enter(object sender, EventArgs e)
        {
            try
            {
                // ✅ Utilisation du user_index de l'utilisateur connecté depuis la base de données
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
                MessageBox.Show($"Erreur lors de la génération de l'ID: {ex.Message}", "Erreur",
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
                    MessageBox.Show("Aucun contexte d'école sélectionné.", "Erreur",
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
                // 1. Récupérer les infos de base (Ecole/User)
                // Ces appels peuvent lever une exception si non initialisés, mais c'est normal à ce stade
                string currentIdEcole = EduKinContext.CurrentIdEcole;
                string currentUsername = EduKinContext.CurrentUserName;
                
                string currentAnneeScol = null;
                
                // 2. Tenter de récupérer l'année scolaire
                try 
                {
                    currentAnneeScol = EduKinContext.CurrentCodeAnnee;
                }
                catch (InvalidOperationException)
                {
                    // Le contexte de l'année n'est pas prêt.
                    // Tenter une auto-initialisation via SchoolYearManager
                    try 
                    {
                        var sym = new SchoolYearManager();
                        // On a besoin de userId. EduKinContext.CurrentUserId devrait marcher si CurrentUserName marche.
                        string userId = EduKinContext.CurrentUserId;
                        
                        bool initSuccess = sym.InitializeContextWithActiveYear(currentIdEcole, userId, currentUsername);
                        
                        if (initSuccess)
                        {
                            // Réessayer de lire la propriété
                            currentAnneeScol = EduKinContext.CurrentCodeAnnee;
                        }
                        else
                        {
                            // Pas d'année active trouvée
                            throw new InvalidOperationException("Aucune année scolaire active trouvée pour cette école.");
                        }
                    }
                    catch (Exception initEx)
                    {
                         MessageBox.Show("Impossible d'accéder au module Finance.\n" +
                                        "Raison : " + initEx.Message + "\n\n" +
                                        "Veuillez configurer et activer une année scolaire dans le menu Administration.", 
                            "Configuration requise", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                
                // Si on arrive ici, on a tout ce qu'il faut
                
                // Récupérer les informations de l'école
                var ecole = _adminService.GetEcole(currentIdEcole);
                var ecoleName = ecole?.denomination ?? "École inconnue";
                var ecoleAddress = "Kinshasa, RDC"; // À adapter selon vos données
                
                // Créer et afficher le formulaire des frais avec le contexte
                using (var fraisForm = new FormFrais(currentIdEcole, currentAnneeScol, currentUsername, ecoleName, ecoleAddress))
                {
                    // Masquer le formulaire d'administration
                    this.Hide();
                    
                    // Afficher le formulaire des frais en mode modal
                    var result = fraisForm.ShowDialog(this);
                    
                    // Réafficher le formulaire d'administration quand FormFrais se ferme
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
    }
}