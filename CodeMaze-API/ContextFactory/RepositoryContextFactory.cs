using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repository;

namespace CodeMaze_API.ContextFactory
{
    // We are using the IDesignTimeDbContextFactory<out TContext>
    // interface that allows design-time services to discover implementations of this interface.
    // TContext = RepositoryContext
    public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
    {
        public RepositoryContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json")
                .Build();

            var builder = new DbContextOptionsBuilder<RepositoryContext>()
                .UseSqlServer(configuration.GetConnectionString("SQL-Server"), 
                b => b.MigrationsAssembly("CodeMaze-API"));

            return new RepositoryContext(builder.Options);
        }
    }
}
