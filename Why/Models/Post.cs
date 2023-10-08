using System.ComponentModel.DataAnnotations;
using Why.Data;

namespace Why.Models;

public class Post
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Post cannot be empty")]
    [MaxLength(255, ErrorMessage = "Post can only be up to 255 characters long")]
    public required string Text { get; set; }

    [Required]
    public required DateTime Date { get; set; }

    [Required]
    public required string UserId { get; set; }

    public User? User { get; set; }
}
