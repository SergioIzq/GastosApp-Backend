namespace AppG.Entidades.BBDD

{
    public class Resumen
    {
        public Resumen()
        {
        }

        public virtual decimal IngresosTotales { get; set; }

        public virtual decimal GastosTotales{get; set; }
        
        public virtual DateTime Periodo{get; set; }
    }

}

