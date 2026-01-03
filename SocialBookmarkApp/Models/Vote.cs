using System.ComponentModel.DataAnnotations;

namespace SocialBookmarkApp.Models;

public class Vote
{
    [Key]
    public int Id { get; set; }
    
    public bool IsUpvote { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public int BookmarkId { get; set; }
    public virtual Bookmark Bookmark { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser? User { get; set; }
}