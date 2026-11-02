using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROJECT_INSUMOS.Datos;
using PROJECT_INSUMOS.Models;
using ProjectInsumosCC___Migracion.Datos;
using ProjectInsumosCC___Migracion.Models;

namespace ProjectInsumosCC___Migracion.Controllers
{
    public class ControlCalidadController : Controller
    {

        TransaccionesDatos _transaccionesDatos = new TransaccionesDatos();
        [Authorize]
        public IActionResult frmBAHIA()
        {
            return View();
        }

        [Authorize]
        public IActionResult frmBEBES()
        {
            return View();
        }
        [Authorize]
        public IActionResult frmCOSMETICOS()
        {
            return View();
        }
        [Authorize]
        public IActionResult frmESTADO()
        {
            return View();
        }
        [Authorize]
        public IActionResult frmGALENICOS()
        {
            return View();
        }
        [Authorize]
        public IActionResult frmGENERICOS()
        {
            return View();
        }
        [Authorize]
        public IActionResult frmLINEACOMERCIALTD()
        {
            return View();
        }
        [Authorize]
        public IActionResult frmLINEACOMERCIALVM()
        {
            return View();
        }
        [Authorize]
        public IActionResult frmMAQUILA()
        {
            return View();
        }
        [Authorize]
        public IActionResult frmNATURALES()
        {
            return View();
        }
        [Authorize]
        public IActionResult frmVARIOS()
        {
            return View();
        }
        [Authorize]
        public IActionResult frmYUMAX()
        {
            return View();
        }


        [Authorize]
        [HttpGet]
        public IActionResult CC_ListarProductosPorCategoria_Controller(int idCategoria)
        {
            try
            {
                var datos = _transaccionesDatos.CC_ListarProductosPorCategoria(idCategoria);

                // Si no hay registros, devuelve 200 OK con data vacía.
                return Ok(new
                {
                    data = datos
                });
            }
            catch (Exception ex)
            {
                // Este bloque solo se ejecuta si existe un error real
                // de conexión, SQL o ejecución del procedimiento.
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    data = Array.Empty<ProductosModel>(),
                    codMensaje = 0,
                    mensaje = "Ocurrió un error al consultar los movimientos."
                });
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult CC_BuscarProtocolosPorProducto_Controller(string Codigo)
        {
            try
            {
                var datos = _transaccionesDatos.CC_BuscarProtocolosPorProducto(Codigo);

                // Si no hay registros, devuelve 200 OK con data vacía.
                return Ok(new
                {
                    data = datos
                });
            }
            catch (Exception ex)
            {
                // Este bloque solo se ejecuta si existe un error real
                // de conexión, SQL o ejecución del procedimiento.
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    data = Array.Empty<ProductosModel>(),
                    codMensaje = 0,
                    mensaje = "Ocurrió un error al consultar los protocolos."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> VerPDF_Controller(long IDproducto)
        {
            var producto = await _transaccionesDatos.ObtenerPDF(IDproducto);

            if (producto == null || producto.PDF == null)
                return NotFound("No se encontró el archivo PDF.");

            return File(producto.PDF, "application/pdf");
        }


    }
}
