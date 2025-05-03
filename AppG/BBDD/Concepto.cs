namespace AppG.Entidades.BBDD

{
    public class Concepto : Entidad
    {
        public Concepto()
        {

          
        }
        
        public virtual string Nombre { get; set; } = string.Empty;

        public virtual Categoria Categoria { get; set; } = new Categoria();
    }

}

