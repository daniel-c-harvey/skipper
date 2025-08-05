using Data.Shared.Data.Repositories;
using Microsoft.Extensions.Logging;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public class ContactRepository : Repository<SkipperContext, ContactEntity>
{
    public ContactRepository(SkipperContext context, ILogger<Repository<SkipperContext, ContactEntity>> logger, Func<IQueryable<ContactEntity>, IQueryable<ContactEntity>>? queryAdditions = null) 
    : base(context, logger, queryAdditions)
    {
    }
}