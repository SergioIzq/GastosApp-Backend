namespace AhorroLand.Shared.Domain.Abstractions.Results;

public record Error(string Code, string Name, string? Message = null)
{
    // --- Errores Base ---
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "Un valor null fue ingresado");

    // --- Métodos de Error Parametrizables ---

    // 1. NOT FOUND (Devuelve 404)
    // El 'detail' por defecto es coherente y se puede sobrescribir con el ID no encontrado.
    public static Error NotFound(string detail = "El recurso solicitado no fue encontrado.") =>
        new("Error.NotFound", "Recurso no encontrado", detail);

    // 2. CONFLICT (Devuelve 409)
    // Mensaje coherente para un conflicto (violación de unicidad).
    public static Error Conflict(string detail = "Ya existe un recurso con una o más propiedades únicas.") =>
        new("Error.Conflict", "Conflicto de recurso", detail);

    // 3. UPDATE FAILURE
    public static Error UpdateFailure(string detail = "Falló la actualización del recurso. Posible conflicto de concurrencia.") =>
        new("Error.UpdateFailure", "Fallo de actualización", detail);

    // 4. DELETE FAILURE
    public static Error DeleteFailure(string detail = "Falló la eliminación del recurso. Posible dependencia activa (foreign key).") =>
        new("Error.DeleteFailure", "Fallo de eliminación", detail);

    // 5. VALIDATION (Devuelve 400)
    public static Error Validation(string detail = "Uno o más campos de entrada son inválidos.") =>
        new("Error.Validation", "Error de validación", detail);

    // 6. FALLO GENÉRICO (Para excepciones no controladas)
    public static Error Failure(string code, string name, string message) =>
        new(code, name, message);
}