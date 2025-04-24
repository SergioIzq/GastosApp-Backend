using AppG.Controllers;
using AppG.Entidades.BBDD;
using AppG.Exceptions;
using NHibernate;
using NHibernate.Linq;
using OfficeOpenXml;
using System.Diagnostics;

namespace AppG.Servicio
{
    public class ConceptoServicio : BaseServicio<Concepto>, IConceptoServicio
    {
        public ConceptoServicio(ISessionFactory sessionFactory) : base(sessionFactory) { }


        public override async Task<Concepto> CreateAsync(Concepto entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {

                // Verificar si la categoría existe en la base de datos
                var existingConcepto = await session.Query<Concepto>()
                    .Where(c => c.Nombre == entity.Nombre && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingConcepto != null && existingConcepto.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"El concepto '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }

                // Guardar la entidad y guardar los cambios
                session.Save(entity);
                await transaction.CommitAsync();

                var id = session.GetIdentifier(entity);
                var createdEntity = await session.GetAsync<Concepto>(id);

                return createdEntity;

            }
        }

        public override async Task UpdateAsync(int id, Concepto entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {



                // Verificar si la categoría existe en la base de datos
                var existingConcepto = await session.Query<Concepto>()
                    .Where(c => c.Nombre == entity.Nombre && c.Id != entity.Id && c.IdUsuario == entity.IdUsuario )
                    .SingleOrDefaultAsync();

                if (existingConcepto != null && existingConcepto.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"El concepto '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }

                session.Update(entity);
                await transaction.CommitAsync();

            }
        }

        public override Task DeleteAsync(int id)
        {
            return base.DeleteAsync(id);
        }

        public void ExportarDatosExcelAsync(Excel<ConceptoDto> res)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string directorioPath = res.DirPath;

            // Comprobar si la ruta del directorio es válida
            if (!Directory.Exists(directorioPath))
            {
                throw new DirectoryNotFoundException($"El directorio especificado no existe: {directorioPath}");
            }

            // Definir la ruta completa del archivo
            var filePath = Path.Combine(directorioPath, "conceptos.xlsx");

            var exportData = new List<dynamic>();

            // Convertir la lista de ingresos a un formato adecuado para Excel
            exportData.AddRange(res.Data.Select(item => new
            {
                Nombre = item?.Nombre ?? string.Empty,
                CategoriaAsociada = item?.CategoriaAsociada ?? string.Empty,
            }));

            exportData.Add(new
            {
                Nombre = "",
                CategoriaAsociada = "",
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
                    catch (FileLoadException ex)
                    {
                        throw new FileLoadException();
                    }
                    worksheet = package.Workbook.Worksheets["Concepto"];

                    if (worksheet == null)
                    {
                        worksheet = package.Workbook.Worksheets.Add("Concepto");
                    }
                }
                else
                {
                    worksheet = package.Workbook.Worksheets.Add("Concepto");
                }

                worksheet.Cells.Clear();

                // Establecer las cabeceras de las columnas
                worksheet.Cells["A1"].Value = "Nombre";
                worksheet.Cells["B1"].Value = "Categoria Asociada";

                // Cargar los datos manualmente a partir de la fila 2
                var row = 2;
                foreach (var item in exportData.Take(exportData.Count - 1))
                {
                    worksheet.Cells[row, 1].Value = item.Nombre;
                    worksheet.Cells[row, 2].Value = item.CategoriaAsociada;
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


        public class ConceptoDto
        {
            public string Nombre { get; set; }

            public string CategoriaAsociada { get; set; }

        }
    }
}

