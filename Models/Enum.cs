using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHouseUI.Models;

public enum UserRole
{
    Admin,
    User,
    Guest
}

public enum RoomType
{
    Bedroom,
    LivingRoom,
    Kitchen,
    Bathroom,
    Garage,
    Office,
    Hallway
}

public enum DeviceType
{
    Light,
    Thermostat,
    Camera,
    Sensor,
    DoorLock
}
