namespace Shared
{
    public interface IAutostartService
    {
        public bool CheckAutostart();

        public void SetAutostart(bool value);
    }
}