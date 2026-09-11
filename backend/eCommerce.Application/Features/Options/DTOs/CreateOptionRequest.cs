namespace eCommerce.Application.Features.Options.DTOs
{
    public class CreateOptionRequest
    {
        public required string Name { get; set; }
        public List<string> OptionValues { get; set; } = new();
    }
}
