using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Infrastructure.Persistence.Data.Usuarios
{
    public class UsuarioReadRepository : AbsReadRepository<Usuario, UsuarioDto>, IUsuarioReadRepository
    {
        public UsuarioReadRepository(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory, "Usuarios")
        {
        }

    }
}