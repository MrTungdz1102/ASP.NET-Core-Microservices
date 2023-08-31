using Microsoft.EntityFrameworkCore;
using Services.TopicSubscriptionAPI.Models;

namespace Services.TopicSubscriptionAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Topic> Topics { get; set; }
    }
}
