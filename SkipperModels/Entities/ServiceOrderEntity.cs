// using Models.Shared.Entities;
//
// namespace SkipperModels.Entities;
//
// public class ServiceOrderEntity : OrderEntity<CustomerEntity>
// {
//     // Service-specific properties
//     public long ServiceTypeId { get; set; }
//     public virtual ServiceTypeEntity ServiceType { get; set; }
//     public DateTime ScheduledDate { get; set; }
//     public string ServiceDescription { get; set; }
//     public int LaborHours { get; set; }
//     public int PartsCost { get; set; } // cents
//     public ServiceStatus ServiceStatus { get; set; }
//
//     public ServiceOrderEntity()
//     {
//         OrderType = OrderType.ServiceOrder;
//     }
// }
//
// // Placeholder entities for the example
// public class ServiceTypeEntity : BaseEntity, IEntity
// {
//     public string Name { get; set; }
//     public string Description { get; set; }
//     public int BasePrice { get; set; } // cents
// }
//
// public enum ServiceStatus
// {
//     Scheduled,
//     InProgress,
//     Completed,
//     Cancelled
// } 