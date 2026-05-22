namespace JWT_Authentication.Dbcontext
{
    using JWT_Authentication.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
