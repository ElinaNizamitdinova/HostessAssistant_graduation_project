using System.ComponentModel.DataAnnotations;

namespace ElinaTestProject.Models.Admin
{
    public partial class AdminRepository
    {

        /// <summary>
        /// Login Request Item
        /// </summary>
        public class LoginRequestItem
        {
            /// <summary>
            /// Login
            /// </summary>
            [Required]
            public string Login { get; set; }
            /// <summary>
            /// Password
            /// </summary>
            [Required]
            public string Password { get; set; }
        }

    }
}