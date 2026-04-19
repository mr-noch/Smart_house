using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using SmartHouseUI.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SmartHouseUI.GuiLogic;

public partial class RoomDevicesPanel : UserControl
{
    private Room? _room;

    public RoomDevicesPanel()
    {
        InitializeComponent();

        var typeBox = this.FindControl<ComboBox>("DeviceTypeComboBox");
        if (typeBox != null)
        {
            typeBox.ItemsSource = Enum.GetValues(typeof(DeviceType));
            typeBox.SelectedIndex = 0;
        }
    }

    public void Open(Room room)
    {
        _room = room;
        var header = this.FindControl<TextBlock>("RoomNameText");
        if (header != null)
        {
            header.Text = room.Name ?? "Нова кімната";
        }

        RenderDevices();
        this.IsVisible = true;
    }

    private void AddDevice(object sender, RoutedEventArgs e)
    {
        if (_room == null)
            return;

        var nameInput = this.FindControl<TextBox>("DeviceNameInput");
        var typeBox = this.FindControl<ComboBox>("DeviceTypeComboBox");

        string name = nameInput?.Text?.Trim() ?? string.Empty;
        DeviceType selectedType = (DeviceType)(typeBox?.SelectedItem ?? DeviceType.Light);

        if (string.IsNullOrWhiteSpace(name))
            return;

        var newDevice = new Device
        {
            Id = GetNextDeviceId(),
            Name = name,
            Type = selectedType,
            RoomType = _room.Type,
            IsOn = false
        };

        _room.Devices.Add(newDevice);
        RenderDevices();
        if (nameInput != null)
            nameInput.Text = string.Empty;
    }

    private void RenderDevices()
    {
        var devicesPanel = this.FindControl<StackPanel>("DevicesListPanel");
        if (devicesPanel == null || _room == null)
            return;

        devicesPanel.Children.Clear();

        foreach (var device in _room.Devices)
        {
            var deviceNameBox = new TextBox
            {
                Text = device.Name,
                Width = 180,
                Background = Brush.Parse("#1f1f21"),
                Foreground = Brushes.White,
                BorderBrush = Brushes.Gray
            };

            var saveButton = new Button
            {
                Content = "Зберегти",
                Background = Brush.Parse("#2d89ef"),
                Foreground = Brushes.White,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right
            };
            saveButton.Click += (_, _) => SaveDeviceName(device, deviceNameBox);

            var deleteButton = new Button
            {
                Content = "Видалити",
                Background = Brush.Parse("#e74c3c"),
                Foreground = Brushes.White,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right
            };
            deleteButton.Click += (_, _) => DeleteDevice(device);

            var headerGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("*, Auto")
            };
            headerGrid.Children.Add(deviceNameBox);
            headerGrid.Children.Add(saveButton);
            Grid.SetColumn(saveButton, 1);

            var row = new Border
            {
                Background = Brush.Parse("#14172D"),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10)
            };

            var content = new StackPanel { Spacing = 6 };
            content.Children.Add(headerGrid);
            content.Children.Add(new TextBlock
            {
                Text = device.Type.ToString(),
                Foreground = Brushes.Gray,
                FontSize = 12
            });
            content.Children.Add(deleteButton);
            row.Child = content;

            devicesPanel.Children.Add(row);
        }
    }

    private void SaveDeviceName(Device device, TextBox nameBox)
    {
        string newName = nameBox.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(newName))
            return;

        device.Name = newName;
        RenderDevices();
    }

    private void DeleteDevice(Device device)
    {
        if (_room == null)
            return;

        _room.Devices.Remove(device);
        RenderDevices();
    }

    private void ClosePanel(object sender, RoutedEventArgs e)
    {
        this.IsVisible = false;
    }

    private int GetNextDeviceId()
    {
        if (_room == null || !_room.Devices.Any())
            return 1;

        return _room.Devices.Max(d => d.Id) + 1;
    }
}
