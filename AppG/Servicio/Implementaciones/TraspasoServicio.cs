using AppG.BBDD.Respuestas.Traspasos;
using AppG.Entidades.BBDD;
using AppG.Exceptions;
using NHibernate;
using NHibernate.Linq;
using OfficeOpenXml;
using System.Diagnostics;

namespace AppG.Servicio
{
    public class TraspasoServicio : BaseServicio<Traspaso>, ITraspasoServicio
    {
        public TraspasoServicio(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public async Task<TraspasoByIdRespuesta> GetTraspasoByIdAsync(int id)
        {
            TraspasoByIdRespuesta response = new TraspasoByIdRespuesta();
            var listaCuentas = new List<Cuenta>();

            response.TraspasoById = await base.GetByIdAsync(id);

            if (response.TraspasoById?.IdUsuario != null)
            {
                var idUsuario = response.TraspasoById.IdUsuario;
                using (var session = _sessionFactory.OpenSession())
                {
                    listaCuentas = await session.Query<Cuenta>()
                                        .Where(c => c.IdUsuario == idUsuario)
                                        .OrderBy(c => c.Nombre)
                                        .ToListAsync();
                }
                response.ListaCuentas = listaCuentas;
            }

            return response;
        }

        public async Task<List<Cuenta>> GetNewTraspasoAsync(int id)
        {
            var listaCuentas = new List<Cuenta>();

            if (id > 0)
            {
                using (var session = _sessionFactory.OpenSession())
                {
                    listaCuentas = await session.Query<Cuenta>()
                                        .Where(c => c.IdUsuario == id)
                                        .OrderBy(c => c.Nombre)
                                        .ToListAsync();
                }
            }

            return listaCuentas;
        }

        public async Task<Traspaso> RealizarTraspaso(Traspaso entity)
        {
            IList<string> errorMessages = new List<string>();
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Buscar la cuenta de origen por nombre
                    var cuentaOrigen = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == entity.CuentaOrigen.Nombre && c.IdUsuario == entity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuentaOrigen == null)
                    {
                        errorMessages.Add($"La cuenta de origen '{entity.CuentaOrigen.Nombre}' no existe.");
                    }

                    // Buscar la cuenta de destino por nombre
                    var cuentaDestino = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == entity.CuentaDestino.Nombre && c.IdUsuario == entity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuentaDestino == null)
                    {
                        errorMessages.Add($"La cuenta de destino '{entity.CuentaDestino.Nombre}' no existe.");
                    }

                    // Realizar el traspaso
                    cuentaOrigen!.Saldo -= entity.Importe;

                    cuentaDestino!.Saldo += entity.Importe;


                    Traspaso traspaso = new Traspaso
                    {
                        CuentaOrigen = cuentaOrigen,
                        SaldoCuentaOrigen = cuentaOrigen.Saldo,
                        CuentaDestino = cuentaDestino,
                        SaldoCuentaDestino = cuentaDestino.Saldo,
                        Fecha = entity.Fecha,
                        Descripcion = entity.Descripcion,
                        Importe = entity.Importe,
                        IdUsuario = entity.CuentaOrigen.IdUsuario
                    };

                    // Guardar los cambios en las cuentas
                    await session.SaveOrUpdateAsync(cuentaOrigen);
                    await session.SaveOrUpdateAsync(cuentaDestino);

                    await session.SaveAsync(traspaso);

                    await transaction.CommitAsync();

                    var id = session.GetIdentifier(traspaso);
                    var createdEntity = await session.GetAsync<Traspaso>(id);

