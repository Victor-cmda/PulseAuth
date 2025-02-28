using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Client
{
    [Key]
    public Guid Id { get; set; }

    public string ClientId { get; set; }
    
    public string ClientSecret { get; set; }

    public string ApiEndpoint { get; set; }

    [ForeignKey("User")]
    public string UserId { get; set; }

    public User User { get; set; }
}