namespace ASPIdentityApp.Models
{
    public class AuthResponse
    {
        public int Id { get; set; }                       // PK
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
        public string? Email { get; set; }
        public string? UserId { get; set; }
    }
}
