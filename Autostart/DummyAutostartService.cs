using Shared;

namespace Autostart
{
    public class DummyAutostartService: IAutostartService
    {
        private bool _autostart;

        public bool CheckAutostart()
        {
            return _autostart;
        }

        public void SetAutostart(bool value)
        {
            _autostart = value;
        }
    }
}
