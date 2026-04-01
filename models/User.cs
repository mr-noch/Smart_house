using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_house.models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public UserRole Role { get; private set; }
        public string Email { get; private set; }
        private string Password;


        public User(int id, string name, UserRole role, string email, string password)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Role = role;
        }

        public void Login()
        {
            Console.WriteLine(Name + " logged in");
        }

        public void Logout()
        {
            Console.WriteLine(Name + " logged out");
        }
    }

    
}
