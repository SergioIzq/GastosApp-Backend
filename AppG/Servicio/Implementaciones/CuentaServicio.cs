using AppG.Entidades.BBDD;
using AppG.Exceptions;
using NHibernate;
using NHibernate.Linq;
using OfficeOpenXml;
using System.Diagnostics;
using System.Linq.Expressions;

namespace AppG.Servicio
{
    public class CuentaServicio : BaseServicio<Cuenta>, ICuentaServicio
    {
        private readonly IGastoProgramadoServicio _gastoProgramadoServicio;
        private readonly IIngresoProgramadoServicio _ingresoProgramadoServicio;

        public CuentaServicio(ISessionFactory sessionFactory, IGastoProgramadoServicio gastoProgramadoServicio, IIngresoProgramadoServicio ingresoProgramadoServicio) : base(sessionFactory) {
            _gastoProgramadoServicio = gastoProgramadoServicio;
            _ingresoProgramadoServicio = ingresoProgramadoServicio;
        }


        public override async Task<Cuenta> CreateAsync(Cuenta entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var existingCuenta = await session.Query<Cuenta>()
                    .Where(c => c.Nombre == entity.Nombre && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingCuenta != null && existingCuenta.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    errorMessages.Add($"La cuenta '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }

                session.Save(entity);
                await transaction.CommitAsync();

                var id = session.GetIdentifier(entity);
                var createdEntity = await session.GetAsync<Cuenta>(id);

                return createdEntity;


            }
        }

        public override async Task UpdateAsync(int id, Cuenta entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {

                var existingCuenta = await session.Query<Cuenta>()
                    .Where(c => c.Nombre == entity.Nombre && c.Id != entity.Id && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingCuenta != null && existingCuenta.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    errorMessages.Add($"La cuenta '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }

                // Guardar la entidad y guardar los cambios
                session.Update(entity);
                await transaction.CommitAsync();
            }
        }

        public override async Task DeleteAsync(int id)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                // Uso dentro de tu método principal
                await EliminarEntidadesRelacionadas<Traspaso>(session, t => t.CuentaOrigen.Id == id);
                await EliminarEntidadesRelacionadas<Traspaso>(session, t => t.CuentaDestino.Id == id);
                await EliminarEntidadesRelacionadas<Ingreso>(session, i => i.Cuenta.Id == id);
                await EliminarEntidadesRelacionadas<Gasto>(session, g => g!.Cuenta!.Id == id);

                // Eliminar los traspasos asociados a la cuenta como origen
                var gastosProgramados = await session.Query<GastoProgramado>()
                    .Where(c => c!.Cuenta!.Id == id)
                    .ToListAsync();

                foreach (var gasto in gastosProgramados)
                {
                    await _gastoProgramadoServicio.DeleteAsync(gasto.Id);
                }

                // Eliminar los traspasos asociados a la cuenta como origen
                var ingresoProgramados = await session.Query<IngresoProgramado>()
                    .Where(c => c!.Cuenta!.Id == id)
                    .ToListAsync();

                foreach (var ingreso in ingresoProgramados)
                {
                    await _ingresoProgramadoServicio.DeleteAsync(ingreso.Id);
                }

                // Eliminar la cuenta
                await base.DeleteAsync(id);

                // Confirmar la transacción
                await transaction.CommitAsync();

            }
        }

        public void ExportarDatosExcelAsync(Excel<CuentaDto> res)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string directorioPath = res.DirPath;

            // Comprobar si la ruta del directorio es válida
            if (!Directory.Exists(directorioPath))
            {
                throw new DirectoryNotFoundException($"El directorio especificado no existe: {directorioPath}");
            }

            // Definir la ruta completa del archivo
            var filePath = Path.Combine(directorioPath, "cuentas.xlsx");

            var exportData = new List<dynamic>();

            // Convertir la lista de ingresos a un formato adecuado para Excel
            exportData.AddRange(res.Data.Select(item => new
            {
                Nombre = item?.Nombre ?? string.Empty,
            }));

            exportData.Add(new
            {
                Nombre = "",
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
                    worksheet = package.Workbook.Worksheets["Cuenta"];

                    if (worksheet == null)
                    {
                        worksheet = package.Workbook.Worksheets.Add("Cuenta");
                    }
                }
                else
                {
                    worksheet = package.Workbook.Worksheets.Add("Cuenta");
                }

                worksheet.Cells.Clear();

                // Establecer las cabeceras de las columnas
                worksheet.Cells["A1"].Value = "Cuenta";

                // Cargar los datos manualmente a partir de la fila 2
                var row = 2;
                foreach (var item in exportData.Take(exportData.Count - 1))
                {
                    worksheet.Cells[row, 1].Value = item.Nombre;
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


        public class CuentaDto
        {
            public required string Nombre { get; set; }

        }

        private async Task EliminarEntidadesRelacionadas<T>(NHibernate.ISession session, Expression<Func<T, bool>> predicate) where T : class
        {
            var entidades = await session.Query<T>()
                .Where(predicate)
                .ToListAsync();

            foreach (var entidad in entidades)
            {
                session.Delete(entidad);
            }
        }
    }
}

