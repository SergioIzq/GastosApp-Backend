using AppG.Controllers;
using AppG.Entidades.BBDD;
using static AppG.Servicio.CategoriaServicio;

namespace AppG.Servicio
{
    public interface ICategoriaServicio : IBaseServicio<Categoria> {
        void ExportarDatosExcelAsync(Excel<CategoriaDto> res);
    }

}