using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static ElinaTestProject.Models.Queue.QueueRepository;

namespace ElinaTestProject.Interfaces.Queue
{
    public interface IQueueRepository
    {
        Task<IActionResult> AddUserToQueueAsync(Queue_dto queue);
        Task<IActionResult> FindUserQueueAsync(QueueFinder finder);
        Task<IActionResult> GetUserQueuePrioritiesAsync(int id);
        Task<IActionResult> UpdateUserQueueAsync(Queue_dto queue);
        Task<IActionResult> CloseUserQueueAsync(int id);
    }
}
