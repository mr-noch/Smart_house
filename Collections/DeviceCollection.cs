using SmartHouseUI.Models;
using SmartHouseUI.Interfaces;
using System;
using System.Collections.Generic;

namespace SmartHouseUI.Collections;

public class DeviceCollection : IRepository<Device>
{
    public int Length {get{return this.Devices.Count;}}
    public DeviceCollection()
    {
        Devices = new List<Device>();
    }
    public Device this[int index]
    {
        get
        {
            return this.Devices[index];
        }
        set
        {
            this.Devices[index] = value;
        }
    }
    private List<Device> Devices;
    public void Add(Device device)
    {
        this.Devices.Add(device);
    }
    public void Remove(Device device)
    {
        this.Devices.Remove(device);
    }
    public int Find(Device device)
    {
        for(int i = 0; i < this.Devices.Count; i++)
        {
            if (this.Devices[i].IsNameEqual(device))
            {
                return i;
            }
        }
        return -1;
    }
}