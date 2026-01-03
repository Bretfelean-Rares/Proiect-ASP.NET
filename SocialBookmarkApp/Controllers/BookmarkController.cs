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
    private readonly UserManager<ApplicationUser> _userManager = userManager;

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
    
    [Authorize]
    public IActionResult Show(int id)
    {
        Bookmark? bookmark = db.Bookmarks
            .Include(b => b.User)
            .Include(b => b.Comments)
            .ThenInclude(c => c.User)
            .Include(b => b.BookmarkCategories)
            .ThenInclude(bc => bc.Category)
            .FirstOrDefault(b => b.Id == id);

        if (bookmark == null)
        {
            return NotFound();
        }

        ViewBag.UserCurent = _userManager.GetUserId(User);
        ViewBag.EsteAdmin = User.IsInRole("Admin");

        if (TempData.ContainsKey("message"))
        {
            ViewBag.Message = TempData["message"];
            ViewBag.Alert = TempData["messageType"];
        }

        return View(bookmark);
    }
    
    //Get
    [AllowAnonymous]
    public IActionResult New()
    {
        Bookmark bookmark = new Bookmark();

        return View(bookmark);
    }

    [HttpPost]
    [Authorize]
    public IActionResult New(Bookmark bookmark)
    {
        bookmark.CreatedAt = DateTime.Now;
        bookmark.UserId = _userManager.GetUserId(User);;
        
        if (ModelState.IsValid)
        {
            db.Bookmarks.Add(bookmark);
            db.SaveChanges();

            TempData["message"] = "Bookmark-ul a fost adaugat";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Index");
        }
        else
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            TempData["message"] = "Formular invalid: " + string.Join(" | ", errors);
            TempData["messageType"] = "alert-danger";
            return View(bookmark);
        }
    }
   
}