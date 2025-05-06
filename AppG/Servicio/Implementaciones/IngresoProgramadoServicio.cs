using AppG.Entidades.BBDD;
using AppG.Exceptions;
using NHibernate;
using NHibernate.Linq;
using Hangfire;
using AppG.BBDD.Respuestas.Ingresos;

namespace AppG.Servicio
{
    public class IngresoProgramadoServicio : BaseServicio<IngresoProgramado>, IIngresoProgramadoServicio
    {
        public IngresoProgramadoServicio(ISessionFactory sessionFactory) : base(sessionFactory)
        {

        }


        public async override Task<IngresoProgramado> CreateAsync(IngresoProgramado entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Obtener la categoría del entity
                    var entityCategoria = entity?.Concepto!.Categoria;
                    if (entityCategoria != null)
                    {
                        // Verificar si la categoría existe en la base de datos
                        var existingCategoria = await session.Query<Categoria>()
                            .Where(c => c.Nombre == entityCategoria.Nombre && c.IdUsuario == entityCategoria.IdUsuario)
                            .SingleOrDefaultAsync();

                        if (existingCategoria != null)
                        {
                            // Asignar el ID de la categoría existente a la entidad
                            entity!.Concepto!.Categoria = existingCategoria;
                        }
                        else
                        {
                            errorMessages.Add($"La categoría '{entityCategoria.Nombre}' no existe.");
                        }
                    }

                    // Guardar el gasto programado
                    session.Save(entity);
                    await transaction.CommitAsync();

                    // Obtener el ID generado
                    var id = session.GetIdentifier(entity);

                    // Recuperar la entidad completa (por si se llenan otros campos en base de datos)
                    var createdEntity = await session.GetAsync<IngresoProgramado>(id);

                    // Programar el trabajo recurrente en Hangfire
                    ProgramarTrabajoRecurrente(createdEntity);

                    // Guardar el HangfireJobId en base de datos si fue modificado
                    using (var updateTransaction = session.BeginTransaction())
                    {
                        await session.UpdateAsync(createdEntity);
                        await updateTransaction.CommitAsync();
                    }

                    return createdEntity;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }


