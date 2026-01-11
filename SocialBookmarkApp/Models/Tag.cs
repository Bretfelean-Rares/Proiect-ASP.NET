using System.ComponentModel.DataAnnotations;

namespace SocialBookmarkApp.Models;

public class Tag
{
    [Key]
    public int Id { get; set; }

    [StringLength(50, ErrorMessage = "Numele tagului nu poate avea mai mult de 50 de caractere.")]
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<BookmarkTag>? BookmarkTags { get; set; }
}