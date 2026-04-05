using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHouseUI.Models;

public class Device
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DeviceType Type { get; set; }
    public bool IsOn { get; set; }
    public RoomType RoomType { get; set; }


    public virtual void TurnOn()
    {
        IsOn = true;
        System.Console.WriteLine(Name + " turned on.");
    }

    public virtual void TurnOff()
    {
        IsOn = false;
        System.Console.WriteLine(Name + " turned off.");
    }
    public bool IsNameEqual(Device device)
    {
        if(this.Name == device.Name)
        {
            return true;
        }
        else return false;
    }
}
