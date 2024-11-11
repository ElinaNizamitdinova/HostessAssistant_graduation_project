using PostGreContext.Enums;
using System.Collections.Generic;

namespace ElinaTestProject.Models.History
{
    public partial class HistoryRepository
    {
        public class HistoryInfo
        {
            public int UserId { get; set; }
            public string UserName { get;set; }
            public Dictionary<List<int?>,OrderStatusTypeEnum> Tables { get; set; }


        }
    }
}
