using PostGreContext.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ElinaTestProject.Models.User
{
    public partial class UserRepository
    {
        public class User_dto
        {
            public int? Id { get; set; }
            [Required(ErrorMessage = "User type not set")]
            public int UserTypeID { get; set; }
            public string UsetTypeName => Enum.GetName(typeof(UserTypeEnum), UserTypeID);
            [Required(ErrorMessage = "User name not set")]
            public string Name { get; set; }
            [Required(ErrorMessage = "User surname not set")]
            public string Surname { get; set; }
            [Required(ErrorMessage = "User date of birth not set")]
            public DateTime DateOfBirth { get; set; }
            [Required(ErrorMessage = "User phone number not set")]
            public string Phone { get; set; }
            public int? TelegramID { get; set; }
            public string Email { get; set; }
        }
    }
}
