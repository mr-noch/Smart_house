using Avalonia.Controls;
using Avalonia.Interactivity;
using SmartHouseUI.Services;
using SmartHouseUI.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using SmartHouseUI.GuiLogic;
using Avalonia;

namespace SmartHouseUI.GuiLogic;

public partial class AddRoomPanel : UserControl
{
    public AddRoomPanel()
    {
        InitializeComponent();

        // Заповнюємо ComboBox значеннями з enum
        var typeBox = this.FindControl<ComboBox>("TypeComboBox");
        if (typeBox != null)
        {
            typeBox.ItemsSource = Enum.GetValues(typeof(RoomType));
            typeBox.SelectedIndex = 0; // Вибираємо перший елемент за замовчуванням
        }
    }

    private void AddRoom(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string name = this.FindControl<TextBox>("RoomNameInput")?.Text ?? "";

        // Отримуємо вибраний тип
        var typeBox = this.FindControl<ComboBox>("TypeComboBox");
        RoomType selectedType = (RoomType)(typeBox?.SelectedItem ?? RoomType.LivingRoom);

        var user = UserSession.CurrentUser;
        if (user != null && !string.IsNullOrWhiteSpace(name))
        {
            user.Rooms.Add(new Room { Name = name, Type = selectedType });
            // Update UI
            var mainWindow = this.Parent?.Parent as MainWindow;
            mainWindow?.RenderRoomsFromList(user.Rooms);
        }
        this.IsVisible = false;
    }
}