using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialBookmarkApp.Data;
using SocialBookmarkApp.Models;

namespace SocialBookmarkApp.Controllers;

public class VoteController(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : Controller
{
    private readonly AppDbContext db = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    // GET
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    [Authorize]
    public IActionResult Toggle(int bookmarkId)
    {
        var userId = _userManager.GetUserId(User);

        var vote = db.Votes
            .FirstOrDefault(v => v.BookmarkId == bookmarkId && v.UserId == userId);

        if (vote == null)
        {
            db.Votes.Add(new Vote
            {
                BookmarkId = bookmarkId,
                UserId = userId,
                IsUpvote = true
            });
        }
        else
        {
            db.Votes.Remove(vote); // unlike
        }

        db.SaveChanges();
        return RedirectToAction("Show", "Bookmark", new { id = bookmarkId });
    }
}