                    return createdEntity;
                }
                catch (Exception)
                {
                    // Rollback de la transacción en caso de error
                    await transaction.RollbackAsync();
                    throw new ValidationException(errorMessages);
                }
            }
        }

        public override async Task DeleteAsync(int id)
        {

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var entity = await session.GetAsync<Traspaso>(id);
                if (entity == null)
                {
                    throw new KeyNotFoundException($"Entidad con ID {id} no encontrada");
                }

                entity.CuentaOrigen.Saldo += entity.Importe;
                entity.CuentaDestino.Saldo -= entity.Importe;

                session.Delete(entity);
                await transaction.CommitAsync();
                session.Clear();
            }
        }

        public override async Task UpdateAsync(int id, Traspaso entity)
        {
            IList<string> errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Obtener el traspaso existente por ID
                    var existingTraspaso = await session.Query<Traspaso>()
                        .Where(t => t.Id == id)
                        .SingleOrDefaultAsync();

                    if (existingTraspaso == null)
                    {
                        throw new Exception($"El traspaso con ID '{id}' no existe.");
                    }

                    // Obtener las cuentas de origen y destino por nombre
                    var cuentaOrigen = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == entity.CuentaOrigen.Nombre && c.IdUsuario == entity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuentaOrigen == null)
                    {
                        errorMessages.Add($"La cuenta de origen '{entity.CuentaOrigen.Nombre}' no existe.");
                    }

                    var cuentaDestino = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == entity.CuentaDestino.Nombre && c.IdUsuario == entity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuentaDestino == null)
                    {
                        errorMessages.Add($"La cuenta de destino '{entity.CuentaDestino.Nombre}' no existe.");
                    }

                    if (errorMessages.Any())
                    {
                        throw new ValidationException(errorMessages);
                    }

                    // Actualizar los saldos de las cuentas
                    // Revertir el saldo anterior de las cuentas involucradas
                    cuentaOrigen!.Saldo += existingTraspaso.Importe;
                    cuentaDestino!.Saldo -= existingTraspaso.Importe;

                    // Aplicar el saldo actual del nuevo traspaso
                    cuentaOrigen.Saldo -= entity.Importe;
                    cuentaDestino.Saldo += entity.Importe;

                    // Actualizar los saldos en la entidad de traspaso
                    entity.SaldoCuentaOrigen = cuentaOrigen.Saldo;
                    entity.SaldoCuentaDestino = cuentaDestino.Saldo;

                    // Actualizar la entidad traspaso existente con la nueva información
                    existingTraspaso.Fecha = entity.Fecha;
                    existingTraspaso.Importe = entity.Importe;
                    existingTraspaso.Descripcion = entity.Descripcion;
                    existingTraspaso.CuentaOrigen = entity.CuentaOrigen;
                    existingTraspaso.CuentaDestino = entity.CuentaDestino;
                    existingTraspaso.SaldoCuentaOrigen = entity.SaldoCuentaOrigen;
                    existingTraspaso.SaldoCuentaDestino = entity.SaldoCuentaDestino;

                    // Guardar las actualizaciones en las cuentas y en el traspaso
                    await session.SaveOrUpdateAsync(cuentaOrigen);
                    await session.SaveOrUpdateAsync(cuentaDestino);
                    await session.SaveOrUpdateAsync(existingTraspaso);

                    // Commit de la transacción
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    // Rollback en caso de error
                    await transaction.RollbackAsync();
                    throw new ValidationException(errorMessages);
                }
            }
        }

        public void ExportarDatosExcelAsync(Excel<TraspasoDto> res)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string directorioPath = res.DirPath;

            // Comprobar si la ruta del directorio es válida
            if (!Directory.Exists(directorioPath))
            {
                throw new DirectoryNotFoundException($"El directorio especificado no existe: {directorioPath}");
            }

            // Definir la ruta completa del archivo
            var filePath = Path.Combine(directorioPath, "traspasos.xlsx");

            var exportData = new List<dynamic>();

            // Convertir la lista de ingresos a un formato adecuado para Excel
            exportData.AddRange(res.Data.Select(item => new
            {
                Fecha = item.Fecha.ToString("dd/MM/yyyy"),
                CuentaOrigen = item.CuentaOrigen ?? string.Empty,
                SaldoCuentaOrigen = item.SaldoCuentaOrigen,
                CuentaDestino = item.CuentaDestino ?? string.Empty,
                SaldoCuentaDestino = item.SaldoCuentaDestino,
                Importe = $"+{item.Importe}",
                Descripcion = item?.Descripcion ?? string.Empty,
            }));

            exportData.Add(new
            {
                Fecha = "",
                CuentaOrigen = "",
                SaldoCuentaOrigen = "",
                CuentaDestino = "",
                SaldoCuentaDestino = "",
                Importe = "",
                Descripcion = "",
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
                        throw new FileLoadException();
                    }
                    worksheet = package.Workbook.Worksheets["Traspaso"];

                    if (worksheet == null)
                    {
                        worksheet = package.Workbook.Worksheets.Add("Traspaso");
                    }
                }
                else
                {
                    worksheet = package.Workbook.Worksheets.Add("Traspaso");
                }

                worksheet.Cells.Clear();

                // Establecer las cabeceras de las columnas
                worksheet.Cells["A1"].Value = "Fecha";
                worksheet.Cells["B1"].Value = "Cuenta Origen";
                worksheet.Cells["C1"].Value = "Saldo Cuenta Origen";
                worksheet.Cells["D1"].Value = "Cuenta Destino";
                worksheet.Cells["E1"].Value = "SaldoCuentaDestino";
                worksheet.Cells["F1"].Value = "Importe";
                worksheet.Cells["G1"].Value = "Descripcion";


                var row = 2;
                foreach (var item in exportData.Take(exportData.Count - 1))
                {
                    worksheet.Cells[row, 1].Value = item.Fecha;
                    worksheet.Cells[row, 2].Value = item.CuentaOrigen;
                    worksheet.Cells[row, 3].Value = item.SaldoCuentaOrigen;
                    worksheet.Cells[row, 4].Value = item.CuentaDestino;
                    worksheet.Cells[row, 5].Value = item.SaldoCuentaDestino;
                    worksheet.Cells[row, 6].Value = item.Importe;
                    worksheet.Cells[row, 7].Value = item.Descripcion;
                    row++;
                }

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


        public class TraspasoDto
        {
            public DateTime Fecha { get; set; }
            public required string CuentaOrigen { get; set; }
            public decimal SaldoCuentaOrigen { get; set; }
            public required string CuentaDestino { get; set; }
            public decimal SaldoCuentaDestino { get; set; }
            public decimal Importe { get; set; }

            public string? Descripcion { get; set; }
        }

    }
}
