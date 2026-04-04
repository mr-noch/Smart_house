using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using SmartHouseUI.Services;

namespace SmartHouseUI.GuiLogic;

public partial class SignUpWindow : Window
{
    private UserAuthService authService = new UserAuthService();

    public SignUpWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void LoginWindowButton_Click(object sender, RoutedEventArgs e)
    {
        var nextWindow = new LoginWindow();
        nextWindow.Show();
        this.Close();
    }

    private void OnSignUpClick(object sender, RoutedEventArgs e)
    {
        var nameInput = this.FindControl<TextBox>("NameInput");
        var emailInput = this.FindControl<TextBox>("EmailInput");
        var passwordInput = this.FindControl<TextBox>("PasswordInput");

        if (nameInput == null || emailInput == null || passwordInput == null) return;

        string name = nameInput.Text;
        string email = emailInput.Text;
        string password = passwordInput.Text;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            return;

        if (authService.SignUp(name, email, password))
        {
            LoginWindowButton_Click(sender, e);
        }
    }
}