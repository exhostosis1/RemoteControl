using RemoteControl.App.Utility;

namespace RemoteControl.App.Interfaces.Web
{
    public interface IListener
    {
        public bool IsListening { get; }
        public IReadOnlyCollection<string> ListeningUris { get; }

        public event HttpEventHandler? OnRequest;

        public void StartListen(string url);

        public void StopListen();
    }
}
