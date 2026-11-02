using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PROJECT_INSUMOS.Datos;
using PROJECT_INSUMOS.Models;
using ProjectInsumosCC___Migracion.Models;
using System.Data;

namespace ProjectInsumosCC___Migracion.Datos
{
    public class TransaccionesDatos
    {
        Conexion cn = new Conexion();
        UsuarioModel _Model2 = new UsuarioModel();


        public UsuarioModel CC_Usuarios_Sincronizar_Datos(UsuarioModel model)
        {
            // Reiniciar modelo de respuesta
            _Model2 = new UsuarioModel();

            try
            {
                using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
                using (var cmd = new SqlCommand("usp_UsuarioSincronizar", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    // Asignación segura de parámetros
                    cmd.Parameters.AddWithValue("@Usuario", string.IsNullOrWhiteSpace(model.Usuario) ? (object)DBNull.Value : model.Usuario);
                    cmd.Parameters.AddWithValue("@NroDocumento", string.IsNullOrWhiteSpace(model.NroDocumento) ? (object)DBNull.Value : model.NroDocumento);
                    cmd.Parameters.AddWithValue("@Nombres", string.IsNullOrWhiteSpace(model.Nombres) ? (object)DBNull.Value : model.Nombres);
                    cmd.Parameters.AddWithValue("@Apellidos", string.IsNullOrWhiteSpace(model.Apellidos) ? (object)DBNull.Value : model.Apellidos);
                    cmd.Parameters.AddWithValue("@Correo", string.IsNullOrWhiteSpace(model.Correo) ? (object)DBNull.Value : model.Correo);
                    // Area es opcional
                    cmd.Parameters.AddWithValue("@Area", string.IsNullOrWhiteSpace(model.Area) ? (object)DBNull.Value : model.Area);
                    conexion.Open();
                    using (var vr = cmd.ExecuteReader())
                    {
                        if (vr.Read())
                        {
                            _Model2.CodMensaje = vr["CodMensaje"] != DBNull.Value ? vr["CodMensaje"].ToString() : "0";
                            _Model2.Mensaje = vr["Mensaje"] != DBNull.Value ? vr["Mensaje"].ToString() : string.Empty;
                        }
                        else
                        {
                            _Model2.CodMensaje = "0";
                            _Model2.Mensaje = "No se pudo completar la operación";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _Model2.CodMensaje = "0";
                _Model2.Mensaje = "Error: " + ex.Message;
            }

            return _Model2;
        }

        public UsuarioModel CC_UsuariosObtener_Datos(string dni)
        {
            // Reiniciar modelo de respuesta
            _Model2 = new UsuarioModel();

            try
            {
                using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
                using (var cmd = new SqlCommand("usp_UsuarioObtener", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    // Asignación segura de parámetros

                    cmd.Parameters.AddWithValue("@NroDocumento", string.IsNullOrWhiteSpace(dni) ? (object)DBNull.Value : dni);
                    conexion.Open();
                    using (var vr = cmd.ExecuteReader())
                    {
                        if (vr.Read())
                        {
                            _Model2.Usuario = vr["Usuario"].ToString();
                            _Model2.NroDocumento = vr["NroDocumento"].ToString();
                            _Model2.Nombres = vr["Nombres"].ToString();
                            _Model2.Apellidos = vr["Apellidos"].ToString();
                            _Model2.Correo = vr["Correo"].ToString();
                            _Model2.Area = vr["Area"].ToString();
                            _Model2.Estado = Convert.ToBoolean(vr["Estado"].ToString());
                            _Model2.FechaIngreso = Convert.ToDateTime(vr["FechaIngreso"].ToString());
                            _Model2.CodMensaje = vr["CodMensaje"] != DBNull.Value ? vr["CodMensaje"].ToString() : "0";
                            _Model2.Mensaje = vr["Mensaje"] != DBNull.Value ? vr["Mensaje"].ToString() : string.Empty;
                        }
                        else
                        {
                            _Model2.CodMensaje = "0";
                            _Model2.Mensaje = "No se pudo completar la operación";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _Model2.CodMensaje = "0";
                _Model2.Mensaje = "Error: " + ex.Message;
            }

            return _Model2;
        }


        public IEnumerable<ProductosModel> CC_ListarProductosPorCategoria( int idCategoria)
        {
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();

                using var cmd = new SqlCommand("ListarProductos", conexion)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 300
                };
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@idCategoria", SqlDbType.Int).Value = idCategoria;
                //cmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = FechaFin.HasValue ? FechaFin.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value;


                using var rd = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                if (!rd.HasRows)
                {
                    yield return new ProductosModel
                    {
                        CodMensaje = "0",
                        Mensaje = "No hay movimientos de columnas para mostrar."
                    };

                    yield break;
                }
                //int xIDproducto = rd.GetOrdinal("IDproducto");
                int xProducto = rd.GetOrdinal("Producto");
                int xCodigo = rd.GetOrdinal("Codigo");
                int xFechaRegistro = rd.GetOrdinal("FechaRegistro");
                int xIDusuario = rd.GetOrdinal("IDusuario");

                // Bandera para saber si ya se envió el primer registro
                //bool primerRegistro = true;

                while (rd.Read())
                {
                    var registro = new ProductosModel
                    {
                       // IDproducto = rd.IsDBNull(xIDproducto) ? 0 : rd.GetInt64(xIDproducto),
                        ProductoNombre = rd.IsDBNull(xProducto) ? "--" : rd.GetString(xProducto),
                        CodigoProducto = rd.IsDBNull(xCodigo) ? "--" : rd.GetString(xCodigo),
                        FechaIngreso = rd.IsDBNull(xFechaRegistro) ? (DateTime?)null : rd.GetDateTime(xFechaRegistro),
                        IDusuario = rd.IsDBNull(xIDusuario) ? "--" : rd.GetString(xIDusuario)
                    };

                    //if (primerRegistro)
                    //{
                    //    registro.CodMensaje = 1;
                    //    registro.Mensaje = string.Empty;
                    //    primerRegistro = false;
                    //}

                    yield return registro;
                }

            }
        }


        public IEnumerable<ProductosModel> CC_BuscarProtocolosPorProducto (string Codigo)
        {
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();

                using var cmd = new SqlCommand("BuscarProducto", conexion)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 300
                };
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@Codigo", SqlDbType.VarChar,20).Value = Codigo;
                //cmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = FechaFin.HasValue ? FechaFin.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value;


                using var rd = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                if (!rd.HasRows)
                {
                    yield return new ProductosModel
                    {
                        CodMensaje = "0",
                        Mensaje = "No hay protocolos por mostrar para mostrar."
                    };

                    yield break;
                }
                int xIDproducto = rd.GetOrdinal("IDproducto");
                int xRuta = rd.GetOrdinal("Ruta");
                int xArchivo = rd.GetOrdinal("Archivo");
                int xCodigo = rd.GetOrdinal("Codigo");
                int xLote = rd.GetOrdinal("Lote");
                int xidCategoria = rd.GetOrdinal("idCategoria");
                int xActivo = rd.GetOrdinal("Activo");
                int xFechaRegistro = rd.GetOrdinal("FechaRegistro");

                // Bandera para saber si ya se envió el primer registro
                //bool primerRegistro = true;

                while (rd.Read())
                {
                    var registro = new ProductosModel
                    {
                        IDproducto = rd.IsDBNull(xIDproducto) ? 0 : rd.GetInt64(xIDproducto),
                        Ruta = rd.IsDBNull(xRuta) ? "--" : rd.GetString(xRuta),
                        Archivo = rd.IsDBNull(xArchivo) ? "--" : rd.GetString(xArchivo),
                        Codigo = rd.IsDBNull(xCodigo) ? "--" : rd.GetString(xCodigo),
                        Lote = rd.IsDBNull(xLote) ? "--" : rd.GetString(xLote),
                        IDcategoria = rd.IsDBNull(xidCategoria) ? 0 : rd.GetInt32(xidCategoria),
                        Estado= rd.IsDBNull(xActivo) ? 0 : rd.GetInt32(xActivo),
                        FechaIngreso = rd.IsDBNull(xFechaRegistro) ? null : rd.GetDateTime(xFechaRegistro),
                    };

                    //if (primerRegistro)
                    //{
                    //    registro.CodMensaje = 1;
                    //    registro.Mensaje = string.Empty;
                    //    primerRegistro = false;
                    //}

                    yield return registro;
                }

            }
        }
        public async Task<ProductosModel?> ObtenerPDF(long IDproducto)
        {
            
            using var conexion = new SqlConnection(cn.GetCadenaSQL());
            using var cmd = new SqlCommand("usp_VerProtocoloPDF", conexion);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@IDproducto", SqlDbType.BigInt).Value = IDproducto;

            await conexion.OpenAsync();

            using var rd = await cmd.ExecuteReaderAsync();

            if (!await rd.ReadAsync())
                return null;

            return new ProductosModel
            {
                PDF = rd.IsDBNull(rd.GetOrdinal("PDF")) ? null : rd.GetFieldValue<byte[]>(rd.GetOrdinal("PDF")),

                Archivo = rd.IsDBNull(rd.GetOrdinal("Archivo"))? null: rd.GetString(rd.GetOrdinal("Archivo"))
            };
        }
    }
}
