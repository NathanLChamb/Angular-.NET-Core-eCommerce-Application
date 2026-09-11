namespace eCommerce.Domain.Catalog.Categories
{
    public class Category
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public Category(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Category name cannot be empty", nameof(name));

            Name = name.Trim();
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Category name cannot be empty", nameof(name));

            Name = name.Trim();
        }
    }
}
