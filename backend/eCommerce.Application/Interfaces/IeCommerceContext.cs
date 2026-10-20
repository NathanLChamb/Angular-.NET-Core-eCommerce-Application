using eCommerce.Domain.Metadata;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Application.Interfaces
{
    public interface IeCommerceContext
    {
        DbSet<CatalogAttribute> CatalogAttributes { get; }
    }
}
