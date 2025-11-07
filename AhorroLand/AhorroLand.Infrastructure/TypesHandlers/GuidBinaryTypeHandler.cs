using Dapper;
using System.Data;

namespace AhorroLand.Infrastructure.TypesHandlers
{
    /// <summary>
    /// TypeHandler para convertir valores de MySQL a Guid.
    /// Maneja tanto BINARY(16) como strings UUID.
    /// </summary>
    public class GuidBinaryTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.Value = value.ToByteArray();
            parameter.DbType = DbType.Binary;
        }

        public override Guid Parse(object value)
        {
            return value switch
            {
                // BINARY(16) directo
                byte[] bytes when bytes.Length == 16 => new Guid(bytes),

                // Ya es un Guid
                Guid guid => guid,

                // String UUID (de BIN_TO_UUID)
                string str when !string.IsNullOrEmpty(str) => Guid.Parse(str),

                _ => throw new DataException($"Cannot convert {value?.GetType().Name ?? "null"} to Guid")
            };
        }
    }
}
