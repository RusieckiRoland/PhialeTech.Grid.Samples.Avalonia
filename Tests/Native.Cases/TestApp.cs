using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Markup.Xaml;
[assembly: AvaloniaTestApplication(typeof(PhialeSamples.NativeTests.Bootstrap))]
namespace PhialeSamples.NativeTests;

public static class Bootstrap
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<TestApp>().UseSkia().UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false });
}
public partial class TestApp : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
}
