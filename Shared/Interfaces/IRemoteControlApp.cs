namespace Shared.Interfaces
{
    public interface IRemoteControlApp
    {
        public void Start(Uri uri);
        public void Stop();
        public string? GetUiListeningUri();
        public string? GetApiListeningUri();
        public bool IsUiListening { get; }
        public bool IsApiListening { get; }
    }
}