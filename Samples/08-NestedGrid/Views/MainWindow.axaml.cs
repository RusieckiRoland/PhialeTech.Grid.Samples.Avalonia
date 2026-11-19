using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using PhialeTech.Grid.Samples.Avalonia.NestedGrid.Composition;
using PhialeTech.Grid.Samples.Avalonia.NestedGrid.Configuration;
using PhialeTech.Grid.Samples.Avalonia.NestedGrid.ViewModels;
using GridControl = PhialeGrid.Avalonia.Controls.PhialeGrid;
namespace PhialeTech.Grid.Samples.Avalonia.NestedGrid.Views;

public partial class MainWindow : Window
{
    public MainWindow(LocationsViewModel model, LocationGridComposition source)
    {
        AvaloniaXamlLoader.Load(this);
        LocationsGrid = this.FindControl<GridControl>("LocationsGrid")!;
        LocationsGrid.BindRuntime(model.Grid.Runtime);
        LocationsGrid.ConfigureColumns(model.Columns);
        LocationsGrid.Api.Workspace.SetCapabilityPolicy(new NestedExamplePolicy());
        LocationsGrid.Api.Projection.IsPagingEnabled = false;
        LocationsGrid.Api.Projection.EnableDataVirtualization = false;
        DataContext = model;
        LocationsGrid.BindDetailGrid(AssetRelation.Create(source, action => Dispatcher.UIThread.Post(action)), child =>
        {
            // A real child Grid borrows its own session. The shared toolbar routes to its runtime.
            child.Api.Workspace.SetCapabilityPolicy(new NestedExamplePolicy());
            child.Api.Projection.IsPagingEnabled = false;
            child.LanguageCode = "pl";
            child.HideUnavailableMenuItems = true;
            // Every newly created child view needs its first projection query.
            // The retained data session is reused after collapse, without reading SQLite again.
            child.AttachedToVisualTree += async (_, _) =>
            {
                if (!child.Api.Data.Current.HasConfirmedQuery)
                    await child.LoadDataAsync(0, 100);
            };
        });
        Closing += (_, e) =>
        {
            if (!LocationsGrid.CanCloseRuntime || model.Grid.IsEditing || source.HasActiveOperations || source.HasPendingChanges)
            {
                e.Cancel = true;
                this.FindControl<TextBlock>("CloseNotice")!.IsVisible = true;
            }
        };
        Opened += async (_, _) => await LocationsGrid.LoadDataAsync(0, 100);
        Closed += (_, _) =>
        {
            LocationsGrid.CloseRuntime();
            model.Dispose();
        };
    }
}
