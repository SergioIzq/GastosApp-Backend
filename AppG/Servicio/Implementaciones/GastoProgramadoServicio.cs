using AppG.BBDD.Respuestas.Gastos;
using AppG.Entidades.BBDD;
using AppG.Exceptions;
using Hangfire;
using NHibernate;
using NHibernate.Linq;
using System.Globalization;

namespace AppG.Servicio
{
    public class GastoProgramadoServicio : BaseServicio<GastoProgramado>, IGastoProgramadoServicio
    {
        private readonly IGastoServicio _gastoServicio;
        private readonly EmailService _emailService;
        public GastoProgramadoServicio(ISessionFactory sessionFactory, IGastoServicio gastoServicio, EmailService emailService) : base(sessionFactory)
        {
            _gastoServicio = gastoServicio;
            _emailService = emailService;
        }


        public async override Task<GastoProgramado> CreateAsync(GastoProgramado entity)
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
                    var createdEntity = await session.GetAsync<GastoProgramado>(id);

                    // Programar el trabajo recurrente en Hangfire
                    await ProgramarTrabajoRecurrente(createdEntity);

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


        public override async Task UpdateAsync(int id, GastoProgramado entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Cargar la entidad existente
                    var existingEntity = await session.GetAsync<GastoProgramado>(id);
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

                    // Fusionar y guardar la entidad actualizada
                    session.Merge(entity);
                    await transaction.CommitAsync();

                    // Reprogramar Hangfire
                    await ProgramarTrabajoRecurrente(entity!);
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
                    var existingEntity = await session.GetAsync<GastoProgramado>(id);
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
        public async Task<GastoRespuesta> GetNewGastoAsync(int idUsuario)
        {
            var errorMessages = new List<string>();
            GastoRespuesta newGasto = new GastoRespuesta();

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

                    var listaProveedores = await session.Query<Proveedor>()
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

                    // Crear objeto respuesta al frontend para nuevos gastos
                    newGasto.ListaProveedores = listaProveedores;
                    newGasto.ListaCuentas = listaCuentas;
                    newGasto.ListaConceptos = listaConceptos;
                    newGasto.ListaCategorias = listaCategorias;
                    newGasto.ListaPersonas = listaPersonas;
                    newGasto.ListaFormasPago = listaFormasPago;

                    return newGasto;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<GastoProgramadoByIdRespuesta> GetGastoByIdAsync(int id)
        {
            GastoProgramadoByIdRespuesta response = new GastoProgramadoByIdRespuesta();

            response.GastoProgramadoById = await base.GetByIdAsync(id);

            if (response.GastoProgramadoById?.Cuenta?.IdUsuario != null)
            {
                response.GastoRespuesta = await GetNewGastoAsync(response.GastoProgramadoById.Cuenta.IdUsuario);
            }

            return response;
        }

        public async Task AplicarGasto(int gastoProgramadoId)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var gastoP = await session.GetAsync<GastoProgramado>(gastoProgramadoId);
                    if (gastoP == null)
                    {
                        throw new Exception($"No se encontró el gasto programado con ID {gastoProgramadoId}");
                    }

                    if (!gastoP.Activo)
                        return;

                    var cuenta = await session.Query<Cuenta>()
                        .Where(c => c.Id == gastoP!.Cuenta!.Id && c.IdUsuario == gastoP.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuenta == null)
                    {
                        throw new Exception($"No se encontró la cuenta con ID {gastoP!.Cuenta!.Id}");
                    }

                    cuenta.Saldo -= Math.Abs(gastoP.Monto);

                    session.Update(cuenta);

                    Gasto gasto = new Gasto();
                    gasto.Proveedor = gastoP.Proveedor;
                    gasto.Concepto = gastoP.Concepto;
                    gasto.Cuenta = gastoP.Cuenta;
                    gasto.Descripcion = gastoP.Descripcion;
                    gasto.Fecha = DateTime.Now;
                    gasto.FormaPago = gastoP.FormaPago;
                    gasto.IdUsuario = gastoP.IdUsuario;
                    gasto.Monto = gastoP.Monto;
                    gasto.Persona = gastoP.Persona;                    

                    await _gastoServicio.CreateAsync(gasto, true);
                    await transaction.CommitAsync();
                    await session.FlushAsync();

                    var usuario = await session.GetAsync<Usuario>(gasto.IdUsuario);

                    var baseUrl = $"https://ahorroland.sergioizq.es/gastos/gasto-detail/{gasto.Id}";
#if DEBUG
                    baseUrl = $"http://localhost:4200/gastos/gasto-detail/{gasto.Id}";
#endif

                    await _emailService.SendEmailAsync(
                            usuario.Correo,
                            "Gasto programado ejecutado - Ahorroland",
                            $@"
                            <html>
                              <body style='font-family: Arial, sans-serif; font-size: 16px; color: #333;'>
                                <h1>Gasto programado ejecutado correctamente</h1>
                                <p>Se ha aplicado automáticamente un ingreso programado con los siguientes detalles:</p>
                                <ul>
                                  <li><strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</li>
                                  <li><strong>Importe:</strong> -{gasto.Monto.ToString("N2", new CultureInfo("es-ES"))} €</li>
                                  < li><strong>Cuenta:</strong> {gasto.Cuenta.Nombre}</li>
                                  <li><strong>Categoria:</strong> {gasto.Concepto.Categoria.Nombre}</li> 
                                  <li><strong>Concepto:</strong> {gasto.Concepto.Nombre}</li>
                                </ul>
                                <p>Puedes ver el gasto registrado en la sección <strong>Operaciones > Gastos</strong> de tu cuenta:</p>
                                <p>
                                  <a href='{baseUrl}' target='_blank' 
                                     style='display: inline-block; padding: 10px 20px; background-color: #1a73e8; color: white; text-decoration: none; border-radius: 4px;'>
                                    Ver Gasto
                                  </a>
                                </p>
                                <p style='margin-top: 20px;'>Si no reconoces este gasto, por favor contacta con el administrador.</p>
                              </body>
                            </html>
                            "
                        );
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        private DateTime? CalcularFechaEjecucionDesdeDiaDelMes(DateTime fechaEjecucion, string frecuencia)
        {
            var hoy = DateTime.Today;
            var año = fechaEjecucion.Year;
            var mes = fechaEjecucion.Month;
            var dia = fechaEjecucion.Day;

            // Frecuencia Diaria
            if (frecuencia.Equals("DIARIA", StringComparison.OrdinalIgnoreCase))
            {
                DateTime fechaActual = DateTime.Now;

                // Obtener solo la fecha (sin hora) de la fecha actual
                DateTime fechaHoy = new DateTime(fechaActual.Year, fechaActual.Month, fechaActual.Day);

                // Si la hora de fechaEjecucion es posterior a la hora actual del día
                if (fechaEjecucion < fechaHoy.AddHours(fechaEjecucion.Hour).AddMinutes(fechaEjecucion.Minute))
                {
                    // Si la hora no ha pasado aún, programar para hoy a esa hora
                    return fechaHoy.AddHours(fechaEjecucion.Hour).AddMinutes(fechaEjecucion.Minute);
                }
                else
                {
                    // Si la hora ya ha pasado hoy, programar para mañana a esa hora
                    return fechaEjecucion.AddDays(1);
                }
            }

            // Frecuencia Mensual
            if (frecuencia.Equals("MENSUAL", StringComparison.OrdinalIgnoreCase))
            {
                // Si el día de ejecución es mayor que el día de hoy y aún no ha pasado, ejecutamos en este mes
                if (fechaEjecucion.Month == hoy.Month && fechaEjecucion.Day >= hoy.Day)
                {
                    // Si el día aún no ha pasado este mes, la ejecución es este mes
                    return new DateTime(hoy.Year, hoy.Month, fechaEjecucion.Day, fechaEjecucion.Hour, fechaEjecucion.Minute, 0);
                }

                // Si el día ya pasó o estamos en un mes posterior, programar para el siguiente mes
                mes++;
                if (mes > 12)
                {
                    mes = 1;
                    año++;
                }

                return new DateTime(año, mes, dia, fechaEjecucion.Hour, fechaEjecucion.Minute, 0);
            }

            if (frecuencia.Equals("SEMANAL", StringComparison.OrdinalIgnoreCase))
            {
                return fechaEjecucion;
            }

            // Si la frecuencia no es válida, devolver null
            return null;
        }

        private async Task ProgramarTrabajoRecurrente(GastoProgramado createdEntity)
        {
            try
            {
                var recurringJobId = $"AplicarGasto_{createdEntity.Id}";

                var fechaEjecucion = CalcularFechaEjecucionDesdeDiaDelMes(
                    createdEntity.FechaEjecucion,
                    createdEntity.Frecuencia
                );

                if (fechaEjecucion != null)
                {
                    var delay = fechaEjecucion.Value - DateTime.Now;

                    if (delay < TimeSpan.Zero)
                    {
                        if (createdEntity.Frecuencia.Equals("DIARIA", StringComparison.OrdinalIgnoreCase))
                        {
                            fechaEjecucion = fechaEjecucion.Value.AddDays(1);
                        }
                        else if (createdEntity.Frecuencia.Equals("SEMANAL", StringComparison.OrdinalIgnoreCase))
                        {
                            fechaEjecucion = fechaEjecucion.Value.AddDays(7);
                        }

                        delay = fechaEjecucion.Value - DateTime.Now;
                    }

                    createdEntity.HangfireJobId = recurringJobId;

                    var hora = fechaEjecucion.Value.Hour;
                    var minuto = fechaEjecucion.Value.Minute;

                    if (hora == 0 && minuto == 0)
                    {
                        hora = 8;
                        minuto = 0;
                    }

                    if (createdEntity.Frecuencia.Equals("DIARIA", StringComparison.OrdinalIgnoreCase))
                    {
                        RecurringJob.AddOrUpdate(
                            recurringJobId,
                            () => AplicarGasto(createdEntity.Id),
                            Cron.Daily(hora, minuto),
                            new RecurringJobOptions
                            {
                                TimeZone = TimeZoneInfo.Local
                            }
                        );
                    }
                    else if (createdEntity.Frecuencia.Equals("SEMANAL", StringComparison.OrdinalIgnoreCase))
                    {
                        DayOfWeek dayOfWeek = fechaEjecucion.Value.DayOfWeek;

                        RecurringJob.AddOrUpdate(
                            recurringJobId,
                            () => AplicarGasto(createdEntity.Id),
                            Cron.Weekly(dayOfWeek, hora, minuto),
                            new RecurringJobOptions
                            {
                                TimeZone = TimeZoneInfo.Local
                            }
                        );
                    }
                    else if (createdEntity.Frecuencia.Equals("MENSUAL", StringComparison.OrdinalIgnoreCase))
                    {
                        RecurringJob.AddOrUpdate(
                            recurringJobId,
                            () => AplicarGasto(createdEntity.Id),
                            Cron.Monthly(fechaEjecucion.Value.Day, hora, minuto),
                            new RecurringJobOptions
                            {
                                TimeZone = TimeZoneInfo.Local
                            }
                        );
                    }
                }
            }
            catch (Exception ex)
            {                
                await DeleteAsync(createdEntity.Id);
                throw new Exception("No se ha podido programar el gasto. " + ex.Message);
            }
        }

    }
}