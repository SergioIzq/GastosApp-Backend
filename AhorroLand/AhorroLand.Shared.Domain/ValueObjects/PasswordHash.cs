namespace AhorroLand.Shared.Domain.ValueObjects;

public readonly record struct PasswordHash
{
    public string Value { get; }

    public PasswordHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < 10)
        {
            throw new ArgumentException("El hash de la contraseña proporcionado no es válido o está vacío.", nameof(value));
        }

        Value = value;
    }
}