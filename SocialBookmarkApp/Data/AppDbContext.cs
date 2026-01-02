using Microsoft.EntityFrameworkCore;

namespace SocialBookmarkApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext>
        options)
        : base(options)
    {
    }
}