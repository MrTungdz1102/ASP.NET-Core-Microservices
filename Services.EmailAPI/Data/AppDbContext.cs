using Microsoft.EntityFrameworkCore;
using Services.EmailAPI.Models;

namespace Services.EmailAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<EmailLogger> EmailLoggers { get; set; }
    }
}
