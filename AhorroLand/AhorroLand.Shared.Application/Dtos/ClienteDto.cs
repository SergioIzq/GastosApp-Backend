namespace AhorroLand.Shared.Application.Dtos
{
    /// <summary>
    /// Representación del cliente para ser enviada fuera de la capa de aplicación.
    /// </summary>
    // SÍ USAR ESTO (Propiedades)
    public record ClienteDto
    {
        public Guid Id { get; init; }
        public required string Nombre { get; init; }
        public Guid UsuarioId { get; init; }
        // Dapper ahora ignorará 'fecha_creacion' y otras columnas
        // porque no hay una propiedad aquí con ese nombre.
    }
}