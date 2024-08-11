namespace ElinaTestProject.Models.Queue
{
    public partial class QueueRepository
    {
        public class QueuePriorityReply
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public int OrderSequence { get; set; }
            public int? PriorityOrderSequence { get; set; }
        }
    }
}
