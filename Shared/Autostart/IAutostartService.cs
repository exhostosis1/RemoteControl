namespace Shared.AutoStart;

public interface IAutoStartService
{
    public bool CheckAutoStart();
    public void SetAutoStart(bool value);
}