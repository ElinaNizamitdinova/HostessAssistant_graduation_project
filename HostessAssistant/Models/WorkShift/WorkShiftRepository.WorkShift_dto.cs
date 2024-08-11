using PostGreContext.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ElinaTestProject.Models.WorkShift
{
    public partial class WorkShiftRepository
    {
        public class WorkShift_dto
        {
            public int? Id { get; set; }
            [Required]
            public int UserTypeId { get; set; }           
            public string UserTypeName => Enum.GetName(typeof(UserTypeEnum), UserTypeId);
            [Required]
            public int StatusId { get; set; }
            public string StatusName => Enum.GetName(typeof(WorkShiftStatusTypeEnum), StatusId);

            public DateTime? StartDateTime { get; set; }
            public DateTime? EndDateTime { get; set; }
        }
    }
}
