using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static ElinaTestProject.Models.Reservation.ReservationRepository;

namespace ElinaTestProject.Interfaces.Reservation
{
    public interface IReservationRepository
    {
        Task<IActionResult> CreateReservationAsync(Reservation_dto reservation);
        Task<IActionResult> FindReservationAsync(ReservationFinder finder);
        Task<IActionResult> GetReservationAsync(int id);
        Task<IActionResult> UpdateReservationAsync(Reservation_dto reservation);
        Task<IActionResult> CloseReservation(int id);
    }
}
