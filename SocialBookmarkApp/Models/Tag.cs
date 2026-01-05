using System.ComponentModel.DataAnnotations;

namespace SocialBookmarkApp.Models;

public class Tag
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<BookmarkTag>? BookmarkTags { get; set; }
}