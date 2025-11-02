using AhorroLand.Domain.Proveedores.Events;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain.Proveedores;

public sealed class Proveedor : AbsEntity
{
    private Proveedor(Guid id, Nombre nombre): base(id)
    {
        Nombre = nombre;
    }

    public Nombre Nombre { get; private set; }

    public static Proveedor Create(Guid id, Nombre nombre)
    {
        var proveedor = new Proveedor(id, nombre);

        proveedor.RaiseDomainEvent(new ProveedorCreatedDomainEvent(proveedor.Id));

        return proveedor;
    }
}