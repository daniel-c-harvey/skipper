namespace Models.Shared.Models;

public interface ICompositeModel<TRoot, TDiscriminator, TInfo> : IKeyed
where TRoot : ICompositeModelRoot<TDiscriminator>
where TInfo : IModel
{
    
    TRoot Root { get; set; }
    TInfo Info { get; set; }
}