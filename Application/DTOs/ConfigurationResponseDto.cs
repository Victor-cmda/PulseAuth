namespace Application.DTOs
{
    public class ConfigurationResponseDto
    {
        public ApiConfigDto ApiConfig { get; set; }

        public ConfigurationResponseDto()
        {
            ApiConfig = new ApiConfigDto();
        }
    }

    public class ApiConfigDto
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ApiEndpoint { get; set; }
    }
}
