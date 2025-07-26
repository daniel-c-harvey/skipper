namespace Models.Shared.Models;

public interface ICompositeModelRoot<TDiscriminator> : IModel
{
    TDiscriminator Discriminator { get; set; }
}