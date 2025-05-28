using AppG.BBDD.Excel;
using AppG.Servicio.Base;

namespace AppG.Servicio
{
    public interface IExcelServicio
    {
        Task<byte[]> ExportarExcelAsync<T>(ExportarOpciones opciones) where T : class, IExportable;
    }
}
