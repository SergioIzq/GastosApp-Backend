using System;
using System.ComponentModel.DataAnnotations;

namespace AppG.Entidades.BBDD
{
    public class Gasto : Entidad
    {
        public Gasto()
        {
        }

        private int _Id;
        public virtual int Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                if (this._Id != value)
                {
                    this._Id = value;
                }
            }
        }

        private decimal _Monto;
        public virtual decimal Monto
        {
            get
            {
                return this._Monto;
            }
            set
            {
                if (this._Monto != value)
                {
                    this._Monto = value;
                }
            }
        }

        private DateTime _Fecha;
        public virtual DateTime Fecha
        {
            get
            {
                return this._Fecha;
            }
            set
            {
                if (this._Fecha != value)
                {
                    this._Fecha = value;
                }
            }
        }

        public virtual Concepto Concepto { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        public virtual Persona Persona { get; set; }
        public virtual Cuenta Cuenta { get; set; }
        public virtual FormaPago FormaPago { get; set; }


        private string? _Descripcion;
        public virtual string? Descripcion
        {
            get
            {
                return this._Descripcion;
            }
            set
            {
                if (this._Descripcion != value)
                {
                    this._Descripcion = value;
                }
            }
        }

    }
}
