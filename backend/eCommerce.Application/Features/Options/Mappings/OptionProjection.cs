using eCommerce.Application.Features.Options.DTOs;
using eCommerce.Domain.Catalog.Options;
using eCommerce.Domain.Metadata;

namespace eCommerce.Application.Mapping
{
    public static class OptionProjection
    {
        public static OptionResponse MapToResponse(this Option option)
        {
            return new OptionResponse
            {
                Id = option.Id,
                Name = option.Name,
                OptionValues = option.Values.Select(value => new OptionValueResponse
                {
                    Id = value.Id,
                    Value = value.Value
                }).ToList()
            };
        }
    }
}
