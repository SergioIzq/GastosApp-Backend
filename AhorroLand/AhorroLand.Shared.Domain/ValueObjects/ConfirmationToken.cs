namespace AhorroLand.Shared.Domain.ValueObjects;

public readonly record struct ConfirmationToken
{
    public string Value { get; }

    private const int TokenLength = 32;

    public ConfirmationToken(string value)
    {
        // 🔑 Regla de Negocio: El token debe tener la longitud esperada.
        if (string.IsNullOrWhiteSpace(value) || value.Length != TokenLength)
        {
            throw new ArgumentException($"El token debe tener exactamente {TokenLength} caracteres.", nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// Método de fábrica para generar un token seguro y aleatorio.
    /// </summary>
    public static ConfirmationToken GenerateNew()
    {
        // 🔑 Lógica de Generación Segura: Usar una fuente criptográficamente segura.
        // Se genera un GUID y se codifica de forma compacta (ej. Base64 URL-safe, truncado).
        // Para simplicidad, usaremos Guid.NewGuid() y lo procesaremos.

        // Generación simple: utiliza el Guid para una cadena aleatoria
        var tokenRaw = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("=", "")
            .Replace("+", "-")
            .Replace("/", "_");

        // Aseguramos la longitud definida por la regla de negocio
        var token = tokenRaw.Substring(0, TokenLength);

        return new ConfirmationToken(token);
    }
}