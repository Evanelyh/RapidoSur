using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RapidoSur.Services;

namespace RapidoSur.Controllers
{
    public class DriverController : Controller
    {
        private readonly DbService _db;

        public DriverController(DbService db)
        {
            _db = db;
        }

        private bool CheckAuth()
        {
            return HttpContext.Session.GetString("IsLoggedIn") == "true" && 
                   HttpContext.Session.GetString("UserType") == "Conductor";
        }

        // 1. DASHBOARD DEL CONDUCTOR (CU-05)
        public IActionResult Dashboard()
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");

            int conductorId = HttpContext.Session.GetInt32("UserId") ?? 0;
            
            // Traer envíos asignados o en ruta para este conductor
            var enviosActivos = _db.GetEnvios(null, conductorId);
            // Filtrar solo los que no están entregados para la vista de trabajo diario
            var filtrados = enviosActivos.FindAll(e => e.EstadoEnvio != "Entregado");

            ViewBag.EnviosActivos = filtrados;
            ViewBag.HistorialGeneral = enviosActivos.FindAll(e => e.EstadoEnvio == "Entregado"); // Historial de entregas hechas

            return View();
        }

        // 2. ACTUALIZAR ESTADO DE ENVÍO (CU-05)
        [HttpPost]
        public IActionResult UpdateStatus(int idEnvio, string estado, string? descripcion)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");

            string conductorNombre = HttpContext.Session.GetString("UserName") ?? "Conductor";
            string descActual = string.IsNullOrEmpty(descripcion) 
                ? $"El conductor cambió el estado del envío a: {estado}." 
                : descripcion;

            bool success = _db.UpdateEnvioEstado(idEnvio, estado, descActual, conductorNombre);
            if (success)
            {
                TempData["SuccessMessage"] = $"Estado del envío actualizado a '{estado}' correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error al actualizar el estado del envío en la base de datos.";
            }

            return RedirectToAction("Dashboard");
        }

        // 3. REGISTRAR INCIDENCIA (CU-07)
        [HttpPost]
        public IActionResult ReportIncident(int idEnvio, string tipoIncidencia, string descripcion)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");

            if (string.IsNullOrEmpty(descripcion))
            {
                TempData["ErrorMessage"] = "La descripción del incidente es obligatoria.";
                return RedirectToAction("Dashboard");
            }

            string conductorNombre = HttpContext.Session.GetString("UserName") ?? "Conductor";
            string descCompleta = $"Incidencia reportada ({tipoIncidencia}): {descripcion}";

            bool success = _db.UpdateEnvioEstado(idEnvio, "Con Incidencia", descCompleta, conductorNombre);
            if (success)
            {
                TempData["SuccessMessage"] = $"Incidencia del tipo '{tipoIncidencia}' registrada y notificada al operador.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error al registrar la incidencia en la base de datos.";
            }

            return RedirectToAction("Dashboard");
        }

        // 4. REPROGRAMAR ENTREGA (CU-06)
        [HttpPost]
        public IActionResult Reprogram(int idEnvio, DateTime nuevaFecha, string motivo)
        {
            if (!CheckAuth()) return RedirectToAction("Login", "Home");

            if (string.IsNullOrEmpty(motivo))
            {
                TempData["ErrorMessage"] = "El motivo de la reprogramación es obligatorio.";
                return RedirectToAction("Dashboard");
            }

            if (nuevaFecha.Date <= DateTime.Now.Date)
            {
                TempData["ErrorMessage"] = "La nueva fecha de entrega debe ser posterior a la fecha actual.";
                return RedirectToAction("Dashboard");
            }

            string conductorNombre = HttpContext.Session.GetString("UserName") ?? "Conductor";
            string descReprogram = $"Entrega reprogramada para el {nuevaFecha.ToString("dd/MM/yyyy")}. Motivo: {motivo}";

            bool success = _db.UpdateEnvioEstado(idEnvio, "Reprogramado", descReprogram, conductorNombre, nuevaFecha.Date);
            if (success)
            {
                TempData["SuccessMessage"] = $"Entrega reprogramada correctamente para el {nuevaFecha.ToString("dd/MM/yyyy")}.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error al reprogramar la entrega en la base de datos.";
            }

            return RedirectToAction("Dashboard");
        }
    }
}
