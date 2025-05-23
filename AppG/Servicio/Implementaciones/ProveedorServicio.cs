﻿using AppG.Controllers;
using AppG.Entidades.BBDD;
using AppG.Exceptions;
using NHibernate;
using NHibernate.Linq;
using OfficeOpenXml;
using System.Diagnostics;

namespace AppG.Servicio
{
    public class ProveedorServicio : BaseServicio<Proveedor>, IProveedorServicio
    {
        private readonly IGastoServicio _gastoServicio;
        private readonly IGastoProgramadoServicio _gastoProgramadosServicio;
        public ProveedorServicio(ISessionFactory sessionFactory, IGastoServicio gastoServicio, IGastoProgramadoServicio gastoProgramadoServicio) : base(sessionFactory)
        {
            _gastoProgramadosServicio = gastoProgramadoServicio;
            _gastoServicio = gastoServicio;
        }


        public override async Task<Proveedor> CreateAsync(Proveedor entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {

                // Verificar si la categoría existe en la base de datos
                var existingProveedor = await session.Query<Proveedor>()
                    .Where(c => c.Nombre == entity.Nombre && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingProveedor != null && existingProveedor.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"El proveedor '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }


                try
                {
                    // Guardar la entidad y guardar los cambios
                    session.Save(entity);
                    await transaction.CommitAsync();

                    var id = session.GetIdentifier(entity);
                    var createdEntity = await session.GetAsync<Proveedor>(id);

                    return createdEntity;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);

                }

            }
        }

        public override async Task UpdateAsync(int id, Proveedor entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {



                // Verificar si la categoría existe en la base de datos
                var existingCliente = await session.Query<Proveedor>()
                    .Where(c => c.Nombre == entity.Nombre && c.Id != entity.Id && c.IdUsuario == entity.IdUsuario)
                    .SingleOrDefaultAsync();

                if (existingCliente != null && existingCliente.Nombre.ToLower() == entity.Nombre.ToLower())
                {
                    // Asignar el ID de la categoría existente a la entidad
                    errorMessages.Add($"El proveedor '{entity.Nombre}' ya existe en la base de datos.");
                    throw new ValidationException(errorMessages);
                }


                try
                {
                    // Guardar la entidad y guardar los cambios
                    session.Update(entity);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);

                }

            }
        }

        public override async Task DeleteAsync(int id)
        {            
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var gastos = await session.Query<Gasto>()
                            .Where(c => c.Concepto.Categoria.Id == id)
                            .ToListAsync();

                var gastosProgramados = await session.Query<GastoProgramado>()
                            .Where(c => c.Concepto.Categoria.Id == id)
                            .ToListAsync();

                foreach (var gasto in gastos)
                {
                    await _gastoServicio.DeleteAsync(gasto.Id);
                }

                foreach (var gastoProgramado in gastosProgramados)
                {
                    await _gastoProgramadosServicio.DeleteAsync(gastoProgramado.Id);
                }
            }

            await base.DeleteAsync(id);
        }

        public void ExportarDatosExcelAsync(Excel<ProveedorDto> res)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string directorioPath = res.DirPath;

            // Comprobar si la ruta del directorio es válida
            if (!Directory.Exists(directorioPath))
            {
                throw new DirectoryNotFoundException($"El directorio especificado no existe: {directorioPath}");
            }

            // Definir la ruta completa del archivo
            var filePath = Path.Combine(directorioPath, "proveedores.xlsx");

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
                    worksheet = package.Workbook.Worksheets["Proveedor"];

                    if (worksheet == null)
                    {
                        worksheet = package.Workbook.Worksheets.Add("Proveedor");
                    }
                }
                else
                {
                    worksheet = package.Workbook.Worksheets.Add("Proveedor");
                }

                worksheet.Cells.Clear();

                // Establecer las cabeceras de las columnas
                worksheet.Cells["A1"].Value = "Proveedor";

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


        public class ProveedorDto
        {
            public required string Nombre { get; set; }

        }
    }
}

