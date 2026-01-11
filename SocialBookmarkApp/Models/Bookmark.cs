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
    
    [StringLength(1000, ErrorMessage="Descrierea nu poate avea mai mult de 1000 de caractere")]
    public string? Description { get; set; }
    
    public string? MediaContent { get; set; }

    public bool IsPublic { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser? User { get; set; }
    
    
    public virtual ICollection<Comment>? Comments { get; set; }
    public virtual ICollection<BookmarkCategory>? BookmarkCategories { get; set; }
    
    public virtual ICollection<BookmarkTag>? BookmarkTags { get; set; }
    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();

    [NotMapped]
    public int VotesCount => Votes.Count;


}