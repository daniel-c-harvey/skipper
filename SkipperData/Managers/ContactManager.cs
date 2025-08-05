using Data.Shared.Managers;
using NetBlocks.Models;
using SkipperData.Data.Repositories;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class ContactManager : Manager<ContactEntity, ContactModel, ContactRepository, ContactEntityToModelConverter>
{
    private readonly AddressManager _addressManager;

    public ContactManager(ContactRepository repository, AddressManager addressManager) : base(repository)
    {
        _addressManager = addressManager;
    }

    public override async Task<ResultContainer<ContactModel>> Add(ContactModel entity)
    {
        if (await _addressManager.Exists(entity.Address) is { Value: false })
        {
            var result = await _addressManager.Add(entity.Address);
            if (result is { Success: true, Value: AddressModel address})
            {
                entity.Address = address;
            }
        }
        return await base.Add(entity);
    }
}