namespace BankBranchAPI.Models
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }

        private UserAccount() { }  
        public static UserAccount Create(string username, string password, bool isAdmin = false)
        {
            return new UserAccount
            {
                Username = username,
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(password),
                IsAdmin = isAdmin
            };
        }
        public bool VerifyPassword(string pwd) => BCrypt.Net.BCrypt.EnhancedVerify(pwd, this.Password);
    }
}
