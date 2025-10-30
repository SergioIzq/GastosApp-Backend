using AppG.BBDD;
using AppG.Entidades.BBDD;
using NHibernate;
using NHibernate.Criterion;
using OfficeOpenXml;
using System.Diagnostics;

namespace AppG.Servicio
{
    public class ResumenServicio : IResumenServicio
    {
        private readonly ISessionFactory _sessionFactory;

        public ResumenServicio(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public virtual async Task<ResumenGastosResponse> GetGastosAsync(int page, int size, string periodoInicio, string periodoFin, int idUsuario)
        {
            try
            {
                using (var session = _sessionFactory.OpenSession())
                {
                    // Convertir los periodos de string a DateTime
                    DateTime inicio = DateTime.Parse(periodoInicio);
                    DateTime fin = DateTime.Parse(periodoFin);

                    // Consultas SQL directas para sumar los importes en la tabla Ingreso
                    string gastosSql = @"
                        SELECT COALESCE(SUM(Importe), 0) 
                        FROM gasto 
                        WHERE Fecha BETWEEN :Inicio AND :Fin AND id_Usuario = :IdUsuario";

                    // Ejecutar la consulta para gastos
                    decimal gastosTotales = await session
                        .CreateSQLQuery(gastosSql)
                        .SetParameter("Inicio", inicio)
                        .SetParameter("Fin", fin)
                        .SetParameter("IdUsuario", idUsuario)
                        .UniqueResultAsync<decimal>();

                    int gastosTotalCount = await session.QueryOver<Gasto>()
                        .Where(g => g.Fecha >= inicio && g.Fecha <= fin)  // Asegurarse que las fechas son inclusivas
                        .And(Restrictions.Eq("IdUsuario", idUsuario))
                        .RowCountAsync();

                    var gastosDetalles = await session.QueryOver<Gasto>()
                        .Where(g => g.Fecha >= inicio && g.Fecha <= fin)  // Asegurarse que las fechas son inclusivas
                        .And(Restrictions.Eq("IdUsuario", idUsuario))
                        .Skip((page - 1) * size)
                        .Take(size)
                        .ListAsync();


                    // Crear la respuesta con el total de registros y los elementos obtenidos
                    var response = new ResumenGastosResponse(
                        gastosTotales,
                        gastosTotalCount,
                        gastosDetalles
                    );

                    return response;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                throw new Exception($"Error al obtener gastos: {ex.Message}", ex);
            }
        }

        public virtual async Task<ResumenIngresosResponse> GetIngresosAsync(int page, int size, string periodoInicio, string periodoFin, int idUsuario)
        {
            try
            {
                using (var session = _sessionFactory.OpenSession())
                {
                    // Convertir los periodos de string a DateTime
                    DateTime inicio = DateTime.Parse(periodoInicio).Date; // Esto establece la hora a 00:00:00
                    DateTime fin = DateTime.Parse(periodoFin).Date.AddDays(1).AddSeconds(-1); // Esto establece la hora a 23:59:59 del mismo día

                    // Consultas SQL directas para sumar los importes en la tabla Ingreso
                    string ingresosSql = @"
                        SELECT COALESCE(SUM(Importe), 0) 
                        FROM ingreso 
                        WHERE Fecha BETWEEN :Inicio AND :Fin AND id_usuario = :IdUsuario";

                    // Ejecutar la consulta para ingresos
                    decimal ingresosTotales = await session
                        .CreateSQLQuery(ingresosSql)
                        .SetParameter("Inicio", inicio)
                        .SetParameter("Fin", fin)
                        .SetParameter("IdUsuario", idUsuario)
                        .UniqueResultAsync<decimal>();

                    int ingresosTotalCount = await session.QueryOver<Ingreso>()
                        .Where(i => i.Fecha >= inicio && i.Fecha <= fin)  // Asegurarse que las fechas son inclusivas
                        .And(Restrictions.Eq("IdUsuario", idUsuario))
                        .RowCountAsync();

                    var ingresosDetalles = await session.QueryOver<Ingreso>()
                        .Where(i => i.Fecha >= inicio && i.Fecha <= fin)  // Asegurarse que las fechas son inclusivas
                        .And(Restrictions.Eq("IdUsuario", idUsuario))
                        .Skip((page - 1) * size)
                        .Take(size)
                        .ListAsync();


                    // Crear la respuesta con el total de registros y los elementos obtenidos
                    var response = new ResumenIngresosResponse(
                        ingresosTotales,
                        ingresosTotalCount,
                        ingresosDetalles
                    );

                    return response;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                throw new Exception($"Error al obtener ingresos: {ex.Message}", ex);
            }
        }

        public void ExportarDatosExcelAsync(ExportExcelRequest request)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var directorioPath = request.DirPath;

            // Comprobar si la ruta del directorio es válida
            if (!Directory.Exists(directorioPath))
            {
                throw new DirectoryNotFoundException($"El directorio especificado no existe: {directorioPath}");
            }

            // Definir la ruta completa del archivo
            var filePath = Path.Combine(directorioPath, "resumen.xlsx");

            var exportData = new List<dynamic>();

            // Convertir la lista de gastos a un formato adecuado para Excel
            exportData.AddRange(request.Datos.Gastos.Select(item => new
            {
                TipoOperacion = "Gasto",
                Fecha = item.Fecha.ToString("dd/MM/yyyy"),
                Persona = item.Persona?.Nombre ?? string.Empty,
                FormaPago = item.FormaPago?.Nombre ?? string.Empty,
                Proveedor = item.Proveedor?.Nombre ?? string.Empty,
                Categoria = item.Concepto?.Nombre ?? string.Empty,
                Concepto = item.Concepto?.Nombre ?? string.Empty,
                Cuenta = item.Cuenta?.Nombre ?? string.Empty,
                Importe = $"-{item.Importe}",
                Cliente = string.Empty  // Cliente no aplicable para Gasto
            }));

            // Convertir la lista de ingresos a un formato adecuado para Excel
            exportData.AddRange(request.Datos.Ingresos.Select(item => new
            {
                TipoOperacion = "Ingreso",
                Fecha = item.Fecha.ToString("dd/MM/yyyy"),
                Persona = item.Persona?.Nombre ?? string.Empty,
                FormaPago = item.FormaPago?.Nombre ?? string.Empty,
                Cliente = item.Cliente?.Nombre ?? string.Empty,
                Categoria = item.Concepto?.Categoria?.Nombre ?? string.Empty,
                Concepto = item.Concepto?.Nombre ?? string.Empty,
                Cuenta = item.Cuenta?.Nombre ?? string.Empty,
                Importe = $"+{item.Importe}",
                Proveedor = string.Empty
            }));

            // Añadir la fila con los totales
            var beneficiosTotales = request.Datos.IngresosTotales - request.Datos.GastosTotales;
            var beneficioFormateado = beneficiosTotales >= 0 ? $"+{beneficiosTotales}" : $"{beneficiosTotales}";

            exportData.Add(new
            {
                TipoOperacion = "Total",
                Fecha = "",
                Persona = "",
                FormaPago = "",
                Proveedor = "",
                Cliente = "",
                Categoria = "",
                Concepto = "",
                Cuenta = "",
                Importe = "",
                IngresosTotales = request.Datos.IngresosTotales,
                GastosTotales = request.Datos.GastosTotales,
                BeneficiosTotales = beneficioFormateado
            });

            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet;

                if (File.Exists(filePath))
                {
                    try
                    {
                        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
                        {
                            package.Load(stream);
                        }
                    }
                    catch (FileLoadException)
                    {

                    }
                    worksheet = package.Workbook.Worksheets["Resumen"];

                    if (worksheet == null)
                    {
                        worksheet = package.Workbook.Worksheets.Add("Resumen");
                    }
                }
                else
                {
                    worksheet = package.Workbook.Worksheets.Add("Resumen");
                }

                worksheet.Cells.Clear();

                // Establecer las cabeceras de las columnas
                worksheet.Cells["A1"].Value = "Tipo Operacion";
                worksheet.Cells["B1"].Value = "Fecha";
                worksheet.Cells["C1"].Value = "Persona";
                worksheet.Cells["D1"].Value = "Forma de Pago";
                worksheet.Cells["E1"].Value = "Proveedor";
                worksheet.Cells["F1"].Value = "Cliente";
                worksheet.Cells["G1"].Value = "Categoria";
                worksheet.Cells["H1"].Value = "Concepto";
                worksheet.Cells["I1"].Value = "Cuenta";
                worksheet.Cells["J1"].Value = "Importe";
                worksheet.Cells["K1"].Value = "Beneficios Totales";
                worksheet.Cells["L1"].Value = "Ingresos Totales";
                worksheet.Cells["M1"].Value = "Gastos Totales";

                // Cargar los datos manualmente a partir de la fila 2
                var row = 2;
                foreach (var item in exportData.Take(exportData.Count - 1))
                {
                    worksheet.Cells[row, 1].Value = item.TipoOperacion;
                    worksheet.Cells[row, 2].Value = item.Fecha;
                    worksheet.Cells[row, 3].Value = item.Persona;
                    worksheet.Cells[row, 4].Value = item.FormaPago;
                    worksheet.Cells[row, 5].Value = item.Proveedor;
                    worksheet.Cells[row, 6].Value = item.Cliente;
                    worksheet.Cells[row, 7].Value = item.Categoria;
                    worksheet.Cells[row, 8].Value = item.Concepto;
                    worksheet.Cells[row, 9].Value = item.Cuenta;
                    worksheet.Cells[row, 10].Value = item.Importe;
                    row++;
                }

                // Añadir la fila de totales
                worksheet.Cells[row, 1].Value = "Totales";
                worksheet.Cells[row, 2].Value = ""; // Fecha
                worksheet.Cells[row, 3].Value = ""; // Persona
                worksheet.Cells[row, 4].Value = ""; // Forma de Pago
                worksheet.Cells[row, 5].Value = ""; // Proveedor
                worksheet.Cells[row, 6].Value = ""; // Cliente
                worksheet.Cells[row, 7].Value = ""; // Categoria
                worksheet.Cells[row, 8].Value = ""; // Concepto
                worksheet.Cells[row, 9].Value = ""; // Cuenta
                worksheet.Cells[row, 10].Value = ""; // Importe
                worksheet.Cells[row, 11].Value = beneficioFormateado; // Beneficios Totales
                worksheet.Cells[row, 12].Value = $"+{request.Datos.IngresosTotales}"; // Ingresos Totales
                worksheet.Cells[row, 13].Value = $"-{request.Datos.GastosTotales}"; // Gastos Totales

                row++;

                if (worksheet.Dimension != null)
                {
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                }

                FileInfo fileInfo = new FileInfo(filePath);
                package.SaveAs(fileInfo);

                // Abrir el archivo en Excel
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
        }

    }
}





