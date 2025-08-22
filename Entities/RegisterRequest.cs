namespace ASPIdentityApp.Entities
{
    public class RegisterRequest
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // will store hashed password
    }
}
