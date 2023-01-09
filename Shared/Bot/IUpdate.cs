namespace Shared.Bot;

public interface IUpdate
{
    public int UpdateId { get; set; }
    public IMessage? Message { get; set; }
}