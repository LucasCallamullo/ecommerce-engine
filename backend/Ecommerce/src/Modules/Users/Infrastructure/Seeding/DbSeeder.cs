namespace Ecommerce.Users.Infrastructure.Seeding;

using Microsoft.EntityFrameworkCore;

using Ecommerce.Shared.Auth.Enums;
using Ecommerce.Shared.Auth.Interfaces;
using Ecommerce.Shared.Common.Extensions;
using Ecommerce.Shared.Database;
using Ecommerce.Users.Domain.Entities;


/// <summary>
/// Handles initial data seeding for system roles, test users, and user-role associations
/// in local development and testing environments.
/// </summary>
public class DbSeeder(IPasswordHasher passwordHasher) : IDbSeeder
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    /// <summary>
    /// Implements the <see cref="IDbSeeder"/> interface contract for automatic dependency injection discovery.
    /// </summary>
    /// <param name="dbContext">The application database context instance.</param>
    public async Task SeedAsync(AppDbContext dbContext)
    {
        await SeedUsersAndRolesAsync(dbContext);
    }

    /// <summary>
    /// Executes the complete seeding pipeline for users, roles, and their mapping table.
    /// Ensures system roles exist before attempting to create users.
    /// </summary>
    /// <param name="dbContext">The database context used to query and persist entities.</param>
    public async Task SeedUsersAndRolesAsync(AppDbContext dbContext)
    {
        // Step 1: Ensure all roles defined in the enum exist in the database and retrieve them as a dictionary.
        var rolesMap = await EnsureAndGetRolesAsync(dbContext);

        // Step 2: Check if users are already seeded to preserve idempotency.
        if (await dbContext.Set<User>().AnyAsync()) return;

        // Step 3: Insert initial development users and persist them to generate primary key identifiers.
        var users = await SeedUsersAsync(dbContext);

        // Step 4: Map seeded users to their corresponding roles in the join table using the tracked roles.
        await SeedUserRolesAsync(dbContext, users, rolesMap);
    }

    /// <summary>
    /// Ensures all predefined roles in <see cref="UserRoleEnum"/> exist in the database, 
    /// inserts any missing roles, and returns a dictionary mapping the enum to tracked entities.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <returns>A dictionary containing all system roles mapped by their enum key.</returns>
    private async Task<Dictionary<UserRoleEnum, Role>> EnsureAndGetRolesAsync(AppDbContext dbContext)
    {
        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Step 1: Construct target roles dynamically from the enum.
        var predefinedRoles = Enum.GetValues<UserRoleEnum>()
            .ToDictionary(
                roleEnum => roleEnum,
                roleEnum => new Role
                {
                    Id = (int)roleEnum,
                    Name = roleEnum.ToString(),
                    Description = roleEnum.GetDescription(),
                    CreatedAt = seedDate
                });

        // Step 2: Fetch existing roles to compare against the enum definition.
        var existingRoles = await dbContext.Set<Role>().ToListAsync();
        var existingRoleIds = existingRoles.Select(r => r.Id).ToHashSet();

        // Step 3: Filter out roles that are already present in the database.
        var missingRoles = predefinedRoles.Values
            .Where(role => !existingRoleIds.Contains(role.Id))
            .ToList();

        // Step 4: Persist missing roles if any new enum values were detected.
        if (missingRoles.Count > 0)
        {
            // Adds the list to memory (synchronous)
            dbContext.Set<Role>().AddRange(missingRoles); 

            // Impacts the entire list in the database at once (asynchronous)
            await dbContext.SaveChangesAsync();
            existingRoles.AddRange(missingRoles);
        }

        // Step 5: Return tracked entities mapped by UserRoleEnum.
        return existingRoles.ToDictionary(r => (UserRoleEnum)r.Id, r => r);
    }
    
    /// <summary>
    /// Inserts a set of default test accounts representing distinct system actor personas into the data store.
    /// Password hashes are computed dynamically at runtime using the registered <see cref="IPasswordHasher"/> service.
    /// </summary>
    /// <param name="dbContext">The database context used to track and persist entities.</param>
    /// <returns>A dictionary mapping persona identifiers to their corresponding persisted <see cref="User"/> instances with generated keys.</returns>
    private async Task<Dictionary<string, User>> SeedUsersAsync(AppDbContext dbContext)
    {
        // Step 1: Compute the baseline development password hash dynamically via the injected password hasher service.
        var defaultPasswordHash = GetDefaultPasswordHash();

        // Step 2: Define initial test accounts representing core domain personas.
        var usersMap = new Dictionary<string, User>
        {
            ["admin"] = new User
            {
                Email = "admin@example.com",
                PasswordHash = defaultPasswordHash,
                FirstName = "Admin",
                LastName = "System",
                Cellphone = "+543510000001",
                Dni = "11111111"
            },
            ["customer"] = new User
            {
                Email = "user@example.com",
                PasswordHash = defaultPasswordHash,
                FirstName = "Standard",
                LastName = "Customer",
                Cellphone = "+543510000002",
                Dni = "22222222"
            },
            ["seller"] = new User
            {
                Email = "seller@example.com",
                PasswordHash = defaultPasswordHash,
                FirstName = "Store",
                LastName = "Seller",
                Cellphone = "+543510000003",
                Dni = "33333333"
            },
            ["support"] = new User
            {
                Email = "support@example.com",
                PasswordHash = defaultPasswordHash,
                FirstName = "Agent",
                LastName = "Support",
                Cellphone = "+543510000004",
                Dni = "44444444"
            },
            ["developer"] = new User
            {
                Email = "lucas.dev@example.com",
                PasswordHash = defaultPasswordHash,
                FirstName = "Lucas",
                LastName = "Callamullo",
                Cellphone = "+543511234567",
                Dni = "40123456"
            }
        };

        // Step 3: Queue all entities in the Change Tracker for insertion.
        await dbContext.Set<User>().AddRangeAsync(usersMap.Values);

        // Step 4: Persist changes to flush records and generate auto-incremented primary key identifiers.
        await dbContext.SaveChangesAsync();

        return usersMap;
    }

    /// <summary>
    /// Generates a standardized cryptographic password hash for development and test user credentials.
    /// </summary>
    /// <returns>A salted cryptographic password hash string generated by the injected <see cref="IPasswordHasher"/>.</returns>
    private string GetDefaultPasswordHash()
    {
        // Generates the hash dynamically using the shared abstraction, guaranteeing algorithm consistency across modules.
        return _passwordHasher.HashPassword("1234");
    }

    /// <summary>
    /// Links created users to their respective system roles in the <see cref="UserRole"/> join table.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="users">Dictionary of persisted users with generated primary keys.</param>
    /// <param name="roles">Dictionary of persisted system roles.</param>
    private async Task SeedUserRolesAsync(
        AppDbContext dbContext,
        IReadOnlyDictionary<string, User> users,
        IReadOnlyDictionary<UserRoleEnum, Role> roles)
    {
        // Step 1: Instantiate user-role junction records associating user primary keys with role IDs.
        var userRoles = new List<UserRole>
        {
            new UserRole { UserId = users["admin"].Id, RoleId = roles[UserRoleEnum.Admin].Id },
            new UserRole { UserId = users["customer"].Id, RoleId = roles[UserRoleEnum.Customer].Id },
            new UserRole { UserId = users["seller"].Id, RoleId = roles[UserRoleEnum.Seller].Id },
            new UserRole { UserId = users["support"].Id, RoleId = roles[UserRoleEnum.Support].Id },
            new UserRole { UserId = users["developer"].Id, RoleId = roles[UserRoleEnum.Admin].Id },
            new UserRole { UserId = users["developer"].Id, RoleId = roles[UserRoleEnum.Customer].Id }
        };

        // Step 2: Insert junction records into the DbSet.
        await dbContext.Set<UserRole>().AddRangeAsync(userRoles);

        // Step 3: Commit user-role assignments to the database.
        await dbContext.SaveChangesAsync();
    }
}