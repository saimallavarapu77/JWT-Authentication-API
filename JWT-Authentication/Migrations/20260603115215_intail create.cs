using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using JWT_Authentication.Dbcontext;

namespace JWT_Authentication
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            // If tools run from solution folder try project folder
            if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
            {
                var possible = Path.Combine(basePath, "JWT-Authentication");
                if (Directory.Exists(possible)) basePath = possible;
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var conn = config.GetConnectionString("DefaultConnection")
                       ?? config["ConnectionStrings:DefaultConnection"]
                       ?? "Server=(localdb)\\mssqllocaldb;Database=JwtDb;Trusted_Connection=True;";

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(conn);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
