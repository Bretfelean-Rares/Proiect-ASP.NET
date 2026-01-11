using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialBookmarkApp.Data;

namespace SocialBookmarkApp.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider
        serviceProvider)
    {
        using (var context = new AppDbContext(
                   serviceProvider.GetRequiredService
                       <DbContextOptions<AppDbContext>>()))
        {
// Verificam daca in baza de date exista cel putin un rol  insemnand ca a fost rulat codul
// De aceea facem return pentru a nu insera rolurile inca o data  Acesta metoda trebuie sa se execute o singura data
            if (context.Roles.Any())
            {
                return; // baza de date contine deja roluri
            }
            // CREAREA ROLURILOR IN BD   daca nu contine roluri, acestea se vor crea
            context.Roles.AddRange(

                new IdentityRole { Id = "b0736292-2838-4097-985b-894820c3d901", Name = "Admin", NormalizedName = "Admin".ToUpper() },

                    new IdentityRole { Id = "b0736292-2838-4097-985b-894820c3d902", Name = "User", NormalizedName = "User".ToUpper() }

                    );

// o noua instanta pe care o vom utiliza pentru crearea parolelor utilizatorilor
// parolele sunt de tip hash
                    var hasher = new PasswordHasher<ApplicationUser>();

// CREAREA USERILOR IN BD
// Se creeaza cate un user pentru fiecare rol
                    context.Users.AddRange(
                    new ApplicationUser

                    {

                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb0",
// primary key
                    UserName = "admin@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@TEST.COM",
                    Email = "admin@test.com",
                    NormalizedUserName = "ADMIN@TEST.COM",
                    PasswordHash = hasher.HashPassword(null,

                        "Admin1!")
                },
                    
                new ApplicationUser

                {

                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb1",
// primary key
                    UserName = "user@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "USER@TEST.COM",
                    Email = "user@test.com",
                    NormalizedUserName = "USER@TEST.COM",
                    PasswordHash = hasher.HashPassword(null,

                        "User1!")
                },
                    
                new ApplicationUser

                {

                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb2",
// primary key
                    UserName = "user2@test.com",
                    EmailConfirmed = true, 
                    NormalizedEmail = "USER2@TEST.COM", 
                    Email = "user2@test.com", 
                    NormalizedUserName = "USER2@TEST.COM", 
                    PasswordHash = hasher.HashPassword(null,

                                "User2!")
                        }
                    );

// ASOCIEREA USER-ROLE
                    context.UserRoles.AddRange(
                        new IdentityUserRole<string>
                        {

                            RoleId = "b0736292-2838-4097-985b-894820c3d901",

                            UserId = "8e445865-a24d-4543-a6c6-9443d048cdb0"
                        },

                        new IdentityUserRole<string>

                        {

                            RoleId = "b0736292-2838-4097-985b-894820c3d902",

                            UserId = "8e445865-a24d-4543-a6c6-9443d048cdb1"
                        },

                        new IdentityUserRole<string>

                        {

                            RoleId = "b0736292-2838-4097-985b-894820c3d902",

                            UserId = "8e445865-a24d-4543-a6c6-9443d048cdb2"
                        }
                    );
    context.SaveChanges();
    var admin = context.Users.FirstOrDefault(u => u.Email == "admin@test.com");
    var user1 = context.Users.FirstOrDefault(u => u.Email == "user@test.com");
    var user2 = context.Users.FirstOrDefault(u => u.Email == "user2@test.com");

    if (admin == null || user1 == null || user2 == null)
        return;
    admin.FirstName = "Admin";
    admin.LastName = "Boss";
    admin.About = "Curator de playlist-uri. Testează tot.";
    admin.ProfileImageUrl = "https://media.istockphoto.com/id/2134890875/video/hell-animation-on-green-screen-emotion-character-4k-video.jpg?s=640x640&k=20&c=PDqg9fu9aSrKfqEXyTeG0rclfpCpBEhfFGA0I7Av_pw=";

    user1.FirstName = "Mara";
    user1.LastName = "Pop";
    user1.About = "";
    user1.ProfileImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTOHljug4PmU0UVPrp4Rkdn72prvTn_kkAQIg&s";

    user2.FirstName = "Radu";
    user2.LastName = "Ionescu";
    user2.About = "🍷🎷❤️";
    user2.ProfileImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/e0/Profile_Picture_for_youtube.gif/250px-Profile_Picture_for_youtube.gif";
    
    var c1 = new Category { CategoryName = "Rock Classics", IsPublic = true,  UserId = user1.Id };
    var c2 = new Category { CategoryName = "Live Sessions", IsPublic = true,  UserId = user1.Id };
    var c3 = new Category { CategoryName = "Synthwave",     IsPublic = true,  UserId = user2.Id };
    var c4 = new Category { CategoryName = "Production",IsPublic = false, UserId = admin.Id };
    var c5 = new Category { CategoryName = "Discover Weekly",IsPublic = true, UserId = admin.Id };

    context.Categories.AddRange(c1, c2, c3, c4, c5);
    context.SaveChanges(); 
    
    var tRock = new Tag { Name = "rock" };
    var tIndie = new Tag { Name = "indie" };
    var tLive = new Tag { Name = "live" };
    var tSynth = new Tag { Name = "synthwave" };
    var tProd = new Tag { Name = "production" };
    var tVideo = new Tag { Name = "video" };
    var tPhoto = new Tag { Name = "photo" };
    
    context.Tags.AddRange(tRock, tIndie, tLive, tSynth, tProd, tVideo, tPhoto);
    context.SaveChanges();
    
    var b1 = new Bookmark {
        Title = "Nirvana – Smells Like Teen Spirit",
        Description = "Clip oficial – grunge classic.",
        MediaContent = @"<iframe width=""100%"" height=""250"" src=""https://www.youtube.com/embed/hTWKbfoikeg"" frameborder=""0"" allowfullscreen></iframe>",
        IsPublic = true, CreatedAt = DateTime.Now.AddDays(-8), UserId = user1.Id
    };
    
    var b2 = new Bookmark {
        Title = "Daft Punk – Random Access Memories (art)",
        Description = "Imagine promo",
        MediaContent = "https://weraveyou.com/wp-content/uploads/2023/03/Daft-Punk-Random-Access-Memories-scaled.jpg",
        IsPublic = true, CreatedAt = DateTime.Now.AddDays(-7), UserId = user2.Id
    };
    
    
    var b3 = new Bookmark {
        Title = "Tiny Desk Concerts",
        Description = "sesiuni live foarte bune.",
        MediaContent = "<iframe width=\"1062\" height=\"597\" src=\"https://www.youtube.com/embed/ferZnZ0_rSM?list=RDferZnZ0_rSM\" title=\"Anderson .Paak &amp; The Free Nationals: NPR Music Tiny Desk Concert\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share\" referrerpolicy=\"strict-origin-when-cross-origin\" allowfullscreen></iframe>",
        IsPublic = true, CreatedAt = DateTime.Now.AddDays(-6), UserId = admin.Id
    };
    
    var b4 = new Bookmark {
        Title = "Jazz!!",
        Description = "Pentru invatat noaptea.",
        MediaContent = "<iframe width=\"1062\" height=\"663\" src=\"https://www.youtube.com/embed/3Q18F--zZ-I?list=RD3Q18F--zZ-I\" title=\"My Little Brown Book\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share\" referrerpolicy=\"strict-origin-when-cross-origin\" allowfullscreen></iframe>",
        IsPublic = true, CreatedAt = DateTime.Now.AddDays(-5), UserId = user2.Id
    };
    
    var b5 = new Bookmark {
        Title = "Concert live",
        Description = "",
        MediaContent = "<iframe width=\"1062\" height=\"597\" src=\"https://www.youtube.com/embed/8Pa9x9fZBtY?list=RD8Pa9x9fZBtY\" title=\"Dire Straits - Sultans Of Swing (Alchemy Live)\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share\" referrerpolicy=\"strict-origin-when-cross-origin\" allowfullscreen></iframe>",
        IsPublic = false, CreatedAt = DateTime.Now.AddDays(-4), UserId = admin.Id
    };
    
    var b6 = new Bookmark {
        Title = "Radiohead – Weird Fishes",
        Description = "Indie favorite.",
        MediaContent = "<iframe width=\"1062\" height=\"597\" src=\"https://www.youtube.com/embed/pcEJyvv6_kc?list=RDpcEJyvv6_kc\" title=\"Weird Fishes/Arpeggi | Radiohead | From The Basement\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share\" referrerpolicy=\"strict-origin-when-cross-origin\" allowfullscreen></iframe>",
        IsPublic = true, CreatedAt = DateTime.Now.AddDays(-3), UserId = user1.Id
    };
    
    var b7 = new Bookmark {
        Title = "Boiler Room sets",
        Description = "Live DJ sets, energie maximă.",
        MediaContent = "<iframe width=\"1062\" height=\"597\" src=\"https://www.youtube.com/embed/-5EQIiabJvk?list=RD-5EQIiabJvk\" title=\"Kaytranada | Boiler Room: Montreal\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share\" referrerpolicy=\"strict-origin-when-cross-origin\" allowfullscreen></iframe>",
        IsPublic = true, CreatedAt = DateTime.Now.AddDays(-2), UserId = user2.Id
    };
    
    var b8 = new Bookmark {
        Title = "Album cover inspiration",
        Description = "Colecție de cover art.",
        MediaContent = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTzyBQmXl9zYbdi-Ar-w0Wiev5WBWI5wriQ2w&s",
        IsPublic = true, CreatedAt = DateTime.Now.AddDays(-1), UserId = admin.Id
    };
    
    context.Bookmarks.AddRange(b1,b2,b3,b4,b5,b6,b7,b8);
    context.SaveChanges();
    
    void LinkTag(Bookmark b, Tag t) => context.BookmarkTags.Add(new BookmarkTag { BookmarkId = b.Id, TagId = t.Id });

    LinkTag(b1, tRock); LinkTag(b1, tVideo);
    LinkTag(b2, tSynth); LinkTag(b2, tProd);
    LinkTag(b3, tLive); LinkTag(b3, tVideo);
    LinkTag(b4, tLive);
    LinkTag(b5, tLive);
    LinkTag(b6, tIndie); LinkTag(b6, tVideo);
    LinkTag(b7, tLive); LinkTag(b7, tVideo);
    LinkTag(b8, tPhoto);

    
    void LinkCat(Bookmark b, Category c) => context.BookmarkCategories.Add(new BookmarkCategory { BookmarkId = b.Id, CategoryId = c.Id });
    
    LinkCat(b1, c1); 
    LinkCat(b6, c1);
    LinkCat(b3, c2); 
    LinkCat(b7, c2);
    LinkCat(b2, c3);
    LinkCat(b5, c4); 
    LinkCat(b8, c5); 


    context.SaveChanges();
        }
    }
    
}