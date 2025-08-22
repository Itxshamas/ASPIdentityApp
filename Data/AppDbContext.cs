using ASPIdentityApp.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ASPIdentityApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AuthResponse> AuthResponses { get; set; }
        public DbSet<LoginRequest> LoginRequests { get; set; }
        public DbSet<RegisterRequest> RegisterRequests { get; set; }
    }
}
