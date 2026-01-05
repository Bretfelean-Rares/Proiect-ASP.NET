using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialBookmarkApp.Models;

public class BookmarkTag
{
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    [Required]
    public int BookmarkId { get; set; }
    [Required]
    public int TagId { get; set; }

    public virtual Bookmark? Bookmark { get; set; }
    public virtual Tag? Tag { get; set; }
}