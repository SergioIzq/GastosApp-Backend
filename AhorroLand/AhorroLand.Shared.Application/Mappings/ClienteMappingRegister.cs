using AhorroLand.Domain;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.ValueObjects;
using Mapster;

namespace AhorroLand.Shared.Application.Mappings
{
    public class ClienteMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Mapeo de Entidad a DTO (Lectura)
            config.ForType<Cliente, ClienteDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Nombre, src => src.Nombre.Value)
                .Map(dest => dest.UsuarioId, src => src.UsuarioId.Value);

            // Mapeo de DTO a Entidad (para reconstituir desde DB)
            config.ForType<ClienteDto, Cliente>()
                .ConstructUsing(src => CreateClienteFromDto(src));
        }

        /// <summary>
        /// Factory method para crear Cliente desde DTO usando reflexión
        /// (necesario porque el constructor es privado).
        /// </summary>
        private static Cliente CreateClienteFromDto(ClienteDto dto)
        {
            // Crear instancia vacía usando constructor sin parámetros
            var cliente = new Cliente();

            // Usar reflexión para setear propiedades privadas
            var idProperty = typeof(Cliente).GetProperty(nameof(Cliente.Id));
            var nombreProperty = typeof(Cliente).GetProperty(nameof(Cliente.Nombre));
            var usuarioIdProperty = typeof(Cliente).GetProperty(nameof(Cliente.UsuarioId));

            idProperty?.SetValue(cliente, dto.Id);
            nombreProperty?.SetValue(cliente, new Nombre(dto.Nombre ?? string.Empty));
            usuarioIdProperty?.SetValue(cliente, new UsuarioId(dto.UsuarioId));

            return cliente;
        }
    }
}