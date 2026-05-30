using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace RapidoSurWinForms
{
    public class TrackingForm : Form
    {
        private readonly DbService _db = new DbService();

        
        private Label lblSearch;
        private TextBox txtPedidoId;
        private Button btnRastrear;

        private Panel panelNotFound;
        private Label lblNotFoundTitle;
        private Label lblNotFoundDesc;

        private Panel panelPending;
        private Label lblPendingTitle;
        private Label lblPendingDesc;

        private Panel panelDetails;
        private GroupBox gbFicha;
        private Label lblClienteVal;
        private Label lblDireccionVal;
        private Label lblCargaVal;
        private Label lblVehiculoVal;
        private Label lblConductorVal;
        private Label lblFechaEstimadaVal;
        private Label lblEstadoBadge;

        private GroupBox gbHistorial;
        private ListBox lbTimeline;

        public TrackingForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Simulador de Rastreo de Envíos para el Cliente - Rápido Sur S.R.L.";
            this.Size = new Size(800, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 244, 248);

            lblSearch = new Label
            {
                Text = "INGRESE EL NÚMERO / ID DE PEDIDO QUE LE PROPORCIONÓ LA EMPRESA:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 110, 120),
                Location = new Point(25, 20),
                Size = new Size(600, 20)
            };
            this.Controls.Add(lblSearch);

            txtPedidoId = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Location = new Point(25, 45),
                Size = new Size(580, 29),
                BackColor = Color.White,
                ForeColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(txtPedidoId);

            btnRastrear = new Button
            {
                Text = "RASTREAR",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(620, 44),
                Size = new Size(135, 31),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnRastrear.FlatAppearance.BorderSize = 0;
            btnRastrear.Click += btnRastrear_Click;
            this.Controls.Add(btnRastrear);

            panelNotFound = new Panel
            {
                Location = new Point(25, 100),
                Size = new Size(730, 350),
                BackColor = Color.White,
                Visible = false
            };
            this.Controls.Add(panelNotFound);

            lblNotFoundTitle = new Label
            {
                Text = "❌ PEDIDO NO ENCONTRADO",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(239, 68, 68),
                Location = new Point(20, 100),
                Size = new Size(690, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            lblNotFoundDesc = new Label
            {
                Text = "El código ingresado no existe en nuestro sistema logístico.\n\nPor favor, valide el número de despacho e intente nuevamente.",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 110, 120),
                Location = new Point(20, 140),
                Size = new Size(690, 80),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelNotFound.Controls.AddRange(new Control[] { lblNotFoundTitle, lblNotFoundDesc });

            panelPending = new Panel
            {
                Location = new Point(25, 100),
                Size = new Size(730, 350),
                BackColor = Color.White,
                Visible = false
            };
            this.Controls.Add(panelPending);

            lblPendingTitle = new Label
            {
                Text = "⚠️ PEDIDO EN COLA DE ESPERA",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(212, 120, 3),
                Location = new Point(20, 60),
                Size = new Size(690, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            lblPendingDesc = new Label
            {
                Text = "El pedido ha sido recibido con éxito en nuestras sucursales y se encuentra en estado 'PENDIENTE'.\n\nActualmente nuestro equipo de logística está organizando la carga y asignando un vehículo de la flota libre.\n\nLe sugerimos volver a rastrear en unos minutos para ver su hoja de ruta asignada.",
                Font = new Font("Segoe UI", 10.5f),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(50, 110),
                Size = new Size(630, 180),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelPending.Controls.AddRange(new Control[] { lblPendingTitle, lblPendingDesc });

            panelDetails = new Panel
            {
                Location = new Point(25, 100),
                Size = new Size(730, 350),
                BackColor = Color.Transparent,
                Visible = false
            };
            this.Controls.Add(panelDetails);

            gbFicha = new GroupBox
            {
                Text = "Ficha Descriptiva del Envío",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 58, 86),
                Location = new Point(0, 0),
                Size = new Size(350, 330),
                BackColor = Color.White
            };
            panelDetails.Controls.Add(gbFicha);

            lblEstadoBadge = new Label
            {
                Text = "ESTADO",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(41, 128, 185),
                Location = new Point(20, 25),
                Size = new Size(310, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            gbFicha.Controls.Add(lblEstadoBadge);

            lblClienteVal = CreateFichaLabel("Cliente:", 70);
            lblDireccionVal = CreateFichaLabel("Dirección Destino:", 110);
            lblCargaVal = CreateFichaLabel("Mercancía / Peso:", 150);
            lblVehiculoVal = CreateFichaLabel("Placa del Camión:", 190);
            lblConductorVal = CreateFichaLabel("Conductor a Cargo:", 230);
            lblFechaEstimadaVal = CreateFichaLabel("Fecha Estimada:", 270);

            gbFicha.Controls.AddRange(new Control[] { lblClienteVal, lblDireccionVal, lblCargaVal, lblVehiculoVal, lblConductorVal, lblFechaEstimadaVal });

            gbHistorial = new GroupBox
            {
                Text = "Línea de Tiempo del Historial de Ruta",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 58, 86),
                Location = new Point(370, 0),
                Size = new Size(360, 330),
                BackColor = Color.White
            };
            panelDetails.Controls.Add(gbHistorial);

            lbTimeline = new ListBox
            {
                Location = new Point(15, 25),
                Size = new Size(330, 290),
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.None,
                HorizontalScrollbar = true
            };
            gbHistorial.Controls.Add(lbTimeline);
        }

        private Label CreateFichaLabel(string title, int top)
        {
            return new Label
            {
                Text = $"{title} Loading...",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(20, top),
                Size = new Size(310, 35)
            };
        }

        private void btnRastrear_Click(object sender, EventArgs e)
        {
            string input = txtPedidoId.Text.Trim();
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out int idPedido))
            {
                MessageBox.Show("Por favor ingrese un número de pedido válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            panelNotFound.Visible = false;
            panelPending.Visible = false;
            panelDetails.Visible = false;

            try
            {
                var envio = _db.GetEnvioByPedido(idPedido);
                if (envio != null)
                {
                    
                    panelDetails.Visible = true;

                    
                    lblEstadoBadge.Text = envio.EstadoEnvio.ToUpper();
                    if (envio.EstadoEnvio == "Asignado") { lblEstadoBadge.BackColor = Color.FromArgb(59, 130, 246); } 
                    else if (envio.EstadoEnvio == "En Ruta") { lblEstadoBadge.BackColor = Color.FromArgb(99, 102, 241); } 
                    else if (envio.EstadoEnvio == "Con Incidencia") { lblEstadoBadge.BackColor = Color.FromArgb(239, 68, 68); } 
                    else if (envio.EstadoEnvio == "Reprogramado") { lblEstadoBadge.BackColor = Color.FromArgb(245, 158, 11); } 
                    else if (envio.EstadoEnvio == "Entregado") { lblEstadoBadge.BackColor = Color.FromArgb(16, 185, 129); } 

                    lblClienteVal.Text = $"Cliente: {envio.ClienteNombre}";
                    lblDireccionVal.Text = $"Destino: {envio.DireccionEntrega}";
                    lblCargaVal.Text = $"Carga: {envio.TipoCarga} ({envio.PesoKg} kg)";
                    lblVehiculoVal.Text = $"Camión: Placa {envio.VehiculoPlaca}";
                    lblConductorVal.Text = $"Chofer: {envio.ConductorNombre}";
                    lblFechaEstimadaVal.Text = $"Fecha Estimada: {envio.FechaEntregaEstimada?.ToString("dd/MM/yyyy")}";

                    
                    lbTimeline.Items.Clear();
                    var historial = _db.GetHistorialEnvio(envio.IdEnvio);
                    foreach (var h in historial)
                    {
                        string dateStr = h.FechaActualizacion.ToString("dd/MM/yyyy hh:mm tt");
                        lbTimeline.Items.Add($"[{dateStr}] - {h.Estado.ToUpper()}");
                        lbTimeline.Items.Add($"   Detalle: {h.Descripcion}");
                        if (!string.IsNullOrEmpty(h.RegistradoPor))
                        {
                            lbTimeline.Items.Add($"   Registrado por: {h.RegistradoPor}");
                        }
                        lbTimeline.Items.Add("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                    }
                }
                else
                {
                    
                    var pedido = _db.GetPedido(idPedido);
                    if (pedido != null)
                    {
                        panelPending.Visible = true;
                    }
                    else
                    {
                        panelNotFound.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar a SQL Server: {ex.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
