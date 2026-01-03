using Microsoft.AspNetCore.Identity;

namespace SocialBookmarkApp.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? About { get; set; }
    public string? ProfileImageUrl { get; set; }
}