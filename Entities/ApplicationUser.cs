using Microsoft.AspNetCore.Identity;

namespace ASPIdentityApp.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Email { get; set; }  // New field
    }
}
