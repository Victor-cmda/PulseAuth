namespace Application.DTOs
{
    public class UpdateConfigurationDto
    {
        public ApiConfigUpdateDto ApiConfig { get; set; }

        public UpdateConfigurationDto()
        {
            ApiConfig = new ApiConfigUpdateDto();
        }
    }

    public class ApiConfigUpdateDto
    {
        public string ApiEndpoint { get; set; }
    }
}
