using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RapidoSur.Models;
using RapidoSur.Services;

namespace RapidoSur.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbService _db;

        public HomeController(DbService db)
        {
            _db = db;
        }

        // 1. PORTAL PÚBLICO DE RASTREO (CU-02)
        public IActionResult Index(int? idPedido)
        {
            if (idPedido.HasValue)
            {
                ViewBag.SearchPerformed = true;
                ViewBag.SearchId = idPedido.Value;
                
                var envio = _db.GetEnvioByPedido(idPedido.Value);
                if (envio != null)
                {
                    ViewBag.Envio = envio;
                    ViewBag.Historial = _db.GetHistorialEnvio(envio.IdEnvio);
                }
                else
                {
                    // Quizá el pedido existe pero aún no se le ha asignado envío
                    var pedido = _db.GetPedido(idPedido.Value);
                    if (pedido != null)
                    {
                        ViewBag.Pedido = pedido;
                    }
                    else
                    {
                        ViewBag.NotFound = true;
                    }
                }
            }
            else
            {
                ViewBag.SearchPerformed = false;
            }
            return View();
        }

        // 2. LOGIN (CU-10)
        [HttpGet]
        public IActionResult Login()
        {
            // Cargar conductores para el inicio de sesión rápido de conductores
            ViewBag.Conductores = _db.GetConductores();
            return View();
        }

        [HttpPost]
        public IActionResult Login(string userType, string? usuario, string? clave, int? idConductor)
        {
            if (userType == "Operador")
            {
                if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(clave))
                {
                    ModelState.AddModelError("", "Por favor ingrese usuario y contraseña.");
                    ViewBag.Conductores = _db.GetConductores();
                    return View();
                }

                var op = _db.Login(usuario, clave);
                if (op != null)
                {
                    HttpContext.Session.SetString("IsLoggedIn", "true");
                    HttpContext.Session.SetString("UserType", "Operador");
                    HttpContext.Session.SetInt32("UserId", op.IdOperador);
                    HttpContext.Session.SetString("UserName", op.NombreCompleto);
                    HttpContext.Session.SetString("UserRole", op.Rol);
                    HttpContext.Session.SetString("UserEmail", op.Correo ?? "");
                    HttpContext.Session.SetString("Username", op.Usuario);

                    return RedirectToAction("Dashboard", "Operator");
                }
                else
                {
                    ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                }
            }
            else if (userType == "Conductor")
            {
                if (!idConductor.HasValue)
                {
                    ModelState.AddModelError("", "Por favor seleccione un conductor.");
                    ViewBag.Conductores = _db.GetConductores();
                    return View();
                }

                var conductores = _db.GetConductores();
                var cond = conductores.Find(c => c.IdConductor == idConductor.Value);
                if (cond != null)
                {
                    HttpContext.Session.SetString("IsLoggedIn", "true");
                    HttpContext.Session.SetString("UserType", "Conductor");
                    HttpContext.Session.SetInt32("UserId", cond.IdConductor);
                    HttpContext.Session.SetString("UserName", $"{cond.Nombre} {cond.Apellido}");
                    HttpContext.Session.SetString("UserRole", "Conductor");
                    HttpContext.Session.SetString("UserLicence", cond.NumLicencia);

                    return RedirectToAction("Dashboard", "Driver");
                }
            }

            ViewBag.Conductores = _db.GetConductores();
            return View();
        }

        // 3. MI PERFIL Y CONFIGURACIÓN
        [HttpGet]
        public IActionResult Profile()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Profile(string nombre, string apellido, string correo, string? claveActual, string? nuevaClave, string? confirmarClave)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            string userType = HttpContext.Session.GetString("UserType") ?? "";
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userType == "Operador")
            {
                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido) || string.IsNullOrEmpty(correo))
                {
                    ViewBag.Error = "Los campos Nombre, Apellido y Correo son obligatorios.";
                    return View();
                }

                string? passwordToUpdate = null;
                if (!string.IsNullOrEmpty(nuevaClave))
                {
                    if (nuevaClave != confirmarClave)
                    {
                        ViewBag.Error = "La nueva contraseña y su confirmación no coinciden.";
                        return View();
                    }
                    passwordToUpdate = nuevaClave;
                }

                bool success = _db.UpdateOperadorProfile(userId, nombre, apellido, correo, passwordToUpdate);
                if (success)
                {
                    HttpContext.Session.SetString("UserName", $"{nombre} {apellido}");
                    HttpContext.Session.SetString("UserEmail", correo);
                    ViewBag.Success = "Perfil actualizado con éxito.";
                }
                else
                {
                    ViewBag.Error = "Error al actualizar el perfil en la base de datos.";
                }
            }
            else
            {
                // Conductores
                ViewBag.Error = "La edición de perfil para conductores no está habilitada en esta demo.";
            }

            return View();
        }

        // 4. LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
