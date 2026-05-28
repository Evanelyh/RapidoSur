using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RapidoSur.Models;
using RapidoSur.Services;

namespace RapidoSur.Controllers
{
    public class OperatorController : Controller
    {
        private readonly DbService _db;

        public OperatorController(DbService db)
        {
            _db = db;
        }

        private bool CheckAuth()
        {
            return HttpContext.Session.GetString("IsLoggedIn") == "true" && 
                   HttpContext.Session.GetString("UserType") == "Operador";
        }

        // 1. DASHBOARD PRINCIPAL (Requerimiento 5 / CU-05 / CU-08)
        public IActionResult Dashboard()
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");

            var pedidos = _db.GetPedidos();
            var vehiculos = _db.GetVehiculos();
            var envios = _db.GetEnvios();

            int pedidosRecibidos = pedidos.Count;
            int vehiculosEnRuta = vehiculos.FindAll(v => v.EstadoOperativo == "En Ruta").Count;
            int entregasCompletadas = pedidos.FindAll(p => p.Estado == "Entregado").Count;
            
            // Retrasos o Incidencias
            int alertasRetrasos = envios.FindAll(e => e.EstadoEnvio == "Con Incidencia" || e.EstadoEnvio == "Reprogramado").Count;

            var model = new DashboardViewModel
            {
                PedidosRecibidos = pedidosRecibidos,
                VehiculosEnRuta = vehiculosEnRuta,
                EntregasCompletadas = entregasCompletadas,
                AlertasRetrasos = alertasRetrasos,
                ÚltimosPedidos = pedidos.Count > 10 ? pedidos.GetRange(0, 10) : pedidos
            };

            return View(model);
        }

        // 2. NUEVO PEDIDO (Requerimiento 1 / CU-01)
        [HttpGet]
        public IActionResult NewOrder()
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");

