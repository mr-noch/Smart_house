using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using SmartHouseUI.Models;
using SmartHouseUI.Services;
using Avalonia.Media;
using System.Linq;
using System.Text.RegularExpressions;

namespace SmartHouseUI.GuiLogic;

public partial class ChooseHouse : Window
{
    private readonly UserAuthService _authService = new UserAuthService();

    public ChooseHouse()
    {
        InitializeComponent();
        RenderHouseOptions();
        ConfigureGrantAccessPanel();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void RenderHouseOptions()
    {
        var housesPanel = this.FindControl<StackPanel>("HousesPanel");
        if (housesPanel == null)
            return;

        housesPanel.Children.Clear();

        var currentUser = UserSession.CurrentUser;
        if (currentUser == null)
            return;

        housesPanel.Children.Add(CreateHouseButton("Мій будинок", currentUser));

        if (currentUser.AccessOwnerIds.Any())
        {
            housesPanel.Children.Add(new TextBlock
            {
                Text = "Будинки, до яких вам надали доступ:",
                Foreground = Brushes.LightGray,
                FontSize = 14,
                Margin = new Avalonia.Thickness(0, 8, 0, 4)
            });

            foreach (var ownerId in currentUser.AccessOwnerIds)
            {
                var owner = _authService.GetUserById(ownerId);
                if (owner == null)
                    continue;

                housesPanel.Children.Add(CreateHouseButton($"Будинок {owner.Name}", owner));
            }
        }
    }

    private Button CreateHouseButton(string text, User owner)
    {
        var button = new Button
        {
            Content = text,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            Background = Brushes.DarkSlateBlue,
            Foreground = Brushes.White,
            CornerRadius = new Avalonia.CornerRadius(10),
            Padding = new Avalonia.Thickness(12)
        };

        button.Click += (_, _) => OpenSelectedHouse(owner);

        return button;
    }

    private void OpenSelectedHouse(User owner)
    {
        UserSession.ActiveHouseOwner = owner;
        var mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }

    private void ConfigureGrantAccessPanel()
    {
        var grantPanel = this.FindControl<Border>("GrantAccessPanel");
        if (grantPanel == null)
            return;

        var currentUser = UserSession.CurrentUser;
        if (currentUser == null || currentUser.Role != UserRole.Admin)
        {
            grantPanel.IsVisible = false;
            return;
        }

        grantPanel.IsVisible = true;
    }

    private void GrantAccessClick(object sender, RoutedEventArgs e)
    {
        var emailInput = this.FindControl<TextBox>("GrantEmailInput");
        var statusText = this.FindControl<TextBlock>("GrantStatusText");
        var currentUser = UserSession.CurrentUser;

        if (emailInput == null || statusText == null || currentUser == null)
            return;

        string email = emailInput.Text?.Trim() ?? string.Empty;

        // Validation
        string validationError = ValidateEmail(email);
        if (!string.IsNullOrEmpty(validationError))
        {
            statusText.Foreground = Brushes.OrangeRed;
            statusText.Text = validationError;
            return;
        }

        var targetUser = _authService.GetUserByEmail(email);
        if (targetUser == null)
        {
            statusText.Foreground = Brushes.OrangeRed;
            statusText.Text = "Користувача з таким email не знайдено.";
            return;
        }

        if (targetUser.Id == currentUser.Id)
        {
            statusText.Foreground = Brushes.OrangeRed;
            statusText.Text = "Ви не можете надати доступ собі.";
            return;
        }

        if (_authService.GrantHouseAccess(currentUser.Id, email))
        {
            statusText.Foreground = Brushes.LightGreen;
            statusText.Text = $"Доступ надано користувачу {targetUser.Name}.";
        }
        else
        {
            statusText.Foreground = Brushes.OrangeRed;
            statusText.Text = "Доступ вже надано або сталася помилка.";
        }
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
