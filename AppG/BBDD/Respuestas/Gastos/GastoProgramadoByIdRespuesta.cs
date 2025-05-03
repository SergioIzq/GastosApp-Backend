using AppG.Entidades.BBDD;

namespace AppG.BBDD.Respuestas.Gastos
{
    public class GastoProgramadoByIdRespuesta
    {
        public GastoProgramadoByIdRespuesta()
        {
            GastoProgramadoById = new GastoProgramado();
            GastoRespuesta = new GastoRespuesta();
        }

        public GastoProgramado GastoProgramadoById { get; set; }
        public GastoRespuesta GastoRespuesta { get; set; }
    }
}
