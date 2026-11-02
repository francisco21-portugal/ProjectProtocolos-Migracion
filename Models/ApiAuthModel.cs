//using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROJECT_INSUMOS.Models
{
    // MODELOS PARA LA API DE USUARIOS DE LABORATORIOS PORTUGAL
    // Endpoint de login (POST): https://usuariosapi.laboratoriosportugal.com/api/auth/login
    // Estos modelos mapean el JSON que envía y devuelve la API.

    /// <summary>
    /// Cuerpo (body) que se envía a la API para autenticar al usuario.
    /// </summary>
    public class ApiLoginRequest
    {
        public string? Usuario { get; set; }
        public string? Password { get; set; }
        // Código con el que la API identifica esta aplicación (PROTCC)
        public string CodAplicacion { get; set; } = "PROTCC";
    }

    /// <summary>
    /// Respuesta completa que devuelve la API al intentar iniciar sesión.
    /// Si "exito" es true, "datos" contiene la información del usuario y el token JWT.
    /// Si "exito" es false, se debe mostrar "mensaje" y/o la lista "errores".
    /// </summary>
    public class ApiLoginResponse
    {
        public ApiLoginDatos? datos { get; set; }
        public bool exito { get; set; }
        public double tiempoEjecucion { get; set; }
        public string? mensaje { get; set; }
        public List<string>? errores { get; set; }
    }

    /// <summary>
    /// Datos del usuario autenticado que devuelve la API cuando exito = true.
    /// </summary>
    public class ApiLoginDatos
    {
        public string? usuario { get; set; }
        public string? dni { get; set; }
        public string? nombres { get; set; }
        public string? apellidos { get; set; }
        public string? correo { get; set; }
        public string? area { get; set; }
        // Token JWT que se usará para autenticar las llamadas a los demás servicios
        public string? token { get; set; }
        // Funcionalidades (menús / submenús) a las que el usuario tiene acceso
        public List<ApiFuncionalidad>? funcionalidades { get; set; }
    }

    /// <summary>
    /// Representa una funcionalidad (menú o submenú) devuelta por la API.
    /// NOTA: Por ahora no se usa para filtrar el menú (se muestran todos),
    /// pero queda listo para cuando se haga el mapeo página <-> funcionalidad.
    /// </summary>
    public class ApiFuncionalidad
    {
        public int idFuncionalidad { get; set; }
        public string? nombre { get; set; }
        public int idFuncionalidadPadre { get; set; }
        public string? ruta { get; set; }
        public string? codPlataforma { get; set; }
        public bool estado { get; set; }
    }


    /// <summary>
    /// Representa la información del usuario obtenida desde Active Directory
    /// y utilizada para sincronizar los datos con el sistema.
    /// </summary>
    public class UsuarioModel
    {
        public string? Usuario { get; set; }
        public string? NroDocumento { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Correo { get; set; }
        public string ?Area { get; set; }
        public string? Mensaje { get; set; }
        public string? CodMensaje { get; set; }
        public bool? Estado { get; set; }
        public DateTime? FechaIngreso{ get; set; }

    }
}
