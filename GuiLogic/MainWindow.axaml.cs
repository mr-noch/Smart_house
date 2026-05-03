using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System.Collections.Generic;
using System.Linq;
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
        var activeHouse = UserSession.ActiveHouse;
        if (activeHouse != null)
        {
            RenderRoomsFromList(activeHouse.Rooms);
            var headerText = this.FindControl<TextBlock>("HouseOwnerText");
            if (headerText != null)
            {
                headerText.Text = activeHouse.Id == UserSession.CurrentUser?.Id
                    ? "Ваш будинок"
                    : $"Будинок користувача {activeHouse.Name}";
            }
        }
    }

    private void LoadRooms()
    {
        var activeHouse = UserSession.ActiveHouse;

        if (activeHouse != null)
        {
            var roomsControl = this.FindControl<ItemsControl>("RoomsControl");

            if (roomsControl != null)
            {
                roomsControl.ItemsSource = activeHouse.Rooms;
            }
        }
    }
    private void ChooseHouse(object sender, RoutedEventArgs e)
    {
        var panel = this.FindControl<ChooseHousePanel>("ChooseHousePanelControl");
        var musicPanel = this.FindControl<UserControl>("MusikPanelControl");

        if (musicPanel != null)
        {
            musicPanel.IsVisible = false;
        }

        if (panel != null)
        {
            panel.IsVisible = true;
        }
    }

    private void AddRoom(object sender, RoutedEventArgs e)
    {
        var panel = this.FindControl<AddRoomPanel>("AddRoomPanelControl");
        var musicPanel = this.FindControl<UserControl>("MusikPanelControl");

        if (musicPanel != null)
        {
            musicPanel.IsVisible = false;
        }

        if (panel != null)
        {
            panel.IsVisible = true;
        }
    }

    private void OpenHome(object sender, RoutedEventArgs e)
    {
        var homeBorder = this.FindControl<Border>("HomeContentBorder");
        var choosePanel = this.FindControl<ChooseHousePanel>("ChooseHousePanelControl");
        var addRoomPanel = this.FindControl<AddRoomPanel>("AddRoomPanelControl");
        var devicesPanel = this.FindControl<RoomDevicesPanel>("RoomDevicesPanelControl");
        var musicPanel = this.FindControl<UserControl>("MusikPanelControl");

        if (homeBorder != null)
        {
            homeBorder.IsVisible = true;
        }

        if (choosePanel != null)
        {
            choosePanel.IsVisible = false;
        }

        if (addRoomPanel != null)
        {
            addRoomPanel.IsVisible = false;
        }

        if (devicesPanel != null)
        {
            devicesPanel.IsVisible = false;
        }

        if (musicPanel != null)
        {
            musicPanel.IsVisible = false;
        }
    }

    private void OpenMusic(object sender, RoutedEventArgs e)
    {
        var musicPanel = this.FindControl<UserControl>("MusikPanelControl");
        var choosePanel = this.FindControl<ChooseHousePanel>("ChooseHousePanelControl");

        if (choosePanel != null)
        {
            choosePanel.IsVisible = false;
        }

        if (musicPanel != null)
        {
            musicPanel.IsVisible = true;
        }
    }

    public void RenderRoomsFromList(List<Room> rooms)
    {

        var grid = this.FindControl<UniformGrid>("RoomsGrid");
        if (grid == null) return;

        grid.Children.Clear();

        foreach (var room in rooms)
        {
            var textGroup = new StackPanel
            {
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Spacing = 4,


            };

            var nameLabel = new TextBlock
            {
                Text = room.Name,
                Foreground = Brushes.White,
                FontSize = 14,
                FontWeight = FontWeight.Bold,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };

            var descLabel = new TextBlock
            {
                Text = room.Type.ToString(),
                Foreground = Brushes.Gray,
                FontSize = 10,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };

            textGroup.Children.Add(nameLabel);
            textGroup.Children.Add(descLabel);

            var deleteButton = new Button
            {
                Foreground = Brushes.White,
                Background = Brush.Parse("#e74c3c"),
                FontSize = 30,
                Padding = new Thickness(0),
                Width = 24,
                Height = 24,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                CornerRadius = new CornerRadius(50),
                Margin = new Thickness(2)
            };

            deleteButton.Click += (sender, e) => DeleteRoom(room);

            var manageDevicesButton = new Button
            {
                Content = "Пристрої",
                Foreground = Brushes.White,
                Background = Brush.Parse("#2d89ef"),
                FontSize = 12,
                CornerRadius = new CornerRadius(8),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 8, 0, 0)
            };
            manageDevicesButton.Click += (sender, e) => OpenDevicePanel(room);

            var cardContent = new Grid();
            cardContent.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            cardContent.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            cardContent.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            cardContent.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            cardContent.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

            cardContent.Children.Add(textGroup);
            cardContent.Children.Add(deleteButton);
            cardContent.Children.Add(manageDevicesButton);
            Grid.SetColumn(textGroup, 0);
            Grid.SetColumn(deleteButton, 1);
            Grid.SetRow(deleteButton, 0);
            Grid.SetRow(textGroup, 0);
            Grid.SetRow(manageDevicesButton, 2);
            Grid.SetColumnSpan(manageDevicesButton, 2);

            var roomCard = new Border
            {
                Background = Brush.Parse("rgb(49, 55, 97)"),
                Width = 150,
                Height = 130,
                CornerRadius = new CornerRadius(12),
                Margin = new Thickness(15),
                Padding = new Thickness(8),
                Child = cardContent
            };
            grid.Children.Add(roomCard);
        }
    }

    private void DeleteRoom(Room room)
    {
        var activeHouse = UserSession.ActiveHouse;
        if (activeHouse != null)
        {
            activeHouse.Rooms.Remove(room);
            RenderRoomsFromList(activeHouse.Rooms);
        }
        UserAuthService.SaveAllUsers();
    }

    private void OpenDevicePanel(Room room)
    {
        var panel = this.FindControl<RoomDevicesPanel>("RoomDevicesPanelControl");
        panel?.Open(room);
    }

    public void LogOut(object sender, RoutedEventArgs e)
    {
        UserSession.Logout();
        var loginWin = new LoginWindow();
        loginWin.Show();
        this.Close();
    }
}