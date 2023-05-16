using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AddressManager.Models;

namespace AddressManager.Data
{
    public class AddressManagerContext : DbContext
    {
        public AddressManagerContext (DbContextOptions<AddressManagerContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<AddressManager.Models.User> User { get; set; } = default!;
        public DbSet<AddressManager.Models.Company> Company { get; set; } = default!;
        public DbSet<AddressManager.Models.Worker> Worker { get; set; } = default!;
        public DbSet<AddressManager.Models.ChangeHistory> ChangeHistory { get; set; } = default!;

       
    }
}
