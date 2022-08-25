namespace Shared
{
    public class ViewModel
    {
        public string? Uri { get; set; }
        public bool IsListening { get; set; }
        public bool Autostart { get; set; }
        public ViewModel(string? uri, bool isListening, bool autostart)
        {
            Uri = uri;
            IsListening = isListening;
            Autostart = autostart;
        }
    }
}
