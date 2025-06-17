using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SkipperData.Data;

namespace SkipperTests;

public static class TestSetup
{
    public static SkipperContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SkipperContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new SkipperContext(options);
    }

    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(NullLoggerProvider.Instance));
    public static ILogger<T> CreateLogger<T>()
    {
        return _loggerFactory.CreateLogger<T>();
    }
}