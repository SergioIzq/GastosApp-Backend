using AppG.Entidades.BBDD;

namespace AppG.BBDD.Respuestas.Ingresos
{
    public class IngresoRespuesta
    {
        public IngresoRespuesta()
        {
            ListaCuentas = new List<Cuenta>();
            ListaClientes = new List<Cliente>();
            ListaConceptos = new List<Concepto>();
            ListaCategorias = new List<Categoria>();
            ListaPersonas = new List<Persona>();
            ListaFormasPago = new List<FormaPago>();
        }

        public IList<Cuenta> ListaCuentas { get; set; }
        public IList<Cliente> ListaClientes { get; set; }
        public IList<Concepto> ListaConceptos { get; set; }
        public IList<Categoria> ListaCategorias { get; set; }
        public IList<Persona> ListaPersonas { get; set; }
        public IList<FormaPago> ListaFormasPago { get; set; }
    }
}
