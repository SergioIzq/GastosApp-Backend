using AppG.Controllers;
using AppG.Entidades.BBDD;
using static AppG.Servicio.GastoServicio;

namespace AppG.Servicio
{
    public interface IGastoServicio : IBaseServicio<Gasto>
    {
        void ExportarDatosExcelAsync(Excel<GastoDto> res);
    }

}