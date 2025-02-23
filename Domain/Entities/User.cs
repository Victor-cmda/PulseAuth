using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public Client Client { get; set; }
        public ICollection<Seller> Sellers { get; set; }
        public string Name { get; set; }
        public string Document { get; set; }
        public string DocumentType { get; set; }
    }
}
