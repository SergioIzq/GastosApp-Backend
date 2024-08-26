using AppG.Controllers;
using AppG.Entidades.BBDD;
using static AppG.Servicio.ProveedorServicio;

namespace AppG.Servicio
{
    public interface IProveedorServicio : IBaseServicio<Proveedor> {
        void ExportarDatosExcelAsync(Excel<ProveedorDto> res);

    }

}