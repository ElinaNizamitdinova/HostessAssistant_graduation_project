using ElinaTestProject.Interfaces.WorkShift;
using ElinaTestProject.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PostGreContext.Context;
using PostGreContext.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElinaTestProject.Models.WorkShift
{
    public partial class WorkShiftRepository : IWorkShiftRepository
    {
        private readonly string _objectName = nameof(WorkShiftRepository);

        private readonly ILogger _logger;
        private readonly TestDbContext _context;

        public WorkShiftRepository(TestDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger(_objectName);
        }
        public async Task<IActionResult> CloseWorkShiftAsync(int id)
        {
            _logger.LogInformation($"Try to close work shift with id: {id}");

            try
            {
                var res = await CloseWs(id).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(res))
                    return new BadRequestObjectResult(res);

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to close work shift with id: {id}");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> CreateWorkShiftAsync(WorkShift_dto workShift)
        {
            _logger.LogInformation($"Try to create new work shift");

            try
            {
                return new OkObjectResult(await AddNewWsAsync(workShift).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to create new work shift");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> GetActiveWorkShiftsAsync()
        {
            _logger.LogInformation($"Try to get all active work shifts");

            try
            {
                var res = await GetAllActiveWsAsync().ConfigureAwait(false);

                return new OkObjectResult(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to get all active work shifts");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        private async Task<WorkShift_dto> AddNewWsAsync(WorkShift_dto ws)
        {
            var ws_db = new PostGreContext.Models.WorkShift
            {
                WorkShiftUserTypeId = ws.UserTypeId,
                WorkShiftStatusId = ws.StatusId,
                StartDateTime = DateTime.UtcNow
            };

            await _context.WorkShifts.AddAsync(ws_db).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            ws.Id = ws_db.WorkShiftId;
            ws.StartDateTime = ws_db.StartDateTime;

            return ws;
        }

        private async Task<string> CloseWs(int id)
        {
            var ws_db = await _context.WorkShifts
                .Include(x => x.Qeues)
                .Where(x => x.WorkShiftStatusId == (int)WorkShiftStatusTypeEnum.Active && x.WorkShiftId == id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (ws_db == null)
                return $"Can't find active work shift with id: {id}";

            ws_db.WorkShiftStatusId = (int)WorkShiftStatusTypeEnum.Closed;
            ws_db.EndDateTime = DateTime.UtcNow;

            //  закрыть смену всем дочерним объектам
            if (ws_db.Qeues != null && ws_db.Qeues.Count > 0)
            {
                foreach (var item in ws_db.Qeues)
                {
                    item.QeueStatusId = (int)QueueStatusTypeEnum.Completed;
                    item.EndDateTime = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return string.Empty;
        }

        private async Task<List<WorkShift_dto>> GetAllActiveWsAsync()
        {
            var result = await _context.WorkShifts
                .Where(x => x.WorkShiftStatusId == (int)WorkShiftStatusTypeEnum.Active)
                .AsNoTracking()
                .Select(x => new WorkShift_dto
                {
                    Id = x.WorkShiftId,
                    UserTypeId = x.WorkShiftUserTypeId,
                    StatusId = x.WorkShiftStatusId,
                    StartDateTime = x.StartDateTime,
                    EndDateTime = x.EndDateTime
                })
                .ToListAsync()
                .ConfigureAwait(false);

            return result;
        }

        Task<(string, PostGreContext.Models.WorkShift)> IWorkShiftRepository.GetActiveWsById(int id, TestDbContext dbContext) => GetActiveWsById(id, dbContext);

        private async Task<(string, PostGreContext.Models.WorkShift)> GetActiveWsById(int id, TestDbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = _context;

            var result = await dbContext.WorkShifts
                .Where(x => x.WorkShiftStatusId == (int)WorkShiftStatusTypeEnum.Active && x.WorkShiftId == id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (result == null)
                return ($"No active work shifts with id: {id} in database", null);

            return (string.Empty, result);
        }
    }
}
