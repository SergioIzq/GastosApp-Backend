namespace AhorroLand.Shared.Application.Abstractions.Clock;

public interface IDateTimeProvider
{
    DateTime currentTime { get; }
}
