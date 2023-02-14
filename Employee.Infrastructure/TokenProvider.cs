using Application.Repositories;

namespace Infrastructure;

public class TokenProvider : ITokenProvider
{
    private const string Token = "0bf7fb56e6a27cbcadc402fc2fce8e3aa9ac2b40d4190698eb4e8df9284e2023";
    public string GetToken()
    {
        return Token;
    }
}
