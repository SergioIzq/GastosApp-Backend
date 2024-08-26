using AppG.Controllers;
using AppG.Entidades.BBDD;
using static AppG.Servicio.FormaPagoServicio;

namespace AppG.Servicio
{
    public interface IFormaPagoServicio : IBaseServicio<FormaPago> {
        void ExportarDatosExcelAsync(Excel<FormaPagoDto> res);

    }

}