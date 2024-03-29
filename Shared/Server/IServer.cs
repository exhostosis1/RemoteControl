using System.ComponentModel;
using Shared.Config;

namespace Shared.Server;

public interface IServer: INotifyPropertyChanged
{
    public int Id { get; set; }
    public bool Status { get; }
    public CommonConfig Config { get; set; }
    public void Start(CommonConfig? config = null);
    public void Restart(CommonConfig? config = null);
    public void Stop();
}