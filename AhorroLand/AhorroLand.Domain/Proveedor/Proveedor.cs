using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain;

public sealed class Proveedor : AbsEntity
{
    private Proveedor(Guid id, Nombre nombre, UsuarioId usuarioId) : base(id)
    {
        Nombre = nombre;
        UsuarioId = usuarioId;
    }

    public Nombre Nombre { get; private set; }
    public UsuarioId UsuarioId { get; private set; }


    public static Proveedor Create(Guid id, Nombre nombre, UsuarioId usuarioId)
    {
        var proveedor = new Proveedor(id, nombre, usuarioId);

        return proveedor;
    }
}