        public override async Task UpdateAsync(int id, IngresoProgramado entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Cargar la entidad existente
                    var existingEntity = await session.GetAsync<Gasto>(id);
                    if (existingEntity == null)
                    {
                        errorMessages.Add($"Entidad con ID {id} no encontrada");
                        throw new ValidationException(errorMessages);
                    }

                    // Obtener la categoría del entity
                    var entityCategoria = entity?.Concepto!.Categoria;
                    if (entityCategoria != null)
                    {
                        // Verificar si la categoría existe en la base de datos
                        var existingCategoria = await session.Query<Categoria>()
                            .Where(c => c.Nombre == entityCategoria.Nombre && c.IdUsuario == entityCategoria.IdUsuario)
                            .SingleOrDefaultAsync();

                        if (existingCategoria != null)
                        {
                            // Asignar el ID de la categoría existente a la entidad
                            entity!.Concepto!.Categoria = existingCategoria;
                        }
                        else
                        {
                            errorMessages.Add($"La categoría '{entityCategoria.Nombre}' no existe.");
                        }
                    }

                    // Buscar la cuenta original del gasto (cuenta del gasto existente)
                    var originalCuenta = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == existingEntity!.Cuenta!.Nombre && c.IdUsuario == existingEntity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (originalCuenta == null)
                    {
                        errorMessages.Add($"La cuenta original '{existingEntity!.Cuenta!.Nombre}' no existe.");
                    }

                    // Buscar la nueva cuenta (cuenta del nuevo entity)
                    var nuevaCuenta = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == entity!.Cuenta!.Nombre && c.IdUsuario == entity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (nuevaCuenta == null)
                    {
                        errorMessages.Add($"La cuenta '{entity!.Cuenta!.Nombre}' no existe.");
                    }

                    if (errorMessages.Count > 0)
                    {
                        throw new ValidationException(errorMessages);
                    }

                    // Revertir el saldo de la cuenta original basado en el gasto anterior
                    if (originalCuenta != null)
                    {
                        originalCuenta.Saldo += existingEntity.Monto;
                        session.Update(originalCuenta);
                    }

                    // Actualizar el saldo de la nueva cuenta basado en el nuevo monto
                    if (nuevaCuenta != null)
                    {
                        nuevaCuenta.Saldo -= entity!.Monto;
                        session.Update(nuevaCuenta);
                    }

                    // Fusionar y guardar la entidad actualizada
                    session.Merge(entity);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
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
                try
                {
                    // Cargar la entidad existente
                    var existingEntity = await session.GetAsync<IngresoProgramado>(id);
                    if (existingEntity == null)
                    {
                        errorMessages.Add($"Entidad con ID {id} no encontrada");
                        throw new ValidationException(errorMessages);
                    }

                    // Eliminar el job de Hangfire si existe
                    if (!string.IsNullOrEmpty(existingEntity.HangfireJobId))
                    {
                        RecurringJob.RemoveIfExists(existingEntity.HangfireJobId);
                    }

                    // Eliminar el gasto
                    await session.DeleteAsync(existingEntity);

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        // Monta objeto para crear un nuevo gasto
        public async Task<IngresoRespuesta> GetNewIngresoAsync(int idUsuario)
        {
            var errorMessages = new List<string>();
            IngresoRespuesta newIngreso = new IngresoRespuesta();

            using (var session = _sessionFactory.OpenSession())
            {
                try
                {
                    var listaCategorias = await session.Query<Categoria>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    var listaConceptos = await session.Query<Concepto>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    var listaCuentas = await session.Query<Cuenta>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    var listaClientes = await session.Query<Cliente>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    var listaPersonas = await session.Query<Persona>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    var listaFormasPago = await session.Query<FormaPago>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    newIngreso.ListaClientes = listaClientes;
                    newIngreso.ListaCuentas = listaCuentas;
                    newIngreso.ListaConceptos = listaConceptos;
                    newIngreso.ListaCategorias = listaCategorias;
                    newIngreso.ListaPersonas = listaPersonas;
                    newIngreso.ListaFormasPago = listaFormasPago;

                    return newIngreso;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<IngresoProgramadoByIdRespuesta> GetIngresoByIdAsync(int id)
        {
            IngresoProgramadoByIdRespuesta response = new IngresoProgramadoByIdRespuesta();

            response.IngresoProgramadoById = await base.GetByIdAsync(id);

            if (response.IngresoProgramadoById?.Cuenta?.IdUsuario != null)
            {
                response.IngresoRespuesta = await GetNewIngresoAsync(response.IngresoProgramadoById.Cuenta.IdUsuario);
            }

            return response;
        }

        public async Task AplicarIngreso(int ingresoProgramadoId)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var ingreso = await session.GetAsync<IngresoProgramado>(ingresoProgramadoId);
                    if (ingreso == null)
                    {
                        throw new Exception($"No se encontró el ingreso programado con ID {ingresoProgramadoId}");
                    }

                    if (!ingreso.Activo)
                        return;

                    var cuenta = await session.Query<Cuenta>()
                        .Where(c => c.Id == ingreso!.Cuenta!.Id && c.IdUsuario == ingreso.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuenta == null)
                    {
                        throw new Exception($"No se encontró la cuenta con ID {ingreso!.Cuenta!.Id}");
                    }

                    cuenta.Saldo += Math.Abs(ingreso.Monto);

                    session.Update(cuenta);

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        private DateTime? CalcularFechaEjecucionDesdeDiaDelMes(int dia, bool ajustarAUltimoDia)
        {
            var hoy = DateTime.Today;
            var año = hoy.Year;
            var mes = hoy.Month;

            if (dia <= hoy.Day)
            {
                mes++;
                if (mes > 12)
                {
                    mes = 1;
                    año++;
                }
            }

            var ultimoDiaDelMes = DateTime.DaysInMonth(año, mes);

            if (dia > ultimoDiaDelMes)
            {
                if (ajustarAUltimoDia)
                {
                    dia = ultimoDiaDelMes;
                }
                else
                {
                    return null; // No se programa
                }
            }

            return new DateTime(año, mes, dia);
        }

        private void ProgramarTrabajoRecurrente(IngresoProgramado createdEntity)
        {
            var recurringJobId = $"AplicarIngreso_{createdEntity.Id}";

            var fechaEjecucion = CalcularFechaEjecucionDesdeDiaDelMes(createdEntity.DiaEjecucion, createdEntity.AjustarAUltimoDia);

            if (fechaEjecucion != null)
            {
                var delay = fechaEjecucion.Value - DateTime.Now;

                if (delay < TimeSpan.Zero)
                {
                    fechaEjecucion = fechaEjecucion.Value.AddDays(1);
                    delay = fechaEjecucion.Value - DateTime.Now;
                }

                createdEntity.HangfireJobId = recurringJobId;

                var hora = fechaEjecucion.Value.Hour;
                var minuto = fechaEjecucion.Value.Minute;

                if (hora == 0 && minuto == 0)
                {
                    hora = 9;
                    minuto = 0;
                }

                RecurringJob.AddOrUpdate(
                    recurringJobId,
                    () => AplicarIngreso(createdEntity.Id),
                    Cron.Monthly(fechaEjecucion.Value.Day, hora, minuto),
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    }
                );

            }
        }
    }
}