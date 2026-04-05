using System.Linq;
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
                Password = password,
                Rooms = new List<Room>
                {
                    new Room { Name = "Вітальня", Type = RoomType.LivingRoom,},
                    new Room { Name = "Кухня", Type = RoomType.Kitchen,},
                    new Room { Name = "Спальня", Type = RoomType.Bedroom,}
                }
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

    public bool LogIn(string email, string password)
    {
        var user = users.FirstOrDefault(u => u.Email == email && u.Password == password);

        if (user != null)
        {
            UserSession.CurrentUser = user;

            System.Console.WriteLine($"User: {user.Name} залогінився. Сесія відкрита.");
            return true;
        }

        // 4. Якщо не знайшли — повертаємо false
        System.Console.WriteLine("Помилка входу: невірний Email або пароль.");
        return false;
    }
}
public static class UserSession
{
    public static User? CurrentUser { get; set; }
    public static bool IsLoggedIn => CurrentUser != null;
    public static void Logout() => CurrentUser = null;
}

