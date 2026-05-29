using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace RapidoSurWinForms
{
    public class DriverForm : Form
    {
        private readonly Form _loginForm;
        private readonly DbService _db = new DbService();
        private List<Envio> activeEnvios = new();

        
        private Panel headerPanel;
        private Label lblDriverWelcome;
        private Label lblDriverSub;

        private Label lblActiveShipments;
        private DataGridView dgvActiveShipments;

        
        private GroupBox gbActions;
        private Label lblEstado;
        private ComboBox cbEstados;
        private Button btnUpdateStatus;

        private GroupBox gbIncidents;
        private Label lblIncidentType;
        private ComboBox cbIncidentTypes;
        private Label lblIncidentDesc;
        private TextBox txtIncidentDesc;
        private Button btnReportIncident;

        private GroupBox gbReprogram;
        private Label lblNuevaFecha;
        private DateTimePicker dtpNuevaFecha;
        private Label lblMotivo;
        private TextBox txtMotivo;
        private Button btnReprogram;

        private Button btnLogout;

        public DriverForm(Form loginForm)
        {
            _loginForm = loginForm;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Terminal Logística del Conductor - Rápido Sur S.R.L.";
            this.Size = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(11, 15, 12); 
            this.FormClosing += (s, e) => Application.Exit();

            
            
            
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(20, 28, 22) 
            };
            this.Controls.Add(headerPanel);

            lblDriverWelcome = new Label
            {
                Text = $"CHOFER: {Session.UserName}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(57, 211, 83), 
                Location = new Point(20, 10),
                Size = new Size(500, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Controls.Add(lblDriverWelcome);

            lblDriverSub = new Label
            {
                Text = $"Licencia: {Session.UserLicence} | Terminal de Campo Móvil",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(150, 170, 155), 
                Location = new Point(20, 38),
                Size = new Size(500, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Controls.Add(lblDriverSub);

            btnLogout = new Button
            {
                Text = "CERRAR SESIÓN",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(780, 15),
                Size = new Size(130, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += btnLogout_Click;
            headerPanel.Controls.Add(btnLogout);

            
            
            
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Location = new Point(20, 85),
                Size = new Size(910, 465),
                ColumnCount = 2,
                RowCount = 1,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));   
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 410f));   
            this.Controls.Add(mainLayout);

            
            
            
            Panel gridContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 15, 0)
            };
            mainLayout.Controls.Add(gridContainer, 0, 0);

            lblActiveShipments = new Label
            {
                Text = "MIS DESPACHOS Y ENVÍOS ACTIVOS EN RUTA:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 25
            };
            gridContainer.Controls.Add(lblActiveShipments);

            dgvActiveShipments = new DataGridView
            {
                Dock = DockStyle.Fill,
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
            dgvActiveShipments.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 22, 17);
            dgvActiveShipments.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvActiveShipments.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvActiveShipments.DefaultCellStyle.BackColor = Color.FromArgb(20, 28, 22);
            dgvActiveShipments.DefaultCellStyle.ForeColor = Color.White;
            dgvActiveShipments.DefaultCellStyle.SelectionBackColor = Color.FromArgb(57, 211, 83); 
            dgvActiveShipments.DefaultCellStyle.SelectionForeColor = Color.Black; 
            dgvActiveShipments.SelectionChanged += dgvActiveShipments_SelectionChanged;
            gridContainer.Controls.Add(dgvActiveShipments);

            
            dgvActiveShipments.BringToFront();

            
            dgvActiveShipments.Columns.Add("IdEnvio", "ID Envío");
            dgvActiveShipments.Columns.Add("Cliente", "Cliente");
            dgvActiveShipments.Columns.Add("Direccion", "Dirección");
            dgvActiveShipments.Columns.Add("Peso", "Peso (kg)");
            dgvActiveShipments.Columns.Add("Estado", "Estado");
            dgvActiveShipments.Columns["IdEnvio"].Width = 60;
            dgvActiveShipments.Columns["Peso"].Width = 70;
            dgvActiveShipments.Columns["Estado"].Width = 80;

            
            
            
            Panel actionsContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                AutoScroll = true
            };
            mainLayout.Controls.Add(actionsContainer, 1, 0);

            
            gbActions = new GroupBox 
            { 
                Text = "1. Actualizar Estado de Ruta", 
                ForeColor = Color.FromArgb(57, 211, 83), 
                Font = new Font("Segoe UI", 9, FontStyle.Bold), 
                Location = new Point(15, 0), 
                Size = new Size(380, 90) 
            };
            actionsContainer.Controls.Add(gbActions);

            lblEstado = new Label { Text = "Nuevo Estado:", ForeColor = Color.White, Location = new Point(15, 20), Size = new Size(100, 20) };
            cbEstados = new ComboBox { Location = new Point(15, 45), Size = new Size(180, 25), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(15, 22, 17), ForeColor = Color.White };
            cbEstados.Items.AddRange(new string[] { "En Ruta", "Entregado" });
            cbEstados.SelectedIndex = 0;
            btnUpdateStatus = new Button { Text = "ACTUALIZAR", Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(215, 43), Size = new Size(140, 28), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(57, 211, 83), ForeColor = Color.Black, Cursor = Cursors.Hand };
            btnUpdateStatus.FlatAppearance.BorderSize = 0;
            btnUpdateStatus.Click += btnUpdateStatus_Click;
            gbActions.Controls.AddRange(new Control[] { lblEstado, cbEstados, btnUpdateStatus });

            
            gbIncidents = new GroupBox 
            { 
                Text = "2. Reportar Incidencia / Retraso", 
                ForeColor = Color.FromArgb(239, 68, 68), 
                Font = new Font("Segoe UI", 9, FontStyle.Bold), 
                Location = new Point(15, 100), 
                Size = new Size(380, 180) 
            };
            actionsContainer.Controls.Add(gbIncidents);

            lblIncidentType = new Label { Text = "Tipo de Novedad:", ForeColor = Color.White, Location = new Point(15, 20), Size = new Size(150, 18) };
            cbIncidentTypes = new ComboBox { Location = new Point(15, 40), Size = new Size(340, 25), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(15, 22, 17), ForeColor = Color.White };
            cbIncidentTypes.Items.AddRange(new string[] { "Tráfico Pesado", "Falla Mecánica del Camión", "Fuerte Lluvia / Mal Clima", "Dirección No Encontrada", "Accidente en Carretera" });
            cbIncidentTypes.SelectedIndex = 0;
            lblIncidentDesc = new Label { Text = "Detalles o Razón:", ForeColor = Color.White, Location = new Point(15, 75), Size = new Size(150, 18) };
            txtIncidentDesc = new TextBox { Location = new Point(15, 95), Size = new Size(340, 23), BackColor = Color.FromArgb(15, 22, 17), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            btnReportIncident = new Button { Text = "REPORTAR INCIDENCIA", Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(15, 135), Size = new Size(340, 32), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(239, 68, 68), ForeColor = Color.White, Cursor = Cursors.Hand };
            btnReportIncident.FlatAppearance.BorderSize = 0;
            btnReportIncident.Click += btnReportIncident_Click;
            gbIncidents.Controls.AddRange(new Control[] { lblIncidentType, cbIncidentTypes, lblIncidentDesc, txtIncidentDesc, btnReportIncident });

            
            gbReprogram = new GroupBox 
            { 
                Text = "3. Reprogramar Fecha de Entrega", 
                ForeColor = Color.FromArgb(245, 158, 11), 
                Font = new Font("Segoe UI", 9, FontStyle.Bold), 
                Location = new Point(15, 290), 
                Size = new Size(380, 155) 
            };
            actionsContainer.Controls.Add(gbReprogram);

            lblNuevaFecha = new Label { Text = "Nueva Fecha Propuesta:", ForeColor = Color.White, Location = new Point(15, 20), Size = new Size(150, 18) };
            dtpNuevaFecha = new DateTimePicker { Location = new Point(15, 40), Size = new Size(150, 25), Format = DateTimePickerFormat.Short, BackColor = Color.FromArgb(15, 22, 17), ForeColor = Color.White, Value = DateTime.Now.AddDays(1) };
            lblMotivo = new Label { Text = "Motivo de Aplazamiento:", ForeColor = Color.White, Location = new Point(180, 20), Size = new Size(150, 18) };
            txtMotivo = new TextBox { Location = new Point(180, 40), Size = new Size(180, 23), BackColor = Color.FromArgb(15, 22, 17), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            btnReprogram = new Button { Text = "REPROGRAMAR ENTREGA", Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(15, 105), Size = new Size(340, 32), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(245, 158, 11), ForeColor = Color.White, Cursor = Cursors.Hand };
            btnReprogram.FlatAppearance.BorderSize = 0;
            btnReprogram.Click += btnReprogram_Click;
            gbReprogram.Controls.AddRange(new Control[] { lblNuevaFecha, dtpNuevaFecha, lblMotivo, txtMotivo, btnReprogram });

            
            lblEstado.ForeColor = Color.White;
            lblIncidentType.ForeColor = Color.White;
            lblIncidentDesc.ForeColor = Color.White;
            lblNuevaFecha.ForeColor = Color.White;
            lblMotivo.ForeColor = Color.White;

            
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            try
            {
                var list = _db.GetEnvios(null, Session.UserId);
                
                activeEnvios = list.FindAll(e => e.EstadoEnvio != "Entregado");

                dgvActiveShipments.Rows.Clear();
                foreach (var e in activeEnvios)
                {
                    dgvActiveShipments.Rows.Add(
                        e.IdEnvio,
                        e.ClienteNombre,
                        e.DireccionEntrega,
                        e.PesoKg.ToString("F2") + " kg",
                        e.EstadoEnvio
                    );
                }

                
                UpdateControlStates();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar tus envíos desde SQL Server: {ex.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateControlStates()
        {
            bool hasSelection = dgvActiveShipments.SelectedRows.Count > 0;
            gbActions.Enabled = hasSelection;
            gbIncidents.Enabled = hasSelection;
            gbReprogram.Enabled = hasSelection;
        }

        private void dgvActiveShipments_SelectionChanged(object sender, EventArgs e)
        {
            UpdateControlStates();
        }

        private void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            if (dgvActiveShipments.SelectedRows.Count == 0) return;
            int rowIndex = dgvActiveShipments.SelectedRows[0].Index;
            var envio = activeEnvios[rowIndex];

            string estado = cbEstados.SelectedItem?.ToString() ?? "En Ruta";
            string desc = $"El conductor {Session.UserName} actualizó el estado del despacho a: {estado}.";

            try
            {
                bool success = _db.UpdateEnvioEstado(envio.IdEnvio, estado, desc, Session.UserName);
                if (success)
                {
                    MessageBox.Show($"¡Estado actualizado con éxito a '{estado}' en la base de datos!", "Estado Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshGrid();
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar el estado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a SQL Server: {ex.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReportIncident_Click(object sender, EventArgs e)
        {
            if (dgvActiveShipments.SelectedRows.Count == 0) return;
            int rowIndex = dgvActiveShipments.SelectedRows[0].Index;
            var envio = activeEnvios[rowIndex];

            string tipo = cbIncidentTypes.SelectedItem?.ToString() ?? "Tráfico Pesado";
            string motivo = txtIncidentDesc.Text.Trim();

            if (string.IsNullOrEmpty(motivo))
            {
                MessageBox.Show("Por favor proporcione una descripción o justificación corta del incidente.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string desc = $"INCIDENCIA REPORTADA POR CHOFER ({tipo}): {motivo}";

            try
            {
                bool success = _db.UpdateEnvioEstado(envio.IdEnvio, "Con Incidencia", desc, Session.UserName);
                if (success)
                {
                    MessageBox.Show($"¡Incidencia registrada correctamente!\n\nEl estado del envío cambió a 'Con Incidencia'. El operador y el cliente final visualizarán la novedad al instante.", "Incidencia Guardada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtIncidentDesc.Clear();
                    RefreshGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a SQL Server: {ex.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReprogram_Click(object sender, EventArgs e)
        {
            if (dgvActiveShipments.SelectedRows.Count == 0) return;
            int rowIndex = dgvActiveShipments.SelectedRows[0].Index;
            var envio = activeEnvios[rowIndex];

            DateTime fecha = dtpNuevaFecha.Value.Date;
            string motivo = txtMotivo.Text.Trim();

            if (string.IsNullOrEmpty(motivo))
            {
                MessageBox.Show("Por favor escriba el motivo de la reprogramación.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (fecha.Date <= DateTime.Now.Date)
            {
                MessageBox.Show("La nueva fecha estimada de entrega debe ser posterior al día de hoy.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string desc = $"ENTREGA APLAZADA / REPROGRAMADA. Nueva fecha: {fecha.ToString("dd/MM/yyyy")}. Motivo: {motivo}";

            try
            {
                bool success = _db.UpdateEnvioEstado(envio.IdEnvio, "Reprogramado", desc, Session.UserName, fecha);
                if (success)
                {
                    MessageBox.Show($"Despacho reprogramado correctamente para el {fecha.ToString("dd/MM/yyyy")}.", "Entrega Aplazada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtMotivo.Clear();
                    RefreshGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a SQL Server: {ex.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            this.Hide();
            _loginForm.Show();
        }
    }
}
