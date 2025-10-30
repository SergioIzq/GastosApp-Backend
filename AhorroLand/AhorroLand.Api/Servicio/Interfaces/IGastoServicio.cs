using AppG.BBDD.Respuestas.Gastos;
using AppG.Entidades.BBDD;

namespace AppG.Servicio
{
    public interface IGastoServicio : IBaseServicio<Gasto>
    {
        Task<GastoRespuesta> GetNewGastoAsync(int idUsuario);
        Task<GastoByIdRespuesta> GetGastoByIdAsync(int id);
        Task<Gasto> CreateAsync(Gasto entity, bool esProgramado);
    }

}