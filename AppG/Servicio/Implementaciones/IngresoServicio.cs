﻿using AppG.BBDD.Respuestas.Ingresos;
using AppG.Controllers;
using AppG.Entidades.BBDD;
using AppG.Exceptions;
using NHibernate;
using NHibernate.Linq;
using OfficeOpenXml;
using System.Diagnostics;

namespace AppG.Servicio
{
    public class IngresoServicio : BaseServicio<Ingreso>, IIngresoServicio
    {
        public IngresoServicio(ISessionFactory sessionFactory) : base(sessionFactory)
        {

        }

        public override async Task<ResponseList<Ingreso>> GetCantidadAsync(int page, int size, int idUsuario)
        {
            try
            {
                using (var session = _sessionFactory.OpenSession())
                {
                    // Contar el número total de registros usando LINQ
                    int totalCount = await session.Query<Ingreso>()
                        .Where(x => x.IdUsuario == idUsuario)
                        .CountAsync();

                    // Calcular el offset para la paginación
                    var offset = (page - 1) * size;

                    // Obtener los resultados paginados usando LINQ
                    var results = await session.Query<Ingreso>()
                        .Where(x => x.IdUsuario == idUsuario)
                        .OrderByDescending(x => x.Fecha)
                        .Skip(offset)
                        .Take(size)
                        .ToListAsync();

                    // Crear la respuesta con el total de registros y los elementos obtenidos
                    return new ResponseList<Ingreso>(results, totalCount);
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                throw new Exception($"Error al obtener cantidad: {ex.Message}", ex);
            }
        }

        public async Task<Ingreso> CreateAsync(Ingreso entity, bool esGastoProgramado = false)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Buscar la cuenta correspondiente por nombre
                    var cuenta = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == entity.Cuenta.Nombre && c.IdUsuario == entity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuenta == null)
                    {
                        errorMessages.Add($"La cuenta '{entity.Cuenta.Nombre}' no existe.");                        
                    }

                    if (errorMessages.Count > 0)
                    {
                        throw new ValidationException(errorMessages);
                    }

                    if (!esGastoProgramado)
                    {
                        cuenta!.Saldo += entity!.Monto;
                    }
                    // Guardar la cuenta actualizada
                    session.Update(cuenta);

                    // Guardar el ingreso
                    session.Save(entity);
                    await transaction.CommitAsync();

                    var id = session.GetIdentifier(entity);
                    var createdEntity = await session.GetAsync<Ingreso>(id);

