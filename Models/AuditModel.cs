namespace PROJECT_INSUMOS.Models
{
    public class AuditModel
    {
        public long IDAuditTrail { get; set; }
        public DateTime? FechaHora { get; set; }
        public DateTime? FechaRegistroAccion { get; set; }
        public string? FechaHoraFormato { get; set; }
        public string? IDusuario { get; set; }
        public string? Usuario { get; set; }
        public string? Accion { get; set; }
        public string? Tabla { get; set; }
        public string? NroRegistro { get; set; }
        public string? NombreCampo { get; set; }
        public string? IDRegistro { get; set; }
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
        public string? Detalle { get; set; }
        public string? UsuarioAccion { get; set; }
        public string? IP { get; set; }
        public string? MotivoCambio { get; set; }
        public string? HostName { get; set; }
        public bool EsDuplicado { get; set; }

    }








    /// <summary>
    /// mejorado
    /// </summary>

    public class AsistentePreguntaModel
    {
        public string Pregunta { get; set; } = string.Empty;
        public string? Respuesta { get; set; }
    }
}
