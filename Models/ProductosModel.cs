namespace ProjectInsumosCC___Migracion.Models
{
    public class ProductosModel
    {


        public long IDproducto { get; set; }
        public string? IDusuario { get; set; }
        public int IDtipoEstado { get; set; }
        public string? CodigoProducto { get; set; }
        public string? ProductoNombre { get; set; }
        public string? Lote { get; set; }
        public DateTime? FechaIngreso { get; set; }

        public int? Estado { get; set; }
        public string? Mensaje { get; set; }
        public string? CodMensaje { get; set; }
        public string? UsuarioNombre { get; set; }
        public byte[]? RowVersion { get; set; }
        public string? RowVersionString { get; set; }
        public string? MotivoCambio { get; set; }
        public string? Ruta { get; set; }
        public string? Archivo { get; set; }
        public string? Codigo { get; set; }
        public int? IDcategoria { get; set; }

        public byte[]? PDF { get; set; }

    }
}
