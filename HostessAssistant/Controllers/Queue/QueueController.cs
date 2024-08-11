using ElinaTestProject.Interfaces.Queue;
using ElinaTestProject.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ElinaTestProject.Models.Queue.QueueRepository;
using static ElinaTestProject.Models.User.UserRepository;

namespace ElinaTestProject.Controllers.Queue
{
    [Route("api/[controller]")]
    public class QueueController : Controller
    {
        private IQueueRepository _queueRepository;

        public QueueController(IQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(Queue_dto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ApplyQueue([FromBody] Queue_dto queue)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ControllerUtils.GetModelErrors(ModelState));

            if (!queue.Id.HasValue)
                return await _queueRepository.AddUserToQueueAsync(queue).ConfigureAwait(false);
            else
                return await _queueRepository.UpdateUserQueueAsync(queue).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<Queue_dto>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Find([FromBody]QueueFinder finder)
        {
            return await _queueRepository.FindUserQueueAsync(finder).ConfigureAwait(false);
        }

        [HttpGet]
        [Route("[action]/{id:int}")]
        [ProducesResponseType(typeof(List<QueuePriorityReply>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetPriorities(int id)
        {
            return await _queueRepository.GetUserQueuePrioritiesAsync(id).ConfigureAwait(false);
        }

        [HttpGet]
        [Route("[action]/{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> CloseQueue(int id)
        {
            return await _queueRepository.CloseUserQueueAsync(id).ConfigureAwait(false);
        }
    }
}
