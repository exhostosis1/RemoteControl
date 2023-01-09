namespace Shared.Bot;

public interface IUpdateResponse
{
    public bool Ok { get; set; }
    public IUpdate[] Result { get; set; }
}