using AuthBlocksModels.Entities;
using AuthBlocksModels.Models;
using Models.Shared.Converters;

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
            TokenHash = entity.TokenHash,
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
            TokenHash = model.TokenHash,
            IsConsumed = model.IsConsumed,
            ConsumedAt = model.ConsumedAt,
        };
    }
}