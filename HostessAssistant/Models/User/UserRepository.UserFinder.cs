namespace ElinaTestProject.Models.User
{
    public partial class UserRepository
    {
        public class UserFinder
        {
            public string UserName { get; set; }
            public int? UserTypeID { get; set; }
            public string UserSurname { get; set; }

        }
    }
}
