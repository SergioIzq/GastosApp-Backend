using AppG.Servicio.Base;

namespace AppG.Entidades.BBDD

{
    public abstract class Entidad : IExportable
    {
        public Entidad()
        {
            Id = 0;
            IdUsuario = 0;
            FechaCreacion = DateTime.Now;
        }

        public virtual int Id { get; set; }

        public virtual int IdUsuario { get; set; }

        public virtual DateTime FechaCreacion { get; set; }

        public virtual object? GetValueByName(string propertyName)
        {
            return GetType().GetProperty(propertyName)?.GetValue(this);
        }
    }

}

