namespace Application.DTOs
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Document { get; set; }
        public string DocumentType { get; set; }
        public string PhoneNumber { get; set; }
        public string? Nationality { get; set; }
        public bool IsForeigner { get; set; } = false;
    }
}
