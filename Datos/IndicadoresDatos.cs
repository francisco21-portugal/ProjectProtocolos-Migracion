using System.Data;
using Microsoft.Data.SqlClient;
using PROJECT_INSUMOS.Models;

namespace PROJECT_INSUMOS.Datos
{
    public class IndicadoresDatos
    {

         IndicadoresModel indicadores = new IndicadoresModel();
       // UsuarioSubAreaModel _usuarioSubArea = new UsuarioSubAreaModel();
        Conexion cn = new Conexion();
        public IndicadoresModel UsuariosIndicadoresDatos()
        {
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("uspUsuarioIndicadores", conexion);

                cmd.CommandType = CommandType.StoredProcedure;
                var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    indicadores.cantUsuarios = Convert.ToInt32(rd["CantUsuarios"].ToString());
                    indicadores.usuariosActivos = Convert.ToInt32(rd["UsuariosActivos"].ToString());
                    indicadores.usuariosInactivos = Convert.ToInt32(rd["UsuariosInactivos"].ToString());
                    indicadores.cantidadArea = Convert.ToInt32(rd["CantidadArea"].ToString());
                    indicadores.cantidadSubArea = Convert.ToInt32(rd["CantidadSubArea"].ToString());
                }
            }
            return indicadores;
        }
    }
}
