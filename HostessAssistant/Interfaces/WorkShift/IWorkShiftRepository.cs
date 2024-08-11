using Microsoft.AspNetCore.Mvc;
using PostGreContext.Context;
using System.Threading.Tasks;
using static ElinaTestProject.Models.WorkShift.WorkShiftRepository;

namespace ElinaTestProject.Interfaces.WorkShift
{
    public interface IWorkShiftRepository
    {
        Task<IActionResult> CreateWorkShiftAsync(WorkShift_dto workShift);
        Task<IActionResult> GetActiveWorkShiftsAsync();        
        Task<IActionResult> CloseWorkShiftAsync(int id);

        internal Task<(string, PostGreContext.Models.WorkShift)> GetActiveWsById(int id, TestDbContext dbContext = null);
    }
}
