using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace RapidoSurWinForms
{
    public class ReportsForm : Form
    {
        private readonly DbService _db = new DbService();

        
        private Label lblReportType;
        private ComboBox cbReportType;
        
        private Label lblFechaInicio;
        private DateTimePicker dtpInicio;
        private Label lblFechaFin;
        private DateTimePicker dtpFin;

        private Button btnGenerar;
        private Label lblResults;
        private DataGridView dgvReportResults;

        public ReportsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Reportes Analíticos del Desempeño Logístico (Acceso Restringido a Administradores)";
            this.Size = new Size(950, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 244, 248);

            Panel paramsPanel = new Panel
            {
                Location = new Point(25, 20),
                Size = new Size(885, 80),
                BackColor = Color.White
            };
            this.Controls.Add(paramsPanel);

            lblReportType = new Label
            {
                Text = "Tipo de Reporte:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 110, 120),
                Location = new Point(20, 15),
                Size = new Size(180, 18)
            };
            cbReportType = new ComboBox
            {
                Location = new Point(20, 36),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9.5f)
            };
            cbReportType.Items.AddRange(new string[] { "Entregas", "Incidencias", "Flota", "Pedidos" });
            cbReportType.SelectedIndex = 0; 

            lblFechaInicio = new Label
            {
                Text = "Fecha de Inicio:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 110, 120),
                Location = new Point(250, 15),
                Size = new Size(150, 18)
            };
            dtpInicio = new DateTimePicker
            {
                Location = new Point(250, 36),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Value = DateTime.Now.AddDays(-30)
            };

            lblFechaFin = new Label
            {
                Text = "Fecha de Fin:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 110, 120),
                Location = new Point(430, 15),
                Size = new Size(150, 18)
            };
            dtpFin = new DateTimePicker
            {
                Location = new Point(430, 36),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Value = DateTime.Now
            };

            btnGenerar = new Button
            {
                Text = "GENERAR REPORTE",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(620, 28),
                Size = new Size(240, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnGenerar.FlatAppearance.BorderSize = 0;
            btnGenerar.Click += btnGenerar_Click;

            paramsPanel.Controls.AddRange(new Control[] { lblReportType, cbReportType, lblFechaInicio, dtpInicio, lblFechaFin, dtpFin, btnGenerar });

            lblResults = new Label
            {
                Text = "RESULTADOS DEL REPORTE GENERADO:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 58, 86),
                Location = new Point(25, 120),
                Size = new Size(500, 20)
            };
            this.Controls.Add(lblResults);

            dgvReportResults = new DataGridView
            {
                Location = new Point(25, 145),
                Size = new Size(885, 345),
                BackgroundColor = Color.White,
                ForeColor = Color.Black,
                GridColor = Color.FromArgb(220, 224, 224),
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
            dgvReportResults.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(235, 240, 245);
            dgvReportResults.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(31, 58, 86);
            dgvReportResults.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvReportResults.DefaultCellStyle.BackColor = Color.White;
            dgvReportResults.DefaultCellStyle.ForeColor = Color.Black;
            dgvReportResults.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
            dgvReportResults.DefaultCellStyle.SelectionForeColor = Color.White;
            this.Controls.Add(dgvReportResults);

            GenerateReport();
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void GenerateReport()
        {
            string tipo = cbReportType.SelectedItem?.ToString() ?? "Entregas";
            DateTime inicio = dtpInicio.Value;
            DateTime fin = dtpFin.Value;

            if (inicio.Date > fin.Date)
            {
                MessageBox.Show("La fecha de inicio no puede ser posterior a la fecha de fin.", "Rango Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var resultados = _db.RunReport(tipo, inicio, fin);
                lblResults.Text = $"RESULTADOS DEL REPORTE GENERADO ({tipo.ToUpper()}): {resultados.Count} REGISTROS ENCONTRADOS";

                
                dgvReportResults.Columns.Clear();
                dgvReportResults.Rows.Clear();

                if (resultados.Count == 0)
                {
                    MessageBox.Show("No se encontraron registros para el tipo de reporte y rango de fechas seleccionados.", "Sin Resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                
                
                var firstRow = resultados[0];
                foreach (string key in firstRow.Keys)
                {
                    dgvReportResults.Columns.Add(key, key);
                }

                
                foreach (var rowDict in resultados)
                {
                    var rowValues = new List<object>();
                    foreach (string key in rowDict.Keys)
                    {
                        object val = rowDict[key];
                        
                        if (val is DateTime dt)
                        {
                            rowValues.Add(dt.ToString("dd/MM/yyyy hh:mm tt"));
                        }
                        else if (val == DBNull.Value || val == null)
                        {
                            rowValues.Add("-");
                        }
                        else
                        {
                            rowValues.Add(val);
                        }
                    }
                    dgvReportResults.Rows.Add(rowValues.ToArray());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar reporte de SQL Server: {ex.Message}", "Error analítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
