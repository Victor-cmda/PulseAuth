namespace Domain.Entities
{
    public class Commerce
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid SellerId { get; set; }
        public Seller Seller { get; set; }

        public CommerceCallback Callback { get; set; }
    }

    public class CommerceCallback
    {
        public Guid Id { get; set; }
        public string Credit { get; set; }
        public string Debit { get; set; }
        public string Boleto { get; set; }
        public string Webhook { get; set; }
        public string SecurityKey { get; set; }

        public Guid CommerceId { get; set; }
        public Commerce Commerce { get; set; }
    }
}
