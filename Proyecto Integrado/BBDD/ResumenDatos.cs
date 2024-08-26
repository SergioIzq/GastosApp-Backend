using AppG.Entidades.BBDD;

namespace AppG.BBDD
{
    public class ResumenDatos
    {
        public List<Gasto> Gastos { get; set; } = new List<Gasto>();
        public List<Ingreso> Ingresos { get; set; } = new List<Ingreso>();
        public decimal IngresosTotales { get; set; }
        public decimal GastosTotales { get; set; }
    }
}
