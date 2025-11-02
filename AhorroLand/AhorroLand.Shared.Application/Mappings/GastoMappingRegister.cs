using AhorroLand.Domain.Gastos;
using AhorroLand.Shared.Application.Dtos;
using Mapster;

namespace AhorroLand.Shared.Application.Mappings
{
    public class GastoMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Mapeo de Entidad a DTO (Lectura)
            config.ForType<Gasto, GastoDto>()

                // Mapeo de Relaciones Aplanadas
                .Map(dest => dest.ConceptoId, src => src.Concepto.Id)
                .Map(dest => dest.ConceptoNombre, src => src.Concepto.Nombre.Value)
                .Map(dest => dest.CategoriaId, src => src.Concepto.Categoria.Id)
                .Map(dest => dest.CategoriaNombre, src => src.Concepto.Categoria.Nombre.Value)
                .Map(dest => dest.ProveedorId, src => src.Proveedor.Id)
                .Map(dest => dest.ProveedorNombre, src => src.Proveedor.Nombre.Value)
                .Map(dest => dest.PersonaId, src => src.Persona.Id)
                .Map(dest => dest.PersonaNombre, src => src.Persona.Nombre.Value)
                .Map(dest => dest.CuentaId, src => src.Cuenta.Id)
                .Map(dest => dest.CuentaNombre, src => src.Cuenta.Nombre.Value)
                .Map(dest => dest.FormaPagoId, src => src.FormaPago.Id)
                .Map(dest => dest.FormaPagoNombre, src => src.FormaPago.Nombre.Value)
                ;
        }
    }
}