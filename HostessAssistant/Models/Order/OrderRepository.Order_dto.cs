using PostGreContext.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ElinaTestProject.Models.Order
{
    public partial class OrderRepository
    {
        public class Order_dto
        {
            public int? Id { get; set; }
            [Required]
            public int WorkShiftId { get; set; }
            [Required]
            public int? UserId { get; set; }
            
            public string? UserName { get; set; }
            [Required]
            public int OrderStatusId { get; set; }
            public int? ReservationId {  get; set; }
            public string OrderStatusName => Enum.GetName(typeof(OrderStatusTypeEnum), OrderStatusId);
            public List<int> TableIdList { get; set; }
            public DateTime StartDateTime { get; set; }
            public DateTime? EndDateTime { get; set; }
            public bool? IsPriority { get; set; }
        }
    }
}
