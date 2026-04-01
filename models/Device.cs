using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_house.models
{
    public class Device
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public bool IsOn { get; set; }
        public string? RoomType { get; set; }


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
}
