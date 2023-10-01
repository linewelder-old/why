using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Why.Data;
using Why.Models;

namespace Why.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public List<Post> Posts { get; set; } = null!;

    private async Task LoadPosts()
    {
        Posts = await _context.Posts
            .OrderByDescending(p => p.Date)
            .Include(p => p.User)
            .ToListAsync();
    }

    public async Task OnGetAsync()
    {
        await LoadPosts();
    }

    [BindProperty]
    [Required(ErrorMessage = "Post message cannot be empty")]
    public string PostMessage { get; set; } = null!;

    public async Task<ActionResult> OnPostAsync()
    {
        await LoadPosts();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            ModelState.AddModelError("", "You must be logged in to post");
            return Unauthorized();
        }

        _context.Posts.Add(new Post
        {
            Text = PostMessage,
            Date = DateTime.Now,
            UserId = user.Id
        });
        await _context.SaveChangesAsync();

        PostMessage = string.Empty;
        return Redirect("/");
    }
}
