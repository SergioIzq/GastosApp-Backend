using AppG.BBDD.Respuestas.Gastos;
using AppG.Entidades.BBDD;
using static AppG.Servicio.GastoServicio;

namespace AppG.Servicio
{
    public interface IGastoServicio : IBaseServicio<Gasto>
    {
        void ExportarDatosExcelAsync(Excel<GastoDto> res);
        Task<GastoRespuesta> GetNewGastoAsync(int idUsuario);
        Task<GastoByIdRespuesta> GetGastoByIdAsync(int id);
        Task<Gasto> CreateAsync(Gasto entity, bool esProgramado);
    }

}