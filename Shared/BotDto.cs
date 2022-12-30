namespace Shared;

public class BotDto : ControlProcessorDto
{
    public string BotUsernames { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty; 
    public string ApiKey { get; set; } = string.Empty;
}