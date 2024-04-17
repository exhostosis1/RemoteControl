using MainApp;

namespace WinUI;

internal static class ApphostProvider
{
    private static AppHost? _app = null;

    public static AppHost AppHost => _app ??= new AppHostBuilder().Build();
}