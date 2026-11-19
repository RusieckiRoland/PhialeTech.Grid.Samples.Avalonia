using Avalonia.Controls;
using PhialeGrid.Core.Details;
using PhialeTech.Grid.Samples.Avalonia.RowDetails.Views;
namespace PhialeTech.Grid.Samples.Avalonia.RowDetails.Details;
// The descriptor stays platform-neutral; only this adapter creates the native UserControl.
public static class AssetDetailsFactory
{
    public static Control Create(GridRowDetailSurfacePayload payload) => new AssetDetailsView { DataContext = payload.ContentDescriptor };
}
