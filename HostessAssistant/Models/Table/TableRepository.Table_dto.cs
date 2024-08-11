using PostGreContext.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ElinaTestProject.Models.Table
{
    public partial class TableRepository
    {
        public class Table_dto
        {
            public int? Id { get; set; }
            [Required]
            public int Number { get; set; }
            [Required]
            public int StatusId { get; set; }
            public string StatusName => Enum.GetName(typeof(TableStatusTypeEnum), StatusId);
            [Required]
            public int MaxCapacity { get; set; }
            public string Comment { get; set; }
        }
    }
}
