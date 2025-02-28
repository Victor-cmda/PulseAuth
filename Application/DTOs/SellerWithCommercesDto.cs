namespace Application.DTOs
{
    public class SellerWithCommercesDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CommerceDto> Commerces { get; set; } = new List<CommerceDto>();
    }
}
