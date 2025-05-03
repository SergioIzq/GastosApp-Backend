using AppG.BBDD.Respuestas.Ingresos;
using AppG.Entidades.BBDD;
using static AppG.Servicio.IngresoServicio;

namespace AppG.Servicio
{
    public interface IIngresoServicio : IBaseServicio<Ingreso>
    {
        void ExportarDatosExcelAsync(Excel<IngresoDto> res);
        Task<IngresoRespuesta> GetNewIngresoAsync(int idUsuario);
        Task<IngresoByIdRespuesta> GetIngresoByIdAsync(int id);
    }

}