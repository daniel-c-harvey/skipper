using Data.Shared.Data.Repositories;
using Microsoft.Extensions.Logging;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public class AddressRepository : Repository<SkipperContext, AddressEntity>
{
    public AddressRepository(SkipperContext context, ILogger<AddressRepository> logger) : base(context, logger) { }
}