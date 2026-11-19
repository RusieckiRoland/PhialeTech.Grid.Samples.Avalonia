using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PhialeGrid.Core;
using PhialeTech.Grid.Samples.Avalonia.Common;
using System.IO;
using PhialeTech.Grid.Samples.Avalonia.HttpRest.Composition;
using PhialeTech.Grid.Samples.Avalonia.HttpRest.ViewModels;
using PhialeTech.Grid.Samples.Avalonia.HttpRest.Views;
namespace PhialeTech.Grid.Samples.Avalonia.HttpRest;

// The application composes the Grid Core; the native control receives its context.
public partial class App : SampleApplication
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
    protected override Task<Window> CreateWindowAsync()
    {
        var source = new StockGridComposition();
        Composition = source;
        var api = GridApiFactory.CreateRuntime();
        return Task.FromResult<Window>(new MainWindow(api, new StocksViewModel(source.CreateBinding, api)));
    }
}
