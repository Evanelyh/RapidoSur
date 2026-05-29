using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace RapidoSurWinForms
{
    public class MainForm : Form
    {
        private readonly Form _loginForm;
        private readonly DbService _db = new DbService();

        
        private Panel sidebarPanel;
        private Panel headerPanel;
        private Panel contentPanel;
        
        
        private Label logoLabel;
        private Button btnDashboard;
        private Button btnNewOrder;
        private Button btnAssignVehicle;
        private Button btnReports;
        private Button btnTrackingSim;
        private Button btnLogout;
        private Panel userPanel;
        private Label lblUserAvatar;
        private Label lblUserName;
        private Label lblUserRole;

        
        private Panel cardPedidos;
        private Label lblPedidosCount;
        private Label lblPedidosTitle;

        private Panel cardVehiculos;
        private Label lblVehiculosCount;
        private Label lblVehiculosTitle;

        private Panel cardEntregas;
        private Label lblEntregasCount;
        private Label lblEntregasTitle;

        private Panel cardAlertas;
        private Label lblAlertasCount;
        private Label lblAlertasTitle;

        
        private Label gridTitle;
        private DataGridView dgvRecentOrders;

        public MainForm(Form loginForm)
        {
            _loginForm = loginForm;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Rápido Sur S.R.L. - Sistema de Gestión Logística (Administrador / Operador)";
            this.Size = new Size(1100, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(11, 15, 12); 
            this.FormClosing += (s, e) => Application.Exit();

            
            
            
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 240,
                BackColor = Color.FromArgb(15, 22, 17) 
            };
            this.Controls.Add(sidebarPanel);

            logoLabel = new Label
            {
                Text = "⚡ Rápido Sur",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(57, 211, 83), 
                Location = new Point(0, 0),
                Size = new Size(240, 70),
                TextAlign = ContentAlignment.MiddleCenter
            };
            sidebarPanel.Controls.Add(logoLabel);

            
            btnDashboard = CreateSidebarButton("Panel Principal", 80, (s, e) => RefreshDashboardData());
            btnNewOrder = CreateSidebarButton("Registrar Pedido", 130, btnNewOrder_Click);
            btnAssignVehicle = CreateSidebarButton("Despachar Flota", 180, btnAssignVehicle_Click);
            btnTrackingSim = CreateSidebarButton("Simulador Cliente", 230, btnTrackingSim_Click);
            
            btnReports = CreateSidebarButton("Reportes ADS", 280, btnReports_Click);
            
            if (Session.UserRole != "Administrador")
            {
                btnReports.Visible = false;
            }

            btnLogout = CreateSidebarButton("Cerrar Sesión", 480, btnLogout_Click);
            btnLogout.BackColor = Color.FromArgb(239, 68, 68);
            btnLogout.ForeColor = Color.White;
            btnLogout.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            
            userPanel = new Panel
            {
                Location = new Point(15, 550),
                Size = new Size(210, 70),
                BackColor = Color.FromArgb(25, 35, 28), 
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            sidebarPanel.Controls.Add(userPanel);

            lblUserAvatar = new Label
            {
                Text = Session.UserName.Substring(0, 1).ToUpper(),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Black, 
                BackColor = Color.FromArgb(57, 211, 83), 
                Location = new Point(10, 12),
                Size = new Size(45, 45),
                TextAlign = ContentAlignment.MiddleCenter
            };
            userPanel.Controls.Add(lblUserAvatar);

            lblUserName = new Label
            {
                Text = Session.UserName,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(60, 13),
                Size = new Size(140, 20)
            };
            userPanel.Controls.Add(lblUserName);

            lblUserRole = new Label
            {
                Text = Session.UserRole,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(150, 170, 155), 
                Location = new Point(60, 35),
                Size = new Size(140, 20)
            };
            userPanel.Controls.Add(lblUserRole);

            
            
            
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(20, 28, 22) 
            };
            this.Controls.Add(headerPanel);

            Label titleLabel = new Label
            {
                Text = "DASHBOARD OPERATIVO DE LOGÍSTICA",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 0),
                Size = new Size(500, 70),
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Controls.Add(titleLabel);

            
            
            
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(11, 15, 12), 
                Padding = new Padding(20)
            };
            this.Controls.Add(contentPanel);

            
            TableLayoutPanel statsTable = new TableLayoutPanel
            {
                Location = new Point(20, 15),
                Size = new Size(820, 100),
                ColumnCount = 4,
                RowCount = 1,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent
            };
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            contentPanel.Controls.Add(statsTable);

            cardPedidos = CreateStatCard("PEDIDOS RECIBIDOS", "0", Color.FromArgb(57, 211, 83)); 
            cardVehiculos = CreateStatCard("CAMIONES EN RUTA", "0", Color.FromArgb(162, 230, 75)); 
            cardEntregas = CreateStatCard("ENTREGAS HECHAS", "0", Color.FromArgb(210, 220, 50)); 
            cardAlertas = CreateStatCard("INCIDENCIAS ACT.", "0", Color.FromArgb(239, 68, 68)); 

            statsTable.Controls.Add(cardPedidos, 0, 0);
            statsTable.Controls.Add(cardVehiculos, 1, 0);
            statsTable.Controls.Add(cardEntregas, 2, 0);
            statsTable.Controls.Add(cardAlertas, 3, 0);

            
            gridTitle = new Label
            {
                Text = "ÚLTIMOS PEDIDOS REGISTRADOS EN EL SISTEMA",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 135),
                Size = new Size(820, 25),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            contentPanel.Controls.Add(gridTitle);

            dgvRecentOrders = new DataGridView
            {
                Location = new Point(20, 165),
                Size = new Size(820, 440),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.FromArgb(20, 28, 22), 
                ForeColor = Color.White,
                GridColor = Color.FromArgb(30, 42, 33),
                BorderStyle = BorderStyle.None,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvRecentOrders.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 22, 17);
            dgvRecentOrders.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRecentOrders.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvRecentOrders.DefaultCellStyle.BackColor = Color.FromArgb(20, 28, 22);
            dgvRecentOrders.DefaultCellStyle.ForeColor = Color.White;
            dgvRecentOrders.DefaultCellStyle.SelectionBackColor = Color.FromArgb(57, 211, 83); 
            dgvRecentOrders.DefaultCellStyle.SelectionForeColor = Color.Black; 
            contentPanel.Controls.Add(dgvRecentOrders);

            
            dgvRecentOrders.Columns.Add("IdPedido", "ID Pedido");
            dgvRecentOrders.Columns.Add("ClienteNombre", "Cliente");
            dgvRecentOrders.Columns.Add("FechaSolicitud", "Fecha Solicitud");
            dgvRecentOrders.Columns.Add("DireccionEntrega", "Dirección");
            dgvRecentOrders.Columns.Add("PesoKg", "Peso (kg)");
            dgvRecentOrders.Columns.Add("Prioridad", "Prioridad");
            dgvRecentOrders.Columns.Add("Estado", "Estado");

            
            dgvRecentOrders.Columns["IdPedido"].Width = 70;
            dgvRecentOrders.Columns["PesoKg"].Width = 80;
            dgvRecentOrders.Columns["Prioridad"].Width = 80;
            dgvRecentOrders.Columns["Estado"].Width = 90;

            
            RefreshDashboardData();

            
            
            
            
            sidebarPanel.SendToBack();
            headerPanel.SendToBack();
            contentPanel.BringToFront();
        }

        private Button CreateSidebarButton(string text, int top, EventHandler onClick)
        {
            var btn = new Button
            {
                Text = "   " + text,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Location = new Point(15, top),
                Size = new Size(210, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(25, 35, 28), 
                ForeColor = Color.FromArgb(200, 220, 205),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += onClick;
            sidebarPanel.Controls.Add(btn);
            return btn;
        }

        private Panel CreateStatCard(string title, string count, Color accentColor)
        {
            Panel card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(20, 28, 22), 
                Padding = new Padding(0)
            };

            
            Panel decBar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 5,
                BackColor = accentColor
            };
            card.Controls.Add(decBar);

            Label titleLbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(140, 150, 170),
                Location = new Point(12, 12),
                Size = new Size(160, 18),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            card.Controls.Add(titleLbl);

            Label countLbl = new Label
            {
                Text = count,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 30),
                Size = new Size(160, 55),
                TextAlign = ContentAlignment.MiddleLeft,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            card.Controls.Add(countLbl);

            
            if (title.Contains("PEDIDOS")) lblPedidosCount = countLbl;
            else if (title.Contains("CAMIONES")) lblVehiculosCount = countLbl;
            else if (title.Contains("ENTREGAS")) lblEntregasCount = countLbl;
            else if (title.Contains("INCIDENCIAS")) lblAlertasCount = countLbl;

            return card;
        }

        public void RefreshDashboardData()
        {
            try
            {
                var pedidos = _db.GetPedidos();
                var vehiculos = _db.GetVehiculos();
                var envios = _db.GetEnvios();

                lblPedidosCount.Text = pedidos.Count.ToString();
                lblVehiculosCount.Text = vehiculos.FindAll(v => v.EstadoOperativo == "En Ruta").Count.ToString();
                lblEntregasCount.Text = pedidos.FindAll(p => p.Estado == "Entregado").Count.ToString();
                lblAlertasCount.Text = envios.FindAll(e => e.EstadoEnvio == "Con Incidencia" || e.EstadoEnvio == "Reprogramado").Count.ToString();

                
                dgvRecentOrders.Rows.Clear();
                int limit = Math.Min(pedidos.Count, 15);
                for (int i = 0; i < limit; i++)
                {
                    var p = pedidos[i];
                    dgvRecentOrders.Rows.Add(
                        p.IdPedido,
                        p.ClienteNombre,
                        p.FechaSolicitud.ToString("dd/MM/yyyy hh:mm tt"),
                        p.DireccionEntrega,
                        p.PesoKg.ToString("F2") + " kg",
                        p.Prioridad,
                        p.Estado
                    );

                    
                    var row = dgvRecentOrders.Rows[i];
                    var stateCell = row.Cells["Estado"];
                    if (p.Estado == "Pendiente") stateCell.Style.ForeColor = Color.FromArgb(245, 158, 11); 
                    else if (p.Estado == "En Ruta" || p.Estado == "Asignado") stateCell.Style.ForeColor = Color.FromArgb(57, 211, 83); 
                    else if (p.Estado == "Entregado") stateCell.Style.ForeColor = Color.FromArgb(16, 185, 129); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos del Dashboard desde SQL Server: {ex.Message}", "Error de Consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNewOrder_Click(object sender, EventArgs e)
        {
            CreatePedidoForm createForm = new CreatePedidoForm(this);
            createForm.ShowDialog();
        }

        private void btnAssignVehicle_Click(object sender, EventArgs e)
        {
            AssignDespatchForm assignForm = new AssignDespatchForm(this);
            assignForm.ShowDialog();
        }

        private void btnTrackingSim_Click(object sender, EventArgs e)
        {
            TrackingForm trackingForm = new TrackingForm();
            trackingForm.ShowDialog();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            ReportsForm reportsForm = new ReportsForm();
            reportsForm.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            this.Hide();
            _loginForm.Show();
        }
    }
}
