using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext
    {
        //public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("identity");

            builder.Entity<IdentityUser>(entity => { entity.ToTable(name: "Users", schema: "identity"); });
            builder.Entity<IdentityRole>(entity => { entity.ToTable(name: "Roles", schema: "identity"); });
            builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles", "identity"); });
            builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims", "identity"); });
            builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogins", "identity"); });
            builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaims", "identity"); });
            builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserTokens", "identity"); });

        }
    }
}
