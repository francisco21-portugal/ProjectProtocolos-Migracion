using Microsoft.Data.SqlClient;
using PROJECT_INSUMOS.Models;
using System.Data;

namespace PROJECT_INSUMOS.Datos
{
    public class AuditDatos
    {
        private readonly Conexion cn = new Conexion();

        public async IAsyncEnumerable<AuditModel> ObtenerAuditUsuariosTransaccionesStreaming_Datos(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            await using var conexion = new SqlConnection(cn.GetCadenaSQL());
            await using var cmd = new SqlCommand("usp_AuditTrailListarUsuarios", conexion)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Si no se recibe rango, se envían null y el SP devuelve todo el histórico.
            cmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = fechaDesde.HasValue ? fechaDesde.Value.Date : DBNull.Value;

            // CORRECCIÓN: antes se mandaba con SqlDbType.Date, que trunca @FechaFin a la
            // medianoche (00:00:00) del día final, dejando fuera todos los registros de ese
            // día posteriores a medianoche. Ahora se manda el último instante del día para
            // que el rango incluya el día final completo.
            cmd.Parameters.Add("@FechaFin", SqlDbType.DateTime).Value = fechaHasta.HasValue? fechaHasta.Value.Date.AddDays(1).AddMilliseconds(-1): DBNull.Value;
            await conexion.OpenAsync();

            await using var rd = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            int idxIDAuditTrail = rd.GetOrdinal("IDAuditTrail");
            int idxIDusuario = rd.GetOrdinal("IDusuario");
            int idxUsuario = rd.GetOrdinal("Usuario");
            int idxTabla = rd.GetOrdinal("Tabla");
            int idxAccion = rd.GetOrdinal("Accion");
            int idxNombreCampo = rd.GetOrdinal("NombreCampo");
            int idxIDRegistro = rd.GetOrdinal("IDRegistro");
            int idxNroRegistro = rd.GetOrdinal("NroRegistro");
            int idxValorAnterior = rd.GetOrdinal("ValorAnterior");
            int idxValorNuevo = rd.GetOrdinal("ValorNuevo");
            int idxMotivoCambio = rd.GetOrdinal("MotivoCambio");
            int idxIP = rd.GetOrdinal("IP");
            int idxFechaRegistroAccion = rd.GetOrdinal("FechaRegistroAccion");

            while (await rd.ReadAsync())
            {
                DateTime? xFechaRegistroAccion = rd.IsDBNull(idxFechaRegistroAccion) ? null : rd.GetDateTime(idxFechaRegistroAccion);

                yield return new AuditModel
                {
                    IDAuditTrail = rd.IsDBNull(idxIDAuditTrail) ? 0 : rd.GetInt64(idxIDAuditTrail),
                    IDusuario = rd.IsDBNull(idxIDusuario) ? null : rd.GetString(idxIDusuario),
                    Usuario = rd.IsDBNull(idxUsuario) ? null : rd.GetString(idxUsuario),
                    Tabla = rd.IsDBNull(idxTabla) ? null : rd.GetString(idxTabla),
                    Accion = rd.IsDBNull(idxAccion) ? null : rd.GetString(idxAccion),
                    NombreCampo = rd.IsDBNull(idxNombreCampo) ? null : rd.GetString(idxNombreCampo),
                    IDRegistro = rd.IsDBNull(idxIDRegistro) ? null : rd.GetString(idxIDRegistro),
                    NroRegistro = rd.IsDBNull(idxNroRegistro) ? null : rd.GetString(idxNroRegistro),
                    ValorAnterior = rd.IsDBNull(idxValorAnterior) ? null : rd.GetString(idxValorAnterior),
                    ValorNuevo = rd.IsDBNull(idxValorNuevo) ? null : rd.GetString(idxValorNuevo),
                    MotivoCambio = rd.IsDBNull(idxMotivoCambio) ? null : rd.GetString(idxMotivoCambio),
                    IP = rd.IsDBNull(idxIP) ? null : rd.GetString(idxIP),
                    FechaRegistroAccion = xFechaRegistroAccion
                };
            }
        }


    }
}
