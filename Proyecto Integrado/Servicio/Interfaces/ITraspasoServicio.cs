using AppG.Controllers;
using AppG.Entidades.BBDD;
using static AppG.Servicio.TraspasoServicio;

namespace AppG.Servicio
{
    public interface ITraspasoServicio : IBaseServicio<Traspaso> {


        Task<Traspaso> RealizarTraspaso(Traspaso traspasoP);
        void ExportarDatosExcelAsync(Excel<TraspasoDto> res);


    }

}