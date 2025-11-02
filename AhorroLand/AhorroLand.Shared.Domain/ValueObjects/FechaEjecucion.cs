namespace AhorroLand.Shared.Domain.ValueObjects;

public readonly record struct FechaEjecucion
{
    public DateTime Valor { get; }

    public FechaEjecucion(DateTime valor)
    {

        if (valor == DateTime.MinValue)
        {
            throw new ArgumentException("La fecha proporcionada no es válida.", nameof(valor));
        }

        Valor = valor;
    }
}