using AuthBlocksModels.Entities;
using AuthBlocksModels.InputModels;
using AuthBlocksModels.Models;
using Models.Shared.Converters;
using Models.Shared.Models;

namespace AuthBlocksModels.Converters;

public class PendingRegistrationEntityToModelConverter : IEntityToModelConverter<PendingRegistration, PendingRegistrationModel>
{
    public static PendingRegistrationModel Convert(PendingRegistration entity)
    {
        return new PendingRegistrationModel()
        {
            Id = entity.Id,
            PendingUserEmail = entity.PendingUserEmail,
            ExpiresAt = entity.ExpiresAt,
            IsConsumed = entity.IsConsumed,
            ConsumedAt = entity.ConsumedAt,
        };
    }

    public static PendingRegistration Convert(PendingRegistrationModel model)
    {
        return new PendingRegistration()
        {
            Id = model.Id,
            PendingUserEmail = model.PendingUserEmail,
            ExpiresAt = model.ExpiresAt,
            IsConsumed = model.IsConsumed,
            ConsumedAt = model.ConsumedAt,
        };
    }
}

public class PendingRegistrationModelToInputConverter : IModelToInputConverter<PendingRegistrationModel, PendingRegistrationInputModel>
{
    public static PendingRegistrationInputModel Convert(PendingRegistrationModel model)
    {
        return new PendingRegistrationInputModel()
        {
            Id = model.Id,
            Email = model.PendingUserEmail,
            ExpiresAt = model.ExpiresAt,
            IsConsumed = model.IsConsumed,
            ConsumedAt = model.ConsumedAt,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
        };
    }

    public static PendingRegistrationModel Convert(PendingRegistrationInputModel input)
    {
        return new PendingRegistrationModel()
        {
            Id = input.Id,
            PendingUserEmail = input.Email,
            ExpiresAt = input.ExpiresAt,
            IsConsumed = input.IsConsumed,
            ConsumedAt = input.ConsumedAt,
            CreatedAt = input.CreatedAt,
            UpdatedAt = input.UpdatedAt,
        };
    }
}