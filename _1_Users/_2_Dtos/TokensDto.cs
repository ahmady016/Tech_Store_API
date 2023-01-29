using TechStoreApi.Auth;

namespace TechStoreApi.Dtos;

public class TokensDto
{
    public TokenDto AccessToken { get; set; }
    public TokenDto RefreshToken { get; set; }
}
