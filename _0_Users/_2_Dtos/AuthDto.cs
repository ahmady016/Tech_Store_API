using Entities;

namespace Auth;

public class AuthDto
{
    public User User { get; set; }
    public TokensResponse Tokens { get; set; }
}
