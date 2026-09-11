namespace eCommerce.Application.Features.Options.DTOs
{
    public class OptionResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<OptionValueResponse> OptionValues { get; set; } = new();
    }
}
