namespace ElinaTestProject.Models.Table
{
    public partial class TableRepository
    {
        public class TableFinder
        {
            public int? TableNumber { get; set; }
            public int? TableStatusId { get; set; }
            public int? MaxCapacity { get; set; }
        }
    }
}
