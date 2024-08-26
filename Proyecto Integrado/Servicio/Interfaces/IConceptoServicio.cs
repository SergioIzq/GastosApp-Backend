using AppG.Entidades.BBDD;
using static AppG.Servicio.ConceptoServicio;

namespace AppG.Servicio
{
    public interface IConceptoServicio : IBaseServicio<Concepto>
    {
        void ExportarDatosExcelAsync(Excel<ConceptoDto> res);

    }
}
