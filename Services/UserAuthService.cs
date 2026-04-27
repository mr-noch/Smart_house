using System.Linq;
using System.Collections.Generic;
using SmartHouseUI.Models;
using System.IO;
using System;

namespace SmartHouseUI.Services;

public partial class UserAuthService
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
                Password = password,
                Rooms = new List<Room>
                {
                    new Room { Name = "LivingRoom", Type = RoomType.LivingRoom,},
                    new Room { Name = "Kitchen", Type = RoomType.Kitchen,},
                    new Room { Name = "Bedroom", Type = RoomType.Bedroom,}
                }
            };
            users.Add(newUser);
            SaveAllUsers();

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

    public bool LogIn(string email, string password)
    {
        var user = users.FirstOrDefault(u => u.Email == email && u.Password == password);

        if (user != null)
        {
            UserSession.CurrentUser = user;
            UserSession.ActiveHouseOwner = null;

            System.Console.WriteLine($"User: {user.Name} залогінився");
            return true;
        }

        System.Console.WriteLine("Помилка входу: невірний Email або пароль.");
        return false;
    }

    public User? GetUserById(int id)
    {
        return users.FirstOrDefault(u => u.Id == id);
    }

    public User? GetUserByEmail(string email)
    {
        return users.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
    }

    public bool GrantHouseAccess(int ownerId, string targetEmail)
    {
        var owner = GetUserById(ownerId);
        if (owner == null)
            return false;

        var target = GetUserByEmail(targetEmail);
        if (target == null)
            return false;

        if (target.Id == owner.Id)
            return false;

        if (target.AccessOwnerIds.Contains(owner.Id))
            return false;

        target.AccessOwnerIds.Add(owner.Id);
        SaveAllUsers();
        return true;
    }

    public bool ChangePassword(string email, string currentPassword, string newPassword)
    {
        var user = GetUserByEmail(email);
        if (user == null)
            return false;

        if (user.Password != currentPassword)
            return false;

        user.Password = newPassword;
        SaveAllUsers();
        return true;
    }
}
public static class UserSession
{
    public static User? CurrentUser { get; set; }
    public static User? ActiveHouseOwner { get; set; }
    public static bool IsLoggedIn => CurrentUser != null;
    public static User? ActiveHouse => ActiveHouseOwner ?? CurrentUser;

    public static void Logout()
    {
        CurrentUser = null;
        ActiveHouseOwner = null;
    }
}



