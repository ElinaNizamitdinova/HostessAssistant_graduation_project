using ElinaTestProject.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PostGreContext.Context;
using System.Threading.Tasks;

namespace ElinaTestProject.Models.Admin
{
    public partial class AdminRepository : IAdminInterface
    {
        private const string _objectName = nameof(AdminRepository);
        private const int _badLoginCountAvailable = 3;

        private readonly TestDbContext _context;
        private readonly ILogger _logger;

        public AdminRepository(TestDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger(_objectName);
        }

        public async Task<IActionResult> LoginRequestAsync(LoginRequestItem item)
        {
            var users = await _context.UserDbs.ToListAsync().ConfigureAwait(false);

            return new OkResult();
        }
    }
}
