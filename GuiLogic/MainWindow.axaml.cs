using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System.Collections.Generic;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using SmartHouseUI.Services;
using SmartHouseUI.Models;
using Avalonia.Media;

namespace SmartHouseUI.GuiLogic;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        LoadRooms();
        var user = UserSession.CurrentUser;
        if (user != null)
        {
            RenderRoomsFromList(user.Rooms);
        }
    }

    private void LoadRooms()
    {
        var user = UserSession.CurrentUser;

        if (user != null)
        {
            var roomsControl = this.FindControl<ItemsControl>("RoomsControl");

            if (roomsControl != null)
            {
                roomsControl.ItemsSource = user.Rooms;
            }
        }
    }

    private void AddRoom(object sender, RoutedEventArgs e)
    {
        var panel = this.FindControl<AddRoomPanel>("AddRoomPanelControl");
        if (panel != null)
        {
            panel.IsVisible = true;
        }
    }

    public void RenderRoomsFromList(List<Room> rooms)
    {

        var grid = this.FindControl<UniformGrid>("RoomsGrid");
        if (grid == null) return;

        grid.Children.Clear(); // Clear existing cards

        foreach (var name in rooms)
        {
            var textGroup = new StackPanel
            {
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Spacing = 4
            };

            var nameLabel = new TextBlock
            {
                Text = name.Name,
                Foreground = Brushes.White,
                FontSize = 14,
                FontWeight = FontWeight.Bold,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };

            var descLabel = new TextBlock
            {
                Text = name.Type.ToString(),
                Foreground = Brushes.Gray,
                FontSize = 10,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };

            textGroup.Children.Add(nameLabel);
            textGroup.Children.Add(descLabel);

            var roomCard = new Border
            {
                Background = Brush.Parse("#252526"),
                Width = 110, // Можна трохи розширити, щоб вліз опис
                Height = 100,
                CornerRadius = new CornerRadius(12),
                Margin = new Thickness(10),
                Child = textGroup // <--- Ось тут ми вставили панель з усіма текстами
            };
            grid.Children.Add(roomCard);
        }
    }
    public void LogOut(object sender, RoutedEventArgs e)
    {
        UserSession.Logout();
        var loginWin = new LoginWindow();
        loginWin.Show();
        this.Close();
    }
}