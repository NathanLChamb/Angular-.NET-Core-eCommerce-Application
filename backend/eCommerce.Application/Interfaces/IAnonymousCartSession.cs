namespace eCommerce.Application.Interfaces
{
    public interface IAnonymousCartSession
    {
        Guid? GetAnonymousId();
        void Clear();
    }
}
