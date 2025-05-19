using AppG.BBDD.Respuestas.Traspasos;
using AppG.Entidades.BBDD;
using AppG.Exceptions;
using Hangfire;
using NHibernate;
using NHibernate.Linq;

namespace AppG.Servicio
{
    public class TraspasoProgramadoServicio : BaseServicio<TraspasoProgramado>, ITraspasoProgramadoServicio
    {
        private readonly EmailService _emailService;
        private readonly ITraspasoServicio _traspasoServicio;
        public TraspasoProgramadoServicio(ISessionFactory sessionFactory, ITraspasoServicio traspasoServicio, EmailService emailService) : base(sessionFactory)
        {
            _traspasoServicio = traspasoServicio;
            _emailService = emailService;
        }


        public async override Task<TraspasoProgramado> CreateAsync(TraspasoProgramado entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Buscar la cuenta de origen por nombre
                    var cuentaOrigen = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == entity.CuentaOrigen.Nombre && c.IdUsuario == entity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuentaOrigen == null)
                    {
                        errorMessages.Add($"La cuenta de origen '{entity.CuentaOrigen.Nombre}' no existe.");
                    }

                    // Buscar la cuenta de destino por nombre
                    var cuentaDestino = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == entity.CuentaDestino.Nombre && c.IdUsuario == entity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuentaDestino == null)
                    {
                        errorMessages.Add($"La cuenta de destino '{entity.CuentaDestino.Nombre}' no existe.");
                    }

                    // Guardar el traspaso programado
                    session.Save(entity);
                    await transaction.CommitAsync();

                    // Obtener el ID generado
                    var id = session.GetIdentifier(entity);

