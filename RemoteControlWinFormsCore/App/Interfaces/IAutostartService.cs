namespace RemoteControl.App.Interfaces
{
    public interface IAutostartService
    {
        public bool CheckAutostart();

        public void SetAutostart(bool value);
    }
}