using System.ComponentModel;

namespace RemoteControl.Config
{
    [DisplayName("Uri")]
    internal class UriConfig : IConfigItem
    {
        [DisplayName("host")] 
        internal string Host { get; set; } = "localhost";

        [DisplayName("scheme")]
        internal string Scheme { get; set; } = "http";

        [DisplayName("port")] 
        internal int Port { get; set; } = 1488;

        internal Uri Uri => new UriBuilder(Scheme, Host, Port).Uri;
    }
}
