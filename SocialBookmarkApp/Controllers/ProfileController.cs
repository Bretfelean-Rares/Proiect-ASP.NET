using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialBookmarkApp.Data;
using SocialBookmarkApp.Models;

namespace SocialBookmarkApp.Controllers;

public class ProfileController(AppDbContext context, UserManager<ApplicationUser> userManager) : Controller
{
    private readonly AppDbContext _db = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    // GET
    public IActionResult Show(string id)
    {
        var user = _db.Users
            .Include(u => u.Bookmarks)
            .FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound();

        // doar bookmark-uri publice
        user.Bookmarks = user.Bookmarks?
            .Where(b => b.IsPublic)
            .ToList();

        return View(user);
    }
    
    public IActionResult MyProfile()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
            return RedirectToAction("Login", "Account");

        return RedirectToAction("Show", new { id = userId });
    }
}