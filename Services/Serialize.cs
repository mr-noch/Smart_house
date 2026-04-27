using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using SmartHouseUI.Models;
using System;
using System.IO;
namespace SmartHouseUI.Services;

using System.Text.Encodings.Web;

using System.Text.Json.Serialization;

public partial class UserAuthService
{
    public static void SaveAllUsers()
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter() }
            };
            string json = JsonSerializer.Serialize(UserAuthService.users, options);
            File.WriteAllText("users.json", json);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Помилка запису: {ex.Message}");
        }
    }

    public static void LoadData()
    {
        string filePath = "users.json";

        if (!File.Exists(filePath)) return;

        try
        {
            string json = File.ReadAllText(filePath);

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                PropertyNameCaseInsensitive = true
            };

            var loadedUsers = JsonSerializer.Deserialize<List<User>>(json, options);

            if (loadedUsers != null)
            {
                users = loadedUsers;

                if (users.Count > 0)
                {
                    nextId = users.Max(u => u.Id);
                }
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Помилка {ex.Message}");
        }
    }

}