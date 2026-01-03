using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialBookmarkApp.Models;

public class Bookmark
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Titlul este obligatoriu")]
    [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 de caractere")]
    [MinLength(5, ErrorMessage = "Titlul trebuie sa aiba mai mult de 5 caractere")]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string? Description { get; set; }
    
    public string? MediaContent { get; set; }

    public bool IsPublic { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public string? AuthorId { get; set; } = string.Empty;
    public virtual ApplicationUser? User { get; set; }

    [ForeignKey(nameof(AuthorId))]
    public ApplicationUser? Author { get; set; }
    
    public virtual ICollection<Comment>? Comments { get; set; }
    public virtual ICollection<BookmarkCategory>? BookmarkCategories { get; set; }


}