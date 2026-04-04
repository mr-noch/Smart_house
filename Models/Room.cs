using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHouseUI.Models;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public List<Device> Devices { get; set; } = new();

    public void AddDevice(Device device)
    {
        Devices.Add(device);
    }

    public void RemoveDevice(Device device)
    {
        Devices.Remove(device);
    }
}
