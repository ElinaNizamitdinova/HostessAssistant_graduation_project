using ElinaTestProject.Interfaces.History;
using ElinaTestProject.Models.Reservation;
using ElinaTestProject.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PostGreContext.Context;
using PostGreContext.Enums;
using PostGreContext.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using static ElinaTestProject.Models.Order.OrderRepository;
using static ElinaTestProject.Models.Reservation.ReservationRepository;
using static ElinaTestProject.Models.Table.TableRepository;

namespace ElinaTestProject.Models.History
{
    public partial class HistoryRepository : IHistoryRepository
    {
        private readonly string _objectName = nameof(HistoryRepository);
        private readonly ILogger _logger;
        private readonly TestDbContext _context;

        public HistoryRepository(TestDbContext context, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(_objectName);
            _context = context;
        }

        public async Task<IActionResult> GetServiceHistory(int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IActionResult> GetWaiterHistory(int id)
        {
            _logger.LogInformation($"Try to get waiter tables by id");
            try
            {
                var (msg, his) = await GetWaiterTableFromDbAsync(id).ConfigureAwait(false);

                if (his == null)
                    return new BadRequestObjectResult(msg);

                return new OkObjectResult(his);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to get reservation by params");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> GetTableHistory(int id)
        {
            throw new System.NotImplementedException();
        }

        private async Task<(string, HistoryInfo?)> GetWaiterTableFromDbAsync(int id)
        {
            Dictionary<List<int?>, OrderStatusTypeEnum> tables = new Dictionary<List<int?>, OrderStatusTypeEnum>();
            foreach (OrderDb order in _context.OrderDbs)
            {
                if (order.UserId == id)
                {
                    List<int?> tablesNumber = [order.OrderTables.Count];
                    foreach (var table in order.OrderTables)
                    {
                        tablesNumber.Add(table.TableId);
                    }
                    tables.Add(tablesNumber, (OrderStatusTypeEnum)Enum.Parse(typeof(OrderStatusTypeEnum), (order.OrderStatusId).ToString()));
                }
            }
            var history = await _context.UserDbs
              .Where(x => x.UserId == id)
              .AsNoTracking()
              .Select(x => new HistoryInfo
              {
                  UserId = x.UserId,
                  UserName = x.UserName,
                  Tables = tables
              }).FirstOrDefaultAsync()
                .ConfigureAwait(false); ;


            if (history == null)
                return ($"User with id: {id} not found", null);

            return (string.Empty, history);

        }
    }
}
