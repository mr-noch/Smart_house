using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using SmartHouseUI.Services;

namespace SmartHouseUI.GuiLogic;

public partial class LoginWindow : Window
{
    private UserAuthService authService = new UserAuthService();
    public LoginWindow()
    {
        InitializeComponent();
    }
    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);


    
    private void OnLoginClick(object sender, RoutedEventArgs e)
    {
        var emailInput = this.FindControl<TextBox>("EmailInput");
        var passwordInput = this.FindControl<TextBox>("PasswordInput");

        if (emailInput == null || passwordInput == null) return;

        string email = emailInput.Text;
        string password = passwordInput.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        authService.LogIn(email, password);
        var nextWindow = new MainWindow();
        nextWindow.Show();
        this.Close();
    }
    private void SignUpWindowButton_Click(object sender, RoutedEventArgs e)
    {
        var nextWindow = new SignUpWindow();
        nextWindow.Show();
        this.Close();
    }
}
