using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Interfaces;
using Dapper;
using System.Data;

namespace AhorroLand.Infrastructure.DataAccess;

public class DapperDomainValidator : IDomainValidator
{
    private readonly IDbConnection _dbConnection;

    // Se inyecta la conexión a la base de datos (Ej. SqlConnection)
    public DapperDomainValidator(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<bool> ExistsAsync<TEntity>(Guid id) where TEntity : AbsEntity
    {
        // 1. Obtener el nombre de la tabla
        // NOTA: Debes tener un mecanismo para mapear TEntity al nombre de su tabla en la DB.
        // Aquí usamos un nombre simple y genérico (Ej: Concepto -> "Conceptos").
        var tableName = GetTableName<TEntity>();

        // 2. Crear el query SQL más eficiente: SELECT 1
        // Esto solo verifica la existencia de un registro, sin devolver datos pesados.
        var sql = $@"
            SELECT 1 
            FROM {tableName} 
            WHERE Id = @Id
            LIMIT 1"; // O TOP 1 en SQL Server, para asegurar que la DB detenga la búsqueda al encontrar el primero.

        var parameters = new { Id = id };

        // 3. Ejecutar la consulta con Dapper
        // Usamos QueryFirstOrDefaultAsync<int?>. Si encuentra algo, devuelve 1 (true). Si no, devuelve null (false).
        var exists = await _dbConnection.QueryFirstOrDefaultAsync<int?>(sql, parameters);

        // 4. Devolver true si existe un resultado (es decir, si 'exists' no es null)
        return exists.HasValue;
    }

    // 💡 Método Auxiliar para obtener el nombre de la tabla
    private static string GetTableName<TEntity>() where TEntity : AbsEntity
    {
        // ⭐ OPTIMIZACIÓN: Aquí puedes implementar un mapeador de atributos para obtener el nombre real de la tabla.
        // Para simplificar, usamos el nombre de la clase en plural:
        var entityName = typeof(TEntity).Name.ToLower();

        if (entityName.EndsWith("s", StringComparison.OrdinalIgnoreCase))
        {
            return entityName;
        }

        // Caso simple de pluralización (ej: "Cuenta" -> "Cuentas")
        return entityName + "s";
    }
}