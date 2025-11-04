using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain.Traspasos.Eventos
{
    public sealed record TraspasoRegistradoDomainEvent(Guid TraspasoId, Guid CuentaOrigenId, Guid CuentaDestinoId, Cantidad Importe) : IDomainEvent;
}
