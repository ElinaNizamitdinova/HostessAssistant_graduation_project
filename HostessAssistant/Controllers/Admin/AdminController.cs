using ElinaTestProject.Interfaces.Admin;
using ElinaTestProject.Utils;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static ElinaTestProject.Models.Admin.AdminRepository;

namespace ElinaTestProject.Controllers.Admin
{
    /// <summary>
    /// Admin Controller
    /// </summary>
    public class AdminController : ControllerBase
    {
        private IAdminInterface _adminRepository;

        public AdminController(IAdminInterface adminRepository)
        {
            _adminRepository = adminRepository;
        }

        [HttpPost]
        [Route("/api/[controller]/[action]")]
        public async Task<IActionResult> LoginRequest([FromBody] LoginRequestItem item)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            return await _adminRepository.LoginRequestAsync(item).ConfigureAwait(false);
        }

    }
}
