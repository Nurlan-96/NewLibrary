using Microsoft.AspNetCore.Identity;

namespace NewLibrary.Core.Entities
{
    public class AppUser:IdentityUser
    {
        public string FullName {  get; set; }

    }
}
