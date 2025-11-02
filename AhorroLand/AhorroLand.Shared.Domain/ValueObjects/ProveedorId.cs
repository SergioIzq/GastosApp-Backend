namespace AhorroLand.Shared.Domain.ValueObjects;

public readonly record struct ProveedorId
{
    // Constructor primario sin lógica
    public Guid Value { get; init; }

    // Constructor secundario con validación
    public ProveedorId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException(nameof(value));

        this.Value = value;
    }
}