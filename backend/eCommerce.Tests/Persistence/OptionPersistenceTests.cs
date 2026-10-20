using eCommerce.Domain.Catalog.Options;
using eCommerce.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Tests.Persistence
{
    [Collection("Database Collection")]
    public class OptionPersistenceTests(PostgresContainerFixture fixture)
    {
        private readonly PostgresContainerFixture _fixture = fixture;

        [Fact]
        public async Task Option_WithValues_PersistsAndReloadsRelationship()
        {
            await _fixture.ResetDatabase();
            await using var context = _fixture.CreateDbContext();

            var option = new Option("Color");
            var red = new OptionValue("Red");
            var blue = new OptionValue("Blue");

            option.AddValue(red);
            option.AddValue(blue);

            context.Options.Add(option);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var result = await context.Options
                .Include(o => o.Values)
                .SingleAsync(o => o.Id == option.Id);

            Assert.Equal(2, result.Values.Count);
            Assert.Contains(result.Values, v => v.Value == "Red");
            Assert.Contains(result.Values, v => v.Value == "Blue");
        }

        [Fact]
        public async Task DeletingOption_CascadeDeletesValues()
        {
            await _fixture.ResetDatabase();
            await using var context = _fixture.CreateDbContext();

            var option = new Option("Color");

            var red = new OptionValue("Red");
            var blue = new OptionValue("Blue");

            option.AddValue(red);
            option.AddValue(blue);

            context.Options.Add(option);

            await context.SaveChangesAsync();

            var optionId = option.Id;
            var redId = red.Id;
            var blueId = blue.Id;

            context.Options.Remove(option);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var optionExists = await context.Options
                .AnyAsync(o => o.Id == optionId);

            var valuesExist = await context.OptionValues
                .AnyAsync(v => v.Id == redId || v.Id == blueId);

            Assert.False(optionExists);
            Assert.False(valuesExist);
        }
    }
}
