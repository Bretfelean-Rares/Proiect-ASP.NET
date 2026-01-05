using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialBookmarkApp.Data;
using SocialBookmarkApp.Models;

namespace SocialBookmarkApp.Controllers;

public class CategoryController(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : Controller
{
    private readonly AppDbContext db = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    // GET
    public IActionResult Index()
    {
        var categories = db.Categories
            .Where(c => c.UserId == _userManager.GetUserId(User));

        ViewBag.Categories = categories;
        return View();
    }
    
    public IActionResult New()
    {
        return View();
    }

    [HttpPost]
    public IActionResult New(Category cat)
    {
        cat.UserId = _userManager.GetUserId(User);

        if (ModelState.IsValid)
        {
            db.Categories.Add(cat);
            db.SaveChanges();
            TempData["message"] = "Categoria a fost adăugată";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
        }

        return View(cat);
    }
    
    [Authorize]
    public IActionResult Show(int id)
    {
        var userId = _userManager.GetUserId(User);

        var category = db.Categories
            .Include(c => c.BookmarkCategories)
            .ThenInclude(bc => bc.Bookmark)
            .ThenInclude(b => b.User)
            .FirstOrDefault(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }
        
        if (!category.IsPublic && category.UserId != userId)
        {
            TempData["message"] = "Nu aveti acces la aceasta categorie.";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index", "Home");
        }

        return View(category);
    }
    
    [HttpPost]
    [Authorize]
    public IActionResult Delete(int id)
    {
        var category = db.Categories
            .Include(c => c.BookmarkCategories)
            .FirstOrDefault(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);

        // Doar owner (si optional Admin) poate sterge
        if (category.UserId != userId && !User.IsInRole("Admin"))
        {
            TempData["message"] = "Nu aveti dreptul sa stergeti categoria.";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index", "Home");
        }

        // Stergem intai legaturile din tabelul asociativ
        if (category.BookmarkCategories != null && category.BookmarkCategories.Any())
        {
            db.BookmarkCategories.RemoveRange(category.BookmarkCategories);
        }

        db.Categories.Remove(category);
        db.SaveChanges();

        TempData["message"] = "Categoria a fost stearsa.";
        TempData["messageType"] = "alert-success";

        return RedirectToAction("Index");
    }
}
