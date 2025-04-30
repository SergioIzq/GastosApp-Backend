using AppG.Entidades.BBDD;
using AppG.Servicio;
using Hangfire;
using NHibernate;

namespace CronJobProgramado
{
    public class GastoProgramadoJob
    {
        private readonly ISessionFactory _sessionFactory;

        public GastoProgramadoJob(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public async Task<GastoProgramado> ProgramarGastoAsync(GastoProgramado entity)
        {            
            try
            {
                // Lógica para calcular la fecha de ejecución y programar el trabajo
                var fechaEjecucion = CalcularFechaEjecucionDesdeDiaDelMes(entity.DiaEjecucion, entity.AjustarAUltimoDia);
                if (fechaEjecucion != null)
                {
                    var delay = fechaEjecucion.Value - DateTime.Now;
                    delay = TimeSpan.Zero;

                    // Aquí puedes reemplazar con un código que ejecute la tarea programada,
                    // por ejemplo, si usas Hangfire o cualquier otra herramienta.
                    BackgroundJob.Schedule<IGastoProgramadoServicio>(
                        x => x.AplicarGasto(entity.Id),
                        delay
                    );
                }

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
    }
}
