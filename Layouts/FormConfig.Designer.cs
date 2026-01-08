namespace EduKin.Layouts
{
    partial class FormConfig
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
            panelMain = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            btnToggleMode = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            lblTitle = new Label();
            panelSelection = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            btnCancel = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            btnSelectSchool = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            lblNoSchool = new Label();
            lstEcoles = new ListView();
            colEcole = new ColumnHeader();
            colAvenue = new ColumnHeader();
            colNumero = new ColumnHeader();
            lblEcoles = new Label();
            lstAvenues = new ListView();
            colAvenueHierarchy = new ColumnHeader();
            txtAvenue = new TextBox();
            lblAvenue = new Label();
            lblSelectionTitle = new Label();
            panelCreation = new Siticone.Desktop.UI.WinForms.SiticonePanel();
            siticoneButton1 = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            btnCreateSchool = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            txtNumero = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            lblNumero = new Label();
            txtAnneeScol = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            lblAnneeScol = new Label();
            txtDenomination = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            lblDenomination = new Label();
            lstNewAvenues = new ListView();
            colNewAvenueHierarchy = new ColumnHeader();
            txtNewAvenue = new TextBox();
            lblNewAvenue = new Label();
            lblCreationTitle = new Label();
            panelMain.SuspendLayout();
            panelSelection.SuspendLayout();
            panelCreation.SuspendLayout();
            SuspendLayout();
            // 
            // panelMain
            // 
            panelMain.BackColor = Color.Transparent;
            panelMain.BorderRadius = 15;
            panelMain.Controls.Add(btnToggleMode);
            panelMain.Controls.Add(lblTitle);
            panelMain.Controls.Add(panelCreation);
            panelMain.Controls.Add(panelSelection);
            panelMain.FillColor = Color.White;
            panelMain.Location = new Point(30, 30);
            panelMain.Name = "panelMain";
            panelMain.ShadowDecoration.BorderRadius = 15;
            panelMain.ShadowDecoration.Depth = 20;
            panelMain.ShadowDecoration.Enabled = true;
            panelMain.Size = new Size(740, 688);
            panelMain.TabIndex = 0;
            // 
            // btnToggleMode
            // 
            btnToggleMode.BorderRadius = 20;
            btnToggleMode.FillColor = Color.FromArgb(5, 21, 48);
            btnToggleMode.Font = new Font("Segoe UI", 10F);
            btnToggleMode.ForeColor = Color.White;
            btnToggleMode.Location = new Point(470, 70);
            btnToggleMode.Name = "btnToggleMode";
            btnToggleMode.Size = new Size(250, 40);
            btnToggleMode.TabIndex = 1;
            btnToggleMode.Text = "Créer une nouvelle école";
            btnToggleMode.Click += btnToggleMode_Click;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(94, 148, 255);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(700, 40);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Configuration de l'École";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelSelection
            // 
            panelSelection.BackColor = Color.Transparent;
            panelSelection.BorderColor = Color.FromArgb(213, 218, 223);
            panelSelection.BorderRadius = 10;
            panelSelection.BorderThickness = 1;
            panelSelection.Controls.Add(btnCancel);
            panelSelection.Controls.Add(btnSelectSchool);
            panelSelection.Controls.Add(lblNoSchool);
            panelSelection.Controls.Add(lstEcoles);
            panelSelection.Controls.Add(lblEcoles);
            panelSelection.Controls.Add(lstAvenues);
            panelSelection.Controls.Add(txtAvenue);
            panelSelection.Controls.Add(lblAvenue);
            panelSelection.Controls.Add(lblSelectionTitle);
            panelSelection.Location = new Point(20, 120);
            panelSelection.Name = "panelSelection";
            panelSelection.Size = new Size(700, 515);
            panelSelection.TabIndex = 2;
            // 
            // btnCancel
            // 
            btnCancel.BorderRadius = 14;
            btnCancel.BorderThickness = 2;
            btnCancel.FillColor = Color.FromArgb(5, 21, 48);
            btnCancel.Font = new Font("Segoe UI", 10F);
            btnCancel.ForeColor = Color.White;
            btnCancel.HoverState.BorderColor = Color.FromArgb(5, 21, 48);
            btnCancel.HoverState.FillColor = Color.White;
            btnCancel.HoverState.Image = Properties.Resources.Ann_Dark;
            btnCancel.Image = Properties.Resources.Ann_light;
            btnCancel.ImageSize = new Size(32, 32);
            btnCancel.Location = new Point(374, 449);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(113, 50);
            btnCancel.TabIndex = 4;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSelectSchool
            // 
            btnSelectSchool.BorderRadius = 14;
            btnSelectSchool.BorderThickness = 2;
            btnSelectSchool.FillColor = Color.FromArgb(5, 21, 48);
            btnSelectSchool.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnSelectSchool.ForeColor = Color.White;
            btnSelectSchool.HoverState.BorderColor = Color.FromArgb(5, 21, 48);
            btnSelectSchool.HoverState.FillColor = Color.White;
            btnSelectSchool.HoverState.Image = Properties.Resources.save_dark;
            btnSelectSchool.Image = Properties.Resources.save_light;
            btnSelectSchool.ImageSize = new Size(32, 32);
            btnSelectSchool.Location = new Point(194, 449);
            btnSelectSchool.Name = "btnSelectSchool";
            btnSelectSchool.Size = new Size(113, 50);
            btnSelectSchool.TabIndex = 12;
            btnSelectSchool.Click += btnSelectSchool_Click;
            // 
            // lblNoSchool
            // 
            lblNoSchool.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
            lblNoSchool.ForeColor = Color.FromArgb(255, 140, 0);
            lblNoSchool.Location = new Point(20, 420);
            lblNoSchool.Name = "lblNoSchool";
            lblNoSchool.Size = new Size(660, 20);
            lblNoSchool.TabIndex = 11;
            lblNoSchool.Text = "Aucune école trouvée sur cette avenue. Cliquez sur \"Créer une nouvelle école\".";
            lblNoSchool.TextAlign = ContentAlignment.MiddleCenter;
            lblNoSchool.Visible = false;
            // 
            // lstEcoles
            // 
            lstEcoles.Columns.AddRange(new ColumnHeader[] { colEcole, colAvenue, colNumero });
            lstEcoles.FullRowSelect = true;
            lstEcoles.GridLines = true;
            lstEcoles.Location = new Point(20, 260);
            lstEcoles.MultiSelect = false;
            lstEcoles.Name = "lstEcoles";
            lstEcoles.Size = new Size(660, 150);
            lstEcoles.TabIndex = 10;
            lstEcoles.UseCompatibleStateImageBehavior = false;
            lstEcoles.View = View.Details;
            lstEcoles.SelectedIndexChanged += lstEcoles_SelectedIndexChanged;
            // 
            // colEcole
            // 
            colEcole.Text = "École";
            colEcole.Width = 350;
            // 
            // colAvenue
            // 
            colAvenue.Text = "Avenue";
            colAvenue.Width = 200;
            // 
            // colNumero
            // 
            colNumero.Text = "N°";
            colNumero.Width = 80;
            // 
            // lblEcoles
            // 
            lblEcoles.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblEcoles.Location = new Point(20, 235);
            lblEcoles.Name = "lblEcoles";
            lblEcoles.Size = new Size(200, 20);
            lblEcoles.TabIndex = 9;
            lblEcoles.Text = "Écoles disponibles:";
            // 
            // lstAvenues
            // 
            lstAvenues.Columns.AddRange(new ColumnHeader[] { colAvenueHierarchy });
            lstAvenues.FullRowSelect = true;
            lstAvenues.GridLines = true;
            lstAvenues.Location = new Point(20, 95);
            lstAvenues.MultiSelect = false;
            lstAvenues.Name = "lstAvenues";
            lstAvenues.Size = new Size(660, 130);
            lstAvenues.TabIndex = 3;
            lstAvenues.UseCompatibleStateImageBehavior = false;
            lstAvenues.View = View.Details;
            lstAvenues.SelectedIndexChanged += lstAvenues_SelectedIndexChanged;
            // 
            // colAvenueHierarchy
            // 
            colAvenueHierarchy.Text = "Avenue → Quartier → Commune → Ville → Province";
            colAvenueHierarchy.Width = 640;
            // 
            // txtAvenue
            // 
            txtAvenue.Font = new Font("Segoe UI", 10F);
            txtAvenue.Location = new Point(130, 53);
            txtAvenue.Name = "txtAvenue";
            txtAvenue.PlaceholderText = "Tapez le nom de l'avenue...";
            txtAvenue.Size = new Size(550, 30);
            txtAvenue.TabIndex = 2;
            txtAvenue.TextChanged += txtAvenue_TextChanged;
            // 
            // lblAvenue
            // 
            lblAvenue.Font = new Font("Segoe UI", 10F);
            lblAvenue.Location = new Point(20, 56);
            lblAvenue.Name = "lblAvenue";
            lblAvenue.Size = new Size(100, 20);
            lblAvenue.TabIndex = 1;
            lblAvenue.Text = "Avenue:";
            // 
            // lblSelectionTitle
            // 
            lblSelectionTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblSelectionTitle.ForeColor = Color.FromArgb(33, 37, 41);
            lblSelectionTitle.Location = new Point(20, 15);
            lblSelectionTitle.Name = "lblSelectionTitle";
            lblSelectionTitle.Size = new Size(660, 25);
            lblSelectionTitle.TabIndex = 0;
            lblSelectionTitle.Text = "Sélectionner une école existante";
            // 
            // panelCreation
            // 
            panelCreation.BackColor = Color.Transparent;
            panelCreation.BorderColor = Color.FromArgb(213, 218, 223);
            panelCreation.BorderRadius = 10;
            panelCreation.BorderThickness = 1;
            panelCreation.Controls.Add(siticoneButton1);
            panelCreation.Controls.Add(btnCreateSchool);
            panelCreation.Controls.Add(txtNumero);
            panelCreation.Controls.Add(lblNumero);
            panelCreation.Controls.Add(txtAnneeScol);
            panelCreation.Controls.Add(lblAnneeScol);
            panelCreation.Controls.Add(txtDenomination);
            panelCreation.Controls.Add(lblDenomination);
            panelCreation.Controls.Add(lstNewAvenues);
            panelCreation.Controls.Add(txtNewAvenue);
            panelCreation.Controls.Add(lblNewAvenue);
            panelCreation.Controls.Add(lblCreationTitle);
            panelCreation.Location = new Point(20, 120);
            panelCreation.Name = "panelCreation";
            panelCreation.Size = new Size(700, 502);
            panelCreation.TabIndex = 3;
            panelCreation.Visible = false;
            // 
            // siticoneButton1
            // 
            siticoneButton1.BorderColor = Color.FromArgb(5, 21, 48);
            siticoneButton1.BorderRadius = 14;
            siticoneButton1.BorderThickness = 1;
            siticoneButton1.FillColor = Color.FromArgb(5, 21, 48);
            siticoneButton1.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            siticoneButton1.ForeColor = Color.White;
            siticoneButton1.HoverState.BorderColor = Color.FromArgb(5, 21, 48);
            siticoneButton1.HoverState.FillColor = Color.White;
            siticoneButton1.HoverState.Image = Properties.Resources.Ann_Dark;
            siticoneButton1.Image = Properties.Resources.Ann_light;
            siticoneButton1.ImageSize = new Size(32, 32);
            siticoneButton1.Location = new Point(374, 435);
            siticoneButton1.Name = "siticoneButton1";
            siticoneButton1.Size = new Size(113, 50);
            siticoneButton1.TabIndex = 18;
            // 
            // btnCreateSchool
            // 
            btnCreateSchool.BorderColor = Color.FromArgb(5, 21, 48);
            btnCreateSchool.BorderRadius = 14;
            btnCreateSchool.BorderThickness = 1;
            btnCreateSchool.FillColor = Color.FromArgb(5, 21, 48);
            btnCreateSchool.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCreateSchool.ForeColor = Color.White;
            btnCreateSchool.HoverState.BorderColor = Color.FromArgb(5, 21, 48);
            btnCreateSchool.HoverState.FillColor = Color.White;
            btnCreateSchool.HoverState.Image = Properties.Resources.save_dark;
            btnCreateSchool.Image = Properties.Resources.save_light;
            btnCreateSchool.ImageSize = new Size(32, 32);
            btnCreateSchool.Location = new Point(194, 435);
            btnCreateSchool.Name = "btnCreateSchool";
            btnCreateSchool.Size = new Size(113, 50);
            btnCreateSchool.TabIndex = 17;
            btnCreateSchool.Click += btnCreateSchool_Click;
            // 
            // txtNumero
            // 
            txtNumero.BorderRadius = 8;
            txtNumero.Cursor = Cursors.IBeam;
            txtNumero.DefaultText = "";
            txtNumero.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtNumero.Font = new Font("Segoe UI", 10F);
            txtNumero.Location = new Point(130, 340);
            txtNumero.Margin = new Padding(3, 4, 3, 4);
            txtNumero.Name = "txtNumero";
            txtNumero.PasswordChar = '\0';
            txtNumero.PlaceholderText = "Ex: 123";
            txtNumero.SelectedText = "";
            txtNumero.Size = new Size(100, 30);
            txtNumero.TabIndex = 16;
            // 
            // lblNumero
            // 
            lblNumero.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblNumero.Location = new Point(20, 345);
            lblNumero.Name = "lblNumero";
            lblNumero.Size = new Size(100, 20);
            lblNumero.TabIndex = 15;
            lblNumero.Text = "N° Parcelle:";
            // 
            // txtAnneeScol
            // 
            txtAnneeScol.BorderRadius = 8;
            txtAnneeScol.Cursor = Cursors.IBeam;
            txtAnneeScol.DefaultText = "";
            txtAnneeScol.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtAnneeScol.Font = new Font("Segoe UI", 10F);
            txtAnneeScol.Location = new Point(130, 295);
            txtAnneeScol.Margin = new Padding(3, 4, 3, 4);
            txtAnneeScol.Name = "txtAnneeScol";
            txtAnneeScol.PasswordChar = '\0';
            txtAnneeScol.PlaceholderText = "Ex: 2024-2025";
            txtAnneeScol.SelectedText = "";
            txtAnneeScol.Size = new Size(200, 30);
            txtAnneeScol.TabIndex = 14;
            // 
            // lblAnneeScol
            // 
            lblAnneeScol.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblAnneeScol.Location = new Point(20, 300);
            lblAnneeScol.Name = "lblAnneeScol";
            lblAnneeScol.Size = new Size(100, 20);
            lblAnneeScol.TabIndex = 13;
            lblAnneeScol.Text = "Année Scol.:";
            // 
            // txtDenomination
            // 
            txtDenomination.BorderRadius = 8;
            txtDenomination.Cursor = Cursors.IBeam;
            txtDenomination.DefaultText = "";
            txtDenomination.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtDenomination.Font = new Font("Segoe UI", 10F);
            txtDenomination.Location = new Point(130, 250);
            txtDenomination.Margin = new Padding(3, 4, 3, 4);
            txtDenomination.Name = "txtDenomination";
            txtDenomination.PasswordChar = '\0';
            txtDenomination.PlaceholderText = "Ex: École Primaire Saint-Joseph";
            txtDenomination.SelectedText = "";
            txtDenomination.Size = new Size(400, 30);
            txtDenomination.TabIndex = 12;
            txtDenomination.TextChanged += txtDenomination_TextChanged;
            // 
            // lblDenomination
            // 
            lblDenomination.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDenomination.Location = new Point(20, 255);
            lblDenomination.Name = "lblDenomination";
            lblDenomination.Size = new Size(110, 20);
            lblDenomination.TabIndex = 11;
            lblDenomination.Text = "Dénomination:";
            // 
            // lstNewAvenues
            // 
            lstNewAvenues.Columns.AddRange(new ColumnHeader[] { colNewAvenueHierarchy });
            lstNewAvenues.FullRowSelect = true;
            lstNewAvenues.GridLines = true;
            lstNewAvenues.Location = new Point(20, 90);
            lstNewAvenues.MultiSelect = false;
            lstNewAvenues.Name = "lstNewAvenues";
            lstNewAvenues.Size = new Size(660, 140);
            lstNewAvenues.TabIndex = 3;
            lstNewAvenues.UseCompatibleStateImageBehavior = false;
            lstNewAvenues.View = View.Details;
            lstNewAvenues.SelectedIndexChanged += lstNewAvenues_SelectedIndexChanged;
            // 
            // colNewAvenueHierarchy
            // 
            colNewAvenueHierarchy.Text = "Avenue → Quartier → Commune → Ville → Province";
            colNewAvenueHierarchy.Width = 640;
            // 
            // txtNewAvenue
            // 
            txtNewAvenue.Font = new Font("Segoe UI", 10F);
            txtNewAvenue.Location = new Point(130, 47);
            txtNewAvenue.Name = "txtNewAvenue";
            txtNewAvenue.PlaceholderText = "Tapez le nom de l'avenue...";
            txtNewAvenue.Size = new Size(550, 30);
            txtNewAvenue.TabIndex = 2;
            txtNewAvenue.TextChanged += txtNewAvenue_TextChanged;
            // 
            // lblNewAvenue
            // 
            lblNewAvenue.Font = new Font("Segoe UI", 10F);
            lblNewAvenue.Location = new Point(20, 50);
            lblNewAvenue.Name = "lblNewAvenue";
            lblNewAvenue.Size = new Size(100, 20);
            lblNewAvenue.TabIndex = 1;
            lblNewAvenue.Text = "Avenue:";
            // 
            // lblCreationTitle
            // 
            lblCreationTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblCreationTitle.ForeColor = Color.FromArgb(33, 37, 41);
            lblCreationTitle.Location = new Point(20, 15);
            lblCreationTitle.Name = "lblCreationTitle";
            lblCreationTitle.Size = new Size(660, 25);
            lblCreationTitle.TabIndex = 0;
            lblCreationTitle.Text = "Créer une nouvelle école";
            // 
            // FormConfig
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 242, 245);
            ClientSize = new Size(800, 730);
            Controls.Add(panelMain);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormConfig";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Configuration École - EduKin";
            Load += FormConfig_Load;
            panelMain.ResumeLayout(false);
            panelSelection.ResumeLayout(false);
            panelSelection.PerformLayout();
            panelCreation.ResumeLayout(false);
            panelCreation.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Siticone.Desktop.UI.WinForms.SiticonePanel panelMain;
        private System.Windows.Forms.Label lblTitle;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnToggleMode;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelSelection;
        private System.Windows.Forms.Label lblSelectionTitle;
        private System.Windows.Forms.Label lblAvenue;
        private System.Windows.Forms.TextBox txtAvenue;
        private System.Windows.Forms.ListView lstAvenues;
        private System.Windows.Forms.ColumnHeader colAvenueHierarchy;
        private System.Windows.Forms.Label lblEcoles;
        private System.Windows.Forms.ListView lstEcoles;
        private System.Windows.Forms.ColumnHeader colEcole;
        private System.Windows.Forms.ColumnHeader colAvenue;
        private System.Windows.Forms.ColumnHeader colNumero;
        private System.Windows.Forms.Label lblNoSchool;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnSelectSchool;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnCancel;
        private Siticone.Desktop.UI.WinForms.SiticonePanel panelCreation;
        private System.Windows.Forms.Label lblCreationTitle;
        private System.Windows.Forms.Label lblNewAvenue;
        private System.Windows.Forms.TextBox txtNewAvenue;
        private System.Windows.Forms.ListView lstNewAvenues;
        private System.Windows.Forms.ColumnHeader colNewAvenueHierarchy;
        private System.Windows.Forms.Label lblDenomination;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txtDenomination;
        private System.Windows.Forms.Label lblAnneeScol;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txtAnneeScol;
        private System.Windows.Forms.Label lblNumero;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txtNumero;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btnCreateSchool;
        private Siticone.Desktop.UI.WinForms.SiticoneButton siticoneButton1;
    }
}
