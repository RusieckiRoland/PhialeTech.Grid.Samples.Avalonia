using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using PhialeGrid.Contracts;
using PhialeGrid.Core;
using PhialeTech.Grid.Samples.Avalonia.Common;
using PhialeTech.Grid.Samples.Avalonia.RestSnapshot.Composition;
using PhialeTech.Grid.Samples.Avalonia.RestSnapshot.ViewModels;
using GridControl = PhialeGrid.Avalonia.Controls.PhialeGrid;
namespace PhialeTech.Grid.Samples.Avalonia.RestSnapshot.Views;

public partial class MainWindow : Window
{
    private readonly PriceReviewViewModel _model;
    private bool _approvedClose, _askingClose;
    public MainWindow(IGridApi api, PriceReviewViewModel model)
    {
        _model = model;
        AvaloniaXamlLoader.Load(this);
        PricesGrid = this.FindControl<GridControl>("PricesGrid")!;
        PricesGrid.BindRuntime(api);
        PricesGrid.ConfigureColumns(model.Columns);
        api.Workspace.SetCapabilityPolicy(new Configuration.PriceReviewPolicy());
        api.Configuration.MultiSelect = true;
        api.Projection.IsPagingEnabled = true;
        api.Projection.PageSize = 100;
        api.Projection.EnableDataVirtualization = false;
        DataContext = model;
        Closing += (_, e) =>
        {
            if (model.Grid?.IsBusy == true)
            {
                e.Cancel = true;
                return;
            }
            if (_approvedClose || model.Grid?.HasPendingChanges != true)
                return;
            e.Cancel = true;
            if (_askingClose)
                return;
            _askingClose = true;
            Dispatcher.UIThread.Post(async () =>
            {
                try
                {
                    var choice = await NoticeWindow.AskAsync(this,
                        "Odrzucić niezapisane zmiany i zamknąć?");
                    if (choice == CloseChoice.Discard)
                    {
                        _approvedClose = true;
                        Close();
                    }
                }
                finally
                {
                    _askingClose = false;
                }
            });
        };
        Closed += (_, _) =>
        {
            model.Dispose();
            PricesGrid.CloseRuntime();
        };
    }
    private async void SecondOperator(object? sender, RoutedEventArgs e)
    {
        var source = new SnapshotComposition();
        var api = GridApiFactory.CreateRuntime();
        var other = new PriceReviewViewModel(api, source) { Address = _model.Address, Supplier = _model.Supplier, Category = _model.Category, ImportedFrom = _model.ImportedFrom, ImportedTo = _model.ImportedTo };
        var window = new MainWindow(api, other) { Title = "06 · Drugi operator", Position = new PixelPoint(Position.X + 30, Position.Y + 30) };
        window.Closed += (_, _) => source.Dispose();
        window.Show();
        await other.LoadAsync();
    }
    private async void DiscardAndReload(object? sender, RoutedEventArgs e)
    {
        if (await NoticeWindow.AskAsync(this, "Odrzucić cały bufor i odczytać aktualny cennik?") != CloseChoice.Discard)
            return;
        _model.Grid?.DiscardChanges();
        await _model.LoadAsync();
    }
    private async Task Policy(int value)
    {
        try
        {
            await _model.SetMarginPolicyAsync(value);
        }
        catch (Exception error) { await new NoticeWindow("Serwer niedostępny", error.Message).ShowDialog(this); }
    }
    private async void StrictPolicy(object? sender, RoutedEventArgs e) => await Policy(35);
    private async void NormalPolicy(object? sender, RoutedEventArgs e) => await Policy(15);
}
