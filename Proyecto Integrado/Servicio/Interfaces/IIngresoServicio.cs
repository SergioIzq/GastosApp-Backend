using AppG.Controllers;
using AppG.Entidades.BBDD;
using System.Text.Json;
using static AppG.Servicio.IngresoServicio;

namespace AppG.Servicio
{
    public interface IIngresoServicio : IBaseServicio<Ingreso> {

        void ExportarDatosExcelAsync(Excel<IngresoDto> res);
    }

}