using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialBookmarkApp.Data;
using SocialBookmarkApp.Models;

namespace SocialBookmarkApp.Controllers;

public class CommentController(AppDbContext context, UserManager<ApplicationUser> userManager) : Controller
{
    private readonly AppDbContext db = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    // GET
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    [Authorize]
    public IActionResult New(Comment comm)
    {
        comm.CreatedAt = DateTime.Now;
        comm.UserId = _userManager.GetUserId(User);

        if(ModelState.IsValid)
        {
            db.Comments.Add(comm);
            db.SaveChanges();
            TempData["message"] = "Comentariul a fost adaugat.";
            TempData["messageType"] = "alert-success";        }

        else
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            TempData["message"] = "Comentariu invalid: " + string.Join(" | ", errors);
            TempData["messageType"] = "alert-danger";         
        }
        return RedirectToAction("Show", "Bookmark", new { id = comm.BookmarkId });
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin, User")] 
    public IActionResult Delete(int id)
    {
        Comment? comm = db.Comments.Find(id);

        if (comm == null)
        {
            return NotFound();
        }

        if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
        {
            int bookmarkId = comm.BookmarkId;

            db.Comments.Remove(comm);
            db.SaveChanges();

            TempData["message"] = "Comentariul a fost sters.";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Show", "Bookmark", new { id = bookmarkId });
        }

        TempData["message"] = "Nu aveti dreptul sa stergeti comentariul.";
        TempData["messageType"] = "alert-danger";
        return RedirectToAction("Show", "Bookmark", new { id = comm.BookmarkId });
    }
    
    [Authorize(Roles = "Admin,User")]
    public IActionResult Edit(int id)
    {
        var category = db.Categories
            .Include(c => c.BookmarkCategories)
            .ThenInclude(bc => bc.Bookmark)
            .FirstOrDefault(c => c.Id == id);

        if (category == null) return NotFound();

        var userId = _userManager.GetUserId(User);
        if (category.UserId != userId && !User.IsInRole("Admin"))
        {
            TempData["message"] = "Nu aveti dreptul sa editati aceasta categorie.";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index");
        }

        if (TempData.ContainsKey("message"))
        {
            ViewBag.Message = TempData["message"];
            ViewBag.Alert = TempData["messageType"];
        }

        return View(category);
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin, User")] 
    public IActionResult Edit(int id, Comment requestComment)
    {
        Comment? comm = db.Comments.Find(id);

        if (comm is null)
        {
            return NotFound();
        }

        if (comm.UserId == _userManager.GetUserId(User)  || User.IsInRole("Admin") )
        {
            if (ModelState.IsValid)
            {
                comm.Content = requestComment.Content;
                db.SaveChanges();

                TempData["message"] = "Comentariul a fost modificat";
                TempData["messageType"] = "alert-success";

                return RedirectToAction("Show", "Bookmark", new { id = comm.BookmarkId });
            }

            // ca sa nu pierzi BookmarkId in view, il pastram
            requestComment.BookmarkId = comm.BookmarkId;
            return View(requestComment);
        }

        TempData["message"] = "Nu aveti dreptul sa editati comentariul";
        TempData["messageType"] = "alert-danger";
        return RedirectToAction("Show", "Bookmark", new { id = comm.BookmarkId });
    }
}