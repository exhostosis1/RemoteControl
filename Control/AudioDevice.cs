using Shared.Control;

namespace Control
{
    internal class AudioDevice : IAudioDevice
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsCurrentControlDevice { get; set; }
    }
}
