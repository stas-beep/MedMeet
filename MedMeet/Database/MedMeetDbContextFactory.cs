using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Database
{
    public class MedMeetDbContextFactory : IDesignTimeDbContextFactory<MedMeetDbContext>
    {
        public MedMeetDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MedMeetDbContext>();

            var connectionString = "server=localhost;port=3306;database=medmeetdb;user=root;password=root";
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 41)); 

            optionsBuilder.UseMySql(connectionString, serverVersion);

            return new MedMeetDbContext(optionsBuilder.Options);
        }
    }
}