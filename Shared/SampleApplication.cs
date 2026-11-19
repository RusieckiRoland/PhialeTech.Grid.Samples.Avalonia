using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using GridControl = global::PhialeGrid.Avalonia.Controls.PhialeGrid;

namespace PhialeTech.Grid.Samples.Avalonia.Common;

// Shared desktop lifecycle only. Sources, sessions and persistence stay visible in each lesson.
public abstract class SampleApplication : Application
{
    protected IDisposable? Composition;
    protected abstract Task<Window> CreateWindowAsync();
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Startup += async (_, _) =>
            {
                try
                {
                    var window = await CreateWindowAsync();
                    desktop.MainWindow = window;
                    if (desktop.Args?.Contains("--smoke") == true)
                    {
                        window.Opacity = 0;
                        window.ShowActivated = false;
                        window.ShowInTaskbar = false;
                    }
                    window.Show();
                    if (desktop.Args?.Contains("--smoke") == true)
                    {
                        await Task.Delay(2000);
                        for (var attempt = 0; attempt < 400 && window.GetVisualDescendants().OfType<GridControl>()
                            .Any(grid => grid.IsRuntimeBound && grid.Api.Data.Current?.IsBusy == true); attempt++)
                            await Task.Delay(50);
                        var screenshot = Environment.GetEnvironmentVariable("PHIALE_SAMPLE_SCREENSHOT");
                        if (!string.IsNullOrEmpty(screenshot))
                        {
                            using var image = new RenderTargetBitmap(new PixelSize((int)window.Bounds.Width, (int)window.Bounds.Height));
                            image.Render(window);
                            image.Save(screenshot);
                        }
                        Console.WriteLine("PASS: " + window.Title);
                        desktop.Shutdown();
                    }
                }
                catch (Exception error)
                {
                    Console.Error.WriteLine(error);
                    if (desktop.Args?.Contains("--smoke") == true)
                    {
                        desktop.Shutdown(1);
                        return;
                    }
                    desktop.MainWindow = new NoticeWindow("Nie udało się otworzyć przykładu", error.Message);
                    desktop.MainWindow.Show();
                }
            };
            desktop.Exit += (_, _) => Composition?.Dispose();
        }
        base.OnFrameworkInitializationCompleted();
    }
}
