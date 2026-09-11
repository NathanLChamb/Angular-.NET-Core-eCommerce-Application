namespace eCommerce.Tests.Infrastructure
{
    [CollectionDefinition("Database Collection")]
    public class DatabaseCollection : ICollectionFixture<PostgresContainerFixture> { }
}
