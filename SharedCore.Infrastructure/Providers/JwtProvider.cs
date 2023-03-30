using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using SharedCore.Domain.Abstraction.Providers;
using SharedCore.Model.Exceptions;
using SharedCore.Model.Options;
using SharedCore.Model.Tokens;

namespace SharedCore.Infrastructure.Providers
{
    public class JwtProvider : IJwtProvider
    {
        private readonly IOptions<JwtOptions> _options;

        public JwtProvider(IOptions<JwtOptions> options)
        {
            _options = options;
        }

        public AccessTokenDto CreateAccessToken(List<Claim>? claims = null)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecurityKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _options.Value.Audience,
                Issuer = _options.Value.Issuer,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow + _options.Value.AccessTokenExpires,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AccessTokenDto
            {
                Token = tokenHandler.WriteToken(token),
                Expires = new DateTimeOffset(tokenDescriptor.Expires.Value),
            };
        }

        public RefreshTokenDto CreateRefreshToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecurityKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow + _options.Value.RefreshTokenExpires,
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new RefreshTokenDto
            {
                Token = tokenHandler.WriteToken(token),
                Expires = new DateTimeOffset(tokenDescriptor.Expires.Value),
            };
        }

        public bool ValidateAccessToken(AccessTokenDto? accessTokenDto)
        {
            var result = accessTokenDto != null && accessTokenDto.Expires > DateTimeOffset.UtcNow;

            return result;
        }

        public bool CanUseTokens(AccessTokenDto? accessTokenDto, RefreshTokenDto? refreshTokenDto, string? refreshToken)
        {
            if (string.IsNullOrWhiteSpace(accessTokenDto?.Token))
            {
                throw new NotFoundAppException("Access token was not found.");
            }

            if (string.IsNullOrWhiteSpace(refreshTokenDto?.Token))
            {
                throw new NotFoundAppException("Refresh token was not found.");
            }

            if (!string.Equals(refreshTokenDto.Token, refreshToken))
            {
                throw new UnauthorizedAppException("Refresh tokens does not match.");
            }

            if (accessTokenDto.Expires > DateTimeOffset.UtcNow)
            {
                return true;
            }

            if (refreshTokenDto.Expires < DateTimeOffset.UtcNow)
            {
                throw new UnauthorizedAppException("Refresh token has expired.");
            }

            return false;
        }
    }
}
