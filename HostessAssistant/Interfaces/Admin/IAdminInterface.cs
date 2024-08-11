using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static ElinaTestProject.Models.Admin.AdminRepository;


namespace ElinaTestProject.Interfaces.Admin
{
    /// <summary>
    /// IAdmin interface
    /// </summary>
    public interface IAdminInterface
    {
        Task<IActionResult> LoginRequestAsync(LoginRequestItem item);
    }
}
