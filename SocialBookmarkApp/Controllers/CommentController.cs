using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize] 
    public IActionResult Delete(int id)
    {
        Comment? comm = db.Comments.Find(id);

        if (comm == null)
        {
            return NotFound();
        }

        if (comm.UserId == _userManager.GetUserId(User))
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
    
    [Authorize] 
    public IActionResult Edit(int id)
    {
        Comment? comm = db.Comments.Find(id);

        if (comm is null)
        {
            return NotFound();
        }

        if (comm.UserId == _userManager.GetUserId(User) /* || User.IsInRole("Admin") */)
        {
            return View(comm);
        }

        TempData["message"] = "Nu aveti dreptul sa editati comentariul";
        TempData["messageType"] = "alert-danger";
        return RedirectToAction("Show", "Bookmark", new { id = comm.BookmarkId });
    }
    [HttpPost]
    [Authorize] // cand adaugi roluri: [Authorize(Roles = "User,Admin")]
    public IActionResult Edit(int id, Comment requestComment)
    {
        Comment? comm = db.Comments.Find(id);

        if (comm is null)
        {
            return NotFound();
        }

        if (comm.UserId == _userManager.GetUserId(User) /* || User.IsInRole("Admin") */)
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