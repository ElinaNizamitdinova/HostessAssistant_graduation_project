namespace ElinaTestProject.Models.Queue
{
    public partial class QueueRepository
    {
        public class QueueFinder
        {
            public int? WorkShiftId { get; set; }
            public int? UserId { get; set; }
            public int? StatusId { get; set; }
        }
    }
}
