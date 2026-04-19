using SmartHouseUI.Models;
using SmartHouseUI.Collections;

namespace SmartHouseUI.Collections;

public static class CollectionFactory
{
    public static DeviceCollection CreateDevices()
    {
        var devices = new DeviceCollection();

        devices.Add(new Light { Id = 1, Name = "Лампа", Type = DeviceType.Light, RoomType = RoomType.Bedroom, IsOn = false, Brightness = 80 });
        devices.Add(new Thermostat { Id = 2, Name = "Термостат", Type = DeviceType.Thermostat, RoomType = RoomType.Kitchen, IsOn = true, CurrentTemperature = 22.5 });
        devices.Add(new Camera { Id = 3, Name = "Камера", Type = DeviceType.Camera, RoomType = RoomType.LivingRoom, IsOn = false, IsRecording = false });

        return devices;
    }
}
