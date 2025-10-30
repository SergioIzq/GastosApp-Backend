using AppG.Entidades.BBDD;

namespace AppG.BBDD.Respuestas.Traspasos
{
    public class TraspasoProgramadoByIdRespuesta
    {
        public TraspasoProgramadoByIdRespuesta()
        {
            TraspasoProgramadoById = new TraspasoProgramado();
            ListaCuentas = new List<Cuenta>();
        }

        public TraspasoProgramado TraspasoProgramadoById { get; set; }
        public IList<Cuenta> ListaCuentas { get; set; }
    }
}