using ElinaTestProject.Interfaces.WorkShift;
using ElinaTestProject.Utils;
using Microsoft.AspNetCore.Mvc;
using PostGreContext.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ElinaTestProject.Models.Table.TableRepository;
using static ElinaTestProject.Models.WorkShift.WorkShiftRepository;

namespace ElinaTestProject.Controllers.WorkShift
{
    [Route("api/[controller]")]
    public class WorkShiftController : Controller
    {
        private readonly IWorkShiftRepository _workShiftRepository;

        public WorkShiftController(IWorkShiftRepository workShiftRepository)
        {
            _workShiftRepository = workShiftRepository;
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(WorkShift_dto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Create([FromBody] WorkShift_dto ws)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ControllerUtils.GetModelErrors(ModelState));

            return await _workShiftRepository.CreateWorkShiftAsync(ws).ConfigureAwait(false);
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<WorkShift_dto>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetActive()
        {
            return await _workShiftRepository.GetActiveWorkShiftsAsync().ConfigureAwait(false);
        }

        [HttpGet]
        [Route("[action]/{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Close(int id)
        {
            return await _workShiftRepository.CloseWorkShiftAsync(id).ConfigureAwait(false);
        }
    }
}
