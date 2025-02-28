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
        public DbSet<Commerce> Commerces { get; set; }
        public DbSet<CommerceCallback> CommerceCallbacks { get; set; }

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
                entity.HasKey(c => c.Id);
                entity.HasIndex(c => c.ClientId).IsUnique();

                entity.HasOne(c => c.User)
                      .WithOne(u => u.Client)
                      .HasForeignKey<Client>(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<Seller>(entity =>
            {
                entity.ToTable(name: "Sellers", schema: "identity");
                entity.HasIndex(m => m.Id).IsUnique();

                entity.HasOne(s => s.User)
                    .WithMany(u => u.Sellers)
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Commerce>(entity =>
            {
                entity.ToTable(name: "Commerces", schema: "identity");
                entity.HasIndex(c => c.Id).IsUnique();

                entity.HasOne(c => c.Seller)
                    .WithMany()
                    .HasForeignKey(c => c.SellerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Callback)
                    .WithOne(cb => cb.Commerce)
                    .HasForeignKey<CommerceCallback>(cb => cb.CommerceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<CommerceCallback>(entity =>
            {
                entity.ToTable(name: "CommerceCallbacks", schema: "identity");
                entity.HasIndex(c => c.Id).IsUnique();
            });
        }
    }
}