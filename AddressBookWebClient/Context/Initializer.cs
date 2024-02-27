using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using AddressBookWebClient.Models;
using AddressBookWebClient.ViewData;
using Microsoft.AspNetCore.Identity;

namespace AddressBookWebClient.Context
{


    public class AddressBookContextFactory : IDesignTimeDbContextFactory<ClientStoreContext>
    {
        public ClientStoreContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var builder = new DbContextOptionsBuilder<ClientStoreContext>();
            builder.UseSqlServer(connectionString);

            return new ClientStoreContext(builder.Options);
        }
    }
    public static class Initializer
    {
        public static async Task InitializeUserAsync(this ServiceProvider provider)
        {
            var context = provider.GetRequiredService<ClientStoreContext>();

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();


            var roles = context.Roles.Any<IdentityRole>();

            if (!roles) 
            {
                var role1 = new IdentityRole { Name = "admin" };
                var role2 = new IdentityRole { Name = "user" };

                await roleManager.CreateAsync(role1);
                await roleManager.CreateAsync(role2);
            }

           
        }
    }
}
