using Models.Shared.Models;

namespace SkipperModels.Models;

public class BusinessCustomerModel : CustomerModel
{
    // Business-specific properties
    public string BusinessName { get; set; }
    public string? TaxId { get; set; }
} 