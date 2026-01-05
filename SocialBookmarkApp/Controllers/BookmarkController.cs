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
        var userId = _userManager.GetUserId(User);
        
        var bookmarks = db.Bookmarks
            .Include(b => b.User)
            .Include(b => b.Votes)   
            .Where(b => b.IsPublic == true ||
                        (userId != null && b.UserId == userId));

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
            .Include(b => b.BookmarkTags)
            .ThenInclude(bt => bt.Tag)
            .Include(b => b.BookmarkCategories)
            .ThenInclude(bc => bc.Category)
            .FirstOrDefault(b => b.Id == id);
        ViewBag.UserCategories = db.Categories
            .Where(c => c.UserId == _userManager.GetUserId(User))
            .ToList();
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
    public IActionResult New(Bookmark bookmark,string tags)
    {
        bookmark.CreatedAt = DateTime.Now;
        bookmark.UserId = _userManager.GetUserId(User);;
        
        if (ModelState.IsValid)
        {
            db.Bookmarks.Add(bookmark);
            db.SaveChanges();
            
            var tagNames = (tags ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim().ToLower())
                .Where(t => t.Length > 0)
                .Distinct()
                .ToList();

            foreach (var name in tagNames)
            {
                var existing = db.Tags.FirstOrDefault(t => t.Name == name);

                if (existing == null)
                {
                    existing = new Tag { Name = name };
                    db.Tags.Add(existing);
                    db.SaveChanges();
                }

                bool alreadyLinked = db.BookmarkTags.Any(bt => bt.BookmarkId == bookmark.Id && bt.TagId == existing.Id);
                if (!alreadyLinked)
                {
                    db.BookmarkTags.Add(new BookmarkTag
                    {
                        BookmarkId = bookmark.Id,
                        TagId = existing.Id
                    });
                }
            }

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
        var bookmark = db.Bookmarks
            .Include(b => b.BookmarkTags)
            .ThenInclude(bt => bt.Tag)
            .FirstOrDefault(b => b.Id == id);
        
        if (bookmark == null) 
            return NotFound();
       
        var userId = _userManager.GetUserId(User);
        if (bookmark.UserId != userId && !User.IsInRole("Admin"))
        {
            TempData["message"] = "Nu aveti dreptul sa editati acest bookmark.";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index");
        }
        ViewBag.TagsText = string.Join(", ",
            bookmark.BookmarkTags.Select(bt => bt.Tag.Name));
        return View(bookmark);
    }
    
    [HttpPost]
    [Authorize(Roles="Admin, User")]
    public IActionResult Edit(int id, Bookmark requestBookmark, string tags)
    {
        var bookmark = db.Bookmarks
            .Include(b => b.BookmarkTags)
            .FirstOrDefault(b => b.Id == id);
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
            
            var oldLinks = db.BookmarkTags
                .Where(bt => bt.BookmarkId == bookmark.Id)
                .ToList();
            db.BookmarkTags.RemoveRange(oldLinks);
            
            var tagNames = (tags ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => t.Length > 0)
                .Select(t => t.ToLower())
                .Distinct()
                .ToList();
            
            foreach (var name in tagNames)
            {
                var tag = db.Tags.FirstOrDefault(t => t.Name == name);
                if (tag == null)
                {
                    tag = new Tag { Name = name };
                    db.Tags.Add(tag);
                    db.SaveChanges(); // ca să primești tag.Id
                }

                db.BookmarkTags.Add(new BookmarkTag
                {
                    BookmarkId = bookmark.Id,
                    TagId = tag.Id
                });
            }

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

    [HttpPost]
    [Authorize]
    public IActionResult AddCategory(int bookmarkId, int categoryId)
    {
        var userId = _userManager.GetUserId(User);

        var bookmark = db.Bookmarks
            .FirstOrDefault(b => b.Id == bookmarkId);

        if (bookmark == null)
        {
            TempData["message"] = "Bookmark invalid";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index");
        }

        bool exists = db.BookmarkCategories.Any(bc =>
            bc.BookmarkId == bookmarkId &&
            bc.CategoryId == categoryId
        );

        if (exists)
        {
            TempData["message"] = "Bookmark-ul este deja în categorie";
            TempData["messageType"] = "alert-warning";
        }
        else
        {
            db.BookmarkCategories.Add(new BookmarkCategory
            {
                BookmarkId = bookmarkId,
                CategoryId = categoryId
            });

            db.SaveChanges();

            TempData["message"] = "Bookmark adăugat în categorie";
            TempData["messageType"] = "alert-success";
        }

        return RedirectToAction("Show", new { id = bookmarkId });
    }
    
    
}