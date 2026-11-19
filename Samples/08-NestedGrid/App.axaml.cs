using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PhialeGrid.Core;
using PhialeTech.Grid.Samples.Avalonia.Common;
using System.IO;
using PhialeTech.Grid.Samples.Avalonia.NestedGrid.Composition;
using PhialeTech.Grid.Samples.Avalonia.NestedGrid.ViewModels;
using PhialeTech.Grid.Samples.Avalonia.NestedGrid.Views;
using PhialeTech.Grid.Samples.Avalonia.NestedGrid.Infrastructure;
namespace PhialeTech.Grid.Samples.Avalonia.NestedGrid;

// The application composes the Grid Core; the native control receives its context.
public partial class App : SampleApplication
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
    protected override Task<Window> CreateWindowAsync()
    {
        var source = new LocationGridComposition(LocationRepository.OpenWorkingCopy());
        Composition = source;
        var api = GridApiFactory.CreateRuntime();
        var context = GridApiFactory.CreateContext<Models.LocationRow>(api, source.Binding);
        return Task.FromResult<Window>(new MainWindow(new LocationsViewModel(context, source.DatabasePath), source));
    }
}
