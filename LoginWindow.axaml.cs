using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace SmartHouseUI;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }
    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void OnLoginClick(object sender, RoutedEventArgs e)
{
    var nextWindow = new MainWindow();
    nextWindow.Show();
    this.Close();
}
}
