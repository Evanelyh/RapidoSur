using System;
using System.Collections.Generic;

namespace RapidoSur.Models
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}";
    }

    public class Pedido
    {
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }
        public string ClienteNombre { get; set; } = string.Empty; // Campo auxiliar
        public DateTime FechaSolicitud { get; set; }
        public string DireccionEntrega { get; set; } = string.Empty;
        public string? TipoCarga { get; set; }
        public decimal PesoKg { get; set; }
        public string Prioridad { get; set; } = "Media";
        public string Estado { get; set; } = "Pendiente";
        public string? Observaciones { get; set; }
    }

    public class Vehiculo
    {
        public int IdVehiculo { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public decimal CapacidadCargaKg { get; set; }
        public string EstadoOperativo { get; set; } = "Disponible";
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public int? Anio { get; set; }

        public string DescripcionCompleta => $"{Marca} {Modelo} ({Placa}) - Cap: {CapacidadCargaKg}kg";
    }

    public class Conductor
    {
        public int IdConductor { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string NumLicencia { get; set; } = string.Empty;
        public string? TipoLicencia { get; set; }
        public string? Telefono { get; set; }
        public bool Disponible { get; set; } = true;
        public DateTime? FechaIngreso { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido} ({TipoLicencia})";
    }

    public class Operador
    {
        public int IdOperador { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
        public string Rol { get; set; } = "Operador";
        public string? Correo { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}";
    }

    public class Envio
    {
        public int IdEnvio { get; set; }
        public int IdPedido { get; set; }
        public int IdVehiculo { get; set; }
        public int IdConductor { get; set; }
        public int IdOperador { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public DateTime? FechaEntregaEstimada { get; set; }
        public DateTime? FechaEntregaReal { get; set; }
        public string? RutaDescripcion { get; set; }
        public string EstadoEnvio { get; set; } = "Asignado";

        // Campos auxiliares para la interfaz
        public string ClienteNombre { get; set; } = string.Empty;
        public string DireccionEntrega { get; set; } = string.Empty;
        public string TipoCarga { get; set; } = string.Empty;
        public decimal PesoKg { get; set; }
        public string VehiculoPlaca { get; set; } = string.Empty;
        public string ConductorNombre { get; set; } = string.Empty;
    }

    public class HistorialEstado
    {
        public int IdHistorial { get; set; }
        public int IdEnvio { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaActualizacion { get; set; }
        public string? Descripcion { get; set; }
        public string? RegistradoPor { get; set; }
    }

    // ViewModels auxiliares
    public class DashboardViewModel
    {
        public int PedidosRecibidos { get; set; }
        public int VehiculosEnRuta { get; set; }
        public int EntregasCompletadas { get; set; }
        public int AlertasRetrasos { get; set; }
        public List<Pedido> ÚltimosPedidos { get; set; } = new();
    }

    public class AsignacionViewModel
    {
        public List<Pedido> PedidosPendientes { get; set; } = new();
        public List<Vehiculo> VehiculosDisponibles { get; set; } = new();
        public List<Conductor> ConductoresDisponibles { get; set; } = new();
    }

    public class ReporteViewModel
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string TipoReporte { get; set; } = "Entregas";
        public int TotalRegistros => Resultados.Count;
        public List<Dictionary<string, object>> Resultados { get; set; } = new();
    }
}
