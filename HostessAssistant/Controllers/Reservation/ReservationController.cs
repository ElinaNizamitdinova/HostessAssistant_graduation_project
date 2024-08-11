
using ElinaTestProject.Interfaces.Reservation;
using ElinaTestProject.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ElinaTestProject.Models.Reservation.ReservationRepository;

namespace ElinaTestProject.Controllers.Reservation
{
    [Route("api/[controller]")]
    public class ReservationController : Controller
    {
        private IReservationRepository _reservationRepository;

        public ReservationController(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        /// <summary>
        /// Get reservation by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/{id:int}")]
        [ProducesResponseType(typeof(Reservation_dto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetReservation(int id)
        {
            return await _reservationRepository.GetReservationAsync(id);
        }

        /// <summary>
        /// Add or update reservation
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(Reservation_dto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Apply([FromBody] Reservation_dto reservation)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ControllerUtils.GetModelErrors(ModelState));

            if (reservation.Id.HasValue)
                return await _reservationRepository.UpdateReservationAsync(reservation).ConfigureAwait(false);
            else
                return await _reservationRepository.CreateReservationAsync(reservation).ConfigureAwait(false);
        }

        /// <summary>
        /// Find reservation by params
        /// </summary>
        /// <param name="finder"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<Reservation_dto>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Find([FromBody] ReservationFinder finder)
        {
            return await _reservationRepository.FindReservationAsync(finder).ConfigureAwait(false);
        }
        /// <summary>
        /// Close reservation by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]/{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Delete(int id)
        {
            return await _reservationRepository.CloseReservation(id).ConfigureAwait(false);
        }
    }
}

