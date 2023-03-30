using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

namespace SharedCore.Domain.Abstraction.Providers
{
    public interface IIdentityUserProvider
    {
        Task<List<Claim>> GetClaimsAsync(IdentityUser identityUser);
    }
}
