using PostGreContext.Enums;
using System;
using System.Collections.Generic;

namespace ElinaTestProject.Models.Reservation
{
    public partial class ReservationRepository
    {
        public class Reservation_dto
        {
            public int? Id { get; set; }
            public int? OriginTypeId { get; set; }
            public string OriginTypeName => Enum.GetName(typeof(OriginTypeEnum), OriginTypeId);
            public TimeSpan Duration { get; set; }
            public DateTime ReservationDt { get; set; }
            public List<int> TableIdList { get; set; }
            public int PersonQuantity { get; set; }
            public int ReservationStatusId { get; set; }
            public string ReservationStatusName => Enum.GetName(typeof(ReservationStatusTypeEnum), ReservationStatusId);
        }
    }
}
