using AppG.Entidades.BBDD;

namespace AppG.BBDD.Respuestas.Gastos
{
    public class GastoRespuesta
    {
        public GastoRespuesta()
        {
            ListaCuentas = new List<Cuenta>();
            ListaProveedores = new List<Proveedor>();
            ListaConceptos = new List<Concepto>();
            ListaCategorias = new List<Categoria>();
            ListaPersonas = new List<Persona>();
            ListaFormasPago = new List<FormaPago>();
        }

        public IList<Cuenta> ListaCuentas { get; set; }
        public IList<Proveedor> ListaProveedores { get; set; }
        public IList<Concepto> ListaConceptos { get; set; }
        public IList<Categoria> ListaCategorias { get; set; }
        public IList<Persona> ListaPersonas { get; set; }
        public IList<FormaPago> ListaFormasPago { get; set; }
    }
}
