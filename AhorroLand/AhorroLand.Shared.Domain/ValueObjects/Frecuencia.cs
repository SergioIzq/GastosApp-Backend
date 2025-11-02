namespace AhorroLand.Shared.Domain.ValueObjects;

public readonly record struct Frecuencia
{
    public string Value { get; init; }

    private static readonly string[] AllowedFrequencies = new[] { "Diaria", "Semanal", "Mensual", "Anual" };

    public Frecuencia(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !AllowedFrequencies.Contains(value, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"La frecuencia '{value}' no es válida. Debe ser una de: {string.Join(", ", AllowedFrequencies)}.");
        }
        Value = value;
    }
}