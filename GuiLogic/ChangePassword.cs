using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using SmartHouseUI.Services;
using System.Text.RegularExpressions;

namespace SmartHouseUI.GuiLogic;

public partial class ChangePassword : Window
{
    private UserAuthService authService = new UserAuthService();

    public ChangePassword()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void OnChangePasswordClick(object sender, RoutedEventArgs e)
    {
        var emailInput = this.FindControl<TextBox>("EmailInput");
        var currentPasswordInput = this.FindControl<TextBox>("CurrentPasswordInput");
        var newPasswordInput = this.FindControl<TextBox>("NewPasswordInput");
        var confirmPasswordInput = this.FindControl<TextBox>("ConfirmPasswordInput");
        var errorText = this.FindControl<TextBlock>("ErrorText");
        var successText = this.FindControl<TextBlock>("SuccessText");

        if (emailInput == null || currentPasswordInput == null || newPasswordInput == null ||
            confirmPasswordInput == null || errorText == null || successText == null)
            return;

        string email = emailInput.Text?.Trim() ?? "";
        string currentPassword = currentPasswordInput.Text?.Trim() ?? "";
        string newPassword = newPasswordInput.Text?.Trim() ?? "";
        string confirmPassword = confirmPasswordInput.Text?.Trim() ?? "";

        errorText.Text = "";
        successText.Text = "";

        string validationError = ValidatePasswordChange(email, currentPassword, newPassword, confirmPassword);
        if (!string.IsNullOrEmpty(validationError))
        {
            errorText.Text = validationError;
            return;
        }

        if (!authService.ChangePassword(email, currentPassword, newPassword))
        {
            errorText.Text = "Невірний email або поточний пароль.";
            return;
        }

        successText.Text = "Пароль успішно змінено";
        emailInput.Text = "";
        currentPasswordInput.Text = "";
        newPasswordInput.Text = "";
        confirmPasswordInput.Text = "";
    }

    private void LoginWindowButton_Click(object sender, RoutedEventArgs e)
    {
        var nextWindow = new LoginWindow();
        nextWindow.Show();
        this.Close();
    }

    private string ValidatePasswordChange(string email, string currentPassword, string newPassword, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "Email не може бути порожнім.";

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return "Невірний формат email.";

        if (string.IsNullOrWhiteSpace(currentPassword))
            return "Поточний пароль не може бути порожнім.";

        if (string.IsNullOrWhiteSpace(newPassword))
            return "Новий пароль не може бути порожнім.";

        if (newPassword.Length < 6)
            return "Новий пароль повинен бути не менше 6 символів.";

        if (currentPassword == newPassword)
            return "Новий пароль повинен відрізнятися від поточного.";

        if (newPassword != confirmPassword)
            return "Паролі не збігаються.";

        return "";
    }
}
