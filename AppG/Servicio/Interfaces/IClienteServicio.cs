using AppG.Controllers;
using AppG.Entidades.BBDD;
using static AppG.Servicio.ClienteServicio;

namespace AppG.Servicio
{
    public interface IClienteServicio : IBaseServicio<Cliente> {
        void ExportarDatosExcelAsync(Excel<ClienteDto> res);

    }

}