using Models.Shared.InputModels;
using SkipperModels;

namespace SkipperModels.InputModels;

public class BusinessCustomerInputModel : CustomerInputModel
{
    // Business-specific properties
    public string BusinessName { get; set; }
    public string? TaxId { get; set; }
} 