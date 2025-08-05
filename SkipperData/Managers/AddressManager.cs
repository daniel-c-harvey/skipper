using Data.Shared.Managers;
using SkipperData.Data.Repositories;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class AddressManager : Manager<AddressEntity, AddressModel, AddressRepository, AddressEntityToModelConverter>
{
    public AddressManager(AddressRepository repository) : base(repository)
    {
    }
}