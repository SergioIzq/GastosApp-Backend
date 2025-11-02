using System.Data;

namespace AhorroLand.Infrastructure.Persistence.Query;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
