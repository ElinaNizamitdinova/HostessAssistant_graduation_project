using Microsoft.AspNetCore.Mvc;
using static ElinaTestProject.Models.Admin.AdminRepository;
using System.Threading.Tasks;

namespace ElinaTestProject.Interfaces.History
{
    public interface IHistoryRepository
    {
        Task<IActionResult> GetTableHistory(int id);
        Task<IActionResult> GetWaiterHistory(int id);
        Task<IActionResult> GetServiceHistory(int id);


    }
}
