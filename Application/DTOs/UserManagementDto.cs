namespace Application.DTOs
{
    public class UserManagementDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public List<string> Roles { get; set; }
        public bool IsLocked { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }

    public class RoleUpdateDto
    {
        public List<string> Roles { get; set; }
    }
}
