using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain;

public sealed class Categoria : AbsEntity
{
    private Categoria(Guid id, Nombre nombre, UsuarioId idUsuario, Descripcion? descripcion = null) : base(id)
    {
        Nombre = nombre;
        IdUsuario = idUsuario;
        Descripcion = descripcion;
    }

    public Nombre Nombre { get; private set; }
    public Descripcion? Descripcion { get; private set; }
    public UsuarioId IdUsuario { get; private set; }

    public static Categoria Create(Nombre nombre, UsuarioId usuarioId, Descripcion? descripcion = null)
    {
        var categoria = new Categoria(Guid.NewGuid(), nombre, usuarioId, descripcion);

        return categoria;
    }

    /// <summary>
    /// Actualiza el nombre y la descripción de la categoría.
    /// </summary>
    /// <param name="nombre">El nuevo Value Object Nombre (ya validado).</param>
    /// <param name="descripcion">El nuevo Value Object Descripcion.</param>
    public void Update(Nombre nombre, Descripcion? descripcion) => (Nombre, Descripcion) = (nombre, descripcion);
}
