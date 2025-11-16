using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace MvcMusicStore.Models
{
    // This context is ONLY for Identity - separate from MusicStoreEntities
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("IdentityConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}