using Models.Shared.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;
using SkipperModels.InputModels;

namespace SkipperModels.Converters;

public class BusinessCustomerConverter : IEntityToModelConverter<BusinessCustomerEntity, BusinessCustomerModel>
{
    public static BusinessCustomerModel Convert(BusinessCustomerEntity entity)
    {
        // Use the generic base converter to set base properties, then add specific properties
        var model = CustomerConverter<BusinessCustomerEntity, BusinessCustomerModel>.Convert(entity);
        
        // Set Business-specific properties
        model.BusinessName = entity.BusinessName;
        model.TaxId = entity.TaxId;
        
        return model;
    }

    public static BusinessCustomerEntity Convert(BusinessCustomerModel model)
    {
        // Use the generic base converter to set base properties, then add specific properties
        var entity = CustomerConverter<BusinessCustomerEntity, BusinessCustomerModel>.Convert(model);
        
        // Set Business-specific properties
        entity.BusinessName = model.BusinessName;
        entity.TaxId = model.TaxId;
        
        return entity;
    }
}

public class BusinessCustomerModelToInputConverter
{
    public static BusinessCustomerInputModel Convert(BusinessCustomerModel model)
    {
        return new BusinessCustomerInputModel
        {
            Id = model.Id,
            AccountNumber = model.AccountNumber,
            Name = model.Name,
            CustomerProfileType = model.CustomerProfileType,
            BusinessName = model.BusinessName,
            TaxId = model.TaxId,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
        };
    }

    public static BusinessCustomerModel Convert(BusinessCustomerInputModel input)
    {
        return new BusinessCustomerModel
        {
            Id = input.Id,
            AccountNumber = input.AccountNumber,
            Name = input.Name,
            CustomerProfileType = input.CustomerProfileType,
            BusinessName = input.BusinessName,
            TaxId = input.TaxId,
            CreatedAt = input.CreatedAt,
            UpdatedAt = input.UpdatedAt,
        };
    }
} 