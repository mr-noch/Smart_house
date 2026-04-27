using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using SmartHouseUI.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Collections.Generic;
using SmartHouseUI.Services;
using System.Text.RegularExpressions;

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
        var errorText = this.FindControl<TextBlock>("ErrorText");

        if (errorText == null) return;

        string name = nameInput?.Text?.Trim() ?? string.Empty;
        DeviceType selectedType = (DeviceType)(typeBox?.SelectedItem ?? DeviceType.Light);

        string validationError = ValidateDeviceName(name);
        if (!string.IsNullOrEmpty(validationError))
        {
            errorText.Text = validationError;
            return;
        }

        errorText.Text = "";

        Device newDevice = selectedType switch
        {
            DeviceType.Thermostat => new Thermostat
            {
                Id = GetNextDeviceId(),
                Name = name,
                Type = selectedType,
                RoomType = _room.Type,
                IsOn = false,
                CurrentTemperature = 20,
                TargetTemperature = 22,
                Mode = "Auto"
            },
            DeviceType.Light => new Light
            {
                Id = GetNextDeviceId(),
                Name = name,
                Type = selectedType,
                RoomType = _room.Type,
                IsOn = false,
                Brightness = 75,
                Color = "White",
                EnergyUsage = 0
            },
            DeviceType.Camera => new Camera
            {
                Id = GetNextDeviceId(),
                Name = name,
                Type = selectedType,
                RoomType = _room.Type,
                IsOn = false,
                IsRecording = false,
                Resolution = "1080p",
                StoragePath = "videos/"
            },
            _ => new Device
            {
                Id = GetNextDeviceId(),
                Name = name,
                Type = selectedType,
                RoomType = _room.Type,
                IsOn = false
            }
        };

        _room.Devices.Add(newDevice);
        UserAuthService.SaveAllUsers();
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

            var functionPanel = new StackPanel { Spacing = 6 };
            functionPanel.Children.Add(new TextBlock
            {
                Text = device.Type.ToString(),
                Foreground = Brushes.Gray,
                FontSize = 12
            });

            var toggleButton = new Button
            {
                Content = device.IsOn ? "Вимкнути" : "Увімкнути",
                Background = device.IsOn ? Brush.Parse("#e74c3c") : Brush.Parse("#2d89ef"),
                Foreground = Brushes.White,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
            };
            toggleButton.Click += (_, _) => ToggleDevicePower(device);
            functionPanel.Children.Add(toggleButton);

            if (device is Thermostat thermostat)
            {
                functionPanel.Children.Add(new TextBlock
                {
                    Text = $"Температура: {thermostat.CurrentTemperature}°C",
                    Foreground = Brushes.LightGray,
                    FontSize = 12
                });
                var thermostatGrid = new Grid { ColumnDefinitions = new ColumnDefinitions("*, *") };
                var increaseButton = new Button
                {
                    Content = "+1°C",
                    Background = Brush.Parse("#3a7bd5"),
                    Foreground = Brushes.White,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    Margin = new Thickness(0, 0, 4, 0)
                };
                increaseButton.Click += (_, _) => UpdateThermostatTemperature(thermostat, 1);
                var decreaseButton = new Button
                {
                    Content = "-1°C",
                    Background = Brush.Parse("#3a7bd5"),
                    Foreground = Brushes.White,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
                };
                decreaseButton.Click += (_, _) => UpdateThermostatTemperature(thermostat, -1);
                thermostatGrid.Children.Add(increaseButton);
                thermostatGrid.Children.Add(decreaseButton);
                Grid.SetColumn(decreaseButton, 1);
                functionPanel.Children.Add(thermostatGrid);

                var modeButton = new Button
                {
                    Content = $"Режим: {thermostat.Mode}",
                    Background = Brush.Parse("#5a5a5a"),
                    Foreground = Brushes.White,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
                };
                modeButton.Click += (_, _) => CycleThermostatMode(thermostat);
                functionPanel.Children.Add(modeButton);
            }
            else if (device is Light light)
            {
                functionPanel.Children.Add(new TextBlock
                {
                    Text = $"Яскравість: {light.Brightness}% | Колір: {light.Color}",
                    Foreground = Brushes.LightGray,
                    FontSize = 12
                });
                var lightGrid = new Grid { ColumnDefinitions = new ColumnDefinitions("*, *") };
                var brightenButton = new Button
                {
                    Content = "+10%",
                    Background = Brush.Parse("#3a7bd5"),
                    Foreground = Brushes.White,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    Margin = new Thickness(0, 0, 4, 0)
                };
                brightenButton.Click += (_, _) => ChangeLightBrightness(light, 10);
                var dimButton = new Button
                {
                    Content = "-10%",
                    Background = Brush.Parse("#3a7bd5"),
                    Foreground = Brushes.White,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
                };
                dimButton.Click += (_, _) => ChangeLightBrightness(light, -10);
                lightGrid.Children.Add(brightenButton);
                lightGrid.Children.Add(dimButton);
                Grid.SetColumn(dimButton, 1);
                functionPanel.Children.Add(lightGrid);

                var colorButton = new Button
                {
                    Content = "Змінити колір",
                    Background = Brush.Parse("#5a5a5a"),
                    Foreground = Brushes.White,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
                };
                colorButton.Click += (_, _) => ChangeLightColor(light);
                functionPanel.Children.Add(colorButton);
            }
            else if (device is Camera camera)
            {
                functionPanel.Children.Add(new TextBlock
                {
                    Text = $"Роздільна здатність: {camera.Resolution} | Запис: {(camera.IsRecording ? "так" : "ні")}",
                    Foreground = Brushes.LightGray,
                    FontSize = 12
                });
                var cameraGrid = new Grid { ColumnDefinitions = new ColumnDefinitions("*, *") };
                var recordButton = new Button
                {
                    Content = camera.IsRecording ? "Зупинити запис" : "Почати запис",
                    Background = camera.IsRecording ? Brush.Parse("#e74c3c") : Brush.Parse("#2d89ef"),
                    Foreground = Brushes.White,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    Margin = new Thickness(0, 0, 4, 0)
                };
                recordButton.Click += (_, _) => ToggleCameraRecording(camera);
                var saveButton2 = new Button
                {
                    Content = "Зберегти відео",
                    Background = Brush.Parse("#5a5a5a"),
                    Foreground = Brushes.White,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
                };
                saveButton2.Click += (_, _) => camera.SaveVideo();
                cameraGrid.Children.Add(recordButton);
                cameraGrid.Children.Add(saveButton2);
                Grid.SetColumn(saveButton2, 1);
                functionPanel.Children.Add(cameraGrid);
            }

            functionPanel.Children.Add(deleteButton);
            row.Child = functionPanel;

            devicesPanel.Children.Add(row);
        }
    }

    private void SaveDeviceName(Device device, TextBox nameBox)
    {
        string newName = nameBox.Text?.Trim() ?? string.Empty;

        string validationError = ValidateDeviceName(newName);
        if (!string.IsNullOrEmpty(validationError))
        {
            return;
        }

        device.Name = newName;
        RenderDevices();
        UserAuthService.SaveAllUsers();
    }

    private void ToggleDevicePower(Device device)
    {
        if (device.IsOn)
            device.TurnOff();
        else
            device.TurnOn();

        UserAuthService.SaveAllUsers();
        RenderDevices();
    }

    private void UpdateThermostatTemperature(Thermostat thermostat, double delta)
    {
        thermostat.TargetTemperature += delta;
        thermostat.CurrentTemperature += delta  ;
        thermostat.SetTemperature(thermostat.TargetTemperature);
        UserAuthService.SaveAllUsers();
        RenderDevices();
    }

