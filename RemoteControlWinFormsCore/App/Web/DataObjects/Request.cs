namespace RemoteControl.App.Web.DataObjects
{
    internal class Request
    {
        internal string Path { get; set; }

        internal Request(string path) => Path = path;
    }
}
