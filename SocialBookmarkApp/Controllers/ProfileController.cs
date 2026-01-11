using Microsoft.AspNetCore.Authorization;
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

        //bookmarkuri publice
        user.Bookmarks = user.Bookmarks?
            .Where(b => b.IsPublic)
            .OrderByDescending(b => b.CreatedAt)
            .ToList();
        
        var currentId = _userManager.GetUserId(User);
        bool isOwner = currentId == id;

        ViewBag.Categories = _db.Categories
            .Where(c => c.UserId == id && (c.IsPublic || isOwner))
            .OrderByDescending(c=>c.Id)
            .ToList();

        ViewBag.IsOwner = isOwner;
        return View(user);
    }
    
    public IActionResult MyProfile()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
            return RedirectToAction("Login", "Account");

        return RedirectToAction("Show", new { id = userId });
    }
    
    [Authorize]
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        return View(user);
    }
    
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string firstName, string lastName, string about, string profileImageUrl)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        user.FirstName = firstName;
        user.LastName = lastName;
        user.About = about;
        user.ProfileImageUrl = profileImageUrl;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            TempData["message"] = "Nu s-a putut salva profilul.";
            TempData["messageType"] = "alert-danger";
            return View(user);
        }

        TempData["message"] = "Profil actualizat!";
        TempData["messageType"] = "alert-success";
        return RedirectToAction("MyProfile");
    }
}