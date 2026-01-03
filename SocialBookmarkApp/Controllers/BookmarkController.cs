using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialBookmarkApp.Data;
using SocialBookmarkApp.Models;

namespace SocialBookmarkApp.Controllers;

public class BookmarkController(AppDbContext context, UserManager<ApplicationUser> userManager) : Controller
{
    
    private readonly AppDbContext db = context;

    // GET
    [AllowAnonymous]
    public IActionResult Index()
    {
        var bookmarks = db.Bookmarks
            .Include(b => b.User)
            .OrderByDescending(b => b.CreatedAt)
            .Where(b => b.IsPublic);

        ViewBag.Bookmarks = bookmarks;

        if (TempData.ContainsKey("message"))
        {
            ViewBag.Message = TempData["message"];
            ViewBag.Alert = TempData["messageType"];
        }

        return View();
    }
    
    
}