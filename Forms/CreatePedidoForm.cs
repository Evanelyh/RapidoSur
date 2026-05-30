using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace RapidoSurWinForms
{
    public class CreatePedidoForm : Form
    {
        private readonly MainForm _parentForm;
        private readonly DbService _db = new DbService();

        
        private RadioButton rbClienteExistente;
        private RadioButton rbClienteNuevo;
        private Panel panelClienteExistente;
        private ComboBox cbClientes;
        private Label lblSelectCliente;

        private Panel panelClienteNuevo;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblApellido;
        private TextBox txtApellido;
        private Label lblTelefono;
        private TextBox txtTelefono;
        private Label lblCorreo;
        private TextBox txtCorreo;
        private Label lblDireccion;
        private TextBox txtDireccion;

        
        private GroupBox gbPedido;
        private Label lblDireccionEntrega;
        private TextBox txtDireccionEntrega;
        private Label lblTipoCarga;
        private TextBox txtTipoCarga;
        private Label lblPesoKg;
        private NumericUpDown numPesoKg;
        private Label lblPrioridad;
        private ComboBox cbPrioridad;
        private Label lblObservaciones;
        private TextBox txtObservaciones;

        private Button btnGuardar;
        private Button btnCancelar;

        public CreatePedidoForm(MainForm parentForm)
        {
            _parentForm = parentForm;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Registrar Nueva Orden de Pedido";
            this.Size = new Size(500, 640);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 244, 248);

            rbClienteExistente = new RadioButton
            {
                Text = "Cliente Registrado",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 58, 86),
                Location = new Point(25, 20),
                Size = new Size(180, 25),
                Checked = true
            };
            rbClienteExistente.CheckedChanged += (s, e) => ToggleClientType();
            this.Controls.Add(rbClienteExistente);

            rbClienteNuevo = new RadioButton
            {
                Text = "Crear Cliente Nuevo",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 58, 86),
                Location = new Point(220, 20),
                Size = new Size(180, 25)
            };
            this.Controls.Add(rbClienteNuevo);

            panelClienteExistente = new Panel
            {
                Location = new Point(25, 55),
                Size = new Size(430, 70),
                BackColor = Color.White
            };
            this.Controls.Add(panelClienteExistente);

            lblSelectCliente = new Label
            {
                Text = "Seleccione el Cliente:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 110, 120),
                Location = new Point(15, 10),
                Size = new Size(400, 18)
            };
            cbClientes = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(15, 30),
                Size = new Size(400, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            panelClienteExistente.Controls.Add(lblSelectCliente);
            panelClienteExistente.Controls.Add(cbClientes);

            panelClienteNuevo = new Panel
            {
                Location = new Point(25, 55),
                Size = new Size(430, 185),
                BackColor = Color.White,
                Visible = false
            };
            this.Controls.Add(panelClienteNuevo);

            lblNombre = new Label { Text = "Nombre:", ForeColor = Color.FromArgb(64, 64, 64), Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), Location = new Point(15, 10), Size = new Size(180, 18) };
            txtNombre = new TextBox { Location = new Point(15, 30), Size = new Size(180, 23), BackColor = Color.White, ForeColor = Color.Black, BorderStyle = BorderStyle.FixedSingle };
            lblApellido = new Label { Text = "Apellido:", ForeColor = Color.FromArgb(64, 64, 64), Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), Location = new Point(220, 10), Size = new Size(185, 18) };
            txtApellido = new TextBox { Location = new Point(220, 30), Size = new Size(195, 23), BackColor = Color.White, ForeColor = Color.Black, BorderStyle = BorderStyle.FixedSingle };

            lblTelefono = new Label { Text = "Teléfono:", ForeColor = Color.FromArgb(64, 64, 64), Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), Location = new Point(15, 65), Size = new Size(180, 18) };
            txtTelefono = new TextBox { Location = new Point(15, 85), Size = new Size(180, 23), BackColor = Color.White, ForeColor = Color.Black, BorderStyle = BorderStyle.FixedSingle };
            lblCorreo = new Label { Text = "Correo:", ForeColor = Color.FromArgb(64, 64, 64), Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), Location = new Point(220, 65), Size = new Size(185, 18) };
            txtCorreo = new TextBox { Location = new Point(220, 85), Size = new Size(195, 23), BackColor = Color.White, ForeColor = Color.Black, BorderStyle = BorderStyle.FixedSingle };

            lblDireccion = new Label { Text = "Dirección de Facturación:", ForeColor = Color.FromArgb(64, 64, 64), Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), Location = new Point(15, 120), Size = new Size(390, 18) };
            txtDireccion = new TextBox { Location = new Point(15, 140), Size = new Size(400, 23), BackColor = Color.White, ForeColor = Color.Black, BorderStyle = BorderStyle.FixedSingle };

            panelClienteNuevo.Controls.AddRange(new Control[] { lblNombre, txtNombre, lblApellido, txtApellido, lblTelefono, txtTelefono, lblCorreo, txtCorreo, lblDireccion, txtDireccion });

            gbPedido = new GroupBox
            {
                Text = "Detalles de la Carga y Entrega",
                ForeColor = Color.FromArgb(31, 58, 86),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Location = new Point(25, 260),
                Size = new Size(430, 260)
            };
            this.Controls.Add(gbPedido);

            lblDireccionEntrega = new Label { Text = "Dirección de Entrega:", ForeColor = Color.FromArgb(64, 64, 64), Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), Location = new Point(15, 25), Size = new Size(400, 18) };
            txtDireccionEntrega = new TextBox { Location = new Point(15, 45), Size = new Size(400, 23), BackColor = Color.White, ForeColor = Color.Black, BorderStyle = BorderStyle.FixedSingle };

            lblTipoCarga = new Label { Text = "Tipo de Mercancía:", ForeColor = Color.FromArgb(64, 64, 64), Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), Location = new Point(15, 80), Size = new Size(180, 18) };
            txtTipoCarga = new TextBox { Location = new Point(15, 100), Size = new Size(180, 23), BackColor = Color.White, ForeColor = Color.Black, BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Ej. Alimentos, Muebles" };

            lblPesoKg = new Label { Text = "Peso de la Carga (kg):", ForeColor = Color.FromArgb(64, 64, 64), Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), Location = new Point(220, 80), Size = new Size(195, 18) };
            numPesoKg = new NumericUpDown
            {
                Location = new Point(220, 100),
                Size = new Size(195, 23),
                BackColor = Color.White,
                ForeColor = Color.Black,
                Minimum = 1,
                Maximum = 99999,
                DecimalPlaces = 2,
                Value = 100
            };

            lblPrioridad = new Label { Text = "Prioridad:", ForeColor = Color.FromArgb(64, 64, 64), Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), Location = new Point(15, 140), Size = new Size(180, 18) };
            cbPrioridad = new ComboBox
            {
                Location = new Point(15, 160),
                Size = new Size(180, 23),
                BackColor = Color.White,
                ForeColor = Color.Black,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbPrioridad.Items.AddRange(new string[] { "Baja", "Media", "Alta" });
            cbPrioridad.SelectedIndex = 1; 

            lblObservaciones = new Label { Text = "Observaciones / Indicaciones Especiales:", ForeColor = Color.FromArgb(64, 64, 64), Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), Location = new Point(15, 195), Size = new Size(400, 18) };
            txtObservaciones = new TextBox { Location = new Point(15, 215), Size = new Size(400, 23), BackColor = Color.White, ForeColor = Color.Black, BorderStyle = BorderStyle.FixedSingle };

            gbPedido.Controls.AddRange(new Control[] { lblDireccionEntrega, txtDireccionEntrega, lblTipoCarga, txtTipoCarga, lblPesoKg, numPesoKg, lblPrioridad, cbPrioridad, lblObservaciones, txtObservaciones });

            btnGuardar = new Button
            {
                Text = "GUARDAR PEDIDO",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(25, 540),
                Size = new Size(210, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += btnGuardar_Click;
            this.Controls.Add(btnGuardar);

            btnCancelar = new Button
            {
                Text = "CANCELAR",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(245, 540),
                Size = new Size(210, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 224, 224),
                ForeColor = Color.FromArgb(64, 64, 64),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancelar);

            LoadClients();
            ToggleClientType();
        }

        private void LoadClients()
        {
            try
            {
                var list = _db.GetClientes();
                cbClientes.DisplayMember = "NombreCompleto";
                cbClientes.ValueMember = "IdCliente";
                cbClientes.DataSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clientes de SQL Server: {ex.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToggleClientType()
        {
            if (rbClienteExistente.Checked)
            {
                panelClienteExistente.Visible = true;
                panelClienteNuevo.Visible = false;
                gbPedido.Location = new Point(25, 140);
                gbPedido.Height = 380; 
            }
            else
            {
                panelClienteExistente.Visible = false;
                panelClienteNuevo.Visible = true;
                gbPedido.Location = new Point(25, 260);
                gbPedido.Height = 260;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            int clienteIdActual = 0;

            if (rbClienteExistente.Checked)
            {
                if (cbClientes.SelectedValue == null)
                {
                    MessageBox.Show("Por favor seleccione un cliente registrado.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                clienteIdActual = (int)cbClientes.SelectedValue;
            }
            else 
            {
                string nombre = txtNombre.Text.Trim();
                string apellido = txtApellido.Text.Trim();

                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido))
                {
                    MessageBox.Show("El nombre y apellido del nuevo cliente son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var nuevoCliente = new Cliente
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    Telefono = txtTelefono.Text.Trim(),
                    Correo = txtCorreo.Text.Trim(),
                    Direccion = txtDireccion.Text.Trim()
                };

                try
                {
                    clienteIdActual = _db.AddCliente(nuevoCliente);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar el nuevo cliente en la base de datos: {ex.Message}", "Error de Guardado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            
            string direccion = txtDireccionEntrega.Text.Trim();
            decimal peso = numPesoKg.Value;

            if (string.IsNullOrEmpty(direccion))
            {
                MessageBox.Show("La dirección de entrega es obligatoria.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nuevoPedido = new Pedido
            {
                IdCliente = clienteIdActual,
                DireccionEntrega = direccion,
                TipoCarga = txtTipoCarga.Text.Trim(),
                PesoKg = peso,
                Prioridad = cbPrioridad.SelectedItem?.ToString() ?? "Media",
                Observaciones = txtObservaciones.Text.Trim()
            };

            try
            {
                _db.AddPedido(nuevoPedido);
                MessageBox.Show("¡Pedido registrado con éxito! Su estado actual es: PENDIENTE.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                
                _parentForm.RefreshDashboardData();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el pedido en la base de datos: {ex.Message}", "Error de Guardado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
