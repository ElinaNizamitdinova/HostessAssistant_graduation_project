using ElinaTestProject.Models.User;
using Microsoft.AspNetCore.Mvc;
using PostGreContext.Context;
using PostGreContext.Models;
using System.Threading.Tasks;
using static ElinaTestProject.Models.User.UserRepository;

namespace ElinaTestProject.Interfaces.User
{
    /// <summary>
    /// User repository interface
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IActionResult> GetUserAsync(int id);
        /// <summary>
        /// Find users
        /// </summary>
        /// <param name="finder"></param>
        /// <returns></returns>
        Task<IActionResult> FindUserAsync(UserFinder finder);
        /// <summary>
        /// Add new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IActionResult> AddUserAsync(User_dto user);
        /// <summary>
        /// Update existing user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IActionResult> UpdateUserAsync(User_dto user);
        /// <summary>
        /// Remove user 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IActionResult> DeleteUserAsync(int id);
        internal Task<(string, UserDb?)> GetUserById(int id, TestDbContext dbContext = null);
    }
}