                    // Recuperar la entidad completa (por si se llenan otros campos en base de datos)
                    var createdEntity = await session.GetAsync<TraspasoProgramado>(id);

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
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw new ValidationException(errorMessages);
                }
            }
        }


        public override async Task UpdateAsync(int id, TraspasoProgramado entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Cargar la entidad existente
                    var existingEntity = await session.GetAsync<TraspasoProgramado>(id);
                    if (existingEntity == null)
                    {
                        errorMessages.Add($"Entidad con ID {id} no encontrada");
                        throw new ValidationException(errorMessages);
                    }

                    // Buscar la cuenta original del traspaso (cuenta del traspaso existente)
                    var originalOrigenCuenta = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == existingEntity!.CuentaOrigen!.Nombre && c.IdUsuario == existingEntity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (originalOrigenCuenta == null)
                    {
                        errorMessages.Add($"La cuenta de origen original '{existingEntity!.CuentaOrigen!.Nombre}' no existe.");
                    }

                    // Buscar la nueva cuenta (cuenta del nuevo entity)
                    var nuevaCuentaOrigen = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == entity!.CuentaOrigen!.Nombre && c.IdUsuario == entity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (nuevaCuentaOrigen == null)
                    {
                        errorMessages.Add($"La cuenta de origen '{entity!.CuentaOrigen!.Nombre}' no existe.");
                    }

                    // Buscar la cuenta destino del traspaso (cuenta del traspaso existente)
                    var originalDestinoCuenta = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == existingEntity!.CuentaDestino!.Nombre && c.IdUsuario == existingEntity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (originalOrigenCuenta == null)
                    {
                        errorMessages.Add($"La cuenta de origen original '{existingEntity!.CuentaDestino!.Nombre}' no existe.");
                    }

                    // Buscar la nueva cuenta (cuenta del nuevo entity)
                    var nuevaCuentaDestino = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == entity!.CuentaDestino!.Nombre && c.IdUsuario == entity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (nuevaCuentaDestino == null)
                    {
                        errorMessages.Add($"La cuenta de origen '{entity!.CuentaDestino!.Nombre}' no existe.");
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
                    var existingEntity = await session.GetAsync<TraspasoProgramado>(id);
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

                    // Eliminar el traspaso
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

        public async Task<List<Cuenta>> GetNewTraspasoAsync(int id)
        {
            var listaCuentas = new List<Cuenta>();

            if (id > 0)
            {
                using (var session = _sessionFactory.OpenSession())
                {
                    listaCuentas = await session.Query<Cuenta>()
                                        .Where(c => c.IdUsuario == id)
                                        .OrderBy(c => c.Nombre)
                                        .ToListAsync();
                }
            }

            return listaCuentas;
        }

        public async Task<TraspasoProgramadoByIdRespuesta> GetTraspasoByIdAsync(int id)
        {
            TraspasoProgramadoByIdRespuesta response = new TraspasoProgramadoByIdRespuesta();
            var listaCuentas = new List<Cuenta>();

            response.TraspasoProgramadoById = await base.GetByIdAsync(id);

            if (response.TraspasoProgramadoById?.IdUsuario != null)
            {
                var idUsuario = response.TraspasoProgramadoById.IdUsuario;
                using (var session = _sessionFactory.OpenSession())
                {
                    listaCuentas = await session.Query<Cuenta>()
                                        .Where(c => c.IdUsuario == idUsuario)
                                        .OrderBy(c => c.Nombre)
                                        .ToListAsync();
                }
                response.ListaCuentas = listaCuentas;
            }

            return response;
        }

        public async Task AplicarTraspaso(int traspasoProgramadoId)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var traspasoP = await session.GetAsync<TraspasoProgramado>(traspasoProgramadoId);
                    if (traspasoP == null)
                    {
                        throw new Exception($"No se encontró el traspaso programado con ID {traspasoProgramadoId}");
                    }

                    if (!traspasoP.Activo)
                        return;

                    var cuentaOrigen = await session.Query<Cuenta>()
                        .Where(c => c.Id == traspasoP!.CuentaOrigen!.Id && c.IdUsuario == traspasoP.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuentaOrigen == null)
                    {
                        throw new Exception($"No se encontró la cuenta con ID {traspasoP!.CuentaOrigen!.Id}");
                    }

                    var cuentaDestino = await session.Query<Cuenta>()
                                        .Where(c => c.Id == traspasoP!.CuentaDestino!.Id && c.IdUsuario == traspasoP.IdUsuario)
                                        .SingleOrDefaultAsync();

                    if (cuentaDestino == null)
                    {
                        throw new Exception($"No se encontró la cuenta con ID {traspasoP!.CuentaDestino!.Id}");
                    }

                    // Realizar el traspaso
                    cuentaOrigen!.Saldo -= traspasoP.Importe;

                    cuentaDestino!.Saldo += traspasoP.Importe;


                    Traspaso traspaso = new Traspaso
                    {
                        CuentaOrigen = cuentaOrigen,
                        SaldoCuentaOrigen = cuentaOrigen.Saldo,
                        CuentaDestino = cuentaDestino,
                        SaldoCuentaDestino = cuentaDestino.Saldo,
                        Fecha = traspasoP.FechaEjecucion,
                        Descripcion = traspasoP.Descripcion,
                        Importe = traspasoP.Importe,
                        IdUsuario = traspasoP.CuentaOrigen.IdUsuario
                    };

                    // Guardar los cambios en las cuentas
                    await session.SaveOrUpdateAsync(cuentaOrigen);
                    await session.SaveOrUpdateAsync(cuentaDestino);
                    
                    await _traspasoServicio.RealizarTraspaso(traspaso, true);
                    await transaction.CommitAsync();

                    var usuario = await session.GetAsync<Usuario>(traspaso.IdUsuario);

                    var baseUrl = "https://ahorroland.sergioizq.es/traspasos";
#if DEBUG
                    baseUrl = "http://localhost:4200/traspasos";
#endif

                    await _emailService.SendEmailAsync(
                            usuario.Correo,
                            "Gasto programado ejecutado - Ahorroland",
                            $@"
                            <html>
                              <body style='font-family: Arial, sans-serif; font-size: 16px; color: #333;'>
                                <h1>Traspaso programado ejecutado correctamente</h1>
                                <p>Se ha aplicado automáticamente un ingreso programado con los siguientes detalles:</p>
                                <ul>
                                  <li><strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</li>
                                  <li><strong>Importe:</strong> {traspaso.Importe:C}</li>
                                  <li><strong>Cuenta origen:</strong> {traspaso.CuentaOrigen.Nombre}</li>
                                  <li><strong>Saldo cuenta origen:</strong> {traspaso.SaldoCuentaOrigen}</li>
                                  <li><strong>Cuenta destino:</strong> {traspaso.CuentaDestino.Nombre}</li>
                                  <li><strong>Saldo cuenta destino:</strong> {traspaso.SaldoCuentaDestino}</li>
                                </ul>
                                <p>Puedes ver el traspaso registrado en la sección <strong>Operaciones > Traspasos</strong> de tu cuenta:</p>
                                <p>
                                  <a href='{baseUrl}' target='_blank' 
                                     style='display: inline-block; padding: 10px 20px; background-color: #1a73e8; color: white; text-decoration: none; border-radius: 4px;'>
                                    Ver Traspaso
                                  </a>
                                </p>
                                <p style='margin-top: 20px;'>Si no reconoces este traspaso, por favor contacta con el administrador.</p>
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

        private async Task ProgramarTrabajoRecurrente(TraspasoProgramado createdEntity)
        {
            try
            {
                var recurringJobId = $"AplicarTraspaso_{createdEntity.Id}";

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
                            () => AplicarTraspaso(createdEntity.Id),
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
                            () => AplicarTraspaso(createdEntity.Id),
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
                            () => AplicarTraspaso(createdEntity.Id),
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
                throw new Exception("No se ha podido programar el traspaso. " + ex.Message);
            }
        }
    }
}