using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace SmartHouseUI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}