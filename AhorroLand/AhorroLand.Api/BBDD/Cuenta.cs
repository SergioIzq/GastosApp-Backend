namespace AppG.Entidades.BBDD

{
    public class Cuenta : Entidad
    {
        public Cuenta()
        {


        }

        public virtual string Nombre { get; set; } = string.Empty;

        public virtual decimal Saldo { get; set; }
    }

}

