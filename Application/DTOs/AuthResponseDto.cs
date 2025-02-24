namespace Application.DTOs;

public class AuthResponseDto
{
    public string AccessToken { get; set; }
    public DateTime ExpiresIn { get; set; }
    public UserDto User{ get; set; }
}
