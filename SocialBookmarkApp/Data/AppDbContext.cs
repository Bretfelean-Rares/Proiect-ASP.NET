using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialBookmarkApp.Models;

namespace SocialBookmarkApp.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>

{
    public AppDbContext(DbContextOptions<AppDbContext>
        options)
        : base(options)
    {
    }
    
    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Vote> Votes => Set<Vote>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<BookmarkCategory> BookmarkCategories => Set<BookmarkCategory>();
    public DbSet<Tag> Tags { get; set; }
    public DbSet<BookmarkTag> BookmarkTags { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<BookmarkTag>()
            .HasKey(bt => new { bt.id, bt.BookmarkId, bt.TagId });

        modelBuilder.Entity<BookmarkTag>()
            .HasOne(bt => bt.Bookmark)
            .WithMany(b => b.BookmarkTags)
            .HasForeignKey(bt => bt.BookmarkId);

        modelBuilder.Entity<BookmarkTag>()
            .HasOne(bt => bt.Tag)
            .WithMany(t => t.BookmarkTags)
            .HasForeignKey(bt => bt.TagId);
        
        modelBuilder.Entity<BookmarkCategory>()
            .HasKey(bc => new { bc.Id, bc.BookmarkId, bc.CategoryId });

        modelBuilder.Entity<BookmarkCategory>()
            .HasOne(bc => bc.Bookmark)
            .WithMany(b => b.BookmarkCategories)
            .HasForeignKey(bc => bc.BookmarkId);

        modelBuilder.Entity<BookmarkCategory>()
            .HasOne(bc => bc.Category)
            .WithMany(c => c.BookmarkCategories)
            .HasForeignKey(bc => bc.CategoryId);
        
        modelBuilder.Entity<Comment>()
            .HasOne<Bookmark>(c => c.Bookmark)
            .WithMany(b=>b.Comments)
            .HasForeignKey(c => c.BookmarkId);
        
        modelBuilder.Entity<Vote>()
            .HasIndex(v => new { v.BookmarkId, v.UserId })
            .IsUnique();
       
    }


}