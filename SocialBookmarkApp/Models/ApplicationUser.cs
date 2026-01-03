using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SocialBookmarkApp.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? About { get; set; }
    public string? ProfileImageUrl { get; set; }

    public virtual ICollection<Bookmark> Bookmarks { get; set; }
        = [];
    public virtual ICollection<Comment> Comments { get; set; } = [];
    public virtual ICollection<Vote> Votes { get; set; } = [];
    public virtual ICollection<Category> Categories { get; set; } = [];

}