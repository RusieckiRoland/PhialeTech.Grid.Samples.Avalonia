using Avalonia;
namespace PhialeTech.Grid.Samples.Avalonia.SqliteBinding;

internal static class Program
{
    [STAThread] public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();
}
