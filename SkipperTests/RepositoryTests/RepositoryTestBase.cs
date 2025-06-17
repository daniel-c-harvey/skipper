using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkipperData.Data;
using SkipperModels.Entities;

namespace SkipperTests.RepositoryTests;

[TestFixture]
public abstract class RepositoryTestBase<T> where T : BaseEntity
{
    protected SkipperContext Context;
    
    [SetUp]
    public virtual void SetUp()
    {
        Context = TestSetup.CreateContext();
    }

    [TearDown]
    public void TearDown()
    {
        Context?.Dispose();
    }
} 