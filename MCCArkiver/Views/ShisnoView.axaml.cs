using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MCCArkiver.ViewModels;

namespace MCCArkiver.Views;

public partial class ShisnoView : UserControl
{
    public ShisnoView()
    {
        InitializeComponent();
        DataContext = new ShisnoViewModel();
    }
}