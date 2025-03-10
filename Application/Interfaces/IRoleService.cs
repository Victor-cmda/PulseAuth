namespace Application.Interfaces
{
    public interface IRoleService
    {
        Task<bool> AssignUserToRoleAsync(string userId, string roleName);
        Task<bool> RemoveUserFromRoleAsync(string userId, string roleName);
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<bool> IsUserInRoleAsync(string userId, string roleName);
    }
}
