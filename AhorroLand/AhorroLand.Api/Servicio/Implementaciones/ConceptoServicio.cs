using AppG.Entidades.BBDD;
using AppG.Exceptions;
using NHibernate;
using NHibernate.Linq;

namespace AppG.Servicio
{
    public class ConceptoServicio : BaseServicio<Concepto>, IConceptoServicio
    {
        private readonly ICategoriaServicio _categoriaServicio;
        public ConceptoServicio(ISessionFactory sessionFactory, ICategoriaServicio categoriaServicio) : base(sessionFactory) { 
            _categoriaServicio = categoriaServicio;
        }


        public override async Task<Concepto> CreateAsync(Concepto entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {

                // Verificar si la categoría existe en la base de datos
                var existingConcepto = await session.Query<Concepto>()
                    .Where(c => c.Nombre == entity.Nombre && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingConcepto != null && existingConcepto.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"El concepto '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }

                // Guardar la entidad y guardar los cambios
                session.Save(entity);
                await transaction.CommitAsync();

                var id = session.GetIdentifier(entity);
                var createdEntity = await session.GetAsync<Concepto>(id);

                return createdEntity;

            }
        }

        public override async Task UpdateAsync(int id, Concepto entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {



                // Verificar si la categoría existe en la base de datos
                var existingConcepto = await session.Query<Concepto>()
                    .Where(c => c.Nombre == entity.Nombre && c.Id != entity.Id && c.IdUsuario == entity.IdUsuario )
                    .SingleOrDefaultAsync();

                if (existingConcepto != null && existingConcepto.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"El concepto '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }

                session.Update(entity);
                await transaction.CommitAsync();

            }
        }

        public override async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
        }
    }
}

