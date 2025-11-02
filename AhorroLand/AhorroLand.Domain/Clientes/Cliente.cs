using AhorroLand.Domain.Clientes.Events;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain.Clientes;

public sealed class Cliente : AbsEntity
{
    private Cliente(Guid id, Nombre nombre) : base(id)
    {
        Nombre = nombre;
    }

    public Nombre Nombre { get; private set; }

    public static Cliente Create(Guid id, Nombre nombre)
    {
        var cliente = new Cliente(id, nombre);

        cliente.RaiseDomainEvent(new ClienteCreatedDomainEvent(cliente.Id));

        return cliente;
    }

    public void Update(Nombre nombre)
    {
        Nombre = nombre;

        RaiseDomainEvent(new ClienteUpdatedDomainEvent(Id));
    }
}
