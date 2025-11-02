namespace AhorroLand.Shared.Domain.ValueObjects;

public readonly record struct Nombre
{
    // Constructor primario sin lógica
    public string Value { get; init; }

    // Constructor secundario con validación
    public Nombre(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 50)
            throw new Exception(value);

        this.Value = value;
    }
}