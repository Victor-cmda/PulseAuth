using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Callback
    {
        [Key]
        public Guid Id { get; set; }
        public string? Credit { get; set; }
        public string? Debit { get; set; }
        public string? Registration { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }

        public User User { get; set; }
    }
}
