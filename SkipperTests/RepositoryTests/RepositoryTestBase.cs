using Models.Shared.Entities;
using SkipperData.Data;

namespace SkipperTests.RepositoryTests;

[TestFixture]
public abstract class RepositoryTestBase<TEntity> 
where TEntity : class, IEntity
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