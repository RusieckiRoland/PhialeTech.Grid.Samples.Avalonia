using PhialeGrid.Core.Interaction;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System.Globalization;
using PhialeGrid.Contracts;
using PhialeGrid.Core.Regions;
using PhialeTech.Grid.Samples.Avalonia.Common;
using PhialeTech.Grid.Samples.Avalonia.LocalEditing.ViewModels;
using GridControl = PhialeGrid.Avalonia.Controls.PhialeGrid;
namespace PhialeTech.Grid.Samples.Avalonia.LocalEditing.Views;

public partial class MainWindow : Window
{
    private readonly IGridApi _api;
    private readonly AssetsViewModel _model;
    private bool _closingAfterChoice, _closeFlowRunning;
    public Func<Task<CloseChoice>>? ConfirmCloseChoice
    { get; set; }
    public MainWindow(IGridApi api, AssetsViewModel model)
    {
        _api = api;
        _model = model;
        AvaloniaXamlLoader.Load(this);
        AssetsGrid = this.FindControl<GridControl>("AssetsGrid")!;
        AssetsGrid.BindRuntime(api);
        AssetsGrid.ConfigureColumns(model.Columns);
        AssetsGrid.Culture = CultureInfo.GetCultureInfo("pl-PL");
        api.Workspace.SetCapabilityPolicy(new Configuration.EditingExamplePolicy());
        api.Projection.IsPagingEnabled = false;
        api.Projection.EnableDataVirtualization = false;
        DataContext = model;
        Opened += (_, _) => api.Workspace.Execute(new(GridRegionCommandKind.Open, GridRegionKind.ChangePanelRegion));
        Closing += (_, e) =>
        {
            if (AssetsGrid.CanCloseRuntime && (_closingAfterChoice || (!_api.Editing.HasPendingEdits && !_api.Editing.IsSurfaceEditing)))
                return;
            e.Cancel = true;
            if (_closeFlowRunning)
                return;
            _closeFlowRunning = true;
            // Defer until Closing returns, even when a save completes synchronously.
            Dispatcher.UIThread.Post(async () =>
            {
                try
                {
                    await FinishClosingAsync();
                }
                finally
                {
                    _closeFlowRunning = false;
                }
            });
        };
        Opened += async (_, _) => await AssetsGrid.LoadDataAsync(0, 100);
        Closed += (_, _) =>
        {
            model.Dispose();
            AssetsGrid.CloseRuntime();
        };
    }
    private async Task FinishClosingAsync()
    {
        while (!AssetsGrid.CanCloseRuntime)
            await Task.Delay(25);
        if (_api.Editing.HasPendingEdits || _api.Editing.IsSurfaceEditing)
        {
            var choice = ConfirmCloseChoice != null ? await ConfirmCloseChoice() : await NoticeWindow.AskAsync(this,
                "Zapisz zmiany w SQLite, odrzuć bufor albo anuluj zamknięcie. Zapis sprawdza cały rekord.");
            if (choice == CloseChoice.Cancel)
                return;
            if (choice == CloseChoice.Save)
            {
                if (_api.Editing.IsSurfaceEditing && !_api.Editing.CommitCell())
                    return;
                if (!(await _model.Grid.SaveAsync()).Succeeded)
                    return;
            }
            else
                _model.Grid.DiscardChanges();
        }
        while (!AssetsGrid.CanCloseRuntime)
            await Task.Delay(25);
        _closingAfterChoice = true;
        Close();
    }
}
