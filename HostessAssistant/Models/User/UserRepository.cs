using ElinaTestProject.Interfaces.User;
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

namespace ElinaTestProject.Models.User
{
    public partial class UserRepository : IUserRepository
    {
        private readonly string _objectName = nameof(UserRepository);

        private readonly ILogger _logger;
        private readonly TestDbContext _context;

        public UserRepository(TestDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger(_objectName);
        }

        public async Task<IActionResult> GetUserAsync(int id)
        {
            _logger.LogInformation($"Try to get user with id: {id}");

            try
            {
                var (msg, usr) = await GetUserFromDbAsync(id).ConfigureAwait(false);

                if (usr == null)
                    return new BadRequestObjectResult(msg);

                return new OkObjectResult(usr);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to get user with id: {id}");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> FindUserAsync(UserFinder finder)
        {
            _logger.LogInformation($"Try to find users");

            try
            {
                var (msg, usrs) = await FindUsersFromDb(finder).ConfigureAwait(false);

                if (usrs == null || usrs.Count == 0)
                    return new BadRequestObjectResult(msg);

                return new OkObjectResult(usrs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to find users");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> AddUserAsync(User_dto user)
        {
            _logger.LogInformation($"Try to add new user with name: {user.Name}");

            try
            {
                return new OkObjectResult(await AddNewUserAsync(user).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to add new user with name: {user.Name}");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }
        public async Task<IActionResult> UpdateUserAsync(User_dto user)
        {
            _logger.LogInformation($"Try to update existing user with id: {user.Id}");

            try
            {
                var (msg, usr) = await UpdateExistingUserAsync(user).ConfigureAwait(false);

                if (usr == null)
                {
                    return new BadRequestObjectResult(msg);
                }

                return new OkObjectResult(usr);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to update existing user with id: {user.Id}");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            _logger.LogInformation($"Try to remove existing user with id: {id}");

            try
            {
                var res = await SetUserAsInActiveAsync(id).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(res))
                {
                    return new BadRequestObjectResult(res);
                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to remove existing user with id: {id}");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        Task<(string, UserDb?)> IUserRepository.GetUserById(int id, TestDbContext dbContext) => GetUserByIdAsync(id, dbContext);

        private async Task<(string, UserDb?)> GetUserByIdAsync(int id, TestDbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = _context;

            var user = await dbContext.UserDbs
               .Where(x => x.UserId == id && x.UserStatusId == (int)UserStatusTypeEnum.Active)
               .FirstOrDefaultAsync()
               .ConfigureAwait(false);

            if (user == null)
                return ($"User with id: {id} not found", null);

            return (string.Empty, user);
        }

        private async Task<(string, User_dto?)> GetUserFromDbAsync(int id)
        {
            var user = await _context.UserDbs
                .Where(x => x.UserId == id && x.UserStatusId == (int)UserStatusTypeEnum.Active)
                .AsNoTracking()
                .Select(x => new User_dto
                {
                    Id = x.UserId,
                    UserTypeID = x.UserTypeId,
                    Name = x.UserName,
                    Surname = x.UserSurname,
                    DateOfBirth = x.DateOfBirth,
                    Phone = x.Phone,
                    TelegramID = x.TelegramId,
                    Email = x.Email,
                    Password = x.PasswordHash

                }).FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (user == null)
                return ($"User with id: {id} not found", null);

            return (string.Empty, user);
        }

        private async Task<(string, List<User_dto>?)> FindUsersFromDb(UserFinder finder)
        {
            var query = _context.UserDbs
                .Where(x => x.UserStatusId == (int)UserStatusTypeEnum.Active)
                .AsNoTracking()
                .Select(x => new User_dto
                {
                    Id = x.UserId,
                    UserTypeID = x.UserTypeId,
                    Name = x.UserName,
                    Surname = x.UserSurname,
                    DateOfBirth = x.DateOfBirth,
                    Phone = x.Phone,
                    TelegramID = x.TelegramId,
                    Email = x.Email,
                    Password = x.PasswordHash
                });

            if (finder != null)
            {
                if (finder.UserTypeID.HasValue)
                    query = query.Where(x => x.UserTypeID == finder.UserTypeID.Value);

                if (string.IsNullOrEmpty(finder.UserName))
                    query = query.Where(x => x.Name == finder.UserName);

                if (string.IsNullOrEmpty(finder.UserSurname))
                    query = query.Where(x => x.Surname == finder.UserSurname);
            }

            var result = await query.ToListAsync().ConfigureAwait(false);

            if (result.Count == 0)
                return ("could not find objects matching the request", null);

            return (string.Empty, result);
        }

        private async Task<User_dto> AddNewUserAsync(User_dto user)
        {
            var db_user = new UserDb
            {
                UserTypeId = user.UserTypeID,
                UserName = user.Name,
                UserSurname = user.Surname,
                DateOfBirth = user.DateOfBirth,
                UserStatusId = (int)UserStatusTypeEnum.Active,
                Phone = user.Phone,
                TelegramId = user.TelegramID,
                Email = user.Email,
                ModifiedDate = DateTime.UtcNow,
                PasswordHash = user.Password
            };

            await _context.UserDbs.AddAsync(db_user);
            await _context.SaveChangesAsync();

            user.Id = db_user.UserId;

            return user;
        }

        private async Task<(string, User_dto?)> UpdateExistingUserAsync(User_dto user)
        {
            var db_user = await _context.UserDbs
                .Where(x => x.UserStatusId == (int)UserStatusTypeEnum.Active && x.UserId == user.Id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (db_user == null)
                return ($"User with id: {user.Id} not found", null);

            db_user.UserName = user.Name;
            db_user.UserSurname = user.Surname;
            db_user.TelegramId = user.TelegramID;
            db_user.Email = user.Email;
            db_user.Phone = user.Phone;
            db_user.ModifiedDate = DateTime.Now;
            db_user.UserTypeId = user.UserTypeID;
            db_user.PasswordHash = user.Password;

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return (string.Empty, user);

        }

        private async Task<string> SetUserAsInActiveAsync(int id)
        {
            var db_user = await _context.UserDbs
                .Where(x => x.UserStatusId == (int)UserStatusTypeEnum.Active && x.UserId == id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (db_user == null)
                return $"User with id: {id} not found";

            db_user.UserStatusId = (int)UserStatusTypeEnum.InActive;
            db_user.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return string.Empty;
        }
    }
}
