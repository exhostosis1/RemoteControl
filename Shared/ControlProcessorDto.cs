namespace Shared;

public abstract class ControlProcessorDto
{
    public string Name { get; set; } = string.Empty;

    public bool Running { get; set; }
}