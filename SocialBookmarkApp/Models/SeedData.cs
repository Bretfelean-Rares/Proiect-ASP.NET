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
                    NormalizedEmail = "USER@TEST.COM", 
                    Email = "user@test.com", 
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
        }
    }
}