using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, ILogger logger)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<AppDbContext>();
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                logger.LogInformation("Applying migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully.");

                logger.LogInformation("Checking roles...");
                await EnsureRolesCreatedAsync(roleManager, logger);

                logger.LogInformation("Checking admin user...");
                await EnsureAdminUserCreatedAsync(userManager, logger);

                logger.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database.");
            }
        }

        private static async Task EnsureRolesCreatedAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    logger.LogInformation($"Creating role: {roleName}");
                    var role = new IdentityRole(roleName);
                    var result = await roleManager.CreateAsync(role);

                    if (result.Succeeded)
                    {
                        logger.LogInformation($"Role {roleName} created successfully.");
                    }
                    else
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        logger.LogWarning($"Failed to create role {roleName}. Errors: {errors}");
                    }
                }
                else
                {
                    logger.LogInformation($"Role {roleName} already exists.");
                }
            }
        }

        private static async Task EnsureAdminUserCreatedAsync(UserManager<User> userManager, ILogger logger)
        {
            const string adminEmail = "admin@pulseauth.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                logger.LogInformation($"Admin user with email {adminEmail} not found. Creating...");

                // Criar o admin inicial
                var clientId = Guid.NewGuid();
                var clientSecret = Guid.NewGuid().ToString();

                var admin = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    Name = "System Administrator",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    DocumentType = "CPF",
                    Document = "00000000000", // Placeholder CPF
                    Client = new Client
                    {
                        Id = Guid.NewGuid(),
                        ClientId = clientId.ToString(),
                        ClientSecret = clientSecret,
                        ApiEndpoint = "https://api.pulsepay.com.br/v1"
                    }
                };

                var result = await userManager.CreateAsync(admin, "Admin@123456");
                if (result.Succeeded)
                {
                    logger.LogInformation("Admin user created successfully.");

                    // Adicionar admin à role Admin
                    var roleResult = await userManager.AddToRoleAsync(admin, "Admin");
                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation("Admin role assigned to admin user.");
                    }
                    else
                    {
                        var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        logger.LogWarning($"Failed to assign Admin role. Errors: {errors}");
                    }

                    logger.LogInformation("=== ADMIN CREDENTIALS ===");
                    logger.LogInformation($"Login: {adminEmail}");
                    logger.LogInformation($"Password: Admin@123456");
                    logger.LogInformation($"ClientId: {clientId}");
                    logger.LogInformation($"ClientSecret: {clientSecret}");
                    logger.LogInformation("========================");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    logger.LogWarning($"Failed to create admin user. Errors: {errors}");
                }
            }
            else
            {
                logger.LogInformation("Admin user already exists.");

                // Verificar se o admin tem a role Admin
                var isInAdminRole = await userManager.IsInRoleAsync(adminUser, "Admin");
                if (!isInAdminRole)
                {
                    logger.LogInformation("Admin user does not have Admin role. Assigning...");
                    var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation("Admin role assigned to existing admin user.");
                    }
                    else
                    {
                        var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        logger.LogWarning($"Failed to assign Admin role. Errors: {errors}");
                    }
                }
            }
        }
    }
}
