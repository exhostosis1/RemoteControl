namespace MainApp.Servers.DataObjects;

internal enum RequestStatus
{
    Ok,
    Text,
    Json,
    NotFound,
    Error,
    File
}