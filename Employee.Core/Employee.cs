using System.Text.Json.Serialization;

namespace Domain;

public record Employee
{
    public Employee(int id, string name, string email, string gender, string status)
    {
        Id = id;
        Name = name;
        Email = email;
        Gender = gender;
        Status = status;
    }
    
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("gender")]
    public string Gender { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; }
}