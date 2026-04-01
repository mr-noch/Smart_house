using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_house.models
{
    public class SmartHome
    {
        public List<User> Users { get; set; } = new List<User>();
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<Device> Devices { get; set; } = new List<Device>();

        public void AddUser(User user)
        {
            Users.Add(user);
        }

        public void RemoveUser(User user)
        {
            Users.Remove(user);
        }

        public void AddDevice(Device device)
        {
            Devices.Add(device);
        }

        public void RemoveDevice(Device device)
        {
            Devices.Remove(device);
        }

        public void AddRoom(Room room)
        {
            Rooms.Add(room);
        }

        public void RemoveRoom(Room room)
        {
            Rooms.Remove(room);
        }
    }
}
