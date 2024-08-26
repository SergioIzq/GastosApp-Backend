using AppG.BBDD;
using AppG.Controllers;
using AppG.Entidades.BBDD;

namespace AppG.Servicio
{
    public interface IResumenServicio : IBaseServicio<Resumen> {

        Task<ResumenGastosResponse> GetGastosAsync(int page, int size, string periodoInicio, string periodoFin);
        Task<ResumenIngresosResponse> GetIngresosAsync(int page, int size, string periodoInicio, string periodoFin);
        void ExportarDatosExcelAsync(ExportExcelRequest request);
    }

}