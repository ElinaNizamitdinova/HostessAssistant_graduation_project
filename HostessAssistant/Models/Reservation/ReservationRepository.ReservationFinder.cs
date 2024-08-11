using System.Collections.Generic;

namespace ElinaTestProject.Models.Reservation
{
    public partial class ReservationRepository
    {
        public class ReservationFinder
        {
            public int? OriginTypeId { get; set; }
            public List<int> TableIdList { get; set; }
            public int? ReservationStatusId { get; set; }
        }
    }
}
