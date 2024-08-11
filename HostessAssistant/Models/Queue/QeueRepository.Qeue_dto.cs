using PostGreContext.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ElinaTestProject.Models.Queue
{
    public partial class QueueRepository
    {
        public class Queue_dto
        {
            public int? Id { get; set; }
            [Required]
            public int WorkShiftId { get; set; }
            [Required]
            public int UserId { get; set; }
            public string? UserName { get; set; }
            [Required]
            public int StatusId { get; set; }
            public string StatusName => Enum.GetName(typeof(QueueStatusTypeEnum), StatusId);
            [Required]
            public int OrderSequence {  get; set; }
            public int? PriorityOrderSequence { get; set; }
            /// <summary>
            /// Время начала смены конкретного работника
            /// </summary>
            public DateTime? StartDateTime { get; set; }
            /// <summary>
            /// Время конца смены работника
            /// </summary>
            public DateTime? EndDateTime { get; set; }
        }
    }
}
