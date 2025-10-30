using AppG.Entidades.BBDD;
using AppG.Exceptions;
using NHibernate;
using NHibernate.Linq;
using System.Linq.Expressions;

namespace AppG.Servicio
{
    public class PersonaServicio : BaseServicio<Persona>, IPersonaServicio
    {
        private readonly IGastoProgramadoServicio _gastoProgramadoServicio;
        public PersonaServicio(ISessionFactory sessionFactory, IGastoProgramadoServicio gastoProgramadoServicio) : base(sessionFactory)
        {
            _gastoProgramadoServicio = gastoProgramadoServicio;
        }


        public override async Task<Persona> CreateAsync(Persona entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {

                // Verificar si la categoría existe en la base de datos
                var existingPersona = await session.Query<Persona>()
                    .Where(c => c.Nombre == entity.Nombre && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingPersona != null && existingPersona.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"La persona '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }


                try
                {
                    // Guardar la entidad y guardar los cambios
                    session.Save(entity);
                    await transaction.CommitAsync();

                    var id = session.GetIdentifier(entity);
                    var createdEntity = await session.GetAsync<Persona>(id);

                    return createdEntity;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                    throw new Exception(ex.Message);

                }

            }
        }

        public override async Task UpdateAsync(int id, Persona entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {



                // Verificar si la categoría existe en la base de datos
                var existingPersona = await session.Query<Persona>()
                    .Where(c => c.Nombre == entity.Nombre && c.Id != entity.Id && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingPersona != null && existingPersona.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"La persona '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }


                try
                {
                    // Guardar la entidad y guardar los cambios
                    session.Update(entity);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);

                }

            }
        }

        public override async Task DeleteAsync(int id)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                await EliminarEntidadesRelacionadas<Ingreso>(session, i => i.Persona.Id == id);
                await EliminarEntidadesRelacionadas<Gasto>(session, g => g!.Persona!.Id == id);

                // Eliminar los traspasos asociados a la cuenta como origen
                var gastosProgramados = await session.Query<GastoProgramado>()
                    .Where(c => c!.Persona!.Id == id)
                    .ToListAsync();

                foreach (var gasto in gastosProgramados)
                {
                    await _gastoProgramadoServicio.DeleteAsync(gasto.Id);
                }

                // Eliminar la cuenta
                await base.DeleteAsync(id);

                // Confirmar la transacción
                await transaction.CommitAsync();
            }
        }

        private async Task EliminarEntidadesRelacionadas<T>(NHibernate.ISession session, Expression<Func<T, bool>> predicate) where T : class
        {
            var entidades = await session.Query<T>()
                .Where(predicate)
                .ToListAsync();

            foreach (var entidad in entidades)
            {
                session.Delete(entidad);
            }
        }
    }
}

