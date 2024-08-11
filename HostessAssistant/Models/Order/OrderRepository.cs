using ElinaTestProject.Interfaces.Order;
using ElinaTestProject.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PostGreContext.Context;
using PostGreContext.Enums;
using PostGreContext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElinaTestProject.Models.Order
{
    public partial class OrderRepository : IOrderRepository
    {
        private readonly string _objectName = nameof(OrderRepository);

        private readonly ILogger _logger;
        private readonly TestDbContext _context;

        public OrderRepository(TestDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger(_objectName);
        }
        public async Task<IActionResult> CloseOrderAsync(int id)
        {
            _logger.LogInformation($"Try to close order with id: {id}");

            try
            {
                var msg = await CloseOrderInDbAsync(id).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(msg))
                    return new OkObjectResult(msg);

                return new OkResult();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to close reservation with id: {id}");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> CreateOrderAsync(Order_dto order)
        {
            _logger.LogInformation($"Try to create order");

            try
            {
                var (msg, ord) = await AddNewOrderAsync(order).ConfigureAwait(false);

                if (ord == null)
                    return new BadRequestObjectResult(msg);

                return new OkObjectResult(ord);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to create order");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> FindOrderAsync(OrderFinder finder)
        {
            _logger.LogInformation($"Try to find order by params");

            try
            {
                var (msg, ordrs) = await FindOrderFromDBAsync(finder).ConfigureAwait(false);

                if (ordrs == null || ordrs.Count == 0)
                    return new BadRequestObjectResult(msg);

                return new OkObjectResult(ordrs);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to find order by params");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> GetOrderAsync(int id)
        {
            _logger.LogInformation($"Try to get order with id {id}");

            try
            {
                var (msg, res) = await GetOrderByIdAsync(id).ConfigureAwait(false);

                if (res == null)
                    return new BadRequestObjectResult(msg);

                return new OkObjectResult(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to get order with id {id}");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> UpdateOrderAsync(Order_dto order)
        {
            _logger.LogInformation($"Try to get udate order with id {order.Id}");

            try
            {
                var (msg, res) = await UpdateExistingOrderAsync(order).ConfigureAwait(false);

                if (res == null)
                    return new BadRequestObjectResult(msg);

                return new OkObjectResult(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to get udate order with id {order.Id}");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        private async Task<string> CloseOrderInDbAsync(int id)
        {
            var order = await _context.OrderDbs
                .Where(x => x.OrderId == id && x.OrderStatusId == (int)OrderStatusTypeEnum.Active)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (order == null)
                return $"No active order with id: {id}";

            return string.Empty;
        }

        private async Task<(string, Order_dto?)> AddNewOrderAsync(Order_dto order)
        {

            // проверим не пересекаемся ли мы с текущими активными order

            var orderTableIds = await _context.OrderTables
                .Include(x => x.Order)
                .Where(x => x.Order != null && x.Order.OrderStatusId == (int)OrderStatusTypeEnum.Active && order.TableIdList.Contains(x.TableId.Value))
                .Select(x => x.TableId)
                .ToListAsync()
                .ConfigureAwait(false);

            if (orderTableIds.Any())
            {
                return ($"Next table(s) is intersect by another order: {String.Join(", ", orderTableIds)}", null);
            }


            // используем транзакцию чтобы обеспечить консистентность данных 
            await _context.Database.BeginTransactionAsync().ConfigureAwait(false);

            try
            {
                var order_db = new OrderDb
                {
                    WorkShiftId = order.WorkShiftId,
                    UserId = order.UserId.Value,
                    ReservationId = order.ReservationId,
                    OrderStatusId = order.OrderStatusId,
                    StartDateTime = DateTime.UtcNow,
                    IsPriority = order.IsPriority,
                };

                await _context.OrderDbs.AddAsync(order_db).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                order.Id = order_db.OrderId;
                order.StartDateTime = order_db.StartDateTime;

                foreach (var id in order.TableIdList)
                {
                    var orderTableDb = new OrderTable
                    {
                        OrderId = order.Id.Value,
                        TableId = id
                    };

                    await _context.OrderTables.AddAsync(orderTableDb).ConfigureAwait(false);
                }

                await _context.SaveChangesAsync().ConfigureAwait(false);

                _context.Database.CommitTransaction();

                return (string.Empty, order);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while order transaction. Rollback transaction!");

                if (_context.Database.CurrentTransaction != null)
                    _context.Database.RollbackTransaction();

                return ($"Exception {ex.Message} while order transaction. Rollback transaction!", null);
            }
        }

        private async Task<(string, List<Order_dto>)> FindOrderFromDBAsync(OrderFinder finder)
        {
            var query = _context.OrderDbs
                .Include(x => x.User)
                .AsNoTracking()
                .Select(x => new Order_dto
                {
                    Id = x.OrderId,
                    UserId = x.UserId,
                    UserName = x.User.UserName,
                    OrderStatusId = x.OrderStatusId,
                    ReservationId = x.ReservationId,
                    StartDateTime = x.StartDateTime,
                    EndDateTime = x.EndDateTime,
                    IsPriority = x.IsPriority.Value
                });

            if (finder != null)
            {
                if (finder.OrderStatusId != null)
                    query = query.Where(x => x.OrderStatusId == finder.OrderStatusId);

                if (finder.UserId != null)
                    query = query.Where(x => x.UserId == finder.UserId);
            }

            var result = await query.ToListAsync().ConfigureAwait(false);

            var tableOrders = await _context.OrderTables
                .Where(x => result.Select(y => y.Id).Contains(x.TableId))
                .ToListAsync()
                .ConfigureAwait(false);

            result.ForEach(x => x.TableIdList = tableOrders.Where(y => y.OrderId == x.Id).Select(x => x.TableId.Value).ToList());

            if (result.Count == 0)
                return ("could not find objects matching the request", null);

            return (string.Empty, result);
        }

        private async Task<(string, Order_dto?)> GetOrderByIdAsync(int id)
        {
            var query = from o in _context.OrderDbs.Where(x => x.OrderId == id)
                        from ot in _context.OrderTables.Where(x => x.OrderId == id).DefaultIfEmpty()
                        from u in _context.UserDbs.Where(x => x.UserId == o.UserId).DefaultIfEmpty()
                        select new
                        {
                            o.OrderId,
                            o.WorkShiftId,
                            o.UserId,
                            u.UserName,
                            o.OrderStatusId,
                            o.ReservationId,
                            table_id = ot.TableId,
                            o.StartDateTime,
                            o.EndDateTime,
                            o.IsPriority,
                        };

            var result = await query.ToListAsync().ConfigureAwait(false);

            var groupedRes = result.GroupBy(r => new
            {
                r.OrderId,
                r.WorkShiftId,
                r.UserId,
                r.UserName,
                r.OrderStatusId,
                r.ReservationId,
                r.StartDateTime,
                r.EndDateTime,
                r.IsPriority
            }).Select(gr => new Order_dto
            {
                Id = gr.Key.OrderId,
                WorkShiftId = gr.Key.WorkShiftId,
                UserId = gr.Key.UserId,
                UserName = gr.Key.UserName,
                OrderStatusId = gr.Key.OrderStatusId,
                ReservationId = gr.Key.ReservationId,
                TableIdList = gr.Where(x => x.table_id.HasValue).Select(x => x.table_id.Value).Distinct().ToList(),
                StartDateTime = gr.Key.StartDateTime,
                EndDateTime = gr.Key.EndDateTime,
                IsPriority = gr.Key.IsPriority
            })
            .FirstOrDefault();


            if (result.Count == 0)
                return ("could not find objects matching the request", null);

            return (string.Empty, groupedRes);

        }

        private async Task<(string, Order_dto?)> UpdateExistingOrderAsync(Order_dto order)
        {
            var res_db = await _context.OrderDbs
                .Where(x => x.OrderStatusId == (int)OrderStatusTypeEnum.Active && order.Id == x.OrderId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (res_db == null)
                return ($"Can't find active order with id: {order.Id}", null);

            res_db.WorkShiftId = order.WorkShiftId;
            res_db.UserId = order.UserId.Value;
            res_db.ReservationId = order.ReservationId;
            res_db.OrderStatusId = order.OrderStatusId;
            res_db.IsPriority = order.IsPriority;

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return (string.Empty, order);

        }
    }
}
