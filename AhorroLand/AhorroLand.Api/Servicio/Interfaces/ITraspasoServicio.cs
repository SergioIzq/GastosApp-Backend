using AppG.BBDD.Respuestas.Traspasos;
using AppG.Entidades.BBDD;

namespace AppG.Servicio
{
    public interface ITraspasoServicio : IBaseServicio<Traspaso>
    {
        Task<TraspasoByIdRespuesta> GetTraspasoByIdAsync(int id);
        Task<List<Cuenta>> GetNewTraspasoAsync(int id);
        Task<Traspaso> RealizarTraspaso(Traspaso traspasoP, bool esProgramado);
    }

}