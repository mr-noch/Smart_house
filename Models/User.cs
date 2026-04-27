using System.Collections.Generic;
namespace SmartHouseUI.Models;

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public UserRole Role { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public List<Room> Rooms { get; set; } = new();
    public List<int> AccessOwnerIds { get; set; } = new();
}

