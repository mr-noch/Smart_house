using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using SmartHouseUI.Services;
using SmartHouseUI.Models;
using Avalonia.Media;
using System.Linq;
using System.Text.RegularExpressions;

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
        var errorText = this.FindControl<TextBlock>("ErrorText");

        if (emailInput == null || passwordInput == null || errorText == null) return;

        string? email = emailInput.Text?.Trim();
        string? password = passwordInput.Text?.Trim();

        string validationError = ValidateLogin(email, password);
        if (!string.IsNullOrEmpty(validationError))
        {
            errorText.Text = validationError;
            return;
        }

        errorText.Text = "";

        if (!authService.LogIn(email, password))
        {
            errorText.Text = "Невірний email або пароль.";
            return;
        }
        
        var mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
        
    }

    private void SignUpWindowButton_Click(object sender, RoutedEventArgs e)
    {
        var nextWindow = new SignUpWindow();
        nextWindow.Show();
        this.Close();
    }

    private void OnForgotPasswordClick(object sender, RoutedEventArgs e)
    {
        var changePasswordWindow = new ChangePassword();
        changePasswordWindow.Show();
        this.Close();
    }

    private string ValidateLogin(string? email, string? password)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "Email не може бути порожнім.";

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return "Невірний формат email.";

        if (string.IsNullOrWhiteSpace(password))
            return "Пароль не може бути порожнім.";

        return "";
    }

    private string ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "Email не може бути порожнім.";

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return "Невірний формат email.";

        return "";
    }
}
