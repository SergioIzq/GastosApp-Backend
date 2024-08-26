using AppG.Entidades.BBDD;

public class ResumenGastosResponse
{
    public decimal GastosTotales { get; set; }
    public int GastosTotalCount { get; set; }
    public IList<Gasto> Gastos { get; set; }

    public ResumenGastosResponse(decimal gastosTotales, int gastosTotalCount, IList<Gasto> gastosDetalles)
    {
        GastosTotales = gastosTotales;
        GastosTotalCount = gastosTotalCount;
        Gastos = gastosDetalles;
    }
}
