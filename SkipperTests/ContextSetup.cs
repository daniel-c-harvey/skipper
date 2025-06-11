using Microsoft.EntityFrameworkCore;
using SkipperData.Data;

namespace SkipperTests;

public static class ContextSetup
{
    public static SkipperContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SkipperContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new SkipperContext(options);
    }
}