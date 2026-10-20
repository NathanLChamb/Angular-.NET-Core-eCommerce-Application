namespace eCommerce.Domain.Catalog.Categories
{
    public class Category
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public Category(string name)
        {
            var validatedName = ValidateName(name);

            Name = validatedName;
        }

        public void Rename(string name)
        {
            var validatedName = ValidateName(name);

            Name = validatedName;
        }

        private static string ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(
                "Category name cannot be empty.", nameof(name));

            return name.Trim();
        }
    }
}
