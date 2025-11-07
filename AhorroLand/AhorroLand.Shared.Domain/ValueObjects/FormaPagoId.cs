using AhorroLand.Shared.Domain.Interfaces;

namespace AhorroLand.Shared.Domain.ValueObjects;

public readonly record struct FormaPagoId : IGuidValueObject
{
    // Constructor primario sin lógica
    public Guid Value { get; init; }

    // Constructor secundario con validación
    public FormaPagoId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException(nameof(value));

        this.Value = value;
    }
}