private void CycleThermostatMode(Thermostat thermostat)
{
    var modes = new[] { "Auto", "Heat", "Cool", "Off" };
    var currentIndex = Array.IndexOf(modes, thermostat.Mode);
    int nextIndex = (currentIndex == -1) ? 0 : (currentIndex + 1) % modes.Length;
    thermostat.Mode = modes[nextIndex];
    thermostat.SetMode(thermostat.Mode);
    UserAuthService.SaveAllUsers();
    RenderDevices();
}

    private void ChangeLightBrightness(Light light, int delta)
    {
        light.Brightness = Math.Clamp(light.Brightness + delta, 0, 100);
        light.SetBrightness(light.Brightness);
        UserAuthService.SaveAllUsers();
        RenderDevices();
    }

    private void ChangeLightColor(Light light)
    {
        light.Color = light.Color == "White" ? "Warm" : "White";
        light.ChangeColor(light.Color);
        UserAuthService.SaveAllUsers();
        RenderDevices();
    }

    private void ToggleCameraRecording(Camera camera)
    {
        if (camera.IsRecording)
            camera.StopRecording();
        else
            camera.StartRecording();

        UserAuthService.SaveAllUsers();
        RenderDevices();
    }

    private void DeleteDevice(Device device)
    {
        if (_room == null)
            return;

        _room.Devices.Remove(device);
        RenderDevices();
        UserAuthService.SaveAllUsers();
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

    private string ValidateDeviceName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Назва пристрою не може бути порожньою.";

        if (name.Length < 2 || name.Length > 50)
            return "Назва пристрою повинна бути від 2 до 50 символів.";

        if (!Regex.IsMatch(name, @"^[a-zA-Zа-яА-ЯіІїЇєЄ0-9\s\-_]+$"))
            return "Назва пристрою може містити тільки букви, цифри, пробіли, дефіси та підкреслення.";

        return "";
    }
}
