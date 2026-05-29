using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace RapidoSurWinForms
{
    public class AssignDespatchForm : Form
    {
        private readonly MainForm _parentForm;
        private readonly DbService _db = new DbService();

        
        private List<Pedido> pendingOrders = new();
        private List<Vehiculo> availableVehicles = new();
        private List<Conductor> availableDrivers = new();

        
        private Label lblPedidos;
        private ListBox lbPedidos;

        private GroupBox gbAsignacion;
        private Label lblVehiculo;
        private ComboBox cbVehiculos;
        private Label lblConductor;
        private ComboBox cbConductores;
        private Label lblFechaEstimada;
        private DateTimePicker dtpFechaEstimada;
        private Label lblRuta;
        private TextBox txtRuta;

        private Button btnDespachar;
        private Button btnCancelar;

        public AssignDespatchForm(MainForm parentForm)
        {
            _parentForm = parentForm;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Módulo de Asignación y Programación de Despacho (Doble Columna)";
            this.Size = new Size(820, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(10, 15, 30);

            
            
            
            lblPedidos = new Label
            {
                Text = "PEDIDOS PENDIENTES DE DESPACHO:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(25, 20),
                Size = new Size(350, 20)
            };
            this.Controls.Add(lblPedidos);

            lbPedidos = new ListBox
            {
                Location = new Point(25, 45),
                Size = new Size(360, 310),
                BackColor = Color.FromArgb(20, 30, 55),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.None
            };
            lbPedidos.SelectedIndexChanged += lbPedidos_SelectedIndexChanged;
            this.Controls.Add(lbPedidos);

            
            
            
            gbAsignacion = new GroupBox
            {
                Text = "Asignación de Recursos Logísticos",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(99, 102, 241),
                Location = new Point(410, 35),
                Size = new Size(370, 320)
            };
            this.Controls.Add(gbAsignacion);

            lblVehiculo = new Label { Text = "Vehículo Disponible (Placa - Capacidad):", ForeColor = Color.White, Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), Location = new Point(20, 30), Size = new Size(330, 18) };
            cbVehiculos = new ComboBox
            {
                Location = new Point(20, 50),
                Size = new Size(330, 25),
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            gbAsignacion.Controls.Add(lblVehiculo);
            gbAsignacion.Controls.Add(cbVehiculos);

            lblConductor = new Label { Text = "Conductor Disponible (Licencia):", ForeColor = Color.White, Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), Location = new Point(20, 95), Size = new Size(330, 18) };
            cbConductores = new ComboBox
            {
                Location = new Point(20, 115),
                Size = new Size(330, 25),
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            gbAsignacion.Controls.Add(lblConductor);
            gbAsignacion.Controls.Add(cbConductores);

            lblFechaEstimada = new Label { Text = "Fecha Estimada de Entrega:", ForeColor = Color.White, Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), Location = new Point(20, 160), Size = new Size(330, 18) };
            dtpFechaEstimada = new DateTimePicker
            {
                Location = new Point(20, 180),
                Size = new Size(330, 25),
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(1)
            };
            gbAsignacion.Controls.Add(lblFechaEstimada);
            gbAsignacion.Controls.Add(dtpFechaEstimada);

            lblRuta = new Label { Text = "Indicaciones de Ruta / Trayecto:", ForeColor = Color.White, Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), Location = new Point(20, 225), Size = new Size(330, 18) };
            txtRuta = new TextBox
            {
                Location = new Point(20, 245),
                Size = new Size(330, 23),
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            gbAsignacion.Controls.Add(lblRuta);
            gbAsignacion.Controls.Add(txtRuta);

            
            
            
            btnDespachar = new Button
            {
                Text = "GENERAR DESPACHO (COMMIT)",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(25, 380),
                Size = new Size(360, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(99, 102, 241),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnDespachar.FlatAppearance.BorderSize = 0;
            btnDespachar.Click += btnDespachar_Click;
            this.Controls.Add(btnDespachar);

            btnCancelar = new Button
            {
                Text = "CANCELAR",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(410, 380),
                Size = new Size(370, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 45, 75),
                ForeColor = Color.FromArgb(180, 190, 210),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancelar);

            
            RefreshData();
        }

        private void RefreshData()
        {
            try
            {
                pendingOrders = _db.GetPedidos("Pendiente");
                availableVehicles = _db.GetVehiculos("Disponible");
                availableDrivers = _db.GetConductores(true);

                
                lbPedidos.Items.Clear();
                foreach (var p in pendingOrders)
                {
                    lbPedidos.Items.Add($"Pedido #{p.IdPedido} - {p.ClienteNombre} ({p.PesoKg}kg)");
                }

                
                cbVehiculos.DisplayMember = "DescripcionCompleta";
                cbVehiculos.ValueMember = "IdVehiculo";
                cbVehiculos.DataSource = availableVehicles;

                cbConductores.DisplayMember = "NombreCompleto";
                cbConductores.ValueMember = "IdConductor";
                cbConductores.DataSource = availableDrivers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar catálogos desde SQL Server: {ex.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lbPedidos_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lbPedidos.SelectedIndex;
            if (index >= 0 && index < pendingOrders.Count)
            {
                var p = pendingOrders[index];
                txtRuta.Text = $"Ruta directa hacia {p.DireccionEntrega}";
            }
        }

        private void btnDespachar_Click(object sender, EventArgs e)
        {
            int index = lbPedidos.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("Por favor seleccione un pedido pendiente de la columna izquierda.", "Selección Requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbVehiculos.SelectedValue == null || cbConductores.SelectedValue == null)
            {
                MessageBox.Show("Por favor asegúrese de tener seleccionado un vehículo y conductor disponibles.", "Selección Requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var pedido = pendingOrders[index];
            var vehiculo = cbVehiculos.SelectedItem as Vehiculo;
            var conductor = cbConductores.SelectedItem as Conductor;

            if (pedido == null || vehiculo == null || conductor == null) return;

            
            
            
            if (vehiculo.CapacidadCargaKg < pedido.PesoKg)
            {
                string msg = $"¡ALERTA DE SOBRECARGA!\n\nEl vehículo seleccionado ({vehiculo.Placa}) tiene una capacidad máxima de {vehiculo.CapacidadCargaKg} kg, el cual es inferior al peso del pedido ({pedido.PesoKg} kg).\n\nAsigne una unidad de mayor capacidad.";
                MessageBox.Show(msg, "Bloqueo de Capacidad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            
            
            var nuevoEnvio = new Envio
            {
                IdPedido = pedido.IdPedido,
                IdVehiculo = vehiculo.IdVehiculo,
                IdConductor = conductor.IdConductor,
                IdOperador = Session.UserId,
                FechaEntregaEstimada = dtpFechaEstimada.Value.Date,
                RutaDescripcion = txtRuta.Text.Trim(),
                VehiculoPlaca = vehiculo.Placa,
                ConductorNombre = $"{conductor.Nombre} {conductor.Apellido}"
            };

            try
            {
                _db.AddEnvio(nuevoEnvio);
                
                string successMsg = $"¡Órden de Despacho ejecutada con éxito!\n\nSe ha guardado en la base de datos (Commit Transaccional):\n1. Registro de Envío creado.\n2. Pedido #{pedido.IdPedido} marcado como Asignado.\n3. Vehículo {vehiculo.Placa} en ruta.\n4. Conductor ocupado.";
                MessageBox.Show(successMsg, "Despacho Confirmado (ACID)", MessageBoxButtons.OK, MessageBoxIcon.Information);

                
                _parentForm.RefreshDashboardData();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en la transacción SQL Server (Rollback activado): {ex.Message}", "Fallo de Transacción", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
