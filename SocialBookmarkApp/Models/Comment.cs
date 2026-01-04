using System.ComponentModel.DataAnnotations;

namespace SocialBookmarkApp.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }
    
    [Required, StringLength(1500)]
    public string Content { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int BookmarkId { get; set; }
    public virtual Bookmark? Bookmark { get; set; } = null!;
    
    
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser? User { get; set; }
    
}