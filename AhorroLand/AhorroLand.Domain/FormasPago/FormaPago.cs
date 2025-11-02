using AhorroLand.Domain.Conceptos.Events;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain.FormasPago;

public sealed class FormaPago : AbsEntity
{
    private FormaPago(Guid id, Nombre nombre) : base(id)
    {
        Nombre = nombre;
    }

    public Nombre Nombre { get; private set; }

    public static FormaPago Create(Guid id, Nombre nombre)
    {
        var formaPago = new FormaPago(id, nombre);

        formaPago.RaiseDomainEvent(new ConceptoCreatedDomainEvent(formaPago.Id));

        return formaPago;
    }
}
