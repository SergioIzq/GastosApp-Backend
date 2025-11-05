using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Infrastructure.Persistence.Data.Categorias
{
    public class CategoriaReadRepository : AbsReadRepository<Categoria, CategoriaDto>, ICategoriaReadRepository
    {
        public CategoriaReadRepository(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory, "Categorias")
        {
        }

    }
}