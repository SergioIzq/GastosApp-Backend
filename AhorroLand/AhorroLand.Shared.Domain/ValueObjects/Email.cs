using System.Text.RegularExpressions;

namespace AhorroLand.Shared.Domain.ValueObjects;

public record Email
{
    private static readonly Regex EmailRegex =
        new(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public Email(string value)
    {
        // 🔑 Regla de Negocio: No puede ser nulo o vacío
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("El correo electrónico no puede estar vacío.", nameof(value));
        }

        // 🔑 Regla de Negocio: Validar formato del email
        if (!EmailRegex.IsMatch(value))
        {
            throw new FormatException($"La dirección de correo '{value}' no tiene un formato válido.");
        }

        // 🔑 Regla de Negocio: Normalizar el correo a minúsculas
        Value = value.ToLowerInvariant();
    }
}