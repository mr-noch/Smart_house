using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHouseUI.Models;

public class Room
{
    public string? Name { get; set; }
    public RoomType Type { get; set; }
    public List<Device> Devices { get; set; } = new();

}
