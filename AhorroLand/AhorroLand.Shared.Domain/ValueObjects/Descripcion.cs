namespace AhorroLand.Shared.Domain.ValueObjects;

public readonly record struct Descripcion(string Value)
{
    public override string ToString() => Value;
}
