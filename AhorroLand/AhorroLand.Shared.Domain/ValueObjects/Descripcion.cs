namespace AhorroLand.Shared.Domain.ValueObjects;

public readonly record struct Descripcion
{
    public readonly string? _Value;

    public Descripcion(string? Value)
    {
        if (!string.IsNullOrEmpty(Value))
            _Value = Value;
    }

    public override string ToString() => _Value is not null ? _Value : string.Empty;
}
