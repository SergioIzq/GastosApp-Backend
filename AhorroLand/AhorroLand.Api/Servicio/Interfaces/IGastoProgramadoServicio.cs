using AppG.BBDD.Respuestas.Gastos;
using AppG.Entidades.BBDD;

namespace AppG.Servicio
{
    public interface IGastoProgramadoServicio : IBaseServicio<GastoProgramado>
    {
        Task<GastoRespuesta> GetNewGastoAsync(int idUsuario);
        Task<GastoProgramadoByIdRespuesta> GetGastoByIdAsync(int id);
        Task AplicarGasto(int gastoProgramadoId);
    }

}