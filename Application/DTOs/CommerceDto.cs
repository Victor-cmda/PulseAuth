namespace Application.DTOs
{
    public class CommerceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public string CreatedAt { get; set; }
        public CommerceCallbackDto Callbacks { get; set; }
    }

    public class CommerceCallbackDto
    {
        public string Credit { get; set; }
        public string Debit { get; set; }
        public string Boleto { get; set; }
        public string Webhook { get; set; }
        public string SecurityKey { get; set; }
    }

    public class CommerceCreateDto
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class CommerceUpdateDto
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class CommerceCallbackUpdateDto
    {
        public string Credit { get; set; }
        public string Debit { get; set; }
        public string Boleto { get; set; }
        public string Webhook { get; set; }
        public string SecurityKey { get; set; }
    }
}
