using ElinaTestProject.Interfaces.Queue;
using ElinaTestProject.Interfaces.User;
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

namespace ElinaTestProject.Models.Queue
{
    public partial class QueueRepository : IQueueRepository
    {
        private readonly string _objectName = nameof(QueueRepository);

        private readonly ILogger _logger;
        private readonly TestDbContext _context;

        private readonly IWorkShiftRepository _workShiftRepository;
        private readonly IUserRepository _userRepository;

        public QueueRepository(TestDbContext context, ILoggerFactory loggerFactory, IWorkShiftRepository workShiftRepository, IUserRepository userRepository)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger(_objectName);
            _workShiftRepository = workShiftRepository;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> AddUserToQueueAsync(Queue_dto queue)
        {
            _logger.LogInformation($"Try to add user to qeue with name: {queue.UserId}");

            try
            {
                var (msg, res) = await AddQeue(queue).ConfigureAwait(false);

                if (res == null)
                    return new BadRequestObjectResult(msg);

                return new OkObjectResult(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to add user to qeue with name: {queue.UserId}");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> CloseUserQueueAsync(int id)
        {
            _logger.LogInformation($"Try to close user queue with id: {id}");

            try
            {
                var msg = await CloseQueueAsync(id).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(msg))
                    return new BadRequestObjectResult(msg);

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to close user queue with id: {id}");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> FindUserQueueAsync(QueueFinder finder)
        {
            _logger.LogInformation($"Try to find queue by params");

            try
            {
                var (msg, queues) = await FindQueueByParamsAsync(finder).ConfigureAwait(false);

                if (queues == null || queues.Count == 0)
                    return new BadRequestObjectResult(msg);

                return new OkObjectResult(queues);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to find queue by params");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> GetUserQueuePrioritiesAsync(int id)
        {
            _logger.LogInformation($"Try to get worker with id {id} queue priorities");

            try
            {
                var (msg, res) = await FindQueueByParamsAsync(new QueueFinder { UserId = id }).ConfigureAwait(false);

                if (res == null || res.Count == 0)
                    return new BadRequestObjectResult(msg);

                return new OkObjectResult(res.Select(x => new QueuePriorityReply
                {
                    UserId = x.UserId,
                    UserName = x.UserName,
                    OrderSequence = x.OrderSequence,
                    PriorityOrderSequence = x.PriorityOrderSequence
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to get worker with id {id} queue priorities");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> UpdateUserQueueAsync(Queue_dto qeue)
        {
            _logger.LogInformation($"Try to get worker with id {qeue.Id} queue");

            try
            {
                var (msg, res) = await UpdateQueueAsync(qeue).ConfigureAwait(false);

                if (res == null)
                    return new BadRequestObjectResult(msg);

                return new OkObjectResult(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to get worker with id {qeue.Id} queue");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        private async Task<(string, Queue_dto?)> AddQeue(Queue_dto qeue)
        {
            var (ws_msg, ws) = await _workShiftRepository.GetActiveWsById(qeue.WorkShiftId, _context).ConfigureAwait(false);

            if (ws == null)
                return (ws_msg, null);

            var (usr_msg, usr) = await _userRepository.GetUserById(qeue.UserId, _context).ConfigureAwait(false);

            if (usr == null)
                return (usr_msg, null);

            // нужно сравнить типы пользователя и рабочей смены. они должны быть одинаковые иначе можем получить кейс
            // в котором официант может быть заведён в смене менеджеров или что-то типа того

            if (ws.WorkShiftUserTypeId != usr.UserTypeId)
                return ($"User type {usr.UserTypeId} mismatch with work shift user type {ws.WorkShiftUserTypeId}!", null);

            // нужно проверить приоритет получения заказа в текущей смене и для текущего типа работника ( актуально только официантов)
            var existing_qeue = await _context.Qeues
                .Where(x => x.QeueStatusId == (int)QueueStatusTypeEnum.Active)
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);

            if (!existing_qeue.Select(x => x.OrderSequence == qeue.OrderSequence).Any())
                return ($"Order sequence: {qeue.OrderSequence} already busy by user id: {existing_qeue.Where(x => qeue.OrderSequence == x.OrderSequence).Select(x => x.UserId)}", null);

            if (!existing_qeue.Select(x => x.PriorityOrderSequence == qeue.PriorityOrderSequence).Any())
                return ($"Priority order sequence: {qeue.PriorityOrderSequence} already busy by user id: {existing_qeue.Where(x => qeue.PriorityOrderSequence == x.PriorityOrderSequence).Select(x => x.UserId)}", null);

            var qeue_db = new PostGreContext.Models.Qeue
            {
                WorkShiftId = qeue.WorkShiftId,
                UserId = qeue.UserId,
                QeueStatusId = (int)QueueStatusTypeEnum.Active,
                OrderSequence = qeue.OrderSequence,
                PriorityOrderSequence = qeue.PriorityOrderSequence,
                StartDateTime = DateTime.UtcNow,
            };

            await _context.Qeues.AddAsync(qeue_db).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            qeue.Id = qeue_db.QeueId;

            return (string.Empty, qeue);
        }

        private async Task<string> CloseQueueAsync(int id)
        {
            var queue = await _context.Qeues.
                Where(x => x.QeueStatusId != (int)QueueStatusTypeEnum.Completed && x.QeueId == id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (queue == null)
                return $"Can't find active or paused queue with id: {id}";

            queue.QeueStatusId = (int)QueueStatusTypeEnum.Completed;
            queue.EndDateTime = DateTime.UtcNow;

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return string.Empty;
        }

        private async Task<(string, List<Queue_dto>?)> FindQueueByParamsAsync(QueueFinder finder)
        {
            var query = _context.Qeues
                .Include(x => x.User)
                .Where(x => x.QeueStatusId != (int)QueueStatusTypeEnum.Completed)
                .Select(x => new Queue_dto
                {
                    Id = x.QeueId,
                    WorkShiftId = x.WorkShiftId,
                    UserId = x.UserId,
                    UserName = x.User.UserName,
                    StatusId = x.QeueStatusId,
                    OrderSequence = x.OrderSequence,
                    PriorityOrderSequence = x.PriorityOrderSequence,
                    StartDateTime = x.StartDateTime
                });

            if (finder != null)
            {
                if (finder.WorkShiftId.HasValue)
                    query = query.Where(x => x.WorkShiftId == finder.WorkShiftId.Value);

                if (finder.UserId.HasValue)
                    query = query.Where(x => x.UserId == finder.UserId.Value);

                if (finder.StatusId.HasValue)
                    query = query.Where(x => x.StatusId == finder.StatusId.Value);
            }

            var result = await query.ToListAsync().ConfigureAwait(false);

            if (result.Count == 0)
                return ("could not find objects matching the request", null);

            return (string.Empty, result);
        }

        private async Task<(string, Queue_dto?)> UpdateQueueAsync(Queue_dto qeue)
        {
            var res = await _context.Qeues
                .Where(x => x.QeueStatusId != (int)QueueStatusTypeEnum.Completed)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (res == null)
                return ($"Can't find active or paused queue with id: {qeue.UserId}", null);

            //TODO: сделать проверку чтобы не было пересечения по приоритетам в случае изменения в очереди.

            res.QeueStatusId = qeue.StatusId;
            res.OrderSequence = qeue.OrderSequence;
            res.PriorityOrderSequence = qeue.PriorityOrderSequence;

            return (string.Empty, qeue);
        }
    }
}
