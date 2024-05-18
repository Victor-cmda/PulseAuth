using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Client
{
    [Key]
    public string ClientId { get; set; }

    [Required]
    public string ClientSecret { get; set; }

    [ForeignKey("UserId")]
    public string UserId { get; set; }

    public User User { get; set; }
}