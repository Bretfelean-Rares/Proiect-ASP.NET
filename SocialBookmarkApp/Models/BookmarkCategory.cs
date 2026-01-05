using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialBookmarkApp.Models;

public class BookmarkCategory
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public int BookmarkId { get; set; }
    [Required]
    public int CategoryId { get; set; }

    public virtual Bookmark? Bookmark { get; set; }
    public virtual Category? Category { get; set; }
}