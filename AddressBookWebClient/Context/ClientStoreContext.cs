using AddressBookWebClient.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AddressBookWebClient.Context
{
    public class ClientStoreContext : IdentityDbContext<AppUser>
    {
        public ClientStoreContext(DbContextOptions<ClientStoreContext> options)
        : base(options)
        {
        }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
