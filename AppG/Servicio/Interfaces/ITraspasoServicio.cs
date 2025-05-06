using AppG.BBDD.Respuestas.Traspasos;
using AppG.Entidades.BBDD;
using static AppG.Servicio.TraspasoServicio;

namespace AppG.Servicio
{
    public interface ITraspasoServicio : IBaseServicio<Traspaso> {

        Task<TraspasoByIdRespuesta> GetTraspasoByIdAsync(int id);
        Task<List<Cuenta>> GetNewTraspasoAsync(int id);
        Task<Traspaso> RealizarTraspaso(Traspaso traspasoP);
        void ExportarDatosExcelAsync(Excel<TraspasoDto> res);


    }

}