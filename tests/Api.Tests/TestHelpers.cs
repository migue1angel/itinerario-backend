
using Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests;

public class TestHelpers
{
    public static AppDbContext CreateContext() =>
           new(new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
