using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Client> Clients { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("identity");

            builder.Entity<User>(entity => 
            { 
                entity.ToTable(name: "Users", schema: "identity");
                entity.HasOne(u => u.Client)
                    .WithOne(c => c.User)
                    .HasForeignKey<Client>(c => c.UserId);
            });

            builder.Entity<IdentityRole>(entity => { entity.ToTable(name: "Roles", schema: "identity"); });
            builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles", "identity"); });
            builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims", "identity"); });
            builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogins", "identity"); });
            builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaims", "identity"); });
            builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserTokens", "identity"); });
            
            builder.Entity<Client>(entity => 
            { 
                entity.ToTable(name: "Clients", schema: "identity");
                entity.HasIndex(c => c.ClientId).IsUnique();
            });
        }
    }
}