using Shared.Config;
using System;

namespace Shared.ControlProcessor;

public interface IControlProcessor: IObservable<bool>
{
    public int Id { get; set; }
    public event ConfigEventHandler? ConfigChanged;
    public bool Working { get; }
    public CommonConfig CurrentConfig { get; set; }
    public void Start(CommonConfig? config = null);
    public void Restart(CommonConfig? config = null);
    public void Stop();
}