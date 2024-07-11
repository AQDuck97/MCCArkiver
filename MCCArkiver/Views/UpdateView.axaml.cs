using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MCCArkiver.ViewModels;

namespace MCCArkiver.Views;

public partial class UpdateView : UserControl
{
    public UpdateView()
    {
        InitializeComponent();
        DataContext = new UpdateViewModel();
    }
}