using AppG.Entidades.BBDD;

namespace AppG.BBDD.Respuestas.Ingresos
{
    public class IngresoProgramadoByIdRespuesta
    {
        public IngresoProgramadoByIdRespuesta()
        {
            IngresoProgramadoById = new IngresoProgramado();
            IngresoRespuesta = new IngresoRespuesta();
        }

        public IngresoProgramado IngresoProgramadoById { get; set; }
        public IngresoRespuesta IngresoRespuesta { get; set; }
    }
}
