using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EduKin.Csharp.Finances;
using EduKin.Csharp.Admins;
using EduKin.Inits;
using EduKinContext = EduKin.Inits.EduKinContext; // Résoudre le conflit de noms

namespace EduKin.Layouts
{
    public partial class FormFrais : Form
    {
        private readonly Paiements _paiements;
        private readonly Administrations _admin;
        private string _currentIdEcole = string.Empty;
        private string _currentAnneeScol = string.Empty;
        private string _currentUsername = string.Empty;
        private bool _isEditingFrais = false;
        private bool _isEditingTypeFrais = false;

        // Constructeur par défaut (pour le designer)
        public FormFrais()
        {
            InitializeComponent();
            _paiements = new Paiements();
            _admin = new Administrations();
            InitializeForm();
        }

        // Constructeur avec contexte (utilisé par FormAdmins)
        public FormFrais(string idEcole, string anneeScol, string username, string ecoleName, string ecoleAddress) : this()
        {
            _currentIdEcole = idEcole;
            _currentAnneeScol = anneeScol;
            _currentUsername = username;

            // Ne pas appeler InitializeForm() ici car il est déjà appelé dans le constructeur par défaut
            // Mais mettre à jour les labels avec les vraies données
            UpdateContextLabels(username, ecoleName, ecoleAddress);
        }

        private void UpdateContextLabels(string username, string ecoleName, string ecoleAddress)
        {
            // Mise à jour des labels de bannière avec les vraies données
            lblUsername.Text = $"Utilisateur: {username}";
            lblEcole.Text = $"École: {ecoleName}";
            lblAdresseEcole.Text = $"Adresse: {ecoleAddress}";
        }

