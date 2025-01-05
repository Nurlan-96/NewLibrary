using NewLibrary.Core.Entities;
using System.Security.Claims;

namespace NewLibrary.Infrastructure.Identity
{
    public interface IClaimsManager 
    {
        int GetCurrentUserId();
        string GetCurrentUserName();
        IEnumerable<Claim> GetUserClaims(AppUser user);
        Claim GetUserClaim(string claimType);
    }
}