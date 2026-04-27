using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using SmartHouseUI.Services;
using System.Text.RegularExpressions;

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
        var errorText = this.FindControl<TextBlock>("ErrorText");

        if (nameInput == null || emailInput == null || passwordInput == null || errorText == null) return;

        string name = nameInput.Text?.Trim() ?? "";
        string email = emailInput.Text?.Trim() ?? "";
        string password = passwordInput.Text?.Trim() ?? "";

        // Validation
        string validationError = ValidateSignUp(name, email, password);
        if (!string.IsNullOrEmpty(validationError))
        {
            errorText.Text = validationError;
            return;
        }

        errorText.Text = "";

        if (authService.SignUp(name, email, password))
        {
            LoginWindowButton_Click(sender, e);
        }
        else
        {
            errorText.Text = "Користувач з таким email вже існує.";
        }
    }

    private string ValidateSignUp(string name, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Ім'я не може бути порожнім.";

        if (name.Length < 2 || name.Length > 50)
            return "Ім'я повинно бути від 2 до 50 символів.";

        if (!Regex.IsMatch(name, @"^[a-zA-Zа-яА-ЯіІїЇєЄ\s]+$"))
            return "Ім'я може містити тільки букви та пробіли.";

        if (string.IsNullOrWhiteSpace(email))
            return "Email не може бути порожнім.";

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return "Невірний формат email.";

        if (string.IsNullOrWhiteSpace(password))
            return "Пароль не може бути порожнім.";

        if (password.Length < 6)
            return "Пароль повинен бути не менше 6 символів.";

        return "";
    }
}