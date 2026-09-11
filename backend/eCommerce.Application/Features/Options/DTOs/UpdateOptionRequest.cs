namespace eCommerce.Application.Features.Options.DTOs
{
    public class UpdateOptionRequest
    {
        public required string Name { get; set; }
        public List<UpdateOptionValueRequest> OptionValues { get; set; } = new();
    }
}
