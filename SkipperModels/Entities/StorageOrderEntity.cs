// Future implementation for Storage Orders
// namespace SkipperModels.Entities;
// 
// public class StorageOrderEntity : OrderEntity
// {
//     // Storage-specific properties
//     public long StorageLocationId { get; set; }
//     public virtual StorageLocationEntity StorageLocation { get; set; }
//     public DateTime StorageStartDate { get; set; }
//     public DateTime? StorageEndDate { get; set; }
//     public string? ItemDescription { get; set; }
//     public decimal? ItemWeight { get; set; }
//     public StorageStatus StorageStatus { get; set; }
// 
//     public StorageOrderEntity()
//     {
//         OrderType = OrderType.StorageOrder;
//     }
// } 