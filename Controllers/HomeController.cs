using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROJECT_INSUMOS.Datos;
using PROJECT_INSUMOS.Models;
using ProjectInsumosCC___Migracion.Datos;
using ProjectInsumosCC___Migracion.Models;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

using System.Security.Claims;

namespace PROJECT_INSUMOS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        UsuarioModel usuarioModel = new UsuarioModel();
        TransaccionesDatos _usuarioDatos = new TransaccionesDatos();

        // TransaccionesDatos _usuarioDatos = new TransaccionesDatos();
        // CAMBIO DE AUTENTICACIÓN: LOGIN VÍA API DE USUARIOS LABORATORIOS PORTUGAL
        // Antes el login se validaba contra la base de datos local
        // (GestionUsuariosDatos.IniiciarSesion_datos). Ahora se valida contra
        // la API externa que devuelve un token JWT para autenticar las funciones.

        // Fábrica de clientes HTTP (registrada en Program.cs con AddHttpClient)
        // para realizar las llamadas a la API de usuarios.
        private readonly IHttpClientFactory _httpClientFactory;

        // URL del endpoint de autenticación (POST) de la API de usuarios.
        private const string ApiLoginUrl = "https://usuariosapi.laboratoriosportugal.com/api/auth/login";

        // Código con el que la API identifica esta aplicación.
        private const string ApiCodAplicacion = "PROTCC";

        // Tiempo de vida de la sesión: 4 horas. Luego de ese tiempo el usuario
        // debe volver a logearse para obtener un token nuevo.
        private const int HorasSesion = 1;

        // Datos auxiliares que siguen usándose en otras pantallas del sistema
        // (gestión de usuarios, indicadores, etc.) contra la base de datos local.
        //IndicadoresDatos indicadoresDatos = new IndicadoresDatos();

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        //login
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

      
        //prueba privacy
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        //prueba privacy
        [Authorize]
        public IActionResult Informacion()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // INICIO DE SESIÓN VÍA API DE USUARIOS
        // Flujo nuevo:
        //   1. Se recibe usuario y contraseña desde la vista (mismo AJAX de siempre).
        //   2. Se llama a la API: POST https://usuariosapi.laboratoriosportugal.com/api/auth/login
        //      enviando { Usuario, Password, CodAplicacion: "KARDEXPOR" }.
        //   3. Si exito = true: se crea la cookie de autenticación con los claims
        //      (nombre, correo, DNI y TOKEN JWT) válida por 4 horas. Al expirar,
        //      el middleware redirige al login para obtener un token nuevo y
        //      regresa al usuario a la página donde estaba (ReturnUrl).
        //   4. Si exito = false: se devuelven los mensajes de error de la API
        //      para mostrarlos en pantalla.
        [AllowAnonymous]
        public async Task<IActionResult> IniiciarSesion_Controller(string user, string pass)
        {


            try
            {
                // PASO 1: Armar el cuerpo (body) que espera la API de usuarios.
                var peticionLogin = new ApiLoginRequest
                {
                    Usuario = user,
                    Password = pass,
                    CodAplicacion = ApiCodAplicacion
                };

                // PASO 2: Llamar a la API (POST) y deserializar la respuesta JSON.
                var clienteHttp = _httpClientFactory.CreateClient();
                var respuestaHttp = await clienteHttp.PostAsJsonAsync(ApiLoginUrl, peticionLogin);
                var respuestaApi = await respuestaHttp.Content.ReadFromJsonAsync<ApiLoginResponse>();

                // PASO 3: Validar la respuesta de la API.
                bool loginCorrecto = respuestaApi != null
                                     && respuestaApi.exito
                                     && respuestaApi.datos != null
                                     && !string.IsNullOrEmpty(respuestaApi.datos.token);

                if (loginCorrecto)
                {
                    // Datos del usuario devueltos por la API
                    var datosApi = respuestaApi?.datos;
                    datosApi.usuario = user; // Guardar el nombre de usuario para la sincronización

                    // PASO 4: Sincronizar el usuario con la base de datos local.
                    // IMPORTANTE: La sincronización se realiza SIEMPRE que las credenciales
                    // sean correctas, independientemente de si tiene funcionalidades o no.
                    // Esto garantiza que el usuario quede registrado en BD aunque no tenga
                    // acceso a ninguna pantalla del sistema.
                    var sincro = SincronizarUsuario(datosApi);

                    // PASO 5: Validar si el usuario tiene funcionalidades asignadas.
                    // Si la lista viene vacía o nula, NO se le permite el ingreso al sistema,
                    // pero ya quedó registrado en BD (paso anterior).
                    bool tieneFuncionalidades = datosApi?.funcionalidades != null
                                               && datosApi.funcionalidades.Count > 0;

                    if (!tieneFuncionalidades)
                    {
                        // El usuario fue sincronizado en BD pero NO tiene acceso a ninguna
                        // pantalla del sistema. Se devuelve exito=false para que la vista
                        // muestre el mensaje sin crear sesión.
                        return Json(new
                        {
                            exito = false,
                            mensaje = "Su usuario no tiene funcionalidades asignadas en este sistema. Contacte al administrador para solicitar acceso."
                        });
                    }

                    // PASO 6: Crear los claims de la sesión con los datos de la API.
                    // El TOKEN JWT queda guardado como claim para poder usarlo luego
                    // en las llamadas a los demás servicios (Authorization: Bearer ...).
                    var claims = new List<Claim>
                    {
                        // Nombre completo: nombres + apellidos
                        new Claim(ClaimTypes.Name, $"{datosApi?.nombres} {datosApi?.apellidos}".Trim()),
                        new Claim(ClaimTypes.Email, datosApi?.correo ?? string.Empty),
                        // DNI de la persona logeada (se usa en todas las funciones AJAX)
                        new Claim("Dni", datosApi?.dni ?? string.Empty),
                        // Token JWT entregado por la API (vence junto con la sesión de 4 horas)
                        new Claim("Token", datosApi?.token ?? string.Empty)
                    };

                    // PASO 7: Agregar cada funcionalidad como un claim individual.
                    // Formato del claim "funcionalidad": "idFuncionalidad"
                    // En el Layout se usa HasFuncionalidad(id) para decidir si mostrar
                    // o no cada menú/submenú. Al agregar una nueva página, se debe:
                    //   1. Agregar la funcionalidad en la API de usuarios (con su idFuncionalidad).
                    //   2. En el Layout, envolver el elemento de menú con:
                    //      @if (HasFuncionalidad(ID_DE_LA_FUNCIONALIDAD)) { ... }
                    //   3. Asignar esa funcionalidad al usuario en la configuración de la API.
                    foreach (var func in datosApi?.funcionalidades ?? new List<ApiFuncionalidad>())
                    {
                        // Guardamos el id de cada funcionalidad para consulta rápida en el Layout
                        claims.Add(new Claim("funcionalidad", func.idFuncionalidad.ToString()));
                    }

                    // PASO 8: Crear la identidad y firmar la sesión (cookie de autenticación).
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                        new AuthenticationProperties
                        {
                            IsPersistent = true,
                            // La sesión dura 4 horas; también está configurado en Program.cs.
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(HorasSesion)
                        });

                    // PASO 9: Devolver a la vista los datos para localStorage.
                    // La vista guardará: user, userid (DNI), token y funcionalidades.
                    var mensajeBienvenida = "";

                    if (sincro != null && !string.IsNullOrWhiteSpace(sincro.Mensaje))
                    {
                        mensajeBienvenida = sincro.Mensaje;
                    }
                    return Json(new
                    {
                        exito = true,
                        mensaje = mensajeBienvenida,
                        dni = datosApi.dni,
                        nombres = datosApi.nombres,
                        apellidos = datosApi.apellidos,
                        correo = datosApi.correo,
                        token = datosApi.token,
                        funcionalidades = datosApi.funcionalidades
                    });
                }
                else
                {
                    // LOGIN RECHAZADO POR LA API: se muestran sus mensajes de error.
                    // El campo "mensaje" puede variar y "errores[]" trae el detalle.

                    // Mensaje principal de la API (o uno genérico si no responde nada útil)
                    var mensajeError = !string.IsNullOrEmpty(respuestaApi?.mensaje)
                        ? respuestaApi.mensaje
                        : "Ocurrió un error al intentar iniciar sesión";

                    // Si la API adjuntó errores detallados, se agregan al final del mensaje
                    if (respuestaApi?.errores != null && respuestaApi.errores.Any())
                    {
                        mensajeError += " " + string.Join(" ", respuestaApi.errores);
                    }

                    return Json(new
                    {
                        exito = false,
                        mensaje = mensajeError
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en IniiciarSesion_Controller: " + ex.Message);
            }
        }

        /// <summary>
        /// Sincroniza los datos del usuario obtenidos desde la API de Active Directory
        /// con los registros almacenados en la base de datos.
        /// </summary>
        /// <param name="datosApi">Datos del usuario devueltos por la API.</param>
        /// <returns>Resultado de la sincronización del usuario.</returns>
        private UsuarioModel SincronizarUsuario(ApiLoginDatos datosApi)
        {
            var model = new UsuarioModel
            {
                Usuario = datosApi.usuario,
                NroDocumento = datosApi.dni,
                Nombres = datosApi.nombres,
                Apellidos = datosApi.apellidos,
                Correo = datosApi.correo,
                Area = datosApi.area
            };

            return _usuarioDatos.CC_Usuarios_Sincronizar_Datos(model);
        }
        // CIERRE DE SESIÓN
        // Elimina la cookie de autenticación y redirige al login. Al volver a iniciar sesión la API entregará un token nuevo.
        public async Task<IActionResult> CerrarSesion_Controller(int xValor)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public IActionResult CC_UsuariosObtener_Controller(string dni)
        {
            var datos = _usuarioDatos.CC_UsuariosObtener_Datos(dni);
            return Json(datos);
        }
    }

}
