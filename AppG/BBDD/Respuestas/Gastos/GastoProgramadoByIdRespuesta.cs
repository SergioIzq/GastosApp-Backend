using AppG.Entidades.BBDD;

namespace AppG.BBDD.Respuestas.Gastos
{
    public class GastoProgramadoByIdRespuesta
    {
        public GastoProgramadoByIdRespuesta()
        {
            GastoById = new GastoProgramado();
            GastoRespuesta = new GastoRespuesta();
        }

        public GastoProgramado GastoById { get; set; }
        public GastoRespuesta GastoRespuesta { get; set; }
    }
}
