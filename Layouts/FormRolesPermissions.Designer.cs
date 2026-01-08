namespace EduKin.Layouts
{
    partial class FormRolesPermissions
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle9 = new DataGridViewCellStyle();
            panelMain = new Panel();
            panelContent = new Panel();
            panelAssignments = new Panel();
            panelAssignmentsContainer = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            panelAssignmentsList = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            dgvRolePermissions = new Siticone.Desktop.UI.WinForms.SiticoneDataGridView();
            panelAssignmentsForm = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            btnRemovePermission = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            btnAssignPermission = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            cmbPermissionAssignment = new Siticone.Desktop.UI.WinForms.SiticoneComboBox();
            lblPermissionAssignment = new Label();
            cmbRoleAssignment = new Siticone.Desktop.UI.WinForms.SiticoneComboBox();
            lblRoleAssignment = new Label();
            panelAssignmentsHeader = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            lblAssignmentsTitle = new Label();
            panelPermissions = new Panel();
            panelPermissionsContainer = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            panelPermissionsList = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            dgvPermissions = new Siticone.Desktop.UI.WinForms.SiticoneDataGridView();
            panelPermissionsForm = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            btnClearPermission = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            btnDeletePermission = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            btnUpdatePermission = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            btnCreatePermission = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            cmbModulePermission = new Siticone.Desktop.UI.WinForms.SiticoneComboBox();
            lblModulePermission = new Label();
            txtDescriptionPermission = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            lblDescriptionPermission = new Label();
            txtNomPermission = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            lblNomPermission = new Label();
            panelPermissionsHeader = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            lblPermissionsTitle = new Label();
            panelRoles = new Panel();
            panelRolesContainer = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            panelRolesList = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            dgvRoles = new Siticone.Desktop.UI.WinForms.SiticoneDataGridView();
            panelRolesForm = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            btnClearRole = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            btnDeleteRole = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            btnUpdateRole = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            btnCreateRole = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            txtDescriptionRole = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            lblDescriptionRole = new Label();
            txtNomRole = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            lblNomRole = new Label();
            panelRolesHeader = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            lblRolesTitle = new Label();
            panelNavigation = new Panel();
            btnAssignments = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            btnPermissions = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            btnRoles = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            panelMain.SuspendLayout();
            panelContent.SuspendLayout();
            panelAssignments.SuspendLayout();
            panelAssignmentsContainer.SuspendLayout();
            panelAssignmentsList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRolePermissions).BeginInit();
            panelAssignmentsForm.SuspendLayout();
            panelAssignmentsHeader.SuspendLayout();
            panelPermissions.SuspendLayout();
            panelPermissionsContainer.SuspendLayout();
            panelPermissionsList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPermissions).BeginInit();
            panelPermissionsForm.SuspendLayout();
            panelPermissionsHeader.SuspendLayout();
            panelRoles.SuspendLayout();
            panelRolesContainer.SuspendLayout();
            panelRolesList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRoles).BeginInit();
            panelRolesForm.SuspendLayout();
            panelRolesHeader.SuspendLayout();
            panelNavigation.SuspendLayout();
            SuspendLayout();
            // 
            // panelMain
            // 
            panelMain.Controls.Add(panelContent);
            panelMain.Controls.Add(panelNavigation);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(0, 0);
            panelMain.Name = "panelMain";
            panelMain.Size = new Size(1200, 700);
            panelMain.TabIndex = 0;
            // 
            // panelContent
            // 
            panelContent.Controls.Add(panelAssignments);
            panelContent.Controls.Add(panelPermissions);
            panelContent.Controls.Add(panelRoles);
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(0, 60);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(1200, 640);
            panelContent.TabIndex = 1;
            // 
            // panelAssignments
            // 
            panelAssignments.Controls.Add(panelAssignmentsContainer);
            panelAssignments.Dock = DockStyle.Fill;
            panelAssignments.Location = new Point(0, 0);
            panelAssignments.Name = "panelAssignments";
            panelAssignments.Size = new Size(1200, 640);
            panelAssignments.TabIndex = 2;
            panelAssignments.Visible = false;
            // 
            // panelAssignmentsContainer
            // 
            panelAssignmentsContainer.Controls.Add(panelAssignmentsList);
            panelAssignmentsContainer.Controls.Add(panelAssignmentsForm);
            panelAssignmentsContainer.Controls.Add(panelAssignmentsHeader);
            panelAssignmentsContainer.Dock = DockStyle.Fill;
            panelAssignmentsContainer.Location = new Point(0, 0);
            panelAssignmentsContainer.Name = "panelAssignmentsContainer";
            panelAssignmentsContainer.Size = new Size(1200, 640);
            panelAssignmentsContainer.TabIndex = 0;
            // 
            // panelAssignmentsList
            // 
            panelAssignmentsList.Controls.Add(dgvRolePermissions);
            panelAssignmentsList.Dock = DockStyle.Fill;
            panelAssignmentsList.Location = new Point(0, 250);
            panelAssignmentsList.Name = "panelAssignmentsList";
            panelAssignmentsList.Size = new Size(1200, 390);
            panelAssignmentsList.TabIndex = 2;
            // 
            // dgvRolePermissions
            // 
            dgvRolePermissions.AllowUserToAddRows = false;
            dgvRolePermissions.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = Color.White;
            dgvRolePermissions.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvRolePermissions.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvRolePermissions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvRolePermissions.DefaultCellStyle = dataGridViewCellStyle3;
            dgvRolePermissions.Dock = DockStyle.Fill;
            dgvRolePermissions.GridColor = Color.FromArgb(231, 229, 255);
            dgvRolePermissions.Location = new Point(0, 0);
            dgvRolePermissions.MultiSelect = false;
            dgvRolePermissions.Name = "dgvRolePermissions";
            dgvRolePermissions.ReadOnly = true;
            dgvRolePermissions.RowHeadersVisible = false;
            dgvRolePermissions.RowHeadersWidth = 51;
            dgvRolePermissions.Size = new Size(1200, 390);
            dgvRolePermissions.TabIndex = 0;
            dgvRolePermissions.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvRolePermissions.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvRolePermissions.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvRolePermissions.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvRolePermissions.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvRolePermissions.ThemeStyle.BackColor = Color.White;
            dgvRolePermissions.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvRolePermissions.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvRolePermissions.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvRolePermissions.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvRolePermissions.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvRolePermissions.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRolePermissions.ThemeStyle.HeaderStyle.Height = 4;
            dgvRolePermissions.ThemeStyle.ReadOnly = true;
            dgvRolePermissions.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvRolePermissions.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvRolePermissions.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvRolePermissions.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvRolePermissions.ThemeStyle.RowsStyle.Height = 29;
            dgvRolePermissions.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvRolePermissions.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            // 
            // panelAssignmentsForm
            // 
            panelAssignmentsForm.Controls.Add(btnRemovePermission);
            panelAssignmentsForm.Controls.Add(btnAssignPermission);
            panelAssignmentsForm.Controls.Add(cmbPermissionAssignment);
            panelAssignmentsForm.Controls.Add(lblPermissionAssignment);
            panelAssignmentsForm.Controls.Add(cmbRoleAssignment);
            panelAssignmentsForm.Controls.Add(lblRoleAssignment);
            panelAssignmentsForm.Dock = DockStyle.Top;
            panelAssignmentsForm.Location = new Point(0, 60);
            panelAssignmentsForm.Name = "panelAssignmentsForm";
            panelAssignmentsForm.Size = new Size(1200, 190);
            panelAssignmentsForm.TabIndex = 1;
            // 
            // btnRemovePermission
            // 
            btnRemovePermission.DisabledState.BorderColor = Color.DarkGray;
            btnRemovePermission.DisabledState.CustomBorderColor = Color.DarkGray;
            btnRemovePermission.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnRemovePermission.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnRemovePermission.FillColor = Color.Red;
            btnRemovePermission.Font = new Font("Segoe UI", 9F);
            btnRemovePermission.ForeColor = Color.White;
            btnRemovePermission.Location = new Point(340, 140);
            btnRemovePermission.Name = "btnRemovePermission";
            btnRemovePermission.Size = new Size(160, 35);
            btnRemovePermission.TabIndex = 5;
            btnRemovePermission.Text = "Retirer Permission";
            btnRemovePermission.Click += BtnRemovePermission_Click;
            // 
            // btnAssignPermission
            // 
            btnAssignPermission.DisabledState.BorderColor = Color.DarkGray;
            btnAssignPermission.DisabledState.CustomBorderColor = Color.DarkGray;
            btnAssignPermission.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnAssignPermission.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnAssignPermission.Font = new Font("Segoe UI", 9F);
            btnAssignPermission.ForeColor = Color.White;
            btnAssignPermission.Location = new Point(60, 140);
            btnAssignPermission.Name = "btnAssignPermission";
            btnAssignPermission.Size = new Size(160, 35);
            btnAssignPermission.TabIndex = 4;
            btnAssignPermission.Text = "Attribuer Permission";
            btnAssignPermission.Click += BtnAssignPermission_Click;
            // 
            // cmbPermissionAssignment
            // 
            cmbPermissionAssignment.BackColor = Color.Transparent;
            cmbPermissionAssignment.BorderRadius = 14;
            cmbPermissionAssignment.DrawMode = DrawMode.OwnerDrawFixed;
            cmbPermissionAssignment.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPermissionAssignment.FocusedColor = Color.FromArgb(94, 148, 255);
            cmbPermissionAssignment.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cmbPermissionAssignment.Font = new Font("Segoe UI", 10F);
            cmbPermissionAssignment.ForeColor = Color.FromArgb(68, 88, 112);
            cmbPermissionAssignment.ItemHeight = 30;
            cmbPermissionAssignment.Location = new Point(200, 90);
            cmbPermissionAssignment.Name = "cmbPermissionAssignment";
            cmbPermissionAssignment.Size = new Size(400, 36);
            cmbPermissionAssignment.TabIndex = 3;
            // 
            // lblPermissionAssignment
            // 
            lblPermissionAssignment.AutoSize = true;
            lblPermissionAssignment.Font = new Font("Segoe UI", 10F);
            lblPermissionAssignment.Location = new Point(60, 90);
            lblPermissionAssignment.Name = "lblPermissionAssignment";
            lblPermissionAssignment.Size = new Size(91, 23);
            lblPermissionAssignment.TabIndex = 2;
            lblPermissionAssignment.Text = "Permission";
            // 
            // cmbRoleAssignment
            // 
            cmbRoleAssignment.BackColor = Color.Transparent;
            cmbRoleAssignment.BorderRadius = 14;
            cmbRoleAssignment.DrawMode = DrawMode.OwnerDrawFixed;
            cmbRoleAssignment.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRoleAssignment.FocusedColor = Color.FromArgb(94, 148, 255);
            cmbRoleAssignment.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cmbRoleAssignment.Font = new Font("Segoe UI", 10F);
            cmbRoleAssignment.ForeColor = Color.FromArgb(68, 88, 112);
            cmbRoleAssignment.ItemHeight = 30;
            cmbRoleAssignment.Location = new Point(200, 40);
            cmbRoleAssignment.Name = "cmbRoleAssignment";
            cmbRoleAssignment.Size = new Size(300, 36);
            cmbRoleAssignment.TabIndex = 1;
            // 
            // lblRoleAssignment
            // 
            lblRoleAssignment.AutoSize = true;
            lblRoleAssignment.Font = new Font("Segoe UI", 10F);
            lblRoleAssignment.Location = new Point(60, 40);
            lblRoleAssignment.Name = "lblRoleAssignment";
            lblRoleAssignment.Size = new Size(43, 23);
            lblRoleAssignment.TabIndex = 0;
            lblRoleAssignment.Text = "Rôle";
            // 
            // panelAssignmentsHeader
            // 
            panelAssignmentsHeader.BackColor = Color.FromArgb(5, 21, 48);
            panelAssignmentsHeader.Controls.Add(lblAssignmentsTitle);
            panelAssignmentsHeader.Dock = DockStyle.Top;
            panelAssignmentsHeader.Location = new Point(0, 0);
            panelAssignmentsHeader.Name = "panelAssignmentsHeader";
            panelAssignmentsHeader.Size = new Size(1200, 60);
            panelAssignmentsHeader.TabIndex = 0;
            // 
            // lblAssignmentsTitle
            // 
            lblAssignmentsTitle.AutoSize = true;
            lblAssignmentsTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblAssignmentsTitle.ForeColor = Color.White;
            lblAssignmentsTitle.Location = new Point(20, 15);
            lblAssignmentsTitle.Name = "lblAssignmentsTitle";
            lblAssignmentsTitle.Size = new Size(371, 37);
            lblAssignmentsTitle.TabIndex = 0;
            lblAssignmentsTitle.Text = "Attribution des Permissions";
            // 
            // panelPermissions
            // 
            panelPermissions.Controls.Add(panelPermissionsContainer);
            panelPermissions.Dock = DockStyle.Fill;
            panelPermissions.Location = new Point(0, 0);
            panelPermissions.Name = "panelPermissions";
            panelPermissions.Size = new Size(1200, 640);
            panelPermissions.TabIndex = 1;
            panelPermissions.Visible = false;
            // 
            // panelPermissionsContainer
            // 
            panelPermissionsContainer.Controls.Add(panelPermissionsList);
            panelPermissionsContainer.Controls.Add(panelPermissionsForm);
            panelPermissionsContainer.Controls.Add(panelPermissionsHeader);
            panelPermissionsContainer.Dock = DockStyle.Fill;
            panelPermissionsContainer.Location = new Point(0, 0);
            panelPermissionsContainer.Name = "panelPermissionsContainer";
            panelPermissionsContainer.Size = new Size(1200, 640);
            panelPermissionsContainer.TabIndex = 0;
            // 
            // panelPermissionsList
            // 
            panelPermissionsList.Controls.Add(dgvPermissions);
            panelPermissionsList.Dock = DockStyle.Fill;
            panelPermissionsList.Location = new Point(0, 350);
            panelPermissionsList.Name = "panelPermissionsList";
            panelPermissionsList.Size = new Size(1200, 290);
            panelPermissionsList.TabIndex = 2;
            // 
            // dgvPermissions
            // 
            dgvPermissions.AllowUserToAddRows = false;
            dgvPermissions.AllowUserToDeleteRows = false;
            dataGridViewCellStyle4.BackColor = Color.White;
            dgvPermissions.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle5.ForeColor = Color.White;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.True;
            dgvPermissions.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            dgvPermissions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = Color.White;
            dataGridViewCellStyle6.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle6.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle6.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle6.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            dgvPermissions.DefaultCellStyle = dataGridViewCellStyle6;
            dgvPermissions.Dock = DockStyle.Fill;
            dgvPermissions.GridColor = Color.FromArgb(231, 229, 255);
            dgvPermissions.Location = new Point(0, 0);
            dgvPermissions.MultiSelect = false;
            dgvPermissions.Name = "dgvPermissions";
            dgvPermissions.ReadOnly = true;
            dgvPermissions.RowHeadersVisible = false;
            dgvPermissions.RowHeadersWidth = 51;
            dgvPermissions.Size = new Size(1200, 290);
            dgvPermissions.TabIndex = 0;
            dgvPermissions.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvPermissions.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvPermissions.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvPermissions.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvPermissions.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvPermissions.ThemeStyle.BackColor = Color.White;
            dgvPermissions.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvPermissions.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvPermissions.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvPermissions.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvPermissions.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvPermissions.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPermissions.ThemeStyle.HeaderStyle.Height = 4;
            dgvPermissions.ThemeStyle.ReadOnly = true;
            dgvPermissions.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvPermissions.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvPermissions.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvPermissions.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvPermissions.ThemeStyle.RowsStyle.Height = 29;
            dgvPermissions.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvPermissions.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dgvPermissions.SelectionChanged += DgvPermissions_SelectionChanged;
            // 
            // panelPermissionsForm
            // 
            panelPermissionsForm.Controls.Add(btnClearPermission);
            panelPermissionsForm.Controls.Add(btnDeletePermission);
            panelPermissionsForm.Controls.Add(btnUpdatePermission);
            panelPermissionsForm.Controls.Add(btnCreatePermission);
            panelPermissionsForm.Controls.Add(cmbModulePermission);
            panelPermissionsForm.Controls.Add(lblModulePermission);
            panelPermissionsForm.Controls.Add(txtDescriptionPermission);
            panelPermissionsForm.Controls.Add(lblDescriptionPermission);
            panelPermissionsForm.Controls.Add(txtNomPermission);
            panelPermissionsForm.Controls.Add(lblNomPermission);
            panelPermissionsForm.Dock = DockStyle.Top;
            panelPermissionsForm.Location = new Point(0, 60);
            panelPermissionsForm.Name = "panelPermissionsForm";
            panelPermissionsForm.Size = new Size(1200, 290);
            panelPermissionsForm.TabIndex = 1;
            // 
            // btnClearPermission
            // 
            btnClearPermission.DisabledState.BorderColor = Color.DarkGray;
            btnClearPermission.DisabledState.CustomBorderColor = Color.DarkGray;
            btnClearPermission.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnClearPermission.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnClearPermission.FillColor = Color.Gray;
            btnClearPermission.Font = new Font("Segoe UI", 9F);
            btnClearPermission.ForeColor = Color.White;
            btnClearPermission.Location = new Point(480, 230);
            btnClearPermission.Name = "btnClearPermission";
            btnClearPermission.Size = new Size(120, 35);
            btnClearPermission.TabIndex = 9;
            btnClearPermission.Text = "Effacer";
            btnClearPermission.Click += BtnClearPermission_Click;
            // 
            // btnDeletePermission
            // 
            btnDeletePermission.DisabledState.BorderColor = Color.DarkGray;
            btnDeletePermission.DisabledState.CustomBorderColor = Color.DarkGray;
            btnDeletePermission.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnDeletePermission.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnDeletePermission.FillColor = Color.Red;
            btnDeletePermission.Font = new Font("Segoe UI", 9F);
            btnDeletePermission.ForeColor = Color.White;
            btnDeletePermission.Location = new Point(340, 230);
            btnDeletePermission.Name = "btnDeletePermission";
            btnDeletePermission.Size = new Size(120, 35);
            btnDeletePermission.TabIndex = 8;
            btnDeletePermission.Text = "Supprimer";
            btnDeletePermission.Click += BtnDeletePermission_Click;
            // 
            // btnUpdatePermission
            // 
            btnUpdatePermission.DisabledState.BorderColor = Color.DarkGray;
            btnUpdatePermission.DisabledState.CustomBorderColor = Color.DarkGray;
            btnUpdatePermission.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnUpdatePermission.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnUpdatePermission.FillColor = Color.Orange;
            btnUpdatePermission.Font = new Font("Segoe UI", 9F);
            btnUpdatePermission.ForeColor = Color.White;
            btnUpdatePermission.Location = new Point(200, 230);
            btnUpdatePermission.Name = "btnUpdatePermission";
            btnUpdatePermission.Size = new Size(120, 35);
            btnUpdatePermission.TabIndex = 7;
            btnUpdatePermission.Text = "Modifier";
            btnUpdatePermission.Click += BtnUpdatePermission_Click;
            // 
            // btnCreatePermission
            // 
            btnCreatePermission.DisabledState.BorderColor = Color.DarkGray;
            btnCreatePermission.DisabledState.CustomBorderColor = Color.DarkGray;
            btnCreatePermission.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnCreatePermission.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnCreatePermission.Font = new Font("Segoe UI", 9F);
            btnCreatePermission.ForeColor = Color.White;
            btnCreatePermission.Location = new Point(60, 230);
            btnCreatePermission.Name = "btnCreatePermission";
            btnCreatePermission.Size = new Size(120, 35);
            btnCreatePermission.TabIndex = 6;
            btnCreatePermission.Text = "Créer";
            btnCreatePermission.Click += BtnCreatePermission_Click;
            // 
            // cmbModulePermission
            // 
            cmbModulePermission.BackColor = Color.Transparent;
            cmbModulePermission.DrawMode = DrawMode.OwnerDrawFixed;
            cmbModulePermission.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbModulePermission.FocusedColor = Color.FromArgb(94, 148, 255);
            cmbModulePermission.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cmbModulePermission.Font = new Font("Segoe UI", 10F);
            cmbModulePermission.ForeColor = Color.FromArgb(68, 88, 112);
            cmbModulePermission.ItemHeight = 30;
            cmbModulePermission.Location = new Point(200, 170);
            cmbModulePermission.Name = "cmbModulePermission";
            cmbModulePermission.Size = new Size(300, 36);
            cmbModulePermission.TabIndex = 5;
            // 
            // lblModulePermission
            // 
            lblModulePermission.AutoSize = true;
            lblModulePermission.Font = new Font("Segoe UI", 10F);
            lblModulePermission.Location = new Point(60, 170);
            lblModulePermission.Name = "lblModulePermission";
            lblModulePermission.Size = new Size(68, 23);
            lblModulePermission.TabIndex = 4;
            lblModulePermission.Text = "Module";
            // 
            // txtDescriptionPermission
            // 
            txtDescriptionPermission.Cursor = Cursors.IBeam;
            txtDescriptionPermission.DefaultText = "";
            txtDescriptionPermission.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtDescriptionPermission.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtDescriptionPermission.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtDescriptionPermission.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtDescriptionPermission.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtDescriptionPermission.Font = new Font("Segoe UI", 9F);
            txtDescriptionPermission.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtDescriptionPermission.Location = new Point(200, 110);
            txtDescriptionPermission.Margin = new Padding(3, 4, 3, 4);
            txtDescriptionPermission.Multiline = true;
            txtDescriptionPermission.Name = "txtDescriptionPermission";
            txtDescriptionPermission.PasswordChar = '\0';
            txtDescriptionPermission.PlaceholderText = "Description de la permission";
            txtDescriptionPermission.SelectedText = "";
            txtDescriptionPermission.Size = new Size(400, 50);
            txtDescriptionPermission.TabIndex = 3;
            // 
            // lblDescriptionPermission
            // 
            lblDescriptionPermission.AutoSize = true;
            lblDescriptionPermission.Font = new Font("Segoe UI", 10F);
            lblDescriptionPermission.Location = new Point(60, 110);
            lblDescriptionPermission.Name = "lblDescriptionPermission";
            lblDescriptionPermission.Size = new Size(96, 23);
            lblDescriptionPermission.TabIndex = 2;
            lblDescriptionPermission.Text = "Description";
            // 
            // txtNomPermission
            // 
            txtNomPermission.Cursor = Cursors.IBeam;
            txtNomPermission.DefaultText = "";
            txtNomPermission.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtNomPermission.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtNomPermission.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtNomPermission.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtNomPermission.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtNomPermission.Font = new Font("Segoe UI", 9F);
            txtNomPermission.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtNomPermission.Location = new Point(200, 60);
            txtNomPermission.Margin = new Padding(3, 4, 3, 4);
            txtNomPermission.Name = "txtNomPermission";
            txtNomPermission.PasswordChar = '\0';
            txtNomPermission.PlaceholderText = "Nom de la permission";
            txtNomPermission.SelectedText = "";
            txtNomPermission.Size = new Size(300, 36);
            txtNomPermission.TabIndex = 1;
            // 
            // lblNomPermission
            // 
            lblNomPermission.AutoSize = true;
            lblNomPermission.Font = new Font("Segoe UI", 10F);
            lblNomPermission.Location = new Point(60, 60);
            lblNomPermission.Name = "lblNomPermission";
            lblNomPermission.Size = new Size(134, 23);
            lblNomPermission.TabIndex = 0;
            lblNomPermission.Text = "Nom Permission";
            // 
            // panelPermissionsHeader
            // 
            panelPermissionsHeader.BackColor = Color.FromArgb(94, 148, 255);
            panelPermissionsHeader.Controls.Add(lblPermissionsTitle);
            panelPermissionsHeader.Dock = DockStyle.Top;
            panelPermissionsHeader.Location = new Point(0, 0);
            panelPermissionsHeader.Name = "panelPermissionsHeader";
            panelPermissionsHeader.Size = new Size(1200, 60);
            panelPermissionsHeader.TabIndex = 0;
            // 
            // lblPermissionsTitle
            // 
            lblPermissionsTitle.AutoSize = true;
            lblPermissionsTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblPermissionsTitle.ForeColor = Color.White;
            lblPermissionsTitle.Location = new Point(20, 15);
            lblPermissionsTitle.Name = "lblPermissionsTitle";
            lblPermissionsTitle.Size = new Size(274, 37);
            lblPermissionsTitle.TabIndex = 0;
            lblPermissionsTitle.Text = "Gestion Permissions";
            // 
            // panelRoles
            // 
            panelRoles.Controls.Add(panelRolesContainer);
            panelRoles.Dock = DockStyle.Fill;
            panelRoles.Location = new Point(0, 0);
            panelRoles.Name = "panelRoles";
            panelRoles.Size = new Size(1200, 640);
            panelRoles.TabIndex = 0;
            // 
            // panelRolesContainer
            // 
            panelRolesContainer.Controls.Add(panelRolesList);
            panelRolesContainer.Controls.Add(panelRolesForm);
            panelRolesContainer.Controls.Add(panelRolesHeader);
            panelRolesContainer.Dock = DockStyle.Fill;
            panelRolesContainer.Location = new Point(0, 0);
            panelRolesContainer.Name = "panelRolesContainer";
            panelRolesContainer.Size = new Size(1200, 640);
            panelRolesContainer.TabIndex = 0;
            // 
            // panelRolesList
            // 
            panelRolesList.Controls.Add(dgvRoles);
            panelRolesList.Dock = DockStyle.Fill;
            panelRolesList.Location = new Point(0, 350);
            panelRolesList.Name = "panelRolesList";
            panelRolesList.Size = new Size(1200, 290);
            panelRolesList.TabIndex = 2;
            // 
            // dgvRoles
            // 
            dgvRoles.AllowUserToAddRows = false;
            dgvRoles.AllowUserToDeleteRows = false;
            dataGridViewCellStyle7.BackColor = Color.White;
            dgvRoles.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            dataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle8.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle8.ForeColor = Color.White;
            dataGridViewCellStyle8.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = DataGridViewTriState.True;
            dgvRoles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            dgvRoles.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle9.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = Color.White;
            dataGridViewCellStyle9.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle9.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle9.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle9.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle9.WrapMode = DataGridViewTriState.False;
            dgvRoles.DefaultCellStyle = dataGridViewCellStyle9;
            dgvRoles.Dock = DockStyle.Fill;
            dgvRoles.GridColor = Color.FromArgb(231, 229, 255);
            dgvRoles.Location = new Point(0, 0);
            dgvRoles.MultiSelect = false;
            dgvRoles.Name = "dgvRoles";
            dgvRoles.ReadOnly = true;
            dgvRoles.RowHeadersVisible = false;
            dgvRoles.RowHeadersWidth = 51;
            dgvRoles.Size = new Size(1200, 290);
            dgvRoles.TabIndex = 0;
            dgvRoles.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvRoles.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvRoles.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvRoles.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvRoles.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvRoles.ThemeStyle.BackColor = Color.White;
            dgvRoles.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvRoles.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvRoles.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvRoles.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvRoles.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvRoles.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRoles.ThemeStyle.HeaderStyle.Height = 4;
            dgvRoles.ThemeStyle.ReadOnly = true;
            dgvRoles.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvRoles.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvRoles.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvRoles.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvRoles.ThemeStyle.RowsStyle.Height = 29;
            dgvRoles.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvRoles.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dgvRoles.SelectionChanged += DgvRoles_SelectionChanged;
            // 
            // panelRolesForm
            // 
            panelRolesForm.Controls.Add(btnClearRole);
            panelRolesForm.Controls.Add(btnDeleteRole);
            panelRolesForm.Controls.Add(btnUpdateRole);
            panelRolesForm.Controls.Add(btnCreateRole);
            panelRolesForm.Controls.Add(txtDescriptionRole);
            panelRolesForm.Controls.Add(lblDescriptionRole);
            panelRolesForm.Controls.Add(txtNomRole);
            panelRolesForm.Controls.Add(lblNomRole);
            panelRolesForm.Dock = DockStyle.Top;
            panelRolesForm.Location = new Point(0, 60);
            panelRolesForm.Name = "panelRolesForm";
            panelRolesForm.Size = new Size(1200, 290);
            panelRolesForm.TabIndex = 1;
            // 
            // btnClearRole
            // 
            btnClearRole.DisabledState.BorderColor = Color.DarkGray;
            btnClearRole.DisabledState.CustomBorderColor = Color.DarkGray;
            btnClearRole.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnClearRole.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnClearRole.FillColor = Color.Gray;
            btnClearRole.Font = new Font("Segoe UI", 9F);
            btnClearRole.ForeColor = Color.White;
            btnClearRole.Location = new Point(480, 200);
            btnClearRole.Name = "btnClearRole";
            btnClearRole.Size = new Size(120, 35);
            btnClearRole.TabIndex = 7;
            btnClearRole.Text = "Effacer";
            btnClearRole.Click += BtnClearRole_Click;
            // 
            // btnDeleteRole
            // 
            btnDeleteRole.DisabledState.BorderColor = Color.DarkGray;
            btnDeleteRole.DisabledState.CustomBorderColor = Color.DarkGray;
            btnDeleteRole.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnDeleteRole.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnDeleteRole.FillColor = Color.Red;
            btnDeleteRole.Font = new Font("Segoe UI", 9F);
            btnDeleteRole.ForeColor = Color.White;
            btnDeleteRole.Location = new Point(340, 200);
            btnDeleteRole.Name = "btnDeleteRole";
            btnDeleteRole.Size = new Size(120, 35);
            btnDeleteRole.TabIndex = 6;
            btnDeleteRole.Text = "Supprimer";
            btnDeleteRole.Click += BtnDeleteRole_Click;
            // 
            // btnUpdateRole
            // 
            btnUpdateRole.DisabledState.BorderColor = Color.DarkGray;
            btnUpdateRole.DisabledState.CustomBorderColor = Color.DarkGray;
            btnUpdateRole.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnUpdateRole.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnUpdateRole.FillColor = Color.Orange;
            btnUpdateRole.Font = new Font("Segoe UI", 9F);
            btnUpdateRole.ForeColor = Color.White;
            btnUpdateRole.Location = new Point(200, 200);
            btnUpdateRole.Name = "btnUpdateRole";
            btnUpdateRole.Size = new Size(120, 35);
            btnUpdateRole.TabIndex = 5;
            btnUpdateRole.Text = "Modifier";
            btnUpdateRole.Click += BtnUpdateRole_Click;
            // 
            // btnCreateRole
            // 
            btnCreateRole.DisabledState.BorderColor = Color.DarkGray;
            btnCreateRole.DisabledState.CustomBorderColor = Color.DarkGray;
            btnCreateRole.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnCreateRole.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnCreateRole.Font = new Font("Segoe UI", 9F);
            btnCreateRole.ForeColor = Color.White;
            btnCreateRole.Location = new Point(60, 200);
            btnCreateRole.Name = "btnCreateRole";
            btnCreateRole.Size = new Size(120, 35);
            btnCreateRole.TabIndex = 4;
            btnCreateRole.Text = "Créer";
            btnCreateRole.Click += BtnCreateRole_Click;
            // 
            // txtDescriptionRole
            // 
            txtDescriptionRole.Cursor = Cursors.IBeam;
            txtDescriptionRole.DefaultText = "";
            txtDescriptionRole.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtDescriptionRole.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtDescriptionRole.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtDescriptionRole.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtDescriptionRole.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtDescriptionRole.Font = new Font("Segoe UI", 9F);
            txtDescriptionRole.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtDescriptionRole.Location = new Point(200, 120);
            txtDescriptionRole.Margin = new Padding(3, 4, 3, 4);
            txtDescriptionRole.Multiline = true;
            txtDescriptionRole.Name = "txtDescriptionRole";
            txtDescriptionRole.PasswordChar = '\0';
            txtDescriptionRole.PlaceholderText = "Description du rôle";
            txtDescriptionRole.SelectedText = "";
            txtDescriptionRole.Size = new Size(400, 60);
            txtDescriptionRole.TabIndex = 3;
            // 
            // lblDescriptionRole
            // 
            lblDescriptionRole.AutoSize = true;
            lblDescriptionRole.Font = new Font("Segoe UI", 10F);
            lblDescriptionRole.Location = new Point(60, 120);
            lblDescriptionRole.Name = "lblDescriptionRole";
            lblDescriptionRole.Size = new Size(96, 23);
            lblDescriptionRole.TabIndex = 2;
            lblDescriptionRole.Text = "Description";
            // 
            // txtNomRole
            // 
            txtNomRole.Cursor = Cursors.IBeam;
            txtNomRole.DefaultText = "";
            txtNomRole.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtNomRole.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtNomRole.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtNomRole.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtNomRole.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtNomRole.Font = new Font("Segoe UI", 9F);
            txtNomRole.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtNomRole.Location = new Point(200, 60);
            txtNomRole.Margin = new Padding(3, 4, 3, 4);
            txtNomRole.Name = "txtNomRole";
            txtNomRole.PasswordChar = '\0';
            txtNomRole.PlaceholderText = "Nom du rôle";
            txtNomRole.SelectedText = "";
            txtNomRole.Size = new Size(300, 36);
            txtNomRole.TabIndex = 1;
            // 
            // lblNomRole
            // 
            lblNomRole.AutoSize = true;
            lblNomRole.Font = new Font("Segoe UI", 10F);
            lblNomRole.Location = new Point(60, 60);
            lblNomRole.Name = "lblNomRole";
            lblNomRole.Size = new Size(86, 23);
            lblNomRole.TabIndex = 0;
            lblNomRole.Text = "Nom Rôle";
            // 
            // panelRolesHeader
            // 
            panelRolesHeader.BackColor = Color.FromArgb(94, 148, 255);
            panelRolesHeader.Controls.Add(lblRolesTitle);
            panelRolesHeader.Dock = DockStyle.Top;
            panelRolesHeader.Location = new Point(0, 0);
            panelRolesHeader.Name = "panelRolesHeader";
            panelRolesHeader.Size = new Size(1200, 60);
            panelRolesHeader.TabIndex = 0;
            // 
            // lblRolesTitle
            // 
            lblRolesTitle.AutoSize = true;
            lblRolesTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblRolesTitle.ForeColor = Color.White;
            lblRolesTitle.Location = new Point(20, 15);
            lblRolesTitle.Name = "lblRolesTitle";
            lblRolesTitle.Size = new Size(191, 37);
            lblRolesTitle.TabIndex = 0;
            lblRolesTitle.Text = "Gestion Rôles";
            // 
            // panelNavigation
            // 
            panelNavigation.BackColor = Color.FromArgb(5, 21, 48);
            panelNavigation.Controls.Add(btnAssignments);
            panelNavigation.Controls.Add(btnPermissions);
            panelNavigation.Controls.Add(btnRoles);
            panelNavigation.Dock = DockStyle.Top;
            panelNavigation.Location = new Point(0, 0);
            panelNavigation.Name = "panelNavigation";
            panelNavigation.Size = new Size(1200, 60);
            panelNavigation.TabIndex = 0;
            // 
            // btnAssignments
            // 
            btnAssignments.BorderColor = Color.White;
            btnAssignments.BorderRadius = 14;
            btnAssignments.BorderThickness = 2;
            btnAssignments.ButtonMode = Siticone.Desktop.UI.WinForms.Enums.ButtonMode.RadioButton;
            btnAssignments.DisabledState.BorderColor = Color.DarkGray;
            btnAssignments.DisabledState.CustomBorderColor = Color.DarkGray;
            btnAssignments.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnAssignments.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnAssignments.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAssignments.ForeColor = Color.White;
            btnAssignments.Location = new Point(799, 22);
            btnAssignments.Name = "btnAssignments";
            btnAssignments.Size = new Size(380, 49);
            btnAssignments.TabIndex = 2;
            btnAssignments.Text = "Attributions";
            btnAssignments.TextOffset = new Point(0, -5);
            btnAssignments.Click += BtnAssignments_Click;
            // 
            // btnPermissions
            // 
            btnPermissions.BorderColor = Color.White;
            btnPermissions.BorderRadius = 14;
            btnPermissions.BorderThickness = 2;
            btnPermissions.ButtonMode = Siticone.Desktop.UI.WinForms.Enums.ButtonMode.RadioButton;
            btnPermissions.DisabledState.BorderColor = Color.DarkGray;
            btnPermissions.DisabledState.CustomBorderColor = Color.DarkGray;
            btnPermissions.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnPermissions.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnPermissions.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnPermissions.ForeColor = Color.White;
            btnPermissions.Location = new Point(410, 22);
            btnPermissions.Name = "btnPermissions";
            btnPermissions.Size = new Size(380, 49);
            btnPermissions.TabIndex = 1;
            btnPermissions.Text = "Permissions";
            btnPermissions.TextOffset = new Point(0, -5);
            btnPermissions.Click += BtnPermissions_Click;
            // 
            // btnRoles
            // 
            btnRoles.BorderColor = Color.White;
            btnRoles.BorderRadius = 14;
            btnRoles.BorderThickness = 2;
            btnRoles.ButtonMode = Siticone.Desktop.UI.WinForms.Enums.ButtonMode.RadioButton;
            btnRoles.DisabledState.BorderColor = Color.DarkGray;
            btnRoles.DisabledState.CustomBorderColor = Color.DarkGray;
            btnRoles.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnRoles.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnRoles.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnRoles.ForeColor = Color.White;
            btnRoles.Location = new Point(19, 22);
            btnRoles.Name = "btnRoles";
            btnRoles.Size = new Size(380, 49);
            btnRoles.TabIndex = 0;
            btnRoles.Text = "Rôles";
            btnRoles.TextOffset = new Point(0, -5);
            btnRoles.Click += BtnRoles_Click;
            // 
            // FormRolesPermissions
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 700);
            Controls.Add(panelMain);
            Name = "FormRolesPermissions";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Gestion des Rôles et Permissions";
            Load += FormRolesPermissions_Load;
            panelMain.ResumeLayout(false);
            panelContent.ResumeLayout(false);
            panelAssignments.ResumeLayout(false);
            panelAssignmentsContainer.ResumeLayout(false);
            panelAssignmentsList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvRolePermissions).EndInit();
            panelAssignmentsForm.ResumeLayout(false);
            panelAssignmentsForm.PerformLayout();
            panelAssignmentsHeader.ResumeLayout(false);
            panelAssignmentsHeader.PerformLayout();
            panelPermissions.ResumeLayout(false);
            panelPermissionsContainer.ResumeLayout(false);
            panelPermissionsList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvPermissions).EndInit();
            panelPermissionsForm.ResumeLayout(false);
            panelPermissionsForm.PerformLayout();
            panelPermissionsHeader.ResumeLayout(false);
            panelPermissionsHeader.PerformLayout();
            panelRoles.ResumeLayout(false);
            panelRolesContainer.ResumeLayout(false);
            panelRolesList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvRoles).EndInit();
            panelRolesForm.ResumeLayout(false);
            panelRolesForm.PerformLayout();
            panelRolesHeader.ResumeLayout(false);
            panelRolesHeader.PerformLayout();
            panelNavigation.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panelMain;
        private Panel panelNavigation;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnAssignments;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnPermissions;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnRoles;
        private Panel panelContent;
        
        // Rôles
        private Panel panelRoles;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelRolesContainer;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelRolesList;
        private Siticone.Desktop.UI.WinForms.SiticoneDataGridView dgvRoles;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelRolesForm;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnClearRole;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnDeleteRole;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnUpdateRole;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnCreateRole;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txtDescriptionRole;
        private Label lblDescriptionRole;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txtNomRole;
        private Label lblNomRole;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelRolesHeader;
        private Label lblRolesTitle;
        
        // Permissions
        private Panel panelPermissions;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelPermissionsContainer;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelPermissionsList;
        private Siticone.Desktop.UI.WinForms.SiticoneDataGridView dgvPermissions;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelPermissionsForm;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnClearPermission;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnDeletePermission;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnUpdatePermission;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnCreatePermission;
        private Siticone.Desktop.UI.WinForms.SiticoneComboBox cmbModulePermission;
        private Label lblModulePermission;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txtDescriptionPermission;
        private Label lblDescriptionPermission;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txtNomPermission;
        private Label lblNomPermission;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelPermissionsHeader;
        private Label lblPermissionsTitle;
        
        // Attributions
        private Panel panelAssignments;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelAssignmentsContainer;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelAssignmentsList;
        private Siticone.Desktop.UI.WinForms.SiticoneDataGridView dgvRolePermissions;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelAssignmentsForm;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnRemovePermission;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnAssignPermission;
        private Siticone.Desktop.UI.WinForms.SiticoneComboBox cmbPermissionAssignment;
        private Label lblPermissionAssignment;
        private Siticone.Desktop.UI.WinForms.SiticoneComboBox cmbRoleAssignment;
        private Label lblRoleAssignment;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelAssignmentsHeader;
        private Label lblAssignmentsTitle;
    }
}