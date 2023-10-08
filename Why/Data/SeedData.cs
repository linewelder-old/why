using System.Text;
using Microsoft.AspNetCore.Identity;
using Why.Models;

namespace Why.Data;

public class SeedData
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public SeedData(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task InitializeAsync()
    {
        if (_context.Users.Any()) return;

        var admin = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "admin@host",
            NormalizedUserName = "ADMIN@HOST",
            EmailConfirmed = true,
            Email = "admin@host",
            NormalizedEmail = "ADMIN@HOST"
        };

        var result = await _userManager.CreateAsync(admin, "admin_");
        if (!result.Succeeded)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Failed to create admin user:");
            foreach (var error in result.Errors)
            {
                builder.AppendLine(error.Description);
            }

            throw new Exception(builder.ToString());
        }

        _context.Posts.Add(new Post
        {
            Text = "Hello, world!",
            UserId = admin.Id,
            Date = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }
}
