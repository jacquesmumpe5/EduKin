using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dapper;
using EduKin.DataSets;
using EduKin.Inits;
using EduKin.Csharp.Admins;

namespace EduKin.Layouts
{
    /// <summary>
    /// Formulaire d'affectation des agents (services, grades, rôles, fonction, salaires)
    /// </summary>
    public partial class FormAffectAgent : Form
    {
        private readonly Administrations _adminService;
        private readonly Agents _agentsService;
        private readonly string _matriculeAgent;

        // Properties to store selected values
        public string? SelectedService { get; private set; }
        public string? SelectedGrade { get; private set; }
        public string? SelectedRole { get; private set; }
        public string? SelectedFonction { get; private set; }
        public decimal SelectedSalBase { get; private set; }
        public decimal SelectedIpr { get; private set; }
        public decimal SelectedSalNet { get; private set; }

        public FormAffectAgent(
            string matriculeAgent,
            string? currentService = null,
            string? currentGrade = null,
            string? currentRole = null,
            string? currentFonction = null,
            decimal currentSalBase = 0,
            decimal currentIpr = 0,
            decimal currentSalNet = 0)
        {
            InitializeComponent();
            _matriculeAgent = matriculeAgent;
            _adminService = new Administrations();
            _agentsService = new Agents();

            // Store current values
            SelectedService = currentService;
            SelectedGrade = currentGrade;
            SelectedRole = currentRole;
            SelectedFonction = currentFonction;
            SelectedSalBase = currentSalBase;
            SelectedIpr = currentIpr;
            SelectedSalNet = currentSalNet;

            InitializeForm();
            LoadCurrentAffectations();
        }

        /// <summary>
        /// Initialise les paramètres du formulaire
        /// </summary>
        private void InitializeForm()
        {
            this.Text = $"Affectation de l'agent - {_matriculeAgent}";
            lblTitle.Text = $"Affectation de l'agent\n{_matriculeAgent}";

            // Load combo boxes
            LoadServices();
            LoadGrades();
            LoadRoles();

            // Wire up event handlers
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        /// <summary>
        /// Charge les services disponibles
        /// </summary>
        private void LoadServices()
        {
            try
            {
                var services = _adminService.GetAllServices();
                CmbServiceAgent.DataSource = services?.ToList();
                CmbServiceAgent.DisplayMember = "description";
                CmbServiceAgent.ValueMember = "id_service";
                CmbServiceAgent.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des services: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Charge les grades disponibles
        /// </summary>
        private void LoadGrades()
        {
            try
            {
                var grades = _adminService.GetAllGrades();
                CmbGradeAgent.DataSource = grades?.ToList();
                CmbGradeAgent.DisplayMember = "libelle_grade";
                CmbGradeAgent.ValueMember = "id_grade";
                CmbGradeAgent.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des grades: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Charge les rôles disponibles
        /// </summary>
        private void LoadRoles()
        {
            try
            {
                var roles = _adminService.GetAllRoles();
                CmbRoleAgent.DataSource = roles?.ToList();
                CmbRoleAgent.DisplayMember = "nom_role";
                CmbRoleAgent.ValueMember = "id_role";
                CmbRoleAgent.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des rôles: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Charge les affectations actuelles de l'agent
        /// </summary>
        private void LoadCurrentAffectations()
        {
            try
            {
                // Charger les affectations actuelles depuis les tables d'affectation
                if (!string.IsNullOrEmpty(SelectedService))
                {
                    CmbServiceAgent.SelectedValue = SelectedService;
                }

                if (!string.IsNullOrEmpty(SelectedGrade))
                {
                    CmbGradeAgent.SelectedValue = SelectedGrade;
                }

                if (!string.IsNullOrEmpty(SelectedRole))
                {
                    CmbRoleAgent.SelectedValue = SelectedRole;
                }

                // Note: Fonction et salaires seront gérés via des contrôles à ajouter au Designer
                // Pour l'instant, ils sont stockés dans les propriétés et retournés au formulaire parent
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des affectations actuelles: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gère le clic sur le bouton Enregistrer
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate at least one affectation is selected
                if (CmbServiceAgent.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner au moins un service.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Store selected values
                SelectedService = CmbServiceAgent.SelectedValue?.ToString();
                SelectedGrade = CmbGradeAgent.SelectedIndex >= 0 ? CmbGradeAgent.SelectedValue?.ToString() : null;
                SelectedRole = CmbRoleAgent.SelectedIndex >= 0 ? CmbRoleAgent.SelectedValue?.ToString() : null;

                // Persister les affectations dans la base de données
                using (var conn = Connexion.Instance.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Désactiver les affectations précédentes (marquer comme inactives)
                            var sqlDesactiver = @"UPDATE t_service_agent SET actif = 0 WHERE fk_agent = @Matricule;
                                                  UPDATE t_grade_agent SET actif = 0 WHERE fk_agent = @Matricule;
                                                  UPDATE t_roles_agents SET actif = 0 WHERE fk_agent = @Matricule;";
                            conn.Execute(sqlDesactiver, new { Matricule = _matriculeAgent }, transaction);

                            // 2. Insérer l'affectation de service
                            if (!string.IsNullOrEmpty(SelectedService))
                            {
                                var sqlService = @"INSERT INTO t_service_agent 
                                    (fk_service, fk_agent, date_affect) 
                                    VALUES (@Service, @Matricule, @DateAffect)";
                                conn.Execute(sqlService, new
                                {
                                    Service = SelectedService,
                                    Matricule = _matriculeAgent,
                                    DateAffect = DateTime.Now
                                }, transaction);
                            }

                            // 3. Insérer l'affectation de grade
                            if (!string.IsNullOrEmpty(SelectedGrade))
                            {
                                var sqlGrade = @"INSERT INTO t_grade_agent 
                                    (fk_grade, fk_agent, date_affect) 
                                    VALUES (@Grade, @Matricule, @DateAffect)";
                                conn.Execute(sqlGrade, new
                                {
                                    Grade = SelectedGrade,
                                    Matricule = _matriculeAgent,
                                    DateAffect = DateTime.Now
                                }, transaction);
                            }

                            // 4. Insérer l'affectation de rôle
                            if (!string.IsNullOrEmpty(SelectedRole))
                            {
                                var sqlRole = @"INSERT INTO t_roles_agents 
                                    (fk_role, fk_agent, date_affect) 
                                    VALUES (@Role, @Matricule, @DateAffect)";
                                conn.Execute(sqlRole, new
                                {
                                    Role = SelectedRole,
                                    Matricule = _matriculeAgent,
                                    DateAffect = DateTime.Now
                                }, transaction);
                            }

                            transaction.Commit();
                            MessageBox.Show("Affectations enregistrées avec succès!",
                                "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"Erreur lors de l'enregistrement des affectations: {ex.Message}", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gère le clic sur le bouton Annuler
        /// </summary>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void FormAffectAgent_Load(object sender, EventArgs e)
        {

        }
    }
}
