namespace UnitOfWorkDemo1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Store hashed passwords in production
    }
}
