namespace AppG.Entidades.BBDD

{
    public class Usuario : Entidad
    {
        public Usuario()
        {
            IdUsuario = Id;
            FechaCreacion = DateTime.Now;
        }

        public virtual string Correo { get; set; } = string.Empty;

        public virtual string Contrasena { get; set; } = string.Empty;
        public virtual string? TokenConfirmacion { get; set; } = null;
        public virtual bool Activo { get; set; }
    }

}

