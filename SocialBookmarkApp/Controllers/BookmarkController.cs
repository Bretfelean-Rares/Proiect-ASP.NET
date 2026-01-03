using Microsoft.AspNetCore.Mvc;

namespace SocialBookmarkApp.Controllers;

public class BookmarkController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}