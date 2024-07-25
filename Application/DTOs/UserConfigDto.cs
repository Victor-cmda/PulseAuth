namespace Application.DTOs
{
    public class UserConfigDto
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public CallbackDto Callbacks { get; set; }
        public List<SellerConfigDto> Sellers { get; set; }
    }

    public class CallbackDto
    {
        public string Credit { get; set; }
        public string Debit { get; set; }
        public string Registration { get; set; }
    }

    public class SellerConfigDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SellerId { get; set; }
    }
}
