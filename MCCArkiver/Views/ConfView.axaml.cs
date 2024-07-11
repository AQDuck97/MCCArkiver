using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MCCArkiver.ViewModels;

namespace MCCArkiver.Views;

public partial class ConfView : UserControl
{
    public ConfView()
    {
        InitializeComponent();
        DataContext = new ConfViewModel();
    }
}