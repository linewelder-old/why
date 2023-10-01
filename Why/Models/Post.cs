using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Why.Models;

public class Post
{
    public int Id { get; set; }

    [Required, MaxLength(255)]
    public required string Text { get; set; }

    [Required]
    public required DateTime Date { get; set; }

    [Required]
    public required string UserId { get; set; }

    public IdentityUser? User { get; set; }
}
