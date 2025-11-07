using AhorroLand.Infrastructure.TypesHandlers;
using AhorroLand.Shared.Domain.ValueObjects;
using Dapper;

namespace AhorroLand.Infrastructure.TypesHandlers
{
    public static class DapperTypeHandlerRegistration
    {
        public static void RegisterGuidValueObjectHandlers()
        {
            // ? Registrar handler para Guid que maneja tanto BINARY(16) como strings UUID
            SqlMapper.AddTypeHandler(new GuidBinaryTypeHandler());

            // ? Registrar handlers para Value Objects específicos
            SqlMapper.AddTypeHandler(new GuidValueObjectTypeHandler<UsuarioId>(g => new UsuarioId(g)));
            SqlMapper.AddTypeHandler(new GuidValueObjectTypeHandler<ClienteId>(g => new ClienteId(g)));
            SqlMapper.AddTypeHandler(new GuidValueObjectTypeHandler<CategoriaId>(g => new CategoriaId(g)));
            SqlMapper.AddTypeHandler(new GuidValueObjectTypeHandler<ConceptoId>(g => new ConceptoId(g)));
            SqlMapper.AddTypeHandler(new GuidValueObjectTypeHandler<CuentaId>(g => new CuentaId(g)));
            SqlMapper.AddTypeHandler(new GuidValueObjectTypeHandler<FormaPagoId>(g => new FormaPagoId(g)));
            SqlMapper.AddTypeHandler(new GuidValueObjectTypeHandler<PersonaId>(g => new PersonaId(g)));
            SqlMapper.AddTypeHandler(new GuidValueObjectTypeHandler<ProveedorId>(g => new ProveedorId(g)));
        }
    }
}
