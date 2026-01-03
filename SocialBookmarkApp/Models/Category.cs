using System.ComponentModel.DataAnnotations;

namespace SocialBookmarkApp.Models;

public class Category
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Numele categoriei este obligatoriu")]
    public string CategoryName { get; set; }
    
    public bool IsPublic { get; set; } = false;
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser? User { get; set; }
    
    public virtual ICollection<BookmarkCategory>? BookmarkCategories { get; set; }


}