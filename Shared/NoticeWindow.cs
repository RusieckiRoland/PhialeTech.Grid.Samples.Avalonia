using Avalonia.Controls;
using Avalonia.Markup.Xaml;
namespace PhialeTech.Grid.Samples.Avalonia.Common;

public enum CloseChoice
{
    Cancel, Save, Discard
}
public partial class NoticeWindow : Window
{
    public string Heading
    { get; }
    public string Message
    { get; }
    public NoticeWindow(string heading, string message, bool choices = false)
    {
        Heading = heading;
        Message = message;
        AvaloniaXamlLoader.Load(this);
        DataContext = this;
        this.FindControl<Button>("SaveChoice")!.IsVisible = choices;
        this.FindControl<Button>("DiscardChoice")!.IsVisible = choices;
        this.FindControl<Button>("SaveChoice")!.Click += (_, _) => Close(CloseChoice.Save);
        this.FindControl<Button>("DiscardChoice")!.Click += (_, _) => Close(CloseChoice.Discard);
        this.FindControl<Button>("CancelChoice")!.Click += (_, _) => Close(CloseChoice.Cancel);
    }
    public static Task<CloseChoice> AskAsync(Window owner, string message) =>
        new NoticeWindow("Niezapisane zmiany", message, choices: true).ShowDialog<CloseChoice>(owner);
}
