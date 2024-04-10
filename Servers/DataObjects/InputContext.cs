namespace Servers.DataObjects;

public class InputContext
{
    #region Web

    public string Path { get; set; } = "";

    #endregion

    #region Bot

    public int Id { get; set; } = 0;
    public string Command { get; set; } = "";
    public DateTime Date { get; set; } = DateTime.UtcNow;

    #endregion
}