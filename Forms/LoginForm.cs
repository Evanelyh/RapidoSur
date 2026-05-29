using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace RapidoSurWinForms
{
    public class LoginForm : Form
    {
        private readonly DbService _db = new DbService();
        private string _userType = "Operador"; // Default

        // Controls
        private Panel cardPanel;
        private Label titleLabel;
        private Label subTitleLabel;
        private Button btnTabOperador;
        private Button btnTabConductor;
        
        // Operator Inputs
        private Panel panelOperador;
        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblClave;
        private TextBox txtClave;
        
        // Conductor Inputs
        private Panel panelConductor;
        private Label lblConductor;
        private ComboBox cbConductor;

        private Button btnLogin;
        private Label lblDemoInfo;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Rápido Sur S.R.L. - Acceso Personal";
            this.Size = new Size(480, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(11, 15, 12); // Deep Dark forest background

            // Glassmorphism Card emulation panel
            cardPanel = new Panel
            {
                Size = new Size(410, 440),
                Location = new Point(25, 25),
                BackColor = Color.FromArgb(20, 28, 22), // Card background (dark forest gray)
                BorderStyle = BorderStyle.None
            };
            this.Controls.Add(cardPanel);

            // Title Icon / Label
            titleLabel = new Label
            {
                Text = "ACCESO AL SISTEMA",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(57, 211, 83), // Primary Apple Green
                Location = new Point(20, 20),
                Size = new Size(370, 35),
                TextAlign = ContentAlignment.MiddleCenter
            };
            cardPanel.Controls.Add(titleLabel);

            subTitleLabel = new Label
            {
                Text = "Rápido Sur S.R.L. - Control Logístico",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(140, 160, 145), // Forest Gray Text
                Location = new Point(20, 55),
                Size = new Size(370, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            cardPanel.Controls.Add(subTitleLabel);

            // Tab Selector Buttons
            btnTabOperador = new Button
            {
                Text = "Operador / Admin",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(20, 95),
                Size = new Size(180, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(57, 211, 83),
                ForeColor = Color.Black, // Dark text on bright background
                Cursor = Cursors.Hand
            };
            btnTabOperador.FlatAppearance.BorderSize = 0;
            btnTabOperador.Click += (s, e) => SwitchTab("Operador");
            cardPanel.Controls.Add(btnTabOperador);

            btnTabConductor = new Button
            {
                Text = "Conductor",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(210, 95),
                Size = new Size(180, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 42, 33), // Dark inactive forest tab
                ForeColor = Color.FromArgb(170, 190, 175),
                Cursor = Cursors.Hand
            };
            btnTabConductor.FlatAppearance.BorderSize = 0;
            btnTabConductor.Click += (s, e) => SwitchTab("Conductor");
            cardPanel.Controls.Add(btnTabConductor);

            // ==========================================
            // PANEL OPERADOR
            // ==========================================
            panelOperador = new Panel
            {
                Location = new Point(20, 150),
                Size = new Size(370, 160),
                BackColor = Color.Transparent
            };
            cardPanel.Controls.Add(panelOperador);

            lblUsuario = new Label
            {
                Text = "Nombre de Usuario:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 10),
                Size = new Size(370, 20)
            };
            txtUsuario = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(0, 32),
                Size = new Size(370, 28),
                BackColor = Color.FromArgb(15, 22, 17), // Moss black background
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Text = "lcontreras" // Preloaded for demo ease
            };
            panelOperador.Controls.Add(lblUsuario);
            panelOperador.Controls.Add(txtUsuario);

            lblClave = new Label
            {
                Text = "Contraseña:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 75),
                Size = new Size(370, 20)
            };
            txtClave = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(0, 97),
                Size = new Size(370, 28),
                BackColor = Color.FromArgb(15, 22, 17),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '•',
                Text = "lcontreras123" // Preloaded for demo ease
            };
            panelOperador.Controls.Add(lblClave);
            panelOperador.Controls.Add(txtClave);

            // ==========================================
            // PANEL CONDUCTOR
            // ==========================================
            panelConductor = new Panel
            {
                Location = new Point(20, 150),
                Size = new Size(370, 160),
                BackColor = Color.Transparent,
                Visible = false
            };
            cardPanel.Controls.Add(panelConductor);

            lblConductor = new Label
            {
                Text = "Seleccione su Nombre:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 25),
                Size = new Size(370, 20)
            };
            cbConductor = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(0, 50),
                Size = new Size(370, 28),
                BackColor = Color.FromArgb(15, 22, 17),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            panelConductor.Controls.Add(lblConductor);
            panelConductor.Controls.Add(cbConductor);

            // ==========================================
            // LOGIN BUTTON
            // ==========================================
            btnLogin = new Button
            {
                Text = "INGRESAR AL SISTEMA",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(20, 320),
                Size = new Size(370, 46),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(57, 211, 83), // Apple Green button
                ForeColor = Color.Black, // Black text on green background
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += btnLogin_Click;
            cardPanel.Controls.Add(btnLogin);

            // DEMO INFO FOOTER
            lblDemoInfo = new Label
            {
                Text = "Demo Admin: usuario: lcontreras | clave: lcontreras123",
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(130, 140, 160),
                Location = new Point(20, 385),
                Size = new Size(370, 35),
                TextAlign = ContentAlignment.MiddleCenter
            };
            cardPanel.Controls.Add(lblDemoInfo);

            // Load conductors dropdown list
            LoadConductors();
        }

        private void LoadConductors()
        {
            try
            {
                var list = _db.GetConductores();
                cbConductor.DisplayMember = "NombreCompleto";
                cbConductor.ValueMember = "IdConductor";
                cbConductor.DataSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar conductores de SQL Server: {ex.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SwitchTab(string type)
        {
            _userType = type;
            if (type == "Operador")
            {
                btnTabOperador.BackColor = Color.FromArgb(57, 211, 83);
                btnTabOperador.ForeColor = Color.Black;
                btnTabConductor.BackColor = Color.FromArgb(30, 42, 33);
                btnTabConductor.ForeColor = Color.FromArgb(170, 190, 175);

                panelOperador.Visible = true;
                panelConductor.Visible = false;
                lblDemoInfo.Text = "Demo Admin: usuario: lcontreras | clave: lcontreras123";
            }
            else
            {
                btnTabOperador.BackColor = Color.FromArgb(30, 42, 33);
                btnTabOperador.ForeColor = Color.FromArgb(170, 190, 175);
                btnTabConductor.BackColor = Color.FromArgb(57, 211, 83);
                btnTabConductor.ForeColor = Color.Black;

                panelOperador.Visible = false;
                panelConductor.Visible = true;
                lblDemoInfo.Text = "Seleccione un chofer de la lista y presione INGRESAR.";
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (_userType == "Operador")
            {
                string usuario = txtUsuario.Text.Trim();
                string clave = txtClave.Text;

                if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(clave))
                {
                    MessageBox.Show("Por favor ingrese su usuario y contraseña.", "Campos Requeridos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var op = _db.Login(usuario, clave);
                if (op != null)
                {
                    // Set Session
                    Session.IsLoggedIn = true;
                    Session.UserType = "Operador";
                    Session.UserId = op.IdOperador;
                    Session.UserName = op.NombreCompleto;
                    Session.UserRole = op.Rol;

                    // Open MainForm
                    this.Hide();
                    MainForm main = new MainForm(this);
                    main.Show();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Error de Acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else // Conductor
            {
                if (cbConductor.SelectedValue == null)
                {
                    MessageBox.Show("Por favor seleccione un conductor.", "Campos Requeridos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var cond = cbConductor.SelectedItem as Conductor;
                if (cond != null)
                {
                    // Set Session
                    Session.IsLoggedIn = true;
                    Session.UserType = "Conductor";
                    Session.UserId = cond.IdConductor;
                    Session.UserName = $"{cond.Nombre} {cond.Apellido}";
                    Session.UserRole = "Conductor";
                    Session.UserLicence = cond.NumLicencia;

                    // Open DriverForm
                    this.Hide();
                    DriverForm driver = new DriverForm(this);
                    driver.Show();
                }
            }
        }
    }
}