        private void InitializeForm()
        {
            // Vérifier si le contexte est défini
            if (string.IsNullOrEmpty(_currentIdEcole))
            {
                // Essayer de récupérer le contexte depuis le nouveau système d'isolation
                try
                {
                    if (EduKinContext.IsConfigured)
                    {
                        _currentIdEcole = EduKinContext.CurrentIdEcole;
                        if (EduKinContext.IsAuthenticated)
                        {
                            _currentUsername = EduKinContext.CurrentUserName;
                        }

                        if (EduKinContext.HasActiveYear)
                        {
                            _currentAnneeScol = EduKinContext.CurrentCodeAnnee;
                        }
                        else if (EduKinContext.IsAuthenticated)
                        {
                            // Tenter de charger l'année active si elle n'a pas encore été initialisée
                            if (EduKinContext.InitializeWithActiveYear())
                            {
                                _currentAnneeScol = EduKinContext.CurrentCodeAnnee;
                            }
                        }
                    }
                    else
                    {
                        // Si aucun contexte n'est pas configuré, ne pas initialiser les données
                        MessageBox.Show("Contexte de l'école non disponible. Veuillez vous reconnecter.", "Erreur",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch
                {
                    // Si aucun contexte n'est disponible, ne pas initialiser les données
                    MessageBox.Show("Contexte de l'école non disponible. Veuillez vous reconnecter.", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Vérifier qu'une année scolaire est configurée - PAS D'ANNÉE CODÉE EN DUR
            if (string.IsNullOrEmpty(_currentAnneeScol))
            {
                MessageBox.Show("Aucune année scolaire n'est configurée. Veuillez d'abord configurer une année scolaire.",
                    "Configuration requise", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Vérifier si l'année est clôturée (lecture seule)
            bool isReadOnlyMode = false;
            try
            {
                if (EduKinContext.IsConfigured && EduKinContext.EstCloturee)
                {
                    isReadOnlyMode = true;
                    MessageBox.Show($"L'année scolaire {EduKinContext.CurrentCodeAnnee} est clôturée.\nLe formulaire sera ouvert en mode lecture seule.",
                        "Année clôturée", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                // Ignorer les erreurs de contexte pour la compatibilité
            }

            // Configuration des contrôles
            ConfigureControls(isReadOnlyMode);

            // Chargement des données seulement si le contexte est valide
            if (!string.IsNullOrEmpty(_currentIdEcole))
            {
                LoadTypeFrais();
                LoadFrais();
                LoadComboBoxes();
            }

            // Configuration des événements
            SetupEventHandlers();

            // État initial des boutons
            SetButtonStates();
        }


        private void SetupEventHandlers()
        {
            // Événements des boutons Frais
            BtnSaveFrais.Click += BtnSaveFrais_Click;
            BtnUpdateFrais.Click += BtnUpdateFrais_Click;
            BtnDeleteFrais.Click += BtnDeleteFrais_Click;
            BtnCancelFrais.Click += BtnCancelFrais_Click;

            // Événements des boutons Type Frais
            BtnSaveTypeFrais.Click += BtnSaveTypeFrais_Click;
            BtnUpdateTypeFrais.Click += BtnUpdateTypeFrais_Click;
            BtnDeleteTypeFrais.Click += BtnDeleteTypeFrais_Click;
            BtnCancelTypeFrais.Click += BtnCancelTypeFrais_Click;

            // Événements des DataGridViews
            DgvFrais.SelectionChanged += DgvFrais_SelectionChanged;
            DgvTypeFrais.SelectionChanged += DgvTypeFrais_SelectionChanged;

            // Événements de filtrage
            TxtFiltreFrais.TextChanged += TxtFiltreFrais_TextChanged;
            TxtFiltreTypeFrais.TextChanged += TxtFiltreTypeFrais_TextChanged;

            // Événements de génération d'ID
            TxtCodeFrais.Enter += TxtCodeFrais_Enter;
            TxtCodeTypeFrais.Enter += TxtCodeTypeFrais_Enter;

            // Empêcher la modification de l'ID lors du focus si on est en mode création
            // La méthode TxtCodeFrais_Enter va générer l'ID et le mettre en lecture seule

            // Événements du menu
            menuItemDeconnexion.Click += MenuItemDeconnexion_Click;
            menuItemQuitter.Click += MenuItemQuitter_Click;
        }

        private void TxtCodeFrais_Enter(object sender, EventArgs e)
        {
            if (!_isEditingFrais && string.IsNullOrEmpty(TxtCodeFrais.Text))
            {
                try
                {
                    // Récupérer dynamiquement l'index utilisateur depuis la base de données
                    var userIndex = _admin.GetUserIndex(EduKinContext.CurrentUserId);

                    // Générer un code Frais: FRS + IndexUser + ...
                    _admin.ExecuteGenerateId(TxtCodeFrais, "t_frais", "cod_frais", "FRS", userIndex);
                }
                catch (Exception ex)
                {
                    // Ignorer silencieusement ou logger si nécessaire, pour ne pas bloquer l'UI
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void TxtCodeTypeFrais_Enter(object sender, EventArgs e)
        {
            if (!_isEditingTypeFrais && string.IsNullOrEmpty(TxtCodeTypeFrais.Text))
            {
                try 
                {
                    // Récupérer dynamiquement l'index utilisateur depuis la base de données
                    var currentUserId = EduKinContext.CurrentUserId;
                    var userIndex = _admin.GetUserIndex(currentUserId);
                    
                    // DEBUG: Afficher l'index récupéré pour comprendre pourquoi 001 est utilisé
                    // A RETIRER UNE FOIS LE PROBLEME RESOLU
                    // MessageBox.Show($"Debug: UserID='{currentUserId}', Index trouvé='{userIndex}'", "Debug Index");

                    // Générer un code TypeFrais: TYF + IndexUser + ...
                    _admin.ExecuteGenerateId(TxtCodeTypeFrais, "t_type_frais", "cod_type_frais", "TYF", userIndex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur génération ID TypeFrais: {ex.Message}");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void ConfigureControls(bool isReadOnlyMode = false)
        {
            // Configuration des DataGridViews
            ConfigureDataGridViews();

            // Par défaut, activer la saisie pour permettre la création (Sauf si l'année est clôturée)
            // On désactive l'édition Type Frais par défaut pour se concentrer sur Frais, ou on active les deux.
            // Activons les deux pour permettre la création.
            SetFieldsReadOnly(isReadOnlyMode, isReadOnlyMode);

            // Si l'année est clôturée, désactiver tous les boutons de modification
            if (isReadOnlyMode)
            {
                BtnSaveFrais.Enabled = false;
                BtnUpdateFrais.Enabled = false;
                BtnDeleteFrais.Enabled = false;
                BtnSaveTypeFrais.Enabled = false;
                BtnUpdateTypeFrais.Enabled = false;
                BtnDeleteTypeFrais.Enabled = false;

                // Afficher un message dans le titre
                this.Text += " - MODE LECTURE SEULE (Année clôturée)";
            }
        }

        private void ConfigureDataGridViews()
        {
            // Configuration DgvFrais
            DgvFrais.AutoGenerateColumns = false;
            DgvFrais.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DgvFrais.MultiSelect = false;

            // Configuration DgvTypeFrais
            DgvTypeFrais.AutoGenerateColumns = false;
            DgvTypeFrais.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DgvTypeFrais.MultiSelect = false;
        }

        private void LoadComboBoxes()
        {
            try
            {
                // Charger les types de frais
                var typeFrais = _paiements.GetAllType_Frais();
                var typeFraisList = typeFrais.Select(tf => new
                {
                    cod_type_frais = tf.cod_type_frais?.ToString() ?? "",
                    description = tf.description?.ToString() ?? ""
                }).ToList();

                CmbTypeFrais.DataSource = typeFraisList;
                CmbTypeFrais.DisplayMember = "description";
                CmbTypeFrais.ValueMember = "cod_type_frais";

                // Réinitialiser la sélection
                CmbTypeFrais.SelectedIndex = -1;

                // Modalités de paiement
                CmbModalitePaiementFrais.Items.Clear();
                CmbModalitePaiementFrais.Items.AddRange(new[] { "Mensuel", "Trimestriel", "Semestriel", "Annuel", "Ponctuel" });

                // Périodes
                CmbPeriodeFrais.Items.Clear();
                CmbPeriodeFrais.Items.AddRange(new[] { "T1", "T2", "T3", "Annuel", "Mensuel" });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des listes: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTypeFrais()
        {
            try
            {
                var typeFrais = _paiements.GetAllType_Frais();

                DgvTypeFrais.DataSource = typeFrais.ToList();

                if (DgvTypeFrais.Columns.Count == 0)
                {
                    DgvTypeFrais.Columns.Add("cod_type_frais", "Code");
                    DgvTypeFrais.Columns.Add("description", "Description");
                }

                DgvTypeFrais.Columns["cod_type_frais"].DataPropertyName = "cod_type_frais";
                DgvTypeFrais.Columns["description"].DataPropertyName = "description";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des types de frais: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadFrais()
        {
            try
            {
                var frais = _paiements.GetFraisByEcole(_currentIdEcole, _currentAnneeScol);

                DgvFrais.DataSource = frais.ToList();

                if (DgvFrais.Columns.Count == 0)
                {
                    DgvFrais.Columns.Add("cod_frais", "Code");
                    DgvFrais.Columns.Add("description", "Description");
                    DgvFrais.Columns.Add("montant", "Montant");
                    DgvFrais.Columns.Add("Type_Frais_desc", "Type Frais");
                    DgvFrais.Columns.Add("modalite", "Modalité");
                    DgvFrais.Columns.Add("periode", "Période");
                }

                DgvFrais.Columns["cod_frais"].DataPropertyName = "cod_frais";
                DgvFrais.Columns["description"].DataPropertyName = "description";
                DgvFrais.Columns["montant"].DataPropertyName = "montant";
                DgvFrais.Columns["Type_Frais_desc"].DataPropertyName = "Type_Frais_desc";
                DgvFrais.Columns["modalite"].DataPropertyName = "modalite";
                DgvFrais.Columns["periode"].DataPropertyName = "periode";

                // Format de la colonne montant
                DgvFrais.Columns["montant"].DefaultCellStyle.Format = "N2";
                DgvFrais.Columns["montant"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des frais: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Gestion des Frais

        private void BtnSaveFrais_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateFraisInput()) return;

                var codFrais = TxtCodeFrais.Text.Trim();
                var description = TxtDescriptFrais.Text.Trim();
                var montant = decimal.Parse(TxtMontantFrais.Text);
                var codTypeFrais = CmbTypeFrais.SelectedValue?.ToString();
                var modalite = CmbModalitePaiementFrais.Text;
                var periode = CmbPeriodeFrais.Text;

                var success = _paiements.CreateFrais(codFrais, description, montant, codTypeFrais,
                    modalite, _currentIdEcole, periode, _currentAnneeScol);

                if (success)
                {
                    MessageBox.Show("Frais créé avec succès!", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset vers le mode Création
                    BtnCancelFrais_Click(sender, e);
                    LoadFrais();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdateFrais_Click(object sender, EventArgs e)
        {
            if (!_isEditingFrais)
            {
                // Activer le mode édition (déverrouiller description, montant, etc, mais PAS le code)
                _isEditingFrais = true;

                // On déverrouille les champs sauf le code
                TxtDescriptFrais.ReadOnly = false;
                TxtMontantFrais.ReadOnly = false;
                CmbTypeFrais.Enabled = true;
                CmbModalitePaiementFrais.Enabled = true;
                CmbPeriodeFrais.Enabled = true;

                SetButtonStates();
            }
            else
            {
                // Sauvegarder les modifications
                try
                {
                    if (!ValidateFraisInput()) return;

                    var codFrais = TxtCodeFrais.Text.Trim();
                    var description = TxtDescriptFrais.Text.Trim();
                    var montant = decimal.Parse(TxtMontantFrais.Text);
                    var modalite = CmbModalitePaiementFrais.Text;
                    var periode = CmbPeriodeFrais.Text;

                    // Note: UpdateFrais dans Paiements ne prend pas tous les champs (ex: typeFrais)
                    // Il faudrait vérifier si on peut mettre à jour le type_frais
                    var success = _paiements.UpdateFrais(codFrais, description, montant, modalite, periode);

                    if (success)
                    {
                        MessageBox.Show("Frais modifié avec succès!", "Succès",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Retour au mode Lecture de la ligne sélectionnée
                        _isEditingFrais = false;

                        // Verrouiller tout
                        SetFieldsReadOnly(true, TxtCodeTypeFrais.ReadOnly);

                        LoadFrais();
                        SetButtonStates();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la modification: {ex.Message}", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDeleteFrais_Click(object sender, EventArgs e)
        {
            if (DgvFrais.SelectedRows.Count == 0) return;

            var result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer ce frais?", "Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var codFrais = DgvFrais.SelectedRows[0].Cells["cod_frais"].Value?.ToString();

                    var success = _paiements.DeleteFrais(codFrais);

                    if (success)
                    {
                        MessageBox.Show("Frais supprimé avec succès!", "Succès",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Reset vers mode Création
                        BtnCancelFrais_Click(sender, e);
                        LoadFrais();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression: {ex.Message}", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnCancelFrais_Click(object sender, EventArgs e)
        {
            _isEditingFrais = false;
            ClearFraisFields();

            // Mode CRÉATION : Déverrouiller les champs (Code sera généré ou saisi)
            TxtCodeFrais.ReadOnly = false;
            TxtDescriptFrais.ReadOnly = false;
            TxtMontantFrais.ReadOnly = false;
            CmbTypeFrais.Enabled = true;
            CmbModalitePaiementFrais.Enabled = true;
            CmbPeriodeFrais.Enabled = true;

            DgvFrais.ClearSelection();

            SetButtonStates();
        }

        private void DgvFrais_SelectionChanged(object sender, EventArgs e)
        {
            if (DgvFrais.SelectedRows.Count > 0)
            {
                var row = DgvFrais.SelectedRows[0];

                TxtCodeFrais.Text = row.Cells["cod_frais"].Value?.ToString() ?? "";
                TxtDescriptFrais.Text = row.Cells["description"].Value?.ToString() ?? "";
                TxtMontantFrais.Text = row.Cells["montant"].Value?.ToString() ?? "";
                CmbModalitePaiementFrais.Text = row.Cells["modalite"].Value?.ToString() ?? "";
                CmbPeriodeFrais.Text = row.Cells["periode"].Value?.ToString() ?? "";

                // Sélectionner le type de frais correspondant
                var typeFraisDesc = row.Cells["Type_Frais_desc"].Value?.ToString();
                if (!string.IsNullOrEmpty(typeFraisDesc))
                {
                    for (int i = 0; i < CmbTypeFrais.Items.Count; i++)
                    {
                        var item = (dynamic)CmbTypeFrais.Items[i];
                        if (item.description == typeFraisDesc)
                        {
                            CmbTypeFrais.SelectedIndex = i;
                            break;
                        }
                    }
                }

                // Mode AFFICHAGE : Verrouiller les champs
                _isEditingFrais = false;
                TxtCodeFrais.ReadOnly = true;
                TxtDescriptFrais.ReadOnly = true;
                TxtMontantFrais.ReadOnly = true;
                CmbTypeFrais.Enabled = false;
                CmbModalitePaiementFrais.Enabled = false;
                CmbPeriodeFrais.Enabled = false;

                SetButtonStates();
            }
        }

        #endregion

        #region Gestion des Types de Frais

        private void BtnSaveTypeFrais_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateTypeFraisInput()) return;

                var codTypeFrais = TxtCodeTypeFrais.Text.Trim();
                var description = TxtDescripTypeFrais.Text.Trim();

                var success = _paiements.CreateType_Frais(codTypeFrais, description);

                if (success)
                {
                    MessageBox.Show("Type de frais créé avec succès!", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset vers mode Création
                    BtnCancelTypeFrais_Click(sender, e);

                    LoadTypeFrais();
                    LoadComboBoxes(); // Recharger pour mettre à jour les listes
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdateTypeFrais_Click(object sender, EventArgs e)
        {
            if (!_isEditingTypeFrais)
            {
                // Activer le mode édition
                _isEditingTypeFrais = true;

                // Déverrouiller description, Verrouiller code
                TxtDescripTypeFrais.ReadOnly = false;
                TxtCodeTypeFrais.ReadOnly = true;

                SetButtonStates();
            }
            else
            {
                // Sauvegarder les modifications
                try
                {
                    if (!ValidateTypeFraisInput()) return;

                    var codTypeFrais = TxtCodeTypeFrais.Text.Trim();
                    var description = TxtDescripTypeFrais.Text.Trim();

                    var success = _paiements.UpdateType_Frais(codTypeFrais, description);

                    if (success)
                    {
                        MessageBox.Show("Type de frais modifié avec succès!", "Succès",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        _isEditingTypeFrais = false;

                        // Revrouiller tout en mode affichage
                        TxtDescripTypeFrais.ReadOnly = true;

                        LoadTypeFrais();
                        LoadComboBoxes();
                        SetButtonStates();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la modification: {ex.Message}", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDeleteTypeFrais_Click(object sender, EventArgs e)
        {
            if (DgvTypeFrais.SelectedRows.Count == 0) return;

            var result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer ce type de frais?", "Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var codTypeFrais = DgvTypeFrais.SelectedRows[0].Cells["cod_type_frais"].Value?.ToString();

                    var success = _paiements.DeleteType_Frais(codTypeFrais);

                    if (success)
                    {
                        MessageBox.Show("Type de frais supprimé avec succès!", "Succès",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Reset vers mode création
                        BtnCancelTypeFrais_Click(sender, e);

                        LoadTypeFrais();
                        LoadComboBoxes();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression: {ex.Message}", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnCancelTypeFrais_Click(object sender, EventArgs e)
        {
            _isEditingTypeFrais = false;
            ClearTypeFraisFields();

            // Mode CRÉATION : Déverrouiller
            TxtCodeTypeFrais.ReadOnly = false;
            TxtDescripTypeFrais.ReadOnly = false;

            DgvTypeFrais.ClearSelection();
            SetButtonStates();
        }

        private void DgvTypeFrais_SelectionChanged(object sender, EventArgs e)
        {
            if (DgvTypeFrais.SelectedRows.Count > 0)
            {
                var row = DgvTypeFrais.SelectedRows[0];

                TxtCodeTypeFrais.Text = row.Cells["cod_type_frais"].Value?.ToString() ?? "";
                TxtDescripTypeFrais.Text = row.Cells["description"].Value?.ToString() ?? "";

                // Mode AFFICHAGE : Verrouiller
                _isEditingTypeFrais = false;
                TxtCodeTypeFrais.ReadOnly = true;
                TxtDescripTypeFrais.ReadOnly = true;

                SetButtonStates();
            }
        }

        #endregion

        #region Filtrage

        private void TxtFiltreFrais_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var filter = TxtFiltreFrais.Text.Trim();
                if (string.IsNullOrEmpty(filter))
                {
                    LoadFrais();
                }
                else
                {
                    var allFrais = _paiements.GetFraisByEcole(_currentIdEcole, _currentAnneeScol);
                    var filteredFrais = allFrais.Where(f =>
                        f.description.ToString().ToLower().Contains(filter.ToLower()) ||
                        f.cod_frais.ToString().ToLower().Contains(filter.ToLower()) ||
                        f.Type_Frais_desc.ToString().ToLower().Contains(filter.ToLower())
                    ).ToList();

                    DgvFrais.DataSource = filteredFrais;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du filtrage: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtFiltreTypeFrais_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var filter = TxtFiltreTypeFrais.Text.Trim();
                if (string.IsNullOrEmpty(filter))
                {
                    LoadTypeFrais();
                }
                else
                {
                    var allTypeFrais = _paiements.GetAllType_Frais();
                    var filteredTypeFrais = allTypeFrais.Where(tf =>
                        tf.description.ToString().ToLower().Contains(filter.ToLower()) ||
                        tf.cod_type_frais.ToString().ToLower().Contains(filter.ToLower())
                    ).ToList();

                    DgvTypeFrais.DataSource = filteredTypeFrais;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du filtrage: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Validation et Utilitaires

        private bool ValidateFraisInput()
        {
            if (string.IsNullOrWhiteSpace(TxtCodeFrais.Text))
            {
                MessageBox.Show("Le code frais est obligatoire.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TxtCodeFrais.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtDescriptFrais.Text))
            {
                MessageBox.Show("La description est obligatoire.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TxtDescriptFrais.Focus();
                return false;
            }

            if (!decimal.TryParse(TxtMontantFrais.Text, out decimal montant) || montant <= 0)
            {
                MessageBox.Show("Le montant doit être un nombre positif.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TxtMontantFrais.Focus();
                return false;
            }

            if (CmbTypeFrais.SelectedValue == null)
            {
                MessageBox.Show("Veuillez sélectionner un type de frais.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CmbTypeFrais.Focus();
                return false;
            }

            return true;
        }

        private bool ValidateTypeFraisInput()
        {
            if (string.IsNullOrWhiteSpace(TxtCodeTypeFrais.Text))
            {
                MessageBox.Show("Le code type frais est obligatoire.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TxtCodeTypeFrais.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtDescripTypeFrais.Text))
            {
                MessageBox.Show("La description est obligatoire.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TxtDescripTypeFrais.Focus();
                return false;
            }

            return true;
        }

        private void ClearFraisFields()
        {
            TxtCodeFrais.Clear();
            TxtDescriptFrais.Clear();
            TxtMontantFrais.Clear();
            CmbTypeFrais.SelectedIndex = -1;
            CmbModalitePaiementFrais.SelectedIndex = -1;
            CmbPeriodeFrais.SelectedIndex = -1;
        }

        private void ClearTypeFraisFields()
        {
            TxtCodeTypeFrais.Clear();
            TxtDescripTypeFrais.Clear();
        }

        private void SetFieldsReadOnly(bool fraisReadOnly, bool typeFraisReadOnly)
        {
            // Champs Frais
            TxtCodeFrais.ReadOnly = fraisReadOnly;
            TxtDescriptFrais.ReadOnly = fraisReadOnly;
            TxtMontantFrais.ReadOnly = fraisReadOnly;
            CmbTypeFrais.Enabled = !fraisReadOnly;
            CmbModalitePaiementFrais.Enabled = !fraisReadOnly;
            CmbPeriodeFrais.Enabled = !fraisReadOnly;

            // Champs Type Frais
            TxtCodeTypeFrais.ReadOnly = typeFraisReadOnly;
            TxtDescripTypeFrais.ReadOnly = typeFraisReadOnly;
        }

        private void SetButtonStates()
        {
            // Boutons Frais (Actif si mode création -Code non vide- ou édition)
            // On simplifie: Si une ligne est sélectionnée, on peut update/delete. Sinon on peut sauver.
            bool hasSelectionFrais = DgvFrais.SelectedRows.Count > 0;

            BtnSaveFrais.Enabled = !hasSelectionFrais && !_isEditingFrais;
            BtnUpdateFrais.Enabled = hasSelectionFrais;
            BtnDeleteFrais.Enabled = hasSelectionFrais && !_isEditingFrais;
            BtnCancelFrais.Enabled = hasSelectionFrais || _isEditingFrais || !string.IsNullOrEmpty(TxtCodeFrais.Text);

            // Boutons Type Frais
            bool hasSelectionType = DgvTypeFrais.SelectedRows.Count > 0;

            BtnSaveTypeFrais.Enabled = !hasSelectionType && !_isEditingTypeFrais;
            BtnUpdateTypeFrais.Enabled = hasSelectionType;
            BtnDeleteTypeFrais.Enabled = hasSelectionType && !_isEditingTypeFrais;
            BtnCancelTypeFrais.Enabled = hasSelectionType || _isEditingTypeFrais || !string.IsNullOrEmpty(TxtCodeTypeFrais.Text);

            // Texte des boutons Update
            BtnUpdateFrais.Text = _isEditingFrais ? "Confirmer" : "Modifier";
            BtnUpdateTypeFrais.Text = _isEditingTypeFrais ? "Confirmer" : "Modifier";
        }

        #endregion

        #region Événements Menu

        private void MenuItemDeconnexion_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Êtes-vous sûr de vouloir vous déconnecter?", "Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void MenuItemQuitter_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Utiliser le contexte transmis ou des valeurs par défaut
            var username = _currentUsername ?? "Admin";

            // Mise à jour de la barre de statut
            toolStripStatusUser.Text = $"Connecté: {username}";
            toolStripStatusTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            // Timer pour mettre à jour l'heure
            var timer = new System.Windows.Forms.Timer { Interval = 1000 };
            timer.Tick += (s, args) => toolStripStatusTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            timer.Start();

            // Initialisation de l'état des boutons (Mode création par défaut)
            BtnCancelFrais_Click(this, EventArgs.Empty);
            BtnCancelTypeFrais_Click(this, EventArgs.Empty);
        }

        private void BtnDeleteTypeFrais_DoubleClick(object sender, EventArgs e)
        {

        }

        private void FormFrais_Load(object sender, EventArgs e)
        {

        }

        private void TxtDescripTypeFrais_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnSaveTypeFrais.PerformClick();
            }
        }
    }
}
