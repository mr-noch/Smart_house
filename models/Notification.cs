using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_house.models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }



        public Notification(int id, string message)
        {
            Id = id;
            Message = message;
            Date = DateTime.Now;
        }

        public void SendMessage()
        {

        }
    }
}