            ViewBag.Clientes = _db.GetClientes();
            return View();
        }

        [HttpPost]
        public IActionResult NewOrder(string clientType, int? idCliente, 
                                      string? nombre, string? apellido, string? telefono, string? correo, string? direccion,
                                      string direccionEntrega, string tipoCarga, decimal pesoKg, string prioridad, string? observaciones)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");

            int clienteIdActual = 0;

            if (clientType == "Existente")
            {
                if (!idCliente.HasValue)
                {
                    ModelState.AddModelError("", "Por favor seleccione un cliente existente.");
                    ViewBag.Clientes = _db.GetClientes();
                    return View();
                }
                clienteIdActual = idCliente.Value;
            }
            else // "Nuevo"
            {
                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido))
                {
                    ModelState.AddModelError("", "El nombre y apellido son obligatorios para un nuevo cliente.");
                    ViewBag.Clientes = _db.GetClientes();
                    return View();
                }

                var nuevoCliente = new Cliente
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    Telefono = telefono,
                    Correo = correo,
                    Direccion = direccion
                };

                try
                {
                    clienteIdActual = _db.AddCliente(nuevoCliente);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al guardar cliente: {ex.Message}");
                    ViewBag.Clientes = _db.GetClientes();
                    return View();
                }
            }

            // Crear el Pedido
            if (string.IsNullOrEmpty(direccionEntrega) || pesoKg <= 0)
            {
                ModelState.AddModelError("", "La dirección de entrega y el peso de la carga son obligatorios.");
                ViewBag.Clientes = _db.GetClientes();
                return View();
            }

            var nuevoPedido = new Pedido
            {
                IdCliente = clienteIdActual,
                DireccionEntrega = direccionEntrega,
                TipoCarga = tipoCarga,
                PesoKg = pesoKg,
                Prioridad = prioridad,
                Observaciones = observaciones
            };

            try
            {
                _db.AddPedido(nuevoPedido);
                TempData["SuccessMessage"] = "¡Pedido registrado con éxito! Estado: Pendiente.";
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al registrar el pedido: {ex.Message}");
                ViewBag.Clientes = _db.GetClientes();
                return View();
            }
        }

        // 3. ASIGNACIÓN DE VEHÍCULOS (Requerimiento 2 / CU-03 / CU-04)
        [HttpGet]
        public IActionResult AssignVehicle()
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");

            var model = new AsignacionViewModel
            {
                PedidosPendientes = _db.GetPedidos("Pendiente"),
                VehiculosDisponibles = _db.GetVehiculos("Disponible"),
                ConductoresDisponibles = _db.GetConductores(true)
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AssignVehicle(int idPedido, int idVehiculo, int idConductor, string? rutaDescripcion, DateTime? fechaEntregaEstimada)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");

            int operadorId = HttpContext.Session.GetInt32("UserId") ?? 1;

            var pedido = _db.GetPedido(idPedido);
            var vehiculo = _db.GetVehiculos().Find(v => v.IdVehiculo == idVehiculo);
            var conductor = _db.GetConductores().Find(c => c.IdConductor == idConductor);

            if (pedido == null || vehiculo == null || conductor == null)
            {
                TempData["ErrorMessage"] = "Error al despachar: Pedido, Vehículo o Conductor no válidos.";
                return RedirectToAction("AssignVehicle");
            }

            // Regla de Negocio: Verificar capacidad de carga (CU-04)
            if (vehiculo.CapacidadCargaKg < pedido.PesoKg)
            {
                TempData["ErrorMessage"] = $"¡Alerta de Capacidad! El camión {vehiculo.Placa} ({vehiculo.CapacidadCargaKg}kg) no soporta el peso del pedido ({pedido.PesoKg}kg).";
                return RedirectToAction("AssignVehicle");
            }

            var nuevoEnvio = new Envio
            {
                IdPedido = idPedido,
                IdVehiculo = idVehiculo,
                IdConductor = idConductor,
                IdOperador = operadorId,
                FechaEntregaEstimada = fechaEntregaEstimada ?? DateTime.Now.AddDays(1).Date,
                RutaDescripcion = string.IsNullOrEmpty(rutaDescripcion) ? $"Entrega directa a {pedido.DireccionEntrega}" : rutaDescripcion,
                VehiculoPlaca = vehiculo.Placa,
                ConductorNombre = $"{conductor.Nombre} {conductor.Apellido}"
            };

            try
            {
                _db.AddEnvio(nuevoEnvio);
                TempData["SuccessMessage"] = $"¡Orden de Despacho generada con éxito! Envío asignado al vehículo {vehiculo.Placa} y conductor {conductor.Nombre}.";
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error en la base de datos al realizar la asignación: {ex.Message}";
                return RedirectToAction("AssignVehicle");
            }
        }

        // 4. REPORTES DE DESEMPEÑO LOGÍSTICO (Requerimiento 5 / CU-08)
        [HttpGet]
        public IActionResult Reports()
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");

            // Solo administradores pueden ver reportes (Regla de negocio: CU-08)
            string role = HttpContext.Session.GetString("UserRole") ?? "";
            if (role != "Administrador")
            {
                TempData["ErrorMessage"] = "Acceso denegado: El módulo de reportes está restringido a Administradores.";
                return RedirectToAction("Dashboard");
            }

            var model = new ReporteViewModel
            {
                FechaInicio = DateTime.Now.AddDays(-30),
                FechaFin = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Reports(string tipoReporte, DateTime fechaInicio, DateTime fechaFin)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");

            string role = HttpContext.Session.GetString("UserRole") ?? "";
            if (role != "Administrador")
            {
                TempData["ErrorMessage"] = "Acceso denegado: Solo Administradores pueden generar reportes.";
                return RedirectToAction("Dashboard");
            }

            var model = new ReporteViewModel
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                TipoReporte = tipoReporte
            };

            try
            {
                model.Resultados = _db.RunReport(tipoReporte, fechaInicio, fechaFin);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al generar reporte: {ex.Message}";
            }

            return View(model);
        }
    }
}
