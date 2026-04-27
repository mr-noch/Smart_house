using Avalonia.Controls;
using Avalonia.Interactivity;
using SmartHouseUI.Services;
using SmartHouseUI.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using SmartHouseUI.GuiLogic;
using Avalonia;
using System.Text.RegularExpressions;

namespace SmartHouseUI.GuiLogic;

public partial class AddRoomPanel : UserControl
{
    public AddRoomPanel()
    {
        InitializeComponent();
        var typeBox = this.FindControl<ComboBox>("TypeComboBox");
        if (typeBox != null)
        {
            typeBox.ItemsSource = Enum.GetValues(typeof(RoomType));
            typeBox.SelectedIndex = 0;
        }
    }

    private void AddRoom(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string name = this.FindControl<TextBox>("RoomNameInput")?.Text ?? "";
        var typeBox = this.FindControl<ComboBox>("TypeComboBox");
        var errorText = this.FindControl<TextBlock>("ErrorText");

        if (errorText == null) return;

        RoomType selectedType = (RoomType)(typeBox?.SelectedItem ?? RoomType.LivingRoom);

        // Validation
        string validationError = ValidateRoomName(name);
        if (!string.IsNullOrEmpty(validationError))
        {
            errorText.Text = validationError;
            return;
        }

        errorText.Text = "";

        var activeHouse = UserSession.ActiveHouse;
        if (activeHouse != null && !string.IsNullOrWhiteSpace(name))
        {
            activeHouse.Rooms.Add(new Room { Name = name, Type = selectedType });
            UserAuthService.SaveAllUsers();

            var mainWindow = this.Parent?.Parent as MainWindow;
            mainWindow?.RenderRoomsFromList(activeHouse.Rooms);
        }
        this.IsVisible = false;
    }

    private string ValidateRoomName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Назва кімнати не може бути порожньою.";

        if (name.Length < 2 || name.Length > 50)
            return "Назва кімнати повинна бути від 2 до 50 символів.";

        if (!Regex.IsMatch(name, @"^[a-zA-Zа-яА-ЯіІїЇєЄ0-9\s\-_]+$"))
            return "Назва кімнати може містити тільки букви, цифри, пробіли, дефіси та підкреслення.";

        return "";
    }
}