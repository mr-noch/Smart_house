using System.Collections.Generic;
using SmartHouseUI.Models;

namespace SmartHouseUI.Services;

public class UserAuthService
{
    private static List<User> users = new();
    private static int nextId = 0;

    public bool LogUser(string Email)
    {
        if (users.Exists(u => u.Email == Email))
        {
            System.Console.WriteLine("user registred");
            return true;
        }
        return false;
    }
    public bool SignUp(string username, string email, string password)
    {
        if (!LogUser(email))
        {
            User newUser = new User
            {
                Id = ++nextId,
                Name = username,
                Role = UserRole.Admin,
                Email = email,
                Password = password
            };

            users.Add(newUser);
            System.Console.WriteLine($" Юзер: {username} зареєстрований");
            foreach (var u in users)
            {
                System.Console.WriteLine($"User: {u.Name}, Email: {u.Email} Id: {u.Id}");
            }
            return true;
        }
        else System.Console.WriteLine("User with this email already exists");
        return false;

    }

    public User LogIn(string email, string password)
    {
        foreach (var u in users)
        {
            if (u.Email == email && u.Password == password)
            {
                System.Console.WriteLine($"User: {u.Name} logged in");
                return u;
            }
        }
        return null;
    }
}