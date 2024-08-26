using AppG.Controllers;
using AppG.Entidades.BBDD;
using static AppG.Servicio.PersonaServicio;

namespace AppG.Servicio
{
    public interface IPersonaServicio : IBaseServicio<Persona> {
        void ExportarDatosExcelAsync(Excel<PersonaDto> res);

    }

}