using AppG.BBDD.Respuestas.Ingresos;
using AppG.Entidades.BBDD;

namespace AppG.Servicio
{
    public interface IIngresoProgramadoServicio : IBaseServicio<IngresoProgramado>
    {
        Task<IngresoRespuesta> GetNewIngresoAsync(int idUsuario);
        Task<IngresoProgramadoByIdRespuesta> GetIngresoByIdAsync(int id);
        Task AplicarIngreso(int IngresoProgramadoId);
    }
}