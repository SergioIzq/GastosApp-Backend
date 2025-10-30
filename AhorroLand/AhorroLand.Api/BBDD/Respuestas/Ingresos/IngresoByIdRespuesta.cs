using AppG.Entidades.BBDD;

namespace AppG.BBDD.Respuestas.Ingresos
{
    public class IngresoByIdRespuesta
    {
        public IngresoByIdRespuesta()
        {
            IngresoById = new Ingreso();
            IngresoRespuesta = new IngresoRespuesta();
        }

        public Ingreso IngresoById { get; set; }
        public IngresoRespuesta IngresoRespuesta { get; set; }
    }
}
