using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ElinaTestProject.Interfaces.History;
using static ElinaTestProject.Models.History.HistoryRepository;

namespace ElinaTestProject.Controllers.History
{
    public class HistoryController : ControllerBase
    {
        public IHistoryRepository _historyRepository;
        [HttpGet]
        [Route("[action]/{id:int}")]
        [ProducesResponseType(typeof(HistoryInfo), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetWaiterHistory(int id)
        {
            return await _historyRepository.GetWaiterHistory(id);
        }
    }
}
