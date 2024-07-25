using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public Client Client { get; set; }
        public Callback Callback { get; set; }
        public ICollection<Seller> Sellers { get; set; }
    }
}
