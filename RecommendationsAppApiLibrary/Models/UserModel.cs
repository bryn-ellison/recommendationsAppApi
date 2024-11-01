using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static RecommendationsAppApiLibrary.Models.Enums;

namespace RecommendationsAppApiLibrary.Models;

public class UserModel
{
    [Required]
    public string Id { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Role Role { get; set; }
    public string? Bio {  get; set; }
    public string? Avatar { get; set; }
}
