using AppG.Entidades.BBDD;
using AppG.Exceptions;
using Hangfire;
using NHibernate;
using NHibernate.Linq;

namespace AppG.Servicio
{
    public class CuentaServicio : BaseServicio<Cuenta>, ICuentaServicio
    {
        private readonly IGastoProgramadoServicio _gastoProgramadoServicio;
        private readonly IIngresoProgramadoServicio _ingresoProgramadoServicio;

        public CuentaServicio(ISessionFactory sessionFactory, IGastoProgramadoServicio gastoProgramadoServicio, IIngresoProgramadoServicio ingresoProgramadoServicio) : base(sessionFactory)
        {
            _gastoProgramadoServicio = gastoProgramadoServicio;
            _ingresoProgramadoServicio = ingresoProgramadoServicio;
        }


        public override async Task<Cuenta> CreateAsync(Cuenta entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var existingCuenta = await session.Query<Cuenta>()
                    .Where(c => c.Nombre == entity.Nombre && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingCuenta != null && existingCuenta.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    errorMessages.Add($"La cuenta '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }

                session.Save(entity);
                await transaction.CommitAsync();

                var id = session.GetIdentifier(entity);
                var createdEntity = await session.GetAsync<Cuenta>(id);

                return createdEntity;


            }
        }

        public override async Task UpdateAsync(int id, Cuenta entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {

                var existingCuenta = await session.Query<Cuenta>()
                    .Where(c => c.Nombre == entity.Nombre && c.Id != entity.Id && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingCuenta != null && existingCuenta.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    errorMessages.Add($"La cuenta '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }

                // Guardar la entidad y guardar los cambios
                session.Update(entity);
                await transaction.CommitAsync();
            }
        }

        public override async Task DeleteAsync(int id)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var traspasosProgramados = await session.Query<TraspasoProgramado>()
                                            .Where(c => c.CuentaOrigen.Id == id || c.CuentaDestino.Id == id)
                                            .ToListAsync();

                    foreach (var traspasoProgramado in traspasosProgramados)
                    {
                        if (!string.IsNullOrEmpty(traspasoProgramado.HangfireJobId))
                        {
                            RecurringJob.RemoveIfExists(traspasoProgramado.HangfireJobId);
                        }
                    }

                    var gastosProgramados = await session.Query<GastoProgramado>()
                        .Where(c => c.Cuenta.Id == id)
                        .ToListAsync();

                    foreach (var gasto in gastosProgramados)
                        await _gastoProgramadoServicio.DeleteAsync(gasto.Id);

                    var ingresosProgramados = await session.Query<IngresoProgramado>()
                        .Where(c => c.Cuenta.Id == id)
                        .ToListAsync();

                    foreach (var ingreso in ingresosProgramados)
                        await _ingresoProgramadoServicio.DeleteAsync(ingreso.Id);

                    await base.DeleteAsync(id); // Esto eliminará la cuenta y en cascada los traspasos

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}

