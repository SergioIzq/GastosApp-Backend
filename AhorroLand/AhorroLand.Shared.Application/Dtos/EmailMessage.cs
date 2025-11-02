namespace AhorroLand.Shared.Application.Dtos
{
    /// <summary>
    /// DTO que encapsula los datos necesarios para enviar un email.
    /// </summary>
    public record EmailMessage(string To, string Subject, string Body);
}
