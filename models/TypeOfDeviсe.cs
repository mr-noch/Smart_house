using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_house.models
{
    public class Thermostat : Device
    {
        public double CurrentTemperature { get; set; }
        public double TargetTemperature { get; set; }
        public string Mode { get; set; }

        public void SetTemperature(double temp)
        {
            TargetTemperature = temp;
        }

        public double GetCurrentTemperature()
        {
            return CurrentTemperature;
        }

        public void IncreaseTemperature(double value)
        {
            TargetTemperature += value;
        }

        public void DecreaseTemperature(double value)
        {
            TargetTemperature -= value;
        }

        public void SetMode(string mode)
        {
            Mode = mode;
        }
    }

    public class Light : Device
    {
        public int Brightness { get; set; }
        public string Color { get; set; }
        public double EnergyUsage { get; set; }

        public void SetBrightness(int level)
        {
            Brightness = level;
        }

        public void DimLight(int level)
        {
            Brightness -= level;
        }

        public void ChangeColor(string color)
        {
            Color = color;
        }

        public double GetEnergyUsage()
        {
            return EnergyUsage;
        }
    }

    public class Camera : Device
    {
        public string Resolution { get; set; }
        public string StoragePath { get; set; }
        public bool IsRecording { get; set; }

        public void StartRecording()
        {
            IsRecording = true;
        }

        public void StopRecording()
        {
            IsRecording = false;
        }

        public void StreamVideo()
        {
            System.Console.WriteLine("Streaming video");
        }

        public void SaveVideo()
        {
            System.Console.WriteLine("Video saved" + StoragePath);
        }
    }
}
