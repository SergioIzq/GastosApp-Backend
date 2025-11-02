namespace AhorroLand.Shared.Domain.ValueObjects;

public readonly record struct FechaRegistro
{
    public DateTime Valor { get; }

    public FechaRegistro(DateTime valor)
    {
        if (valor > DateTime.UtcNow)
        {
            throw new ArgumentOutOfRangeException(
                nameof(valor),
                "La fecha de registro no puede ser una fecha futura.");
        }

        if (valor == DateTime.MinValue)
        {
            throw new ArgumentException("La fecha proporcionada no es válida.", nameof(valor));
        }

        Valor = valor;
    }

    public static FechaRegistro Hoy()
    {
        return new FechaRegistro(DateTime.Today);
    }
}