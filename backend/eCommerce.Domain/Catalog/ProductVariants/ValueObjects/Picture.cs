namespace eCommerce.Domain.Catalog.ProductVariants.ValueObjects
{
    public record Picture
    {
        public string Role { get; private set; }
        public Uri Location { get; private set; }

        public Picture(string role, string location)
        {
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentNullException(
                nameof(role), "Picture role cannot be empty");

            if (!Uri.TryCreate(location, UriKind.Absolute, out var uri)) throw new ArgumentException(
                "Picture location must be a valid absolute URI.", nameof(location));

            Role = role;
            Location = uri;
        }
        internal Picture() { }
        internal void ChangeRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentNullException(
                nameof(role), "Picture role cannot be empty");

            Role = role;
        }

        internal void Update(string location)
        {
            if (!Uri.TryCreate(location, UriKind.Absolute, out var uri)) throw new ArgumentException(
                "Picture location must be a valid absolute URI.", nameof(location));

            Location = uri;
        }
    }
}
