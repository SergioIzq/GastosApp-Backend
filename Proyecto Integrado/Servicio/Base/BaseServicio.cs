using NHibernate;
using NHibernate.Linq;
using AppG.Controllers;
using AppG.Entidades.BBDD;
using System.Drawing.Printing;
using NHibernate.Criterion;

namespace AppG.Servicio
{
    public class BaseServicio<T> : IBaseServicio<T> where T : class
    {
        protected readonly ISessionFactory _sessionFactory;

        public BaseServicio(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public virtual async Task<ResponseList<T>> GetAllAsync<T>(int idUsuario) where T : Entidad
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var entities = await session.Query<T>()
                    .Where(e => e.IdUsuario == idUsuario)
                    .ToListAsync();
                var totalRecords = entities.Count();
                return new ResponseList<T>(entities, totalRecords);
            }
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                return await session.GetAsync<T>(id);
            }
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                await session.SaveAsync(entity);
                await transaction.CommitAsync();

                // Recargar la entidad para obtener el ID y cualquier otra propiedad generada
                var id = session.GetIdentifier(entity);
                var createdEntity = await session.GetAsync<T>(id);

                return createdEntity;
            }
        }


        public virtual async Task UpdateAsync(int id, T entity)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var existingEntity = await session.LoadAsync<T>(id);
                if (existingEntity == null)
                {
                    throw new KeyNotFoundException($"Entidad con ID {id} no encontrada");
                }
                session.Merge(entity);
                await transaction.CommitAsync();
            }
        }

        public virtual async Task DeleteAsync(int id)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var entity = await session.GetAsync<T>(id);
                if (entity == null)
                {
                    throw new KeyNotFoundException($"Entidad con ID {id} no encontrada");
                }
                session.Delete(entity);
                await transaction.CommitAsync();
                session.Clear();
            }
        }

        public virtual async Task<ResponseList<T>> GetCantidadAsync<T>(int page, int size, int idUsuario) where T : Entidad
        {
            try
            {
                using (var session = _sessionFactory.OpenSession())
                {
                    // Contar el número total de registros usando Criteria
                    var totalCountCriteria = session.CreateCriteria<T>()
                                                     .Add(Restrictions.Eq("IdUsuario", idUsuario))
                                                     .SetProjection(Projections.RowCount());
                    int totalCount = (int)totalCountCriteria.UniqueResult();

                    // Calcular el offset para la paginación
                    var offset = (page - 1) * size;

                    // Obtener los resultados paginados
                    var pagedResultsCriteria = session.CreateCriteria<T>()
                                                       .Add(Restrictions.Eq("IdUsuario", idUsuario)) // Agregar la restricción por idUsuario
                                                       .SetFirstResult(offset)
                                                       .SetMaxResults(size)
                                                       .AddOrder(Order.Asc("id")); // Asegúrate de tener una columna para ordenar

                    var results = pagedResultsCriteria.List<T>();

                    // Crear la respuesta con el total de registros y los elementos obtenidos
                    return new ResponseList<T>(results, totalCount);
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                throw new Exception($"Error al obtener cantidad: {ex.Message}", ex);
            }
        }


    }
}