using ElinaTestProject.Interfaces.Reservation;
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
using static ElinaTestProject.Models.User.UserRepository;

namespace ElinaTestProject.Models.Reservation
{
    public partial class ReservationRepository : IReservationRepository
    {
        private readonly string _objectName = nameof(ReservationRepository);

        private readonly ILogger _logger;
        private readonly TestDbContext _context;

        public ReservationRepository(TestDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger(_objectName);
        }


        public async Task<IActionResult> CloseReservation(int id)
        {
            _logger.LogInformation($"Try to close reservation with id: {id}");

            try
            {
                var msg = await CompleteReservation(id).ConfigureAwait(false);

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

        public async Task<IActionResult> CreateReservationAsync(Reservation_dto reservation)
        {
            _logger.LogInformation($"Try to create reservation for date: {reservation.ReservationDt}");

            try
            {
                var (msg, rsv) = await AddReservationToDbAsync(reservation).ConfigureAwait(false);

                if (rsv == null)
                    return new BadRequestObjectResult(msg);

                return new OkObjectResult(rsv);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to create reservation for date: {reservation.ReservationDt}");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }

        }

        public async Task<IActionResult> FindReservationAsync(ReservationFinder finder)
        {
            _logger.LogInformation($"Try to find reservation by params");

            try
            {
                var (msg, rsrv) = await FindReservationFromDB(finder).ConfigureAwait(false);

                if (rsrv == null || rsrv.Count == 0)
                    return new BadRequestObjectResult(msg);

                return new OkObjectResult(rsrv);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to find reservation by params");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> GetReservationAsync(int id)
        {
            _logger.LogInformation($"Try to get reservation by params");
            try
            {
                var (msg, rsrv) = await GetReservationFromDbAsync(id).ConfigureAwait(false);

                if (rsrv == null)
                    return new BadRequestObjectResult(msg);

                return new OkObjectResult(rsrv);

            }
            catch(Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to get reservation by params");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async  Task<IActionResult> UpdateReservationAsync(Reservation_dto reservation)
        {
            _logger.LogInformation($"Try to update existing reservation");

            try
            {
                var (msg, rsrv) = await UpdateExistingReservationAsync(reservation).ConfigureAwait(false);

                if (rsrv == null)
                {
                    return new BadRequestObjectResult(msg);
                }

                return new OkObjectResult(rsrv);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to update existing reservation");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        private async Task<(string, Reservation_dto?)> AddReservationToDbAsync(Reservation_dto reservation)
        {
            var startTime = reservation.ReservationDt;
            var endTime = reservation.ReservationDt + reservation.Duration;

            var tableIds = await _context.TableDbs
                .Where(x => x.TableStatusId == (int)TableStatusTypeEnum.Ready)
                .Select(x => x.TableId)
                .ToListAsync()
                .ConfigureAwait(false);

            var excepts = reservation.TableIdList.Except(tableIds);

            if (excepts.Any())
            {
                return ($"Next table(s) is invalid: {String.Join(", ", excepts)}", null);
            }

            //TODO: реализовать метод автоматического назначения столов по количеству мест
            // проверка на пересечения броней

            //TODO: проверить!!!
            var existingReservations = await _context.ReservationTables
                .Include(x => x.Reservation)
                .Where(x => x.Reservation.ReservationStatusId == (int)ReservationStatusTypeEnum.Active
                        && reservation.TableIdList.Contains(x.TableId.Value)
                        && (x.Reservation.CreateDateTime >= startTime || x.Reservation.CreateDateTime <= endTime))
                .ToListAsync()
                .ConfigureAwait(false);

            if (existingReservations.Any())
            {
                return ($"the reservation has intersections", null);
            }


            // используем транзакцию чтобы обеспечить консистентность данных 
            await _context.Database.BeginTransactionAsync().ConfigureAwait(false);

            try
            {
                var reservation_db = new PostGreContext.Models.Reservation
                {
                    OriginTypeId = reservation.OriginTypeId,
                    Duration = reservation.Duration,
                    CreateDateTime = reservation.ReservationDt,
                    PersonQuantity = reservation.PersonQuantity,
                    ReservationStatusId = reservation.ReservationStatusId,
                };

                await _context.Reservations.AddAsync(reservation_db).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                var reservationTables = new List<ReservationTable>(reservation.TableIdList.Count);

                foreach (var id in reservation.TableIdList)
                {
                    var rt = new ReservationTable
                    {
                        ReservationId = reservation_db.ReservationId,
                        TableId = id,
                    };
                    reservationTables.Add(rt);
                }

                await _context.ReservationTables.AddRangeAsync(reservationTables).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                reservation.Id = reservation_db.ReservationId;

                _context.Database.CommitTransaction();

                return (string.Empty, reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while reservation transaction. Rollback transaction!");

                if (_context.Database.CurrentTransaction != null)
                    _context.Database.RollbackTransaction();

                return ($"Exception {ex.Message} while reservation transaction. Rollback transaction!", null);
            }

        }

        private async Task<string> CompleteReservation(int id)
        {
            var rsv = await _context.Reservations
                .Where(x => x.ReservationId == id && x.ReservationStatusId == (int)ReservationStatusTypeEnum.Active)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (rsv == null)
                return $"No active reservation with id: {id}";

            return string.Empty;

        }
        private async Task<(string, List<Reservation_dto>?)> FindReservationFromDB(ReservationFinder finder)
        {
            var query = _context.Reservations
                .Where(x => x.ReservationStatusId == (int)ReservationStatusTypeEnum.Active)
                .AsNoTracking()
                .Select(x => new Reservation_dto
                {
                    Id = x.ReservationId,
                    OriginTypeId =x.OriginTypeId,
                    Duration = x.Duration,
                    PersonQuantity = x.PersonQuantity,
                    ReservationStatusId = x.ReservationStatusId,

                   
                });

            if (finder != null)
            {
                if (finder.OriginTypeId.HasValue)
                    query = query.Where(x => x.OriginTypeId == finder.OriginTypeId);

                if (finder.TableIdList.Count > 0)
                    query = query.Where(x => x.TableIdList == finder.TableIdList);

                if(finder.ReservationStatusId.HasValue)
                    query = query.Where(x => x.ReservationStatusId == finder.ReservationStatusId);
            }

            var result = await query.ToListAsync().ConfigureAwait(false);

            if (result.Count == 0)
                return ("could not find objects matching the request", null);

            return (string.Empty, result);
        }

        private async Task<(string, Reservation_dto?)> GetReservationFromDbAsync(int id)
        {
            var reservation = await _context.Reservations
                .Where(x => x.ReservationId == id && x.ReservationStatusId == (int)ReservationStatusTypeEnum.Active)
                .AsNoTracking()
                .Select(x => new Reservation_dto
                {
                    Id = x.ReservationId,
                    OriginTypeId = x.OriginTypeId,
                    Duration = x.Duration,
                    PersonQuantity = x.PersonQuantity,
                    ReservationStatusId = x.ReservationStatusId,


                }).FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (reservation == null)
                return ($"User with id: {id} not found", null);

            return (string.Empty, reservation);
        }
        private async Task<(string, Reservation_dto?)> UpdateExistingReservationAsync(Reservation_dto reservation)
        {
            var db_reservation = await _context.Reservations
                .Where(x => x.ReservationStatusId == (int)ReservationStatusTypeEnum.Active && x.ReservationId == reservation.Id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (db_reservation == null) {
                return ($"Reservation not found", null); }

            db_reservation.ReservationId= reservation.Id.Value;
            db_reservation.OriginTypeId = reservation.OriginTypeId;
            db_reservation.ReservationStatusId = reservation.ReservationStatusId;
            db_reservation.Duration= reservation.Duration;
            db_reservation.CreateDateTime = reservation.ReservationDt;


            await _context.SaveChangesAsync().ConfigureAwait(false);

            return (string.Empty, reservation);

        }
    }
}
