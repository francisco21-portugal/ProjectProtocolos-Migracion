using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROJECT_INSUMOS.Datos;
using PROJECT_INSUMOS.Models;

namespace PROJECT_INSUMOS.Controllers
{
    public class AuditTrailController : Controller
    {
        private readonly AuditDatos _auditDatos = new AuditDatos();

        /// <summary>
        /// Vista para el historial y movimientos de usuarios
        /// </summary>
        public IActionResult AuditUsuarios()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ObtenerAuditUsuariosTransaccionesStreaming_Controller(DateTime? fechaDesde = null, DateTime? fechaHasta = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var registros = new List<AuditModel>();
                await foreach (var registro in _auditDatos.ObtenerAuditUsuariosTransaccionesStreaming_Datos(fechaDesde, fechaHasta).WithCancellation(cancellationToken))
                {
                    registros.Add(registro);
                }
                return Ok(new
                {
                    data = registros
                });
            }
            catch (Exception ex)
            {
                // El detalle técnico queda solo en la consola del servidor: no se expone
                // información de la base de datos al navegador.
                System.Diagnostics.Debug.WriteLine("AuditUsuarios: " + ex);

                return Json(new
                {
                    data = new List<AuditModel>(),
                    error = "No se pudieron obtener los registros de auditoría."
                });
            }
        }
    }
}
