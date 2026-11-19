using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PhialeGrid.Core;
using PhialeTech.Grid.Samples.Avalonia.Common;
using System.IO;
using PhialeTech.Grid.Samples.Avalonia.RowDetails.Composition;
using PhialeTech.Grid.Samples.Avalonia.RowDetails.ViewModels;
using PhialeTech.Grid.Samples.Avalonia.RowDetails.Views;
using PhialeTech.Grid.Samples.Avalonia.RowDetails.Infrastructure;
namespace PhialeTech.Grid.Samples.Avalonia.RowDetails;

// The application composes the Grid Core; the native control receives its context.
public partial class App : SampleApplication
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
    protected override Task<Window> CreateWindowAsync()
    {
        var source = new AssetGridComposition(AssetCatalog.Read());
        Composition = source;
        var api = GridApiFactory.CreateRuntime();
        var context = GridApiFactory.CreateContext<PhialeTech.Grid.Samples.Avalonia.LocalEditing.Models.AssetRow>(api, source.Binding);
        return Task.FromResult<Window>(new MainWindow(new AssetsViewModel(context)));
    }
}
