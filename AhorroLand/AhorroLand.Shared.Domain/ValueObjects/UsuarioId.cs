using AhorroLand.Shared.Domain.Interfaces;

namespace AhorroLand.Shared.Domain.ValueObjects;

/// <summary>
/// 🚀 OPTIMIZADO: Value Object para UsuarioId.
/// </summary>
public readonly record struct UsuarioId : IGuidValueObject
{
    // Constructor primario sin lógica
    public Guid Value { get; init; }

    // Constructor secundario con validación
    public UsuarioId(Guid value)
    {
        // 🚀 OPTIMIZACIÓN: Validación más rápida con comparación directa
        if (value == Guid.Empty)
            throw new ArgumentException("UsuarioId no puede ser Guid.Empty", nameof(value));

        this.Value = value;
    }

    // 🚀 OPTIMIZACIÓN: Factory method estático para evitar boxing
    public static UsuarioId Create(Guid value) => new(value);

    // Override ToString para logging eficiente
    public override string ToString() => Value.ToString("D");
}