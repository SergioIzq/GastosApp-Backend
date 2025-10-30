using AppG.BBDD.Excel;
using AppG.Entidades.BBDD;
using AppG.Servicio.Base;
using NHibernate;
using OfficeOpenXml;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace AppG.Servicio
{
    public class ExcelServicio : IExcelServicio
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExcelServicio(ISessionFactory sessionFactory, IHttpContextAccessor httpContextAccessor)
        {
            _sessionFactory = sessionFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<byte[]> ExportarExcelAsync<T>(ExportarOpciones opciones) where T : class, IExportable
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            IList<T> datos;

            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            IQueryable<T> query = session.Query<T>();

            var idUsuario = ObtenerIdUsuario();

            if (typeof(T).GetProperty("IdUsuario") != null)
            {
                var parameterWhere = Expression.Parameter(typeof(T), "x");
                var propiedadIdUsuario = Expression.Property(parameterWhere, "IdUsuario");
                var constanteIdUsuario = Expression.Constant(idUsuario);
                var igual = Expression.Equal(propiedadIdUsuario, constanteIdUsuario);

                var lambdaWhere = Expression.Lambda<Func<T, bool>>(igual, parameterWhere);
                query = query.Where(lambdaWhere);
            }

            // Orden dinámico
            string nombrePropOrden = typeof(T) == typeof(Gasto)
                                    || typeof(T) == typeof(Ingreso)
                                    || typeof(T) == typeof(Traspaso)
                                    ? "Fecha"
                                    : "FechaCreacion";

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, nombrePropOrden);

            var propType = typeof(T).GetProperty(nombrePropOrden)!.PropertyType;
            Type delegateType;

            if (propType == typeof(DateTime))
                delegateType = typeof(Func<,>).MakeGenericType(typeof(T), typeof(DateTime));
            else if (propType == typeof(DateTime?))
                delegateType = typeof(Func<,>).MakeGenericType(typeof(T), typeof(DateTime?));
            else
                throw new Exception($"La propiedad {nombrePropOrden} debe ser DateTime o DateTime?");

            var lambdaOrder = Expression.Lambda(delegateType, property, parameter);

            var orderByDescendingMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), propType);

            query = (IQueryable<T>)orderByDescendingMethod.Invoke(null, new object[] { query, lambdaOrder })!;

            if (opciones.Origen == "tabla")
            {
                query = query
                    .Skip(((opciones.Pagina ?? 1) - 1) * (opciones.Tamano ?? 10))
                    .Take(opciones.Tamano ?? 10);
            }

            datos = query.ToList();

            tx.Commit();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add(opciones.NombreArchivo);

            opciones.Columnas = ObtenerColumnas(typeof(T), opciones.Columnas);

            // Cabeceras
            for (int i = 0; i < opciones.Columnas.Count; i++)
            {
                var cell = worksheet.Cells[1, i + 1];
                var nombreCompleto = opciones.Columnas[i];
                var partes = nombreCompleto.Split('.');

                string textoVisible;

                if (partes.Length == 1)
                {
                    textoVisible = partes[0];
                }
                else if (partes.Length > 1)
                {
                    var entidad = partes[0];
                    var propiedad = partes.Last();

                    textoVisible = $"{entidad} ({propiedad})";

                    if (entidad == "Concepto" && partes.Length > 2)
                    {
                        if (partes[1] == "Categoria")
                            textoVisible = $"Categoría ({propiedad})";
                    }
                }
                else
                {
                    textoVisible = nombreCompleto;
                }

                // Aplica split camel case solo para mostrar
                textoVisible = SplitCamelCase(textoVisible);

                cell.Value = textoVisible;

                var comentarioFormateado = string.Join(" > ", partes);
                var comment = cell.AddComment(comentarioFormateado, "Sistema");
                comment.AutoFit = true;

                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                cell.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                cell.Style.Font.Size = 12;
            }

            // Datos
            for (int row = 0; row < datos.Count; row++)
            {
                for (int col = 0; col < opciones.Columnas.Count; col++)
                {
                    var cell = worksheet.Cells[row + 2, col + 1];
                    var nombreColumna = opciones.Columnas[col];
                    var valor = GetValueByNameExtendido(datos[row], nombreColumna);

                    // Comprobamos si la columna contiene 'Programado'
                    bool tipoEsProgramado = typeof(T).Name.Contains("Programado", StringComparison.OrdinalIgnoreCase);
                    bool columnaEsProgramada = nombreColumna.Contains("Programado", StringComparison.OrdinalIgnoreCase);

                    bool esProgramado = tipoEsProgramado || columnaEsProgramada;

                    bool esFecha = valor is DateTime;

                    DateTime fecha = DateTime.MinValue;

                    if (esProgramado && esFecha)
                    {
                        // Extraer el objeto padre que contiene la columna Programado.*
                        var partes = nombreColumna.Split('.');
                        object objetoActual = datos[row];

                        for (int i = 0; i < partes.Length - 1; i++)
                        {
                            var propInfo = objetoActual.GetType().GetProperty(partes[i]);
                            if (propInfo != null)
                                objetoActual = propInfo.GetValue(objetoActual)!;
                            else
                                break;
                        }

                        // Buscar propiedad Frecuencia
                        var frecuenciaProp = objetoActual?.GetType().GetProperty("Frecuencia");
                        var frecuencia = frecuenciaProp?.GetValue(objetoActual)?.ToString();

                        // Buscar propiedad Frecuencia
                        var fechaEjecucionProp = objetoActual?.GetType().GetProperty("FechaEjecucion");
                        var fechaEjecucionObj = fechaEjecucionProp?.GetValue(objetoActual);

                        if (fechaEjecucionObj is DateTime fechaEjecucion)
                        {
                            switch (frecuencia)
                            {
                                case "DIARIA":
                                    cell.Value = fechaEjecucion.ToString("HH:mm");
                                    break;
                                case "SEMANAL":
                                    cell.Value = fechaEjecucion.ToString("dddd", new System.Globalization.CultureInfo("es-ES")).ToUpper(); // Lunes, Martes...
                                    break;
                                case "MENSUAL":
                                    cell.Value = fechaEjecucion.Day.ToString(); // Ej: "27"
                                    break;
                                default:
                                    cell.Value = fechaEjecucion.ToString("dd/MM/yyyy"); // Fallback
                                    break;
                            }
                        }
                        else
                        {
                            cell.Value = ""; // O puedes usar otro valor por defecto
                        }
                    }
                    else if (esFecha)
                    {
                        cell.Value = ((DateTime)valor!);
                        cell.Style.Numberformat.Format = "dd/MM/yyyy";
                    }
                    else if (valor is bool booleano)
                    {
                        cell.Value = booleano ? "Sí" : "No";
                    }
                    else
                    {
                        cell.Value = valor;
                    }

                    cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    cell.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Hair;
                }
            }


            if (worksheet.Dimension != null)
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            return await Task.FromResult(package.GetAsByteArray());
        }


        private int ObtenerIdUsuario()
        {
            var subClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (subClaim == null)
            {
                throw new UnauthorizedAccessException("El token no contiene la claim 'sub'.");
            }



            return int.Parse(subClaim.Value);
        }

        public static object? GetValueByNameExtendido(object? obj, string propertyPath)
        {
            if (obj == null || string.IsNullOrWhiteSpace(propertyPath))
                return null;

            var props = propertyPath.Split('.');
            object? current = obj;

            foreach (var prop in props)
            {
                if (current == null) return null;

                var type = current.GetType();
                var propInfo = type.GetProperty(prop);

                if (propInfo == null)
                    return null;

                current = propInfo.GetValue(current);
            }

            return current;
        }

        public static List<string> ObtenerColumnas(
            Type tipo,
            List<string> columnasUsuario,
            string? prefijo = null,
            HashSet<Type>? visitados = null)
        {
            if (visitados == null)
                visitados = new HashSet<Type>();

            var resultado = new List<string>();
            var excluidas = new HashSet<string> { "Id", "FechaCreacion", "IdUsuario", "HangfireJobId" };

            if (visitados.Contains(tipo))
                return resultado;

            visitados.Add(tipo);

            foreach (var prop in tipo.GetProperties())
            {
                if (prop.GetMethod == null || !prop.GetMethod.IsPublic) continue;
                if (excluidas.Contains(prop.Name)) continue;

                var nombreCompleto = string.IsNullOrEmpty(prefijo) ? prop.Name : $"{prefijo}.{prop.Name}";
                bool esComplejo = prop.PropertyType.IsClass && prop.PropertyType != typeof(string);
                bool esNivelRaiz = string.IsNullOrEmpty(prefijo);

                if (esComplejo)
                {
                    if ((esNivelRaiz && columnasUsuario.Contains(prop.Name)) || !esNivelRaiz)
                    {
                        var subColumnas = ObtenerColumnas(prop.PropertyType, columnasUsuario, nombreCompleto, visitados);
                        resultado.AddRange(subColumnas);
                    }
                }
                else
                {
                    if (!esNivelRaiz || columnasUsuario.Contains(prop.Name))
                    {
                        resultado.Add(nombreCompleto);
                    }
                }
            }

            visitados.Remove(tipo);
            return resultado;
        }


        private string SplitCamelCase(string text)
        {
            return Regex.Replace(text, @"(?<=[a-z0-9])([A-Z])", " $1");
        }
    }
}