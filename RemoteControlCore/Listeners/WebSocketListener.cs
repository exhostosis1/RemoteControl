using RemoteControlCore.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RemoteControlCore.Listeners
{
    class WebSocketListener: IApiListener
    {
        private string _ip;
        private int _port;
        private TcpListener _server;

        public bool Listening { get; private set; }

        private async void DoJob()
        {
            var client = await _server.AcceptTcpClientAsync();
            var stream = client.GetStream();

            var unused = Task.Run(DoJob);

            var bytes = new byte[1024];
            while (true)
            {
                var count = await stream.ReadAsync(bytes, 0, bytes.Length); // match against "get"

                var s = Encoding.UTF8.GetString(bytes, 0, count);

                if (s.ToLower().StartsWith("get"))
                {
                    // 1. Obtain the value of the "Sec-WebSocket-Key" request header without any leading or trailing whitespace
                    // 2. Concatenate it with "258EAFA5-E914-47DA-95CA-C5AB0DC85B11" (a special GUID specified by RFC 6455)
                    // 3. Compute SHA-1 and Base64 hash of the new value
                    // 4. Write the hash back as the value of "Sec-WebSocket-Accept" response header in an HTTP response
                    var swk = Regex.Match(s, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();
                    var swka = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                    var swkaSha1 = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swka));
                    var swkaSha1Base64 = Convert.ToBase64String(swkaSha1);

                    // HTTP/1.1 defines the sequence CR LF as the end-of-line marker
                    var response = Encoding.UTF8.GetBytes(
                        "HTTP/1.1 101 Switching Protocols\r\n" +
                        "Connection: Upgrade\r\n" +
                        "Upgrade: websocket\r\n" +
                        "Sec-WebSocket-Accept: " + swkaSha1Base64 + "\r\n\r\n");

                    stream.Write(response, 0, response.Length);
                }
                else
                {
                    bool unused1 = (bytes[0] & 0b10000000) != 0,
                        mask = (bytes[1] & 0b10000000) !=
                               0; // must be true, "All messages from the client to the server have this bit set"

                    int opcode = bytes[0] & 0b00001111, // expecting 1 - text message
                        msglen = bytes[1] - 128, // & 0111 1111
                        offset = 2;

                    if (opcode == 8) return;

                    if (msglen == 126)
                    {
                        // was ToUInt16(bytes, offset) but the result is incorrect
                        msglen = BitConverter.ToUInt16(new[] {bytes[3], bytes[2]}, 0);
                        offset = 4;
                    }
                    else if (msglen == 127)
                    {
                        Console.WriteLine("TODO: msglen == 127, needs qword to store msglen");
                        // i don't really know the byte order, please edit this
                        // msglen = BitConverter.ToUInt64(new byte[] { bytes[5], bytes[4], bytes[3], bytes[2], bytes[9], bytes[8], bytes[7], bytes[6] }, 0);
                        // offset = 10;
                    }

                    if (mask)
                    {
                        var decoded = new byte[msglen];
                        var masks = new[]
                            {bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3]};
                        offset += 4;

                        for (var i = 0; i < msglen; ++i)
                            decoded[i] = (byte) (bytes[offset + i] ^ masks[i % 4]);

                        var text = Encoding.UTF8.GetString(decoded);

                        OnApiRequest?.Invoke(text, stream);
                    }
                }
            }
        }

        public void StartListen(UriBuilder ub)
        {
            _ip = ub.Host;
            _port = ub.Port;

            StartListen();
        }

        public void StartListen()
        {
            _server = new TcpListener(IPAddress.Parse(_ip), ++_port);

            _server.Start();

            Task.Run(DoJob);

            Listening = true;
        }

        public void StopListen()
        {
            _server.Stop();
            Listening = false;
        }

        public void RestartListen()
        {
            StopListen();
            StartListen();
        }

        public void RestartListen(UriBuilder ub)
        {
            _ip = ub.Host;
            _port = ub.Port;

            RestartListen();
        }

        public event ApiEventHandler OnApiRequest;
    }
}