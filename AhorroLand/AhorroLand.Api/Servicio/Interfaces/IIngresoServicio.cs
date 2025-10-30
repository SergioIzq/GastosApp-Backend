using AppG.BBDD.Respuestas.Ingresos;
using AppG.Entidades.BBDD;

namespace AppG.Servicio
{
    public interface IIngresoServicio : IBaseServicio<Ingreso>
    {
        Task<IngresoRespuesta> GetNewIngresoAsync(int idUsuario);
        Task<IngresoByIdRespuesta> GetIngresoByIdAsync(int id);
        Task<Ingreso> CreateAsync(Ingreso entity, bool esGastoProgramado = false);
    }

}