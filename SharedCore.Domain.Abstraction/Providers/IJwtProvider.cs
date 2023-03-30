using System.Security.Claims;

using SharedCore.Model.Tokens;

namespace SharedCore.Domain.Abstraction.Providers
{
    public interface IJwtProvider
    {
        AccessTokenDto CreateAccessToken(List<Claim>? claims = null);
        RefreshTokenDto CreateRefreshToken();

        bool ValidateAccessToken(AccessTokenDto? accessTokenDto);
        bool CanUseTokens(AccessTokenDto? accessTokenDto, RefreshTokenDto? refreshTokenDto, string? refreshToken);
    }
}
