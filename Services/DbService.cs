using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using RapidoSur.Models;

namespace RapidoSur.Services
{
    public class DbService
    {
        private readonly string _connectionString;

        public DbService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        // ==========================================
        // 1. GESTIÓN DE CLIENTES
        // ==========================================
        public List<Cliente> GetClientes()
        {
            var list = new List<Cliente>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT * FROM cliente ORDER BY nombre, apellido", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Cliente
                {
                    IdCliente = reader.GetInt32("id_cliente"),
                    Nombre = reader.GetString("nombre"),
                    Apellido = reader.GetString("apellido"),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString("telefono"),
                    Correo = reader.IsDBNull(reader.GetOrdinal("correo")) ? null : reader.GetString("correo"),
                    Direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString("direccion"),
                    FechaRegistro = reader.GetDateTime("fecha_registro")
                });
            }
            return list;
        }

        public Cliente? GetCliente(int id)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT * FROM cliente WHERE id_cliente = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Cliente
                {
                    IdCliente = reader.GetInt32("id_cliente"),
                    Nombre = reader.GetString("nombre"),
                    Apellido = reader.GetString("apellido"),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString("telefono"),
                    Correo = reader.IsDBNull(reader.GetOrdinal("correo")) ? null : reader.GetString("correo"),
                    Direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString("direccion"),
                    FechaRegistro = reader.GetDateTime("fecha_registro")
                };
            }
            return null;
        }

        public int AddCliente(Cliente c)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(
                "INSERT INTO cliente (nombre, apellido, telefono, correo, direccion) VALUES (@nombre, @apellido, @telefono, @correo, @direccion); SELECT LAST_INSERT_ID();", 
                conn);
            cmd.Parameters.AddWithValue("@nombre", c.Nombre);
            cmd.Parameters.AddWithValue("@apellido", c.Apellido);
            cmd.Parameters.AddWithValue("@telefono", (object?)c.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@correo", (object?)c.Correo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", (object?)c.Direccion ?? DBNull.Value);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // ==========================================
        // 2. GESTIÓN DE PEDIDOS
        // ==========================================
        public List<Pedido> GetPedidos(string? estado = null)
        {
            var list = new List<Pedido>();
            using var conn = GetConnection();
            conn.Open();
            
            string query = @"SELECT p.*, CONCAT(c.nombre, ' ', c.apellido) AS cliente_nombre 
                             FROM pedido p 
                             INNER JOIN cliente c ON p.id_cliente = c.id_cliente";
            
            if (!string.IsNullOrEmpty(estado))
            {
                query += " WHERE p.estado = @estado";
            }
            query += " ORDER BY p.fecha_solicitud DESC";

            using var cmd = new MySqlCommand(query, conn);
            if (!string.IsNullOrEmpty(estado))
            {
                cmd.Parameters.AddWithValue("@estado", estado);
            }

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Pedido
                {
                    IdPedido = reader.GetInt32("id_pedido"),
                    IdCliente = reader.GetInt32("id_cliente"),
                    ClienteNombre = reader.GetString("cliente_nombre"),
                    FechaSolicitud = reader.GetDateTime("fecha_solicitud"),
                    DireccionEntrega = reader.GetString("direccion_entrega"),
                    TipoCarga = reader.IsDBNull(reader.GetOrdinal("tipo_carga")) ? null : reader.GetString("tipo_carga"),
                    PesoKg = reader.GetDecimal("peso_kg"),
                    Prioridad = reader.GetString("prioridad"),
                    Estado = reader.GetString("estado"),
                    Observaciones = reader.IsDBNull(reader.GetOrdinal("observaciones")) ? null : reader.GetString("observaciones")
                });
            }
            return list;
        }

        public Pedido? GetPedido(int id)
        {
            using var conn = GetConnection();
            conn.Open();
            string query = @"SELECT p.*, CONCAT(c.nombre, ' ', c.apellido) AS cliente_nombre 
                             FROM pedido p 
                             INNER JOIN cliente c ON p.id_cliente = c.id_cliente 
                             WHERE p.id_pedido = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Pedido
                {
                    IdPedido = reader.GetInt32("id_pedido"),
                    IdCliente = reader.GetInt32("id_cliente"),
                    ClienteNombre = reader.GetString("cliente_nombre"),
                    FechaSolicitud = reader.GetDateTime("fecha_solicitud"),
                    DireccionEntrega = reader.GetString("direccion_entrega"),
                    TipoCarga = reader.IsDBNull(reader.GetOrdinal("tipo_carga")) ? null : reader.GetString("tipo_carga"),
                    PesoKg = reader.GetDecimal("peso_kg"),
                    Prioridad = reader.GetString("prioridad"),
                    Estado = reader.GetString("estado"),
                    Observaciones = reader.IsDBNull(reader.GetOrdinal("observaciones")) ? null : reader.GetString("observaciones")
                };
            }
            return null;
        }

        public int AddPedido(Pedido p)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(
                @"INSERT INTO pedido (id_cliente, direccion_entrega, tipo_carga, peso_kg, prioridad, estado, observaciones) 
                  VALUES (@id_cliente, @direccion_entrega, @tipo_carga, @peso_kg, @prioridad, 'Pendiente', @observaciones); 
                  SELECT LAST_INSERT_ID();", 
                conn);
            cmd.Parameters.AddWithValue("@id_cliente", p.IdCliente);
            cmd.Parameters.AddWithValue("@direccion_entrega", p.DireccionEntrega);
            cmd.Parameters.AddWithValue("@tipo_carga", (object?)p.TipoCarga ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@peso_kg", p.PesoKg);
            cmd.Parameters.AddWithValue("@prioridad", p.Prioridad);
            cmd.Parameters.AddWithValue("@observaciones", (object?)p.Observaciones ?? DBNull.Value);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool UpdatePedidoEstado(int idPedido, string estado)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("UPDATE pedido SET estado = @estado WHERE id_pedido = @id", conn);
            cmd.Parameters.AddWithValue("@estado", estado);
            cmd.Parameters.AddWithValue("@id", idPedido);
            return cmd.ExecuteNonQuery() > 0;
        }

        // ==========================================
        // 3. GESTIÓN DE VEHÍCULOS
        // ==========================================
        public List<Vehiculo> GetVehiculos(string? estado = null)
        {
            var list = new List<Vehiculo>();
            using var conn = GetConnection();
            conn.Open();
            string query = "SELECT * FROM vehiculo";
            if (!string.IsNullOrEmpty(estado))
            {
                query += " WHERE estado_operativo = @estado";
            }
            query += " ORDER BY marca, modelo";
            using var cmd = new MySqlCommand(query, conn);
            if (!string.IsNullOrEmpty(estado))
            {
                cmd.Parameters.AddWithValue("@estado", estado);
            }
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Vehiculo
                {
                    IdVehiculo = reader.GetInt32("id_vehiculo"),
                    Placa = reader.GetString("placa"),
                    Tipo = reader.GetString("tipo"),
                    CapacidadCargaKg = reader.GetDecimal("capacidad_carga_kg"),
                    EstadoOperativo = reader.GetString("estado_operativo"),
                    Marca = reader.IsDBNull(reader.GetOrdinal("marca")) ? null : reader.GetString("marca"),
                    Modelo = reader.IsDBNull(reader.GetOrdinal("modelo")) ? null : reader.GetString("modelo"),
                    Anio = reader.IsDBNull(reader.GetOrdinal("anio")) ? null : reader.GetInt32("anio")
                });
            }
            return list;
        }

        public bool UpdateVehiculoEstado(int idVehiculo, string estado)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("UPDATE vehiculo SET estado_operativo = @estado WHERE id_vehiculo = @id", conn);
            cmd.Parameters.AddWithValue("@estado", estado);
            cmd.Parameters.AddWithValue("@id", idVehiculo);
            return cmd.ExecuteNonQuery() > 0;
        }

        // ==========================================
        // 4. GESTIÓN DE CONDUCTORES
        // ==========================================
        public List<Conductor> GetConductores(bool? disponible = null)
        {
            var list = new List<Conductor>();
            using var conn = GetConnection();
            conn.Open();
            string query = "SELECT * FROM conductor";
            if (disponible.HasValue)
            {
                query += " WHERE disponible = @disponible";
            }
            query += " ORDER BY nombre, apellido";
            using var cmd = new MySqlCommand(query, conn);
            if (disponible.HasValue)
            {
                cmd.Parameters.AddWithValue("@disponible", disponible.Value);
            }
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Conductor
                {
                    IdConductor = reader.GetInt32("id_conductor"),
                    Nombre = reader.GetString("nombre"),
                    Apellido = reader.GetString("apellido"),
                    NumLicencia = reader.GetString("num_licencia"),
                    TipoLicencia = reader.IsDBNull(reader.GetOrdinal("tipo_licencia")) ? null : reader.GetString("tipo_licencia"),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString("telefono"),
                    Disponible = reader.GetBoolean("disponible"),
                    FechaIngreso = reader.IsDBNull(reader.GetOrdinal("fecha_ingreso")) ? null : reader.GetDateTime("fecha_ingreso")
                });
            }
            return list;
        }

        public bool UpdateConductorDisponibilidad(int idConductor, bool disponible)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("UPDATE conductor SET disponible = @disponible WHERE id_conductor = @id", conn);
            cmd.Parameters.AddWithValue("@disponible", disponible);
            cmd.Parameters.AddWithValue("@id", idConductor);
            return cmd.ExecuteNonQuery() > 0;
        }

        // ==========================================
        // 5. AUTENTICACIÓN
        // ==========================================
        public Operador? Login(string usuario, string clave)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT * FROM operador WHERE usuario = @user AND clave = @pass", conn);
            cmd.Parameters.AddWithValue("@user", usuario);
            cmd.Parameters.AddWithValue("@pass", clave);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Operador
                {
                    IdOperador = reader.GetInt32("id_operador"),
                    Nombre = reader.GetString("nombre"),
                    Apellido = reader.GetString("apellido"),
                    Usuario = reader.GetString("usuario"),
                    Rol = reader.GetString("rol"),
                    Correo = reader.IsDBNull(reader.GetOrdinal("correo")) ? null : reader.GetString("correo")
                };
            }
            return null;
        }

        public bool UpdateOperadorProfile(int idOperador, string nombre, string apellido, string correo, string? nuevaClave)
        {
            using var conn = GetConnection();
            conn.Open();
            string query = "UPDATE operador SET nombre = @nombre, apellido = @apellido, correo = @correo";
            if (!string.IsNullOrEmpty(nuevaClave))
            {
                query += ", clave = @clave";
            }
            query += " WHERE id_operador = @id";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@apellido", apellido);
            cmd.Parameters.AddWithValue("@correo", correo);
            cmd.Parameters.AddWithValue("@id", idOperador);
            if (!string.IsNullOrEmpty(nuevaClave))
            {
                cmd.Parameters.AddWithValue("@clave", nuevaClave);
            }
            return cmd.ExecuteNonQuery() > 0;
        }

        // ==========================================
        // 6. GESTIÓN DE ENVÍOS Y SEGUIMIENTO
        // ==========================================
        public List<Envio> GetEnvios(string? estado = null, int? idConductor = null)
        {
            var list = new List<Envio>();
            using var conn = GetConnection();
            conn.Open();
            string query = @"SELECT e.*, 
                             CONCAT(c.nombre, ' ', c.apellido) AS cliente_nombre,
                             p.direccion_entrega, p.tipo_carga, p.peso_kg,
                             v.placa AS vehiculo_placa,
                             CONCAT(cond.nombre, ' ', cond.apellido) AS conductor_nombre
                             FROM envio e
                             INNER JOIN pedido p ON e.id_pedido = p.id_pedido
                             INNER JOIN cliente c ON p.id_cliente = c.id_cliente
                             INNER JOIN vehiculo v ON e.id_vehiculo = v.id_vehiculo
                             INNER JOIN conductor cond ON e.id_conductor = cond.id_conductor
                             WHERE 1=1";

            if (!string.IsNullOrEmpty(estado))
            {
                query += " AND e.estado_envio = @estado";
            }
            if (idConductor.HasValue)
            {
                query += " AND e.id_conductor = @conductor";
            }
            query += " ORDER BY e.fecha_asignacion DESC";

            using var cmd = new MySqlCommand(query, conn);
            if (!string.IsNullOrEmpty(estado))
            {
                cmd.Parameters.AddWithValue("@estado", estado);
            }
            if (idConductor.HasValue)
            {
                cmd.Parameters.AddWithValue("@conductor", idConductor.Value);
            }

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Envio
                {
                    IdEnvio = reader.GetInt32("id_envio"),
                    IdPedido = reader.GetInt32("id_pedido"),
                    IdVehiculo = reader.GetInt32("id_vehiculo"),
                    IdConductor = reader.GetInt32("id_conductor"),
                    IdOperador = reader.GetInt32("id_operador"),
                    FechaAsignacion = reader.GetDateTime("fecha_asignacion"),
                    FechaEntregaEstimada = reader.IsDBNull(reader.GetOrdinal("fecha_entrega_estimada")) ? null : reader.GetDateTime("fecha_entrega_estimada"),
                    FechaEntregaReal = reader.IsDBNull(reader.GetOrdinal("fecha_entrega_real")) ? null : reader.GetDateTime("fecha_entrega_real"),
                    RutaDescripcion = reader.IsDBNull(reader.GetOrdinal("ruta_descripcion")) ? null : reader.GetString("ruta_descripcion"),
                    EstadoEnvio = reader.GetString("estado_envio"),
                    ClienteNombre = reader.GetString("cliente_nombre"),
                    DireccionEntrega = reader.GetString("direccion_entrega"),
                    TipoCarga = reader.GetString("tipo_carga"),
                    PesoKg = reader.GetDecimal("peso_kg"),
                    VehiculoPlaca = reader.GetString("vehiculo_placa"),
                    ConductorNombre = reader.GetString("conductor_nombre")
                });
            }
            return list;
        }

        public Envio? GetEnvio(int id)
        {
            using var conn = GetConnection();
            conn.Open();
            string query = @"SELECT e.*, 
                             CONCAT(c.nombre, ' ', c.apellido) AS cliente_nombre,
                             p.direccion_entrega, p.tipo_carga, p.peso_kg,
                             v.placa AS vehiculo_placa,
                             CONCAT(cond.nombre, ' ', cond.apellido) AS conductor_nombre
                             FROM envio e
                             INNER JOIN pedido p ON e.id_pedido = p.id_pedido
                             INNER JOIN cliente c ON p.id_cliente = c.id_cliente
                             INNER JOIN vehiculo v ON e.id_vehiculo = v.id_vehiculo
                             INNER JOIN conductor cond ON e.id_conductor = cond.id_conductor
                             WHERE e.id_envio = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Envio
                {
                    IdEnvio = reader.GetInt32("id_envio"),
                    IdPedido = reader.GetInt32("id_pedido"),
                    IdVehiculo = reader.GetInt32("id_vehiculo"),
                    IdConductor = reader.GetInt32("id_conductor"),
                    IdOperador = reader.GetInt32("id_operador"),
                    FechaAsignacion = reader.GetDateTime("fecha_asignacion"),
                    FechaEntregaEstimada = reader.IsDBNull(reader.GetOrdinal("fecha_entrega_estimada")) ? null : reader.GetDateTime("fecha_entrega_estimada"),
                    FechaEntregaReal = reader.IsDBNull(reader.GetOrdinal("fecha_entrega_real")) ? null : reader.GetDateTime("fecha_entrega_real"),
                    RutaDescripcion = reader.IsDBNull(reader.GetOrdinal("ruta_descripcion")) ? null : reader.GetString("ruta_descripcion"),
                    EstadoEnvio = reader.GetString("estado_envio"),
                    ClienteNombre = reader.GetString("cliente_nombre"),
                    DireccionEntrega = reader.GetString("direccion_entrega"),
                    TipoCarga = reader.GetString("tipo_carga"),
                    PesoKg = reader.GetDecimal("peso_kg"),
                    VehiculoPlaca = reader.GetString("vehiculo_placa"),
                    ConductorNombre = reader.GetString("conductor_nombre")
                };
            }
            return null;
        }

        public Envio? GetEnvioByPedido(int idPedido)
        {
            using var conn = GetConnection();
            conn.Open();
            string query = @"SELECT e.*, 
                             CONCAT(c.nombre, ' ', c.apellido) AS cliente_nombre,
                             p.direccion_entrega, p.tipo_carga, p.peso_kg,
                             v.placa AS vehiculo_placa,
                             CONCAT(cond.nombre, ' ', cond.apellido) AS conductor_nombre
                             FROM envio e
                             INNER JOIN pedido p ON e.id_pedido = p.id_pedido
                             INNER JOIN cliente c ON p.id_cliente = c.id_cliente
                             INNER JOIN vehiculo v ON e.id_vehiculo = v.id_vehiculo
                             INNER JOIN conductor cond ON e.id_conductor = cond.id_conductor
                             WHERE e.id_pedido = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", idPedido);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Envio
                {
                    IdEnvio = reader.GetInt32("id_envio"),
                    IdPedido = reader.GetInt32("id_pedido"),
                    IdVehiculo = reader.GetInt32("id_vehiculo"),
                    IdConductor = reader.GetInt32("id_conductor"),
                    IdOperador = reader.GetInt32("id_operador"),
                    FechaAsignacion = reader.GetDateTime("fecha_asignacion"),
                    FechaEntregaEstimada = reader.IsDBNull(reader.GetOrdinal("fecha_entrega_estimada")) ? null : reader.GetDateTime("fecha_entrega_estimada"),
                    FechaEntregaReal = reader.IsDBNull(reader.GetOrdinal("fecha_entrega_real")) ? null : reader.GetDateTime("fecha_entrega_real"),
                    RutaDescripcion = reader.IsDBNull(reader.GetOrdinal("ruta_descripcion")) ? null : reader.GetString("ruta_descripcion"),
                    EstadoEnvio = reader.GetString("estado_envio"),
                    ClienteNombre = reader.GetString("cliente_nombre"),
                    DireccionEntrega = reader.GetString("direccion_entrega"),
                    TipoCarga = reader.GetString("tipo_carga"),
                    PesoKg = reader.GetDecimal("peso_kg"),
                    VehiculoPlaca = reader.GetString("vehiculo_placa"),
                    ConductorNombre = reader.GetString("conductor_nombre")
                };
            }
            return null;
        }

        public int AddEnvio(Envio e)
        {
            using var conn = GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();
            try
            {
                // 1. Insertar el Envío
                string query = @"INSERT INTO envio (id_pedido, id_vehiculo, id_conductor, id_operador, fecha_asignacion, fecha_entrega_estimada, ruta_descripcion, estado_envio) 
                                 VALUES (@id_pedido, @id_vehiculo, @id_conductor, @id_operador, NOW(), @fecha_est, @ruta, 'Asignado');
                                 SELECT LAST_INSERT_ID();";
                
                using var cmd = new MySqlCommand(query, conn, transaction);
                cmd.Parameters.AddWithValue("@id_pedido", e.IdPedido);
                cmd.Parameters.AddWithValue("@id_vehiculo", e.IdVehiculo);
                cmd.Parameters.AddWithValue("@id_conductor", e.IdConductor);
                cmd.Parameters.AddWithValue("@id_operador", e.IdOperador);
                cmd.Parameters.AddWithValue("@fecha_est", (object?)e.FechaEntregaEstimada ?? DateTime.Now.AddDays(1).Date);
                cmd.Parameters.AddWithValue("@ruta", (object?)e.RutaDescripcion ?? DBNull.Value);
                
                int idEnvio = Convert.ToInt32(cmd.ExecuteScalar());

                // 2. Actualizar estado del Pedido a 'Asignado'
                using var cmdPedido = new MySqlCommand("UPDATE pedido SET estado = 'Asignado' WHERE id_pedido = @id_pedido", conn, transaction);
                cmdPedido.Parameters.AddWithValue("@id_pedido", e.IdPedido);
                cmdPedido.ExecuteNonQuery();

                // 3. Actualizar estado del Vehículo a 'En Ruta'
                using var cmdVehiculo = new MySqlCommand("UPDATE vehiculo SET estado_operativo = 'En Ruta' WHERE id_vehiculo = @id_vehiculo", conn, transaction);
                cmdVehiculo.Parameters.AddWithValue("@id_vehiculo", e.IdVehiculo);
                cmdVehiculo.ExecuteNonQuery();

                // 4. Actualizar disponibilidad del Conductor a FALSE
                using var cmdCond = new MySqlCommand("UPDATE conductor SET disponible = FALSE WHERE id_conductor = @id_conductor", conn, transaction);
                cmdCond.Parameters.AddWithValue("@id_conductor", e.IdConductor);
                cmdCond.ExecuteNonQuery();

                // 5. Insertar en Historial de Estados
                string histQuery = @"INSERT INTO historial_estado (id_envio, estado, fecha_actualizacion, descripcion, registrado_por) 
                                     VALUES (@id_envio, 'Asignado', NOW(), @desc, @operador)";
                using var cmdHist = new MySqlCommand(histQuery, conn, transaction);
                cmdHist.Parameters.AddWithValue("@id_envio", idEnvio);
                cmdHist.Parameters.AddWithValue("@desc", $"Pedido asignado al vehículo con placa {e.VehiculoPlaca} y conductor {e.ConductorNombre}.");
                cmdHist.Parameters.AddWithValue("@operador", $"Operador ID: {e.IdOperador}");
                cmdHist.ExecuteNonQuery();

                transaction.Commit();
                return idEnvio;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public bool UpdateEnvioEstado(int idEnvio, string estado, string desc, string registradoPor, DateTime? nuevaFechaEst = null)
        {
            using var conn = GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();
            try
            {
                var envio = GetEnvio(idEnvio);
                if (envio == null) return false;

                // 1. Actualizar el estado del envío
                string query = "UPDATE envio SET estado_envio = @estado";
                if (estado == "Entregado")
                {
                    query += ", fecha_entrega_real = NOW()";
                }
                if (nuevaFechaEst.HasValue)
                {
                    query += ", fecha_entrega_estimada = @nuevaFecha";
                }
                query += " WHERE id_envio = @id";

                using var cmd = new MySqlCommand(query, conn, transaction);
                cmd.Parameters.AddWithValue("@estado", estado);
                cmd.Parameters.AddWithValue("@id", idEnvio);
                if (nuevaFechaEst.HasValue)
                {
                    cmd.Parameters.AddWithValue("@nuevaFecha", nuevaFechaEst.Value);
                }
                cmd.ExecuteNonQuery();

                // 2. Actualizar el estado del pedido asociado
                using var cmdPedido = new MySqlCommand("UPDATE pedido SET estado = @estado WHERE id_pedido = @id_pedido", conn, transaction);
                cmdPedido.Parameters.AddWithValue("@estado", estado);
                cmdPedido.Parameters.AddWithValue("@id_pedido", envio.IdPedido);
                cmdPedido.ExecuteNonQuery();

                // 3. Si el estado es final (Entregado, etc.), liberar vehículo y chofer
                if (estado == "Entregado")
                {
                    // Liberar vehículo
                    using var cmdVeh = new MySqlCommand("UPDATE vehiculo SET estado_operativo = 'Disponible' WHERE id_vehiculo = @id_veh", conn, transaction);
                    cmdVeh.Parameters.AddWithValue("@id_veh", envio.IdVehiculo);
                    cmdVeh.ExecuteNonQuery();

                    // Liberar conductor
                    using var cmdCond = new MySqlCommand("UPDATE conductor SET disponible = TRUE WHERE id_conductor = @id_cond", conn, transaction);
                    cmdCond.Parameters.AddWithValue("@id_cond", envio.IdConductor);
                    cmdCond.ExecuteNonQuery();
                }
                else if (estado == "En Ruta")
                {
                    // Asegurar que el vehículo está marcado como 'En Ruta' y el conductor 'no disponible'
                    using var cmdVeh = new MySqlCommand("UPDATE vehiculo SET estado_operativo = 'En Ruta' WHERE id_vehiculo = @id_veh", conn, transaction);
                    cmdVeh.Parameters.AddWithValue("@id_veh", envio.IdVehiculo);
                    cmdVeh.ExecuteNonQuery();

                    using var cmdCond = new MySqlCommand("UPDATE conductor SET disponible = FALSE WHERE id_conductor = @id_cond", conn, transaction);
                    cmdCond.Parameters.AddWithValue("@id_cond", envio.IdConductor);
                    cmdCond.ExecuteNonQuery();
                }

                // 4. Insertar historial de actualización
                string histQuery = @"INSERT INTO historial_estado (id_envio, estado, fecha_actualizacion, descripcion, registrado_por) 
                                     VALUES (@id_envio, @estado, NOW(), @desc, @user)";
                using var cmdHist = new MySqlCommand(histQuery, conn, transaction);
                cmdHist.Parameters.AddWithValue("@id_envio", idEnvio);
                cmdHist.Parameters.AddWithValue("@estado", estado);
                cmdHist.Parameters.AddWithValue("@desc", desc);
                cmdHist.Parameters.AddWithValue("@user", registradoPor);
                cmdHist.ExecuteNonQuery();

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        public List<HistorialEstado> GetHistorialEnvio(int idEnvio)
        {
            var list = new List<HistorialEstado>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand(
                "SELECT * FROM historial_estado WHERE id_envio = @id ORDER BY fecha_actualizacion DESC", 
                conn);
            cmd.Parameters.AddWithValue("@id", idEnvio);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new HistorialEstado
                {
                    IdHistorial = reader.GetInt32("id_historial"),
                    IdEnvio = reader.GetInt32("id_envio"),
                    Estado = reader.GetString("estado"),
                    FechaActualizacion = reader.GetDateTime("fecha_actualizacion"),
                    Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString("descripcion"),
                    RegistradoPor = reader.IsDBNull(reader.GetOrdinal("registrado_por")) ? null : reader.GetString("registrado_por")
                });
            }
            return list;
        }

        // ==========================================
        // 7. GENERACIÓN DE REPORTES LOGÍSTICOS (CU-08)
        // ==========================================
        public List<Dictionary<string, object>> RunReport(string tipo, DateTime inicio, DateTime fin)
        {
            var list = new List<Dictionary<string, object>>();
            using var conn = GetConnection();
            conn.Open();

            string query = "";
            if (tipo == "Entregas")
            {
                query = @"SELECT e.id_envio AS 'ID Envío', p.id_pedido AS 'ID Pedido', 
                         CONCAT(c.nombre, ' ', c.apellido) AS 'Cliente',
                         p.tipo_carga AS 'Tipo Carga', p.peso_kg AS 'Peso (kg)',
                         v.placa AS 'Placa Vehículo', 
                         CONCAT(cond.nombre, ' ', cond.apellido) AS 'Conductor',
                         e.fecha_asignacion AS 'Fecha Asignado', 
                         e.fecha_entrega_real AS 'Fecha Entregado',
                         e.estado_envio AS 'Estado'
                         FROM envio e
                         INNER JOIN pedido p ON e.id_pedido = p.id_pedido
                         INNER JOIN cliente c ON p.id_cliente = c.id_cliente
                         INNER JOIN vehiculo v ON e.id_vehiculo = v.id_vehiculo
                         INNER JOIN conductor cond ON e.id_conductor = cond.id_conductor
                         WHERE e.fecha_asignacion BETWEEN @inicio AND @fin
                         ORDER BY e.fecha_asignacion DESC";
            }
            else if (tipo == "Incidencias")
            {
                query = @"SELECT h.id_historial AS 'ID Historial', e.id_envio AS 'ID Envío',
                         e.estado_envio AS 'Estado Envío',
                         h.fecha_actualizacion AS 'Fecha Ocurrida',
                         h.descripcion AS 'Detalle Incidencia', 
                         h.registrado_por AS 'Reportado Por'
                         FROM historial_estado h
                         INNER JOIN envio e ON h.id_envio = e.id_envio
                         WHERE h.estado = 'Con Incidencia' 
                         AND h.fecha_actualizacion BETWEEN @inicio AND @fin
                         ORDER BY h.fecha_actualizacion DESC";
            }
            else if (tipo == "Flota")
            {
                query = @"SELECT v.id_vehiculo AS 'ID Vehículo', v.placa AS 'Placa', 
                         v.tipo AS 'Tipo', v.capacidad_carga_kg AS 'Capacidad (kg)',
                         v.estado_operativo AS 'Estado', v.marca AS 'Marca', v.modelo AS 'Modelo',
                         COUNT(e.id_envio) AS 'Total Envíos Realizados'
                         FROM vehiculo v
                         LEFT JOIN envio e ON v.id_vehiculo = e.id_vehiculo
                         GROUP BY v.id_vehiculo, v.placa, v.tipo, v.capacidad_carga_kg, v.estado_operativo, v.marca, v.modelo
                         ORDER BY COUNT(e.id_envio) DESC";
            }
            else // "Pedidos"
            {
                query = @"SELECT p.id_pedido AS 'ID Pedido', 
                         CONCAT(c.nombre, ' ', c.apellido) AS 'Cliente',
                         p.fecha_solicitud AS 'Fecha Solicitud', p.direccion_entrega AS 'Dirección Entrega',
                         p.tipo_carga AS 'Tipo Carga', p.peso_kg AS 'Peso (kg)',
                         p.prioridad AS 'Prioridad', p.estado AS 'Estado'
                         FROM pedido p
                         INNER JOIN cliente c ON p.id_cliente = c.id_cliente
                         WHERE p.fecha_solicitud BETWEEN @inicio AND @fin
                         ORDER BY p.fecha_solicitud DESC";
            }

            using var cmd = new MySqlCommand(query, conn);
            // Asegurar fecha inicio sea las 00:00:00 y fecha fin las 23:59:59
            cmd.Parameters.AddWithValue("@inicio", inicio.Date);
            cmd.Parameters.AddWithValue("@fin", fin.Date.AddDays(1).AddSeconds(-1));

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.GetValue(i);
                }
                list.Add(row);
            }
            return list;
        }
    }
}
