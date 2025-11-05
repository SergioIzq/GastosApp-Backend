using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Infrastructure.Persistence.Data.Clientes
{
    public class ClienteReadRepository : AbsReadRepository<Cliente, ClienteDto>, IClienteReadRepository
    {
        public ClienteReadRepository(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory, "clientes")
        {
        }

    }
}