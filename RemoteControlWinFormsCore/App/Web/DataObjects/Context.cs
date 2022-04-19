namespace RemoteControl.App.Web.DataObjects
{
    internal class Context
    {
        internal Request Request { get; set; }
        internal Response Response { get; set; } = new();

        internal Context(string path) => Request = new Request(path);
    }
}
