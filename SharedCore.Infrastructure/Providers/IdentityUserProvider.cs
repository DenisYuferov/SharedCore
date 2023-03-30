using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;

using SharedCore.Domain.Abstraction.Providers;

namespace SharedCore.Infrastructure.Providers
{
    public class IdentityUserProvider : IIdentityUserProvider
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IdentityUserProvider(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
        public async Task<List<Claim>> GetClaimsAsync(IdentityUser user)
        {
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
    }
}
