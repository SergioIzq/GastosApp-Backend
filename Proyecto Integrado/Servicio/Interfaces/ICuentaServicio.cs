using AppG.Entidades.BBDD;
using static AppG.Servicio.CuentaServicio;

namespace AppG.Servicio
{
    public interface ICuentaServicio : IBaseServicio<Cuenta> {
        void ExportarDatosExcelAsync(Excel<CuentaDto> res);

    }

}