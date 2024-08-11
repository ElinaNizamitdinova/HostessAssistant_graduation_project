using ElinaTestProject.Interfaces.User;
using ElinaTestProject.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ElinaTestProject.Models.User.UserRepository;

namespace ElinaTestProject.Controllers.User
{
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/{id:int}")]
        [ProducesResponseType(typeof(User_dto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetUser(int id)
        {
            return await _userRepository.GetUserAsync(id);
        }

        /// <summary>
        /// Add or update user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(User_dto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Apply([FromBody] User_dto user)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ControllerUtils.GetModelErrors(ModelState));

            if (user.Id.HasValue)
                return await _userRepository.UpdateUserAsync(user).ConfigureAwait(false);
            else
                return await _userRepository.AddUserAsync(user).ConfigureAwait(false);
        }

        /// <summary>
        /// Find user by params
        /// </summary>
        /// <param name="finder"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<User_dto>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Find([FromBody] UserFinder finder)
        {
            return await _userRepository.FindUserAsync(finder).ConfigureAwait(false);
        }

        [HttpDelete]
        [Route("[action]/{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Delete(int id)
        {
            return await _userRepository.DeleteUserAsync(id).ConfigureAwait(false);
        }
    }
}
