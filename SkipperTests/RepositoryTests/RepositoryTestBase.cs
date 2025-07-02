using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkipperData.Data;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperTests.RepositoryTests;

[TestFixture]
public abstract class RepositoryTestBase<TEntity, TDto> 
where TEntity : class, IEntity<TEntity, TDto>
where TDto : class, IModel<TDto, TEntity>
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