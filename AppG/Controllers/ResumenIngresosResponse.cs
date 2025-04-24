using AppG.Entidades.BBDD;

public class ResumenIngresosResponse
{
    public decimal IngresosTotales { get; set; }
    public int IngresosTotalCount { get; set; }
    public IList<Ingreso> Ingresos { get; set; }

    public ResumenIngresosResponse(decimal ingresosTotales, int ingresosTotalCount, IList<Ingreso> ingresosDetalles)
    {
        IngresosTotales = ingresosTotales;
        IngresosTotalCount = ingresosTotalCount;
        Ingresos = ingresosDetalles;
    }
}