                    return createdEntity;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();                    
                    throw new Exception(ex.Message);
                }
            }
        }


        public override async Task UpdateAsync(int id, Ingreso entity)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Cargar la entidad existente
                    var existingEntity = await session.GetAsync<Ingreso>(id);
                    if (existingEntity == null)
                    {
                        throw new KeyNotFoundException($"Entidad con ID {id} no encontrada");
                    }

                    // Obtener la categoría del entity
                    var entityCategoria = entity?.Concepto.Categoria;
                    if (entityCategoria != null)
                    {
                        // Verificar si la categoría existe en la base de datos
                        var existingCategoria = await session.Query<Categoria>()
                            .Where(c => c.Nombre == entityCategoria.Nombre && c.IdUsuario == entity!.IdUsuario)
                            .SingleOrDefaultAsync();

                        if (existingCategoria != null)
                        {
                            // Asignar el ID de la categoría existente a la entidad
                            entity!.Concepto.Categoria = existingCategoria;
                        }
                        else
                        {
                            errorMessages.Add($"La categoría '{entityCategoria.Nombre}' no existe.");
                        }
                    }

                    // Buscar la cuenta original del ingreso (cuenta del ingreso existente)
                    var originalCuenta = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == existingEntity.Cuenta.Nombre && c.IdUsuario == existingEntity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (originalCuenta == null)
                    {
                        errorMessages.Add($"La cuenta original '{existingEntity.Cuenta.Nombre}' no existe.");
                    }

                    // Buscar la nueva cuenta (cuenta del nuevo entity)
                    var nuevaCuenta = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == entity!.Cuenta.Nombre && c.IdUsuario == entity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (nuevaCuenta == null)
                    {
                        errorMessages.Add($"La cuenta '{entity!.Cuenta.Nombre}' no existe.");
                    }

                    if (errorMessages.Count > 0)
                    {
                        throw new ValidationException(errorMessages);
                    }

                    // Revertir el saldo de la cuenta original basado en el ingreso anterior
                    if (originalCuenta != null)
                    {
                        originalCuenta.Saldo -= existingEntity.Monto;
                        session.Update(originalCuenta);
                    }

                    // Actualizar el saldo de la nueva cuenta basado en el nuevo monto
                    if (nuevaCuenta != null)
                    {
                        nuevaCuenta.Saldo += entity!.Monto;
                        session.Update(nuevaCuenta);
                    }

                    // Fusionar y guardar la entidad actualizada
                    session.Merge(entity);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public override async Task DeleteAsync(int id)
        {
            var errorMessages = new List<string>();

            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // Cargar la entidad existente
                    var existingEntity = await session.GetAsync<Ingreso>(id);
                    if (existingEntity == null)
                    {
                        throw new KeyNotFoundException($"Entidad con ID {id} no encontrada");
                    }

                    // Buscar la cuenta correspondiente por nombre
                    var cuenta = await session.Query<Cuenta>()
                        .Where(c => c.Nombre == existingEntity.Cuenta.Nombre && c.IdUsuario == existingEntity.IdUsuario)
                        .SingleOrDefaultAsync();

                    if (cuenta == null)
                    {
                        errorMessages.Add($"La cuenta '{existingEntity.Cuenta.Nombre}' no existe.");
                    }

                    if (errorMessages.Count > 0)
                    {
                        throw new ValidationException(errorMessages);
                    }                    

                    cuenta!.Saldo -= existingEntity.Monto;

                    // Guardar la cuenta actualizada
                    session.Update(cuenta);

                    // Eliminar el ingreso
                    await session.DeleteAsync(existingEntity);

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public void ExportarDatosExcelAsync(Excel<IngresoDto> res)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string directorioPath = res.DirPath;

            // Comprobar si la ruta del directorio es válida
            if (!Directory.Exists(directorioPath))
            {
                throw new DirectoryNotFoundException($"El directorio especificado no existe: {directorioPath}");
            }

            // Definir la ruta completa del archivo
            var filePath = Path.Combine(directorioPath, "ingresos.xlsx");

            var exportData = new List<dynamic>();

            // Convertir la lista de ingresos a un formato adecuado para Excel
            exportData.AddRange(res.Data.Select(item => new
            {
                TipoOperacion = "Ingreso",
                Fecha = item.Fecha.ToString("dd/MM/yyyy"),
                Persona = item.Persona?.Nombre ?? string.Empty,
                FormaPago = item.FormaPago?.Nombre ?? string.Empty,
                Cliente = item.Cliente?.Nombre ?? string.Empty,
                Categoria = item.Categoria?.Nombre ?? string.Empty,
                Concepto = item.Concepto?.Nombre ?? string.Empty,
                Cuenta = item.Cuenta?.Nombre ?? string.Empty,
                Descripcion = item?.Descripcion ?? string.Empty,
                Importe = $"+{item!.Importe}",
            }));           

            exportData.Add(new
            {
                TipoOperacion = "Ingreso",
                Fecha = "",
                Persona = "",
                FormaPago = "",
                Cliente = "",
                Categoria = "",
                Concepto = "",
                Cuenta = "",
                Descripcion = "",
                Importe = ""
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
                    worksheet = package.Workbook.Worksheets["Ingresos"];

                    if (worksheet == null)
                    {
                        worksheet = package.Workbook.Worksheets.Add("Ingresos");
                    }
                }
                else
                {
                    worksheet = package.Workbook.Worksheets.Add("Ingresos");
                }

                worksheet.Cells.Clear();

                // Establecer las cabeceras de las columnas
                worksheet.Cells["A1"].Value = "Tipo Operacion";
                worksheet.Cells["B1"].Value = "Fecha";
                worksheet.Cells["C1"].Value = "Persona";
                worksheet.Cells["D1"].Value = "Forma de Pago";
                worksheet.Cells["E1"].Value = "Cliente";
                worksheet.Cells["F1"].Value = "Categoria";
                worksheet.Cells["G1"].Value = "Concepto";
                worksheet.Cells["H1"].Value = "Cuenta";
                worksheet.Cells["I1"].Value = "Descripcion";
                worksheet.Cells["J1"].Value = "Importe";

                // Cargar los datos manualmente a partir de la fila 2
                var row = 2;
                foreach (var item in exportData.Take(exportData.Count - 1))
                {
                    worksheet.Cells[row, 1].Value = item.TipoOperacion;
                    worksheet.Cells[row, 2].Value = item.Fecha;
                    worksheet.Cells[row, 3].Value = item.Persona;
                    worksheet.Cells[row, 4].Value = item.FormaPago;
                    worksheet.Cells[row, 5].Value = item.Cliente;
                    worksheet.Cells[row, 6].Value = item.Categoria;
                    worksheet.Cells[row, 7].Value = item.Concepto;
                    worksheet.Cells[row, 8].Value = item.Cuenta;
                    worksheet.Cells[row, 9].Value = item.Descripcion;
                    worksheet.Cells[row, 10].Value = item.Importe;
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

        public async Task<IngresoRespuesta> GetNewIngresoAsync(int idUsuario)
        {
            IngresoRespuesta newIngreso = new IngresoRespuesta();

            using (var session = _sessionFactory.OpenSession())
            {
                try
                {
                    var listaCategorias = await session.Query<Categoria>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    var listaConceptos = await session.Query<Concepto>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    var listaCuentas = await session.Query<Cuenta>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    var listaClientes = await session.Query<Cliente>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    var listaPersonas = await session.Query<Persona>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    var listaFormasPago = await session.Query<FormaPago>()
                            .Where(c => c.IdUsuario == idUsuario)
                            .OrderBy(c => c.Nombre)
                            .ToListAsync();

                    // Crear objeto respuesta al frontend para nuevos gastos
                    newIngreso.ListaClientes = listaClientes;
                    newIngreso.ListaCuentas = listaCuentas;
                    newIngreso.ListaConceptos = listaConceptos;
                    newIngreso.ListaCategorias = listaCategorias;
                    newIngreso.ListaPersonas = listaPersonas;
                    newIngreso.ListaFormasPago = listaFormasPago;

                    return newIngreso;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<IngresoByIdRespuesta> GetIngresoByIdAsync(int id)
        {
            IngresoByIdRespuesta response = new IngresoByIdRespuesta();

            response.IngresoById = await base.GetByIdAsync(id);

            if (response.IngresoById?.Cuenta?.IdUsuario != null)
            {
                response.IngresoRespuesta = await GetNewIngresoAsync(response.IngresoById.Cuenta.IdUsuario);
            }

            return response;
        }

        public class IngresoDto
        {
            public required DateTime Fecha { get; set; }
            public required Persona Persona { get; set; }
            public required FormaPago FormaPago { get; set; }
            public required Cliente Cliente { get; set; }
            public required Categoria Categoria { get; set; }
            public required Concepto Concepto { get; set; }
            public required Cuenta Cuenta { get; set; }
            public required decimal Importe { get; set; }

            public string? Descripcion { get; set; }
        }

    }
}
