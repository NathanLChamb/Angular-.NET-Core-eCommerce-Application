namespace eCommerce.Application.Interfaces
{
    public interface ICurrentUser
    {
        string? UserId { get; }
        Guid? AnonymousId { get; }
    }
}
