using AppG.BBDD.Respuestas.Traspasos;
using AppG.Entidades.BBDD;

namespace AppG.Servicio
{
    public interface ITraspasoProgramadoServicio : IBaseServicio<TraspasoProgramado>
    {
        Task<List<Cuenta>> GetNewTraspasoAsync(int idUsuario);
        Task<TraspasoProgramadoByIdRespuesta> GetTraspasoByIdAsync(int id);
        Task AplicarTraspaso(int traspasoProgramadoId);
    }
}