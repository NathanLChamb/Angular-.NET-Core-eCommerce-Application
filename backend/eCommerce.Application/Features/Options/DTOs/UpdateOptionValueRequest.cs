namespace eCommerce.Application.Features.Options.DTOs
{
    public class UpdateOptionValueRequest
    {
        public int? Id { get; set; }
        public required string Value { get; set; }
    }
}
