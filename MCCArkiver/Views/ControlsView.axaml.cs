using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MCCArkiver.ViewModels;

namespace MCCArkiver.Views;

public partial class ControlsView : UserControl
{
    public ControlsView()
    {
        InitializeComponent();
        DataContext = new ControlsViewModel();
    }
}