using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Callback> Callbacks { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("identity");

            ConfigureUserEntity(builder);
            ConfigureIdentityTables(builder);
            ConfigureClientEntity(builder);
            ConfigureSellerEntity(builder);
            ConfigureCallbackEntity(builder);
        }

        private void ConfigureUserEntity(ModelBuilder builder)
        {
            builder.Entity<User>(entity =>
            {
                entity.ToTable(name: "Users", schema: "identity");
                entity.HasOne(u => u.Client)
                    .WithOne(c => c.User)
                    .HasForeignKey<Client>(c => c.UserId);
                entity.HasOne(u => u.Callback)
                    .WithOne(c => c.User)
                    .HasForeignKey<Callback>(c => c.UserId);
            });
        }

        private void ConfigureIdentityTables(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>(entity => { entity.ToTable(name: "Roles", schema: "identity"); });
            builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles", "identity"); });
            builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims", "identity"); });
            builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogins", "identity"); });
            builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaims", "identity"); });
            builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserTokens", "identity"); });
        }

        private void ConfigureClientEntity(ModelBuilder builder)
        {
            builder.Entity<Client>(entity =>
            {
                entity.ToTable(name: "Clients", schema: "identity");
                entity.HasIndex(c => c.UserId).IsUnique();
            });
        }

        private void ConfigureSellerEntity(ModelBuilder builder)
        {
            builder.Entity<Seller>(entity =>
            {
                entity.ToTable(name: "Sellers", schema: "identity");
                entity.HasIndex(m => m.Id).IsUnique();

                entity.HasOne(s => s.User)
                    .WithMany(u => u.Sellers)
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureCallbackEntity(ModelBuilder builder)
        {
            builder.Entity<Callback>(entity =>
            {
                entity.ToTable(name: "Callbacks", schema: "identity");
                entity.HasIndex(m => m.UserId).IsUnique();
            });
        }
    }
}
