
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SampleIdentity.Core.Entities;
using SampleIdentity.Core.Entities.ApplicationUserAggregate;
using SampleIdentity.Infrastructure.Data.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Infrastructure.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
             : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ApplicationUserConfigurations());

            base.OnModelCreating(modelBuilder);
        }

        #region Tables Members

       
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        #endregion

        #region View Members



        #endregion
    }
}
