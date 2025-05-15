using AppG.Entidades.BBDD;

namespace AppG.BBDD.Respuestas.Traspasos
{
    public class TraspasoByIdRespuesta
    {
        public TraspasoByIdRespuesta()
        {
            TraspasoById = new Traspaso();
            ListaCuentas = new List<Cuenta>();
        }

        public Traspaso TraspasoById { get; set; }
        public IList<Cuenta> ListaCuentas { get; set; }
    }
}