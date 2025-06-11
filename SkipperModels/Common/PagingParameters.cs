using System.Linq.Expressions;

namespace SkipperModels.Common;

public class PagingParameters<T>
{
    private const int MaxPageSize = 100;
    private int _pageSize = 20;

    public int Page { get; set; } = 1;
    
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public Expression<Func<T, object>>? OrderBy { get; set; }
    public bool IsDescending { get; set; } = false;

    public int Skip => (Page - 1) * PageSize;
} 