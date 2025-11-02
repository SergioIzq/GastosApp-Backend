namespace AhorroLand.Shared.Domain.ValueObjects;

public record struct Descripcion(string Value)
{
    public override string ToString() => Value;
}
