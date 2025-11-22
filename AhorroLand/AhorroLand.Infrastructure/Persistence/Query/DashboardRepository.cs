using AhorroLand.Shared.Application.Dtos;
using Dapper;
using System.Globalization;
using ApplicationInterface = AhorroLand.Application.Interfaces;

namespace AhorroLand.Infrastructure.Persistence.Query;

/// <summary>
/// Implementación del repositorio de dashboard con métricas avanzadas y filtros.
/// </summary>
public sealed class DashboardRepository : ApplicationInterface.IDashboardRepository, IDashboardRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DashboardRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<DashboardResumenDto?> GetDashboardResumenAsync(
        Guid usuarioId,
      DateTime? fechaInicio = null,
     DateTime? fechaFin = null,
        Guid? cuentaId = null,
        Guid? categoriaId = null,
      CancellationToken cancellationToken = default)
    {
 using var connection = _dbConnectionFactory.CreateConnection();

  // Establecer fechas del período (mes actual por defecto)
        var primerDiaMesActual = fechaInicio ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var ultimoDiaMesActual = fechaFin ?? primerDiaMesActual.AddMonths(1).AddDays(-1);
      var primerDiaMesAnterior = primerDiaMesActual.AddMonths(-1);
        var ultimoDiaMesAnterior = primerDiaMesActual.AddDays(-1);

        // Calcular días del mes
 var diasTranscurridos = (DateTime.Now - primerDiaMesActual).Days + 1;
        var diasRestantes = (ultimoDiaMesActual - DateTime.Now).Days;
      var diasTotalesMes = (ultimoDiaMesActual - primerDiaMesActual).Days + 1;

        // Construir filtros adicionales
        var filtroCuenta = cuentaId.HasValue ? "AND cta.id = @CuentaId" : "";
        var filtroCategoria = categoriaId.HasValue ? "AND cat.id = @CategoriaId" : "";

   // 1. Balance total de cuentas (con filtro opcional)
   var sqlBalanceTotal = $@"
            SELECT COALESCE(SUM(saldo), 0) as Total
            FROM cuentas cta
          WHERE cta.id_usuario = @UsuarioId
   {(cuentaId.HasValue ? "AND cta.id = @CuentaId" : "")}";
        var balanceTotal = await connection.ExecuteScalarAsync<decimal>(
         sqlBalanceTotal,
            new { UsuarioId = usuarioId, CuentaId = cuentaId });

     // 2. Ingresos del mes actual (con filtros)
    var sqlIngresosMesActual = $@"
          SELECT COALESCE(SUM(i.importe), 0) as Total
            FROM ingresos i
       INNER JOIN cuentas cta ON i.id_cuenta = cta.id
  INNER JOIN conceptos con ON i.id_concepto = con.id
 INNER JOIN categorias cat ON con.id_categoria = cat.id
            WHERE i.id_usuario = @UsuarioId 
   AND i.fecha >= @FechaInicio 
            AND i.fecha <= @FechaFin
            {filtroCuenta}
    {filtroCategoria}";
     var ingresosMesActual = await connection.ExecuteScalarAsync<decimal>(
     sqlIngresosMesActual,
      new { UsuarioId = usuarioId, FechaInicio = primerDiaMesActual, FechaFin = ultimoDiaMesActual, CuentaId = cuentaId, CategoriaId = categoriaId });

        // 3. Gastos del mes actual (con filtros)
        var sqlGastosMesActual = $@"
     SELECT COALESCE(SUM(g.importe), 0) as Total
            FROM gastos g
            INNER JOIN cuentas cta ON g.id_cuenta = cta.id
   INNER JOIN conceptos con ON g.id_concepto = con.id
      INNER JOIN categorias cat ON con.id_categoria = cat.id
  WHERE g.id_usuario = @UsuarioId 
  AND g.fecha >= @FechaInicio 
  AND g.fecha <= @FechaFin
       {filtroCuenta}
            {filtroCategoria}";
        var gastosMesActual = await connection.ExecuteScalarAsync<decimal>(
  sqlGastosMesActual,
       new { UsuarioId = usuarioId, FechaInicio = primerDiaMesActual, FechaFin = ultimoDiaMesActual, CuentaId = cuentaId, CategoriaId = categoriaId });

        // ⚡ NUEVO: Cálculo de métricas avanzadas
        var gastoPromedioDiario = diasTranscurridos > 0 ? gastosMesActual / diasTranscurridos : 0;
    var proyeccionGastosFinMes = gastoPromedioDiario * diasTotalesMes;

        // 4. Resumen de cuentas
        var sqlCuentas = $@"
 SELECT 
          id as Id,
         nombre as Nombre,
    saldo as Saldo
   FROM cuentas
            WHERE id_usuario = @UsuarioId
  {(cuentaId.HasValue ? "AND id = @CuentaId" : "")}
     ORDER BY saldo DESC";
        var cuentas = (await connection.QueryAsync<CuentaResumenDto>(
            sqlCuentas,
  new { UsuarioId = usuarioId, CuentaId = cuentaId })).ToList();

     // 5. Top 5 categorías con más gastos
     var sqlTopCategorias = $@"
            SELECT 
    cat.id as CategoriaId,
        cat.nombre as CategoriaNombre,
SUM(g.importe) as TotalGastado,
       COUNT(*) as CantidadTransacciones,
         (SUM(g.importe) / NULLIF(@TotalGastos, 0) * 100) as PorcentajeDelTotal
            FROM gastos g
            INNER JOIN conceptos con ON g.id_concepto = con.id
  INNER JOIN categorias cat ON con.id_categoria = cat.id
    INNER JOIN cuentas cta ON g.id_cuenta = cta.id
          WHERE g.id_usuario = @UsuarioId 
            AND g.fecha >= @FechaInicio 
     AND g.fecha <= @FechaFin
            {filtroCuenta}
        {(categoriaId.HasValue ? "AND cat.id = @CategoriaId" : "")}
       GROUP BY cat.id, cat.nombre
     ORDER BY TotalGastado DESC
    LIMIT 5";
        var topCategorias = (await connection.QueryAsync<CategoriaGastoDto>(
  sqlTopCategorias,
  new
  {
              UsuarioId = usuarioId,
        FechaInicio = primerDiaMesActual,
     FechaFin = ultimoDiaMesActual,
   TotalGastos = gastosMesActual,
     CuentaId = cuentaId,
 CategoriaId = categoriaId
            })).ToList();

        // 6. Últimos 10 movimientos
        var sqlUltimosIngresos = $@"
      SELECT 
        i.id as Id,
                'Ingreso' as Tipo,
     i.importe as Importe,
    i.fecha as Fecha,
     con.nombre as Concepto,
cat.nombre as Categoria,
            cta.nombre as Cuenta
            FROM ingresos i
            INNER JOIN conceptos con ON i.id_concepto = con.id
    INNER JOIN categorias cat ON con.id_categoria = cat.id
         INNER JOIN cuentas cta ON i.id_cuenta = cta.id
       WHERE i.id_usuario = @UsuarioId
   {filtroCuenta}
   {filtroCategoria}
            ORDER BY i.fecha DESC
     LIMIT 5";
        var sqlUltimosGastos = $@"
            SELECT 
              g.id as Id,
    'Gasto' as Tipo,
        g.importe as Importe,
 g.fecha as Fecha,
  con.nombre as Concepto,
    cat.nombre as Categoria,
cta.nombre as Cuenta
     FROM gastos g
            INNER JOIN conceptos con ON g.id_concepto = con.id
            INNER JOIN categorias cat ON con.id_categoria = cat.id
        INNER JOIN cuentas cta ON g.id_cuenta = cta.id
       WHERE g.id_usuario = @UsuarioId
            {filtroCuenta}
       {filtroCategoria}
            ORDER BY g.fecha DESC
         LIMIT 5";
        var ultimosIngresos = await connection.QueryAsync<MovimientoResumenDto>(
 sqlUltimosIngresos,
  new { UsuarioId = usuarioId, CuentaId = cuentaId, CategoriaId = categoriaId });
      var ultimosGastos = await connection.QueryAsync<MovimientoResumenDto>(
      sqlUltimosGastos,
  new { UsuarioId = usuarioId, CuentaId = cuentaId, CategoriaId = categoriaId });
        var ultimosMovimientos = ultimosIngresos
    .Concat(ultimosGastos)
    .OrderByDescending(m => m.Fecha)
  .Take(10)
   .ToList();

        // 7. Comparativa con mes anterior
        var sqlIngresosMesAnterior = $@"
     SELECT COALESCE(SUM(i.importe), 0) as Total
       FROM ingresos i
            INNER JOIN cuentas cta ON i.id_cuenta = cta.id
    INNER JOIN conceptos con ON i.id_concepto = con.id
            INNER JOIN categorias cat ON con.id_categoria = cat.id
    WHERE i.id_usuario = @UsuarioId 
     AND i.fecha >= @FechaInicio 
  AND i.fecha <= @FechaFin
    {filtroCuenta}
     {filtroCategoria}";
        var ingresosMesAnterior = await connection.ExecuteScalarAsync<decimal>(
            sqlIngresosMesAnterior,
            new { UsuarioId = usuarioId, FechaInicio = primerDiaMesAnterior, FechaFin = ultimoDiaMesAnterior, CuentaId = cuentaId, CategoriaId = categoriaId });
        var sqlGastosMesAnterior = $@"
    SELECT COALESCE(SUM(g.importe), 0) as Total
    FROM gastos g
            INNER JOIN cuentas cta ON g.id_cuenta = cta.id
         INNER JOIN conceptos con ON g.id_concepto = con.id
  INNER JOIN categorias cat ON con.id_categoria = cat.id
   WHERE g.id_usuario = @UsuarioId 
        AND g.fecha >= @FechaInicio 
AND g.fecha <= @FechaFin
      {filtroCuenta}
            {filtroCategoria}";
        var gastosMesAnterior = await connection.ExecuteScalarAsync<decimal>(
         sqlGastosMesAnterior,
      new { UsuarioId = usuarioId, FechaInicio = primerDiaMesAnterior, FechaFin = ultimoDiaMesAnterior, CuentaId = cuentaId, CategoriaId = categoriaId });
        var diferenciaIngresos = ingresosMesActual - ingresosMesAnterior;
        var diferenciaGastos = gastosMesActual - gastosMesAnterior;
        var porcentajeCambioIngresos = ingresosMesAnterior > 0
 ? (diferenciaIngresos / ingresosMesAnterior) * 100
            : 0;
 var porcentajeCambioGastos = gastosMesAnterior > 0
   ? (diferenciaGastos / gastosMesAnterior) * 100
      : 0;

        // ⚡ NUEVO: 8. Histórico de los últimos 6 meses
        var historicoUltimos6Meses = await ObtenerHistoricoUltimos6MesesAsync(
            connection, usuarioId, cuentaId, categoriaId, filtroCuenta, filtroCategoria);
     // ⚡ NUEVO: 9. Generar alertas
        var alertas = GenerarAlertas(
            gastosMesActual,
     ingresosMesActual,
        gastoPromedioDiario,
     proyeccionGastosFinMes,
 porcentajeCambioGastos,
            balanceTotal);
 // Construir el DTO de respuesta
        var resumen = new DashboardResumenDto
  {
            BalanceTotal = balanceTotal,
            IngresosMesActual = ingresosMesActual,
            GastosMesActual = gastosMesActual,
   BalanceMesActual = ingresosMesActual - gastosMesActual,
    TotalCuentas = cuentas.Count,
      Cuentas = cuentas,
       TopCategoriasGastos = topCategorias,
            UltimosMovimientos = ultimosMovimientos,
    ComparativaMesAnterior = new ComparativaMensualDto
            {
           IngresosMesAnterior = ingresosMesAnterior,
   GastosMesAnterior = gastosMesAnterior,
    DiferenciaIngresos = diferenciaIngresos,
                DiferenciaGastos = diferenciaGastos,
 PorcentajeCambioIngresos = porcentajeCambioIngresos,
                PorcentajeCambioGastos = porcentajeCambioGastos
            },
       // ⚡ Nuevas propiedades
 GastoPromedioDiario = gastoPromedioDiario,
         ProyeccionGastosFinMes = proyeccionGastosFinMes,
            DiasTranscurridosMes = diasTranscurridos,
DiasRestantesMes = diasRestantes,
HistoricoUltimos6Meses = historicoUltimos6Meses,
            Alertas = alertas
     };
        return resumen;
  }

    /// <summary>
    /// Obtiene el histórico de ingresos y gastos de los últimos 6 meses.
  /// </summary>
    private async Task<List<HistoricoMensualDto>> ObtenerHistoricoUltimos6MesesAsync(
        System.Data.IDbConnection connection,
        Guid usuarioId,
      Guid? cuentaId,
        Guid? categoriaId,
        string filtroCuenta,
   string filtroCategoria)
    {
        var historico = new List<HistoricoMensualDto>();
      var fechaActual = DateTime.Now;
        for (int i = 0; i < 6; i++)
        {
            var mes = fechaActual.AddMonths(-i);
            var primerDia = new DateTime(mes.Year, mes.Month, 1);
    var ultimoDia = primerDia.AddMonths(1).AddDays(-1);
            var sqlIngresos = $@"
     SELECT COALESCE(SUM(i.importe), 0)
 FROM ingresos i
         INNER JOIN cuentas cta ON i.id_cuenta = cta.id
    INNER JOIN conceptos con ON i.id_concepto = con.id
      INNER JOIN categorias cat ON con.id_categoria = cat.id
      WHERE i.id_usuario = @UsuarioId
        AND i.fecha >= @FechaInicio
        AND i.fecha <= @FechaFin
        {filtroCuenta}
       {filtroCategoria}";
            var sqlGastos = $@"
           SELECT COALESCE(SUM(g.importe), 0)
 FROM gastos g
       INNER JOIN cuentas cta ON g.id_cuenta = cta.id
         INNER JOIN conceptos con ON g.id_concepto = con.id
    INNER JOIN categorias cat ON con.id_categoria = cat.id
     WHERE g.id_usuario = @UsuarioId
 AND g.fecha >= @FechaInicio
         AND g.fecha <= @FechaFin
     {filtroCuenta}
  {filtroCategoria}";
  var ingresos = await connection.ExecuteScalarAsync<decimal>(
    sqlIngresos,
        new { UsuarioId = usuarioId, FechaInicio = primerDia, FechaFin = ultimoDia, CuentaId = cuentaId, CategoriaId = categoriaId });
var gastos = await connection.ExecuteScalarAsync<decimal>(
    sqlGastos,
         new { UsuarioId = usuarioId, FechaInicio = primerDia, FechaFin = ultimoDia, CuentaId = cuentaId, CategoriaId = categoriaId });
      historico.Add(new HistoricoMensualDto
          {
   Anio = mes.Year,
    Mes = mes.Month,
                MesNombre = new CultureInfo("es-ES").DateTimeFormat.GetMonthName(mes.Month),
         TotalIngresos = ingresos,
     TotalGastos = gastos,
 Balance = ingresos - gastos
 });
        }
        return historico;
    }

    /// <summary>
    /// Genera alertas inteligentes basadas en las métricas del dashboard.
    /// </summary>
    private List<AlertaDto> GenerarAlertas(
     decimal gastosMesActual,
        decimal ingresosMesActual,
   decimal gastoPromedioDiario,
        decimal proyeccionGastosFinMes,
        decimal porcentajeCambioGastos,
        decimal balanceTotal)
    {
        var alertas = new List<AlertaDto>();
        // Alerta: Proyección de gastos supera ingresos
        if (proyeccionGastosFinMes > ingresosMesActual && ingresosMesActual > 0)
        {
         alertas.Add(new AlertaDto
            {
 Tipo = "warning",
     Titulo = "Proyección de gastos alta",
     Mensaje = $"A este ritmo, tus gastos superarán tus ingresos en {Math.Abs(proyeccionGastosFinMes - ingresosMesActual):C2}",
              Icono = "⚠️"
            });
        }

   // Alerta: Incremento significativo en gastos
        if (porcentajeCambioGastos > 20)
        {
       alertas.Add(new AlertaDto
  {
         Tipo = "danger",
     Titulo = "Gastos aumentaron significativamente",
                Mensaje = $"Tus gastos han aumentado un {porcentajeCambioGastos:F1}% respecto al mes anterior",
   Icono = "🔴"
            });
}

        // Alerta: Balance negativo
        if (ingresosMesActual - gastosMesActual < 0)
        {
alertas.Add(new AlertaDto
          {
       Tipo = "danger",
             Titulo = "Balance negativo este mes",
    Mensaje = $"Estás gastando más de lo que ingresas: {Math.Abs(ingresosMesActual - gastosMesActual):C2} en negativo",
     Icono = "❌"
            });
 }

        // Alerta: Balance total bajo
        if (balanceTotal < gastoPromedioDiario * 30 && balanceTotal > 0)
        {
            alertas.Add(new AlertaDto
 {
            Tipo = "warning",
                Titulo = "Fondo de emergencia bajo",
     Mensaje = "Tu balance total no cubre un mes de gastos. Considera aumentar tus ahorros",
                Icono = "⚠️"
   });
        }

        // Alerta positiva: Ahorro este mes
     if (ingresosMesActual > gastosMesActual && gastosMesActual > 0)
        {
 var ahorro = ingresosMesActual - gastosMesActual;
            var porcentajeAhorro = (ahorro / ingresosMesActual) * 100;
     alertas.Add(new AlertaDto
      {
         Tipo = "success",
       Titulo = "¡Excelente! Estás ahorrando",
       Mensaje = $"Has ahorrado {ahorro:C2} este mes ({porcentajeAhorro:F1}% de tus ingresos)",
   Icono = "✅"
            });
        }

    // Alerta: Sin movimientos
        if (gastosMesActual == 0 && ingresosMesActual == 0)
        {
            alertas.Add(new AlertaDto
            {
      Tipo = "info",
           Titulo = "Sin movimientos registrados",
      Mensaje = "No tienes transacciones registradas en este período",
            Icono = "ℹ️"
            });
        }

        return alertas;
    }
}
