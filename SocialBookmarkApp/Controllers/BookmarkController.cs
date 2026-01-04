using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialBookmarkApp.Data;
using SocialBookmarkApp.Models;

namespace SocialBookmarkApp.Controllers;

public class BookmarkController(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : Controller
{
    
    private readonly AppDbContext db = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    
    // GET
    [AllowAnonymous]
    public IActionResult Index(string sort, int page)
    {
        int pageSize = 5;
        if(page < 1) 
            page = 1;
        if (sort == null)
            sort = "recent";
        
        ViewBag.Sort = sort;
        ViewBag.Page = page;
        
        var bookmarks = db.Bookmarks
            .Include(b => b.User)
            .Include(b => b.Votes)   
            .Where(b => b.IsPublic);

        if (sort == "recent")
        {
            bookmarks = bookmarks.OrderByDescending(b => b.CreatedAt);
        }
        else if (sort == "popular")
        {
            bookmarks = bookmarks.OrderByDescending(b => b.Votes.Count)
                .ThenByDescending(b => b.CreatedAt);;
        }
        int totalItems = bookmarks.Count();
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        if (totalPages == 0)
            totalPages = 1;
        
        var bookmarksPage = bookmarks
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.Bookmarks = bookmarksPage;
        ViewBag.TotalPages = totalPages;

        if (TempData.ContainsKey("message"))
        {
            ViewBag.Message = TempData["message"];
            ViewBag.Alert = TempData["messageType"];
        }

        return View();
    }
    
    [AllowAnonymous]
    public IActionResult Show(int id)
    {
        ViewBag.UserCurent = _userManager.GetUserId(User);
        Bookmark? bookmark = db.Bookmarks
            .Include(b => b.User)
            .Include(b => b.Votes)
            .Include(b => b.Comments)
            .ThenInclude(c => c.User)
            .Include(b => b.BookmarkCategories)
            .ThenInclude(bc => bc.Category)
            .FirstOrDefault(b => b.Id == id);

        ViewBag.UserHasLiked = bookmark.Votes
            .Any(v => v.UserId == _userManager.GetUserId(User));
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
    [Authorize(Roles="Admin, User")]
    public IActionResult New()
    {
        Bookmark bookmark = new Bookmark();

        return View(bookmark);
    }

    [HttpPost]
    [Authorize(Roles="Admin, User")]
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
    
    [Authorize(Roles="Admin, User")]
    public IActionResult Edit(int id)
    {
        var bookmark = db.Bookmarks.Find(id);
        if (bookmark == null) return NotFound();

        var userId = _userManager.GetUserId(User);
        if (bookmark.UserId != userId && !User.IsInRole("Admin"))
        {
            TempData["message"] = "Nu aveti dreptul sa editati acest bookmark.";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index");
        }

        return View(bookmark);
    }
    
    [HttpPost]
    [Authorize(Roles="Admin, User")]
    public IActionResult Edit(int id, Bookmark requestBookmark)
    {
        var bookmark = db.Bookmarks.Find(id);
        if (bookmark == null) return NotFound();

        var userId = _userManager.GetUserId(User);
        if (bookmark.UserId != userId && !User.IsInRole("Admin"))
        {
            TempData["message"] = "Nu aveti dreptul sa editati acest bookmark.";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index");
        }

        if (ModelState.IsValid)
        {
            bookmark.Title = requestBookmark.Title;
            bookmark.Description = requestBookmark.Description;
            bookmark.MediaContent = requestBookmark.MediaContent;
            bookmark.IsPublic = requestBookmark.IsPublic;

            db.SaveChanges();

            TempData["message"] = "Bookmark-ul a fost modificat.";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Show", new { id = bookmark.Id });
        }

        return View(requestBookmark);
    }
    
    [HttpPost]
    [Authorize(Roles="Admin, User")]
    public IActionResult Delete(int id)
    {
        var bookmark = db.Bookmarks.Find(id);
        if (bookmark == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (bookmark.UserId != userId && !User.IsInRole("Admin"))
        {
            TempData["message"] = "Nu aveti dreptul sa stergeti acest bookmark.";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index");
        }

        db.Bookmarks.Remove(bookmark);
        db.SaveChanges();

        TempData["message"] = "Bookmark-ul a fost sters.";
        TempData["messageType"] = "alert-success";
        return RedirectToAction("Index");
    }


}