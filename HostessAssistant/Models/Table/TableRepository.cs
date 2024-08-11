using ElinaTestProject.Interfaces.Table;
using ElinaTestProject.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PostGreContext.Context;
using PostGreContext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElinaTestProject.Models.Table
{
    public partial class TableRepository : ITableRepository
    {
        private readonly string _objectName = nameof(TableRepository);

        private readonly ILogger _logger;
        private readonly TestDbContext _context;

        public TableRepository(ILoggerFactory loggerFactory, TestDbContext context)
        {
            _logger = loggerFactory.CreateLogger(_objectName);
            _context = context;
        }

        public async Task<IActionResult> FindTableAsync(TableFinder finder)
        {
            _logger.LogInformation($"Try to find table by params");

            try
            {
                var (msg, tbls) = await FindTablesFromDb(finder).ConfigureAwait(false);

                if(tbls == null || tbls.Count == 0)
                    return new BadRequestObjectResult(msg);

                return new OkObjectResult(tbls);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to find table by params");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> AddTableAsync(Table_dto table)
        {
            _logger.LogInformation($"Try to add new table with number: {table.Number}");

            try
            {
                return new OkObjectResult(await AddNewTableAsync(table).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to add new table with number: {table.Number}");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> DeleteTableAsync(int id)
        {
            _logger.LogInformation($"Try to remove table with id: {id}");

            try
            {
                var res = await RemoveTableAsync(id).ConfigureAwait(false);

                if (string.IsNullOrEmpty(res))
                    return new BadRequestObjectResult(res);

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to remove table with id: {id}");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }

        }

        public async Task<IActionResult> GetTableAsync(int id)
        {
            _logger.LogInformation($"Try to get from database table with id: {id}");

            try
            {
                var (msg, table) = await GetTableFromDbAsync(id).ConfigureAwait(false);

                if (table == null)
                    return new BadRequestObjectResult(msg);

                return new OkObjectResult(table);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to get from database table with id: {id}");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> UpdateTableAsync(Table_dto table)
        {
            _logger.LogInformation($"Try to update existing table with id: {table.Id}");

            try
            {
                var (msg, tbl) = await UpdateExistingTableAsync(table).ConfigureAwait(false);

                if (tbl == null)
                    return new BadRequestObjectResult(msg);

                return new OkObjectResult(tbl);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to update existing table with id: {table.Id}");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        private async Task<Table_dto> AddNewTableAsync(Table_dto table)
        {
            var db_table = new TableDb
            {
                TableNumber = table.Number,
                TableStatusId = table.StatusId,
                MaxCapacity = table.MaxCapacity,
                Comment = table.Comment
            };

            await _context.TableDbs.AddAsync(db_table);
            await _context.SaveChangesAsync();

            table.Id = db_table.TableId;

            return table;
        }

        private async Task<string> RemoveTableAsync(int id)
        {
            var (msg, tbl) = await GetTableFromDataBase(id).ConfigureAwait(false);

            if (tbl == null)
                return msg;

            _context.TableDbs.Remove(tbl);

            await _context.SaveChangesAsync();

            return string.Empty;
        }
        private async Task<(string, Table_dto?)> GetTableFromDbAsync(int id)
        {
            var (msg, tbl) = await GetTableFromDataBase(id).ConfigureAwait(false);

            if (tbl == null)
                return (msg, null);

            return (string.Empty, new Table_dto
            {
                Id = tbl.TableId,
                Number = tbl.TableNumber,
                StatusId = tbl.TableStatusId,
                MaxCapacity = tbl.MaxCapacity,
                Comment = tbl.Comment
            });
        }

        private async Task<(string, Table_dto?)> UpdateExistingTableAsync(Table_dto table)
        {
            var (msg, tbl) = await GetTableFromDataBase(table.Id.Value).ConfigureAwait(false);

            if (tbl == null)
                return (msg, null);

            tbl.TableNumber = table.Number;
            tbl.TableStatusId = table.StatusId;
            tbl.MaxCapacity = table.MaxCapacity;
            tbl.Comment = table.Comment;

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return (string.Empty, table);
        }

        private async Task<(string, TableDb?)> GetTableFromDataBase(int id)
        {
            var tbl = await _context.TableDbs.FindAsync(id).ConfigureAwait(false);

            if (tbl == null)
                return ($"Can't find table with id: {id}", null);

            return (string.Empty, tbl);
        }

        private async Task<(string, List<Table_dto>?)> FindTablesFromDb(TableFinder finder)
        {
            var query = _context.TableDbs
                .AsNoTracking()
                .Select(x => new Table_dto
                {
                    Id = x.TableId,
                    Number = x.TableNumber,
                    StatusId = x.TableStatusId,
                    MaxCapacity = x.MaxCapacity,
                    Comment = x.Comment
                });

            if (finder != null)
            {
                if (finder.TableNumber.HasValue)
                    query = query.Where(x => x.Number == finder.TableNumber.Value);

                if (finder.TableStatusId.HasValue)
                    query = query.Where(x => x.StatusId == finder.TableStatusId.Value);

                if (finder.MaxCapacity.HasValue)
                    query = query.Where(x => x.MaxCapacity >= finder.MaxCapacity.Value);
            }

            var result = await query.ToListAsync().ConfigureAwait(false);

            if (result.Count == 0)
                return ("could not find objects matching the request", null);

            return (string.Empty, result);

        }

    }
}
