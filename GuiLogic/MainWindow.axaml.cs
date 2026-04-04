using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace SmartHouseUI.GuiLogic;

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

    private void LogOut(object sender, RoutedEventArgs e)
    {
        var nextWindow = new LoginWindow();
        nextWindow.Show();
        this.Close();
    }
}